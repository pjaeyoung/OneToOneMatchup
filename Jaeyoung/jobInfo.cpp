// jobInfo.cpp: 콘솔 응용 프로그램의 진입점을 정의합니다.
//

#include "stdafx.h"


//무기 종류에 따른 직업
enum eJOB
{
	em_DEFAULT,
	em_WARRIOR,
	em_MAGICIAN,
	em_ARCHER,
	em_TANKER
};

//상태 정보 
enum eSTATUS 
{
	em_HP,
	em_ATk,
	em_DEF,
	em_SPEED
};

/* 획득한 무기에 따른 JOB 정보 */
class Jobs
{
	int flag;
	float hp;
	float atk;
	float def;
	float speed;

public: 
	Jobs(int f = 0)
	{
		flag = f;
		if (flag == eJOB::em_DEFAULT) 
		{

		}
		else if (flag == eJOB::em_WARRIOR) 
		{

		}
		else if (flag == eJOB::em_MAGICIAN) 
		{

		}
		else if (flag == eJOB::em_ARCHER) 
		{

		}
		else if (flag == eJOB::em_TANKER) 
		{

		}
	}

	float GetSatus(int status) 
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
};

int main()
{
    return 0;
}

