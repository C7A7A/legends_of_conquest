using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            StartBattle(new string[] { "GreatShadow", "Mage", "MageBoss", "Shadow" });
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

    public void StartBattle(string[] enemiesToSpawn) {
        if (!isBattleActive) {
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
                // kill character
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
                print("You won");
            } else if (allPlayersAreDead) {
                print("You lost");
            }

            battleScene.SetActive(false);
            GameManager.instance.battleIsActive = false;
            isBattleActive = false;
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
                Instantiate(
                    battleMovesList[i].EffectToUse,
                    activeCharacters[selectedPlayerToAttack].transform.position,
                    activeCharacters[selectedPlayerToAttack].transform.rotation
                );

                movePower = battleMovesList[i].movePower;
            }
        }

        Instantiate(
            attackParticle,
            activeCharacters[currentTurn].transform.position,
            activeCharacters[currentTurn].transform.rotation
        );

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
}
