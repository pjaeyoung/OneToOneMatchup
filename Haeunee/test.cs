using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    GameObject[] effect;
    ParticleSystem[] particle;
    // Start is called before the first frame update
    void Awake()
    {
        effect = GameObject.FindGameObjectsWithTag("effect");
        int len = effect.Length;
        particle = new ParticleSystem[len];
        for(int i = 0; i<len; i++)
        {
            if(effect[i].name == "HitEffect")
            {
                particle[i] = effect[i].GetComponentInChildren<ParticleSystem>();
            }
        }
        Debug.Log("effect 1 :" + effect[0].name + ", effect2 :" + effect[1].name);
    }

  

}
