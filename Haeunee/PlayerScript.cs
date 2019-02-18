﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어를 움직이는 스크립트
public class PlayerScript : MonoBehaviour
{
    GameObject sockServObj; //서버 오브젝트
    SocketServer sockServ; //서버 스크립트
    GameEnterScript playerInfo;
    SpawnScript spawnInfo;
    Rigidbody playerRigidBody;
    AnimationController playerAniCon; //애니메이션
    Animation animation;
    AttackMgr enemyAtk;

    ShotManager shotMgr;
    GameObject enemyObj;

    int weaponNum = -1;
    int sensibilityX = 5;
    int atkAni = 0;

    bool idleAni = true;
    bool aniEnd = false;

    void Awake()
    {
        sockServObj = GameObject.Find("SocketServer");
        sockServ = sockServObj.GetComponent<SocketServer>();

        playerInfo = sockServObj.GetComponent<GameEnterScript>();
        weaponNum = playerInfo.savCharInfo.weapon;

        spawnInfo = sockServObj.GetComponent<SpawnScript>();

        shotMgr = GetComponentInChildren<ShotManager>();
        shotMgr.ShotPosChange(weaponNum);

        animation = GetComponent<Animation>();

        playerRigidBody = GetComponent<Rigidbody>();
        playerAniCon = GetComponent<AnimationController>();
        StartCoroutine(MoveDelay()); //플레이어의 정보 전송하는 코루틴
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        if(Input.GetMouseButton(2)) //회전
        {
            transform.Rotate(0, Input.GetAxisRaw("Mouse X") * sensibilityX, 0);
        }
        if (Input.GetMouseButtonDown(0) && idleAni == true) //공격
        {
            idleAni = false;
            string atkName = "";
            if (atkAni == 0)
                atkName = playerAniCon.GetAniName("Attack01");
            else if (atkAni == 1)
                atkName = playerAniCon.GetAniName("Attack02");
            else if (atkAni == 2)
                atkName = playerAniCon.GetAniName("Critical01");
            else if (atkAni == 3)
                atkName = playerAniCon.GetAniName("Critical02");
            animation.Play(atkName);
            StartCoroutine(EndAni(animation[atkName].length - 0.1f));
            sAtk atk = new sAtk((int)eMSG.em_ATK, atkAni);
            sockServ.SendMsg(atk);
            atkAni++;
            if (atkAni >= 4)
                atkAni = 0;
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
        { //움직임, 움직이는 애니
            if (Input.GetKey(KeyCode.W))
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
            animation.CrossFade(playerAniCon.GetAniName("Move"));
        }
        else //가만히 있는 애니
        {
            animation.CrossFade(playerAniCon.GetAniName("Idle"));
        }

        if (Input.GetKey(KeyCode.Space)&&transform.position.y <= 0.6f) //점프
        {
            playerRigidBody.AddForce(new Vector3(0, 150, 0));
        }

        if (aniEnd == true)
        {
            aniEnd = false;
            if (weaponNum ==(int)eWEAPON.em_BOW|| weaponNum==(int)eWEAPON.em_WAND)
                shotMgr.Shooting();
            else
                enemyAtk.AtkPoss();
        }
    }

    IEnumerator MoveDelay() //0.031초마다 플레이어의 위치, 회전을 상대 유저에게 보냄
    {
        yield return new WaitForSeconds(0.31f);
        enemyObj = spawnInfo.nowEnemy;
        enemyAtk = enemyObj.GetComponent<AttackMgr>();
        shotMgr.FindPoint();
        while (true)
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            sMove move = new sMove((int)eMSG.em_MOVE, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w);
            sockServ.SendMsg(move);
            yield return new WaitForSeconds(0.031f);
        }
    }

    IEnumerator EndAni(float delay)
    {
        yield return new WaitForSeconds(delay);
        idleAni = true;
        aniEnd = true;
    }
}
