using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCntrl_itemField : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpSpeed = 100.0f;
    public float rotSpeed = 100.0f;
    public Button[] itemButton;
    public LayerChange layerChange;
    GameMgr GM;
    Scene scene;
    AnimationManager AM;
    GameObject canvas;
    GameObject[] touchableItems;
    GameObject chkHighlightBox;
    GameObject fullItem;
    Rigidbody playerRigid;

    int IsJump; // 공중에 있는 상태면 TRUE, 땅에 닿인 상태면 FALSE
    int IsHighlight; // 하이라이트박스 내에 있으면 TRUE, 아니면 FALSE

    private void Awake()
    {
        IsJump = (int)eBOOLEAN.FALSE;
        IsHighlight = (int)eBOOLEAN.FALSE;
        GM = GameObject.Find("gameMgr").GetComponent<GameMgr>();
        AM = transform.GetComponent<AnimationManager>();
        scene = SceneManager.GetActiveScene();
        canvas = GameObject.Find("Canvas");
        chkHighlightBox = transform.GetChild(9).gameObject;
        playerRigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (scene.name == "ItemCollectScene") // touchable layer인 오브젝트 정보 저장, 무기 아이템 layer변경(LayerChange 스크립트) 
        {
            fullItem = canvas.transform.Find("fullItemMSG").gameObject;
            layerChange = GM.GetComponent<LayerChange>();
            ItemSpawn s_itemSpawn = GameObject.Find("itemSpawn").GetComponent<ItemSpawn>();
            int itemSpawnLens = s_itemSpawn.itemSpawn.Length;

            GameObject[] Items = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            int len = Items.Length;
            touchableItems = new GameObject[itemSpawnLens];
            int idx = 0;
            for (int i = 0; i < len; i++)
            {
                if (Items[i].layer == (int)eLAYER.TOUCHABLE)
                {
                    touchableItems[idx] = Items[i];
                    if (touchableItems[idx].tag == "weapon")
                        layerChange.InputWeaponArr(touchableItems[idx]);
                    idx++;
                }
            }
        }

    }

    private void FixedUpdate()
    {
        Move();
        if (Input.GetMouseButton(2))
            Rot();
    }

    /* 아이템 필드 상 게임오브젝트 외곽선 하이라이트 표시 : OnTriggerEnter, OnTriggerExit, OnOffHighlight */
    private void OnTriggerEnter(Collider other) //chkHighlightBox에 부딪친 오브젝트 중 touchable layer인 경우에만 하이라이트 표시 
    {
        GameObject obj = other.gameObject;
        if (scene.name == "ItemCollectScene")
        {
            if (obj.layer == (int)eLAYER.TOUCHABLE)
            {
                Debug.Log("triggerEnter");
                IsHighlight = (int)eBOOLEAN.TRUE;
                OnOffHighlight(obj, IsHighlight);
            }
        }
    }

    private void OnTriggerExit(Collider other) //chkHighlightBox에서 벗어나면 하이라이트 해제
    {
        GameObject obj = other.gameObject;
        if (scene.name == "ItemCollectScene")
        {
            if (obj.layer == (int)eLAYER.TOUCHABLE)
            {
                IsHighlight = (int)eBOOLEAN.FALSE;
                OnOffHighlight(obj, IsHighlight);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.transform.gameObject;
        if (obj.tag == "floor")
            IsJump = (int)eBOOLEAN.FALSE;
    }
    void Update()
    {
        if (scene.name == "ItemCollectScene")
        {
            if (Input.GetMouseButtonDown(0))
                TouchItem();
        }
    }

    /* 캐릭터 움직임 제어(상하좌우 wsad, 점프 space, 회전 마우스 가운데) : Move, Rot */
    void Move()
    {
        AM.PlayAnimation("Idle");
        if (Input.GetKey(KeyCode.D))
            this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.A))
            this.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            this.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.W))
            this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsJump == (int)eBOOLEAN.FALSE)
            {
                IsJump = (int)eBOOLEAN.TRUE;
                playerRigid.AddForce(0, 150, 0, ForceMode.Acceleration);
            }
        }
    }

    void Rot()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime);
    }

    /* 아이템 필드에서 아이템 터치(마우스 왼쪽 버튼 클릭) : TouchItem , fullItemMsgEnd */

    void TouchItem() //아이템 가방이 차기 전 : 아이템 가방 이미지 변경 및 itemPool로 위치 변경, 아이템 가방이 다 찬 후: 경고 메세지 
    {
        Debug.Log("Touch");

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(cameraRay, out rayHit))
        {
            GameObject rayObj = rayHit.transform.gameObject;
            if (rayObj.tag == "item" && rayObj.GetComponent<outline>().OutlineMode == outline.Mode.OutlineAndSilhouette)
            {
                int num = GM.CitemCount.GetItemNum();
                num++;
                int IsMaxItem = GM.CitemCount.changeGetItemNum(num);
                if (IsMaxItem == (int)eBOOLEAN.FALSE)
                {
                    fullItem.SetActive(false);
                    int emtyNum = GM.getEmtyImgIndex();
                    itemBtn s_itemBtn = itemButton[emtyNum].GetComponent<itemBtn>(); //아이템 버튼 스크립트에서 아이템 목록 입력! 
                    s_itemBtn.InputGetItemArr(rayObj, emtyNum);

                    GM.changeItemImg(rayObj);
                    rayObj.SetActive(false);
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
                if(rayObj.tag == "weapon" && rayObj.GetComponentInChildren<outline>().OutlineMode == outline.Mode.OutlineAndSilhouette)
                {
                    weaponArmorBtn s_weaponBtn = canvas.transform.Find("btn_GetWeapon").GetComponent<weaponArmorBtn>();
                    s_weaponBtn.inputGameObj(rayObj);
                }
                else if (rayObj.tag == "armor" && rayObj.GetComponentInChildren<outline>().OutlineMode == outline.Mode.OutlineAndSilhouette)
                {
                    weaponArmorBtn s_ArmorBtn = canvas.transform.Find("btn_GetArmor").GetComponent<weaponArmorBtn>();
                    s_ArmorBtn.inputGameObj(rayObj);
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

    /* 아이템 필드에서 외곽선 하이라이트 표시 여부 결정 */
    void OnOffHighlight(GameObject hitObj, int IsHighlight)
    {
        if (IsHighlight == (int)eBOOLEAN.TRUE)
            hitObj.GetComponentInChildren<outline>().OutlineMode = outline.Mode.OutlineAndSilhouette;
        else
            hitObj.GetComponentInChildren<outline>().OutlineMode = outline.Mode.SilhouetteOnly;
    }
}

