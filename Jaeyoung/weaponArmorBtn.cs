using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class weaponArmorBtn : MonoBehaviour {

    public ItemFieldCntrl GM;
    public Button ArmorBtn;
    public Button weaponBtn;
    GameObject Armor;
    GameObject Weapon;

    /* 무기, 방어구 버튼과 연결된 게임오브젝트 정보 저장 : inputGameObj, changeWeaponImg, changeArmorImg */
    public void inputGameObj(GameObject obj)
    {
        Debug.Log("obj.tag : " + obj.tag);
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
            string itemName = GM.getAccurateName(Weapon.name);
            GM.CPlayerInfo.changeWeapon(itemName);
            changeWeaponImg(itemName);
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
            string itemName = GM.getAccurateName(Armor.name);
            GM.CPlayerInfo.changeArmor(itemName);
            changeArmorImg(itemName);
        }       
    }

    void changeWeaponImg(string _itemName)
    {
        
        Sprite spr = Resources.Load<Sprite>("Sprites/" + _itemName);
        weaponBtn.GetComponent<Image>().sprite = spr;
    }

    void changeArmorImg(string _itemName)
    {
        Sprite spr = Resources.Load<Sprite>("Sprites/" + _itemName);
        ArmorBtn.GetComponent<Image>().sprite = spr;
    }

    
}   
