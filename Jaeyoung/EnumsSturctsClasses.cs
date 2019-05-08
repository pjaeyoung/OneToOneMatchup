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
    em_LOGOUT,
    em_LOGINCHECK,
    em_MATCHREQUEST,
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
    em_CHAT,
}

enum Friend //WaitScene 소셜창 탭
{
    em_LIST = 1,
    em_SEARCH,
    em_REQUEST,
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

enum eEFFSOUND //사운드 타입
{
    em_ARROW,
    em_ARROWHIT,
    em_SWING1,
    em_SWING2,
    em_MAGIHIT,
    em_SWORD1,
    em_SWORD2,
    em_SWORD3,
    em_SWORD4,
    em_WIND,
    em_BOMB,
}

enum eItemBtnArrFlag // 아이템 가방 배열 체크 모드
{
    em_CHKGETNUM, 
    em_CHKEMPTY
}

struct sLogin
{
    private int flag;
    public int loginSucc;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
    public char[] nick;
    public sLogin(char[] name, int succ, int f = (int)eMSG.em_LOGIN)
    {
        flag = f;
        nick = name;
        loginSucc = succ;
    }
}

struct sLogout
{
    private int flag;
    public sLogout(int f = (int)eMSG.em_LOGOUT)
    {
        flag = f;
    }
}

struct sLoginCheck
{
    private int flag;
    public int loginChk;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
    public char[] nick;
    public sLoginCheck(char[] name, int chk, int f = (int)eMSG.em_LOGINCHECK)
    {
        flag = f;
        nick = name;
        loginChk = chk;
    }
}

struct sMatchReq
{
    private int flag;
    public int matchSucc;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
    public char[] recvUserNick;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
    public char[] sendUserNick;
    public sMatchReq(char[] user, char[] friend, int succ, int f = (int)eMSG.em_MATCHREQUEST)
    {
        flag = f;
        recvUserNick = friend;
        sendUserNick = user;
        matchSucc = succ;
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
    public int atkType;
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
    public sThrowObj(int f = (int)eMSG.em_THROWOBJ)
    {
        flag = f;
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
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public char[] enemyNick;
    public sEnd(int res, char[] enemy, int f = (int)eMSG.em_END)
    {
        flag = f;
        result = res;
        enemyNick = enemy;
    }
}

public struct sChat //채팅
{
    private int flag;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
    public char[] chat;
    public sChat(char[] ch, int f = (int)eMSG.em_CHAT)
    {
        flag = f;
        chat = ch;
    }
}



