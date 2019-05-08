using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemFieldCntrl : MonoBehaviour {

    GameEnterScript enter;
    public Scene scene;
    public ItemBtn s_itemBtn; //아이템 가방창 
    PlayerScript s_player; 
    GameObject waitImg; //둘 다 데이터를 주고 받을 때까지 필요한 시간에 띄울 이미지
    GameObject itemBtnCanvas; //아이템 가방창만 GameScene으로 옮기기 위해 분리된 Canvas 
    RawImage alarmImg; //경고 이미지 
    Text T_timer; //시간 표시 
    GameObject[] itemBagArr;// GameScene의 아이템가방 

    bool gameEnter = false; //GameScene 입장 가능 여부 판단  
    bool btnSet = false; // 아이템 버튼 정보 옮기기  
    bool alarmOn = false; //alarmImg 출력 판단 
    bool timerOn = true; //timer 출력 판단 

    int min; //분
    float timer; //제한 시간 타이머

    private void Start()
    {
        waitImg = GameObject.Find("Canvas/fightGameStartMSG");
        waitImg.SetActive(false);
        itemBtnCanvas = GameObject.Find("itemBtnCanvas");
        s_player = GameObject.Find("Player").GetComponent<PlayerScript>();
        s_itemBtn = itemBtnCanvas.GetComponentInChildren<ItemBtn>();
        s_player.GM = this;
        s_itemBtn.GM = this;
        alarmImg = itemBtnCanvas.transform.GetChild(0).GetComponent<RawImage>();
        alarmImg.gameObject.SetActive(false);
        T_timer = GameObject.Find("Canvas/Timer").GetComponent<Text>();
        min = 0;
        timer = 0;

        GameObject serverObj = GameObject.Find("GameMgr2/MatchingCntrl");
        enter = serverObj.GetComponent<GameEnterScript>();
    }

    void Update ()
    {
        scene = SceneManager.GetActiveScene();
        if (scene.name == "ItemCollectScene")
        {
            if (min >= 1 && gameEnter == false) //60초 완료. GameScene 넘어갈 준비 
            {
                itemBtnCanvas.SetActive(false);
                waitImg.SetActive(true);
                alarmImg.gameObject.SetActive(false);
                gameEnter = true;
                min = 0;
                sendPlayerInfoToServ();
            }
            else
                showTime();
        }
        else if (scene.name == "GameScene") // 소비아이템 목록만 GameScene에서 출력, 위치 좌측 상단으로 이동 
        {
            if (!btnSet)
            {
                btnSet = true;
                itemBagArr = GameObject.FindGameObjectsWithTag("itemBag");
                itemBagArr[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + getItemName(enter.savCharInfo.item1));
                itemBagArr[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + getItemName(enter.savCharInfo.item2));
                itemBagArr[2].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + getItemName(enter.savCharInfo.item3));
            }
        }
	}

    string getItemName(int _itemCode) // 서버에서 받은 소비아이템 코드에서 이름 명칭 받아오기 
    {
        if (_itemCode == (int)eITEM.em_HP_POTION)
            return "hpPotion";
        else if (_itemCode == (int)eITEM.em_SPEED_POTION)
            return "speedPotion";
        else if (_itemCode == (int)eITEM.em_DAMAGE_UP_POTIOM)
            return "damageUpPotion";
        else if (_itemCode == (int)eITEM.em_DEFENCE_UP_POTION)
            return "defenceUpPotion";
        else
            return "error";
    }

    int getItemCode(string _itemName) //서버용 소비아이템 코드 번호 
    {
        if (_itemName == "hpPotion")
            return (int)eITEM.em_HP_POTION;
        else if (_itemName == "speedPotion")
            return (int)eITEM.em_SPEED_POTION;
        else if (_itemName == "damageUpPotion")
            return (int)eITEM.em_DAMAGE_UP_POTIOM;
        else if (_itemName == "defenceUpPotion")
            return (int)eITEM.em_DEFENCE_UP_POTION;
        else
            return -1;
    }

    int GetArmorCode (string armorName) //서버용 방어구 코드 번호 
    {
        string ArmorGender = armorName.Substring(0, 1);
        string ArmorNumStr = armorName.Substring(8, 1);
        int ArmorNumInt = int.Parse(ArmorNumStr);
        return ArmorNumInt;
    }

    int GetWeaponCode (string weapon) //서버용 무기 코드 번호 
    {
        string w = s_itemBtn.getAccurateName(weapon);
        if (w == "greatSword")
            return (int)eWEAPON.em_GREATESWORD;
        else if (w == "wand")
            return (int)eWEAPON.em_WAND;
        else if (w == "bow")
            return (int)eWEAPON.em_BOW;
        else if (w == "swordAndShield")
            return (int)eWEAPON.em_SWORDANDSHIELD;
        else
            return -1;
    }

    void sendPlayerInfoToServ() //씬 전환 전 최종 playerInfo 서버 전달 (서버프로그래머 담당)
    {
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            if (s_itemBtn.getItemArr[i].tag == "item")
            {
                count++;
                string itemName = s_itemBtn.getAccurateName(s_itemBtn.getItemArr[i].name);
                if (count == 1)
                    enter.savCharInfo.item1 = getItemCode(itemName);
                else if (count == 2)                      
                    enter.savCharInfo.item2 = getItemCode(itemName);
                else                                      
                    enter.savCharInfo.item3 = getItemCode(itemName);
            }
            else if (s_itemBtn.getItemArr[i].tag == "weapon")
            {
                enter.savCharInfo.weapon = GetWeaponCode(s_itemBtn.getItemArr[i].name);
            }
            else if (s_itemBtn.getItemArr[i].tag == "armor")
            {
                enter.savCharInfo.armor = GetArmorCode(s_itemBtn.getItemArr[i].name);
            }
        }
        SocketServer.SingleTonServ().SendMsg(enter.savCharInfo); //현재 유저가 선택한 모든 값(외형, 옷, 무기)을 상대에게 보냄
    }

    /* 타이머(아이템 필드에서 사용. 60초 측정 및 UI 표시 , 끝나기 10초 전 알람 표시) : showTime, Time2Str, AlarmActive */
    void showTime()
    {
        if (timerOn == true)
            timer += Time.deltaTime;
        if (timer >= 60)
        {
            timer = 0;
            min++;
            timerOn = false;
            s_player.gameObject.GetComponentInChildren<Camera>().enabled = false;
            s_player.enabled = false;
        }
        if (timer >= 50 && alarmOn == false)
        {
            alarmOn = true;
            alarmImg.gameObject.SetActive(true);
            StartCoroutine(AlarmActive(timer));
        }
        int sec = (int)timer;
        string s_time = Time2Str(min, sec);
        T_timer.text = s_time;
    }

    string Time2Str(int _min, int _sec) // int를 str로 바꾸기
    {
        string time = string.Format("{0:00} : {1:00}", _min, _sec);
        return time;
    }

    public int changeUsedItemImg(int idx) // GameScene에서 아이템 버튼 사용 시 emty이미지 변경 
    {
        Sprite spr = Resources.Load<Sprite>("Sprites/img_emty");
        Sprite itemBtnSpr = itemBagArr[idx].GetComponent<Image>().sprite;
        if (itemBtnSpr.name == spr.name)
        {
            Debug.Log("이미 사용한 아이템");
            return (int)eITEMUSE.USED;
        }
        else
        {
            itemBagArr[idx].gameObject.GetComponent<Image>().sprite = spr;
            return (int)eITEMUSE.UNUSED;
        }
    }

    IEnumerator AlarmActive(float _timer) //끝나기 10초 전 알람  
    {
        while (_timer < 60)
        {
            alarmImg.CrossFadeAlpha(0, 1.0f, false);
            yield return new WaitForSeconds(1.0f);
            alarmImg.CrossFadeAlpha(1, 1.0f, false);
            yield return new WaitForSeconds(1.0f);
        }
    }
  
}
