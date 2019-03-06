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
    GameMgr GM;

    ShotManager shotMgr;
    GameObject enemyObj;

    Text hpText;
    HpBar playerHPBar;

    GameObject Block;

    int weaponNum = -1;
    int sensibilityX = 5;
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
    bool ItemimgSet = false;

    GameObject[] ItemImg;

    void Awake()
    {
        sockServObj = GameObject.Find("SocketServer");
        playerInfo = sockServObj.GetComponent<GameEnterScript>();
        weaponNum = playerInfo.savCharInfo.weapon;

        spawnInfo = sockServObj.GetComponent<SpawnScript>();

        GM = GameObject.Find("gameMgr").GetComponent<GameMgr>();
        shotMgr = GetComponentInChildren<ShotManager>();
        shotMgr.ShotPosChange(weaponNum);

        playerRigidBody = GetComponent<Rigidbody>();
        playerAniCon = GetComponent<AnimationController>();
        StartCoroutine(MoveDelay()); //플레이어의 정보 전송하는 코루틴
        hpText = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<Text>();
        playerHPBar = GameObject.Find("Canvas").transform.GetChild(3).GetComponent<HpBar>();
        Block = GameObject.Find("Canvas").transform.GetChild(4).gameObject;

        ItemImg = new GameObject[4];
        ItemImg[(int)eITEM.em_HP_POTION] = GameObject.Find("HpPotionImg");
        ItemImg[(int)eITEM.em_SPEED_POTION] = GameObject.Find("SpdPotionImg");
        ItemImg[(int)eITEM.em_DAMAGE_UP_POTIOM] = GameObject.Find("AtkPotionImg");
        ItemImg[(int)eITEM.em_DEFENCE_UP_POTION] = GameObject.Find("DefPotionImg");
        for (int i = 0; i < 4; i++)
            ItemImg[i].SetActive(false);
    }

    private void FixedUpdate()
    {
        if(Block.activeSelf == false)
        {
            if (Input.GetMouseButton(2)) //회전
            {
                transform.Rotate(0, Input.GetAxisRaw("Mouse X") * sensibilityX, 0);
            }
            if (Input.GetMouseButtonDown(0) && idleAni == true) //공격
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
            else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
            { //움직임, 움직이는 애니
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
            else //가만히 있는 애니
            {
                playerAniCon.PlayAnimation("Idle");
            }

            if (Input.GetKeyDown(KeyCode.Space) && transform.position.y <= 0.6f) //점프
            {
                playerRigidBody.AddForce(new Vector3(0, 150, 0));
            }

            if (aniEnd == true) //애니메이션 다씉나고
            {
                aniEnd = false;
                if (weaponNum == (int)eWEAPON.em_BOW || weaponNum == (int)eWEAPON.em_WAND)
                    shotMgr.Shooting(); //원거리 샷 발사
                else
                    enemyAtk.AtkPoss(false); //근거리 공격 불가능
            }
        }

    }

    void Update()
    {
        if(Block.activeSelf == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
            {//아이템 사용
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
                StartCoroutine(EndItem(useItem.itemNum)); //아이템 끝나는 시간
                int IsItemUsed = GM.changeUsedItemImg(useItem.itemNum); //아이템 한 번 사용 시 더 이상 사용 못함 
                if(IsItemUsed == (int)eITEMUSE.UNUSED) 
                    SocketServer.SingleTonServ().SendMsg(useItem);
            }
            if (Input.GetKeyDown(KeyCode.Escape)) //ESC 눌렀을 때 뜨는 창 
            {
                Block.SetActive(true);
                Block.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        if (damaged==true) //데미지를 받았을 때 애니메이션 재생
        {
            damaged = false;
            if (dmgAni == 0)
                playerAniCon.PlayAtkDmg("GetDamage01");
            else if (dmgAni == 1)
                playerAniCon.PlayAtkDmg("GetDamage02");
            Debug.Log("Damaged");
        }

        if (nowHp != playerHp) //hp변했을 때
        {
            playerHp = nowHp;
            playerHPBar.changeHpBar(playerHp);
            hpText.text = "Player Hp: " + playerHp;
            if(playerHp <= 0) //죽음
            {
                playerAniCon.PlayDeath("Death");
                StartCoroutine(DeathEnd(playerAniCon.GetAniLength("Death")));
            }
        }

        if(itemImgChange == true)
        {
            itemImgChange = false;
            ItemImg[itemImgNum].SetActive(ItemimgSet);
        }

        if(gameEnd == true) //게임 끝
        {
            GameObject.Destroy(GM.gameObject);
            SceneManager.LoadScene("EndScene");
            gameEnd = false;
        }
    }
  
    public void PlayerDamage(sHit hit)
    {
        damaged = true;
        dmgAni = hit.dmgAni;
    }

    public void ChangePlayerHp(int hp) //서버에서 hp변화 받아오기
    {
        if(nowHp != hp)
        {
            nowHp = hp;
        }
    }

    public void ChangePlayerSpeed(int speed) //속도 변화
    {
        playerSpeed = speed;
    }

    public void ChangeItemImg(int itemNum, bool show)
    {
        itemImgChange = true;
        itemImgNum = itemNum;
        ItemimgSet = show;
    }

    public void ChangeWaitScene()
    {
        gameEnd = true;
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
        sEndItem endItem = new sEndItem(itemNum,0,0);
        SocketServer.SingleTonServ().SendMsg(endItem);
    }
}
