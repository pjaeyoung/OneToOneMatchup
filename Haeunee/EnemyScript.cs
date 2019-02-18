using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//적 유저 움직임 스크립트
public class EnemyScript : MonoBehaviour {
    Vector3 enemyPos;
    Quaternion enemyRot;
    AnimationController playerAniCon;
    Animation animation;
    bool atkAni = false;
    int atkAniNum = 0;
    int weaponType = -1;
    bool aniEnd = false;
    ShotManager shotMgr;

    void Start () {
        playerAniCon = GetComponent<AnimationController>();
        weaponType = GetComponent<HeroCustomize>().IndexWeapon.CurrentType;
        shotMgr = GetComponentInChildren<ShotManager>();
        animation = GetComponent<Animation>();
        shotMgr.ShotPosChange(weaponType);
    }

    private void Update()
    {
        if (atkAni == true) //공격
        {
            atkAni = false;
            string atkName = "";
            if (atkAniNum == 0)
                atkName = playerAniCon.GetAniName("Attack01");
            else if (atkAniNum == 1)
                atkName = playerAniCon.GetAniName("Attack02");
            else if (atkAniNum == 2)
                atkName = playerAniCon.GetAniName("Critical01");
            else if (atkAniNum == 3)
                atkName = playerAniCon.GetAniName("Critical02");
            animation.Play(atkName);
            StartCoroutine(EndAni(animation[atkName].length - 0.1f));
        }
        else if (enemyPos.x + 0.5 <= transform.position.x || enemyPos.x - 0.5 >= transform.position.x ||
            enemyPos.z + 0.5 <= transform.position.z|| enemyPos.z - 0.5 >= transform.position.z)//움직임
        {
            transform.position = Vector3.Lerp(transform.position, enemyPos, 0.5f);
            animation.CrossFade(playerAniCon.GetAniName("Move"));
        }
        else //가만히 있을 때 애니메이션
        {
            animation.CrossFade(playerAniCon.GetAniName("Idle"));
        }
        if (enemyRot!=transform.rotation) //회전
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, enemyRot, 0.5f);
        }

        if (aniEnd == true)
        {
            aniEnd = false;
            if (weaponType == (int)eWEAPON.em_BOW || weaponType == (int)eWEAPON.em_WAND)
                shotMgr.Shooting();
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

    IEnumerator EndAni(float delay)
    {
        yield return new WaitForSeconds(delay);
        aniEnd = true;
    }
}
