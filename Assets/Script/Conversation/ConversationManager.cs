using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class ConversationManager : MonoSingleton<ConversationManager>
{
    public enum ConversationState
    {
        SpaceProgress,
        AutoProgress,
    }

    public class CoroutineInterruptToken
    {

        /// <summary>
        /// The state that the token is in.
        /// </summary>
        enum State
        {
            NotRunning,
            Running,
            Interrupted,
        }
        private State state = State.NotRunning;

        public bool CanInterrupt => state == State.Running;
        public bool WasInterrupted => state == State.Interrupted;
        public void Start() => state = State.Running;
        public void Interrupt()
        {
            if (CanInterrupt == false)
            {
                //throw new InvalidOperationException($"Cannot stop {nameof(CoroutineInterruptToken)}; state is {state} (and not {nameof(State.Running)}");
            }
            state = State.Interrupted;
        }

        public void Complete() => state = State.NotRunning;
    }


    [SerializeField] ConversationState currentConversationState = ConversationState.SpaceProgress;
    public DialogueParser.Dialogue currentDialogue;
    int currentDialogueLineIndex = 0;

    [Header("UI Reference")]
    [SerializeField] TextMeshProUGUI UpCharacterTM;
    [SerializeField] TextMeshProUGUI DownCharacterTM;
    public RectTransform UpperDialogueGameObject;
    public RectTransform DownDialogueGameObject;

    [Header("Jitter")]
    public float jitterAmount = 5f; // Adjust the amount of jitter as needed
    public float jitterSpeed = 5f;
    private Vector3 originalPosition_Up;
    private Vector3 originalPosition_Down;
    public bool bjitterUpDialogueGameObject;
    public bool bjitterDownDialogueGameObject;

    [Header("TypeWriter")]
    [SerializeField] float typewriterEffectSpeed = 20f;
    CoroutineInterruptToken currentStopToken = new CoroutineInterruptToken();
    public Text lineText = null;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition_Up = UpperDialogueGameObject.anchoredPosition3D;
        originalPosition_Down = DownDialogueGameObject.anchoredPosition3D;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentConversationState == ConversationState.SpaceProgress)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LoadNextLineInCurrentDialogue();
                Debug.Log("Load Next Dialogue");
            }
        }

        if (bjitterUpDialogueGameObject)
        {
            float xOffset = Random.Range(-jitterAmount, jitterAmount);
            float yOffset = Random.Range(-jitterAmount, jitterAmount);
            Vector3 jitteredPosition = originalPosition_Up + new Vector3(xOffset, yOffset, 0f);

            // Smoothly move the UI element towards the new position
            UpperDialogueGameObject.anchoredPosition3D = Vector3.Lerp(UpperDialogueGameObject.anchoredPosition3D, jitteredPosition, Time.deltaTime * jitterSpeed);
        }

        if (bjitterDownDialogueGameObject)
        {
            float xOffset = Random.Range(-jitterAmount, jitterAmount);
            float yOffset = Random.Range(-jitterAmount, jitterAmount);
            Vector3 jitteredPosition = originalPosition_Down + new Vector3(xOffset, yOffset, 0f);

            // Smoothly move the UI element towards the new position
            DownDialogueGameObject.anchoredPosition3D = Vector3.Lerp(DownDialogueGameObject.anchoredPosition3D, jitteredPosition, Time.deltaTime * jitterSpeed);
        }
    }

    void LoadNextLineInCurrentDialogue()
    {
        currentDialogueLineIndex++;
        if (currentDialogueLineIndex >= currentDialogue.lines.Count)
        {
            Debug.Log("Reach end of the dialogue");
            return;
        }

        DialogueParser.Line currentLine = currentDialogue.lines[currentDialogueLineIndex];
        
        if (currentLine.text[0] == '[')
        {
            string eventName = currentLine.text.Substring(1, currentLine.text.Length - 3);
            TriggerEvent(eventName);
            return;
        }

        switch (currentLine.character)
        {
            case DialogueParser.Character.Up:
                UpCharacterTM.text = currentLine.text;
                StartCoroutine(Typewriter
                        (UpCharacterTM,
                        typewriterEffectSpeed,
                        currentStopToken
                    ));
                break;
            case DialogueParser.Character.Down:
                DownCharacterTM.text = currentLine.text;
                StartCoroutine(Typewriter
                       (DownCharacterTM,
                       typewriterEffectSpeed,
                       currentStopToken
                   ));
                break;
            case DialogueParser.Character.Both:
                UpCharacterTM.text = currentLine.text;
                DownCharacterTM.text = currentLine.text;

                StartCoroutine(Typewriter
                       (DownCharacterTM,
                       typewriterEffectSpeed,
                       currentStopToken
                   ));
                StartCoroutine(Typewriter
                   (UpCharacterTM,
                   typewriterEffectSpeed,
                   currentStopToken
               ));
                break;

        }

        if (currentStopToken.WasInterrupted)
        {
                    // The typewriter effect was interrupted. Stop this
                    // entire coroutine.
                 //yield break;
        }
    }

    public static IEnumerator Typewriter(TextMeshProUGUI text, float lettersPerSecond,  CoroutineInterruptToken stopToken = null)
    {
        stopToken?.Start();

        // Start with everything invisible
     
        text.maxVisibleCharacters = 0;

        // Wait a single frame to let the text component process its
        // content, otherwise text.textInfo.characterCount won't be
        // accurate
        yield return null;

        // How many visible characters are present in the text?
        var characterCount = text.textInfo.characterCount;

        // Early out if letter speed is zero, text length is zero
        if (lettersPerSecond <= 0 || characterCount == 0)
        {
            // Show everything and return
            text.maxVisibleCharacters = characterCount;
            stopToken?.Complete();
            yield break;
        }

        // Convert 'letters per second' into its inverse
        float secondsPerLetter = 1.0f / lettersPerSecond;

        // If lettersPerSecond is larger than the average framerate, we
        // need to show more than one letter per frame, so simply
        // adding 1 letter every secondsPerLetter won't be good enough
        // (we'd cap out at 1 letter per frame, which could be slower
        // than the user requested.)
        //
        // Instead, we'll accumulate time every frame, and display as
        // many letters in that frame as we need to in order to achieve
        // the requested speed.
        var accumulator = Time.deltaTime;

        while (text.maxVisibleCharacters < characterCount)
        {
            if (stopToken?.WasInterrupted ?? false)
            {
                yield break;
            }

            // We need to show as many letters as we have accumulated
            // time for.
            while (accumulator >= secondsPerLetter)
            {
                text.maxVisibleCharacters += 1;
                accumulator -= secondsPerLetter;
            }
            accumulator += Time.deltaTime;

            yield return null;
        }

        // We either finished displaying everything, or were
        // interrupted. Either way, display everything now.
        text.maxVisibleCharacters = characterCount;

        stopToken?.Complete();
    }


    public void TriggerEvent(string eventName)
    {
        switch (eventName)
        {
            case "StartGlitch":
                StartGlitch();
                return;
            case "StopGlitch":
                StopGlitch();
                return;
            case "StartDialogueBoxJitter_Up":
                StartDialogueBoxJitter_Up();
                return;
            case "StopDialogueBoxJitter_Up":
                StopDialogueBoxJitter_Up();
                return;
            case "StartDialogueBoxJitter_Down":
                StartDialogueBoxJitter_Down();
                return;
            case "StopDialogueBoxJitter_Down":
                StopDialogueBoxJitter_Down();
                return;
            case "StartRain":
                StartRain();
                return;
            case "StopRain":
                StopRain();
                return;
            case "StartLSD":
                StartLSD();
                return;
            case "StopLSD":
                StopLSD();
                return;
            case "SwitchLanguage":
                SwitchLanguage();
                return;
        }

        Debug.Log("Error Command " + eventName);
    }

    public void FadeToDark()
    { 
    
    }

    public void StartGlitch()
    {
        Debug.Log("Start Glitch");
    }

    public void StopGlitch()
    {
        Debug.Log("Stop Glitch");
    }

    public void StartDialogueBoxJitter_Up()
    {
        Debug.Log("Start Dialogue Box Jitter");
        bjitterUpDialogueGameObject = true;
    }

    public void StopDialogueBoxJitter_Up()
    {
        Debug.Log("Stop Dialogue Box Jitter");
        bjitterUpDialogueGameObject = false;
    }

    public void StartDialogueBoxJitter_Down()
    {
        Debug.Log("Start Dialogue Box Jitter");
        bjitterDownDialogueGameObject = true;
    }

    public void StopDialogueBoxJitter_Down()
    {
        Debug.Log("Stop Dialogue Box Jitter");
        bjitterDownDialogueGameObject = false;
    }

    public void StartRain()
    {
        Debug.Log("Start Rain");
    }

    public void StopRain()
    {
        Debug.Log("Stop Rain");
    }

    public void StartLSD()
    {
        Debug.Log("Start LSD");
    }

    public void StopLSD()
    {
        Debug.Log("Stop LSD");
    }

    public void SwitchLanguage()
    {
        Debug.Log("Switch Language");
    }

}
