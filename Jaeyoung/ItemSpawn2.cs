using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn2 : MonoBehaviour
{
    public GameObject[] itemSpawns;
    public GameObject[] items;
    int ranItemNum = 0;
    int spawnLen;
    int itemLen;

    public void Awake()
    {
        spawnLen = itemSpawns.Length;
        itemLen = items.Length;
        RandomSpawn();
    }

    void RandomSpawn()
    {
        for (int i = 0; i < spawnLen; i++)
        {
            ranItemNum = Random.Range(0, itemLen - 1);
            Instantiate(items[ranItemNum], itemSpawns[i].transform.position, items[ranItemNum].transform.rotation);
        }
    }

}
