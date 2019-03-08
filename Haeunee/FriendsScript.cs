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
    public GameObject friendWin;
    public GameObject friendListWin;
    public GameObject searchWin;
    public InputField searchNickInput;
    public GameObject searchFailWin;
    public GameObject alreadyFriendWin;
    public GameObject reqSuccWin;
    public Text searchInfo;
    public GameObject requestListWin;
    public GameObject reqScroll;
    public GameObject acceptWin;
    public GameObject existImg;
    List<GameObject> tmpObj;
    float reqExist = 10;
    string[] savReqList;
    string btnText;
    public GameObject searchInfoImg;
    public GameObject refuseWin;

    string requestName;

    // Use this for initialization
    void Start () {
        tmpObj = new List<GameObject>();
        webServ = GameObject.Find("WebServer");
        webScript = webServ.GetComponent<WebServerScript>();
        nick = webScript.nick;
        friendBtn = friendBtnObj.GetComponent<Button>();
        friendText = friendBtn.GetComponentInChildren<Text>();

        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=reqlist");
        sendInfo.Append("&nick=" + nick);
        string url = "http://192.168.0.22:10000/Friends";
        string respData = webScript.ConnectServer(url, sendInfo);

        savReqList = respData.Split(',');

        if (respData != "no request")
        {
            existImg.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(delWin== true||searchFailWin==true
            || alreadyFriendWin==true|| reqSuccWin==true
            || acceptWin == true|| refuseWin==true)//알림창 시간따라 닫기
        {
            winTime += Time.deltaTime;
            if(winTime>=1.5f)
            {
                delWin.SetActive(false);
                searchFailWin.SetActive(false);
                alreadyFriendWin.SetActive(false);
                reqSuccWin.SetActive(false);
                acceptWin.SetActive(false);
                refuseWin.SetActive(false);
                winTime = 0;
            }
        }

        reqExist += Time.deltaTime;
        if(friendListOn == false && reqExist >= 10)
        {
            reqExist = 0;
            StringBuilder sendInfo = new StringBuilder();
            sendInfo.Append("flag=reqlist");
            sendInfo.Append("&nick=" + nick);
            string url = "http://192.168.0.22:10000/Friends";
            string respData = webScript.ConnectServer(url, sendInfo);

            if (respData != "no request")
            {
                string[] nowReqList = respData.Split(',');

                int reqLen = nowReqList.Length;
                int savLen = savReqList.Length;
                if(reqLen==savLen)
                {
                    for (int i = 0; i < reqLen; i++)
                    {
                        if (nowReqList[i] != savReqList[i])
                            existImg.SetActive(true);
                    }
                }
                else
                    existImg.SetActive(true);
            }
        }

    }

    public void FriendBtnClick()
    {
        existImg.SetActive(false);
        if (friendListOn == false) //친구 리스트 불러오기
        {
            friendWin.SetActive(true);
            friendListOn = true;
            FriendList();
        }
    }


    public void FriendList()
    {
        if(tmpObj != null)
        {
            int objLen = tmpObj.Count;
            for (int i = 0; i < objLen; i++)
            {
                GameObject.Destroy(tmpObj[i]);
            }
            tmpObj.Clear();
        }
        int y = -25;

        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=list");
        sendInfo.Append("&nick=" + nick);
        string url = "http://192.168.0.22:10000/Friends";

        string respData = webScript.ConnectServer(url, sendInfo);
        string[] friendList = respData.Split(',');

        int friendLen = friendList.Length;
        for (int i = 0; i < friendLen; i++)
        {
            Vector3 chatPos = new Vector3(5, y, 0);
            Quaternion q = new Quaternion(0, 0, 0, 0);
            friendText.text = friendList[i];
            GameObject tmp = Instantiate(friendBtnObj, Vector3.zero, q, friendScroll.transform);
            tmpObj.Add(tmp);
            tmp.transform.localPosition = chatPos;
            Button tmpBtn = tmp.GetComponent<Button>();
            tmpBtn.onClick.AddListener(FriendNameClick);
            y -= 20;
            Debug.Log(friendList[i]);
        }
        searchWin.SetActive(false);
        friendListWin.SetActive(true);
        requestListWin.SetActive(false);
    }

    public void FriendDelete() //친구 삭제
    {
        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=del");
        sendInfo.Append("&nick=" + nick);
        sendInfo.Append("&friend=" + btnText);
        string url = "http://192.168.0.22:10000/Friends";

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
        btnText = nowBtnObj.GetComponentInChildren<Text>().text.ToString();
        if (btnText == "no friend"||btnText=="no request")
            nowBtnObj = null;
        else
        {
            int btnArrSize = tmpObj.Count;
            for (int i = 0; i < btnArrSize; i++)
                tmpObj[i].GetComponent<Image>().color = Color.clear;
            nowBtnObj.GetComponent<Image>().color = Color.gray;
        }
    }

    public void SearchBtnClick()
    {
        string searchName = searchNickInput.text.ToString();
        searchNickInput.text = "";
        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=search");
        sendInfo.Append("&nick=" + nick);
        sendInfo.Append("&search=" + searchName);
        string url = "http://192.168.0.22:10000/Friends";
        string respData = webScript.ConnectServer(url, sendInfo);

        if(respData == "fail")
        {
            searchFailWin.SetActive(true);
        }
        else
        {
            searchInfo.text = "이름 : " + searchName + "\n점수 : " + respData;
            requestName = searchName;
            searchInfoImg.SetActive(true);
        }
    }

    public void FriendSearch()
    {
        searchWin.SetActive(true);
        friendListWin.SetActive(false);
        requestListWin.SetActive(false);
        if (tmpObj != null)
        {
            int objLen = tmpObj.Count;
            for (int i = 0; i < objLen; i++)
            {
                GameObject.Destroy(tmpObj[i]);
            }
            tmpObj.Clear();
        }
    }

    public void FriendRequest()
    {
        if (tmpObj != null)
        {
            int objLen = tmpObj.Count;
            for (int i = 0; i < objLen; i++)
            {
                GameObject.Destroy(tmpObj[i]);
            }
            tmpObj.Clear();
        }
        int y = 85;
        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=reqlist");
        sendInfo.Append("&nick=" + nick);
        string url = "http://192.168.0.22:10000/Friends";
        string respData = webScript.ConnectServer(url, sendInfo);

        string[] reqList = respData.Split(',');
        savReqList = reqList;

        int reqLen = reqList.Length;
        for (int i = 0; i < reqLen; i++)
        {
            Vector3 chatPos = new Vector3(-150, y, 0);
            Quaternion q = new Quaternion(0, 0, 0, 0);
            friendText.text = reqList[i];
            GameObject tmp = Instantiate(friendBtnObj, Vector3.zero, q, reqScroll.transform);
            tmpObj.Add(tmp);
            tmp.transform.localPosition = chatPos;
            Button tmpBtn = tmp.GetComponent<Button>();
            tmpBtn.onClick.AddListener(FriendNameClick);
            y -= 20;
            Debug.Log(reqList[i]);
        }
        requestListWin.SetActive(true);
        searchWin.SetActive(false);
        friendListWin.SetActive(false);
    }

    public void RequestBtnClick()
    {
        if(searchInfoImg==true)
        {
            StringBuilder sendInfo = new StringBuilder();
            sendInfo.Append("flag=request");
            sendInfo.Append("&nick=" + nick);
            sendInfo.Append("&request=" + requestName);
            string url = "http://192.168.0.22:10000/Friends";
            string respData = webScript.ConnectServer(url, sendInfo);

            if (respData == "already")
            {
                alreadyFriendWin.SetActive(true);
            }
            else if (respData == "succ")
            {
                reqSuccWin.SetActive(true);
            }
            searchInfoImg.SetActive(false);
        }
    }

    public void AcceptBtnClick()
    {
        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=accept");
        sendInfo.Append("&nick=" + nick);
        sendInfo.Append("&accept=" + btnText);
        string url = "http://192.168.0.22:10000/Friends";
        string respData = webScript.ConnectServer(url, sendInfo);

        if (respData == "succ")
        {
            int savLen = savReqList.Length;
            for (int i = 0; i < savLen; i++)
            {
                if (savReqList[i] == btnText)
                {
                    while(i + 1 < savLen)
                    {
                        savReqList[i] = savReqList[i + 1];
                        i++;
                    }
                }
            }
            acceptWin.SetActive(true);
            nowBtnObj.SetActive(false);
        }
    }

    public void RefuseBtnClick()
    {
        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=refuse");
        sendInfo.Append("&nick=" + nick);
        sendInfo.Append("&refuse=" + btnText);
        string url = "http://192.168.0.22:10000/Friends";
        string respData = webScript.ConnectServer(url, sendInfo);

        if (respData == "succ")
        {
            int savLen = savReqList.Length;
            for (int i = 0; i < savLen; i++)
            {
                if (savReqList[i] == btnText)
                {
                    while (i + 1 < savLen)
                    {
                        savReqList[i] = savReqList[i + 1];
                        i++;
                    }
                }
            }
            refuseWin.SetActive(true);
            nowBtnObj.SetActive(false);
        }
    }

    public void CloseBtnClick()
    {
        friendWin.SetActive(false);
        friendListOn = false;
    }
}
