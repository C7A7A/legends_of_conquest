using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScenes : MonoBehaviour
{
    [SerializeField] float waitToLoadTime;


    // Start is called before the first frame update
    void Start()
    {
        if (waitToLoadTime > 0) {
            StartCoroutine(LoadScene());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LoadScene() {
        yield return new WaitForSeconds(waitToLoadTime);

        SceneManager.LoadScene(PlayerPrefs.GetString("Current_Scene"));
        GameManager.instance.LoadData();
        QuestManager.instance.LoadQuestData();
    }
}
