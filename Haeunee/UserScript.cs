using System.Collections;
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
    public GameObject overlapWin; //중복 알림화면
    public GameObject succWin; //회원가입 성공 알림 화면
    public GameObject failWin; //로그인 실패 알림 화면
    public GameObject alreadyLogin;
    GameObject server; //웹서버 오브젝트
    WebServerScript web; //웹 서버 스크립트
    public string nick; //닉네임
    float winTime; //알림 화면 떠 있는 시간
    bool loginResult = false;
    int loginSucc = 0;

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
        if (overlapWin.activeSelf || succWin.activeSelf || failWin.activeSelf || alreadyLogin.activeSelf)
        { //켜져있는 창 시간지나면 닫기
            winTime += Time.deltaTime;
            if (winTime >= 1)
            {
                winTime = 0;
                overlapWin.SetActive(false);
                succWin.SetActive(false);
                failWin.SetActive(false);
                alreadyLogin.SetActive(false);
            }
        }

        if (loginResult) //중복체크 결과에 따른 반응 
        {
            loginResult = false;
            if (loginSucc == 0) //로그인 성공
            {
                web.nick = nick;
                if (tuto == 0)
                {
                    tutorialWin.SetActive(true);
                    StringBuilder sendInfo = new StringBuilder();
                    sendInfo.Append("nick=" + nick);
                    sendInfo.Append("&tuto=" + tuto);
                    web.ConnectServer("http://192.168.0.22:10000/Tuto", sendInfo);
                }
                if (tuto == 1)
                    SceneManager.LoadScene("WaitScene");
            }
            else if (loginSucc == 1) //실패
            {
                alreadyLogin.SetActive(true);
            }
        }

        if (tutorialWin.activeSelf == true)
        {
            if (EventSystem.current.currentSelectedGameObject == YesBtn)
            {
                block.SetActive(false);
                loading.LoadScene("TutorialScene");
            }
            else if (EventSystem.current.currentSelectedGameObject == NoBtn)
            {
                block.SetActive(false);
                loading.LoadScene("WaitScene");
            }
        }
    }

    public void SignUp() //회원가입
    {
        string respJson = ConnectServer("http://192.168.0.22:10000/SignUp");
        Debug.Log(respJson);

        if (respJson.Equals("succ"))
        {
            succWin.SetActive(true);
        }
        else if (respJson.Equals("overlap"))
        {
            overlapWin.SetActive(true);
        }
        else if (respJson.Equals("fail"))
        {
            failWin.SetActive(true);
        }
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
            failWin.SetActive(true);
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
