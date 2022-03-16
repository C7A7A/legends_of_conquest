using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] PlayerStats[] playerStats;

    public bool gameMenuOpened = false, dialogBoxOpened = false;

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
        if (gameMenuOpened || dialogBoxOpened ) {
            Player.instance.IsMovementActive = false;
        } else {
            Player.instance.IsMovementActive = true;
        }
    }

    public PlayerStats[] GetPlayerStats() {
        return playerStats;
    }
}
