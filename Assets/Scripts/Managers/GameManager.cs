using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] PlayerStats[] playerStats;

    public bool gameMenuOpened = false, dialogBoxOpened = false, shopOpened = false;

    public int currentGold;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        playerStats = FindObjectsOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMenuOpened || dialogBoxOpened || shopOpened) {
            Player.instance.IsMovementActive = false;
        } else {
            Player.instance.IsMovementActive = true;
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            Debug.Log("Data has been saved");
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            Debug.Log("Data has been loaded");
            LoadData();
        }
    }

    public PlayerStats[] GetPlayerStats() {
        return playerStats;
    }

    public void SaveData() {
        SavePlayerPosition();
        SavePlayerStats();

        PlayerPrefs.SetInt("Number_Of_Items", Inventory.instance.GetItemsList().Count);

        for (int i = 0; i < Inventory.instance.GetItemsList().Count; i++) {
            ItemsManager itemInInventory = Inventory.instance.GetItemsList()[i];
            PlayerPrefs.SetString("Item_" + i + "_Name", itemInInventory.itemName);

            if (itemInInventory.isStackable) {
                PlayerPrefs.SetInt("Items_" + i + "_Amount", itemInInventory.amount);
            }
        }

        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
    }

    private static void SavePlayerPosition() {
        PlayerPrefs.SetFloat("Player_Pos_X", Player.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Pos_Y", Player.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Pos_Z", Player.instance.transform.position.z);
    }

    private void SavePlayerStats() {
        for (int i = 0; i < playerStats.Length; i++) {

            PlayerStats stats = playerStats[i];
            string name = stats.playerName;

            if (playerStats[i].gameObject.activeInHierarchy) {
                PlayerPrefs.SetInt("Player_" + name + "_active", 1);
            } else {
                PlayerPrefs.SetInt("Player_" + name + "_active", 0);
            }

            PlayerPrefs.SetInt("Player_" + name + "_level", stats.playerLevel);
            PlayerPrefs.SetInt("Player_" + name + "_CurrentXP", stats.currentXP);

            PlayerPrefs.SetInt("Player_" + name + "_MaxHP", stats.maxHP);
            PlayerPrefs.SetInt("Player_" + name + "_CurrentHP", stats.currentHP);

            PlayerPrefs.SetInt("Player_" + name + "_MaxMana", stats.maxMana);
            PlayerPrefs.SetInt("Player_" + name + "_CurrentMana", stats.currentMana);

            PlayerPrefs.SetInt("Player_" + name + "_Dexterity", stats.dexterity);
            PlayerPrefs.SetInt("Player_" + name + "_Defence", stats.defence);

            PlayerPrefs.SetString("Player_" + name + "_EquiptWeapon", stats.equiptWeaponName);
            PlayerPrefs.SetString("Player_" + name + "_EquiptArmor", stats.equiptArmorName);

            PlayerPrefs.SetInt("Player_" + name + "_WeaponPower", stats.weaponPower);
            PlayerPrefs.SetInt("Player_" + name + "_ArmorDefence", stats.armorDefence);
        }
    }

    public void LoadData() {
        LoadPlayerPosition();
        LoadPlayerStats();

        for (int i = 0; i < PlayerPrefs.GetInt("Number_Of_Items"); i++) {
            string itemName = PlayerPrefs.GetString("Item_" + i + "_Name");
            ItemsManager itemToAdd = ItemsAssets.instance.GetItemAsset(itemName);
            
            int itemAmount = 0;
            if (PlayerPrefs.HasKey("Items_" + i + "_Amount")) {
                itemAmount = PlayerPrefs.GetInt("Items_" + i + "_Amount");
            }

            Inventory.instance.AddItems(itemToAdd);
            if (itemToAdd.isStackable && itemAmount > 1) {
                itemToAdd.amount = itemAmount;
            }
        }
    }

    private void LoadPlayerStats() {
        for (int i = 0; i < playerStats.Length; i++) {
            PlayerStats stats = playerStats[i];
            string name = stats.playerName;

            if (PlayerPrefs.GetInt("Player_" + name + "_active") == 0) {
                stats.gameObject.SetActive(false);
            } else if (PlayerPrefs.GetInt("Player_" + name + "_active") == 1) {
                stats.gameObject.SetActive(true);
            }

            stats.playerLevel = PlayerPrefs.GetInt("Player_" + name + "_level");
            stats.currentXP = PlayerPrefs.GetInt("Player_" + name + "_CurrentXP");

            stats.maxHP = PlayerPrefs.GetInt("Player_" + name + "_MaxHP");
            stats.currentHP = PlayerPrefs.GetInt("Player_" + name + "_CurrentHP");

            stats.maxMana = PlayerPrefs.GetInt("Player_" + name + "_MaxMana");
            stats.currentMana = PlayerPrefs.GetInt("Player_" + name + "_CurrentMana");

            stats.dexterity = PlayerPrefs.GetInt("Player_" + name + "_Dexterity");
            stats.defence = PlayerPrefs.GetInt("Player_" + name + "_Defence");

            stats.equiptWeaponName = PlayerPrefs.GetString("Player_" + name + "_EquiptWeapon");
            stats.equiptArmorName = PlayerPrefs.GetString("Player_" + name + "_EquiptArmor");

            stats.weaponPower = PlayerPrefs.GetInt("Player_" + name + "_WeaponPower");
            stats.armorDefence = PlayerPrefs.GetInt("Player_" + name + "_ArmorDefence");
        }
    }

    private static void LoadPlayerPosition() {
        float pos_x = PlayerPrefs.GetFloat("Player_Pos_X");
        float pos_y = PlayerPrefs.GetFloat("Player_Pos_Y");
        float pos_z = PlayerPrefs.GetFloat("Player_Pos_Z");

        Player.instance.transform.position = new Vector3(pos_x, pos_y, pos_z);
    }
}
