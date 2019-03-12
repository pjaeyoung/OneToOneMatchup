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
    GameObject enemy;
    GameObject player;
    AnimationController playerAni;
    AnimationController enemyAni;

    void Start()
    {
        PlayerScript playerScript = SocketServer.SingleTonServ().NowPlayerScript();
        player = playerScript.transform.gameObject;
        player.transform.rotation = Quaternion.Euler(0, 180, 0);
        playerAni = player.GetComponent<AnimationController>();
        playerScript.enabled = false;
        player.GetComponentInChildren<Camera>().enabled = false;
        Camera.main.enabled = true;

        EnemyScript enemyScript = SocketServer.SingleTonServ().NowEnemyScript();
        enemy = enemyScript.transform.gameObject;
        enemy.transform.rotation = Quaternion.Euler(0, 180, 0);
        enemyAni = enemy.GetComponent<AnimationController>();
        enemyScript.enabled = false;
        enemy.transform.Find("Canvas").gameObject.SetActive(false);

        server = GameObject.Find("WebServer").GetComponent<WebServerScript>();
        GameObject.Destroy(GameObject.Find("itemBtnCanvas"));
        GameObject.Destroy(GameObject.Find("SocketServer"));
        result = SocketServer.SingleTonServ().GetResult();
        if (result == (int)eRESULT.em_WIN)
        {
            player.transform.position = new Vector3(-3, -2, 13);
            enemy.transform.position = new Vector3(3, -2, 13);
            enemyAni.PlayAnimation("Idle");
            StartCoroutine(DeathAni(result));
            loseText.SetActive(false);
            winning = ResultSave("win");
        }
        else if (result == (int)eRESULT.em_LOSE)
        {
            player.transform.position = new Vector3(3, -2, 13);
            enemy.transform.position = new Vector3(-3, -2, 13);
            playerAni.PlayAnimation("Idle");
            StartCoroutine(DeathAni(result));
            winText.SetActive(false);
            winning = ResultSave("lose");
        }
        int win = Mathf.FloorToInt(float.Parse(winning));
        winningRate.text = server.nick + "님의 승률 : " + win;
    }
    
    public void HomeBtn()
    {
        GameObject.Destroy(player.transform.parent.gameObject);
        GameObject.Destroy(enemy.transform.parent.gameObject);
        SceneManager.LoadScene("WaitScene");
    }

    string ResultSave(string result)
    {
        string Url = "http://192.168.0.22:10000/BattleEnd";
        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=" + result);
        sendInfo.Append("&nick=" + server.nick);
        return server.ConnectServer(Url, sendInfo);
    }    

    IEnumerator DeathAni(int res)
    {
        yield return new WaitForSeconds(1.0f);
        if (result == (int)eRESULT.em_WIN)
            enemyAni.PlayDeath("Death");
        else if (result == (int)eRESULT.em_LOSE)
            playerAni.PlayDeath("Death");
    }
}
