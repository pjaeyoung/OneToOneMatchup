using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//카메라 벽 뚫지 않게 하기 위한 스크립트
public class PlayerCameraScript : MonoBehaviour {

    private void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)eLAYER.TOUCHWALL) //카메라가 벽에 부딪히면 레이어를 변경시켜서 벽이 안보이게 함
        {
            other.gameObject.layer = (int)eLAYER.WALL;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == (int)eLAYER.WALL)
        {
            other.gameObject.layer = (int)eLAYER.TOUCHWALL;
        }
    }

	void Update () {
        if (Input.GetMouseButton(2)) //회전
        {
            transform.Rotate(-Input.GetAxisRaw("Mouse Y") * 5, 0, 0);
        }
    }
}
