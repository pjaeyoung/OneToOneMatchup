#pragma once


enum eMSG //�޼��� ����
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
};

enum eATKTYPE
{
	em_NORMAL = 1,
	em_OBJTHROW,
};

enum eRESULT
{
	em_WIN = 1,
	em_LOSE,
};

struct sFlag //� ������ ���´��� ������ �÷���
{
	int flag;
	sFlag(int f = 0) : flag(f) {};
};

struct sLogin : sFlag
{
	sLogin() :sFlag(flag = eMSG::em_LOGIN) {};
	int loginSucc = 0;
	char nick[30];
};

struct sGameRoom : sFlag //��Ī ����
{
	sGameRoom() :sFlag(flag = eMSG::em_ENTER) {};
	int userNum = 0; //�濡 ���� �������� �����ϱ� ���� ��
};

struct Item
{
	int item[3];
};

struct sCharInfo :sFlag //ĳ���� ����
{
	sCharInfo() :sFlag(flag = eMSG::em_CHARINFO) {};
	int weapon; //����
	int cloth; //��
	int gender, hair, hairColor, face; //ĳ���� ���� ����
	int item[3];
};

struct sMove :sFlag //������
{
	sMove() :sFlag(flag = eMSG::em_MOVE) {};
	float x, y, z; //��ġ ��ǥ
	float rotX, rotY, rotZ, rotW; //ȸ�� ��ǥ
};

struct sAtk :sFlag //����
{
	sAtk() :sFlag(flag = eMSG::em_ATK) {};
	int atkNum;
};

struct sHit :sFlag //���� ����
{
	sHit() :sFlag(flag = eMSG::em_HIT) {};
	int dmgAni = 0;
	int atkType = 0; //�Ϲݰ���, ���ǰ��� ����
	int hp = 0;
};

struct sChangeInfo :sFlag //hp���� ����
{
	sChangeInfo() :sFlag(flag = eMSG::em_INFO) {};
	int hp = 0;
};

struct sItemSpawn :sFlag //���� ������ ���� ����
{
	sItemSpawn() :sFlag(flag = eMSG::em_ITEMSPAWN) {};
	int itemKind[10];
};

struct sUseItem :sFlag //������ ���
{
	sUseItem() :sFlag(flag = eMSG::em_USEITEM) {};
	int itemNum = 0;
	int hp = 0;
	int speed = 0;
};

struct sEndItem :sFlag //������ ȿ�� ��
{
	sEndItem() :sFlag(flag = eMSG::em_ENDITEM) {};
	int itemNum = 0;
	int hp = 0;
	int speed = 0;
};

struct sGetObj :sFlag //���� �ݱ�
{
	sGetObj() :sFlag(flag = eMSG::em_GETOBJ) {};
	int itemNum = 0;
};

struct sThrowObj :sFlag //���� ������
{
	sThrowObj() :sFlag(flag = eMSG::em_THROWOBJ) {};
	float throwPosX, throwPosY, throwPosZ = 0;
};

struct sReady :sFlag
{
	sReady() :sFlag(flag = eMSG::em_READY) {};
};

struct sEnd : sFlag //�׺� ��ư ���, ����
{
	sEnd() : sFlag(flag = eMSG::em_END) {};
	int result = 0;
};

