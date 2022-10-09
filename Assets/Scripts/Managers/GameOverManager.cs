using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOverManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBackgroundMusic(4);
        Player.instance.gameObject.SetActive(false);
        MenuManager.instance.gameObject.SetActive(false);
        BattleManager.instance.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     
    public void QuitToMainMenu() {
        DestroyGameSession();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLastSave() {
        DestroyGameSession();
        SceneManager.LoadScene("LoadingScene");
    }

    private static void DestroyGameSession() {
        Destroy(GameManager.instance.gameObject);
        Destroy(Player.instance.gameObject);
        Destroy(MenuManager.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);
    }

    public void QuitGame() {
        Debug.Log("You`ve quit the game!", gameObject);
        Application.Quit();
    }
}
