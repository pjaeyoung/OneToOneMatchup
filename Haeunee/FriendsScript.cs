using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine.UI;
using UnityEngine.EventSystems;

enum Friend
{
    em_LIST=1,
    em_SEARCH,
    em_REQUEST,
}

public class FriendsScript : MonoBehaviour {
    string nick; //유저 닉네임
    GameObject webServ; //웹 서버 연결하는 스크립트를 가진 오브젝트
    WebServerScript webScript; //웹서버 연결 스크립트
    public GameObject friendBtnObj; //등록된 친구를 조회해주고 선택할 버튼
    public GameObject friendScroll; //친구 조회하는 스크롤
    Button friendBtn; //등록된 친구를 선택하기 위해 버튼으로 형성
    Text friendText; //친구 이름을 변경시키기 위해 텍스트 가져옴
    bool friendListOn = false; //친구리스트가 켜져 있는지 체크
    public GameObject delWin;
    float winTime = 0.0f;
    GameObject nowBtnObj;

    string friendName;

    // Use this for initialization
    void Start () {
        webServ = GameObject.Find("WebServer");
        webScript = webServ.GetComponent<WebServerScript>();
        nick = webScript.nick;
        friendBtn = friendBtnObj.GetComponent<Button>();
        friendText = friendBtn.GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
		if(delWin.activeSelf==true)//알림창 시간따라 닫기
        {
            winTime += Time.deltaTime;
            if(winTime>=2)
            {
                delWin.SetActive(false);
                winTime = 0;
            }
        }
	}

    public void FriendBtnClick()
    {
        if(friendListOn==false) //친구 리스트 불러오기
        {
            int y = -25;
            friendListOn = true;

            StringBuilder sendInfo = new StringBuilder();
            sendInfo.Append("flag=list");
            sendInfo.Append("&nick=" + nick);
            string url = "http://localhost:10000/Friends";

            string respData = webScript.ConnectServer(url, sendInfo);
            string[] friendList = respData.Split(',');

            int friendLen = friendList.Length;
            for (int i = 0; i < friendLen; i++)
            {
                Vector3 chatPos = new Vector3(5, y, 0);
                Quaternion q = new Quaternion(0, 0, 0, 0);
                friendText.text = friendList[i];
                GameObject tmp = Instantiate(friendBtnObj, Vector3.zero, q, friendScroll.transform);
                tmp.transform.localPosition = chatPos;
                Button tmpBtn = tmp.GetComponent<Button>();
                tmpBtn.onClick.AddListener(FriendNameClick);
                y -= 20;
                Debug.Log(friendList[i]);
            }
        }
    }

    public void FriendList()
    {
       
    }

    public void FriendDelete() //친구 삭제
    {
        Button nowBtn = nowBtnObj.GetComponent<Button>();
        Text btnText = nowBtn.GetComponentInChildren<Text>();
        friendName = btnText.text.ToString();

        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=del");
        sendInfo.Append("&nick=" + nick);
        sendInfo.Append("&friend=" + friendName);
        string url = "http://localhost:10000/Friends";

        string respData = webScript.ConnectServer(url, sendInfo);
        if(respData=="succ")
        {
            delWin.SetActive(true);
            nowBtnObj.SetActive(false);
        }
    }

    public void FriendNameClick()
    {
        nowBtnObj = EventSystem.current.currentSelectedGameObject;
    }

    public void SearchBtnClick()
    {

    }

    public void FriendSearch()
    {

    }

    public void FriendRequest()
    {

    }
}
