using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum eANIMATION //애니메이션의 종류를 나타내는 enum
{
    em_IDLE = 0,
    em_MOVE,
    em_ATTACK01 = 0,
    em_ATTACK02,
    em_ATTACK03,
    em_ATTACK04,
    em_DAMEGE01 = 0,
    em_DAMEGE02,
}

public class AnimationController : MonoBehaviour {
    Animation animation;
    public int weaponIndex; //무기 종류 저장

    [System.Serializable]
    public class AnimationSet //무기에 따른 애니메이션을 우선순위별로 저장한 클래스
    {
        public List<AnimationClip> standardAni; //가만히 있기, 움직이기(우선순위 0(가장 낮음))
        public List<AnimationClip> atkAni; //공격(우선순위 1)
        public List<AnimationClip> damageAni; //공격당함(우선순위 2)
        public AnimationClip death;//죽음(우선순위 3(가장 높음))
    }
    
    public List<AnimationSet> weapon;

    // Use this for initialization
    void Start () {
        animation = GetComponent<Animation>();//현재 오브젝트의 애니메이션
        animation.wrapMode = WrapMode.Loop; //기본적으로 무한반복
        int weaponLength = weapon.Count;
        for(int i =0; i< weaponLength; i++) //무기의 종류에 따른 반복문
        {
            int atkLength = weapon[i].atkAni.Count;
            for(int j=0;j<atkLength;j++) //공격 애니메이션의 종류 반복문, 우선순위 설정을 위함
            {
                animation[weapon[i].atkAni[j].name].wrapMode = WrapMode.Once;
                animation[weapon[i].atkAni[j].name].layer = 1;
            }
            int dmgLength = weapon[i].damageAni.Count;
            for (int j = 0; j < dmgLength; j++)//공격 받는 애니메이션의 종류 반복문, 우선순위 설정을 위함
            {
                animation[weapon[i].damageAni[j].name].wrapMode = WrapMode.Once;
                animation[weapon[i].damageAni[j].name].layer = 2;
            }
            animation[weapon[i].death.name].wrapMode = WrapMode.Once; //죽는 애니메이션
            animation[weapon[i].death.name].layer = 3;
        }
        animation.Stop();
    }

    public void PlayAnimation(string _anim) //무기 종류에 따라 애니메이션을 다르게 불러오기 위한 함수
    {
        if (weaponIndex == (int)eWEAPON.em_SHIElD) //칼, 방패 애니메이션
        {
            if (_anim == "Idle")
                animation.CrossFade(weapon[(int)eWEAPON.em_SHIElD].standardAni[(int)eANIMATION.em_IDLE].name);
            else if (_anim == "Move")
                animation.CrossFade(weapon[(int)eWEAPON.em_SHIElD].standardAni[(int)eANIMATION.em_MOVE].name);
            else if (_anim == "Attack01")
                animation.CrossFade(weapon[(int)eWEAPON.em_SHIElD].atkAni[(int)eANIMATION.em_ATTACK01].name);
            else if (_anim == "Attack02")
                animation.CrossFade(weapon[(int)eWEAPON.em_SHIElD].atkAni[(int)eANIMATION.em_ATTACK02].name);
            else if (_anim == "Critical01")
                animation.CrossFade(weapon[(int)eWEAPON.em_SHIElD].atkAni[(int)eANIMATION.em_ATTACK03].name);
            else if (_anim == "Critical02")
                animation.CrossFade(weapon[(int)eWEAPON.em_SHIElD].atkAni[(int)eANIMATION.em_ATTACK04].name);
            else if (_anim == "GetDamage01")
                animation.CrossFade(weapon[(int)eWEAPON.em_SHIElD].damageAni[(int)eANIMATION.em_DAMEGE01].name);
            else if (_anim == "GetDamage02")
                animation.CrossFade(weapon[(int)eWEAPON.em_SHIElD].damageAni[(int)eANIMATION.em_DAMEGE02].name);
            else if (_anim == "Death")
                animation.CrossFade(weapon[(int)eWEAPON.em_SHIElD].death.name);

        }
        else if (weaponIndex == (int)eWEAPON.em_SWORD||weaponIndex==(int)eWEAPON.em_STICK) //대검, 막대기 애니메이션
        {
            if (_anim == "Idle")
                animation.CrossFade(weapon[(int)eWEAPON.em_SWORD].standardAni[(int)eANIMATION.em_IDLE].name);
            else if (_anim == "Move")
                animation.CrossFade(weapon[(int)eWEAPON.em_SWORD].standardAni[(int)eANIMATION.em_MOVE].name);
            else if (_anim == "Attack01")
                animation.CrossFade(weapon[(int)eWEAPON.em_SWORD].atkAni[(int)eANIMATION.em_ATTACK01].name);
            else if (_anim == "Attack02")
                animation.CrossFade(weapon[(int)eWEAPON.em_SWORD].atkAni[(int)eANIMATION.em_ATTACK02].name);
            else if (_anim == "Critical01")
                animation.CrossFade(weapon[(int)eWEAPON.em_SWORD].atkAni[(int)eANIMATION.em_ATTACK03].name);
            else if (_anim == "Critical02")
                animation.CrossFade(weapon[(int)eWEAPON.em_SWORD].atkAni[(int)eANIMATION.em_ATTACK04].name);
            else if (_anim == "GetDamage01")
                animation.CrossFade(weapon[(int)eWEAPON.em_SWORD].damageAni[(int)eANIMATION.em_DAMEGE01].name);
            else if (_anim == "GetDamage02")
                animation.CrossFade(weapon[(int)eWEAPON.em_SWORD].damageAni[(int)eANIMATION.em_DAMEGE02].name);
            else if (_anim == "Death")
                animation.CrossFade(weapon[(int)eWEAPON.em_SWORD].death.name);
        }
        else if (weaponIndex == (int)eWEAPON.em_DAGGER) //단검 애니메이션
        {
            if (_anim == "Idle")
                animation.CrossFade(weapon[(int)eWEAPON.em_DAGGER].standardAni[(int)eANIMATION.em_IDLE].name);
            else if (_anim == "Move")
                animation.CrossFade(weapon[(int)eWEAPON.em_DAGGER].standardAni[(int)eANIMATION.em_MOVE].name);
            else if (_anim == "Attack01")
                animation.CrossFade(weapon[(int)eWEAPON.em_DAGGER].atkAni[(int)eANIMATION.em_ATTACK01].name);
            else if (_anim == "Attack02")
                animation.CrossFade(weapon[(int)eWEAPON.em_DAGGER].atkAni[(int)eANIMATION.em_ATTACK02].name);
            else if (_anim == "Critical01")
                animation.CrossFade(weapon[(int)eWEAPON.em_DAGGER].atkAni[(int)eANIMATION.em_ATTACK03].name);
            else if (_anim == "Critical02")
                animation.CrossFade(weapon[(int)eWEAPON.em_DAGGER].atkAni[(int)eANIMATION.em_ATTACK04].name);
            else if (_anim == "GetDamage01")
                animation.CrossFade(weapon[(int)eWEAPON.em_DAGGER].damageAni[(int)eANIMATION.em_DAMEGE01].name);
            else if (_anim == "GetDamage02")
                animation.CrossFade(weapon[(int)eWEAPON.em_DAGGER].damageAni[(int)eANIMATION.em_DAMEGE02].name);
            else if (_anim == "Death")
                animation.CrossFade(weapon[(int)eWEAPON.em_DAGGER].death.name);
        }
        else if (weaponIndex == (int)eWEAPON.em_BOW) //활 애니메이션
        {
            if (_anim == "Idle")
                animation.CrossFade(weapon[(int)eWEAPON.em_BOW].standardAni[(int)eANIMATION.em_IDLE].name);
            else if (_anim == "Move")
                animation.CrossFade(weapon[(int)eWEAPON.em_BOW].standardAni[(int)eANIMATION.em_MOVE].name);
            else if (_anim == "Attack01")
                animation.CrossFade(weapon[(int)eWEAPON.em_BOW].atkAni[(int)eANIMATION.em_ATTACK01].name);
            else if (_anim == "Attack02")
                animation.CrossFade(weapon[(int)eWEAPON.em_BOW].atkAni[(int)eANIMATION.em_ATTACK02].name);
            else if (_anim == "Critical01")
                animation.CrossFade(weapon[(int)eWEAPON.em_BOW].atkAni[(int)eANIMATION.em_ATTACK03].name);
            else if (_anim == "Critical02")
                animation.CrossFade(weapon[(int)eWEAPON.em_BOW].atkAni[(int)eANIMATION.em_ATTACK04].name);
            else if (_anim == "GetDamage01")
                animation.CrossFade(weapon[(int)eWEAPON.em_BOW].damageAni[(int)eANIMATION.em_DAMEGE01].name);
            else if (_anim == "GetDamage02")
                animation.CrossFade(weapon[(int)eWEAPON.em_BOW].damageAni[(int)eANIMATION.em_DAMEGE02].name);
            else if (_anim == "Death")
                animation.CrossFade(weapon[(int)eWEAPON.em_BOW].death.name);
        }
        else if (weaponIndex == (int)eWEAPON.em_WAND) //마법봉 애니메이션
        {
            if (_anim == "Idle")
                animation.CrossFade(weapon[(int)eWEAPON.em_WAND].standardAni[(int)eANIMATION.em_IDLE].name);
            else if (_anim == "Move")
                animation.CrossFade(weapon[(int)eWEAPON.em_WAND].standardAni[(int)eANIMATION.em_MOVE].name);
            else if (_anim == "Attack01")
                animation.CrossFade(weapon[(int)eWEAPON.em_WAND].atkAni[(int)eANIMATION.em_ATTACK01].name);
            else if (_anim == "Attack02")
                animation.CrossFade(weapon[(int)eWEAPON.em_WAND].atkAni[(int)eANIMATION.em_ATTACK02].name);
            else if (_anim == "Critical01")
                animation.CrossFade(weapon[(int)eWEAPON.em_WAND].atkAni[(int)eANIMATION.em_ATTACK03].name);
            else if (_anim == "Critical02")
                animation.CrossFade(weapon[(int)eWEAPON.em_WAND].atkAni[(int)eANIMATION.em_ATTACK04].name);
            else if (_anim == "GetDamage01")
                animation.CrossFade(weapon[(int)eWEAPON.em_WAND].damageAni[(int)eANIMATION.em_DAMEGE01].name);
            else if (_anim == "GetDamage02")
                animation.CrossFade(weapon[(int)eWEAPON.em_WAND].damageAni[(int)eANIMATION.em_DAMEGE02].name);
            else if (_anim == "Death")
                animation.CrossFade(weapon[(int)eWEAPON.em_WAND].death.name);
        }
    }
}

