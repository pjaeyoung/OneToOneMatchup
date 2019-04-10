using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class UserSearch : MonoBehaviour
{
    public GameObject nickSearchWin;
    public GameObject passSearchWin;
    public InputField nickPhoneInput;
    public InputField passPhoneInput;
    public InputField passNickInput;
    public GameObject nickSearchSuccWin;
    public GameObject nickSearchFailWin;
    public GameObject passChangeWin;
    public GameObject passSearchFailWin;
    public InputField passChangeInput;
    public GameObject changeSuccWin;
    WebServerScript web;
    string userNick;
    float winTime = 0;

    void Start()
    {
        passChangeInput.contentType = InputField.ContentType.Password;
        web = GameObject.Find("WebServer").GetComponent<WebServerScript>();
    }
    
    void Update()
    {
        if(nickSearchFailWin.activeSelf|| passSearchFailWin.activeSelf|| changeSuccWin.activeSelf)
        {
            winTime += Time.deltaTime;
            if(winTime>=1.5f)
            {
                winTime = 0;
                nickSearchFailWin.SetActive(false);
                passSearchFailWin.SetActive(false);
                if(changeSuccWin.activeSelf)
                {
                    changeSuccWin.SetActive(false);
                    passChangeWin.SetActive(false);
                    passSearchWin.SetActive(false);
                }
            }
        }
    }

    public void NickSearchClick()
    {
        nickSearchWin.SetActive(true);
    }

    public void PassSearchClick()
    {
        passSearchWin.SetActive(true);
    }

    public void NickSearch()
    {
        string nickPhone = nickPhoneInput.text;
        nickPhoneInput.text = "";

        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=nick");
        sendInfo.Append("&phone=" + nickPhone);

        string url = "http://192.168.0.22:10000/UserSearch";
        string respData = web.ConnectServer(url, sendInfo);

        if(respData=="fail")
            nickSearchFailWin.SetActive(true);
        else
        {
            nickSearchSuccWin.GetComponentInChildren<Text>().text = "당신의 닉네임은 '" + respData + "' 입니다.";
            nickSearchSuccWin.SetActive(true);
        }
        
    }

    public void PassSearch()
    {
        string passPhone = passPhoneInput.text;
        string passNick = passNickInput.text;
        passPhoneInput.text = "";
        passNickInput.text = "";

        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=pass");
        sendInfo.Append("&phone=" + passPhone);
        sendInfo.Append("&nick=" + passNick);

        string url = "http://192.168.0.22:10000/UserSearch";
        string respData = web.ConnectServer(url, sendInfo);

        if(respData=="succ")
        {
            passChangeWin.SetActive(true);
            userNick = passNick;
        }
        else
            passSearchFailWin.SetActive(true);
    }

    public void ChangeBtnClick()
    {
        string changePass = passChangeInput.text;
        passChangeInput.text = "";

        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("nick=" + userNick);
        sendInfo.Append("&pass=" + changePass);

        string url = "http://192.168.0.22:10000/ChangePassword";
        string respData = web.ConnectServer(url, sendInfo);

        if (respData == "succ")
        {
            changeSuccWin.SetActive(true);
        }
    }
}
