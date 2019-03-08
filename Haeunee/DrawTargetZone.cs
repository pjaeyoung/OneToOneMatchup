using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTargetZone : MonoBehaviour
{
    //targetZone 움직임 한계치
    float maxX = 100.0f;
    float minX = -100.0f;
    float maxZ = 180.0f;
    float minZ = 0.0f;
    
    public void drawTargetZone()
    {
        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");
        Vector3 newPos = new Vector3(x, 0, y);
        Vector3 nowPos = transform.localPosition;
        nowPos += newPos * 15;
        if (nowPos.x > maxX)
            nowPos.x = maxX;
        if (nowPos.x < minX)
            nowPos.x = minX;
        if (nowPos.z > maxZ)
            nowPos.z = maxZ;
        if (nowPos.z < minZ)
            nowPos.z = minZ;
        transform.localPosition = nowPos;
        Debug.Log("x : " + nowPos.x + " y : " + nowPos.y + " z : " + nowPos.z);
    }
}
