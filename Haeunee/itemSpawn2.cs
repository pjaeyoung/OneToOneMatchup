using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSpawn2 : MonoBehaviour //GameScene에서 아이템 스폰 
{
    public GameObject[] itemKind; //index 는 spawn index값, [index] 값은 item 종류
    public Transform[] itemSpawns;

    bool IsInstanceItemOK = false; //서버에서 스폰 정보 받았을 때 true로 바뀜
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

    public void setItemSpawns(int[] _result) //서버에서 보낸 스폰 길이와 클라이언트에 정한 길이 동일 유무 판단 
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

    void instanceItem() //아이템 스폰 
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
