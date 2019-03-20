using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

//레이어 INDEX 명칭 
enum eLAYER
{
    WALL = 9,
    WEAPON,
    ENEMY,
    TOUCHWALL,
    TOUCHABLE,
}

//파티클 종류 
enum ePARTICLE
{
    em_HIT, //아이템 던질 때 
    em_MAGIG, //마법 공격할 때 
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
    em_LOGIN = 1,
    em_ENTER,
    em_CHARINFO,
    em_MOVE,
    em_ATK,
    em_HIT,
    em_INFO,
    em_ITEMSPAWN,
    em_USEITEM,
    em_ENDITEM,
    em_GETOBJ,
    em_THROWOBJ,
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
}

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

enum eATKTYPE
{
    em_NORMAL = 1,
    em_OBJTHROW,
};

struct sLogin
{
    private int flag;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
    public char[] nick;
    public int loginSucc;
    public sLogin(char[] name, int succ, int f = (int)eMSG.em_LOGIN)
    {
        flag = f;
        nick = name;
        loginSucc = succ;
    }
}

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
    int atkType;
    public int hp;
    public sHit(int ani,int type, int nowHp, int f = (int)eMSG.em_HIT)
    {
        flag = f;
        dmgAni = ani;
        atkType = type;
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

public struct sItemSpawn //아이템 랜덤 생성 
{
    private int flag;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public int[] itemKind;
    public sItemSpawn(int[] itemArr, int f = (int)eMSG.em_ITEMSPAWN)
    {
        flag = f;
        itemKind = itemArr;
    }
}

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
}

struct sGetObj
{
    int flag;
    public int itemNum;
    public sGetObj(int item, int f = (int)eMSG.em_GETOBJ)
    {
        flag = f;
        itemNum = item;
    }
};

struct sThrowObj
{
    int flag;
    public float throwPosX, throwPosY, throwPosZ;
    public sThrowObj(float x, float y, float z, int f = (int)eMSG.em_THROWOBJ)
    {
        flag = f;
        throwPosX = x;
        throwPosY = y;
        throwPosZ = z;
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

/* 아이템 클래스 : 아이템 획득 갯수, 빈 아이템가방 넘버 */
public class ItemCount
{
    int getItemNum;
    int maxItemNum;

    public ItemCount()
    {
        getItemNum = 0;
        maxItemNum = 3;
    }

    public int GetItemNum()
    {
        return getItemNum;
    }

    public int changeGetItemNum(int changeNum)
    {
        if (changeNum <= maxItemNum)
        {
            getItemNum = changeNum;
            return 0;
        }
        else
            return 1;
    }
}

/* fightScene에 들고 갈 플레이어 정보 (서버 연동) */
public class PlayerInfo
{
    int weapon;
    int armor;
    int[] getItemArr;

    public PlayerInfo()
    {
        weapon = (int)eWEAPON.em_STICK;
        armor = (int)eARMOR.em_DEFAULT_AMR;
        getItemArr = new int[3];
    }

    public void InputGetItemArr(int i, int index)
    {
        getItemArr[index] = i;
    }

    public void changeArmor(string armorName)
    {
        string ArmorGender = armorName.Substring(0, 1);
        string ArmorNumStr = armorName.Substring(8, 1);
        int ArmorNumInt = int.Parse(ArmorNumStr);
        if (ArmorGender == "F" || ArmorGender == "M")
            armor = ArmorNumInt;
    }

    public void changeWeapon(string w)
    {
        if (w == "greatSword")
        {
            weapon = (int)eWEAPON.em_GREATESWORD;
        }
        else if (w == "wand")
        {
            weapon = (int)eWEAPON.em_WAND;
        }
        else if (w == "bow")
        {
            weapon = (int)eWEAPON.em_BOW;
        }
        else if (w == "swordAndShield")
        {
            weapon = (int)eWEAPON.em_SWORDANDSHIELD;
        }
    }

    public int getPlayerWeapon()
    {
        return weapon;
    }

    public int getPlayerArmor()
    {
        return armor;
    }

    public int getPlayerItemArr(int idx)
    {
        return getItemArr[idx];
    }
}

