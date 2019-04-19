﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class UserSearch : MonoBehaviour
{
    public GameObject nickSearchWin; //닉네임 찾기창
    public GameObject passSearchWin; //페스워드 변경 확인창
    public InputField nickPhoneInput; //닉네임 찾기창 - 전화번호 입력 
    public InputField passPhoneInput; //페스워드 변경 확인창 - 전화번호 입력 
    public InputField passNickInput; //페스워드 변경 확인창 - 닉네임 입력
    public GameObject passChangeWin; //페스워드 변경창 
    public InputField passChangeInput; //페스워드 변경창 - 변경할 페스워드 입력 
    public GameObject MSGWin; //메세지창 출력 
    public Image NickWinOpenBtn;
    public Image PassWinOpenBtn;
    Color activeColor;//활성화된 창의 버튼 색
    Color inActiveColor; //비활성화된 창의 버튼 색
    WebServerScript web;
    string userNick;
    float winTime = 0;
    bool IsClickSearchNickBtn = false; //예외처리: 유저의 닉네임 표시창은 closeBtn을 누를 경우에만 닫기

    private void Awake()
    {
        activeColor = new Color32(131, 39, 185, 255);
        inActiveColor = new Color32(151, 109, 176, 255);
    }

    void Start()
    {
        passChangeInput.contentType = InputField.ContentType.Password;
        web = GameObject.Find("WebServer").GetComponent<WebServerScript>();
    }
    
    void Update()
    {
        if(MSGWin.activeSelf && !IsClickSearchNickBtn)
        {
            winTime += Time.deltaTime;
            if (winTime >= 1.5f)
            {
                winTime = 0;
                MSGWin.SetActive(false);
            }
        }
    }

    public void NickSearchWinOpen()
    {
        NickWinOpenBtn.color = activeColor;
        PassWinOpenBtn.color = inActiveColor;
        nickSearchWin.SetActive(true);
        passSearchWin.SetActive(false);
    }

    public void PassSearchWinOpen()
    {
        NickWinOpenBtn.color = inActiveColor;
        PassWinOpenBtn.color = activeColor;
        nickSearchWin.SetActive(false);
        passSearchWin.SetActive(true);
    }

    public void CloseBtn()
    {
        if (passChangeWin.activeSelf)
            passChangeWin.SetActive(false);
        else
        {
            MSGWin.transform.GetChild(1).gameObject.SetActive(false);
            EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    public void NickSearch()
    {
        string nickPhone = nickPhoneInput.text;
        nickPhoneInput.text = "";

        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("flag=nick");
        sendInfo.Append("&phone=" + nickPhone);

        string url = "UserSearch";
        string respData = web.ConnectServer(url, sendInfo);

        if (respData == "fail")
        {
            MSGWin.GetComponentInChildren<Text>().text = "가입되지 않은 번호입니다.";
            MSGWin.SetActive(true);
        }
        else
        {
            MSGWin.GetComponentInChildren<Text>().text = "당신의 닉네임은 '" + respData + "' 입니다.";
            IsClickSearchNickBtn = true;
            MSGWin.SetActive(true);
            MSGWin.transform.GetChild(1).gameObject.SetActive(true);
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

        string url = "UserSearch";
        string respData = web.ConnectServer(url, sendInfo);

        if(respData=="succ")
        {
            passChangeWin.SetActive(true);
            userNick = passNick;
        }
        else
        {
            MSGWin.GetComponentInChildren<Text>().text = "입력하신 정보를 다시 확인하세요.";
            MSGWin.SetActive(true);
        }
    }

    public void ChangeBtnClick()
    {
        string changePass = passChangeInput.text;
        passChangeInput.text = "";

        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("nick=" + userNick);
        sendInfo.Append("&pass=" + changePass);

        string url = "ChangePassword";
        string respData = web.ConnectServer(url, sendInfo);

        if (respData == "succ")
        {
            MSGWin.GetComponentInChildren<Text>().text = "비밀번호가 변경되었습니다.";
            MSGWin.SetActive(true);
            transform.parent.gameObject.SetActive(false);
        }
    }
}
