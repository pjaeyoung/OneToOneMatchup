using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour {
    Slider slider;
    bool IsInit = false; // 초기화를 했는가? 

    private void Awake()
    {
        slider = transform.GetComponent<Slider>();
    }

    public void changeHpBar(int value) 
    {
        if(!IsInit) 
        {
            IsInit = true;
            slider.maxValue = value;
            slider.value = value;
        }
        if (slider.maxValue >= value)
            slider.value = value;
        else
            slider.value = slider.maxValue;
    }
}
