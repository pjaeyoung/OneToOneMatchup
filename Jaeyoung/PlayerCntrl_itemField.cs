﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCntrl_itemField : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpSpeed = 100.0f;
    public int sensibilityX = 10;
    public Button[] itemButton;
    public LayerChange layerChange;
    public ItemFieldCntrl GM;
    Scene scene;
    AnimationManager AM;
    GameObject canvas;
    GameObject[] touchableItems;
    GameObject fullItem;
    GameObject highlightBox;
    Rigidbody playerRigid;
    Quaternion nowRot;

    bool IsJump; // 공중에 있는 상태면 TRUE, 땅에 닿인 상태면 FALSE
    bool IsLayerChange = false; //layerchage를 했는 지 여부 
    
    private void Awake()
    {
        IsJump = false;
        AM = transform.GetComponent<AnimationManager>();
        scene = SceneManager.GetActiveScene();
        canvas = GameObject.Find("Canvas");
        playerRigid = GetComponent<Rigidbody>();
        highlightBox = GameObject.Find("chkHighlight");
        fullItem = canvas.transform.Find("fullItemMSG").gameObject;
        nowRot = transform.localRotation;
    }

    private void FixedUpdate()
    {
        Move();
        if (Input.GetMouseButton(2))
        {
            Cursor.lockState = CursorLockMode.Confined;
            Rot();
        }
        else if (Input.GetMouseButtonUp(2))
            Cursor.lockState = CursorLockMode.None;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.tag == "floor")
            IsJump = false;
    }

    void Update()
    {
        if (!IsLayerChange) //무기아이템 GameScene으로 넘어갈 때 weapon layer로 변경 
        {
            IsLayerChange = true;
            layerChange = GM.gameObject.GetComponent<LayerChange>();
            ItemSpawn1 s_itemSpawn = GameObject.Find("itemSpawn").GetComponent<ItemSpawn1>();
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
        if (Input.GetMouseButtonDown(0) && highlightBox.activeSelf == true)
            TouchItem();
    }

    /* 캐릭터 움직임 제어(상하좌우 wsad, 점프 space, 회전 마우스 가운데) : Move, Rot */
    void Move()
    {
        AM.PlayAnimation("Idle");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsJump == false && transform.position.y < 2)
            {
                IsJump = true;
                playerRigid.AddForce(0, 300, 0, ForceMode.Acceleration);
            }
        }
        if (Input.GetKey(KeyCode.D))
            this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.A))
            this.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            this.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.W))
            this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

    }

    void Rot()
    {
        float RotX = Input.GetAxis("Mouse X") * sensibilityX;
        nowRot *= Quaternion.Euler(Vector3.up * RotX);
        transform.rotation = Quaternion.Slerp(transform.localRotation, nowRot, 6 * Time.deltaTime);
    }

    /* 아이템 필드에서 아이템 터치(마우스 왼쪽 버튼 클릭) : TouchItem , fullItemMsgEnd */

    void TouchItem() //아이템 가방이 차기 전 : 아이템 가방 이미지 변경 및 itemPool로 위치 변경, 아이템 가방이 다 찬 후: 경고 메세지 
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(cameraRay, out rayHit))
        {
            GameObject rayObj = rayHit.transform.gameObject;
            Debug.Log("rayHit : " + rayObj.name);
            int possess = (int)eBOOLEAN.FALSE;
            if (rayObj.GetComponentInChildren<outline>() != null) // outline 스크립트 있는 지 여부 판단 
                possess = (int)eBOOLEAN.TRUE;
            
            if(possess == (int)eBOOLEAN.TRUE && rayObj.GetComponentInChildren<outline>().isActiveAndEnabled == true)
            {
                if (rayObj.tag == "item")
                {
                    int num = GM.CitemCount.GetItemNum();
                    num++;
                    int IsMaxItem = GM.CitemCount.changeGetItemNum(num);
                    if (IsMaxItem == (int)eBOOLEAN.FALSE)
                    {
                        fullItem.SetActive(false);
                        int emtyNum = GM.getEmtyImgIndex();
                        GameObject.Find("itemBtnCanvas/btn_GetItem").GetComponent<itemBtn>().InputGetItemArr(rayObj, emtyNum);
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
                else if (rayObj.tag == "weapon" || rayObj.tag == "armor")
                {
                     canvas.GetComponent<weaponArmorBtn>().inputGameObj(rayObj);
                }
            }
        }
    }

    /* 꽉찬 아이템창 알림 메세지 화면 표시 시간 제어 */
    IEnumerator fullItemMsgEnd(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        obj.SetActive(false);
    }

}

