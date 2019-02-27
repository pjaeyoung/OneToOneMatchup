using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneScript : MonoBehaviour
{
    int result;
    public GameObject winText;
    public GameObject loseText;

    void Start()
    {
        GameObject.Destroy(GameObject.Find("itemBtnCanvas"));
        GameObject.Destroy(GameObject.Find("SocketServer"));
        result = SocketServer.SingleTonServ().GetResult();
        if (result == (int)eRESULT.em_WIN)
            loseText.SetActive(false);
        if (result == (int)eRESULT.em_LOSE)
            winText.SetActive(false);
    }

    public void HomeBtn()
    {
        SceneManager.LoadScene("WaitScene");
    }
}
