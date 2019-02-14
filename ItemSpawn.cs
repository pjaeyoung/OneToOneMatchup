﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour {
    public Transform[] itemSpawn;
    public GameObject[] items;
    public GameObject[] weapons;
    public GameObject[] armors;

    int[] usedSpawnNum; //사용된 itemSpawn idx 저장

    enum eBOOLEAN
    {
        FALSE , TRUE
    }

    void Awake ()
    {
        int spawnCount = itemSpawn.Length;
        int weaponCount = weapons.Length;
        int armorCount = weaponCount + armors.Length;
        int itemCount = armorCount + items.Length;
        int ran = 0;
        usedSpawnNum = new int[spawnCount];

        for(int i = 0; i < spawnCount; i++)
        {
            usedSpawnNum[i] = -1;
        }

        /* 스폰 생성 순서: 무기>방어구>아이템 */
        for(int i = 0; i<spawnCount; i++)
        {
            if (i < weaponCount)
            {
                while (true)
                {
                    ran = Random.Range(0, spawnCount);
                    int IsuseableNum = IsSpawnUseable(ran);
                    if (IsuseableNum == (int)eBOOLEAN.TRUE)
                    {
                        callWeapon(ran, i);
                        break;
                    }
                    else
                        continue;
                }
            }
            else if(i >= weaponCount && i < armorCount)
            {
                while (true)
                {
                    ran = Random.Range(0, spawnCount);
                    int IsuseableNum = IsSpawnUseable(ran);
                    if (IsuseableNum == (int)eBOOLEAN.TRUE)
                    {
                        callArmor(ran, weaponCount, i);
                        break;
                    }
                    else
                        continue;
                }
            }
            else if(i >= armorCount && i < itemCount)
            {
                while (true)
                {
                    ran = Random.Range(0, spawnCount);
                    int IsuseableNum = IsSpawnUseable(ran);
                    if (IsuseableNum == (int)eBOOLEAN.TRUE)
                    {
                        callItem(ran, armorCount, i);
                        break;
                    }
                    else
                        continue;
                }
            }
        }
	}

    void callWeapon(int num, int idx)
    {
        Vector3 pos = itemSpawn[num].position;
        Quaternion rot = weapons[idx].transform.rotation;
        Instantiate(weapons[idx], pos, rot);
    }

    void callArmor(int num , int weaponLen, int idx)
    {
        int cmpidx = idx - weaponLen;
        Vector3 pos = itemSpawn[num].position;
        Quaternion rot = armors[cmpidx].transform.rotation;
        Instantiate(armors[cmpidx], pos, rot);
    }

    void callItem(int num, int weaponArmorLen, int idx)
    {
        int cmpidx = idx - weaponArmorLen;
        Vector3 pos = itemSpawn[num].position;
        Quaternion rot = items[cmpidx].transform.rotation;
        Instantiate(items[cmpidx], pos, rot);
    }

    /* itemSpawn[num] 에 이미 스폰된 아이템이 있는지 여부 판단 */
    int IsSpawnUseable(int num) 
    {
        if(usedSpawnNum[num] == -1)
        {
            usedSpawnNum[num] = num;
            return (int)eBOOLEAN.TRUE;
        }
        else
            return (int)eBOOLEAN.FALSE;
    }
}
