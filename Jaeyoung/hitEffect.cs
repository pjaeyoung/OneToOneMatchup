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
    AttackMgr atkMgr;
    ParticleSystem particle;
    EffSoundController sound;
    Vector3 waitPos = new Vector3(1000, 0, 1000); //이펙트 풀 대기 위치 
    public bool effStart = false; //이펙트 발동 
    public bool getAtkMgr = false; // AttackMgr 한 번만 받아서 저장하기 

    private void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        sound = gameObject.GetComponentInChildren<EffSoundController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        name = this.gameObject.name;
        GameObject obj = other.gameObject;
  
        if(obj.tag == "Shootable" || obj.tag == "floor" || obj.tag == "Enemy" || obj.tag == "Player")
        {
            if (name != "ChinkEffect")
                sound.PlayEff((int)eEFFSOUND.em_BOMB);
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
            if(s_Enemy != null)
                atkMgr = s_Enemy.gameObject.GetComponent<AttackMgr>();
        }
        if (effStart)
        {
            effStart = false;
            particle.Play();
            StartCoroutine(Delay());
        }
    }

    IEnumerator Delay() // 1초 후 이펙트 위치 변경 
    {
        if(particle.gameObject.transform.parent.name == "MagicEffect")
        {
            yield return new WaitForSeconds(4f);
            transform.position = waitPos;
            particle.Stop();
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            transform.position = waitPos;
            particle.Stop();
        }
    }
}
