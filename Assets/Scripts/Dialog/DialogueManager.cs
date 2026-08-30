using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Data")]
    public DialogueLine[] dialogueLines;

    private Dictionary<int, DialogueLine> dialogueDictionary;

    private DialogueLine currentDialogue;

    [Header("Systems")]
    public CharacterRegistry characterRegistry;

    public ChoiceSystem choiceSystem;

    [Header("UI")]
    public TMP_Text speakerNameText;

    public TMP_Text dialogueText;

    [Header("Buttons")]
    public GameObject nextButton;

    private void Awake()
    {
        dialogueDictionary = new Dictionary<int, DialogueLine>();

        foreach (DialogueLine line in dialogueLines)
        {
            if (!dialogueDictionary.ContainsKey(line.dialogueID))
            {
                dialogueDictionary.Add(line.dialogueID, line);
            }
        }
    }

    private void Start()
    {
        choiceSystem.Setup(this);

        GoToDialogue(0);
    }

    public void GoToDialogue(int dialogueID)
    {
        if (!dialogueDictionary.ContainsKey(dialogueID))
        {
            Debug.LogWarning("Dialogue ID not found: " + dialogueID);
            return;
        }

        currentDialogue = dialogueDictionary[dialogueID];

        ShowDialogue();
    }

    void ShowDialogue()
    {
        // Update Text UI
        speakerNameText.text = currentDialogue.speakerName;
        dialogueText.text = currentDialogue.dialogueText;

        // All Characters Idle
        characterRegistry.SetAllIdle();

        // Get Speaker
        CharacterActor speaker =
            characterRegistry.GetCharacter(currentDialogue.speakerID);

        // Speaker Talk
        if (speaker != null)
        {
            speaker.PlayTalk();
        }

        // Choice Check
        if (currentDialogue.hasChoices)
        {
            nextButton.SetActive(false);

            choiceSystem.ShowChoices(currentDialogue.choices);
        }
        else
        {
            nextButton.SetActive(true);

            choiceSystem.ClearChoices();
        }
    }

    public void NextDialogue()
    {
        if (currentDialogue.hasChoices)
        {
            return;
        }

        GoToDialogue(currentDialogue.nextDialogueID);
    }
}