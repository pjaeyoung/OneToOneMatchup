using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class exitBtn : MonoBehaviour
{
    public RawImage exitMSG;
    public PlayerCntrl_itemField playerCntrl;

    public void showMSG()
    {
        exitMSG.gameObject.SetActive(true);
    }

    public void ExitOk()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
}

    public void ExitNo()
    {
        playerCntrl.enabled = true;
        exitMSG.gameObject.SetActive(false);
    }

    public void LogOutOk()
    {
        sLogout logout = new sLogout((int)eMSG.em_LOGOUT);
        SocketServer.SingleTonServ().SendMsg(logout);
        SceneManager.LoadScene("LoginScene");
    }

    public void LogOutNo()
    {
        exitMSG.gameObject.SetActive(false);
    }
}
