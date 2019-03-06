using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 서버 오브젝트에 AddComponent하기 */

//레이어 INDEX 명칭 
enum eLAYER
{
    WALL = 9,
    WEAPON,
    ENEMY,
    TOUCHWALL,
    TOUCHABLE,
}

//애니메이션의 종류
enum eANIMATION
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

enum eMSG //메세지 종류
{
    em_ENTER = 1,
    em_CHARINFO,
    em_MOVE,
    em_ATK,
    em_HIT,
    em_INFO,
    em_ITEMSPAWN,
    em_USEITEM,
    em_ENDITEM,
    em_READY,
    em_END,
}

// 참, 거짓 판단 
enum eBOOLEAN
{
    FALSE,
    TRUE
}

//무기 종류
enum eWEAPON 
{
    em_STICK = -1,
    em_SWORDANDSHIELD,
    em_GREATESWORD,
    em_DAGGER,
    em_BOW,
    em_WAND,
}

//방어구 종류 
enum eARMOR
{
    em_DEFAULT_AMR,
    em_ARMOR,
    em_SUIT,
    em_ROBE,
};

//아이템 종류
enum eITEM
{
    em_HP_POTION,
    em_SPEED_POTION,
    em_DAMAGE_UP_POTIOM,
    em_DEFENCE_UP_POTION
}

//성별
enum eGENDER
{
    MALE = 1,
    FEMALE
}

//아이템 사용 여부
enum eITEMUSE
{
    UNUSED,
    USED
}

enum eRESULT
{
    em_WIN = 1,
    em_LOSE,
};
struct sGameRoom //매칭 정보
{
    public int flag;
    public int userNum;
    public sGameRoom(int u, int f = (int)eMSG.em_ENTER)
    {
        flag = f;
        userNum = 0;
    }
}

public struct sCharInfo //획득한 아이템 정보
{
    private int flag;
    public int weapon; //무기
    public int armor; //방어구
    public int gender, hair, hairColor, face; //캐릭터 외형 정보
    public int item1, item2, item3; //소비아이템 (hpPotion)

    public sCharInfo(int w, int c, int gen, int inputHair, int inputColor, int inputFace, int i1, int i2, int i3, int f = (int)eMSG.em_CHARINFO)
    {
        flag = f;
        weapon = w;
        armor = c;
        gender = gen;
        hair = inputHair;
        hairColor = inputColor;
        face = inputFace;
        item1 = i1;
        item2 = i2;
        item3 = i3;
    }
}

public struct sMove //움직임, 회전
{
    private int flag;
    public float x, y, z;
    public float rotX, rotY, rotZ, rotW;

    public sMove(float inputX, float inputY, float inputZ, float inrotX, float inrotY, float inrotZ, float inrotW, int f = (int)eMSG.em_MOVE)
    {
        flag = f;
        x = inputX;
        y = inputY;
        z = inputZ;
        rotX = inrotX;
        rotY = inrotY;
        rotZ = inrotZ;
        rotW = inrotW;
    }
}

public struct sAtk //공격
{
    private int flag;
    public int atkAni;
    public sAtk(int ani, int f = (int)eMSG.em_ATK)
    {
        flag = f;
        atkAni = ani;
    }
}

public struct sHit //공격 성공
{
    private int flag;
    public int dmgAni;
    public int hp;
    public sHit(int ani, int nowHp, int f = (int)eMSG.em_HIT)
    {
        flag = f;
        dmgAni = ani;
        hp = nowHp;
    }
}

public struct sChangeInfo //hp정보 전달
{
    private int flag;
    public int hp;
    public sChangeInfo(int nowHp, int f = (int)eMSG.em_INFO)
    {
        flag = f;
        hp = nowHp;
    }
};

struct sItemSpawn
{
    private int flag;
    public int[] itemKind;
    public sItemSpawn(int[] itemArr, int f = (int)eMSG.em_ITEMSPAWN)
    {
        flag = f;
        itemKind = itemArr;
    }
};

public struct sUseItem //아이템 사용
{
    private int flag;
    public int itemNum; // 아이템 버튼 index
    public int hp;
    public int speed;
    public sUseItem(int item, int nowHp, int spd, int f = (int)eMSG.em_USEITEM)
    {
        flag = f;
        itemNum = item;
        hp = nowHp;
        speed = spd;
    }
}

struct sEndItem //아이템 효과 끝
{
    private int flag;
    public int itemNum; // 아이템 버튼 index
    public int hp;
    public int speed;
    public sEndItem(int item, int nowHp, int spd, int f = (int)eMSG.em_ENDITEM)
    {
        flag = f;
        itemNum = item;
        hp = nowHp;
        speed = spd;
    }
};

public struct sReady
{
    private int flag;
    public sReady(int f = (int)eMSG.em_READY)
    {
        flag = f;
    }
}

public struct sEnd //항복 버튼 사용, 죽음
{
    private int flag;
    public int result;
    public sEnd(int res, int f = (int)eMSG.em_END)
    {
        flag = f;
        result = res;
    }
}


