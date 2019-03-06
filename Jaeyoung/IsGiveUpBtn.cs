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
        sEnd end = new sEnd((int)eMSG.em_END);
        SocketServer.SingleTonServ().SendMsg(end);
        Debug.Log("END");
        //서버에 항복 메세지 전달
        //나와 상대방 모두 대기 화면으로 바꾸기 
    }

    public void onClickNoBtn()
    {
        GiveUpImg.SetActive(false);
        Block.SetActive(false);
    }
}
