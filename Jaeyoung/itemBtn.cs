using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class itemBtn : MonoBehaviour
{
    public ItemFieldCntrl GM;
    Transform Player;
    GameObject[] getItemArr; //itemPool로 옮겨진 아이템 목록 
    int idx;

    private void Awake()
    {
        Player = GameObject.FindWithTag("Player").transform;
        getItemArr = new GameObject[3];
        idx = 0;
    }

    public void InputGetItemArr(GameObject obj, int index) //아이템 가방 창(0,1,2)과 획득한 아이템 배열(0,1,2)의 배치 일치시킴. 
    {
        if (!obj)
            Debug.Log("Item is null");
        else
            getItemArr[index] = obj;
    }

    public void OnEndDrag()
    {
        if (GM.scene.name == "ItemCollectScene")
        {
            int btnArrLength = GM.ItemBtn.Length;
            idx = 0;
            for (int i = 0; i < btnArrLength; i++)
            {
                if (EventSystem.current.currentSelectedGameObject.name == GM.ItemBtn[i].name)
                {
                    idx = i;
                    break;
                }
            }
            if (getItemArr[idx] != null)
            {
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5);
                Vector3 throwPos = Camera.main.ScreenToWorldPoint(mousePos);
                getItemArr[idx].transform.position = throwPos;
                getItemArr[idx].SetActive(true);
                getItemArr[idx] = null;

                Sprite spr = Resources.Load<Sprite>("Sprites/img_emty");
                GM.ItemBtn[idx].GetComponent<Image>().sprite = spr;

                int num = GM.CitemCount.GetItemNum();
                num--;
                GM.CitemCount.changeGetItemNum(num);
            }
            else
                Debug.Log("OnEndDrag : No item");
        }
    }
}
