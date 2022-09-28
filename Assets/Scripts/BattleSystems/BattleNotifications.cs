using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleNotifications : MonoBehaviour
{
    [SerializeField] float timeAlive;
    [SerializeField] TextMeshProUGUI textNotice;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text) {
        textNotice.text = text;
    }

    public void Activate() {
        gameObject.SetActive(true);
        StartCoroutine(MakeNoticeDisappear());
    }

    private IEnumerator MakeNoticeDisappear() {
        yield return new WaitForSeconds(timeAlive);
        gameObject.SetActive(false);
    }
}
