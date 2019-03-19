﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour {
    GameEnterScript enter;
    public GameObject waitImg; //둘 다 데이터를 주고 받을 때까지 필요한 시간에 띄울 이미지

    public Text T_timer;
    public Button[] itemBtn; //화면에 표시되는 '획득한 아이템 목록'(안드로이드용 고려)
    public ItemCount CitemCount;
    public PlayerInfo CPlayerInfo;
    public GameObject canvas;
    public Scene scene;
    bool gameEnter = false;
    bool interactive = true; // 아이템 버튼 interactive 설정

    float countTimer; //카운트다운 타이머 
    int min; //분
    float timer; //제한 시간 타이머

    private void Awake()
    {
        CitemCount = new ItemCount();
        CPlayerInfo = new PlayerInfo();
        countTimer = 10;
        min = 0;
        timer = 0;
    }

    private void Start()
    {
        GameObject serverObj = GameObject.Find("GameMgr");
        enter = serverObj.GetComponent<GameEnterScript>();
    }

    void Update ()
    {
        scene = SceneManager.GetActiveScene();
        if(scene.name == "ItemCollectScene")
        {
            if (min >= 1 && gameEnter == false)
            {
                changeLayerToWeapon();
                gameEnter = true;
                min = 0;
                DontDestroyObject();
                StartCoroutine(waitChangeScene());
            }
            else
                showTime();
        }
        else if(scene.name == "GameScene")
        {
            if(interactive == true)
            {
                interactive = false;
                for (int i = 0; i < 3; i++)
                {
                    itemBtn[i].enabled = false;
                    itemBtn[i].gameObject.GetComponent<itemBtn>().enabled = false;
                }
            }
        }
	}

    /*fightScene 전환하기 전 카운트 다운 : CountDown, waitChangeScene , sendPlayerInfoToServ */
    void countDown ()
    {
        waitImg.SetActive(true);
        int count = (int)countTimer;
        GameObject fightStart = GameObject.Find("Canvas").transform.Find("fightGameStartMSG").gameObject;
        fightStart.SetActive(true);
        Text countText = GameObject.Find("countDown").GetComponent<Text>();
        string s_count;
        
        if(countTimer>=0)
        {
            s_count = string.Format("{0:00}", count);
            countText.text = s_count;
            countTimer -= 1;
        }
    }
  
    IEnumerator waitChangeScene() 
    {
        while(countTimer >= 0)
        {
            yield return new WaitForSeconds(1);
            countDown();
        }
       sendPlayerInfoToServ();
    }

    void changeLayerToWeapon()
    {
        LayerChange LC = this.transform.GetComponent<LayerChange>();
        for(int i = 0; i<4; i++)
            LC.OutputWeapon(i).layer = (int)eLAYER.WEAPON; 
    }

    void sendPlayerInfoToServ() //카운트 다운 끝난 후 씬 전환 전 최종 playerInfo 서버 전달 
    {
        enter.savCharInfo.weapon = CPlayerInfo.getPlayerWeapon(); //선택한 무기, 방어구, 소비아이템 값 저장
        enter.savCharInfo.armor = CPlayerInfo.getPlayerArmor();
        enter.savCharInfo.item1 = CPlayerInfo.getPlayerItemArr(0);
        enter.savCharInfo.item2 = CPlayerInfo.getPlayerItemArr(1);
        enter.savCharInfo.item3 = CPlayerInfo.getPlayerItemArr(2);
        SocketServer.SingleTonServ().SendMsg(enter.savCharInfo); //현재 유저가 선택한 모든 값(외형, 옷, 무기)을 상대에게 보냄
    }

    void DontDestroyObject()
    {
        DontDestroyOnLoad(this.transform.gameObject);
        DontDestroyOnLoad(canvas);
    } //GameScene에도 사용할 게임오브젝트 유지 

    /* 타이머(아이템 필드에서 사용. 60초 측정 및 UI 표시) : showTime, TimerToString */
    void showTime()
    {
        timer += Time.deltaTime;
        if (timer > 200)
        {
            timer = 0;
            min++;
        }
        int sec = (int)timer;
        string s_time = TimerToString(min, sec);
        T_timer.text = s_time;
    }

    string TimerToString(int _min, int _sec)
    {
        string time = string.Format("{0:00} : {1:00}", _min, _sec);
        return time;
    }

    /* 아이템 가방 : 소비아이템 전용 */
    public int getEmtyImgIndex() //빈 아이템가방 인덱스 가져오기
    {
        int index = 0;
        if(itemBtn.Length != 0)
        {
            foreach (Button itemImg in itemBtn)
            {
                string imgName = itemImg.GetComponent<Image>().sprite.name;
                if (imgName == "img_emty")
                    break;
                index++;
            }
        }
        return index;
    }

    public void changeItemImg(GameObject obj) //빈 아이템가방 인덱스에 아이템 이미지 삽입 
    {
        
        int emtyIndex = getEmtyImgIndex();
        if(emtyIndex >-1 && emtyIndex < 4)
        {
            string itemName = getAccurateName(obj.name);
            string fileName = "Sprites/";
            int eitem = changeItemArrInCPlayerInfo(itemName);
            if (eitem != -1)
                CPlayerInfo.InputGetItemArr(eitem, emtyIndex);
            else
                Debug.Log("no Image");
            Sprite spr = Resources.Load<Sprite>(fileName + itemName);
            if (spr != null)
                itemBtn[emtyIndex].GetComponent<Image>().sprite = spr;
            else
                Debug.Log("null");
        }
    }

    public int changeUsedItemImg(int idx) // GameScene에서 아이템 버튼 사용 시 emty이미지 변경 
    {
        Sprite spr = Resources.Load<Sprite>("Sprites/img_emty");
        Sprite itemBtnSpr = itemBtn[idx].GetComponent<Image>().sprite;
        if(itemBtnSpr.name == spr.name)
        {
            Debug.Log("이미 사용한 아이템");
            return (int)eITEMUSE.USED;
        }
        else
        {
            itemBtn[idx].GetComponent<Image>().sprite = spr;
            return (int)eITEMUSE.UNUSED;
        }
    }

    int changeItemArrInCPlayerInfo(string _itemName) // PlayerInfo 클래스의 멤버 변수 ItemArr정보에 입력 
    {
        if (_itemName == "hpPotion(Clone)")
            return (int)eITEM.em_HP_POTION;
        else if (_itemName == "speedPotion(Clone)")
            return (int)eITEM.em_SPEED_POTION;
        else if (_itemName == "damageUpPotion(Clone)")
            return (int)eITEM.em_DAMAGE_UP_POTIOM;
        else if (_itemName == "defenceUpPotion(Clone)")
            return (int)eITEM.em_DEFENCE_UP_POTION;
        else
            return -1;
    }

    public string getAccurateName(string name)
    {
        string temp = "";
        int len = name.Length;
        for (int i = 0; i < len; i++)
        {
            char sub = name[i];
            if (sub == '(')
                break;
            temp += name[i];
        }
        return temp;
    }
}
