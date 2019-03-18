using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitEffect : MonoBehaviour
{
    EnemyScript s_Enemy;
    AttackMgr atkMgr;
    public bool IsAtkMgr = false;
    int AtkMgrCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        StartCoroutine(Delay());
        if(obj.tag == "Enemy" && IsAtkMgr == true)
        {
            atkMgr.HitSucc((int)eATKTYPE.em_OBJTHROW);
        }
    }

    private void Update()
    { 
        if(IsAtkMgr == true && AtkMgrCount == 0)
        {
            AtkMgrCount = 1;
            s_Enemy = SocketServer.SingleTonServ().NowEnemyScript();
            atkMgr = s_Enemy.gameObject.GetComponent<AttackMgr>();
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);
        transform.position = new Vector3(1000, 1000, 1000);
    }
}
