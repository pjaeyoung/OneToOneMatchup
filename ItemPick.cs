using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//아이템을 선택하는 화면에서 아이템 선택하는 캐릭터에 필요한 스크립트
public class ItemPick : MonoBehaviour {
    Ray cameraRay; //선택을 위한 레이캐스트
    RaycastHit rayHit;
    public Text weaponText; //무엇을 선택했는지
    public Text clothText;
    public Text timerText; //타이머
    float playTime=0;
    int weapon = -1; //선택한 무기 값(default -1이어야 함(아무것도 선택하지 않은 무기))
    int cloth = 3; //선택한 방어구 값(default 3이어야 함(아무것도 선택하지 않은 방어구))
    SocketServer server;
    GameEnterScript enter;
    public GameObject waitImg; //둘 다 데이터를 주고 받을 때까지 필요한 시간에 띄울 이미지
    bool gameEnter = false; //제한시간이 지난 후 한번만 기능을 실행하기 위한 변수

	// Use this for initialization
	void Start () {
        StartCoroutine(LoadDelay());
    }

    IEnumerator LoadDelay() //시작 시 오브젝트가 모두 로드될 때까지 기다리기 위한 코루틴
    {
        yield return new WaitForSeconds(0.5f);
        GameObject serverObj = GameObject.Find("SocketServer");
        server = serverObj.GetComponent<SocketServer>();
        enter = serverObj.GetComponent<GameEnterScript>();
    }

    void Update () {
        if(playTime>=10&& gameEnter==false) //제한 시간이 지난 후
        {
            gameEnter = true;
            enter.savCharInfo.weapon = weapon; //선택한 무기, 방어구 값 저장
            enter.savCharInfo.cloth = cloth;
            waitImg.SetActive(true);
            server.SendMsg(enter.savCharInfo); //현재 유저가 선택한 모든 값(외형, 옷, 무기)을 상대에게 보냄
        }
        else //제한 시간 지나기 전 시간 체크
        {
            playTime += Time.deltaTime;
            timerText.text = "Time: " + string.Format("{0:0}", playTime);
        }
        if (Input.GetMouseButton(2)) //화면 회전
        {
            transform.Rotate(0, Input.GetAxisRaw("Mouse X") * 5, 0);
        }
        if (Input.GetKey(KeyCode.W)) //움직임
        {
            transform.Translate(Vector3.forward * 0.1f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * 0.1f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * 0.1f);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * 0.1f);
        }

        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Input.GetMouseButtonDown(0)) //클릭한 아이템 이름 가져오기
        {
            if(Physics.Raycast(cameraRay,out rayHit))
            {
                string item = rayHit.transform.gameObject.name;
                Debug.Log(item);
                ChangeText(item);
            }
        }
	}

    void ChangeText(string text) //아이템 이름에 따라 표시되는 텍스트를 변경하고 선택된 아이템 정보를 저장하는 함수
    {
        if (text == "Shield")
        {
            weaponText.text = "선택한 무기: 방패와 검";
            weapon = (int)eWEAPON.em_SHIElD;
        }
        else if (text == "Wand")
        {
            weaponText.text = "선택한 무기: 마법봉";
            weapon = (int)eWEAPON.em_WAND;
        }
        else if (text == "Bow")
        {
            weaponText.text = "선택한 무기: 활";
            weapon = (int)eWEAPON.em_BOW;
        }
        else if (text == "GreatSword")
        {
            weaponText.text = "선택한 무기: 대검";
            weapon = (int)eWEAPON.em_SWORD;
        }
        else if (text == "F_suit01" || text == "m_suit02")
        {
            clothText.text = "선택한 방어구: 가벼운 옷";
            cloth = 0;
        }
        else if (text == "F_suit02" || text == "m_suit01")
        {
            clothText.text = "선택한 방어구: 갑옷";
            cloth = 1;
        }   
        else if (text == "F_suit03" || text == "m_suit03")
        {
            clothText.text = "선택한 방어구: 로브";
            cloth = 2;
        }
    }
}
