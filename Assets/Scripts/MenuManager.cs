using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;
    private Animator animator;

    public static MenuManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeImage()
    {
        animator = imageToFade.GetComponent<Animator>();
        animator.SetTrigger("StartFading");
    }
}
