using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] GameObject objectToActivate;
    [SerializeField] string questToCheck;
    [SerializeField] bool activateIfComplete;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) {
            CheckForCompletion();
        }
    }

    public void CheckForCompletion() {
        if (QuestManager.instance.CheckIfComplete(questToCheck)) {
            objectToActivate.SetActive(activateIfComplete);
        }
    }
}
