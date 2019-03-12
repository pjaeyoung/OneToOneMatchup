using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour {
    public Vector3 rayPoint;
	
	void Start () {
        Destroy(gameObject, 0.3f);
    }	
    
	void Update () {
        transform.Translate(rayPoint * 0.15f);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer== (int)eLAYER.ENEMY || other.tag == "Shootable")
        {
            Destroy(gameObject);
        }
    }
}
