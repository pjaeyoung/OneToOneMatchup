using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSpawn2 : MonoBehaviour
{
    public GameObject[] itemKind;
    public Transform[] itemSpawns;

    int IsInstanceItemOK = (int)eBOOLEAN.FALSE;
    int spawnLen;
    int[] result;

    private void Update()
    {
        if (IsInstanceItemOK == (int)eBOOLEAN.TRUE)
        {
            IsInstanceItemOK = (int)eBOOLEAN.FALSE;
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
            IsInstanceItemOK = (int)eBOOLEAN.TRUE;
        }
    }

    void instanceItem()
    {
        for (int i = 0; i < spawnLen; i++)
        {
            int itemIdx = result[i]; //아이템 종류 
            Instantiate(itemKind[itemIdx], itemSpawns[i].position, itemKind[itemIdx].transform.rotation, itemSpawns[i]);
        }
    }
}
