using System.Collections;
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
    ItemFieldCntrl GM;
    ShotManager shotMgr;
    GameObject enemyObj;
    Text hpText;
    HpBar playerHPBar;
    GameObject Block;
    GameObject highlightBox;
    Camera playerCamera;
    GameObject getItem = null;
    itemSpawn2 s_itemSpawn2;
    hitEffect s_hitEffect;
    GameObject ChinkEffect;
    Quaternion nowRot;

    public int weaponNum = -1;
    int sensibilityX = 10;
    int atkAni = 0;

    bool idleAni = true;
    bool aniEnd = false;
    bool damaged = false;
    int dmgAni = 0;
    int atkType = 0;
    int playerHp = 0;
    int playerSpeed = 1;
    int nowHp = 0;
    bool gameEnd = false;

    int itemImgNum = 0;
    bool itemImgChange = false;
    bool itemImgSet = false;

    bool IsJump = false; // 공중에 있는 상태면 TRUE, 땅에 닿인 상태면 FALSE
    bool itemPoss = true;

    GameObject[] ItemImg;

    BgmController sound;
    EffSoundController effSound;

    void Awake()
    {
        DontDestroyOnLoad(transform.parent);
        sockServObj = GameObject.Find("GameMgr2/MatchingCntrl");
        playerInfo = sockServObj.GetComponent<GameEnterScript>();
        weaponNum = playerInfo.savCharInfo.weapon;

        spawnInfo = sockServObj.GetComponent<SpawnScript>();

        GM = GameObject.Find("GameMgr2/itemFieldCntrl").GetComponent<ItemFieldCntrl>();
        shotMgr = GetComponentInChildren<ShotManager>();
        shotMgr.ShotPosChange(weaponNum);
        shotMgr.point = GameObject.Find("PointPrefab");
        shotMgr.point.SetActive(false);

        playerRigidBody = GetComponent<Rigidbody>();
        playerAniCon = GetComponent<AnimationController>();
        StartCoroutine(MoveDelay()); //플레이어의 정보 전송하는 코루틴
        hpText = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<Text>();
        playerHPBar = GameObject.Find("Canvas").transform.GetChild(3).GetComponent<HpBar>();
        Block = GameObject.Find("Canvas").transform.GetChild(4).gameObject;

        highlightBox = transform.Find("chkHighlight").gameObject;
        playerCamera = transform.Find("Camera").GetComponent<Camera>();

        s_itemSpawn2 = GameObject.Find("itemSpawnArr").GetComponent<itemSpawn2>();
        s_hitEffect = GameObject.Find("HitEffect").GetComponent<hitEffect>();
        ChinkEffect = GameObject.Find("ChinkEffect");

        nowRot = transform.localRotation;

        ItemImg = new GameObject[4];
        ItemImg[(int)eITEM.em_HP_POTION] = GameObject.Find("HpPotionImg").gameObject;
        ItemImg[(int)eITEM.em_SPEED_POTION] = GameObject.Find("SpdPotionImg").gameObject;
        ItemImg[(int)eITEM.em_DAMAGE_UP_POTIOM] = GameObject.Find("AtkPotionImg").gameObject;
        ItemImg[(int)eITEM.em_DEFENCE_UP_POTION] = GameObject.Find("DefPotionImg").gameObject;
        for (int i = 0; i < 4; i++)
            ItemImg[i].SetActive(false);

        sound = GameObject.Find("GameMgr").GetComponent<BgmController>();
        effSound = gameObject.GetComponentInChildren<EffSoundController>();
    }

    private void FixedUpdate()
    {
        if (Block.activeSelf == false)
        {
            if (Input.GetMouseButtonDown(0) && highlightBox.activeSelf == true) //하이라이트 박스 출력 중일 때만 아이템 클릭 가능
            {
                itemClick();
            }
            else if (getItem != null)
            {
                if (Input.GetMouseButton(0))
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Rot();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Cursor.lockState = CursorLockMode.None;
                    ItemThrow();
                }
            }
            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Confined;
                Rot();
            }
            else if (Input.GetMouseButtonUp(1))
                Cursor.lockState = CursorLockMode.None;
            if (Input.GetMouseButtonDown(0) && idleAni == true && getItem == null)
                Attack();
            else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
            {
                if (getItem == null)
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
            IsJump = false;
    }

    void Update()
    {
        if (Block.activeSelf == false)
        {
            if(itemPoss==true)
            {
                useItem();
            }
        }

        if (damaged == true) //데미지를 받았을 때 애니메이션 재생
            DamageAniAct();

        if (nowHp != playerHp) //hp변했을 때
            changeHPTextAndDeathAniAct();

        if (itemImgChange == true) //아이템을 사용하여 아이템 이미지가 바뀐 경우
            setItemImg();

        if (gameEnd == true) //게임 끝
            changeEndScene();
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

        if (weaponNum == (int)eWEAPON.em_BOW)
            effSound.PlayEff((int)eEFFSOUND.em_ARROW);
        else
        {
            if (atkAni % 2 == 0)
                effSound.PlayEff((int)eEFFSOUND.em_SWING1);
            if (atkAni % 2 == 1)
                effSound.PlayEff((int)eEFFSOUND.em_SWING2);
        }

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
        if (!IsJump)
        {
            IsJump = true;
            playerRigidBody.AddForce(0, 300, 0, ForceMode.Acceleration);
        }
    }

    void shotMgrStart()
    {
        aniEnd = false;
        if (weaponNum == (int)eWEAPON.em_BOW || weaponNum == (int)eWEAPON.em_WAND)
            shotMgr.Shooting(0); //원거리 샷 발사
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

    void DamageAniAct() // 피격 애니메이션 
    {
        damaged = false;
        if (dmgAni == 0)
            playerAniCon.PlayAtkDmg("GetDamage01");
        else if (dmgAni == 1)
            playerAniCon.PlayAtkDmg("GetDamage02");
        if (spawnInfo.enemyInfo.weapon == (int)eWEAPON.em_GREATESWORD || spawnInfo.enemyInfo.weapon == (int)eWEAPON.em_SWORDANDSHIELD)
            ChinkEffect.transform.position = transform.position + Vector3.up * 2;
        Debug.Log("Damaged");
        EnemyScript enemyScript = enemyObj.GetComponent<EnemyScript>();
        if (atkType == (int)eATKTYPE.em_NORMAL)
        {
            if (enemyScript.weaponType == (int)eWEAPON.em_SWORDANDSHIELD)
            {
                if (dmgAni == 0)
                    effSound.PlayEff((int)eEFFSOUND.em_SWORD1);
                else if (dmgAni == 1)
                    effSound.PlayEff((int)eEFFSOUND.em_SWORD2);
            }
            else if (enemyScript.weaponType == (int)eWEAPON.em_GREATESWORD)
            {
                if (dmgAni == 0)
                    effSound.PlayEff((int)eEFFSOUND.em_SWORD3);
                else if (dmgAni == 1)
                    effSound.PlayEff((int)eEFFSOUND.em_SWORD4);
            }
            else if (enemyScript.weaponType == (int)eWEAPON.em_BOW)
                effSound.PlayEff((int)eEFFSOUND.em_ARROWHIT);
            else if (enemyScript.weaponType == (int)eWEAPON.em_WAND)
                effSound.PlayEff((int)eEFFSOUND.em_MAGIHIT);

        }
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
                hitObj.GetComponent<Rigidbody>().useGravity = false;

                int itemNum = s_itemSpawn2.GetObjNum(hitObj);
                sGetObj getObj = new sGetObj(itemNum);
                SocketServer.SingleTonServ().SendMsg(getObj);
                Debug.Log("send Succ");
                transform.Find("Canvas").Find("ThrowPoint").gameObject.SetActive(true);
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

    void ItemThrow()
    {
        getItem.GetComponent<Rigidbody>().useGravity = true;
        getItem.GetComponent<Rigidbody>().velocity = transform.forward * 15;
        itemCntrl cntrl = getItem.GetComponent<itemCntrl>();
        cntrl.isDestroyOK = true;
        getItem = null;
        transform.Find("Canvas").Find("ThrowPoint").gameObject.SetActive(false);

        sThrowObj throwObj = new sThrowObj((int)eMSG.em_THROWOBJ);
        SocketServer.SingleTonServ().SendMsg(throwObj);
    }

    public void PlayerDamage(sHit hit) // 플레이어가 피격 당했다는 정보 받음 
    {
        damaged = true;
        dmgAni = hit.dmgAni;
        atkType = hit.atkType;
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

    void changeEndScene() // EndScene 전환 
    {
        SceneManager.LoadScene("EndScene");
        gameEnd = false;
    }

    public void GameEndTrue() //게임이 끝났다는 정보 
    {
        gameEnd = true;
    }

    public void passOnItemSpawnInfo(int[] result) //itemSpawn2 스크립트에 서버에서 받은 정보 전달
    {
        s_itemSpawn2.setItemSpawns(result);
    }

    public void GetAtkMgrFromServer(bool b) //AttackMgr 스크립트를 hitEffect에 전달 
    {
        s_hitEffect.getAtkMgr = b;
    }

    IEnumerator MoveDelay() //0.031초마다 플레이어의 위치, 회전을 상대 유저에게 보냄
    {
        yield return new WaitForSeconds(0.31f);
        enemyObj = spawnInfo.nowEnemy;
        enemyAtk = enemyObj.GetComponent<AttackMgr>();
        enemyAtk.playerWeapon = weaponNum;
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
        sEnd dead = new sEnd(0, null);
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