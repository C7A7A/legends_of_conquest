using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacters : MonoBehaviour
{

    [SerializeField] bool isPlayer;
    [SerializeField] string[] attacksAvailable;

    public string characterName;
    public int currentHP, maxHP, currentMana, maxMana, dexterity, defence, weaponPower, armorDefence;
    public bool isDead;
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsPlayer() {
        return isPlayer;
    }

    public string[] AttacksAvailable() {
        return attacksAvailable;
    }

    public void TakeDamage(int damageToReceive) {
        currentHP -= damageToReceive;

        if (currentHP < 0) {
            currentHP = 0;
        }
    }

    public void UseItem(ItemsManager itemToUse) {
        if (itemToUse.itemType == ItemsManager.ItemType.Item) {
            if (itemToUse.affectType == ItemsManager.AffectType.HP) {
                AddHP(itemToUse.amountOfAffect);
            } else if (itemToUse.affectType == ItemsManager.AffectType.Mana) {
                AddMana(itemToUse.amountOfAffect);
            }
        }
    }

    private void AddMana(int amountOfAffect) {
        currentMana += amountOfAffect;
    }

    private void AddHP(int amountOfAffect) {
        currentHP += amountOfAffect;
    }
}
