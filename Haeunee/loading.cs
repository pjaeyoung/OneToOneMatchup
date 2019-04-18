using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loading : MonoBehaviour
{
    static string nextScene; //전환할 씬 이름 
    Image loadingBar; 
    float fill = 0f;

    private void Awake()
    {
        loadingBar = GameObject.Find("loading").GetComponent<Image>();
    }

    void Start()
    {
        StartCoroutine(Loading()); 
    }
    
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator Loading()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        StartCoroutine(OnDelay(op.progress));

        while (op.progress < 0.9f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        op.allowSceneActivation = true;
    }

    IEnumerator OnDelay(float f) //로딩바 진행 정도 표시 
    {
        while(f <= 1f)
        {
            fill += 0.1f;
            if (fill >= 1.2f)
                fill = 0;
            loadingBar.fillAmount = fill;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
