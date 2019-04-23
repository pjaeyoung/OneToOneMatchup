using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemBtn : MonoBehaviour
{
    public ItemFieldCntrl GM;
    Button ArmorBtn;
    Button WeaponBtn;
    GameObject fullItemMSG; //아이템 가방 다 찼다는 메세지 
    GameObject[] getItemArr; //itemPool로 옮겨진 아이템 목록 
    Transform Player;
    GameObject Armor;
    GameObject Weapon;
    int idx;

    private void Awake()
    {
        Player = GameObject.FindWithTag("Player").transform;
        ArmorBtn = GameObject.Find("btn_GetArmor").GetComponent<Button>();
        WeaponBtn = GameObject.Find("btn_GetWeapon").GetComponent<Button>();
        fullItemMSG = GameObject.Find("Canvas").transform.Find("fullItemMSG").gameObject;
        getItemArr = new GameObject[3];
        idx = 0;
    }

    public void InputGetItemArr(GameObject obj) //아이템 가방 창(0,1,2)과 획득한 아이템 배열(0,1,2)의 배치 일치시킴. 
    {
        int emtyNum = GM.getEmtyImgIndex();
        if (emtyNum >= 3)
        {
            fullItemMSG.GetComponentInChildren<Text>().text = "아이템이 다 찼습니다.";
            fullItemMSG.SetActive(true);
            StartCoroutine(fullItemMsgEnd(fullItemMSG));
        }
        else
        {
            getItemArr[emtyNum] = obj;
            GM.changeItemImg(getItemArr[emtyNum]);
            getItemArr[emtyNum].SetActive(false);
        }
    }

    public void OnEndDrag() // 버튼에 들어있는 아이템 밖으로 꺼내기 
    {
        if (GM.scene.name == "ItemCollectScene")
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5);
            Vector3 throwPos = Camera.main.ScreenToWorldPoint(mousePos);

            if(EventSystem.current.currentSelectedGameObject == ArmorBtn.gameObject) 
            {
                if (Armor != null)
                {
                    Armor.transform.position = throwPos;
                    Armor.SetActive(true);
                    Armor = null;
                    Sprite spr = Resources.Load<Sprite>("Sprites/img_emty");
                    ArmorBtn.GetComponent<Image>().sprite = spr;
                }
            }
            else if (EventSystem.current.currentSelectedGameObject == WeaponBtn.gameObject) 
            {
                if(Weapon != null)
                {
                    Weapon.transform.position = throwPos;
                    Weapon.SetActive(true);
                    Weapon = null;
                    Sprite spr = Resources.Load<Sprite>("Sprites/img_emty");
                    WeaponBtn.GetComponent<Image>().sprite = spr;
                }
            }
            else
            {
                int btnArrLength = GM.itemBtn.Length;
                idx = 0;
                for (int i = 0; i < btnArrLength; i++)
                {
                    if (EventSystem.current.currentSelectedGameObject.name == GM.itemBtn[i].name)
                    {
                        idx = i;
                        break;
                    }
                }

                if (getItemArr[idx] != null)
                {
                    getItemArr[idx].transform.position = throwPos;
                    getItemArr[idx].SetActive(true);
                    getItemArr[idx] = null;

                    Sprite spr = Resources.Load<Sprite>("Sprites/img_emty");
                    GM.itemBtn[idx].GetComponent<Image>().sprite = spr;
                }
                else
                    Debug.Log("OnEndDrag : No item");
            }
        }
    }

    public void inputGameObj(GameObject obj)
    {
        Debug.Log("obj.tag : " + obj.tag);
   
        if (obj.tag == "weapon")
        {
            if (Weapon != null)
            {
                fullItemMSG.GetComponentInChildren<Text>().text = "이미 무기를 소지하고 있습니다.";
                fullItemMSG.SetActive(true);
                StartCoroutine(fullItemMsgEnd(fullItemMSG));
                return;
            }
            Weapon = obj;
            Weapon.SetActive(false);
            string itemName = GM.getAccurateName(Weapon.name);
            GM.CPlayerInfo.changeWeapon(itemName);
            changeWeaponImg(itemName);
        }
        else if (obj.tag == "armor")
        {
            if (Armor != null)
            {
                fullItemMSG.GetComponentInChildren<Text>().text = "이미 방어구를 소지하고 있습니다.";
                fullItemMSG.SetActive(true);
                StartCoroutine(fullItemMsgEnd(fullItemMSG));
                return;
            }
            Armor = obj;
            Armor.SetActive(false);
            string itemName = GM.getAccurateName(Armor.name);
            GM.CPlayerInfo.changeArmor(itemName);
            changeArmorImg(itemName);
        }
    }

    void changeWeaponImg(string _itemName)
    {
        Sprite spr = Resources.Load<Sprite>("Sprites/" + _itemName);
        WeaponBtn.GetComponent<Image>().sprite = spr;
    }

    void changeArmorImg(string _itemName)
    {
        Sprite spr = Resources.Load<Sprite>("Sprites/" + _itemName);
        ArmorBtn.GetComponent<Image>().sprite = spr;
    }

    /* 꽉찬 아이템창 알림 메세지 화면 표시 시간 제어 */
    IEnumerator fullItemMsgEnd(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        obj.SetActive(false);
    }
}
