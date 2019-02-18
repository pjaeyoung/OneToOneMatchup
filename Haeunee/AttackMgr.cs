using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMgr : MonoBehaviour {
    public bool aktPossible = false;
	void Start () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (aktPossible==true && other.gameObject.layer ==10)
        {
            aktPossible = false;
            Debug.Log("Hit");
        }
    }

    public void AtkPoss()
    {
        aktPossible = true;
        Invoke("ImpossAtk", 0.5f);
    }

    void ImpossAtk()
    {
        aktPossible = false;
    }
}
