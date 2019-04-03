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
        exitMSG.gameObject.transform.parent.gameObject.SetActive(true);
        exitMSG.gameObject.SetActive(true);
        block.SetActive(true);
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
        exitMSG.gameObject.SetActive(false);
        if (block == null)
            Debug.Log("null");
        block.SetActive(false);
    }

    public void LogOutOk()
    {
        sLogout logout = new sLogout((int)eMSG.em_LOGOUT);
        SocketServer.SingleTonServ().SendMsg(logout);
        GameObject[] dontDestroy = GameObject.FindGameObjectsWithTag("dontDestroy");
        int len = dontDestroy.Length;
        for (int i = 0; i < len; i++)
            Destroy(dontDestroy[i]);
        block.SetActive(false);
        StartCoroutine(OnDelay());
    }

    public void LogOutNo()
    {
        block.SetActive(false);
        exitMSG.gameObject.SetActive(false);
    }

    IEnumerator OnDelay()
    {
        yield return new WaitForSeconds(0.5f);
        loading.LoadScene("LoginScene");
    }
}
