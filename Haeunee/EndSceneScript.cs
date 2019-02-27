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
        result = SocketServer.SingleTonServ().GetResult();
        if (result == (int)eRESULT.em_WIN)
            loseText.SetActive(false);
        if (result == (int)eRESULT.em_WIN)
            winText.SetActive(false);
    }

    public void HomeBtn()
    {
        SceneManager.LoadScene("WaitScene");
    }
}
