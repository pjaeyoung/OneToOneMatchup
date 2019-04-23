using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShotController : MonoBehaviour {
    public Vector3 rayPoint;
    GameObject MagicEffect;
    GameObject StickEffect;
    EffSoundController effSound;
    Scene scene;
    public bool setPos = false;

    private void Awake()
    {
        scene = SceneManager.GetActiveScene();
        if (scene.name == "GameScene")
        {
            MagicEffect = GameObject.Find("MagicEffect");
            StickEffect = GameObject.Find("StickEffect");
        }
    }

    void Start ()
    {
        effSound = transform.parent.GetChild(1).GetComponent<EffSoundController>();
        effSound.PlayEff((int)eEFFSOUND.em_WIND);
    }	
    
	void Update () //플레이어의 레이 방향으로 이동
    {
        if(setPos == true)
        {
            setPos = false;
            StartCoroutine(PosDelay()); //0.35초 후 shot pool 로 옮기기 
        }
        transform.parent.Translate(rayPoint * 0.15f);
	}

    private void OnTriggerEnter(Collider other)
    {//물건이나, 적에 닿으면 파괴
        if (other.gameObject.layer == (int)eLAYER.ENEMY || other.tag == "Shootable" || other.tag == "Player")
        {
            transform.parent.transform.position = new Vector3(1000, 1000, 1000);
            GetComponent<ShotController>().enabled = false;
            if (scene.name == "GameScene")
            {
                if (transform.name == "Magic")
                {
                    MagicEffect.transform.position = other.transform.position;
                    MagicEffect.GetComponent<hitEffect>().effStart = true;
                }
                else if (transform.name == "Arrow")
                {
                    StickEffect.transform.position = other.transform.position + Vector3.up * 2;
                    StickEffect.GetComponent<hitEffect>().effStart = true;
                }
            }
        }
    }

    IEnumerator PosDelay()
    {
        yield return new WaitForSeconds(0.35f);
        transform.parent.transform.position = new Vector3(1000, 1000, 1000);
        GetComponent<ShotController>().enabled = false;
    }
}
