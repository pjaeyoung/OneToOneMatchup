using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    GameObject highlightBox;
    GameObject targetZone;
    DrawTargetZone s_drawTZ;
    Camera playerCamera;
    GameObject getItem = null;

    public int isDestroyOK = (int)eBOOLEAN.FALSE;

    private void Awake()
    {
        highlightBox = transform.Find("chkHighlight").gameObject;
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        targetZone = transform.Find("targetZone").gameObject;
        s_drawTZ = targetZone.GetComponent<DrawTargetZone>();
    }

    private void FixedUpdate()
    {
        //Debug.Log("offset : " + offset);
        if(getItem == null)
        {
            Move();
        }
        Rot();
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * 20 * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * 20 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * 20 * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * 20 * Time.deltaTime);
        }
    }

    void Rot()
    {
        if (Input.GetMouseButton(2))
        {
            float y = Input.GetAxis("Mouse X") * 10;
            Vector3 rot = new Vector3(0, y, 0);
            transform.Rotate(rot, Space.World);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && highlightBox.activeSelf == true)
        {
            itemClick();
        }
        if(getItem != null)
        {
            if (Input.GetMouseButton(1))
            {
                targetZone.SetActive(true);
                s_drawTZ.drawTargetZone();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Vector3 newPos = targetZone.transform.position;
                targetZone.SetActive(false);
                getItem.GetComponent<Rigidbody>().useGravity = true;
                isDestroyOK = (int)eBOOLEAN.TRUE;
                TransferItem(newPos);
            }
        }
    }

    void itemClick()
    {
        highlightBox.SetActive(false);
        Ray cameraRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if(Physics.Raycast(cameraRay, out rayHit))
        {
            GameObject hitObj = rayHit.transform.gameObject;
            if(hitObj.layer == (int)eLAYER.TOUCHABLE)
            {
                hitObj.GetComponent<Rigidbody>().useGravity = false;
                Vector3 newPos = transform.position;
                newPos.y += 5;
                hitObj.transform.position = newPos;
                getItem = hitObj;
            }
            else
            {
                highlightBox.SetActive(true);
            }
        }
    }

    void TransferItem(Vector3 TZPos)
    {
        Vector3 dir = TZPos - getItem.transform.localPosition;
        Debug.Log("dir.x : " + dir.x + " dir.y : " + dir.y);
        getItem.GetComponent<Rigidbody>().velocity = getItem.transform.TransformDirection(dir.x, 0, dir.z);
    }
}
