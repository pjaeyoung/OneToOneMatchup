using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//적 유저 움직임 스크립트
public class EnemyScript : MonoBehaviour
{
    Vector3 enemyPos;
    Quaternion enemyRot;
    AnimationController playerAniCon;
    bool atkAni = false;
    int atkAniNum = 0;
    public int weaponType = -1;
    ShotManager shotMgr;
    int enemyHp = 0;
    Text hpText;
    HpBar enemyHpBar;
    int nowHp;
    GameObject getObj;
    itemSpawn2 s_itemSpawn2;
    bool objGet = false;
    int objNum;
    bool objThrow = false;
    Vector3 targetPos;

    public EffSoundController effSound;

    void Start()
    {
        DontDestroyOnLoad(transform.parent);
        playerAniCon = GetComponent<AnimationController>();
        weaponType = GetComponent<HeroCustomize>().IndexWeapon.CurrentType;
        shotMgr = GetComponentInChildren<ShotManager>();
        shotMgr.ShotPosChange(weaponType);
        hpText = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<Text>();
        enemyHpBar = transform.Find("Canvas").transform.GetChild(0).GetComponent<HpBar>();
        s_itemSpawn2 = GameObject.Find("itemSpawnArr").GetComponent<itemSpawn2>();
        effSound = gameObject.GetComponentInChildren<EffSoundController>();
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

        if(MoveLimit(enemyPos.x, transform.position.x) ||
            MoveLimit(enemyPos.y, transform.position.y)|| MoveLimit(enemyPos.z, transform.position.z)) //움직임
            transform.position = Vector3.Lerp(transform.position, enemyPos, 0.5f);

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
            if (weaponType == (int)eWEAPON.em_BOW)
                effSound.PlayEff((int)eEFFSOUND.em_ARROW);
            else
            {
                if (atkAniNum % 2 == 0)
                    effSound.PlayEff((int)eEFFSOUND.em_SWING1);
                if (atkAniNum % 2 == 1)
                    effSound.PlayEff((int)eEFFSOUND.em_SWING2);
            }
            StartCoroutine(EndAni(playerAniCon.GetAniLength(atkName)));
        }
        else if (MoveLimit(enemyPos.x, transform.position.x) || MoveLimit(enemyPos.z, transform.position.z))
        {
            playerAniCon.PlayAnimation("Move");//움직임 애니메이션(점프일때 적용x)
            Debug.Log("enemy Move");
        }
        else //가만히 있을 때 애니메이션
        {
            playerAniCon.PlayAnimation("Idle");
        }

        if (enemyRot != transform.rotation) //회전
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, enemyRot, 0.5f);
        }

        if(objGet==true)//물건 들기
        {
            objGet = false;
            getObj = s_itemSpawn2.GetObj(objNum);
            getObj.GetComponent<Rigidbody>().useGravity = false;
            Vector3 newPos = transform.position;
            newPos.y += 5;
            getObj.transform.position = newPos;
        }

        if(objThrow==true)//물건 던지기
        {
            objThrow = false;
            itemCntrl cntrl = getObj.GetComponent<itemCntrl>();
            cntrl.isDestroyOK = true;
            cntrl.TransferItem(targetPos);
        }
    }

    bool MoveLimit(float pos1,float pos2)
    {
        return pos1 + 0.1 < pos2 || pos1 - 0.1 > pos2;
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

    IEnumerator EndAni(float delay)//공격 애니메이션 끝나고 원거리 공격 발사
    {
        yield return new WaitForSeconds(delay);
        if (weaponType == (int)eWEAPON.em_BOW || weaponType == (int)eWEAPON.em_WAND)
            shotMgr.Shooting();
    }

    public void GetThrowObj(int num)
    {
        objGet = true;
        objNum = num;
    }

    public void ThrowObj(Vector3 pos)
    {
        objThrow = true;
        targetPos = pos;
    }
}
