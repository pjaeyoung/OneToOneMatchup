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
    GameObject[] spawnItemList;
    GameObject getItem;

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
        return spawnItemList[itemNum];
    }

    public void prepareTransferItem(GameObject obj, Vector3 newPos)
    {
        getItem = obj;
        Debug.Log("prepareTransfer");
        
        getItem.GetComponent<Rigidbody>().useGravity = true;
        TransferItem(newPos);
    }

    public void TransferItem(Vector3 TZPos)
    {
        Debug.Log("Transfer");
        Vector3 dir = TZPos - getItem.transform.position;
        getItem.GetComponent<Rigidbody>().velocity = getItem.transform.TransformDirection(dir.x, 0, dir.z);
        getItem = null;
    }
}
