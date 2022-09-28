using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleMagicButtons : MonoBehaviour
{
    public string spellName;
    public int spellCost;
    public TextMeshProUGUI spellNameText, spellCostText;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Press() {
        if (BattleManager.instance.GetCurrentCharacter().currentMana >= spellCost) {
            BattleManager.instance.magicChoicePanel.SetActive(false);
            BattleManager.instance.OpenTargetMenu(spellName);
            BattleManager.instance.GetCurrentCharacter().currentMana -= spellCost;
        } else {
            BattleManager.instance.battleNotice.SetText("You don't have enough mana");
            BattleManager.instance.battleNotice.Activate();
            BattleManager.instance.magicChoicePanel.SetActive(false);
        }
    }
}
