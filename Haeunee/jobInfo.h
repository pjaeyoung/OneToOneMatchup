#pragma once

#include "stdafx.h"

//���� ����
enum eWEAPON
{
	em_STICK = -1,
	em_SHIElD,
	em_SWORD,
	em_DAGGER,
	em_BOW,
	em_WAND,
};

//���� ���� 
enum eSTATUS
{
	em_HP,
	em_ATk,
	em_DEF,
	em_SPEED
};

enum eARMOR
{
	em_DEFAULT_AMR,
	em_ARMOR,
	em_SUIT,
	em_ROBE,
};

enum eITEM
{
	em_HP_POTION,
	em_SPEED_POTION,
	em_DAMAGE_UP_POTIOM,
	em_DEFENCE_UP_POTION
};

/* ȹ���� ���⿡ ���� JOB ���� */
class Status
{
public:
	int flag = 0;
	float hp = 0;
	float atk = 0;
	float def = 0;
	float speed = 0;

	Status(int f = -1)
	{
		flag = f;
		if (flag == eWEAPON::em_STICK)
		{
			hp = 200;
			atk = 10;
			def = 5;
			speed = 2;
		}
		else if (flag == eWEAPON::em_SHIElD)
		{
			hp = 300;
			atk = 30;
			def = 20;
			speed = 2;
		}
		else if (flag == eWEAPON::em_SWORD)
		{
			hp = 300;
			atk = 40;
			def = 10;
			speed = 2;
		}
		else if (flag == eWEAPON::em_BOW)
		{
			hp = 300;
			atk = 30;
			def = 5;
			speed = 3;
		}
		else if (flag == eWEAPON::em_WAND)
		{
			hp = 300;
			atk = 30;
			def = 10;
			speed = 2;
		}
		else if (flag == 10+ eARMOR::em_DEFAULT_AMR)
		{
			hp = 50;
			atk = 10;
			def = 5;
			speed = 1;
		}
		else if (flag == 10 + eARMOR::em_ARMOR)
		{
			hp = 100;
			atk = 10;
			def = 10;
			speed = 1;
		}
		else if (flag == 10 + eARMOR::em_SUIT)
		{
			hp = 100;
			atk = 10;
			def = 5;
			speed = 2;
		}
		else if (flag == 10 + eARMOR::em_ROBE)
		{
			hp = 100;
			atk = 20;
			def = 5;
			speed = 1;
		}
		else if (flag == 20 + eITEM::em_HP_POTION)
		{
			hp = 100;
		}
		else if (flag == 20 + eITEM::em_SPEED_POTION)
		{
			speed = 1;
		}
		else if (flag == 20 + eITEM::em_DAMAGE_UP_POTIOM)
		{
			atk = 5;
		}
		else if (flag == 20 + eITEM::em_DEFENCE_UP_POTION)
		{
			def = 5;
		}
	}

	float GetStat(int status)
	{
		if (status == eSTATUS::em_HP)
			return hp;
		else if (status == eSTATUS::em_ATk)
			return atk;
		else if (status == eSTATUS::em_DEF)
			return def;
		else if (status == eSTATUS::em_SPEED)
			return speed;
	}

	void PlusStat(Status* stat)//���� ���ϱ�(������ ���)
	{
		this->hp += stat->hp;
		this->atk += stat->atk;
		this->def += stat->def;
		this->speed += stat->speed;
	}

	void MinusStat(Status* stat) //���� ����(������ �ð� ��)
	{
		this->atk -= stat->atk;
		this->def -= stat->def;
		this->speed -= stat->speed;
	}
};