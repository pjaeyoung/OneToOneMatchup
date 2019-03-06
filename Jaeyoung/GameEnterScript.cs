using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

//매칭 버튼을 누르고 아이템 씬으로 전환시키기 위한 스크립트
public class GameEnterScript : MonoBehaviour {
    public GameObject matchingImg; //매칭중 이미지
    bool matchActive = false; //매칭 버튼을 누르고 매칭이 아직 되지 않았을 때를 체크
    public sCharInfo savCharInfo; //플레이어 캐릭터의 정보를 모두 저장할 구조체
    HeroCustomize heroCustomize;
    bool matchSuccess = false; //매칭 성공
    public int myItem1, myItem2, myItem3; //내가 선택한 아이템 세가지 저장 변수
    GameObject player; //플레이어 오브젝트
    HeroCustomize playerCustom; //플레이어의 외형을 변경하는 스크립트
    GameObject enemy; //적
    HeroCustomize enemyCustom; //적의 외형을 변경하는 스크립트

    void Start () {
        DontDestroyOnLoad(this); //서버 오브젝트 파괴되지 않게 함
        SocketServer.SingleTonServ().GetEnterScript(this);
        heroCustomize = GameObject.Find("Player").GetComponent<HeroCustomize>();
    }

    private void Update()
    {
        SocketServer.SingleTonServ().WaitRecieve();
        if(matchingImg!=null && matchingImg.activeSelf==false) //매칭중이라는 이미지 띄우기
            matchingImg.SetActive(matchActive);

        if (matchSuccess == true) //매칭이 성공되었을 때
        {
            matchSuccess = false;
            int gender = 0;
            if (heroCustomize.Gender.Equals("Male"))
                gender = (int)eGENDER.MALE;
            else if (heroCustomize.Gender.Equals("Female"))
                gender = (int)eGENDER.FEMALE;
            savCharInfo = new sCharInfo(0, 0, gender, heroCustomize.IndexHair.CurrentIndex,
                heroCustomize.IndexColorHair.CurrentIndex, heroCustomize.IndexFace.CurrentIndex, -1, -1, -1);
            SceneManager.LoadScene("ItemCollectScene");
        }
    }

    public void RandomClick()
    {//매칭 버튼을 눌렀을 때 플레이어의 정보를 저장하고 플레이어가 매칭버튼을 눌렀다는 것을 서버에 전달
        EventSystem.current.currentSelectedGameObject.SetActive(false);
        sGameRoom enter = new sGameRoom(0);
        SocketServer.SingleTonServ().SendMsg(enter);
    }

    public void Matching() //매칭이 되지 않았음
    {
        matchActive = true;
    }

    public void MatchSucc() //매칭 성공 하였음
    {
        matchSuccess = true;
    }
}
