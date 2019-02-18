using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
    AnimationController playerAniCon; //애니메이션
    Animation animation;
    bool idleAni = true;
    bool aniEnd = false;
    HeroCustomize custom;
    int sensibilityX = 5;
    int atkAni = 0;
    Rigidbody playerRigidBody;
    ShotManager shotMgr;
    int weaponNum = 4;
    public GameObject enemyObj;
    AttackMgr enemyAtk;

    void Start () {
        playerAniCon = GetComponent<AnimationController>();
        playerRigidBody = GetComponent<Rigidbody>();
        custom = GetComponent<HeroCustomize>();
        custom.IndexWeapon.CurrentType = weaponNum;
        playerAniCon.weaponIndex = weaponNum;
        animation = GetComponent<Animation>();
        shotMgr = GetComponentInChildren<ShotManager>();
        shotMgr.ShotPosChange(weaponNum);
        enemyAtk = enemyObj.GetComponent<AttackMgr>();
    }
	
    private void FixedUpdate()
    {
        if (Input.GetMouseButton(2)) //회전
        {
            transform.Rotate(0, Input.GetAxisRaw("Mouse X") * sensibilityX, 0);
        }

        if (Input.GetMouseButtonDown(0) && idleAni == true) //공격
        {
            idleAni = false;
            string atkName = "";
            if (atkAni == 0)
                atkName = playerAniCon.GetAniName("Attack01");
            else if (atkAni == 1)
                atkName = playerAniCon.GetAniName("Attack02");
            else if (atkAni == 2)
                atkName = playerAniCon.GetAniName("Critical01");
            else if (atkAni == 3)
                atkName = playerAniCon.GetAniName("Critical02");
            animation.Play(atkName);
            StartCoroutine(EndAni(animation[atkName].length - 0.1f));
            sAtk atk = new sAtk((int)eMSG.em_ATK, atkAni);
            atkAni++;
            if (atkAni >= 4)
                atkAni = 0;
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
        { //움직임, 움직이는 애니
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * 0.1f);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * 0.1f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * 0.1f);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * 0.1f);
            }
            animation.CrossFade(playerAniCon.GetAniName("Move"));
        }
        else //가만히 있는 애니
        {
            animation.CrossFade(playerAniCon.GetAniName("Idle"));
        }
        if (Input.GetKey(KeyCode.Space) && transform.position.y <= 0.6f) //점프
        {
            playerRigidBody.AddForce(new Vector3(0, 150, 0));
        }
        if(aniEnd==true)
        {
            aniEnd = false;
            if (weaponNum >= 3)
                shotMgr.Shooting();
            else
                enemyAtk.AtkPoss();
        }
    }

    IEnumerator EndAni(float delay)
    {
        yield return new WaitForSeconds(delay);
        idleAni = true;
        aniEnd = true;
    }

    void Delay()
    {
        idleAni = true;
        aniEnd = true;
    }
}
