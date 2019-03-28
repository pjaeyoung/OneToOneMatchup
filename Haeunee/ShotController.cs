using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour {
    public Vector3 rayPoint;
    EffSoundController effSound;
	
	void Start () {
        StartCoroutine(PosDelay()); //0.35초 후 이동
        effSound = GetComponentInChildren<EffSoundController>();
        effSound.PlayEff((int)eEFFSOUND.em_WIND);
    }	
    
	void Update () {//플레이어의 레이 방향으로 이동
        transform.Translate(rayPoint * 0.15f);
	}

    private void OnTriggerEnter(Collider other)
    {//물건이나, 적에 닿으면 이동
        if(other.gameObject.layer== (int)eLAYER.ENEMY || other.tag == "Shootable")
        {
            gameObject.transform.position = new Vector3(1000, 1000, 1000);
            GetComponent<ShotController>().enabled = false;
        }
    }

    IEnumerator PosDelay()
    {
        yield return new WaitForSeconds(0.35f);
        gameObject.transform.position = new Vector3(1000, 1000, 1000);
        GetComponent<ShotController>().enabled = false;
    }
}
