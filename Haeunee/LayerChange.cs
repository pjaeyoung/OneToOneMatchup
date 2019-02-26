using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerChange : MonoBehaviour
{
    GameObject[] weapons = new GameObject[4];
    int index = 0;

    public void InputWeaponArr(GameObject weapon)
    {
        weapons[index] = weapon;
        index++;
    }

    public GameObject OutputWeapon(int i)
    {
        return weapons[i];
    }
}
