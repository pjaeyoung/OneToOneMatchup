using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemSpawn : MonoBehaviour
{
    string sceneName;
    /* ItemCollectScene에서 아이템 스폰 */
    public Transform[] itemSpawn;
    public GameObject[] items;
    public GameObject[] weapons;
    public GameObject[] armors;
    GameEnterScript enter;
    int[] usedSpawnNum; //사용된 itemSpawn idx 저장

     /* GameScene에서 아이템 스폰 */
    public GameObject[] itemKind; //index 는 spawn index값, [index] 값은 item 종류
    bool IsInstanceItemOK = false; //서버에서 스폰 정보 받았을 때 true로 바뀜
    int spawnLen;
    int[] result;
    GameObject[] spawnItemList; //스폰된 item 리스트 


    void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "ItemCollectScene")
            spawnInItemCollectScene();
    }

    private void Update()
    {
        if (sceneName == "GameScene" && IsInstanceItemOK == true)
        {
            IsInstanceItemOK = false;
            instanceItem();
        }
    }

    void spawnInItemCollectScene()
    {
        enter = GameObject.Find("GameMgr2/MatchingCntrl").GetComponent<GameEnterScript>();
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
                    bool IsuseableNum = IsSpawnUseable(ran);
                    if (IsuseableNum)
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
                    bool IsuseableNum = IsSpawnUseable(ran);
                    if (IsuseableNum)
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
                    bool IsuseableNum = IsSpawnUseable(ran);
                    if (IsuseableNum)
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

    void callWeapon(int num, int idx) //Weapon 배치
    {
        Vector3 pos = itemSpawn[num].position;
        Quaternion rot = weapons[idx].transform.rotation;
        Instantiate(weapons[idx], pos, rot);
    } 

    void callArmor(int num, int weaponLen, int idx) // Armor 배치 
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

    void callItem(int num, int idx) //아이템 배치 
    {
        Vector3 pos = itemSpawn[num].position;
        Quaternion rot = items[idx].transform.rotation;
        Instantiate(items[idx], pos, rot);
    }

    /* itemSpawn[num] 에 이미 스폰된 아이템이 있는지 여부 판단 */

    bool IsSpawnUseable(int num)
    {
        if (usedSpawnNum[num] == -1)
        {
            usedSpawnNum[num] = num;
            return true;
        }
        else
            return false;
    }

    public void setItemSpawns(int[] _result) //서버에서 보낸 스폰 길이와 클라이언트에 정한 길이 동일 유무 판단 
    {
        spawnLen = itemSpawn.Length;
        result = _result;
        int resultLen = _result.Length;
        if (spawnLen != resultLen)
            Debug.Log("spawnLen and resultLen is not same");
        else
        {
            IsInstanceItemOK = true;
        }
    }

    void instanceItem() //아이템 스폰 
    {
        spawnItemList = new GameObject[10];
        for (int i = 0; i < spawnLen; i++)
        {
            int itemIdx = result[i]; //아이템 종류 
            spawnItemList[i] = Instantiate(itemKind[itemIdx], itemSpawn[i].position, itemKind[itemIdx].transform.rotation, itemSpawn[i]);
        }
    }

    public int GetObjNum(GameObject obj)
    {
        for (int i = 0; i < spawnLen; i++)
        {
            if (spawnItemList[i] == obj)
                return i;
        }
        return 0;
    }

    public GameObject GetObj(int itemNum)
    {
        Debug.Log("itemNum: " + itemNum);
        Debug.Log("itemName: " + spawnItemList[itemNum].name);
        return spawnItemList[itemNum];
    }

}
