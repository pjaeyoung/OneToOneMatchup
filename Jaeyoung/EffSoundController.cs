using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum eEFFSOUND
{
    em_ARROW,
    em_ARROWHIT,
    em_SWING1,
    em_SWING2,
    em_MAGIHIT,
    em_SWORD1,
    em_SWORD2,
    em_SWORD3,
    em_SWORD4,
    em_WIND,
}

public class EffSoundController : MonoBehaviour
{
    public AudioSource audioEff;
    Slider effVolSlider;
    public AudioClip[] effClip;

    private void Awake()
    {
        GameObject soundMgr = GameObject.Find("SoundMgr");
        GameObject setWin = soundMgr.transform.Find("SettingWin").gameObject;
        effVolSlider = setWin.transform.Find("EffSoundSlider").GetComponent<Slider>();
    }

    public void PlayEff(int effNum)
    {
        audioEff.clip = effClip[effNum];
        audioEff.volume = effVolSlider.value;
        audioEff.Play();
    }
}
