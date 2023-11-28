using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConversationManager : MonoSingleton<ConversationManager>
{
    public enum ConversationState
    {
        SpaceProgress,
        //Down,
    }

    [SerializeField] ConversationState currentConversationState = ConversationState.SpaceProgress;
    public DialogueParser.Dialogue currentDialogue;
    int currentDialogueLineIndex = 0;

    [Header("UI Reference")]
    [SerializeField] TextMeshProUGUI UpCharacterTM;
    [SerializeField] TextMeshProUGUI DownCharacterTM;


    // Start is called before the first frame update
    void Start()
    {
        
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

        switch (currentLine.character)
        {
            case DialogueParser.Character.Up:
                UpCharacterTM.text = currentLine.text;
                break;
            case DialogueParser.Character.Down:
                DownCharacterTM.text = currentLine.text;
                break;

        }
     
    }
}
