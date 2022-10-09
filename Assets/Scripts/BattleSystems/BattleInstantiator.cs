using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInstantiator : MonoBehaviour
{
    [SerializeField] BattleTypeManager[] availableBattles;
    [SerializeField] bool activateOnEnter;
    private bool inArea;

    [SerializeField] float timeBetweenBattles;
    private float battleCounter;

    [SerializeField] bool deactivateAfterStarting;

    [SerializeField] bool canRunAway;

    [SerializeField] bool shouldCompleteQuest;
    public string questToComplete;

    // Start is called before the first frame update
    void Start()
    {
        battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (inArea && Player.instance.IsMovementActive) {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
                battleCounter -= Time.deltaTime;
            }
        }

        if (battleCounter <= 0) {
            battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f);
            StartCoroutine(StartBattleCoroutine());
        }
    }

    private IEnumerator StartBattleCoroutine() {
        MenuManager.instance.FadeImage();
        GameManager.instance.battleIsActive = true;

        int selectBattle = Random.Range(0, availableBattles.Length);

        BattleManager.instance.itemsReward = availableBattles[selectBattle].rewardItems;
        BattleManager.instance.XPRewardAmount = availableBattles[selectBattle].rewardXP;

        BattleRewardsHandler.instance.markQuestComplete = shouldCompleteQuest;
        BattleRewardsHandler.instance.questToComplete = questToComplete;


        yield return new WaitForSeconds(1.5f);

        MenuManager.instance.FadeOut();
        BattleManager.instance.StartBattle(availableBattles[selectBattle].enemies, canRunAway);

        if (deactivateAfterStarting) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (activateOnEnter) {
                StartCoroutine(StartBattleCoroutine());
            } else {
                inArea = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            inArea = false;
        }
    }
}
