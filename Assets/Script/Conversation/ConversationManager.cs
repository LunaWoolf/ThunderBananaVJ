using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;
using LimitlessDev.RetroLookPro;
using DigitalRuby.RainMaker;
using UnityEngine.SceneManagement;

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
    public GameObject UpperDialogueBox;
    public GameObject DownDialogueBox;
    public RectTransform UpperDialogueGameObject;
    public RectTransform DownDialogueGameObject;
    public UnityEngine.UI.Image OverlayImage;
    public UnityEngine.UI.Image OverlayBackImage;
    public GameObject DigitalRain;
    public FrameAnimation headShake;

    [Header("Jitter")]
    public float jitterAmount = 5f; // Adjust the amount of jitter as needed
    public float jitterSpeed = 5f;
    private Vector3 originalPosition_Up;
    private Vector3 originalPosition_Down;
    public bool bjitterUpDialogueGameObject;
    public bool bjitterDownDialogueGameObject;

    [Header("Glitch")]
    bool isGlitchTextCutOff = false;
    int StartGlitchIndex = 0;
    int GlitchCutOffIndex = 1;

    [Header("TypeWriter")]
    [SerializeField] float typewriterEffectSpeed = 20f;
    CoroutineInterruptToken currentStopToken = new CoroutineInterruptToken();
    public Text lineText = null;
    public float autoLoadConversationCoolDown = 0.1f;

    [Header("Rain")]
    [SerializeField] GameObject RainPerfab;
    [SerializeField] GameObject videoPlayer_Rain;
    bool isRaining = false;

    [Header("LSD")]
    bool isLSD = false;
    [SerializeField] GameObject LSDGameObject;

    [Header("VideoReference")]
    [SerializeField] RawImage videoRawImageRef;
    [SerializeField] RenderTexture DefaultTexture;
    [SerializeField] RenderTexture RainTexture;
    [SerializeField] RenderTexture LSDTexture;

    [Header("End")]
    [SerializeField] PlayableAsset EndTimeline;
    [SerializeField] PlayableAsset SwitchTimeline;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 25;
        originalPosition_Up = UpperDialogueGameObject.anchoredPosition3D;
        originalPosition_Down = DownDialogueGameObject.anchoredPosition3D;
        DownCharacterTM.text = "   ";
        UpCharacterTM.text = "   ";
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
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGlitchTextCutOff == true)
                    StopGlitch();
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

        //Debug
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isRaining) StopRain();
            else StartRain();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (isLSD) StopLSD();
            else StartLSD();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SwitchLanguage();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            End();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
            Destroy(this.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (headShake.isStopped)
            {
                headShake.Restart();
            }
            else
            {
                headShake.Stop();
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            NegativePostProcessing.SetActive(!NegativePostProcessing.activeSelf);
       
           
        }


        if (Input.GetKeyDown(KeyCode.Alpha1)) { if (currentDialogue.songLineIndex.Count > 0) currentDialogueLineIndex = currentDialogue.songLineIndex[0]; EndOfSong(); EnableColormapPalette(true); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { if (currentDialogue.songLineIndex.Count > 1) currentDialogueLineIndex = currentDialogue.songLineIndex[1]; EndOfSong(); EnableColormapPalette(true); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { if (currentDialogue.songLineIndex.Count > 2) currentDialogueLineIndex = currentDialogue.songLineIndex[2]; EndOfSong(); EnableColormapPalette(true); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { if (currentDialogue.songLineIndex.Count > 3) currentDialogueLineIndex = currentDialogue.songLineIndex[3]; EndOfSong(); EnableColormapPalette(true); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { if (currentDialogue.songLineIndex.Count > 4) currentDialogueLineIndex = currentDialogue.songLineIndex[4]; EndOfSong(); EnableColormapPalette(true); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { if (currentDialogue.songLineIndex.Count > 5) currentDialogueLineIndex = currentDialogue.songLineIndex[5]; EndOfSong(); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { if (currentDialogue.songLineIndex.Count > 6) currentDialogueLineIndex = currentDialogue.songLineIndex[6]; EndOfSong(); }
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
                UpperDialogueBox.SetActive(true);
              
                break;
            case DialogueParser.Character.Down:
                DownCharacterTM.text = currentLine.text;
                StartCoroutine(Typewriter
                       (DownCharacterTM,
                       typewriterEffectSpeed,
                       currentStopToken
                   ));
                DownDialogueBox.SetActive(true);
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
                UpperDialogueBox.SetActive(true);
                DownDialogueBox.SetActive(true);
                break;

        }

        if (currentStopToken.WasInterrupted)
        {
                    // The typewriter effect was interrupted. Stop this
                    // entire coroutine.
                 //yield break;
        }
    }

    public IEnumerator Typewriter(TextMeshProUGUI text, float lettersPerSecond,  CoroutineInterruptToken stopToken = null)
    {
        stopToken?.Start();

        // Start with everything invisible
     
        text.maxVisibleCharacters = 0;

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

        if (currentConversationState == ConversationState.AutoProgress)
        {
            yield return new WaitForSeconds(autoLoadConversationCoolDown);
            LoadNextLineInCurrentDialogue();
        }
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
                EndGlitchText();
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
            case "EndOfSong":
                EndOfSong();
                return;
            case "FadeToBlack":
                FadeBlack();
                return;
            case "ChangeToRed":
                ChangeToRed();
                return;
            case "END":
                End();
                return;
            case "EN":
                End();
                return;
            case "NegativeOn":
                EnableNegative(true);
                LoadNextLineInCurrentDialogue();
                return;
            case "NegativeOff":
                EnableNegative(false);
                LoadNextLineInCurrentDialogue();
                return;


        }

        Debug.Log("Error Command " + eventName);
        LoadNextLineInCurrentDialogue();
    }

   

    public void StartGlitch()
    {
        currentConversationState = ConversationState.AutoProgress;
        StartGlitchIndex = currentDialogueLineIndex;
        LoadNextLineInCurrentDialogue();
        Debug.Log("Start Glitch");
    }

    public void StopGlitch()
    {
        currentDialogueLineIndex = GlitchCutOffIndex;
        isGlitchTextCutOff = false;
        currentConversationState = ConversationState.SpaceProgress;
        Debug.Log("Stop Glitch");
    }
    public void EndGlitchText()
    {
        isGlitchTextCutOff = true;
        GlitchCutOffIndex = currentDialogueLineIndex;
        currentDialogueLineIndex = StartGlitchIndex;
        LoadNextLineInCurrentDialogue();
        // currentConversationState = ConversationState.SpaceProgress;
        Debug.Log("GlitchText Cutoff");
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
        UpperDialogueGameObject.anchoredPosition3D = originalPosition_Up;
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
        DownDialogueGameObject.anchoredPosition3D = originalPosition_Down;
    }

    public void StartRain()
    {
        Debug.Log("Start Rain");
        isRaining = true;
        //if(!RainPerfab.activeSelf) RainPerfab.SetActive(true);
        //RainPerfab.GetComponent<RainScript>().RePlay();
        videoRawImageRef.texture = RainTexture;
        videoRawImageRef.color = new Color(0.3915094f, 0.4188518f, 1, .5f);
        //cover the character
        OverlayImage.color = new Color(0f, 0.2f, 0.8f, 0.3f);
        videoPlayer_Rain.SetActive(true);
    }

    public void StopRain()
    {
        Debug.Log("Stop Rain");
        isRaining = false;
       // RainPerfab.SetActive(false);
       // RainPerfab.GetComponent<RainScript>().Pause();

        videoRawImageRef.texture = DefaultTexture;
        videoRawImageRef.color = new Color(1f,1f, 1f, 1f);
        OverlayImage.color = new Color(0f, 0.2f, 0.8f, 0f);
        videoPlayer_Rain.SetActive(false);
    }

    public void StartLSD()
    {
        Debug.Log("Start LSD");
        EnableColormapPalette(false);
        isLSD = true;
        DigitalRain.SetActive(false);
        LSDGameObject.SetActive(true);
        videoRawImageRef.texture = LSDTexture;
    }

    public void StopLSD()
    {
        Debug.Log("Stop LSD");
        EnableColormapPalette(true);
        isLSD = false;
        DigitalRain.SetActive(true);
        LSDGameObject.SetActive(false);
        videoRawImageRef.texture = DefaultTexture;
        
    }

    public void SwitchLanguage()
    {
        Debug.Log("Switch Language");
        StartLSD();
        this.GetComponent<PlayableDirector>().playableAsset = SwitchTimeline;
        this.GetComponent<PlayableDirector>().enabled = true;
        this.GetComponent<PlayableDirector>().Play();
     
    }

    [SerializeField] GameObject PixelPostProcessing;


    public void EnableColormapPalette(bool isOn)
    {
       
        PixelPostProcessing.SetActive(isOn);

    }

    [SerializeField] GameObject NegativePostProcessing;


    public void EnableNegative(bool isOn)
    {

        NegativePostProcessing.SetActive(isOn);

    }

    public void EndOfSong()
    {
        UpperDialogueBox.SetActive(false);
        DownDialogueBox.SetActive(false);
    }

    public void FadeBlack()
    {
        OverlayBackImage.color = new Color(0f, 0f, 0f, OverlayBackImage.color.a + 0.3f);
    }

    public void ChangeToRed()
    {
        OverlayBackImage.color = new Color(0.7f, 0f, 0f, 1f);
    }

    public void End()
    {
        Debug.Log("ReachEnd");
        StartLSD();
        OverlayBackImage.color = new Color(0.7f, 0f, 0f, 0f);
        //EnableColormapPalette(true);
        UpperDialogueBox.gameObject.SetActive(false);
        DownDialogueBox.gameObject.SetActive(false);
        this.GetComponent<PlayableDirector>().playableAsset = EndTimeline;
        this.GetComponent<PlayableDirector>().enabled = true;
        this.GetComponent<PlayableDirector>().Play();
    }
}
