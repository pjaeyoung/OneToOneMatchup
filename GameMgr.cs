using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* 아이템 클래스 : 아이템 획득 갯수, 빈 아이템가방 넘버 */
public class Item
{
    int getItemNum; 
    int maxItemNum;
    int emtyNum;

    public Item()
    {
        getItemNum = 0;
        maxItemNum = 3;
        emtyNum = 0;
    }

    public int GetItemNum()
    {
        return getItemNum;
    }

    public int GetEmtyNum()
    {
        return emtyNum;
    }

    public void changeEmtyNum(int changeNum)
    {
        emtyNum = changeNum;
    }

    public int changeGetItemNum(int changeNum)
    {
        if(changeNum <= maxItemNum)
        {
            getItemNum = changeNum;
            return 0;
        }
        else
            return 1;
    }
}

public class Jobs
{
    string ImageName;
    float hp;
    float atk;
    float def;

    public Jobs(string n, float a, float d)
    {
        ImageName = n;
        hp = 10;
        atk = a;
        def = d;
    }
}
//벨런스 조정 나중에 
public class Warrior : Jobs
{
    public Warrior() : base("img_weapon_sword", 2, 5) { }
}

public class Magician : Jobs
{
    public Magician() : base("wand", 1, 2) { }
}

public class PlayerInfo
{
    string job;
    string armor;
    string[] getItemArr;

    public PlayerInfo()
    {
        job = "";
        armor = "";
        getItemArr = new string[3];
    }

    public void InputGetItemArr(string i, int index)
    {
        getItemArr[index] = i;
    }

    public void changePlayerInfo(string j, string a)
    {
        job = j;
        armor = a;
    }

    public string getPlayerJob()
    {
        return job;
    }

    public string getPlayerArmor()
    {
        return armor;
    }

    public string[] getPlayerItemArr()
    {
        return getItemArr;
    }
}

public class GameMgr : MonoBehaviour {
    enum eBOOLEAN
    {
        FALSE, TRUE
    }

    public Text T_timer;
    public Button[] itemBtn; //화면에 표시되는 '획득한 아이템 목록'(안드로이드용 고려)
    public Item Citem;
    public Jobs CJobs;
    public Warrior CWarrior;
    public Magician CMagician;
    public PlayerInfo CPlayerInfo;

    float countTimer; //카운트다운 타이머 
    int min; //분
    float timer; //제한 시간 타이머

    private void Awake()
    {
        Screen.SetResolution(Screen.width, Screen.width * 16 / 9, true); //
        Citem = new Item();
        countTimer = 11;
        min = 0;
        timer = 0;
    }

    void Start () //fightScene까지 끝나면 gameMgr destroy
    {
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "fightScene")
        {
            min = 0;
        }
    }
	
	void Update ()
    {
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "ItemCollectScene")
        {
            if (min >= 1)
            {
                DontDestroyOnLoad(this.transform.gameObject);
                StartCoroutine("waitChangeScene");
                countTimer -= Time.deltaTime;
            }
            else
            {
                showTime();
            }
        }
        else if (scene.name == "fightScene")
        {
            //showTime();
        }
	}

    /*fightScene 전환하기 전 카운트 다운 : CountDown, waitChangeScene */
    void countDown ()
    {
        int count = (int)countTimer;
        GameObject fightStart = GameObject.Find("Canvas").transform.Find("fightGameStartMSG").gameObject;
        fightStart.SetActive(true);
        Text countText = GameObject.Find("countDown").GetComponent<Text>();
        string s_count;
      
        s_count = string.Format("{0:00}", count);
        countText.text = s_count;
    }

  
    IEnumerator waitChangeScene() 
    {
        while(countTimer >= 0)
        {
            countDown();
            yield return new WaitForSeconds(1);
        }
        SceneManager.LoadScene(3);
    }
    
    /* 타이머(아이템 필드에서 사용. 60초 측정 및 UI 표시) : showTime, TimerToString */
    void showTime()
    {
        timer += Time.deltaTime;
        if (timer > 60)
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
    int getEmtyImgIndex() //빈 아이템가방 인덱스 가져오기
    {
        int index = 0;
        if(itemBtn.Length != 0)
        {
            foreach (Button itemImg in itemBtn)
            {
                string imgName = itemImg.GetComponent<Image>().sprite.name;
                if (imgName == "img_emty")
                {
                    int num = Citem.GetEmtyNum();
                    if(num >= 0 && num <3)
                        Citem.changeEmtyNum(index + 1);
                    break;
                }
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
            string name = obj.GetComponent<MeshRenderer>().material.name;
            string objImg = getAccurateName(name);
            string fileName = "Sprites/";
            Sprite spr = Resources.Load<Sprite>(fileName + objImg);
            if (spr != null)
                itemBtn[emtyIndex].GetComponent<Image>().sprite = spr;
            else
                Debug.Log("null");
        }
    }

    public string getAccurateName(string name) //material.name 반환값에서 (instance) 부분 삭제 
    {
        string cmp = "";
        int nameLength = name.Length;
        for(int i = 0; i<nameLength; i++)
        {
            if (name[i] == ' ')
            {
                break;
            }
            cmp += name[i];
        }
        return cmp;
    }
}
