using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class weaponArmorBtn : MonoBehaviour {

    GameMgr GM;
    public Button ArmorBtn;
    public Button weaponBtn;
    GameObject Armor;
    GameObject Weapon;

    private void Awake()
    {
        GM = GameObject.Find("itemFieldMgr").GetComponent<GameMgr>();
    }

    /* 무기, 방어구 버튼과 연결된 게임오브젝트 정보 저장 : inputGameObj, changeWeaponImg, changeArmorImg */
    public void inputGameObj(GameObject obj)
    {
        Vector3 newPos = obj.transform.position;
        if(obj.tag == "weapon")
        {
            if (Weapon != null)
            {
                Weapon.transform.position = newPos;
                Weapon.SetActive(true);
            }
            Weapon = obj;
            Weapon.SetActive(false);
            string str = Weapon.name;
            GM.CPlayerInfo.changeWeapon(str);
            changeWeaponImg(str);
        }
        else if(obj.tag == "armor")
        {
            if (Armor != null)
            {
                Armor.transform.position = newPos;
                Armor.SetActive(true);
            }
            Armor = obj;
            Armor.SetActive(false);
            string str = Armor.name;
            GM.CPlayerInfo.changeArmor(str);
            changeArmorImg(str);
        }       
    }

    void changeWeaponImg(string _itemName)
    {
        string itemName = GM.getAccurateName(_itemName);
        Sprite spr = Resources.Load<Sprite>("Sprites/" + itemName);
        weaponBtn.GetComponent<Image>().sprite = spr;
    }

    void changeArmorImg(string _itemName)
    {
        string itemName = GM.getAccurateName(_itemName);
        Sprite spr = Resources.Load<Sprite>("Sprites/" + itemName);
        ArmorBtn.GetComponent<Image>().sprite = spr;
    }

    
}   
