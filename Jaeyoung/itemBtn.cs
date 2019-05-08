using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemBtn : MonoBehaviour
{
    public ItemFieldCntrl GM; // 아이템 수집 화면 제어 오브젝트 (수집한 아이템 서버로 전송 담당)
    public GameObject fullItemMSG; //아이템 가방 다 찼다는 메세지 
    public Button[] ItemBagArr; //아이템 가방 
    public GameObject[] getItemArr = new GameObject[5]; // 아이템 가방에 들어있는 아이템

    /* 아이템 가방창에서 아이템 버리기 기능 (드래그 앤 드랍) */
    public void PointerDown()
    {
        if(GM.scene.name == "ItemCollectScene")
        {
            if (Input.GetMouseButtonDown(1))
                return;
        }
    }

    public void EndDrag(BaseEventData data) // 버튼에 들어있는 아이템 밖으로(마우스 위치) 꺼내기 
    {
        if (GM.scene.name == "ItemCollectScene")
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5);
            Vector3 throwPos = Camera.main.ScreenToWorldPoint(mousePos);

  
            for (int i = 0; i < 5; i++)
            {
                if (EventSystem.current.currentSelectedGameObject.name == ItemBagArr[i].name)
                {
                    if (getItemArr[i] != null)
                    {
                        getItemArr[i].transform.position = throwPos;
                        getItemArr[i].gameObject.SetActive(true);
                        getItemArr[i] = null;
                        Sprite spr = Resources.Load<Sprite>("Sprites/img_emty");
                        ItemBagArr[i].GetComponent<Image>().sprite = spr;
                    }
                    else
                        Debug.Log("OnEndDrag : No item");

                }
            }
        }
    }


    /* 아이템가방(배열)에 획득한 오브젝트 정보 저장 */

    // 오브젝트 tag 체크 (item, weapon, armor)
    // 오브젝트 최대 획득 개수 체크 ( item = 3, weapon = 1, armor = 1 )
    // eItemBtnArrFlag(체크 모드) 1. em_CHKGETNUM : 획득개수 체크 2. em_CHKEMPTY : 빈이미지 체크 

    public void InputGetItemArr(GameObject obj) 
    {
        int count = chkGetItemArr((int)eItemBtnArrFlag.em_CHKGETNUM, obj.tag); // 소지 개수 
        if (getItemArr[0] == null)
            Debug.Log("null");
;        while (count >= 1)
        {
            if (count == 1)
            {
                if (obj.tag == "weapon")
                    fullItemMSG.GetComponentInChildren<Text>().text = "이미 무기를 소지하고 있습니다.";
                else if (obj.tag == "armor")
                    fullItemMSG.GetComponentInChildren<Text>().text = "이미 방어구를 소지하고 있습니다.";
                else if (obj.tag == "item")
                    break;
            }
            else if (count == 2)
                break;
            else if (obj.tag == "item")
            {
                if (count == 3)
                    fullItemMSG.GetComponentInChildren<Text>().text = "이미 소비아이템 3개를 소지하고 있습니다.";
                else
                    return;
            }
            fullItemMSG.SetActive(true);
            StartCoroutine(fullItemMsgEnd());
            return;
        }

        int idx = chkGetItemArr((int)eItemBtnArrFlag.em_CHKEMPTY, null);
        getItemArr[idx] = obj;
        obj.SetActive(false);
        changeItemImg(obj, idx);
    }

    int chkGetItemArr(int flag , string tag) // 소지 아이템 개수 체크 혹은 빈 이미지 여부 체크 
    {
        int count = 0;

        for (int i = 0; i < 5; i++)
        {
            if (flag == (int)eItemBtnArrFlag.em_CHKGETNUM)
            {
                if (getItemArr[i] == null)
                    break;
                if (getItemArr[i].tag == tag)
                {
                    if (tag == "weapon" || tag == "armor")
                        return ++count;
                    else if (tag == "item")
                        count++;
                }
            }
            else if (flag == (int)eItemBtnArrFlag.em_CHKEMPTY)
            {
                string imgName = ItemBagArr[i].gameObject.GetComponent<Image>().sprite.name;
                if (imgName == "img_emty")
                    break;
                count++;
            }
        }

        return count;
    }

    void changeItemImg(GameObject obj, int idx) //빈 아이템가방 인덱스에 아이템 이미지 삽입 
    {
        string itemName = getAccurateName(obj.name);
        string fileName = "Sprites/";
        Sprite spr = Resources.Load<Sprite>(fileName + itemName);
        if (spr != null)
            ItemBagArr[idx].gameObject.GetComponent<Image>().sprite = spr;
        else
            Debug.LogError("img null");
    }

    public string getAccurateName(string name) //(clone) 부분 삭제한 이름 획득 
    {
        string temp = "";
        int len = name.Length;
        for (int i = 0; i < len; i++)
        {
            char sub = name[i];
            if (sub == '(')
                break;
            temp += name[i];
        }
        return temp;
    }

    /* 꽉찬 아이템창 알림 메세지 화면 표시 시간 제어 */
    IEnumerator fullItemMsgEnd()
    {
        yield return new WaitForSeconds(0.8f);
        fullItemMSG.SetActive(false);
    }
}
