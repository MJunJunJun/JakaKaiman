using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [Header("Dialogue")]
    public int dialogueID;

    public int nextDialogueID;

    [Header("Speaker")]
    public string speakerID;

    public string speakerName;

    [TextArea(3, 5)]
    public string dialogueText;

    [Header("Choices")]
    public bool hasChoices;

    public ChoiceData[] choices;
}