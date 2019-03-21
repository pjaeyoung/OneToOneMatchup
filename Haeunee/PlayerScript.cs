﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//플레이어를 움직이는 스크립트
public class PlayerScript : MonoBehaviour
{
    GameObject sockServObj; //서버 오브젝트
    GameEnterScript playerInfo;
    SpawnScript spawnInfo;
    Rigidbody playerRigidBody;
    AnimationController playerAniCon; //애니메이션
    AttackMgr enemyAtk;
    GameMgr GM;

    ShotManager shotMgr;
    GameObject enemyObj;

    Text hpText;
    HpBar playerHPBar;

    GameObject Block;

    GameObject highlightBox;
    GameObject targetZone;
    DrawTargetZone s_drawTZ;
    Camera playerCamera;
    GameObject getItem = null;

    itemSpawn2 s_itemSpawn2;
    hitEffect s_hitEffect;

    Quaternion nowRot;

    int weaponNum = -1;
    int sensibilityX = 10;
    int atkAni = 0;

    bool idleAni = true;
    bool aniEnd = false;
    bool damaged = false;
    int dmgAni = 0;
    int playerHp = 0;
    int playerSpeed = 1;
    int nowHp = 0;
    bool gameEnd = false;

    int itemImgNum = 0;
    bool itemImgChange = false;
    bool itemImgSet = false;

    int IsJump; // 공중에 있는 상태면 TRUE, 땅에 닿인 상태면 FALSE
    bool itemPoss = true;

    GameObject[] ItemImg;

    void Awake()
    {
        DontDestroyOnLoad(transform.parent);
        sockServObj = GameObject.Find("GameMgr");
        playerInfo = sockServObj.GetComponent<GameEnterScript>();
        weaponNum = playerInfo.savCharInfo.weapon;

        spawnInfo = sockServObj.GetComponent<SpawnScript>();

        GM = GameObject.Find("itemFieldMgr").GetComponent<GameMgr>();
        shotMgr = GetComponentInChildren<ShotManager>();
        shotMgr.ShotPosChange(weaponNum);

        playerRigidBody = GetComponent<Rigidbody>();
        playerAniCon = GetComponent<AnimationController>();
        StartCoroutine(MoveDelay()); //플레이어의 정보 전송하는 코루틴
        hpText = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<Text>();
        playerHPBar = GameObject.Find("Canvas").transform.GetChild(3).GetComponent<HpBar>();
        Block = GameObject.Find("Canvas").transform.GetChild(4).gameObject;

        highlightBox = transform.Find("chkHighlight").gameObject;
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        targetZone = transform.Find("targetZone").gameObject;
        s_drawTZ = targetZone.GetComponent<DrawTargetZone>();

        s_itemSpawn2 = GameObject.Find("itemSpawnArr").GetComponent<itemSpawn2>();
        s_hitEffect = GameObject.Find("HitEffect").GetComponent<hitEffect>();

        nowRot = transform.localRotation;

        ItemImg = new GameObject[4];
        ItemImg[(int)eITEM.em_HP_POTION] = GameObject.Find("HpPotionImg").gameObject;
        ItemImg[(int)eITEM.em_SPEED_POTION] = GameObject.Find("SpdPotionImg").gameObject;
        ItemImg[(int)eITEM.em_DAMAGE_UP_POTIOM] = GameObject.Find("AtkPotionImg").gameObject;
        ItemImg[(int)eITEM.em_DEFENCE_UP_POTION] = GameObject.Find("DefPotionImg").gameObject;
        for (int i = 0; i < 4; i++)
            ItemImg[i].SetActive(false);

    }

    private void FixedUpdate()
    {
        if (Block.activeSelf == false)
        {
            if (Input.GetMouseButton(2))
            {
                Cursor.lockState = CursorLockMode.Confined;
                Rot();
            }
            else if (Input.GetMouseButtonUp(2))
                Cursor.lockState = CursorLockMode.None;
            if (Input.GetMouseButtonDown(0) && idleAni == true && getItem == null)
                Attack();
            else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
            {
                if(getItem == null)
                    Move();
            }
            else
                playerAniCon.PlayAnimation("Idle");

            if (Input.GetKeyDown(KeyCode.Space)) //점프
                Jump();
            if (aniEnd == true) //애니메이션 다끝나고
                shotMgrStart();
        }
      
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.tag == "floor")
            IsJump = (int)eBOOLEAN.FALSE;
    }

    void Update()
    {
        if (Block.activeSelf == false)
        {
            if(itemPoss==true)
            {
                useItem();
            }
            ActiveBlock();
        }

        if (damaged == true) //데미지를 받았을 때 애니메이션 재생
            DamageAniAct();

        if (nowHp != playerHp) //hp변했을 때
            changeHPTextAndDeathAniAct();

        if (itemImgChange == true) //아이템을 사용하여 아이템 이미지가 바뀐 경우
            setItemImg();

        if (gameEnd == true) //게임 끝
            changeEndScene();

        if (Input.GetMouseButtonDown(1) && highlightBox.activeSelf == true) //하이라이트 박스 출력 중일 때만 아이템 클릭 가능
        {
            itemClick();
        }
        if(getItem != null)
        {
            if (Input.GetMouseButton(1)) //드레그 중일 때 targetZone 출력 및 위치 조정
            {
                ActiveTargetZone();
            }
            else if (Input.GetMouseButtonUp(1)) //마우스에서 손을 뗀 시점의 targetZone 위치로 아이템 이동 
            {
                Vector3 newPos = targetZone.transform.position;
                itemCntrl cntrl = getItem.GetComponent<itemCntrl>();
                cntrl.TransferItem(newPos);
                cntrl.isDestroyOK = true;
                targetZone.SetActive(false);
                getItem = null;
                sThrowObj thObj = new sThrowObj(newPos.x, newPos.y, newPos.z);
                SocketServer.SingleTonServ().SendMsg(thObj);
            }
        }
    }

    void Rot() // 좌우 회전
    {
        float RotX = Input.GetAxis("Mouse X") * sensibilityX;
        nowRot *= Quaternion.Euler(Vector3.up * RotX);
        transform.rotation = Quaternion.Slerp(transform.localRotation, nowRot, 6 * Time.deltaTime);
    }

    void Attack() // 공격(애니메이션 재생, 서버에 정보 전송)
    {
        enemyAtk.AtkPoss(true);
        idleAni = false;
        string atkName = "";
        if (atkAni == 0)
            atkName = "Attack01";
        else if (atkAni == 1)
            atkName = "Attack02";
        else if (atkAni == 2)
            atkName = "Critical01";
        else if (atkAni == 3)
            atkName = "Critical02";
        playerAniCon.PlayAtkDmg(atkName);
        StartCoroutine(EndAni(playerAniCon.GetAniLength(atkName)));
        sAtk atk = new sAtk(atkAni);
        SocketServer.SingleTonServ().SendMsg(atk);
        atkAni++;
        if (atkAni >= 4)
            atkAni = 0;
    }

    void Move() // 움직임 
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * playerSpeed / 20);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * playerSpeed / 20);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * playerSpeed / 20);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * playerSpeed / 20);
        }
        playerAniCon.PlayAnimation("Move");
    }

    void Jump() // 점프 
    {
        if (IsJump == (int)eBOOLEAN.FALSE)
        {
            IsJump = (int)eBOOLEAN.TRUE;
            playerRigidBody.AddForce(0, 300, 0, ForceMode.Acceleration);
        }
    }

    void shotMgrStart()
    {
        aniEnd = false;
        if (weaponNum == (int)eWEAPON.em_BOW || weaponNum == (int)eWEAPON.em_WAND)
            shotMgr.Shooting(); //원거리 샷 발사
        else
            enemyAtk.AtkPoss(false); //근거리 공격 불가능
    }

    void useItem() //아이템 사용
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            sUseItem useItem = new sUseItem(-1, 0, 0);
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                useItem.itemNum = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                useItem.itemNum = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                useItem.itemNum = 2;
            }
            itemPoss = false;
            StartCoroutine(EndItem(useItem.itemNum)); //아이템 끝나는 시간
            int IsItemUsed = GM.changeUsedItemImg(useItem.itemNum); //아이템 사용 되었을 때 서버에 정보 전송
            if (IsItemUsed == (int)eITEMUSE.UNUSED)
                SocketServer.SingleTonServ().SendMsg(useItem);
        }
    }

    void ActiveBlock() // block 활성화 
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //ESC 눌렀을 때 뜨는 창 
        {
            Block.SetActive(true);
            Block.transform.GetChild(0).gameObject.SetActive(true);
            GetComponentInChildren<PlayerCameraScript>().enabled = false;
            this.enabled = false;
        }
    }

    void DamageAniAct() // 피격 애니메이션 
    {
        damaged = false;
        if (dmgAni == 0)
            playerAniCon.PlayAtkDmg("GetDamage01");
        else if (dmgAni == 1)
            playerAniCon.PlayAtkDmg("GetDamage02");
        Debug.Log("Damaged");
    }

    void changeHPTextAndDeathAniAct()
    {
        playerHp = nowHp;
        playerHPBar.changeHpBar(playerHp);
        hpText.text = "Player Hp: " + playerHp;
        if (playerHp <= 0) //죽음
        {
            playerAniCon.PlayDeath("Death");
            StartCoroutine(DeathEnd(playerAniCon.GetAniLength("Death")));
        }
    }

    void setItemImg() // 아이템 사용 이미지 
    {
        itemImgChange = false;
        if(itemImgNum != -1)
            ItemImg[itemImgNum].SetActive(itemImgSet);
    }

    void changeEndScene() // EndScene 전환 
    {
        Debug.Log("changeEndScene()");
        GameObject.Destroy(GM.gameObject);
        SceneManager.LoadScene("EndScene");
        gameEnd = false;
    }

    void itemClick() // 던질 아이템 클릭 가능 여부 체크 및 클릭한 아이템 머리 위로 위치 이동, targetZone 생성 조건 On! 
    {
        highlightBox.SetActive(false);
        Ray cameraRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(cameraRay, out rayHit))
        {
            GameObject hitObj = rayHit.transform.gameObject;
            outline ObjOutline = hitObj.GetComponent<outline>();
            if (hitObj.layer == (int)eLAYER.TOUCHABLE && ObjOutline.isActiveAndEnabled == true)
            {
                Debug.Log("click");
                hitObj.GetComponent<Rigidbody>().useGravity = false;
                Vector3 newPos = transform.position;
                newPos.y += 5;
                hitObj.transform.position = newPos;
                getItem = hitObj;

                int itemNum = s_itemSpawn2.GetObjNum(hitObj);
                sGetObj getObj = new sGetObj(itemNum);
                SocketServer.SingleTonServ().SendMsg(getObj);
                Debug.Log("send Succ");
            }
            else
            {
                highlightBox.SetActive(true);
            }
        }
        else
        {
            highlightBox.SetActive(true);
        }
    }

    void ActiveTargetZone() // targetZone(던질 아이템이 날아갈 곳) 활성화 
    {
        targetZone.SetActive(true);
        s_drawTZ.drawTargetZone();
    }   

    public void PlayerDamage(sHit hit) // 플레이어가 피격 당했다는 정보 받음 
    {
        damaged = true;
        dmgAni = hit.dmgAni;
    }

    public void ChangePlayerHp(int hp) //서버에서 hp변화 받아오기
    {
        if (nowHp != hp)
        {
            nowHp = hp;
        }
    }

    public void ChangePlayerSpeed(int speed) //속도 변화
    {
        playerSpeed = speed;
    }

    public void ChangeItemImg(int itemNum, bool show) // 아이템 이미지를 변경시켜야 한다는 정보
    {
        itemImgChange = true;
        itemImgNum = itemNum;
        itemImgSet = show;
    }

    public void ChangeWaitScene() //게임이 끝났다는 정보 
    {
        Debug.Log("[PlayerScript] ChangeWaitScene()");
        gameEnd = true;
    }

    public void passOnItemSpawnInfo(int[] result) //itemSpawn2 스크립트에 서버에서 받은 정보 전달
    {
        s_itemSpawn2.setItemSpawns(result);
    }

    public void CreateHitEffect(bool b) //아이템 던졌을 때 이펙트 활성화 조건 
    {
        s_hitEffect.IsAtkMgr = b;
    }

    IEnumerator MoveDelay() //0.031초마다 플레이어의 위치, 회전을 상대 유저에게 보냄
    {
        yield return new WaitForSeconds(0.31f);
        enemyObj = spawnInfo.nowEnemy;
        enemyAtk = enemyObj.GetComponent<AttackMgr>();
        while (true)
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            sMove move = new sMove(pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w);
            SocketServer.SingleTonServ().SendMsg(move);
            yield return new WaitForSeconds(0.031f);
        }
    }

    IEnumerator EndAni(float delay)
    {//애니메이션 끝
        yield return new WaitForSeconds(delay);
        idleAni = true;
        aniEnd = true;
    }

    IEnumerator DeathEnd(float delay)
    {
        yield return new WaitForSeconds(delay);
        //서버에 죽었다는 메세지 보내기 
        sEnd dead = new sEnd(0);
        SocketServer.SingleTonServ().SendMsg(dead);
    }

    IEnumerator EndItem(int itemNum)
    {//아이템 지속시간 끝
        yield return new WaitForSeconds(5);
        itemPoss = true;
        sEndItem endItem = new sEndItem(itemNum, 0, 0);
        SocketServer.SingleTonServ().SendMsg(endItem);
    }
}