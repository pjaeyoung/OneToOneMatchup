﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

//웹서버 사용, 유저 로그인, 회원가입 등 
public class UserScript : MonoBehaviour
{
    public InputField nickInput; //닉네임 입력
    public InputField passInput; //비밀번호 입력
    public GameObject MSGWin; //메세지 전달 창
    public GameObject SearchWin; //닉네임, 비밀번호 찾기 창 
    GameObject server; //웹서버 오브젝트
    WebServerScript web; //웹 서버 스크립트
    public string nick; //닉네임
    float winTime; //알림 화면 떠 있는 시간
    bool loginResult = false;
    int loginSucc = 0;

    public GameObject signUpWin;
    public InputField signUpNick;
    public InputField signUpPass;
    public InputField signUpPhone;

    int tuto = -1;
    public GameObject tutorialWin; //튜토리얼 진행여부 묻는 화면 
    public GameObject YesBtn;
    public GameObject NoBtn;
    public GameObject block;

    private void Start()
    {
        SocketServer.SingleTonServ().GetUserScript(this);
        server = GameObject.Find("WebServer");
        web = server.GetComponent<WebServerScript>();
        passInput.contentType = InputField.ContentType.Password; //비밀번호 창에 별표 띄우기
    }

    private void Update()
    {
        if (MSGWin.activeSelf && !SearchWin.activeSelf)
        { //켜져있는 창 시간지나면 닫기
            winTime += Time.deltaTime;
            if (winTime >= 0.8f)
            {
                winTime = 0;
                MSGWin.SetActive(false);
            }
        }

        if (loginResult) //중복체크 결과에 따른 반응 
        {
            loginResult = false;
            if (loginSucc == 0) //로그인 성공
            {
                web.nick = nick;
                GameObject.Find("SoundMgr").gameObject.transform.GetChild(3).GetChild(5).gameObject.SetActive(true);

                if (tuto == 0)
                    tutorialWin.SetActive(true);
                if (tuto == 1)
                    loading.LoadScene("WaitScene");
            }
            else if (loginSucc == 1) //실패
            {
                MSGWin.GetComponentInChildren<Text>().text = "이미 로그인 중입니다.";
                MSGWin.SetActive(true);
            }
        }

        if (tutorialWin.activeSelf == true)
        {
            if (EventSystem.current.currentSelectedGameObject == YesBtn)
            {
                block.SetActive(false);
                StringBuilder sendInfo = new StringBuilder();
                sendInfo.Append("nick=" + nick);
                sendInfo.Append("&tuto=" + tuto);
                web.ConnectServer("http://192.168.0.22:10000/Tuto", sendInfo);
                loading.LoadScene("TutorialScene");
            }
            else if (EventSystem.current.currentSelectedGameObject == NoBtn)
            {
                block.SetActive(false);
                StringBuilder sendInfo = new StringBuilder();
                sendInfo.Append("nick=" + nick);
                sendInfo.Append("&tuto=" + tuto);
                web.ConnectServer("http://192.168.0.22:10000/Tuto", sendInfo);
                loading.LoadScene("WaitScene");
            }
        }
    }

    public void SignUp() //회원가입
    {
        signUpWin.SetActive(true);
    }

    public void SignUpClick()
    {
        string phoneNum = signUpPhone.text;
        int phoneLen = phoneNum.Length;
        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("nick=" + signUpNick.text);
        sendInfo.Append("&password=" + signUpPass.text);
        sendInfo.Append("&phone=" + phoneNum);
        signUpNick.text = "";
        signUpPass.text = "";
        signUpPhone.text = "";

        if (phoneLen<10)
        {
            MSGWin.GetComponentInChildren<Text>().text = "올바른 전화번호를 입력해주세요.";
            MSGWin.SetActive(true);
        }
        else
        {
            for(int i=0;i<phoneLen; i++)
            {
                if(phoneNum[i] =='0'|| phoneNum[i] == '1'|| phoneNum[i] == '2'|| phoneNum[i] == '3'|| phoneNum[i] == '4'
                    || phoneNum[i] == '5'|| phoneNum[i] == '6'|| phoneNum[i] == '7'|| phoneNum[i] == '8' || phoneNum[i] == '9')
                    continue;
                else
                {
                    MSGWin.GetComponentInChildren<Text>().text = "올바른 전화번호를 입력해주세요.";
                    MSGWin.SetActive(true);
                    return;
                }
            }

            string respJson = web.ConnectServer("http://192.168.0.22:10000/SignUp", sendInfo);
            Debug.Log(respJson);

            if (respJson.Equals("succ"))
            {
                MSGWin.GetComponentInChildren<Text>().text = "회원가입 되었습니다.";
                MSGWin.SetActive(true);
            }
            else if (respJson.Equals("overlap"))
            {
                MSGWin.GetComponentInChildren<Text>().text = "이미 등록된 닉네임입니다.";
                MSGWin.SetActive(true);
            }
            else if (respJson.Equals("phone overlap"))
            {
                MSGWin.GetComponentInChildren<Text>().text = "이미 입력된 전화번호입니다. ";
                MSGWin.SetActive(true);
            }
            else if (respJson.Equals("fail"))
            {
                MSGWin.GetComponentInChildren<Text>().text = "회원정보를 다시 확인해주세요.";
                MSGWin.SetActive(true);
            }
            signUpWin.SetActive(false);
        }
    }

    public void CloseBtn()
    {
        GameObject nowBtnObj = EventSystem.current.currentSelectedGameObject;
        nowBtnObj.transform.parent.gameObject.SetActive(false);
    }

    public void SearchWinOpen()
    {
        SearchWin.SetActive(true);
    }

    public void Login() //로그인
    {
        string respJson = ConnectServer("http://192.168.0.22:10000/SignIn");
        Debug.Log(respJson);
        string[] loginResult = respJson.Split(',');
        if (loginResult[0].Equals("login")) //존재하는 아이디와 패스워드일 때
        {
            sLogin login = new sLogin(nick.ToCharArray(), 1);
            tuto = int.Parse(loginResult[1]);
            SocketServer.SingleTonServ().SendMsg(login); //소켓 서버에서 이미 로그인 된 아이디는 아닌지 확인
        }
        else if (respJson.Equals("fail"))
        {
            MSGWin.GetComponentInChildren<Text>().text = "회원정보를 다시 확인해주세요.";
            MSGWin.SetActive(true);
        }
    }

    string ConnectServer(string Url) //서버에 정보 보내기
    {
        nick = nickInput.text;
        bool nickOk = nick.Contains(",") || nick.Contains("&") || nick.Contains("^") || nick.Contains(" ") || nick.Contains("!");
        string password = passInput.text;
        if (nick != "" && password != "" && nickOk == false)
        {
            nickInput.text = "";
            passInput.text = "";

            StringBuilder sendInfo = new StringBuilder();
            sendInfo.Append("nick=" + nick);
            sendInfo.Append("&password=" + password);

            return web.ConnectServer(Url, sendInfo);
        }
        else
            return "fail";
    }

    public void LoginResult(char[] loginNick, int succ) //소켓서버에서 로그인 중복체크 결과
    {
        nick = "";
        int i = 0;
        while (loginNick[i] != '\0')
        {
            nick += loginNick[i];
            i++;
        }
        loginSucc = succ; //중복이 아니라면 0 중복이면 1
        loginResult = true;
    }
}
