using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour {
    Slider slider;
    GameObject enemyUI;
    Vector3 sliderPos;
    public float distance = 5;

    private void Awake()
    {
        enemyUI = GameObject.Find("enemyUI");
        slider = enemyUI.transform.GetChild(0).GetComponent<Slider>();
    }

    private void Start()
    {
        slider.minValue = 0;
        /* player, enemy가 가진 weapon에 따라 hp 값 초기화 */
        if(slider.name == "enemyHP")
        {
            slider.maxValue = 10; //player, enemy weapon 불러오기 
            slider.value = slider.maxValue;

        }
        else if(slider.name == "playerHP")
        {
            slider.maxValue = 10;
            slider.value = slider.maxValue;
        }

    }

    private void FixedUpdate()
    {
        /* slider의 부모(canvas) 움직임 제어 */
        sliderPos = GameObject.Find("Enemy").transform.position;
        sliderPos.y += distance;
        enemyUI.transform.position = sliderPos;
    }

    void changeHpBar(int changeNum , int effect) //damage or heal 구분 
    {
        int IsValueChangeOK = 0;
        if (effect == (int)eHPEFEECT.em_DAMAGE)
        {
            IsValueChangeOK = (int)slider.value - changeNum;
            if (IsValueChangeOK >= slider.minValue)
                slider.value = IsValueChangeOK;
        }
            
        else if (effect == (int)eHPEFEECT.em_HEAL)
        {
            IsValueChangeOK = (int)slider.value + changeNum;
            if (IsValueChangeOK <= slider.maxValue)
                slider.value = IsValueChangeOK;
        }
    }
}
