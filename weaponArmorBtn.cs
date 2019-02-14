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
        GM = GameObject.Find("gameMgr").GetComponent<GameMgr>();
    }

    /* 무기, 방어구 버튼과 연결된 게임오브젝트 정보 저장 : inputGameObj, changeWeaponImg, changeArmorImg */
    public void inputGameObj(GameObject obj)
    {
        Vector3 newPos = obj.transform.position;
        string imgName = obj.GetComponent<MeshRenderer>().material.name;
        int imgNameLen = imgName.Length;
        string cmp = "";
        for(int i = 0; i < imgNameLen; i++)
        {
            if(cmp == "img_weapon")
            {
                if(Weapon != null)
                {
                    Weapon.transform.position = newPos;
                    Weapon.SetActive(true);
                }
                Weapon = obj;
                Weapon.SetActive(false);
                string str = GM.getAccurateName(imgName);
                changeWeaponImg(str);
            }
            else if(cmp == "img_armor")
            {
                if(Armor != null)
                {
                    Armor.transform.position = newPos;
                    Armor.SetActive(true);
                }
                Armor = obj;
                Armor.SetActive(false);
                string str = GM.getAccurateName(imgName);
                changeArmorImg(str);
            }
            cmp += imgName[i];
        }
    }

    void changeWeaponImg(string imgName)
    {
        Sprite spr = Resources.Load<Sprite>("Sprites/" + imgName);
        weaponBtn.GetComponent<Image>().sprite = spr;
    }

    void changeArmorImg(string imgName)
    {
        Sprite spr = Resources.Load<Sprite>("Sprites/" + imgName);
        ArmorBtn.GetComponent<Image>().sprite = spr;
    }

}   
