using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class KeyInputCntrl : MonoBehaviour
{
    public GameObject itemFieldCntrl;
    public GameObject MatchingCntrl;
    public InputField NickInput;
    public InputField PassInput;
    bool gameWinActive;//게임창 활성화 여부 
    string sceneName = "";

    void Update()
    {
        if (sceneName != SceneManager.GetActiveScene().name)
            sceneName = SceneManager.GetActiveScene().name;
        
        /* GameMgr2 활성화 제어 */
        if(sceneName == "WaitScene" && MatchingCntrl == null || sceneName == "WaitScene" && itemFieldCntrl == null)
        {
            MatchingCntrl = GameObject.Find("GameMgr2/MatchingCntrl");
            itemFieldCntrl = GameObject.Find("GameMgr2").transform.GetChild(0).gameObject;
        }

        if (sceneName == "ItemCollectScene" && !itemFieldCntrl.activeSelf)
            itemFieldCntrl.SetActive(true);

        /* 키입력 예외처리 */
        if (gameWinActive) // 활성화된 게임창에서만 키입력 받기 
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //일시정지 
            {
                if (sceneName == "LoadingScene" || sceneName == "ItemCollectScene")
                    return;
                else if (sceneName == "LoginScene")
                {
                    if (GameObject.Find("Canvas").transform.Find("SignUpWin").gameObject.activeSelf
                        || GameObject.Find("Canvas").transform.Find("SearchWin").gameObject.activeSelf
                        || GameObject.Find("Canvas").transform.Find("tutorialWin").gameObject.activeSelf)
                        return;
                    else
                        GameObject.Find("GameMgr").GetComponent<ExitBtn>().showMSG();
                }
                else if (sceneName == "WaitScene")
                {
                    if (GameObject.Find("WinCanvas").transform.GetChild(1).gameObject.activeSelf
                        || GameObject.Find("WinCanvas").transform.GetChild(2).gameObject.activeSelf)
                        return;
                    else
                        GameObject.Find("GameMgr").GetComponent<ExitBtn>().showMSG();
                } 
                else if (sceneName == "GameScene")
                    GameObject.Find("GameMgr").GetComponent<ExitBtn>().showMSG();
            }
            else if (Input.GetKeyDown(KeyCode.Return)) //login 혹은 chat send
            {
                if (sceneName == "LoginScene" && GameObject.Find("Canvas/loginWin").activeSelf)
                    GameObject.Find("Canvas/loginWin/LoginBtn").GetComponent<UserScript>().Login();
                else if (sceneName == "WaitScene")
                    GameObject.Find("btnCanvas/ChatScroll").GetComponent<ChatScript>().SendBtnClick();
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                if(sceneName == "LoginScene")
                {
                    if (NickInput.IsActive() && PassInput.IsActive() && NickInput.gameObject.activeSelf && PassInput.gameObject.activeSelf) // id, password inputField 간 이동
                    {
                        if (NickInput.isFocused)
                            PassInput.Select();
                        else if (PassInput.isFocused)
                            NickInput.Select();
                    }
                }
            }
        }
    }

    private void OnApplicationFocus(bool focus) //게임창 활성화 : true, 비활성화 : false
    {
        gameWinActive = focus;
    }
}
