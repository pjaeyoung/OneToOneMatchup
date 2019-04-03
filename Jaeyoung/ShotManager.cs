using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotManager : MonoBehaviour {
    Ray ray;
    RaycastHit rayHit;
    public GameObject point;
    float maxDistance = 20;
    int myWeaponType;
    GameObject nowShot;
    GameObject arrow;
    GameObject magic;

    void Start () {
        ray = new Ray();
        arrow = GameObject.Find("ArrowPrefab");
        magic = GameObject.Find("MagicPrefab");
    }
	
	void Update () {
        ray.origin = transform.position;
        ray.direction = -transform.forward;
        if (transform.parent.name=="Player(Clone)")
        {
            if (Physics.Raycast(ray.origin, ray.direction, out rayHit, maxDistance))
            { //레이에 물건, 적이 닿았을 경우 표시
                if (point != null && (rayHit.collider.tag == "Shootable"|| rayHit.collider.tag == "Enemy"))
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
    { //샷 생성
        if (myWeaponType == (int)eWEAPON.em_BOW)
            nowShot = arrow;
        else if (myWeaponType == (int)eWEAPON.em_WAND)
            nowShot = magic;
        nowShot.transform.position = transform.position;
        nowShot.GetComponentInChildren<ShotController>().enabled = true;
        nowShot.transform.GetChild(0).transform.eulerAngles = GetComponentInParent<Transform>().eulerAngles;
        nowShot.GetComponentInChildren<ShotController>().rayPoint = ray.direction * 10;
    }

    public void ShotPosChange(int weaponType)
    { //무기에 따라 샷이 날아갈 곳 변경
        myWeaponType = weaponType;
        if (weaponType==(int)eWEAPON.em_WAND)
            transform.localPosition = new Vector3(4, 20, 17);
        else if (weaponType == (int)eWEAPON.em_BOW)
            transform.localPosition = new Vector3(6, 12, 22);
        else
        {
            GetComponent<ShotManager>().enabled = false;
        }
    }

    public void FindPoint() //닿은 부분 표시 찾기
    {
        point = GameObject.Find("PointPrefab");
        point.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green);
    }
}
