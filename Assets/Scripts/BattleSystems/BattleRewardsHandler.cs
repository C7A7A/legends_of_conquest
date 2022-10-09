using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleRewardsHandler : MonoBehaviour
{
    public static BattleRewardsHandler instance;

    [SerializeField] TextMeshProUGUI XPText, itemsText;
    [SerializeField] GameObject rewardScreen;

    [SerializeField] ItemsManager[] rewardItems;
    [SerializeField] int XPReward;

    public bool markQuestComplete;
    public string questToComplete;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) {
            OpenRewardScreen(15000, rewardItems);
        }
    }

    public void OpenRewardScreen(int XPEarned, ItemsManager[] ItemsEarned) {
        XPReward = XPEarned;
        rewardItems = ItemsEarned;

        XPText.text = XPEarned + " XP";
        itemsText.text = "";

        foreach (ItemsManager item in rewardItems) {
            itemsText.text += item.itemName + "  ";
        }

        rewardScreen.SetActive(true);
    }

    public void CloseRewardScreen() {
        foreach (PlayerStats player in GameManager.instance.GetPlayerStats()) {
            if (player.gameObject.activeInHierarchy) {
                player.AddXP(XPReward);
            }
        }

        foreach (ItemsManager item in rewardItems) {
            Inventory.instance.AddItems(item);
        }

        if (markQuestComplete) {
            QuestManager.instance.MarkQuestComplete(questToComplete);
        }

        rewardScreen.SetActive(false);
        GameManager.instance.battleIsActive = false;
    }
}
