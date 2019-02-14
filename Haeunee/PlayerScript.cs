using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어를 움직이는 스크립트
public class PlayerScript : MonoBehaviour
{
    GameObject sockServObj; //서버 오브젝트
    SocketServer sockServ; //서버 스크립트
    Rigidbody playerRigidBody;
    AnimationController playerAniCon; //애니메이션
    int sensibilityX = 5;
    int atkAni = 0;

    void Start()
    {
        sockServObj = GameObject.Find("SocketServer");
        sockServ = sockServObj.GetComponent<SocketServer>();
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

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
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
            playerAniCon.PlayAnimation("Move");
        }
        else //가만히 있는 애니
        {
            playerAniCon.PlayAnimation("Idle");
        }

        if (Input.GetKey(KeyCode.Space)&&transform.position.y <= 0.3f) //점프
        {
            transform.Translate(Vector3.up * 0.5f);
        }
        if (Input.GetMouseButtonDown(0)) //공격
        {
            if (atkAni == 0)
                playerAniCon.PlayAnimation("Attack01");
            else if (atkAni == 1)
                playerAniCon.PlayAnimation("Attack02");
            else if (atkAni == 2)
                playerAniCon.PlayAnimation("Critical01");
            else if (atkAni == 3)
                playerAniCon.PlayAnimation("Critical02");
            sAtk atk = new sAtk((int)eMSG.em_ATK, atkAni);
            sockServ.SendMsg(atk);
            atkAni++;
            if (atkAni >= 4)
                atkAni = 0;
        }
    }

    IEnumerator MoveDelay() //0.031초마다 플레이어의 위치, 회전을 상대 유저에게 보냄
    {
        while (true)
        {
            yield return new WaitForSeconds(0.031f);
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            sMove move = new sMove((int)eMSG.em_MOVE, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w);
            sockServ.SendMsg(move);
        }
    }
}
