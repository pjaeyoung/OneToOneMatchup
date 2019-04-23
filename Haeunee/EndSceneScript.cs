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

    private void Awake()
    {
        if(GameObject.Find("GameMgr/MSGWin") != null && GameObject.Find("GameMgr/MSGWin").activeSelf)
            GameObject.Find("GameMgr/MSGWin").SetActive(false);
    }

    void Start()
    { //플레이어 캐릭터와 적 캐릭터만 나오게 하기
        PlayerScript playerScript = SocketServer.SingleTonServ().NowPlayerScript();
        player = playerScript.transform.gameObject;
        player.transform.rotation = Quaternion.Euler(0, 180, 0);
        playerAni = player.GetComponent<AnimationController>();
        playerScript.enabled = false;
        playerAni.PlayAtkDmg("Idle");
        player.GetComponentInChildren<Camera>().enabled = false;
        Camera.main.enabled = true;

        EnemyScript enemyScript = SocketServer.SingleTonServ().NowEnemyScript();
        enemy = enemyScript.transform.gameObject;
        enemy.transform.rotation = Quaternion.Euler(0, 180, 0);
        enemyAni = enemy.GetComponent<AnimationController>();
        enemyScript.enabled = false;
        enemy.transform.Find("Canvas").gameObject.SetActive(false);
        enemy.GetComponent<Rigidbody>().isKinematic = false;
        enemyAni.PlayAtkDmg("Idle");

        server = GameObject.Find("WebServer").GetComponent<WebServerScript>();
        GameObject.Destroy(GameObject.Find("itemBtnCanvas"));
        result = SocketServer.SingleTonServ().GetResult();
        //결과에 따라 맞는 위치에 배치하기
        if (result == (int)eRESULT.em_WIN)
        {
            player.transform.position = new Vector3(-3, -2, 13);
            enemy.transform.position = new Vector3(3, -2, 13);
            StartCoroutine(DeathAni(result));
            loseText.SetActive(false);
            winning = ResultSave("win");
        }
        else if (result == (int)eRESULT.em_LOSE)
        {
            player.transform.position = new Vector3(3, -2, 13);
            enemy.transform.position = new Vector3(-3, -2, 13);
            StartCoroutine(DeathAni(result));
            winText.SetActive(false);
            winning = ResultSave("lose"); 
        }
        //승률 표시
        int win = Mathf.FloorToInt(float.Parse(winning));
        winningRate.text = server.nick + "님의 승률 : " + win;
    }
    
    public void HomeBtn()
    {
        BgmController sound = GameObject.Find("GameMgr").GetComponent<BgmController>();
        sound.ChangeBgm();
        StartCoroutine(OutDelay());        
    }

    string ResultSave(string result) //웹서버와 연결, 승률 계산하여 가져오기
    {
        string Url = "BattleEnd";
        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=" + result);
        sendInfo.Append("&nick=" + server.nick);
        return server.ConnectServer(Url, sendInfo);
    }    

    IEnumerator DeathAni(int res) //진 캐릭터 죽는 애니메이션 재생하기
    {
        yield return new WaitForSeconds(1.0f);
        if (result == (int)eRESULT.em_WIN)
            enemyAni.PlayDeath("Death");
        else if (result == (int)eRESULT.em_LOSE)
            playerAni.PlayDeath("Death");
    }

    IEnumerator OutDelay() 
    {
        GameObject.Destroy(player.transform.parent.gameObject);
        GameObject.Destroy(enemy.transform.parent.gameObject);
        GameObject.Destroy(GameObject.Find("itemBtnCanvas"));
        GameObject.Destroy(GameObject.Find("GameMgr2"));
        BgmController sound = GameObject.Find("GameMgr").GetComponent<BgmController>();
        sound.ChangeBgm();
        yield return new WaitForSeconds(1.0f);
        loading.LoadScene("WaitScene");
    }
}
