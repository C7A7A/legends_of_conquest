using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;
    private Animator animator;
    [SerializeField] GameObject menu;

    [SerializeField] GameObject[] statsButton;

    public static MenuManager instance;

    private PlayerStats[] playerStats;
    [SerializeField] TextMeshProUGUI[] nameText, HPText, manaText, currentXPText, currentXPPercent;
    [SerializeField] Slider[] XPSlider;
    [SerializeField] Image[] characterImage;
    [SerializeField] GameObject[] characterPanel;

    [SerializeField] TextMeshProUGUI statName, statHP, statMana, statDex, statDef;
    [SerializeField] Image characterStatImage;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        DeactivatedCharacterPanels();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (menu.activeInHierarchy) {
                menu.SetActive(false);
                GameManager.instance.gameMenuOpened = false;
            } else {
                menu.SetActive(true);
                UpdateStats();
                GameManager.instance.gameMenuOpened = true;
            }
        }
    }

    public void UpdateStats() {
        playerStats = GameManager.instance.GetPlayerStats();

        for (int i = 0; i < playerStats.Length; i++) {
            characterPanel[i].SetActive(true);

            nameText[i].text = playerStats[i].playerName;
            HPText[i].text = "HP:        " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
            manaText[i].text = "Mana:     " + playerStats[i].currentMana + "/" + playerStats[i].maxMana;
            currentXPText[i].text = "XP:        " + playerStats[i].currentXP;

            characterImage[i].sprite = playerStats[i].characterImage;

            XPSlider[i].maxValue = playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
            XPSlider[i].value = playerStats[i].currentXP;

            currentXPPercent[i].text = Mathf.Round((float)playerStats[i].currentXP / (float)playerStats[i].xpForNextLevel[playerStats[i].playerLevel] * 100)
                .ToString() + " %";
        }
    }

    public void StatsMenu() {
        for (int i = 0; i < playerStats.Length; i++) {
            statsButton[i].SetActive(true);

            statsButton[i].GetComponentInChildren<TextMeshProUGUI>().text = playerStats[i].playerName;
        }

        StatsMenuUpdate(0);
    }

    public void StatsMenuUpdate(int playerSelectedNumber) {
        PlayerStats playerSelected = playerStats[playerSelectedNumber];

        statName.text = playerSelected.playerName;
        statHP.text = playerSelected.currentHP.ToString() + "/" + playerSelected.maxHP.ToString();
        statMana.text = playerSelected.currentMana.ToString() + "/" + playerSelected.maxMana.ToString();
        
        statDex.text = playerSelected.dexterity.ToString();
        statDef.text = playerSelected.defence.ToString();

        characterStatImage.sprite = playerSelected.characterImage;
    }

    public void QuitGame() {
        Application.Quit();
        Debug.Log("You`ve quit the game");
    }

    public void FadeImage() {
        animator = imageToFade.GetComponent<Animator>();
        animator.SetTrigger("StartFading");
    }

    private void DeactivatedCharacterPanels() {
        for (int i = 0; i < characterPanel.Length; i++) {
            characterPanel[i].SetActive(false);
        }
    }
}
