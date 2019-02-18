using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour {
    public Vector3 rayPoint;
	
	void Start () {
        Destroy(gameObject, 2.0f);
    }	
    
	void Update () {
        transform.Translate(rayPoint * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==11)
        {
            Destroy(gameObject);
            Debug.Log("Hit");
        }
    }
}
