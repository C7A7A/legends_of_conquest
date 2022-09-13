using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogHandler : MonoBehaviour
{
    public string[] sentences;
    private bool canActivateBox;

    [SerializeField] bool shouldActivateQuest;
    [SerializeField] string questToMark;
    [SerializeField] bool markAsComplete;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canActivateBox && Input.GetButtonUp("Fire1") && !DialogController.instance.IsDialogBoxActive()) {
            DialogController.instance.ActivateDialog(sentences);

            // quick fix -> Dialog Panel doesn't close without it.
            // TODO: You need too click twice to go to second sentence.
            canActivateBox = false;

            if (shouldActivateQuest) {
                DialogController.instance.ActivateQuestAtEnd(questToMark, markAsComplete);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player")) {
            canActivateBox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            canActivateBox = false;
        }
    }
}
