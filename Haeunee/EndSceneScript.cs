using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;

public class EndSceneScript : MonoBehaviour
{
    int result;
    public GameObject winText;
    public GameObject loseText;
    public Text winningRate;
    WebServerScript server;
    string winning;

    void Start()
    {
        server = GameObject.Find("WebServer").GetComponent<WebServerScript>();
        GameObject.Destroy(GameObject.Find("itemBtnCanvas"));
        GameObject.Destroy(GameObject.Find("SocketServer"));
        result = SocketServer.SingleTonServ().GetResult();
        if (result == (int)eRESULT.em_WIN)
        {
            loseText.SetActive(false);
            winning = ResultSave("win");
        }
        else if (result == (int)eRESULT.em_LOSE)
        {
            winText.SetActive(false);
            winning = ResultSave("lose");
        }
        winningRate.text = server.nick + "님의 승률 : " + winning;
    }

    public void HomeBtn()
    {
        SceneManager.LoadScene("WaitScene");
    }

    string ResultSave(string result)
    {
        string Url = "http://localhost:10000/BattleEnd";
        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=" + result);
        sendInfo.Append("&nick=" + server.nick);
        return server.ConnectServer(Url, sendInfo);
    }    
}
