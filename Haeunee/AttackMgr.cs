using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMgr : MonoBehaviour {
    public bool aktPossible = false;
    AnimationController aniCon;
    EnemyScript enemyScript;
    public int playerWeapon = -1;

	void Start () {
        aniCon = GetComponent<AnimationController>();
        enemyScript = GetComponent<EnemyScript>();
    }

    private void OnTriggerEnter(Collider other)
    {//공격 애니메이션 재생되고 있을 때, 무기에만 피격되게 함
        if (aktPossible==true && other.gameObject.layer == (int)eLAYER.WEAPON)
        {
            aktPossible = false;
            HitSucc((int)eATKTYPE.em_NORMAL);
        }
    }

    public void HitSucc(int atkType)
    {//맞은 경우 애니메이션 재생하고 서버에 정보 전송
        int randDmgAni = Random.Range(0, 1);
        sHit hit = new sHit(randDmgAni, atkType, 0);
        if (randDmgAni == 0)
            aniCon.PlayAtkDmg("GetDamage01");
        else if (randDmgAni == 1)
            aniCon.PlayAtkDmg("GetDamage02");
        SocketServer.SingleTonServ().SendMsg(hit);

        if (playerWeapon == (int)eWEAPON.em_SWORDANDSHIELD)
        {
            if (randDmgAni == 0)
                enemyScript.effSound.PlayEff((int)eEFFSOUND.em_SWORD1);
            else if (randDmgAni == 1)
                enemyScript.effSound.PlayEff((int)eEFFSOUND.em_SWORD2);
        }
        else if (playerWeapon == (int)eWEAPON.em_GREATESWORD)
        {
            if (randDmgAni == 0)
                enemyScript.effSound.PlayEff((int)eEFFSOUND.em_SWORD3);
            else if (randDmgAni == 1)
                enemyScript.effSound.PlayEff((int)eEFFSOUND.em_SWORD4);
        }
        else if (playerWeapon == (int)eWEAPON.em_BOW)
            enemyScript.effSound.PlayEff((int)eEFFSOUND.em_ARROWHIT);
        else if (enemyScript.weaponType == (int)eWEAPON.em_WAND)
            enemyScript.effSound.PlayEff((int)eEFFSOUND.em_MAGIHIT);

        Debug.Log("Hit");
    }

    public void AtkPoss(bool atk)
    {
        aktPossible = atk;
    }
}
