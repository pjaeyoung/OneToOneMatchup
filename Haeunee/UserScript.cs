using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

//웹서버 사용, 유저 로그인, 회원가입 등 
public class UserScript : MonoBehaviour {
    public InputField nickInput; //닉네임 입력
    public InputField passInput; //비밀번호 입력
    public GameObject overlapWin; //중복 알림화면
    public GameObject succWin; //회원가입 성공 알림 화면
    public GameObject failWin; //로그인 실패 알림 화면
    GameObject server; //웹서버 오브젝트
    WebServerScript web; //웹 서버 스크립트
    public string nick; //닉네임
    float winTime; //알림 화면 떠 있는 시간

    private void Start()
    {
        server = GameObject.Find("WebServer");
        web = server.GetComponent<WebServerScript>();
        passInput.contentType = InputField.ContentType.Password; //비밀번호 창에 별표 띄우기
    }

    private void Update()
    {
        if(overlapWin.activeSelf==true||succWin.activeSelf==true||failWin.activeSelf==true)
        { //켜져있는 창 시간지나면 닫기
            winTime += Time.deltaTime;
            if(winTime>=3)
            {
                winTime = 0;
                overlapWin.SetActive(false);
                succWin.SetActive(false);
                failWin.SetActive(false);
            }
        }
    }

    public void SignUp() //회원가입
    {
        string respJson = ConnectServer("http://192.168.0.22:10000/SignUp");
        Debug.Log(respJson);

        if(respJson.Equals("succ"))
        {
            succWin.SetActive(true);
        }
        else if(respJson.Equals("overlap"))
        {
            overlapWin.SetActive(true);
        }
    }

    public void Login() //로그인
    {
        string respJson = ConnectServer("http://192.168.0.22:10000/SignIn");
        Debug.Log(respJson);

        if (respJson.Equals("login"))
        {
            web.nick = nick;
            SceneManager.LoadScene("WaitScene");
        }
        else if (respJson.Equals("fail"))
        {
            failWin.SetActive(true);
        }
    }

    string ConnectServer(string Url) //서버에 정보 보내기
    {
        nick = nickInput.text;
        string password = passInput.text;
        nickInput.text = "";
        passInput.text = "";

        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("nick=" + nick);
        sendInfo.Append("&password=" + password);

        return web.ConnectServer(Url, sendInfo);
    }
}
