using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText, nameText;
    [SerializeField] GameObject dialogBox, nameBox;

    [SerializeField] string[] dialogSentences;
    [SerializeField] int currentSentence;

    public static DialogController instance;

    private bool dialogJustStarted;

    private string questToMark;
    private bool markQuestComplete;
    private bool shouldMarkQuest;

    // Start is called before the first frame update
    void Start()
    {
        dialogText.text = dialogSentences[currentSentence];
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogBox.activeInHierarchy) {
            if (Input.GetButtonUp("Fire1")) {
                
                if (!dialogJustStarted) {
                    currentSentence++;
                    
                    if (currentSentence >= dialogSentences.Length) {
                        dialogBox.SetActive(false);
                        GameManager.instance.dialogBoxOpened = false;

                        if (shouldMarkQuest) {
                            shouldMarkQuest = false;
                            if (markQuestComplete) {
                                QuestManager.instance.MarkQuestComplete(questToMark);
                            } else {
                                QuestManager.instance.MarkQuestInComplete(questToMark);
                            }
                        }

                    } else {
                        CheckForCharacterName();
                        dialogText.text = dialogSentences[currentSentence];
                    }

                } else {
                    dialogJustStarted = false;
                }

            }
        }
    }

    public void ActivateDialog(string[] sentences) {
        dialogSentences = sentences;
        currentSentence = 0;

        CheckForCharacterName();
        dialogText.text = dialogSentences[currentSentence];
        dialogBox.SetActive(true);

        dialogJustStarted = true;
        GameManager.instance.dialogBoxOpened = true;
    }

    public bool IsDialogBoxActive() {
        return dialogBox.activeInHierarchy;
    }

    private void CheckForCharacterName() {
        if (dialogSentences[currentSentence].StartsWith("#")) {
            nameText.text = dialogSentences[currentSentence].Replace("#", "");
            currentSentence++;
        }
    }

    public void ActivateQuestAtEnd(string questName, bool markComplete) {
        questToMark = questName;
        markQuestComplete = markComplete;
        shouldMarkQuest = true;
    }
}
