using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour {
    public Vector3 rayPoint;
	
	void Start () {
        Destroy(gameObject, 0.35f); //0.35초 후 파괴
    }	
    
	void Update () {//플레이어의 레이 방향으로 이동
        transform.Translate(rayPoint * 0.15f);
	}

    private void OnTriggerEnter(Collider other)
    {//물건이나, 적에 닿으면 파괴
        if(other.gameObject.layer== (int)eLAYER.ENEMY || other.tag == "Shootable")
        {
            Destroy(gameObject);
        }
    }
}
