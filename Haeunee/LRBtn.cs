using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LRBtn : MonoBehaviour
{
    public Transform Player;
    bool rot = false;

    public void IsRotOk() //버튼 클릭시 bool 값 변경 
    {
        if (rot)
        {
            rot = false;
            if (gameObject.name == "LBtn")
                transform.GetChild(0).GetComponent<Image>().color = Color.white;
            else
                GetComponentInChildren<Image>().color = Color.white;
        }
        else
        {
            rot = true;
            if (gameObject.name == "LBtn")
                transform.GetChild(0).GetComponent<Image>().color = Color.gray;
            else
                GetComponentInChildren<Image>().color = Color.gray;
        }
    }

    public void rightRot() //오른쪽 버튼 : 오른쪽 회전
    {
        if(rot)
            Player.Rotate(Vector3.up, -1.5f);
    }

    public void leftRot() //왼쪽 버튼 : 왼쪽 회전 
    {
        if(rot)
            Player.Rotate(Vector3.up, 1.5f);
    }
}
