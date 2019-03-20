﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//카메라 벽 뚫지 않게 하기 위한 스크립트
public class PlayerCameraScript : MonoBehaviour {
    float MouseY = 0;

    private void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {//카메라가 벽에 부딪히면 레이어를 변경시켜서 벽이 안보이게 함
        if (other.gameObject.layer == (int)eLAYER.TOUCHWALL) 
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

	void FixedUpdate () {
        //카메라 위아래 회전
        if (Input.GetMouseButton(2))
        {
            MouseY += Input.GetAxisRaw("Mouse Y");
            if (MouseY < -7 || MouseY > 10)
                return;
            else
            {
                transform.Rotate(Vector3.left, Input.GetAxis("Mouse Y") * 3);
            }
        }
    }
}
