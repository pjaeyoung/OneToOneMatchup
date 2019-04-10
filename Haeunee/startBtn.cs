using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startBtn : MonoBehaviour
{
    public GameObject loginWin;
    public RectTransform title;
    public Text text;

    float minX = -153f; //x축 이동 위치 최솟값
    float maxX = -187f; //x축 이동 위치 최댓값
    float minY = 36f;   //y축 이동 위치 최솟값
    float maxY = 127f;  //y축 이동 위치 최댓값
    float timer = 0f;
    bool move = false; // startBtn 누르면 true!

    /* 더 추가할 기능 : title 사이즈 확대 애니메이션 , btnImg FadeIn */


    private void Update()
    {
        if(move == true && timer <= 2f)
        {
            timer += Time.deltaTime;
            title.anchoredPosition = new Vector3(title.anchoredPosition.x, Mathf.SmoothStep(minY, maxY, timer), 0);
        }
    }

    public void ActiveLoginWin() //로그인 창 활성화 
    {
        move = true;
        StartCoroutine(ActiveDelay());
    }

    IEnumerator ActiveDelay()
    {
        yield return new WaitForSeconds(2f);
        loginWin.SetActive(true);
        this.gameObject.SetActive(false);
    }

}
