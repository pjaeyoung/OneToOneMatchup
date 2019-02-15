// itemInfo.cpp: 콘솔 응용 프로그램의 진입점을 정의합니다.
//

#include "stdafx.h"

//아이템 종류
enum eITEM
{
	em_HP_POTION,
	em_SPEED_POTION,
	em_DAMAGE_UP_POTIOM,
	em_DEFENCE_UP_POTION
};

class ItemInfo 
{
public:
	void WhatTypeOfItemAct(int idx, int stat)
	{
		if (idx == eITEM::em_HP_POTION)
		{
			HpPotionAct(stat);
		}
		else if (idx == eITEM::em_SPEED_POTION)
		{
			SpeedUpPotionAct(stat);
		}
		else if (idx == eITEM::em_DAMAGE_UP_POTIOM)
		{
			DamageUpPotionAct(stat);
		}
		else if (idx == eITEM::em_DEFENCE_UP_POTION)
		{
			DefenceUpPotionAct(stat);
		}
	}

private:
	float newHp;
	float newSpeed;
	float newAtk;
	float newDef;

	void HpPotionAct(int stat)
	{
		newHp = stat + 2;
	}

	void SpeedUpPotionAct(int stat)
	{
		newSpeed = stat + 10;
	}

	void DamageUpPotionAct(int stat)
	{
		newAtk = stat + 5;
	}

	void DefenceUpPotionAct(int stat)
	{
		newDef = stat + 5;
	}
};


int main()
{
    return 0;
}

