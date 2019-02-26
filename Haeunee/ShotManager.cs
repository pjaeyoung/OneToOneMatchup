using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotManager : MonoBehaviour {
    Ray ray;
    RaycastHit rayHit;
    public GameObject point;
    public GameObject ball;
    public GameObject arrow;
    float maxDistance = 20;
    int myWeaponType;
    GameObject nowShot;

    void Start () {
        ray = new Ray();
        FindPoint();
    }
	
	// Update is called once per frame
	void Update () {
        ray.origin = transform.position;
        ray.direction = -transform.forward;
        if (transform.parent.name=="Player(Clone)")
        {
            if (Physics.Raycast(ray.origin, ray.direction, out rayHit, maxDistance))
            {
                if (point != null && rayHit.collider.tag == "Shootable")
                {
                    point.SetActive(true);
                    point.transform.position = rayHit.point;
                    point.transform.localRotation = Quaternion.FromToRotation(Vector3.up, rayHit.normal);
                }
            }
            else if (point != null)
            {
                point.SetActive(false);
            }
        }
    }

    public void Shooting()
    {
        if (myWeaponType == (int)eWEAPON.em_BOW)
            nowShot = Instantiate(arrow, transform.position, Quaternion.identity);
        else if (myWeaponType == (int)eWEAPON.em_WAND)
            nowShot = Instantiate(ball, transform.position, Quaternion.identity);
        nowShot.transform.GetChild(0).transform.eulerAngles = GetComponentInParent<Transform>().eulerAngles;
        nowShot.GetComponent<ShotController>().rayPoint = ray.direction * 10;
    }

    public void ShotPosChange(int weaponType)
    {
        myWeaponType = weaponType;
        if (weaponType==(int)eWEAPON.em_WAND)
            transform.localPosition = new Vector3(8, 20, 0);
        else if (weaponType == (int)eWEAPON.em_BOW)
            transform.localPosition = new Vector3(0, 12, 7);
        else
        {
            GetComponent<ShotManager>().enabled = false;
        }
    }

    void FindPoint()
    {
        point = GameObject.Find("PointPrefab");
        point.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green);
    }
}
