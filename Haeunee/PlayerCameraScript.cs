using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//카메라 벽 뚫지 않게 하기 위한 스크립트
public class PlayerCameraScript : MonoBehaviour {

	void Start () {
		
	}

	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Wall") //카메라가 벽에 부딪히면 레이어를 변경시켜서 벽이 안보이게 함
        {
            other.gameObject.layer = 9;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Wall")
        {
            other.gameObject.layer = 0;
        }
    }
}
