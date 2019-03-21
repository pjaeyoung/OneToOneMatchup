using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IsGiveUpBtn : MonoBehaviour
{
    GameObject Block;
    PlayerScript s_Player;
    SpawnScript s_spawn;

    private void Awake()
    {
        Block = transform.parent.transform.parent.gameObject;
        s_spawn = GameObject.Find("GameMgr").GetComponent<SpawnScript>();
    }

    public void onClickYesBtn()
    {
        s_Player = SocketServer.SingleTonServ().NowPlayerScript().GetComponent<PlayerScript>();
        s_Player.enabled = true;
        sEnd end = new sEnd((int)eMSG.em_END);
        SocketServer.SingleTonServ().SendMsg(end);
        Debug.Log("END");
        //서버에 항복 메세지 전달
        //나와 상대방 모두 대기 화면으로 바꾸기 
    }

    public void onClickNoBtn()
    {
        Debug.Log("No Click");
        s_Player = SocketServer.SingleTonServ().NowPlayerScript().GetComponent<PlayerScript>();
        s_Player.GetComponentInChildren<PlayerCameraScript>().enabled = true;
        s_Player.enabled = true;
        Block.SetActive(false);
    }
}
