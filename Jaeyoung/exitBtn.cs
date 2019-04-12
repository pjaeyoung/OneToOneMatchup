using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class exitBtn : MonoBehaviour
{
    public RawImage exitMSG;
    public GameObject block;

    public void showMSG()
    {
        string name = SceneManager.GetActiveScene().name;
        string msg = "";
        if (name == "LoginScene")
            msg = "게임을 종료하시겠습니까?";
        else
            msg = "로그아웃하시겠습니까?";
        exitMSG.GetComponentInChildren<Text>().text = msg; 
        exitMSG.gameObject.SetActive(true);
        exitMSG.transform.GetChild(1).gameObject.SetActive(true);
        block.SetActive(true);
    }

    public void YesBtn()
    {
        string name = SceneManager.GetActiveScene().name;
        if (name == "LoginScene")
            ExitOk();
        else
            LogOutOk();
    }

    public void NoBtn()
    {
        string name = SceneManager.GetActiveScene().name;
        if (name == "LoginScene")
            ExitNo();
        else
            LogOutNo();
    }

    void ExitOk()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
}

    void ExitNo()
    {
        exitMSG.gameObject.SetActive(false);
        if (block == null)
            Debug.Log("null");
        block.SetActive(false);
    }

    void LogOutOk()
    {
        sLogout logout = new sLogout((int)eMSG.em_LOGOUT);
        SocketServer.SingleTonServ().SendMsg(logout);
        GameObject[] dontDestroy = GameObject.FindGameObjectsWithTag("dontDestroy");
        int len = dontDestroy.Length;
        for (int i = 0; i < len; i++)
            Destroy(dontDestroy[i]);
        block.SetActive(false);
        loading.LoadScene("LoginScene");
    }

    void LogOutNo()
    {
        block.SetActive(false);
        exitMSG.gameObject.SetActive(false);
    }
}
