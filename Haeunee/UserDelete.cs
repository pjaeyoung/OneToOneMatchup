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
    public GameObject MSGWin;
    float winTime = 0;
    bool delete = false;

    void Start()
    {
        webObj = GameObject.Find("WebServer");
        web = webObj.GetComponent<WebServerScript>();
    }
    
    void Update()
    {
        if(MSGWin.activeSelf)
        {
            winTime += Time.deltaTime;
            if(winTime>=2)
            {
                if(delete)
                {
                    loading.LoadScene("LoginScene");
                    GameObject.Destroy(webObj);
                    GameObject.Destroy(transform.parent.transform.parent.gameObject);
                }
                MSGWin.SetActive(false);
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

        string url = "UserDel";
        string respData = web.ConnectServer(url, sendInfo);

        if (respData == "succ")
        {
            MSGWin.GetComponentInChildren<Text>().text = "탈퇴되었습니다.";
            delete = true;
        }
        else
        {
            MSGWin.GetComponentInChildren<Text>().text = "회원정보를 다시 확인해주세요.";
            delete = false;
        }
        MSGWin.SetActive(true);
    }

    public void CancelBtnClick()
    {
        passInput.text = "";
        userDelWin.SetActive(false);
    }
}
