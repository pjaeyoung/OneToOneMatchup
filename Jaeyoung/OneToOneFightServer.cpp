// ProjectGameServer.cpp: 콘솔 응용 프로그램의 진입점을 정의합니다.
//
#pragma comment(lib,"ws2_32.lib")

#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <process.h>
#include <winsock2.h>
#include <Windows.h>
#include <iostream>
#include <list>
#include "../OneToOneFightServer/jobInfo.h"
using namespace std;

#define BUF_SIZE 100
#define READ 3
#define WRITE 5 

enum eMSG //메세지 종류
{
	em_ENTER = 1,
	em_CHARINFO,
	em_MOVE,
	em_ATK,
	em_HIT,
	em_INFO,
	em_USEITEM,
	em_READY,
	em_ESC,
	em_DEAD,
};

struct sFlag //어떤 정보를 보냈는지 구분할 플래그
{
	int flag;
	sFlag(int f = 0) : flag(f) {};
};

struct sGameRoom : sFlag
{
	sGameRoom() :sFlag(flag = eMSG::em_ENTER) {};
	int userNum = 0; //방에 들어온 유저들을 구분하기 위한 것
};

struct Item
{
	int item[3];
};

struct sCharInfo :sFlag //캐릭터 정보
{
	sCharInfo() :sFlag(flag = eMSG::em_CHARINFO) {};
	int weapon; //무기
	int cloth; //방어구
	int gender, hair, hairColor, face; //캐릭터 외형 정보
	int item[3];
};

struct sMove :sFlag //움직임
{
	sMove() :sFlag(flag = eMSG::em_MOVE) {};
	float x, y, z; //위치 좌표
	float rotX, rotY, rotZ, rotW; //회전 좌표
};

struct sAtk :sFlag //공격
{
	sAtk() :sFlag(flag = eMSG::em_ATK) {};
	int atkNum;
};

struct sHit :sFlag //공격 성공
{
	sHit() :sFlag(flag = eMSG::em_HIT) {};
	int dmgAni;
	int hp;
};

struct sChangeInfo :sFlag //hp정보 전달
{
	sChangeInfo() :sFlag(flag = eMSG::em_INFO) {};
	int hp;
	int speed;
};

struct sUseItem :sFlag //아이템 사용
{
	sUseItem() :sFlag(flag = eMSG::em_USEITEM) {};
	int itemNum;
	int hp;
};

struct sESC : sFlag //항복 버튼 사용 
{
	sESC() : sFlag(flag = eMSG::em_ESC) {};
};

struct sDEAD : sFlag // 죽음
{
	sDEAD() : sFlag(flag = eMSG::em_DEAD) {};
};

typedef struct
{
	SOCKET hClntSock;
	SOCKADDR_IN clntAddr;
}PER_HANDLE_DATA, *LPPER_HANDLE_DATA;

typedef struct
{
	OVERLAPPED overlapped;
	WSABUF wsabuf;
	char buffer[BUF_SIZE];
	int rwMode;
}PER_IO_DATA, *LPPER_IO_DATA;

unsigned WINAPI GameThread(LPVOID CompletionPortIO);
void Enter(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void SendMsg(SOCKET sendSock, char* data);
void UserInfo(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void GameMsg(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void HitSuccess(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void UseItem(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void Ready(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void EndGameScene(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void ErrorHandling(const char* msg);

class CInGameRoom //방, 아이템 정보 받아서 캐릭터의 전투력 저장한다!
{
public:
	SOCKET userSock[2];
	Status* userStat[2];
	Item userItem[2];
	int GetEnemyNum(int playerNum)
	{
		int enemyNum = 0;
		if (playerNum == 0)
			enemyNum = 1;
		else if (playerNum == 1)
			enemyNum = 0;
		return enemyNum;
	}

	void StatSetting(int userNum, sCharInfo info)
	{
		userStat[userNum] = new Status(info.weapon);
		Status* userAmorStat = new Status(info.cloth + 10);
		userStat[userNum]->ChangeStat(userAmorStat);
		for (int i = 0; i < 3; i++)
			userItem[userNum].item[i] = info.item[i];
	}

	int HitHpInfo(int userNum,sHit hit)
	{
		int enemyNum = GetEnemyNum(userNum);
		userStat[enemyNum]->hp -= (userStat[userNum]->atk - userStat[enemyNum]->def);
		return userStat[enemyNum]->hp;
	}

	int NowHp(int userNum)
	{
		return userStat[userNum]->hp;
	}

	int NowSpeed(int userNum)
	{
		return userStat[userNum]->speed;
	}

	sUseItem UseItem(int userNum, sUseItem useItem)
	{
		Status* itemStat = new Status(userItem[userNum].item[useItem.itemNum] + 20);
		userStat[userNum]->ChangeStat(itemStat);
		useItem.itemNum = userItem[userNum].item[useItem.itemNum];
		useItem.hp = userStat[userNum]->hp;
		return useItem;
	}

	void InputAllData(int userNum)
	{
		int enemyNum = GetEnemyNum(userNum);
		sChangeInfo info;
		info.hp = userStat[enemyNum]->hp;
		info.speed = userStat[userNum]->speed;
		SendMsg(userSock[userNum], (char*)&info);
	}
};

list<CInGameRoom*> roomList; //방의 리스트
list<CInGameRoom*>::iterator roomItor;


HANDLE hMutex;

int main() //서버 생성, 연결
{
	WSADATA wsaData;
	HANDLE hComport;
	SYSTEM_INFO sysInfo;
	LPPER_IO_DATA ioInfo;
	LPPER_HANDLE_DATA handleInfo;

	SOCKET hServSock;
	SOCKADDR_IN servAddr;
	unsigned long recvBytes, i, flags = 0;

	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
		ErrorHandling("WSAStartup() error!");

	hComport = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0);
	GetSystemInfo(&sysInfo);
	for (i = 0; i < sysInfo.dwNumberOfProcessors; i++)
		_beginthreadex(NULL, 0, GameThread, (LPVOID)hComport, 0, NULL);

	hServSock = WSASocket(AF_INET, SOCK_STREAM, 0, NULL, 0, WSA_FLAG_OVERLAPPED);
	memset(&servAddr, 0, sizeof(servAddr));
	servAddr.sin_family = AF_INET;
	servAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	servAddr.sin_port = htons(10001);

	if (bind(hServSock, (SOCKADDR*)&servAddr, sizeof(servAddr)))
		ErrorHandling("bind() error!");

	if (listen(hServSock, 5) == SOCKET_ERROR)
		ErrorHandling("listen() error!");

	hMutex = CreateMutex(NULL, FALSE, NULL);
	while (true)
	{
		SOCKET hClntSock;
		SOCKADDR_IN clntAddr;
		int addrLen = sizeof(clntAddr);

		hClntSock = accept(hServSock, (SOCKADDR*)&clntAddr, &addrLen);
		handleInfo = new PER_HANDLE_DATA;
		handleInfo->hClntSock = hClntSock;
		memcpy(&(handleInfo->clntAddr), &clntAddr, addrLen);

		CreateIoCompletionPort((HANDLE)hClntSock, hComport, (DWORD)handleInfo, 0);

		ioInfo = new PER_IO_DATA;
		memset(&(ioInfo->overlapped), 0, sizeof(OVERLAPPED));

		ioInfo->wsabuf.len = BUF_SIZE;
		ioInfo->wsabuf.buf = ioInfo->buffer;
		ioInfo->rwMode = READ;
		WSARecv(handleInfo->hClntSock, &(ioInfo->wsabuf), 1, &recvBytes, &flags, &(ioInfo->overlapped), NULL);
	}

    return 0;
}

unsigned WINAPI GameThread(LPVOID pComport)
{
	HANDLE hComport = pComport;
	SOCKET sock;
	DWORD bytesTrans;
	LPPER_HANDLE_DATA handleInfo;
	LPPER_IO_DATA ioInfo;
	DWORD flags = 0;

	while (true)
	{
		GetQueuedCompletionStatus(hComport, &bytesTrans, (LPDWORD)&handleInfo, (LPOVERLAPPED*)&ioInfo, INFINITE);
		sock = handleInfo->hClntSock;
		sFlag flag; //플레그 받아오기
		memcpy(&flag.flag, ioInfo->buffer, sizeof(int));

		if (ioInfo->rwMode == READ)
		{
			if (bytesTrans <= 0)
			{
				closesocket(sock);
				delete(handleInfo);
				delete(ioInfo);
				continue;
			}

			if (flag.flag == eMSG::em_ENTER) //입장
				Enter(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_CHARINFO)
				UserInfo(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_MOVE || flag.flag == em_ATK)
				GameMsg(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_HIT)
				HitSuccess(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_USEITEM)
				UseItem(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_READY)
				Ready(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_ESC || flag.flag == eMSG::em_DEAD)
				EndGameScene(handleInfo, ioInfo);

			ioInfo = new PER_IO_DATA;
			memset(&(ioInfo->overlapped), 0, sizeof(OVERLAPPED));
			ioInfo->wsabuf.len = BUF_SIZE;
			ioInfo->wsabuf.buf = ioInfo->buffer;
			ioInfo->rwMode = READ;
			WSARecv(sock, &(ioInfo->wsabuf), 1, NULL, &flags, &(ioInfo->overlapped), NULL);
		}
		else
		{
			delete(ioInfo);
		}
	}

	return 0;
}

void Enter(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo)
{
	sGameRoom room;
	memcpy(&room, ioInfo->buffer, sizeof(room));
	delete(ioInfo);
	WaitForSingleObject(hMutex, INFINITE);
	if (roomList.empty() == true) //만들어진 방이 없을 때
	{
		CInGameRoom* newRoom = new CInGameRoom(); //방을 만들어 넣음
		newRoom->userSock[0] = handleInfo->hClntSock;
		newRoom->userSock[1] = 0;
		roomList.push_back(newRoom);
		room.userNum = 0; //아직 매칭이 되지 않음(0)
		SendMsg(handleInfo->hClntSock, (char*)&room);
	}
	else
	{
		roomItor = roomList.begin();
		while (roomItor!=roomList.end()) //만들어진 방에 빈자리 체크
		{
			CInGameRoom* checkRoom = *roomItor;
			roomItor++;
			if (checkRoom->userSock[1] == 0) //한 자리가 비어있을 경우
			{
				checkRoom->userSock[1] = handleInfo->hClntSock;
				for (int i = 0; i < 2; i++)
				{
					room.userNum = i + 1; //먼저 들어온 유저(1) 두번째로 들어온 유저(2) 전송
					SendMsg(checkRoom->userSock[i], (char*)&room);
				}
				break;
			}
			else if (roomItor == roomList.end()) //두 자리 다 차있는 경우
			{
				CInGameRoom* newRoom = new CInGameRoom();
				newRoom->userSock[0] = handleInfo->hClntSock;
				newRoom->userSock[1] = 0;
				roomList.push_back(newRoom);
				room.userNum = 0;
				SendMsg(handleInfo->hClntSock, (char*)&room);
			}
		}
	}
	ReleaseMutex(hMutex);
}

void GameMsg(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo)
{ //받은 데이터 그대로 상대에게 전송하기
	PER_IO_DATA bufData = *ioInfo;
	delete(ioInfo);
	WaitForSingleObject(hMutex, INFINITE);
	roomItor = roomList.begin();
	while (roomItor != roomList.end())
	{
		CInGameRoom* checkRoom = *roomItor;
		roomItor++;
		if (checkRoom->userSock[0] == handleInfo->hClntSock || checkRoom->userSock[1] == handleInfo->hClntSock)
		{
			if (checkRoom->userSock[0] == handleInfo->hClntSock)
				SendMsg(checkRoom->userSock[1], (char*)&bufData.buffer);
			else if (checkRoom->userSock[1] == handleInfo->hClntSock)
				SendMsg(checkRoom->userSock[0], (char*)&bufData.buffer);
			break;
		}
	}
	ReleaseMutex(hMutex);
}

void UserInfo(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo)
{
	sCharInfo charInfo;
	memcpy(&charInfo, ioInfo->buffer, sizeof(charInfo));
	delete(ioInfo);
	WaitForSingleObject(hMutex, INFINITE);
	roomItor = roomList.begin();
	sHit hit;
	while (roomItor != roomList.end()) //유저의 방 찾기
	{
		CInGameRoom* checkRoom = *roomItor;
		roomItor++;
		if (checkRoom->userSock[0] == handleInfo->hClntSock) //유저 정보대로 유저 방 안에 스탯 저장하기
		{
			checkRoom->StatSetting(0, charInfo);
			SendMsg(checkRoom->userSock[1], (char*)&charInfo);
		}
		else if (checkRoom->userSock[1] == handleInfo->hClntSock)
		{
			checkRoom->StatSetting(1, charInfo);
			SendMsg(checkRoom->userSock[0], (char*)&charInfo);
		}
	}
	ReleaseMutex(hMutex);
}

void HitSuccess(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo)
{
	sHit hit;
	memcpy(&hit, ioInfo->buffer, sizeof(hit));
	delete(ioInfo);
	WaitForSingleObject(hMutex, INFINITE);
	roomItor = roomList.begin();
	sChangeInfo hpInfo;
	while (roomItor != roomList.end()) //유저의 방 찾기
	{
		CInGameRoom* checkRoom = *roomItor;
		roomItor++;
		if (checkRoom->userSock[0] == handleInfo->hClntSock) //유저 정보대로 공격 결과에 따라 hp저장, 전송
		{
			hit.hp = checkRoom->HitHpInfo(0, hit);
			hpInfo.hp = hit.hp;
			SendMsg(checkRoom->userSock[1], (char*)&hit);
			SendMsg(checkRoom->userSock[0], (char*)&hpInfo);
			break;
		}
		else if (checkRoom->userSock[1] == handleInfo->hClntSock)
		{
			hit.hp = checkRoom->HitHpInfo(1, hit);
			hpInfo.hp = hit.hp;
			SendMsg(checkRoom->userSock[0], (char*)&hit);
			SendMsg(checkRoom->userSock[1], (char*)&hpInfo);
			break;
		}
	}
	ReleaseMutex(hMutex);
}

void UseItem(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo)
{
	sUseItem useItem;
	memcpy(&useItem, ioInfo->buffer, sizeof(useItem));
	delete(ioInfo);
	sChangeInfo Info;
	WaitForSingleObject(hMutex, INFINITE);
	roomItor = roomList.begin();
	while (roomItor != roomList.end()) //유저의 방 찾기
	{
		CInGameRoom* checkRoom = *roomItor;
		roomItor++;
		if (checkRoom->userSock[0] == handleInfo->hClntSock) 
		{//유저 정보대로 아이템 사용 결과에 따라 hp와 speed저장, 전송
			useItem = checkRoom->UseItem(0, useItem);
			Info.hp = checkRoom->NowHp(1);
			Info.speed = checkRoom->NowSpeed(0);
			SendMsg(checkRoom->userSock[0], (char*)&useItem);
			SendMsg(checkRoom->userSock[1], (char*)&Info);
			break;
		}
		else if (checkRoom->userSock[1] == handleInfo->hClntSock)
		{
			useItem = checkRoom->UseItem(1, useItem);
			Info.hp = checkRoom->NowHp(0);
			Info.speed = checkRoom->NowSpeed(1);
			SendMsg(checkRoom->userSock[1], (char*)&useItem);
			SendMsg(checkRoom->userSock[0], (char*)&Info);
			break;
		}
	}
	ReleaseMutex(hMutex);
}

void Ready(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo)
{
	delete(ioInfo);
	WaitForSingleObject(hMutex, INFINITE);
	roomItor = roomList.begin();
	sHit hit;
	while (roomItor != roomList.end()) //유저의 방 찾기
	{
		CInGameRoom* checkRoom = *roomItor;
		roomItor++;
		if (checkRoom->userSock[0] == handleInfo->hClntSock) //초기 유저 정보 보내기
		{
			hit.hp = checkRoom->NowHp(0);
			SendMsg(checkRoom->userSock[0], (char*)&hit);
			checkRoom->InputAllData(0);
			break;
		}
		else if (checkRoom->userSock[1] == handleInfo->hClntSock)
		{
			hit.hp = checkRoom->NowHp(1);
			SendMsg(checkRoom->userSock[1], (char*)&hit);
			checkRoom->InputAllData(1);
			break;
		}
	}
	ReleaseMutex(hMutex);
}

void EndGameScene(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo) //방 전체 유저에게 받은 메세지 되돌리기 
{
	PER_IO_DATA bufData = *ioInfo;
	delete(ioInfo);
	WaitForSingleObject(hMutex, INFINITE);
	roomItor = roomList.begin();
	while (roomItor != roomList.end())
	{
		CInGameRoom* checkRoom = *roomItor;
		roomItor++;
		if (checkRoom->userSock[0] == handleInfo->hClntSock || checkRoom->userSock[1] == handleInfo->hClntSock)
		{
			SendMsg(checkRoom->userSock[0], (char*)&bufData.buffer);
			SendMsg(checkRoom->userSock[1], (char*)&bufData.buffer);
			break;
		}
	}
	ReleaseMutex(hMutex);
}

void SendMsg(SOCKET sendSock, char* data) //메세지 보내기
{
	LPPER_IO_DATA ioInfo = new PER_IO_DATA;
	memcpy(ioInfo->buffer, data, BUF_SIZE);
	ioInfo->wsabuf.buf = ioInfo->buffer;
	memset(&(ioInfo->overlapped), 0, sizeof(OVERLAPPED));
	ioInfo->wsabuf.len = BUF_SIZE;
	ioInfo->rwMode = WRITE;
	WSASend(sendSock, &(ioInfo->wsabuf), 1, NULL, 0, &(ioInfo->overlapped), NULL);
}


void ErrorHandling(const char* msg)
{
	fputs(msg, stderr);
	fputc('\n', stderr);
	exit(1);
}

