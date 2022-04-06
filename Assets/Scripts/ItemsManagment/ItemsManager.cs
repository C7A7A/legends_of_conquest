using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public enum ItemType {
        Item,
        Weapon,
        Armor
    }
    public ItemType itemType;

    public string itemName, itemDescription;
    public int valueInCoins;
    public Sprite itemsImage;

    public int amountOfAffect;
    public enum AffectType {
        HP,
        Mana
    }
    public AffectType affectType;

    public int weaponDexterity, armorDefence;

    public bool isStackable;
    public int amount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseItem(int characterToUseOn) {

        PlayerStats selectedCharacter = GameManager.instance.GetPlayerStats()[characterToUseOn];

        if (itemType == ItemType.Item) {
            if (affectType == AffectType.HP) {
                selectedCharacter.AddHP(amountOfAffect);
            } else if (affectType == AffectType.Mana) {
                selectedCharacter.AddMana(amountOfAffect);
            }
        } else if (itemType == ItemType.Weapon) {
            if (selectedCharacter.equiptWeaponName != "") {
                Inventory.instance.AddItems(selectedCharacter.equiptWeapon);
            }

            selectedCharacter.EquipWeapon(this);
        } else if (itemType == ItemType.Armor) {
            if (selectedCharacter.equiptArmorName != "") {
                Inventory.instance.AddItems(selectedCharacter.equiptArmor);
            }

            selectedCharacter.EquipArmor(this);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Inventory.instance.AddItems(this);
            SelfDestroy();
        }   
    }

    private void SelfDestroy() {
        gameObject.SetActive(false);
    }
}
