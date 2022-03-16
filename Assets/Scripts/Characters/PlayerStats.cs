using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public string playerName;

    public Sprite characterImage;

    [SerializeField] int maxLevel = 50;
    public int playerLevel = 1;
    public int currentXP;
    public int[] xpForNextLevel;
    // [SerializeField] int baseLevelXP = 100;

    public int maxHP = 100;
    public int currentHP;

    public int maxMana = 30;
    public int currentMana;

    public int dexterity;
    public int defence;

    // Start is called before the first frame update
    void Start()
    {
        xpForNextLevel = new int[maxLevel];

        for (int i = 0; i < maxLevel; i++) {
            xpForNextLevel[i] = (int)(0.02f * i * i * i + 3.06f * i * i + 105.6f * i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            AddXP(100);
        }
    }

    public void AddXP(int amountXP) {
        currentXP += amountXP;

        if (currentXP > xpForNextLevel[playerLevel]) {

            currentXP -= xpForNextLevel[playerLevel];
            playerLevel++;

            if (playerLevel % 2 == 0) {
                dexterity++;
            } else {
                defence++;
            }

            maxHP = (int)(maxHP * 1.2f);
            currentHP = maxHP;

            maxMana = (int)(maxMana * 1.08f);
            currentMana = maxMana;
        }
    }
}
