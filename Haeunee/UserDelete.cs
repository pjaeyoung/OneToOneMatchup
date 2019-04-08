using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserDelete : MonoBehaviour
{
    WebServerScript web;
    GameObject webObj;
    public GameObject userDelWin;
    public InputField passInput;
    public GameObject delSuccWin;
    public GameObject delFailWin;
    float winTime = 0;
    void Start()
    {
        webObj = GameObject.Find("WebServer");
        web = webObj.GetComponent<WebServerScript>();
    }
    
    void Update()
    {
        if(delSuccWin.activeSelf||delFailWin.activeSelf)
        {
            winTime += Time.deltaTime;
            if(winTime>=2)
            {
                if(delSuccWin.activeSelf)
                {
                    SceneManager.LoadScene("LoginScene");
                    GameObject.Destroy(webObj);
                    GameObject.Destroy(transform.parent.transform.parent.gameObject);
                }
                delFailWin.SetActive(false);
                winTime = 0;
            }
        }
    }

    public void UserDelBtn()
    {
        userDelWin.SetActive(true);
    }

    public void DelBtnClick()
    {
        string delPass = passInput.text;
        passInput.text = "";

        StringBuilder sendInfo = new StringBuilder();
        sendInfo.Append("nick=" + web.nick);
        sendInfo.Append("&password=" + delPass);

        string url = "http://192.168.0.22:10000/UserDel";
        string respData = web.ConnectServer(url, sendInfo);

        if (respData == "succ")
        {
            delSuccWin.SetActive(true);
        }
        else
        {
            delFailWin.SetActive(true);
        }
    }

    public void CancelBtnClick()
    {
        passInput.text = "";
        userDelWin.SetActive(false);
    }
}
