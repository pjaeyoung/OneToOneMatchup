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
                    transform.Translate(Vector3.forward * playerSpeed / 10);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    transform.Translate(Vector3.back * playerSpeed / 10);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.Translate(Vector3.right * playerSpeed / 10);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    transform.Translate(Vector3.left * playerSpeed / 10);
                }
                playerAniCon.PlayAnimation("Move");
            }
            else //가만히 있는 애니
            {
                playerAniCon.PlayAnimation("Idle");
            }

            if (Input.GetKey(KeyCode.Space) && transform.position.y <= 0.6f) //점프
            {
                playerRigidBody.AddForce(new Vector3(0, 150, 0));
            }

            if (aniEnd == true)
            {
                aniEnd = false;
                if (weaponNum == (int)eWEAPON.em_BOW || weaponNum == (int)eWEAPON.em_WAND)
                    shotMgr.Shooting();
                else
                    enemyAtk.AtkPoss(false);
            }
        }

    }

    void Update()
    {
        if(Block.activeSelf == false)
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

        if (damaged==true)
        {
            damaged = false;
            if (dmgAni == 0)
                playerAniCon.PlayAtkDmg("GetDamage01");
            else if (dmgAni == 1)
                playerAniCon.PlayAtkDmg("GetDamage02");
            Debug.Log("Damaged");
        }

        if (nowHp != playerHp)
        {
            playerHp = nowHp;
            playerHPBar.changeHpBar(playerHp);
            hpText.text = "Player Hp: " + playerHp;
            if(playerHp <= 0)
            {
                playerAniCon.PlayDeath("Death");
                StartCoroutine(DeathEnd(playerAniCon.GetAniLength("Death")));
            }
        }

        if(gameEnd == true)
        {
            SceneManager.LoadScene("EndScene");
            gameEnd = false;
        }
    }

  
    public void PlayerDamage(sHit hit)
    {
        damaged = true;
        dmgAni = hit.dmgAni;
    }

    public void ChangePlayerHp(int hp)
    {
        if(nowHp != hp)
        {
            nowHp = hp;
        }
    }

    public void ChangePlayerSpeed(int speed)
    {
        playerSpeed = speed;
    }

    public void ChangeWaitScene()
    {
        StartCoroutine(changeDelay());
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
    {
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

    IEnumerator changeDelay()
    {
        yield return new WaitForSeconds(2.0f);
        gameEnd = true;
    }
}
