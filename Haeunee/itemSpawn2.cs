using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSpawn2 : MonoBehaviour
{
    public GameObject[] itemKind; //index 는 spawn index값, [index] 값은 item 종류
    public Transform[] itemSpawns;

    bool IsInstanceItemOK = false; 
    int spawnLen;
    int[] result;
    GameObject[] spawnItemList; //스폰된 item 리스트 

    private void Update()
    {
        if (IsInstanceItemOK == true)
        {
            IsInstanceItemOK = false;
            instanceItem();
        }
    }

    public void setItemSpawns(int[] _result)
    {
        spawnLen = itemSpawns.Length;
        result = _result;
        int resultLen = _result.Length;
        if (spawnLen != resultLen)
            Debug.Log("spawnLen and resultLen is not same");
        else
        {
            IsInstanceItemOK = true;
        }
    }

    void instanceItem()
    {
        spawnItemList = new GameObject[10];
        for (int i = 0; i < spawnLen; i++)
        {
            int itemIdx = result[i]; //아이템 종류 
            spawnItemList[i] = Instantiate(itemKind[itemIdx], itemSpawns[i].position, itemKind[itemIdx].transform.rotation, itemSpawns[i]);
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
