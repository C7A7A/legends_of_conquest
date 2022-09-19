using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        for (int i = 0; i < battleMovesList.Length; i++) {
            if (battleMovesList[i].moveName == activeCharacters[currentTurn].AttacksAvailable()[selectedAttack]) {
                Instantiate(
                    battleMovesList[i].EffectToUse,
                    activeCharacters[selectedPlayerToAttack].transform.position,
                    activeCharacters[selectedPlayerToAttack].transform.rotation
                );
            }
        }
    }
}
