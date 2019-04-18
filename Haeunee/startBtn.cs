using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startBtn : MonoBehaviour
{
    public GameObject loginWin;
    Animator anim;
    bool loginWinAct = false;

    private void Awake()
    {
        GetComponent<Image>().enabled = false;
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(ActiveDelay(1));
    }

    public void ActiveLoginWin() //로그인 창 활성화 
    {
        loginWinAct = true;
        anim.SetBool("act", true);
        StartCoroutine(ActiveDelay(2));
    }

    IEnumerator ActiveDelay(float f)
    {
        yield return new WaitForSeconds(f);
        if(loginWinAct == false)
        {
            GetComponent<Image>().enabled = true;
            anim.SetBool("prepare", true);
        }
        else
        {
            loginWin.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

}
