using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCntrl : MonoBehaviour {
    enum eBOOLEAN
    {
        FALSE, TRUE
    }

    public float moveSpeed = 5.0f;
    public float jumpSpeed = 100.0f;
    public float rotSpeed = 100.0f;
    float rotTimer;
    public Button[] itemButton;
    GameMgr GM;
    AnimationManager AM;
    GameObject canvas;

    int IsJump; // 공중에 있는 상태면 TRUE, 땅에 닿인 상태면 FALSE
    //int IsActiveHighlight; //아이템 외곽선 하이라이트 상태면 TRUE, 아니면 FALSE

    private void Awake()
    {
        IsJump = (int)eBOOLEAN.FALSE;
       // IsActiveHighlight = (int)eBOOLEAN.FALSE;
        GM = GameObject.Find("gameMgr").GetComponent<GameMgr>();
        AM = transform.GetComponent<AnimationManager>();
        canvas = GameObject.Find("Canvas");
    }

    void Start ()
    {
       
	}
	
	private void FixedUpdate ()
    {
        Move();
        if (Input.GetMouseButton(2))
            Rot();
	}

    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == "floor")
        {
            IsJump = (int)eBOOLEAN.FALSE;
        }
    }

    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (Input.GetMouseButtonDown(0))
        {
            if (scene.name == "ItemCollectScene")
            {
                TouchItem();
            }
            else if (scene.name == "fightScene")
            {

            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if(scene.name == "fightScene")
            {

            }
        }
    }

    /* 캐릭터 움직임 제어(상하좌우 wsad, 점프 space, 회전 마우스 가운데) : Move, Rot */
    void Move()
    {
        if (Input.GetKey(KeyCode.D))
        {
            AM.PlayAnimation("Move");
            this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            AM.PlayAnimation("Move");
            this.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            AM.PlayAnimation("Move");
            this.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            AM.PlayAnimation("Move");
            this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsJump == (int)eBOOLEAN.FALSE)
            {
                IsJump = (int)eBOOLEAN.TRUE;
                this.transform.Translate(Vector3.up * jumpSpeed * Time.deltaTime);
            }
        }
    }

    void Rot() 
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime);
        //float y = Input.mousePosition.x * Time.deltaTime * rotSpeed;
        //Vector3 rotY = new Vector3(0, y, 0);
        //transform.rotation = Quaternion.Euler(rotY);
    }

    /* 아이템 필드에서 아이템 터치(마우스 왼쪽 버튼 클릭) : TouchItem , fullItemMsgEnd */

    void TouchItem() //아이템 가방이 차기 전 : 아이템 가방 이미지 변경 및 itemPool로 위치 변경, 아이템 가방이 다 찬 후: 경고 메세지 
    {
        Debug.Log("Touch!");

        GameObject fullItem = canvas.transform.Find("fullItemMSG").gameObject;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if(Physics.Raycast(ray, out rayHit,Mathf.Infinity))
        {
            Debug.Log("hit!");

            GameObject obj = rayHit.transform.gameObject;
            if (obj.tag == "item")
            {
                Debug.Log("소비아이템");
                int num = GM.Citem.GetItemNum();
                num++;
                int IsMaxItem = GM.Citem.changeGetItemNum(num);
                if (IsMaxItem == (int)eBOOLEAN.FALSE)
                {
                    fullItem.SetActive(false);
                    int emtyNum = GM.Citem.GetEmtyNum();
                    itemBtn s_itemBtn = itemButton[emtyNum].GetComponent<itemBtn>(); //아이템 버튼 스크립트에서 아이템 목록 입력! 
                    s_itemBtn.InputGetItemArr(obj, emtyNum);
                    
                    GM.changeItemImg(obj);
                    obj.SetActive(false);
                }
                else
                {
                    num = 3;
                    fullItem.SetActive(true);
                    StartCoroutine("fullItemMsgEnd", fullItem);
                }
            }
            else
            {
                Debug.Log("무기/방어구 아이템");
                if(obj.tag == "weapon")
                {
                    weaponArmorBtn s_weaponBtn = canvas.transform.Find("btn_GetWeapon").GetComponent<weaponArmorBtn>();
                    s_weaponBtn.inputGameObj(obj);
                }
                else if(obj.tag == "armor")
                {
                    weaponArmorBtn s_ArmorBtn = canvas.transform.Find("btn_GetArmor").GetComponent<weaponArmorBtn>();
                    s_ArmorBtn.inputGameObj(obj);
                }
            }
        }
    }

    /* 꽉찬 아이템창 알림 메세지 화면 표시 시간 제어 */
    IEnumerator fullItemMsgEnd(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        obj.SetActive(false);
    }

    void chkHighlight() //일단 보류....
    {
       
    }

    /* BoxCast 디버깅용, 최종 빌드전에 삭제 */
    //private void OnDrawGizmos()
    //{
    //    Vector3 fwd = transform.TransformDirection(transform.forward);
    //    Vector3 center = transform.position;
    //    Quaternion rot = transform.rotation;
    //    Vector3 scale = new Vector3(10, 2, 4);

    //    float maxDistance = 6.0f;
    //    Gizmos.DrawWireCube(center, scale);

    //    GameObject[] objArr = GameObject.FindGameObjectsWithTag("touchable");
    //    if (objArr.Length != 0)
    //    {
    //        foreach (GameObject objArrChild in objArr)
    //        {
    //            RaycastHit[] hitArr;
    //            hitArr = Physics.BoxCastAll(center, scale / 2, fwd, rot, maxDistance, 9);
    //            foreach (RaycastHit hitArrChild in hitArr)
    //            {
    //                GameObject hit = hitArrChild.transform.gameObject;
    //                if (objArrChild == hit)
    //                {
    //                    hit.GetComponent<outline>().enabled = true;
    //                    IsActiveHighlight = (int)eBOOLEAN.TRUE;
    //                }
    //                else
    //                {
    //                    objArrChild.GetComponent<outline>().enabled = false;
    //                    IsActiveHighlight = (int)eBOOLEAN.FALSE;
    //                }
    //            }
    //        }
    //    }
    //}
}

