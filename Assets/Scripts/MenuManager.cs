using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;
    private Animator animator;

    [SerializeField] GameObject menu;


    public static MenuManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (menu.activeInHierarchy) {
                menu.SetActive(false);
                GameManager.instance.gameMenuOpened = false;
            } else {
                menu.SetActive(true);
                GameManager.instance.gameMenuOpened = true;
            }
        }
    }

    public void FadeImage()
    {
        animator = imageToFade.GetComponent<Animator>();
        animator.SetTrigger("StartFading");
    }
}
