using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour {
    public Vector3 rayPoint;
    GameObject MagicEffect;
    GameObject StickEffect;

    private void Awake()
    {
        MagicEffect = GameObject.Find("MagicEffect");
        StickEffect = GameObject.Find("StickEffect");
    }

    void Start ()
    {
        Destroy(transform.parent.gameObject, 0.35f); //0.35초 후 파괴
    }	
    
	void Update () //플레이어의 레이 방향으로 이동
    {
        transform.parent.Translate(rayPoint * 0.15f);
	}

    private void OnTriggerEnter(Collider other)
    {//물건이나, 적에 닿으면 파괴
        if(other.gameObject.layer == (int)eLAYER.ENEMY || other.tag == "Shootable" || other.tag == "Player")
        {
            if (transform.name == "Magic")
                MagicEffect.transform.position = other.gameObject.transform.position;
            else if (transform.name == "Arrow")
                StickEffect.transform.position = other.gameObject.transform.position + Vector3.up * 2;
            Destroy(transform.parent.gameObject);
        }
    }
}
