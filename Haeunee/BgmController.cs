using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BgmController : MonoBehaviour
{
    public AudioSource audioBgm;
    public Slider bgmVolSlider;
    public AudioClip[] bgmClip;
    string nowScene;
    bool fadeIn = false;
    bool fadeOut = false;
    public GameObject SettingWin;
    string sceneName;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
    
    void Update()
    {
        if(fadeIn==true)
        {
            audioBgm.volume += Time.deltaTime;
            if(audioBgm.volume >= bgmVolSlider.value)
                fadeIn = false;
        }
        else if (fadeOut == true)
        {
            audioBgm.volume -= 0.01f;
            if (audioBgm.volume <= 0)
            {
                fadeOut = false;
                SceneChange();
            }
        }
        else
            audioBgm.volume = bgmVolSlider.value;
    }

    public void ChangeBgm(string name)
    {
        fadeOut = true;
        sceneName = name;
    }

    public void SettingBtnClick()
    {
        SettingWin.SetActive(true);
    }

    public void CloseBtnClick()
    {
        SettingWin.SetActive(false);
    }

    void SceneChange()
    {
        SceneManager.LoadScene(sceneName);
        if (sceneName == "LoginScene" || sceneName == "WaitScene")
        {
            audioBgm.clip = bgmClip[0];
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (sceneName == "ItemCollectScene")
        {
            audioBgm.clip = bgmClip[1];
            transform.GetChild(0).gameObject.SetActive(false);
        }
        if (sceneName == "GameScene")
            audioBgm.clip = bgmClip[2];

        audioBgm.Play();
        audioBgm.volume = 0;
        fadeIn = true;
    }
}
