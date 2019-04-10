using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitEffect : MonoBehaviour
{
    /* 이펙트 종류 
     HitEffect; //물건을 던졌을 때 나타나는 이펙트
     MagicEffect; //마법 공격시 나타나는 이펙트
     ChinkEffect; //칼 공격시 나타나는 이펙트 
     StickEffect; //화살 공격시 나타나는 이펙트
    */

    EnemyScript s_Enemy;
    Vector3 pos;
    AttackMgr atkMgr;
    ParticleSystem particle;

    public bool getAtkMgr = false; // AttackMgr 한 번만 받아서 저장하기 
    string effectName = ""; //이펙트 이름 구별용 

    private void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        name = this.gameObject.name;
        GameObject obj = other.gameObject;

        if(obj.tag == "Shootable" || obj.tag == "floor" || obj.tag == "Enemy" || obj.tag == "Player")
        {
            particle.Play();
            StartCoroutine(Delay());
            if (name == "HitEffect" && obj.tag == "Enemy" && getAtkMgr == true)
            {
                atkMgr.HitSucc((int)eATKTYPE.em_OBJTHROW);
                Debug.Log("send itemHitSucc");
            }
        }

    }

    private void Update()
    { 
        if(getAtkMgr == true) 
        {
            s_Enemy = SocketServer.SingleTonServ().NowEnemyScript();
            atkMgr = s_Enemy.gameObject.GetComponent<AttackMgr>();
        }
    }

    IEnumerator Delay() // 1초 후 이펙트 위치 변경 
    {
        yield return new WaitForSeconds(1);
        transform.position = new Vector3(1000, 1000, 1000);
        particle.Stop();
    }
}
