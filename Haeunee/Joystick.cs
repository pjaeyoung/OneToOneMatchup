using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Joystick : MonoBehaviour
{
    public RectTransform stick;
    Vector2 bgSize = Vector2.zero;
    Vector2 bgPos;
    GameObject player;
    Vector2 moveDir;
    Vector3 movePos;
    int moveSpeed = 5;
    bool move = false;
    AnimationController playerAni;
    PlayerScript playerScript;
    PlayerCntrl_itemField itemPlayerCntrl;
    string nowScene;

    void Start()
    {
        bgSize = gameObject.GetComponent<RectTransform>().sizeDelta;
        bgPos = gameObject.GetComponent<RectTransform>().position;
    }
    
    void Update()
    {
        if(move == true)
        {
            movePos = new Vector3(moveDir.x, 0, moveDir.y);
            player.transform.Translate(movePos * moveSpeed * Time.deltaTime);
            playerAni.PlayAnimation("Move");
        }
        else
            playerAni.PlayAnimation("Idle");
    }

    public void OnDrag(BaseEventData data)
    {
        PointerEventData eventData = data as PointerEventData;
        Vector2 touchPos = eventData.position;
        moveDir = (touchPos - bgPos).normalized;
        move = true;
        if (Vector2.Distance(bgPos, touchPos)>= bgSize.x/2)
        {
            stick.position = bgPos+((touchPos - bgPos).normalized * bgSize.x / 2);
        }
        else
        {
            stick.position = touchPos;
        }
    }

    public void OnDragEnd(BaseEventData eventData)
    {
        move = false;
        stick.position = bgPos;
        moveDir = Vector2.zero;
    }

    public void AtkBtnClick()
    {
        if (nowScene == "GameScene")
            playerScript.Attack();
        else if (nowScene == "ItemCollectScene")
            itemPlayerCntrl.Jump();
    }

    public void JumpBtnClick()
    {
        playerScript.Jump();
    }

    public void GetBtnClick()
    {
        if (nowScene == "ItemCollectScene")
        {
            if (itemPlayerCntrl.highlightBox.activeSelf == true)
                itemPlayerCntrl.TouchItem();
        }
    }

    public void ChangePlayer(GameObject nowPlayer)
    {
        nowScene = SceneManager.GetActiveScene().name;
        player = nowPlayer;
        if (nowScene == "GameScene")
        {
            playerAni = player.GetComponent<AnimationController>();
            playerScript = player.GetComponent<PlayerScript>();
        }
        else if (nowScene == "ItemCollectScene")
            itemPlayerCntrl = player.GetComponent<PlayerCntrl_itemField>();
    }

    public void ChangeSpeed(int nowSpeed)
    {
        moveSpeed = nowSpeed;
    }
}
