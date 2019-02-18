using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class itemBtn : MonoBehaviour
{
    public Transform Player;
    GameObject[] getItemArr; //itemPool로 옮겨진 아이템 목록 
    GameMgr GM;
    Button[] btnArr;
    int idx;

    private void Awake()
    {
        GM = GameObject.Find("gameMgr").GetComponent<GameMgr>();
        getItemArr = new GameObject[3];
        btnArr = GM.itemBtn;
        idx = 0;
    }

    private void Start()
    {

        /* 마우스 버튼 클릭 후 드래그 구현 -> 아이템 가방에 든 물건 꺼내기(버리기) */
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
        entry_PointerDown.eventID = EventTriggerType.PointerDown;
        entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerDown);

        EventTrigger.Entry entry_Drag = new EventTrigger.Entry();
        entry_Drag.eventID = EventTriggerType.Drag;
        entry_Drag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_Drag);

        EventTrigger.Entry entry_EndDrag = new EventTrigger.Entry();
        entry_EndDrag.eventID = EventTriggerType.EndDrag;
        entry_EndDrag.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_EndDrag);
    }

    /* -------------------------------------------- ItemCollectScene ---------------------------------------------------------------------- */
    public void InputGetItemArr(GameObject obj, int index) //아이템 가방 창(0,1,2)과 획득한 아이템 배열(0,1,2)의 배치 일치시킴. 
    {
        if (!obj)
            Debug.Log("Item is null");
        else
            getItemArr[index] = obj;
    }

    void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("pointer Down");
       
        int btnArrLength = btnArr.Length;
        idx = 0;
        for (int i = 0; i < btnArrLength; i++)
        {
            if (transform.name == btnArr[i].name)
            {
                idx = i;
                break;
            }
        }

    }

    void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
    }

    void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        if (getItemArr[idx] != null)
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5);
            Vector3 throwPos = Camera.main.ScreenToWorldPoint(mousePos);
            getItemArr[idx].transform.position = throwPos;
            getItemArr[idx].SetActive(true);
            getItemArr[idx] = null;

            Sprite spr = Resources.Load<Sprite>("Sprites/img_emty");
            btnArr[idx].GetComponent<Image>().sprite = spr;

            int num = GM.CitemCount.GetItemNum();
            num--;
            GM.CitemCount.changeGetItemNum(num);
        }
        else
            Debug.Log("No item");
    }

    /* ----------------------------------------------------fightScene------------------------------------------------- */
}
