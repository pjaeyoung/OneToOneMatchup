﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatScript : MonoBehaviour
{
    public GameObject chatScroll;
    public InputField chatInput;
    string printChat = "";
    string nowChat = "";
    int nowTextNum = 0;

    WebServerScript web;

    void Start()
    {
        web = GameObject.Find("WebServer").GetComponent<WebServerScript>();
        SocketServer.SingleTonServ().GetChatScript(this);
    }
    
    void Update()
    {
        if(printChat!=nowChat)
        {
            for(int i = 20; i>=1;i--)
            {
                chatScroll.transform.GetChild(i).GetComponent<Text>().text
                    = chatScroll.transform.GetChild(i - 1).GetComponent<Text>().text;
            }
            chatScroll.transform.GetChild(0).GetComponent<Text>().text = nowChat;
            printChat = nowChat;
        }
    }

    public void SendBtnClick()
    {
        string chatMsg = web.nick + " : " + chatInput.text;
        chatInput.text = "";
        if(chatMsg.Length<=30)
        {
            sChat chat = new sChat(chatMsg.ToCharArray());
            SocketServer.SingleTonServ().SendMsg(chat);
        }
        else
        {
            nowChat = "글자 수 제한이 있습니다.";
        }
    }

    public void ChatRecv(string chat)
    {
        nowChat = chat;
    }
}
