using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class RankingScript : MonoBehaviour
{
    string nick; //유저 닉네임
    GameObject webServ; //웹 서버 연결하는 스크립트를 가진 오브젝트
    WebServerScript webScript; //웹서버 연결 스크립트
    List<GameObject> tmpObj;
    public GameObject rankUser;
    public GameObject rankScroll;
    public GameObject userInfo;
    public GameObject rankingWin;

    private void Start()
    {
        tmpObj = new List<GameObject>();
        webServ = GameObject.Find("WebServer");
        webScript = webServ.GetComponent<WebServerScript>();
        nick = webScript.nick;       
    }

    public void RankBtnClick()
    {
        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=reqlist");
        sendInfo.Append("&nick=" + nick);
        string url = "http://192.168.0.22:10000/Ranking";
        string respData = webScript.ConnectServer(url, sendInfo);
        int y = -25;
        string[] rankList = respData.Split(',');
        string[] nickList = new string[100];
        string[] scoreList = new string[100];
        int rankLen = rankList.Length;
        string userScore = rankList[0];
        int nickNum = 0;
        int scoreNum = 0;
        for (int i=1;i< rankLen;i++)
        {
            if(i%2 == 0)
            {
                scoreList[scoreNum] = rankList[i];
                scoreNum++;
            }
            else
            {
                nickList[nickNum] = rankList[i];
                nickNum++;
            }
        }

        int userRank = 0;
        for (int i = 0; i < rankLen/2; i++)
        {
            if (nickList[i] == nick)
                userRank = i + 1;
            Vector3 chatPos = new Vector3(5, y, 0);
            Quaternion q = new Quaternion(0, 0, 0, 0);
            Text rankText = rankUser.GetComponentInChildren<Text>();
            rankText.text = (i+1) + "          " + nickList[i]+"                    "+scoreList[i];
            GameObject tmp = Instantiate(rankUser, Vector3.zero, q, rankScroll.transform);
            tmpObj.Add(tmp);
            tmp.transform.localPosition = chatPos;
            y -= 20;
        }
        if(userRank==0)
            userInfo.GetComponentInChildren<Text>().text = "내 정보\n닉네임 : " + nick + "          순위 : 100위권 밖          점수 : " + userScore;
        else
            userInfo.GetComponentInChildren<Text>().text = "내 정보\n닉네임 : " + nick + "          순위 : " +userRank+ "          점수 : " + userScore;
        rankingWin.SetActive(true);
    }

    public void CloseBtnClick()
    {
        rankingWin.SetActive(false);
        for (int i = 0; i < tmpObj.Count; i++)
        {
            GameObject.Destroy(tmpObj[i]);
        }
        tmpObj.Clear();
    }

}
