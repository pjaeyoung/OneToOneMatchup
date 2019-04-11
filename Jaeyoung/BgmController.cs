using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BgmController : MonoBehaviour
{
    public AudioSource audioBgm;
    public Slider bgmVolSlider;
    public AudioClip[] bgmClip;
    string nowScene;
    bool fadeIn = false; //화면 전환시 볼륨 자연스럽게 전환 
    bool fadeOut = false;
    public GameObject SettingWin; //설정창 
    public GameObject SetUpBtn; //설정창 버튼
    public Button ExitBtn; //나가기창 버튼
    public GameObject block; //마우스 클릭 방지 
    string sceneName;

    void Start()
    {
        DontDestroyOnLoad(this);
        sceneName = SceneManager.GetActiveScene().name;
        BGMSetAndBtnOnOff();
    }
    
    void Update()
    {
        if (fadeIn==true)
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
            }
        }
        else
            audioBgm.volume = bgmVolSlider.value;

        if(sceneName != SceneManager.GetActiveScene().name)
        {
            sceneName = SceneManager.GetActiveScene().name;
            BGMSetAndBtnOnOff();
        }
    }

    public void ChangeBgm()
    {
        fadeOut = true;
    }

    public void SettingBtnClick()
    {
        SettingWin.SetActive(true);
        block.SetActive(true);
        ExitBtn.interactable = false;
    }

    public void CloseBtnClick()
    {
        SettingWin.SetActive(false);
        block.SetActive(false);
        ExitBtn.interactable = true;
    }
    
    void BGMSetAndBtnOnOff() //씬 별 BGM setting, SetUpBtn과 ExitBtn 화면 출력 여부 판단
    {
        if (sceneName == "LoginScene" || sceneName == "WaitScene")
        {
            transform.GetChild(0).gameObject.SetActive(true);
            ExitBtn.gameObject.SetActive(true);
            SetUpBtn.SetActive(true);
            if (audioBgm.clip.name == bgmClip[0].name)
                return;
            audioBgm.clip = bgmClip[0];
        }
        else
        {
            ExitBtn.gameObject.SetActive(false);
            SetUpBtn.SetActive(false);
            SettingWin.SetActive(false);
            block.SetActive(false);
            if (sceneName == "ItemCollectScene")
            {
                audioBgm.clip = bgmClip[1];
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (sceneName == "GameScene")
            {
                audioBgm.clip = bgmClip[2];
            }
            else if (sceneName == "LoadingScene")
            {
                return;
            }
        }
        audioBgm.Play();
        audioBgm.volume = 0;
        fadeIn = true;
    }
}
