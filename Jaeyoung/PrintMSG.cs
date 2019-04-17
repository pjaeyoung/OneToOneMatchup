using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
        //ESC를 눌렀거나 ExitBtn 혹은 searchBtn 클릭시 예외
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("GameMgr/ExitBtn")
            || SceneManager.GetActiveScene().name == "WaitScene" &&
            EventSystem.current.currentSelectedGameObject == GameObject.Find("WinCanvas").transform.GetChild(1).GetChild(4).GetChild(0))
            return;
        else if(!Input.GetKeyDown(KeyCode.Escape)) 
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
