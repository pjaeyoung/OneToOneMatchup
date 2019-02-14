using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//적 유저 움직임 스크립트
public class EnemyScript : MonoBehaviour {
    Vector3 enemyPos;
    Quaternion enemyRot;
    AnimationController playerAniMgr;
    bool atkAni = false;
    int atkAniNum = 0;

    void Start () {
        playerAniMgr = GetComponent<AnimationController>();
    }

    private void Update()
    {
        if (enemyPos.x != transform.position.x|| enemyPos.y != transform.position.y)//움직임
        {
            transform.position = Vector3.Lerp(transform.position, enemyPos, 0.5f);
            playerAniMgr.PlayAnimation("Move");
        }
        else //가만히 있을 때 애니메이션
        {
            playerAniMgr.PlayAnimation("Idle");
        }
        if (enemyRot!=transform.rotation) //회전
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, enemyRot, 0.5f);
        }

        if(atkAni == true) //공격
        {
            atkAni = false;
            if (atkAniNum == 0)
                playerAniMgr.PlayAnimation("Attack01");
            else if (atkAniNum == 1)
                playerAniMgr.PlayAnimation("Attack02");
            else if (atkAniNum == 2)
                playerAniMgr.PlayAnimation("Critical01");
            else if (atkAniNum == 3)
                playerAniMgr.PlayAnimation("Critical02");
        }
    }

    public void EnemyMove(Vector3 pos, Quaternion rot) //움직인 좌표, 회전각 받아오기
    {
        enemyPos = pos;
        enemyRot = rot;
    }

    public void EnemyAtk(int num)//공격 여부와 공격 애니메이션 받아오기
    {
        atkAni = true;
        atkAniNum = num;
    }
}
