using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] string[] questNames;
    [SerializeField] bool[] questMarkersCompleted;

    public static QuestManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        questMarkersCompleted = new bool[questNames.Length];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            print(CheckIfComplete("Defeat Dragon"));
            MarkQuestComplete("Steal The Gem");
            MarkQuestInComplete("Take Monster Soul");
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            Debug.Log("Data has been saved");
            SaveQuestData();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            Debug.Log("Data has been loaded");
            LoadQuestData();
        }
    }

    public int GetQuestNumber(string questToFind) {
        int questIndex = Array.IndexOf(questNames, questToFind);

        if (questIndex == -1) {
            Debug.LogWarning("Quest" + questToFind + " does not exist");
        }

        return questIndex;
    }

    public bool CheckIfComplete(string questToCheck) {
        int questNumberToCheck = GetQuestNumber(questToCheck);

        if (questNumberToCheck != -1) {
            return questMarkersCompleted[questNumberToCheck];
        }

        return false;
    }

    public void MarkQuestComplete(string questToMark) {
        int numberToCheck = GetQuestNumber(questToMark);

        if (numberToCheck != -1) {
            questMarkersCompleted[numberToCheck] = true;
        }

        UpdateQuestObjects();
    }

    public void MarkQuestInComplete(string questToMark) {
        int numberToCheck = GetQuestNumber(questToMark);

        if (numberToCheck != -1) {
            questMarkersCompleted[numberToCheck] = false;
        }

        UpdateQuestObjects();
    }

    public void UpdateQuestObjects() {
        QuestObject[] questObjects = FindObjectsOfType<QuestObject>();

        if (questObjects.Length > 0) {
            foreach (QuestObject questObject in questObjects) {
                questObject.CheckForCompletion();
            }
        }
    }

    public void SaveQuestData() {
        for (int i = 0; i < questNames.Length; i++) {
            string keyToUse = "QuestMarker_" + questNames[i];

            if (questMarkersCompleted[i]) {
                PlayerPrefs.SetInt(keyToUse, 1);
            } else {
                PlayerPrefs.SetInt(keyToUse, 0);
            }
        }
    }

    public void LoadQuestData() {
        for (int i = 0; i < questNames.Length; i++) {
            int valueToSet = 0;
            string keyToUse = "QuestMarker_" + questNames[i];

            if (PlayerPrefs.HasKey(keyToUse)) {
                valueToSet = PlayerPrefs.GetInt(keyToUse);
            }

            if (valueToSet == 0) {
                questMarkersCompleted[i] = false;
            } else if (valueToSet == 1) {
                questMarkersCompleted[i] = true;
            }
        }
    }
}
