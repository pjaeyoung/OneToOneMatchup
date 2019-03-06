using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//적 유저 움직임 스크립트
public class EnemyScript : MonoBehaviour {
    Vector3 enemyPos;
    Quaternion enemyRot;
    AnimationController playerAniCon;
    bool atkAni = false;
    int atkAniNum = 0;
    int weaponType = -1;
    ShotManager shotMgr;
    int enemyHp = 0;
    Text hpText;
    HpBar enemyHpBar;
    int nowHp;

    void Start () {
        playerAniCon = GetComponent<AnimationController>();
        weaponType = GetComponent<HeroCustomize>().IndexWeapon.CurrentType;
        shotMgr = GetComponentInChildren<ShotManager>();
        shotMgr.ShotPosChange(weaponType);
        hpText = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<Text>();
        enemyHpBar = transform.Find("Canvas").transform.GetChild(0).GetComponent<HpBar>();
    }

    private void Update()
    {
        if (enemyHp != nowHp)
        {
            enemyHp = nowHp;
            enemyHpBar.changeHpBar(enemyHp);
            hpText.text = "Enemy Hp: " + enemyHp;
            if (enemyHp <= 0)
                playerAniCon.PlayDeath("Death");
        }

        if (atkAni == true) //공격
        {
            atkAni = false;
            string atkName = "";
            if (atkAniNum == 0)
                atkName = "Attack01";
            else if (atkAniNum == 1)
                atkName = "Attack02";
            else if (atkAniNum == 2)
                atkName = "Critical01";
            else if (atkAniNum == 3)
                atkName = "Critical02";
            playerAniCon.PlayAtkDmg(atkName);
            StartCoroutine(EndAni(playerAniCon.GetAniLength(atkName)));
        }
        else if (enemyPos.x + 0.1 <= transform.position.x || enemyPos.x - 0.1 >= transform.position.x ||
            enemyPos.z + 0.1 <= transform.position.z|| enemyPos.z - 0.1 >= transform.position.z)//움직임
        {
            transform.position = Vector3.Lerp(transform.position, enemyPos, 0.5f);
            playerAniCon.PlayAnimation("Move");
        }
        else //가만히 있을 때 애니메이션
        {
            playerAniCon.PlayAnimation("Idle");
        }
        if (enemyRot!=transform.rotation) //회전
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, enemyRot, 0.5f);
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

    public void ChangeEnemyHp(int hp)
    {
        if (nowHp != hp)
        {
            nowHp = hp;
        }
    }

    IEnumerator EndAni(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (weaponType == (int)eWEAPON.em_BOW || weaponType == (int)eWEAPON.em_WAND)
            shotMgr.Shooting();
    }
}
