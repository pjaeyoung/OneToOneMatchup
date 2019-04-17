using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ExitBtn : MonoBehaviour
{
    GameObject MSGWin;
    public GameObject block;
    PlayerScript s_Player;
    SpawnScript s_spawn;
    bool isLogout = false;

    private void Awake()
    {
        MSGWin = GameObject.Find("GameMgr").transform.GetChild(4).gameObject;
    }

    private void Update()
    {
        if (isLogout)
        {
            isLogout = false;
            string text = "로그아웃되셨습니다.";
            if (!MSGWin.activeSelf)
                MSGWin.SetActive(true);
            MSGWin.GetComponent<PrintMSG>().print(text);
            MSGWin.transform.GetChild(1).GetComponent<Image>().enabled = false;
            StartCoroutine(onDelay());
        }
    }

    public void showMSG()
    {
        string name = SceneManager.GetActiveScene().name;
        string msg = "";
        if (name == "LoginScene")
            msg = "게임을 종료하시겠습니까?";
        else if (name == "WaitScene")
            msg = "로그아웃하시겠습니까?";
        else if (name == "GameScene")
        {
            s_Player = SocketServer.SingleTonServ().NowPlayerScript().GetComponent<PlayerScript>();
            s_Player.enabled = false;
            msg = "항복하시겠습니까?";
        }
        MSGWin.SetActive(true);
        MSGWin.GetComponent<PrintMSG>().print(msg);
        MSGWin.transform.GetChild(1).gameObject.SetActive(true);
        block.SetActive(true);
    }

    public void YesBtn()
    {
        string name = SceneManager.GetActiveScene().name;
        if (name == "LoginScene")
            ExitOk();
        else if(name == "WaitScene" && MSGWin.GetComponentInChildren<Text>().text == "로그아웃하시겠습니까?")
            LogOutOk();
        else if(name == "GameScene")
        {
            s_Player = SocketServer.SingleTonServ().NowPlayerScript().GetComponent<PlayerScript>();
            s_Player.enabled = true;
            sEnd end = new sEnd((int)eMSG.em_END);
            SocketServer.SingleTonServ().SendMsg(end);
            Debug.Log("END");
            //서버에 항복 메세지 전달
            //나와 상대방 모두 대기 화면으로 바꾸기 
        }
    }

    public void NoBtn()
    {
        string name = SceneManager.GetActiveScene().name;
        if (name == "LoginScene")
            ExitNo();
        else if(name == "WaitScene")
            LogOutNo();
        else if(name == "GameScene")
        {
            s_Player = SocketServer.SingleTonServ().NowPlayerScript().GetComponent<PlayerScript>();
            s_Player.enabled = true;
            block.SetActive(false);
            MSGWin.SetActive(false);
        }
    }

    void ExitOk() //게임창 닫기
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
}

    void ExitNo() //게임창 닫기 취소 
    {
        MSGWin.gameObject.SetActive(false);
        if (block == null)
            Debug.Log("null");
        block.SetActive(false);
    }

    void LogOutOk() //LoginScene으로 돌아가기 
    {
        sLogout logout = new sLogout((int)eMSG.em_LOGOUT);
        SocketServer.SingleTonServ().SendMsg(logout);
        block.SetActive(false);
        isLogout = true;
    }

    void LogOutNo() //LoginScene 돌아가기 취소 
    {
        block.SetActive(false);
        MSGWin.SetActive(false);
    }

    IEnumerator onDelay() //LoginScene으로 돌아가기 전 딜레이 :  GameMgr, GameMgr2 삭제 
    {
        yield return new WaitForSeconds(1.0f);
        loading.LoadScene("LoginScene");
        
        GameObject[] dontDestroy = GameObject.FindGameObjectsWithTag("dontDestroy");
        int len = dontDestroy.Length;
        for (int i = 0; i < len; i++)
            Destroy(dontDestroy[i]);
    }
}
