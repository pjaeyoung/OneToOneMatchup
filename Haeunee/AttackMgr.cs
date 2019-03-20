using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMgr : MonoBehaviour {
    public bool aktPossible = false;
    AnimationController aniCon;
	void Start () {
        aniCon = GetComponent<AnimationController>();
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

        Debug.Log("Hit");
    }

    public void AtkPoss(bool atk)
    {
        aktPossible = atk;
    }
}
