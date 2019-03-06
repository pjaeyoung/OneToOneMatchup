using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTargetZone : MonoBehaviour
{

    public void drawTargetZone()
    {
        Debug.Log("draw");
        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");
        Vector3 newPos = new Vector3(x, 0, y);
        transform.localPosition += newPos * 10;
    }
}
