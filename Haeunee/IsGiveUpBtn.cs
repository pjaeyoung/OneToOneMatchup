using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsGiveUpBtn : MonoBehaviour
{
    GameObject Block;
    GameObject GiveUpImg;

    private void Awake()
    {
        Block = transform.parent.transform.parent.gameObject;
        GiveUpImg = transform.parent.gameObject;
    }

    public void onClickYesBtn()
    {
        sEnd esc = new sEnd(0);
        SocketServer.SingleTonServ().SendMsg(esc);
        Debug.Log("ESC");
        //서버에 항복 메세지 전달
    }

    public void onClickNoBtn()
    {
        GiveUpImg.SetActive(false);
        Block.SetActive(false);
    }
}
