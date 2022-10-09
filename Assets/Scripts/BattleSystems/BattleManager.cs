using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    private bool isBattleActive;

    [SerializeField] GameObject battleScene;
    [SerializeField] List<BattleCharacters> activeCharacters = new List<BattleCharacters>();

    [SerializeField] Transform[] playerPositions;
    [SerializeField] Transform[] enemyPositions;

    [SerializeField] BattleCharacters[] playerPrefabs, enemyPrefabs;

    [SerializeField] int currentTurn;
    [SerializeField] bool waitingForTurn;
    [SerializeField] GameObject UIButtonHolder;

    [SerializeField] BattleMoves[] battleMovesList;

    [SerializeField] ParticleSystem attackParticle;
    [SerializeField] CharacterDamageGUI damageAmountText;

    [SerializeField] GameObject[] playersBattleStats;
    [SerializeField] TextMeshProUGUI[] playerNamesText;
    [SerializeField] Slider[] HPSliders, manaSliders;

    [SerializeField] GameObject enemyTargetPanel;
    [SerializeField] BattleTargetButtons[] targetButtons;

    public GameObject magicChoicePanel;
    [SerializeField] BattleMagicButtons[] magicButtons;

    public BattleNotifications battleNotice;

    [SerializeField] float chanceToRunAway = 0.5f;

    public GameObject itemsToUseMenu;
    [SerializeField] ItemsManager selectedItem;
    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] Transform itemSlotContainerParent;
    [SerializeField] TextMeshProUGUI itemName, itemDescription;

    [SerializeField] GameObject characterChoicePanel;
    [SerializeField] TextMeshProUGUI[] playerNames;

    [SerializeField] string gameOverScene;

    private bool runAway;
    public int XPRewardAmount;
    public ItemsManager[] itemsReward;

    private bool canRun;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            StartBattle(new string[] { "GreatShadow", "Mage", "MageBoss", "Shadow" }, true);
        }

        if (Input.GetKeyDown(KeyCode.N)) {
            NextTurn();
        }

        CheckPlayerButtonHolder();
    }

    private void CheckPlayerButtonHolder() {
        if (isBattleActive) {
            if (waitingForTurn) {
                if (activeCharacters[currentTurn].IsPlayer()) {
                    UIButtonHolder.SetActive(true);
                } else {
                    UIButtonHolder.SetActive(false);
                    StartCoroutine(EnemyMoveCourutine());
                }
            }
        }
    }

    public void StartBattle(string[] enemiesToSpawn, bool canRunAway) {
        if (!isBattleActive) {
            canRun = canRunAway;
            SetUpBattle();
            AddPlayers();
            AddEnemies(enemiesToSpawn);
            UpdatePlayerStats();

            waitingForTurn = true;
            currentTurn = 0;
        }
    }

    private void AddEnemies(string[] enemiesToSpawn) {
        for (int i = 0; i < enemiesToSpawn.Length; i++) {
            if (enemiesToSpawn[i] != "") {
                for (int j = 0; j < enemyPrefabs.Length; j++) {
                    if (enemyPrefabs[j].characterName == enemiesToSpawn[i]) {

                        BattleCharacters newEnemy = Instantiate(
                            enemyPrefabs[j],
                            enemyPositions[i].position,
                            enemyPositions[i].rotation,
                            enemyPositions[i]
                        );

                        activeCharacters.Add(newEnemy);
                    }
                }
            }
        }
    }

    private void AddPlayers() {
        for (int i = 0; i < GameManager.instance.GetPlayerStats().Length; i++) {
            if (GameManager.instance.GetPlayerStats()[i].gameObject.activeInHierarchy) {
                for (int j = 0; j < playerPrefabs.Length; j++) {
                    if (playerPrefabs[j].characterName == GameManager.instance.GetPlayerStats()[i].playerName) {
                        BattleCharacters newPlayer = Instantiate(
                            playerPrefabs[j],
                            playerPositions[i].position,
                            playerPositions[i].rotation,
                            playerPositions[i]
                        );

                        activeCharacters.Add(newPlayer);
                        ImportPlayerstats(i);
                    }
                }
            }
        }
    }

    private void ImportPlayerstats(int i) {
        PlayerStats player = GameManager.instance.GetPlayerStats()[i];

        activeCharacters[i].currentHP = player.currentHP;
        activeCharacters[i].maxHP = player.maxHP;

        activeCharacters[i].currentMana = player.currentMana;
        activeCharacters[i].maxMana = player.maxMana;

        activeCharacters[i].dexterity = player.dexterity;
        activeCharacters[i].defence = player.defence;

        activeCharacters[i].weaponPower = player.weaponPower;
        activeCharacters[i].armorDefence = player.armorDefence;
    }

    private void SetUpBattle() {
        isBattleActive = true;
        GameManager.instance.battleIsActive = true;

        transform.position = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            transform.position.z
        );

        battleScene.SetActive(true);
        
    }

    private void NextTurn() {
        currentTurn++;
        if (currentTurn >= activeCharacters.Count) {
            currentTurn = 0;
        }

        waitingForTurn = true;
        UpdateBattle();
        UpdatePlayerStats();
    }

    private void UpdateBattle() {
        bool allEnemiesAreDead = true;
        bool allPlayersAreDead = true;

        for (int i = 0; i < activeCharacters.Count; i++) {
            if (activeCharacters[i].currentHP < 0) {
                activeCharacters[i].currentHP = 0;
            }

            if (activeCharacters[i].currentHP == 0) {
                if (activeCharacters[i].IsPlayer() && !activeCharacters[i].isDead) {
                    activeCharacters[i].KillPlayer();
                } else if (!activeCharacters[i].IsPlayer() && !activeCharacters[i].isDead) {
                    activeCharacters[i].KillEnemy();
                }
            } else {
                if (activeCharacters[i].IsPlayer()) {
                    allPlayersAreDead = false;
                } else {
                    allEnemiesAreDead = false;
                }
            }
        }

        if (allEnemiesAreDead || allPlayersAreDead) {
            if (allEnemiesAreDead) {
                StartCoroutine(EndBattleCoroutine());
            } else if (allPlayersAreDead) {
                StartCoroutine(GameOverCouroutine());
            }
        } else {
            while (activeCharacters[currentTurn].currentHP == 0) {
                currentTurn++;

                if (currentTurn >= activeCharacters.Count) {
                    currentTurn = 0;
                }
            }
        }
    }

    public IEnumerator EnemyMoveCourutine() {
        waitingForTurn = false;

        yield return new WaitForSeconds(1f);
        EnemyAttack();

        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void EnemyAttack() {
        List<int> players = new List<int>();

        for (int i = 0; i < activeCharacters.Count; i++) {
            if (activeCharacters[i].IsPlayer() && activeCharacters[i].currentHP > 0) {
                players.Add(i);
            }
        }

        int selectedPlayerToAttack = players[Random.Range(0, players.Count)];
        int selectedAttack = Random.Range(0, activeCharacters[currentTurn].AttacksAvailable().Length);
        int movePower = 0;


        for (int i = 0; i < battleMovesList.Length; i++) {
            if (battleMovesList[i].moveName == activeCharacters[currentTurn].AttacksAvailable()[selectedAttack]) {
                movePower = GetMovePowerAndInstantiateEffect(selectedPlayerToAttack, i);
            }
        }

        InstantiateEffectOnAttackingCharacter();

        DealDamage(selectedPlayerToAttack, movePower);

        UpdatePlayerStats();
    }

    private void DealDamage(int characterToAttack, int movePower) {
        BattleCharacters activeCharacter = activeCharacters[currentTurn];
        BattleCharacters attackedCharacter = activeCharacters[characterToAttack];

        float attackPower = activeCharacter.dexterity + activeCharacter.weaponPower;
        float defenceAmount = attackedCharacter.defence + attackedCharacter.armorDefence;
        
        float damageAmount = (attackPower / defenceAmount) * movePower * Random.Range(0.9f, 1.1f);
        int damageToDeal = (int)damageAmount;

        if (CriticalHit()) {
            damageToDeal *= 2;
            Debug.Log("CRITICAL HIT! " + damageToDeal);
        }

        Debug.Log(activeCharacter.characterName + " just dealt " + damageAmount + " (" + damageToDeal + ") to " + attackedCharacter, this);

        attackedCharacter.TakeDamage(damageToDeal);

        CharacterDamageGUI characterDamageText = Instantiate(
            damageAmountText,
            attackedCharacter.transform.position,
            attackedCharacter.transform.rotation
        );

        characterDamageText.SetDamage(damageToDeal);
    }

    private bool CriticalHit() {
        if (Random.value <= 0.8f) {
            return true;
        }
        return false;
    }

    public void UpdatePlayerStats() {
        for (int i = 0; i < playerNamesText.Length; i++) {
            if (activeCharacters.Count > i) {
                if (activeCharacters[i].IsPlayer()) {
                    BattleCharacters playerData = activeCharacters[i];

                    playerNamesText[i].text = playerData.characterName;

                    HPSliders[i].maxValue = playerData.maxHP;
                    HPSliders[i].value = playerData.currentHP;

                    manaSliders[i].maxValue = playerData.maxMana;
                    manaSliders[i].value = playerData.currentMana;
                } else {
                    playersBattleStats[i].gameObject.SetActive(false);
                }
            } else {
                playersBattleStats[i].gameObject.SetActive(false);
            }
        }
    }

    // Player attacking methods

    public void PlayerAttack(string moveName, int target) {
        int movePower = 0;

        for (int i = 0; i < battleMovesList.Length; i++) {
            if (battleMovesList[i].moveName == moveName) {
                movePower = GetMovePowerAndInstantiateEffect(target, i);
            }
        }

        InstantiateEffectOnAttackingCharacter();
        DealDamage(target, movePower);

        NextTurn();

        enemyTargetPanel.SetActive(false);
    }

    public void OpenTargetMenu(string moveName) {
        enemyTargetPanel.SetActive(true);

        List<int> Enemies = new List<int>();

        for (int i = 0; i < activeCharacters.Count; i++) {
            if (!activeCharacters[i].IsPlayer()) {
                Enemies.Add(i);
            }
        }

        for (int i = 0; i < targetButtons.Length; i++) {
            if (Enemies.Count > i && activeCharacters[Enemies[i]].currentHP > 0) {
                targetButtons[i].gameObject.SetActive(true);
                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattleTarget = Enemies[i];
                targetButtons[i].targetName.text = activeCharacters[Enemies[i]].characterName;
            } else {
                targetButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void InstantiateEffectOnAttackingCharacter() {
        Instantiate(
                    attackParticle,
                    activeCharacters[currentTurn].transform.position,
                    activeCharacters[currentTurn].transform.rotation
                );
    }

    private int GetMovePowerAndInstantiateEffect(int target, int i) {
        int movePower;
        Instantiate(
               battleMovesList[i].EffectToUse,
               activeCharacters[target].transform.position,
               activeCharacters[target].transform.rotation
           );

        movePower = battleMovesList[i].movePower;
        return movePower;
    }

    public void OpenMagicPanel() {
        magicChoicePanel.SetActive(true);

        for (int i = 0; i < magicButtons.Length; i++) {
            if (activeCharacters[currentTurn].AttacksAvailable().Length > i) {
                magicButtons[i].gameObject.SetActive(true);
                magicButtons[i].spellName = GetCurrentCharacter().AttacksAvailable()[i];
                magicButtons[i].spellNameText.text = magicButtons[i].spellName;
                for (int j = 0; j < battleMovesList.Length; j++) {
                    if (magicButtons[i].spellName == battleMovesList[j].moveName) {
                        magicButtons[i].spellCost = battleMovesList[j].manaCost;
                        magicButtons[i].spellCostText.text = $"cost: {battleMovesList[j].manaCost}";
                    }
                }
            } else {
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public BattleCharacters GetCurrentCharacter() {
        return activeCharacters[currentTurn];
    }

    public void RunAway() {
        if (canRun) {
            if (Random.value > chanceToRunAway) {
                runAway = true;
                StartCoroutine(EndBattleCoroutine());
            } else {
                NextTurn();
                battleNotice.SetText("You were unable to escape");
                battleNotice.Activate();
            }
        }
    }

    public void UpdateItemsInInventory() {
        itemsToUseMenu.SetActive(true);

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

    public void SelectedItemToUse(ItemsManager itemToUse) {
        selectedItem = itemToUse;
        itemName.text = selectedItem.itemName;
        itemDescription.text = selectedItem.itemDescription;
    }

    public void OpenCharacterMenu() {
        if (selectedItem) {
            characterChoicePanel.SetActive(true);

            for (int i = 0; i < activeCharacters.Count; i++) {
                if (activeCharacters[i].IsPlayer()) {
                    PlayerStats activePlayer = GameManager.instance.GetPlayerStats()[i];

                    playerNames[i].text = activePlayer.playerName;

                    bool activePlayerInHierarchy = activePlayer.gameObject.activeInHierarchy;
                    playerNames[i].transform.parent.gameObject.SetActive(activePlayerInHierarchy);
                }
            }
        } else {
            Debug.Log("No item selected", this.gameObject);
        }
    }

    public void UseItemButton(int selectedPlayer) {
        activeCharacters[selectedPlayer].UseItem(selectedItem);
        Inventory.instance.RemoveItem(selectedItem);

        UpdatePlayerStats();
        CloseCharacterChoice();
        UpdateItemsInInventory();
    }

    public void CloseCharacterChoice() {
        characterChoicePanel.SetActive(false);
        itemsToUseMenu.SetActive(false);
    }

    public IEnumerator EndBattleCoroutine() {
        isBattleActive = false;

        UIButtonHolder.SetActive(false);
        enemyTargetPanel.SetActive(false);
        magicChoicePanel.SetActive(false);

        if (!runAway) {
            battleNotice.SetText("You won!");
            battleNotice.Activate();
        }

        yield return new WaitForSeconds(3f);

        foreach (BattleCharacters playerInBattle in activeCharacters) {
            if (playerInBattle.IsPlayer()) {
                foreach (PlayerStats playerWithStats in GameManager.instance.GetPlayerStats()) {
                    if (playerInBattle.characterName == playerWithStats.playerName) {
                        playerWithStats.currentHP = playerInBattle.currentHP;
                        playerWithStats.currentMana = playerInBattle.currentMana;
                    }
                }
            }

            Destroy(playerInBattle.gameObject);
        }

        battleScene.SetActive(false);
        activeCharacters.Clear();

        if (runAway) {
            GameManager.instance.battleIsActive = false;
            runAway = false;
        } else {
            BattleRewardsHandler.instance.OpenRewardScreen(XPRewardAmount, itemsReward);
        }

        currentTurn = 0;
    }

    public IEnumerator GameOverCouroutine() {
        battleNotice.SetText("You lost!");
        battleNotice.Activate();

        yield return new WaitForSeconds(3f);

        isBattleActive = false;
        SceneManager.LoadScene(gameOverScene);
    }
}
