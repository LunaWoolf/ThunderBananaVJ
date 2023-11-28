using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public enum Character
    {
        Up,
        Down,
    }

    public struct Line
    {
        public Character character;
        public string text;
    }

    public struct Dialogue
    {
        public List<Line> lines;
    }

    [Header("Dialogue Ref")]
    public TextAsset DialogueRef;

    public List<Dialogue> dialogueList;



    // Start is called before the first frame update
    void Start()
    {
        ParseDialogue();

    }

    public void ParseDialogue()
    {
        if (DialogueRef == null) return;

        //split dialogue base on -
        string[] dialogueRawArray = DialogueRef.text.Split("\n");
        Dialogue tempDialogue = new Dialogue();
        tempDialogue.lines = new List<Line>();

        foreach (string l in dialogueRawArray)
        {
            if (l.Contains("//") || l == "" || !l.Contains(":"))
                continue;

            tempDialogue.lines.Add(ParseLine(l));
        }

        ConversationManager.instance.currentDialogue = tempDialogue;
        Debug.Log("Finish Parsing Dialogue");
    }

    public Line ParseLine(string l)
    {
        Line line = new Line();
      
        // Parse Enum

        string[] lineSplit = l.Split(":");
        if (lineSplit[0] != null)
        {
            line.character = CharacterParse(lineSplit[0]);
        }

        if (lineSplit[1] != null)
        {
            line.text = lineSplit[1];
        }

        return line;
    }

    public Character CharacterParse(string s)
    {
        switch (s)
        {
            case "A":
                return Character.Up;
                //break;
            case "B":
                return Character.Down;
            //break;
            case "A,B":
                return Character.Down;
                //break;
        }
        Debug.LogError("Invalid Character: " + s);

        return Character.Up;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
