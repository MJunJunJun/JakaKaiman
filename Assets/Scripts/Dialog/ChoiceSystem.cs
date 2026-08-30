using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceSystem : MonoBehaviour
{
    [Header("UI")]
    public GameObject choiceButtonPrefab;

    public Transform choicePanel;

    private DialogueManager dialogueManager;

    public void Setup(DialogueManager manager)
    {
        dialogueManager = manager;
    }

    public void ShowChoices(ChoiceData[] choices)
    {
        ClearChoices();

        choicePanel.gameObject.SetActive(true);

        foreach (ChoiceData choice in choices)
        {
            GameObject buttonObj =
                Instantiate(choiceButtonPrefab, choicePanel);

            TMP_Text buttonText =
                buttonObj.GetComponentInChildren<TMP_Text>();

            buttonText.text = choice.choiceText;

            Button button =
                buttonObj.GetComponent<Button>();

            int nextID = choice.nextDialogueID;

            button.onClick.AddListener(() =>
            {
                dialogueManager.GoToDialogue(nextID);

                ClearChoices();
            });
        }
    }

    public void ClearChoices()
    {
        foreach (Transform child in choicePanel)
        {
            Destroy(child.gameObject);
        }

        choicePanel.gameObject.SetActive(false);
    }
}