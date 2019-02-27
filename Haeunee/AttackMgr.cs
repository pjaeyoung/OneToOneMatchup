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
    {
        if (aktPossible==true && other.gameObject.layer == (int)eLAYER.WEAPON)
        {
            aktPossible = false;
            HitSucc();
        }
    }

    void HitSucc()
    {
        int randDmgAni = Random.Range(0, 1);
        sHit hit = new sHit((int)eMSG.em_HIT, randDmgAni, 0);
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
