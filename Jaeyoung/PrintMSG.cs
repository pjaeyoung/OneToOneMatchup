using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PrintMSG : MonoBehaviour
{
    public GameObject YesBtn;
    float winActTime = 0f; //MSG창 열리는 시간 측정 

    private void Start()
    {
        if (YesBtn.activeSelf)
            YesBtn.GetComponent<Image>().enabled = true;
    }

    private void Update()
    {
        winActTime += Time.deltaTime;
        if(!Input.GetKeyDown(KeyCode.Escape) //ESC를 눌렀거나 EXitBtn클릭시 예외
            || EventSystem.current.currentSelectedGameObject != GameObject.Find("GameMgr/ExitBtn"))
        {
            if (winActTime >= 1.5f && !YesBtn.activeSelf)
            {
                winActTime = 0f;
                gameObject.SetActive(false);
            }
            
        }
    }

    public void print(string msg)
    {
        GetComponentInChildren<Text>().text = msg;
    }
}
