using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn1 : MonoBehaviour
{
    public Transform[] itemSpawn;
    public GameObject[] items;
    public GameObject[] weapons;
    public GameObject[] armors;
    GameEnterScript enter;
    int[] usedSpawnNum; //사용된 itemSpawn idx 저장

    void Awake()
    {
        enter = GameObject.Find("SocketServer").GetComponent<GameEnterScript>();
        int spawnCount = itemSpawn.Length;
        int weaponCount = weapons.Length;
        int armorCount = weaponCount + 3;
        int itemCount = armorCount + items.Length;
        int ran = 0;
        int itemIdx = 0;
        usedSpawnNum = new int[spawnCount];

        for (int i = 0; i < spawnCount; i++)
        {
            usedSpawnNum[i] = -1;
        }

        /* 스폰 생성 순서: 무기>방어구>아이템 */
        for (int i = 0; i < spawnCount; i++)
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
            else if (i >= weaponCount && i < armorCount)
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
            else if (i >= armorCount && i < spawnCount)
            {
                while (true)
                {
                    ran = Random.Range(0, spawnCount);
                    int IsuseableNum = IsSpawnUseable(ran);
                    if (IsuseableNum == (int)eBOOLEAN.TRUE)
                    {
                        callItem(ran, itemIdx);
                        itemIdx++;
                        if (itemIdx > 3)
                            itemIdx = 0;
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

    void callArmor(int num, int weaponLen, int idx)
    {
        int cmpidx = 0;
        if (enter.savCharInfo.gender == (int)eGENDER.MALE)
            cmpidx = idx - weaponLen + 3;
        else if(enter.savCharInfo.gender == (int)eGENDER.FEMALE)
            cmpidx = idx - weaponLen;
        Vector3 pos = itemSpawn[num].position;
        Quaternion rot = armors[cmpidx].transform.rotation;
        Instantiate(armors[cmpidx], pos, rot);
    }

    void callItem(int num, int idx)
    {
        Vector3 pos = itemSpawn[num].position;
        Quaternion rot = items[idx].transform.rotation;
        Instantiate(items[idx], pos, rot);
    }

    /* itemSpawn[num] 에 이미 스폰된 아이템이 있는지 여부 판단 */
    int IsSpawnUseable(int num)
    {
        if (usedSpawnNum[num] == -1)
        {
            usedSpawnNum[num] = num;
            return (int)eBOOLEAN.TRUE;
        }
        else
            return (int)eBOOLEAN.FALSE;
    }
}
