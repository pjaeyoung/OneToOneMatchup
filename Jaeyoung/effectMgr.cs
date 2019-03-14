using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effectMgr : MonoBehaviour
{
    public GameObject[] effect;//대전 필드에서 사용할 이펙트 prefab 정보 
    public ParticleSystem[] particle; //이펙트 prefab에 있는 ParticleSystem script

    private void Awake()
    {
        int len = effect.Length;
        particle = new ParticleSystem[len];
        particle[(int)ePARTICLE.em_HIT] = effect[(int)ePARTICLE.em_HIT].GetComponentInChildren<ParticleSystem>();
        particle[(int)ePARTICLE.em_MAGIG] = effect[(int)ePARTICLE.em_MAGIG].GetComponentInChildren<ParticleSystem>();
    }
}
