using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;
    private Animator animator;
    public GameObject menu;

    [SerializeField] GameObject[] statsButton;

    public static MenuManager instance;

    private PlayerStats[] playerStats;
    [SerializeField] TextMeshProUGUI[] nameText, HPText, manaText, currentXPText, currentXPPercent;
    [SerializeField] Slider[] XPSlider;
    [SerializeField] Image[] characterImage;
    [SerializeField] GameObject[] characterPanel;

    [SerializeField] TextMeshProUGUI statName, statHP, statMana, statDex, statDef, statEquiptWeapon, statEquiptArmor, statWeaponPower, statArmorDefence;
    [SerializeField] Image characterStatImage;

    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] RectTransform itemSlotContainerParent;

    public TextMeshProUGUI itemName, itemDescription;

    public ItemsManager activeItem;

    [SerializeField] GameObject characterChoicePanel;
    [SerializeField] TextMeshProUGUI[] itemsCharacterChoiceNames;

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
                UpdateStats();
                menu.SetActive(true);
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

        statEquiptWeapon.text = playerSelected.equiptWeaponName;
        statEquiptArmor.text = playerSelected.equiptArmorName;

        statWeaponPower.text = playerSelected.weaponPower.ToString();
        statArmorDefence.text = playerSelected.armorDefence.ToString();
    }

    public void UpdateItemsInventory() {
        foreach (RectTransform itemSlot in itemSlotContainerParent) {
            Destroy(itemSlot.gameObject);
        }
        
        List<ItemsManager> itemsList = Inventory.instance.GetItemsList();

        foreach (ItemsManager item in itemsList) {
            RectTransform itemSlot = Instantiate(itemSlotContainer, itemSlotContainerParent).GetComponent<RectTransform>();

            Image itemImage = itemSlot.Find("ItemImage").GetComponent<Image>();
            itemImage.sprite = item.itemsImage;

            TextMeshProUGUI itemAmountText = itemSlot.Find("ItemAmountText").GetComponent<TextMeshProUGUI>();

            if (item.amount > 1) {
                itemAmountText.text = item.amount.ToString();
            } else {
                itemAmountText.text = "";
            }

            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
        }
    }

    public void DiscardItem() {
        Inventory.instance.RemoveItem(activeItem);
        UpdateItemsInventory();

        AudioManager.instance.PlaySFX(3);
    }

    public void UseItem(int selectedCharacter) {
        activeItem.UseItem(selectedCharacter);
        OpenCharacterChoicePanel();

        Inventory.instance.RemoveItem(activeItem);
        UpdateItemsInventory();

        AudioManager.instance.PlaySFX(8);
    }

    public void OpenCharacterChoicePanel() {
        characterChoicePanel.SetActive(true);

        if (activeItem) {
            for (int i = 0; i < playerStats.Length; i++) {
                PlayerStats activePlayer = GameManager.instance.GetPlayerStats()[i];
                itemsCharacterChoiceNames[i].text = activePlayer.playerName;

                bool activePlayerAvailable = activePlayer.gameObject.activeInHierarchy;
                itemsCharacterChoiceNames[i].transform.parent.gameObject.SetActive(activePlayerAvailable);
            }
        }
    }

    public void CloseCharacterChoicePanel() {
        characterChoicePanel.SetActive(false);
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
