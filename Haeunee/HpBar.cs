using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour {
    Slider slider;
    int IsInit = (int)eBOOLEAN.FALSE; // 초기화를 했는가? 

    private void Awake()
    {
        slider = transform.GetComponent<Slider>();
    }

    public void changeHpBar(int value) 
    {
        if(IsInit == (int)eBOOLEAN.FALSE) 
        {
            IsInit = (int)eBOOLEAN.TRUE;
            slider.maxValue = value;
        }
        if(slider.maxValue >= value)
            slider.value = value;
    }
}
