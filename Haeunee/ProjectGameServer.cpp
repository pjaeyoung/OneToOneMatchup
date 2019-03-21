// ProjectGameServer.cpp: 콘솔 응용 프로그램의 진입점을 정의합니다.
//
#pragma comment(lib,"ws2_32.lib")

#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <process.h>
#include <winsock2.h>
#include <Windows.h>
#include <string.h>
#include <iostream>
#include <list>
#include <time.h>
#include "../ProjectGameServer/jobInfo.h"
#include "../ProjectGameServer/MessageInfo.h"
using namespace std;

#define BUF_SIZE 100
#define READ 3
#define WRITE 5 


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
void MakeServer();
void Login(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void Logout(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void Enter(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void SendMsg(SOCKET sendSock, char* data);
void UserInfo(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void GameMsg(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void HitSuccess(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void UseItem(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void EndItem(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void Ready(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void EndGameScene(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void ErrorHandling(const char* msg);

class CInGameRoom //방, 아이템 정보 받아서 캐릭터의 전투력 저장
{
private:
	SOCKET userSock[2]; //소켓
	Status* userStat[2]; //변하는 스탯
	Item userItem[2]; //유저가 가진 소비 아이템 정보
	int maxHp[2];

public:
	int count = 0;
	int itemKind[10];

	void SetSock(int userNum, SOCKET sock) //두 유저 소켓 저장
	{
		userSock[userNum] = sock;
	}

	SOCKET GetSock(int userNum) //소켓 정보 가져오는 함수
	{
		return userSock[userNum];
	}

	int GetEnemyNum(int playerNum) //적의 번호 가져오기
	{
		int enemyNum = 0;
		if (playerNum == 0)
			enemyNum = 1;
		else if (playerNum == 1)
			enemyNum = 0;
		return enemyNum;
	}

	void StatSetting(int userNum, sCharInfo info) //초기 정보 저장
	{
		userStat[userNum] = new Status(info.weapon);
		Status* userAmorStat = new Status(info.cloth + 10);
		userStat[userNum]->PlusStat(userAmorStat);
		maxHp[userNum] = userStat[userNum]->GetStat(eSTATUS::em_HP);
		for (int i = 0; i < 3; i++)
			userItem[userNum].item[i] = info.item[i];
	}

	int HitHpInfo(int userNum, sHit hit) //공격 성공했을 때
	{
		int enemyNum = GetEnemyNum(userNum);
		if (hit.atkType == eATKTYPE::em_NORMAL)
			userStat[enemyNum]->hp -= (userStat[userNum]->atk - userStat[enemyNum]->def);
		else if (hit.atkType == eATKTYPE::em_OBJTHROW)
			userStat[enemyNum]->hp -= 20;
		return userStat[enemyNum]->hp;
	}

	int NowHp(int userNum) //현재 hp가져오는 함수
	{
		return userStat[userNum]->GetStat(eSTATUS::em_HP);
	}

	int NowSpeed(int userNum) //현재 speed가져오는 함수
	{
		return userStat[userNum]->GetStat(eSTATUS::em_SPEED);
	}

	sUseItem UseItem(int userNum, sUseItem useItem) //아이템 사용했을 때
	{
		Status* itemStat = new Status(userItem[userNum].item[useItem.itemNum] + 20);
		userStat[userNum]->PlusStat(itemStat);
		useItem.itemNum = userItem[userNum].item[useItem.itemNum];
		if (userStat[userNum]->GetStat(eSTATUS::em_HP) > maxHp[userNum])
			userStat[userNum]->hp = maxHp[userNum];
		useItem.hp = userStat[userNum]->GetStat(eSTATUS::em_HP);
		useItem.speed = userStat[userNum]->GetStat(eSTATUS::em_SPEED);
		return useItem;
	}

	sEndItem EndItem(int userNum, sEndItem endItem) //아이템 시간 끝
	{
		Status* itemStat = new Status(userItem[userNum].item[endItem.itemNum] + 20);
		userStat[userNum]->MinusStat(itemStat);
		endItem.itemNum = userItem[userNum].item[endItem.itemNum];
		endItem.speed = userStat[userNum]->GetStat(eSTATUS::em_SPEED);
		return endItem;
	}
};

list<CInGameRoom*> roomList; //방의 리스트
list<CInGameRoom*>::iterator roomItor;

HANDLE hMutex;

struct sUserInfo
{
	char nick[30] = { 0 };
	int userSock;
};

list<sUserInfo>loginList;
list<sUserInfo>::iterator loginItor;

int main() //서버 생성, 연결
{
	MakeServer();

    return 0;
}

void MakeServer()
{
	srand(time(NULL));
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
				roomItor = roomList.begin();
				sEnd end;
				while (roomItor != roomList.end()) //유저의 방 찾기
				{
					CInGameRoom* checkRoom = *roomItor;
					if (checkRoom->GetSock(0) == handleInfo->hClntSock || checkRoom->GetSock(1) == handleInfo->hClntSock)
					{
						if (checkRoom->GetSock(0) == handleInfo->hClntSock) 
						{
							end.result = eRESULT::em_WIN;
							SendMsg(checkRoom->GetSock(1), (char*)&end);
						}
						else if (checkRoom->GetSock(1) == handleInfo->hClntSock)
						{
							end.result = eRESULT::em_WIN;
							SendMsg(checkRoom->GetSock(0), (char*)&end);
						}
						delete(checkRoom);
						roomList.erase(roomItor);
						break;
					}
					roomItor++;
				}

				loginItor = loginList.begin();
				while (loginItor != loginList.end())
				{
					sUserInfo nowNick = *loginItor;
					if (nowNick.userSock == handleInfo->hClntSock)
					{
						loginList.erase(loginItor);
						break;
					}
					loginItor++;
				}

				closesocket(sock);
				delete(handleInfo);
				delete(ioInfo);
				continue;
			}

			if (flag.flag == eMSG::em_LOGIN) //로그인
				Login(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_LOGOUT) //로그아웃
				Logout(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_ENTER) //매칭버튼 클릭
				Enter(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_CHARINFO) //대전할 정보 받기
				UserInfo(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_MOVE || flag.flag == eMSG::em_ATK 
				|| flag.flag == eMSG::em_GETOBJ || flag.flag == eMSG::em_THROWOBJ)
				GameMsg(handleInfo, ioInfo); //움직임, 공격, 물건 줍기, 던지기
			else if (flag.flag == eMSG::em_HIT) //공격 성공
				HitSuccess(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_USEITEM) //아이템 사용
				UseItem(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_ENDITEM) //아이템 제한시간 종료
				EndItem(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_READY) //유저 게임 준비(정보에 따라 적, 본인 캐릭터 스폰)
				Ready(handleInfo, ioInfo);
			else if (flag.flag == eMSG::em_END) //게임 종료
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

void Login(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo) //로그인 한 경우
{
	sLogin loginInfo;
	memcpy(&loginInfo, ioInfo->buffer, sizeof(loginInfo));
	delete(ioInfo);
	sUserInfo user;
	strcpy_s(user.nick, loginInfo.nick);

	for (int i = 0; i < 30; i++)
	{
		if(user.nick[i] == ',')
			user.nick[i] = '\0';
	}

	WaitForSingleObject(hMutex, INFINITE);
	if (loginList.empty()) //로그인한 유저 리스트가 비어있는 경우
	{
		user.userSock = handleInfo->hClntSock;
		loginList.push_back(user);
		loginInfo.loginSucc = 0;
		SendMsg(handleInfo->hClntSock, (char*)&loginInfo);
	}
	else
	{
		loginItor = loginList.begin();
		while (loginItor != loginList.end()) //이미 로그인한 유저인지 체크
		{
			sUserInfo nowNick = *loginItor;
			loginItor++;
			if (strcmp(nowNick.nick, user.nick)==0)
			{
				loginInfo.loginSucc = 1;
				SendMsg(handleInfo->hClntSock, (char*)&loginInfo);
				break;
			}
			else if (loginItor == loginList.end())
			{
				user.userSock = handleInfo->hClntSock;
				loginList.push_back(user);
				loginInfo.loginSucc = 0;
				SendMsg(handleInfo->hClntSock, (char*)&loginInfo);
			}
		}
	}
	ReleaseMutex(hMutex);
}

void Logout(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo)
{
	loginItor = loginList.begin();
	while (loginItor != loginList.end())
	{
		sUserInfo nowNick = *loginItor;
		if (nowNick.userSock == handleInfo->hClntSock)
		{
			loginList.erase(loginItor);
			break;
		}
		loginItor++;
	}
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
		newRoom->SetSock(0, handleInfo->hClntSock);
		newRoom->SetSock(1, 0);
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
			if (checkRoom->GetSock(1) == 0) //한 자리가 비어있을 경우
			{
				checkRoom->SetSock(1, handleInfo->hClntSock);
				for (int i = 0; i < 2; i++)
				{
					room.userNum = i + 1; //먼저 들어온 유저(1) 두번째로 들어온 유저(2) 전송
					SendMsg(checkRoom->GetSock(i), (char*)&room);
				}
				break;
			}
			else if (roomItor == roomList.end()) //두 자리 다 차있는 경우
			{
				CInGameRoom* newRoom = new CInGameRoom();
				newRoom->SetSock(0, handleInfo->hClntSock);
				newRoom->SetSock(1, 0);
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
		if (checkRoom->GetSock(0) == handleInfo->hClntSock || checkRoom->GetSock(1) == handleInfo->hClntSock)
		{
			if (checkRoom->GetSock(0) == handleInfo->hClntSock)
				SendMsg(checkRoom->GetSock(1), (char*)&bufData.buffer);
			else if (checkRoom->GetSock(1) == handleInfo->hClntSock)
				SendMsg(checkRoom->GetSock(0), (char*)&bufData.buffer);
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
	sReady ready;
	while (roomItor != roomList.end()) //유저의 방 찾기
	{
		CInGameRoom* checkRoom = *roomItor;
		roomItor++;
		if (checkRoom->GetSock(0) == handleInfo->hClntSock || checkRoom->GetSock(1) == handleInfo->hClntSock)
		{
			if (checkRoom->GetSock(0) == handleInfo->hClntSock) //유저 정보대로 유저 방 안에 스탯 저장하기
			{
				checkRoom->StatSetting(0, charInfo);
				SendMsg(checkRoom->GetSock(1), (char*)&charInfo);
			}
			else if (checkRoom->GetSock(1) == handleInfo->hClntSock)
			{
				checkRoom->StatSetting(1, charInfo);
				SendMsg(checkRoom->GetSock(0), (char*)&charInfo);
			}

			checkRoom->count++;
			if (checkRoom->count == 2)
			{
				checkRoom->count = 0;
				SendMsg(checkRoom->GetSock(1), (char*)&ready);
				SendMsg(checkRoom->GetSock(0), (char*)&ready);
			}
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
		if (checkRoom->GetSock(0) == handleInfo->hClntSock) //유저 정보대로 공격 결과에 따라 hp저장, 전송
		{
			hit.hp = checkRoom->HitHpInfo(0, hit);
			hpInfo.hp = hit.hp;
			SendMsg(checkRoom->GetSock(1), (char*)&hit);
			SendMsg(checkRoom->GetSock(0), (char*)&hpInfo);
			break;
		}
		else if (checkRoom->GetSock(1) == handleInfo->hClntSock)
		{
			hit.hp = checkRoom->HitHpInfo(1, hit);
			hpInfo.hp = hit.hp;
			SendMsg(checkRoom->GetSock(0), (char*)&hit);
			SendMsg(checkRoom->GetSock(1), (char*)&hpInfo);
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
		if (checkRoom->GetSock(0) == handleInfo->hClntSock)
		{//유저 정보대로 아이템 사용 결과에 따라 hp와 speed저장, 전송
			useItem = checkRoom->UseItem(0, useItem);
			Info.hp = checkRoom->NowHp(0);
			SendMsg(checkRoom->GetSock(0), (char*)&useItem);
			SendMsg(checkRoom->GetSock(1), (char*)&Info);
			break;
		}
		else if (checkRoom->GetSock(1) == handleInfo->hClntSock)
		{
			useItem = checkRoom->UseItem(1, useItem);
			Info.hp = checkRoom->NowHp(1);
			SendMsg(checkRoom->GetSock(1), (char*)&useItem);
			SendMsg(checkRoom->GetSock(0), (char*)&Info);
			break;
		}
	}
	ReleaseMutex(hMutex);
}

void EndItem(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo)
{
	sEndItem endItem;
	memcpy(&endItem, ioInfo->buffer, sizeof(endItem));
	delete(ioInfo);
	sChangeInfo Info;
	WaitForSingleObject(hMutex, INFINITE);
	roomItor = roomList.begin();
	while (roomItor != roomList.end()) //유저의 방 찾기
	{
		CInGameRoom* checkRoom = *roomItor;
		roomItor++;
		if (checkRoom->GetSock(0) == handleInfo->hClntSock)
		{//유저 정보대로 아이템 사용이 끝난 결과에 따라 hp와 speed저장, 전송
			endItem = checkRoom->EndItem(0, endItem);
			Info.hp = checkRoom->NowHp(0);
			SendMsg(checkRoom->GetSock(0), (char*)&endItem);
			SendMsg(checkRoom->GetSock(1), (char*)&Info);
			break;
		}
		else if (checkRoom->GetSock(1) == handleInfo->hClntSock)
		{
			endItem = checkRoom->EndItem(1, endItem);
			Info.hp = checkRoom->NowHp(1);
			SendMsg(checkRoom->GetSock(1), (char*)&endItem);
			SendMsg(checkRoom->GetSock(0), (char*)&Info);
			break;
		}
	}
	ReleaseMutex(hMutex);
}

void Ready(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo) //두 유저 모두 준비
{
	delete(ioInfo);
	WaitForSingleObject(hMutex, INFINITE);
	roomItor = roomList.begin();
	sUseItem userInfo;
	sChangeInfo enemyinfo;
	sItemSpawn itemSpawn;
	
	while (roomItor != roomList.end()) //유저의 방 찾기
	{
		CInGameRoom* checkRoom = *roomItor;
		roomItor++;
		if (checkRoom->GetSock(0) == handleInfo->hClntSock || checkRoom->GetSock(1) == handleInfo->hClntSock)
		{ //던질 수 있는 아이템 종류 랜덤으로 두 유저에게 같게
			checkRoom->count++;
			if (checkRoom->count == 1)
			{
				for (int i = 0; i < 10; i++)
				{
					int spawnItemNum = rand() % 10;
					itemSpawn.itemKind[i] = spawnItemNum;
					checkRoom->itemKind[i] = spawnItemNum;
				}
			}
			else
			{
				for (int i = 0; i < 10; i++)
					itemSpawn.itemKind[i] = checkRoom->itemKind[i];
			}

			if (checkRoom->GetSock(0) == handleInfo->hClntSock) //초기 유저 정보 보내기
			{
				userInfo.hp = checkRoom->NowHp(0);
				userInfo.speed = checkRoom->NowSpeed(0);
				userInfo.itemNum = -1;
				SendMsg(checkRoom->GetSock(0), (char*)&userInfo);
				enemyinfo.hp = checkRoom->NowHp(1);
				SendMsg(checkRoom->GetSock(0), (char*)&enemyinfo);
				SendMsg(checkRoom->GetSock(0), (char*)&itemSpawn);//필드에 생성될 아이템 정보 보내기
			}
			else if (checkRoom->GetSock(1) == handleInfo->hClntSock)
			{
				userInfo.hp = checkRoom->NowHp(1);
				userInfo.speed = checkRoom->NowSpeed(1);
				userInfo.itemNum = -1;
				SendMsg(checkRoom->GetSock(1), (char*)&userInfo);
				enemyinfo.hp = checkRoom->NowHp(0);
				SendMsg(checkRoom->GetSock(1), (char*)&enemyinfo);
				SendMsg(checkRoom->GetSock(1), (char*)&itemSpawn);
			}
			break;
		}
	}
	ReleaseMutex(hMutex);
}

void EndGameScene(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo) 
{
	sEnd end;
	memcpy(&end, ioInfo->buffer, sizeof(end));
	delete(ioInfo);
	WaitForSingleObject(hMutex, INFINITE);
	roomItor = roomList.begin();
	while (roomItor != roomList.end())
	{
		CInGameRoom* checkRoom = *roomItor;
		if (checkRoom->GetSock(0) == handleInfo->hClntSock || checkRoom->GetSock(1) == handleInfo->hClntSock)
		{
			if (checkRoom->GetSock(0) == handleInfo->hClntSock)
			{ //먼저 메세지 받은 유저에게 졌다는 메세지, 상대에게 이겼다는 메세지 보내기
				end.result = eRESULT::em_WIN;
				SendMsg(checkRoom->GetSock(1), (char*)&end);
				end.result = eRESULT::em_LOSE;
				SendMsg(checkRoom->GetSock(0), (char*)&end);
			}
			else if (checkRoom->GetSock(1) == handleInfo->hClntSock)
			{
				end.result = eRESULT::em_WIN;
				SendMsg(checkRoom->GetSock(0), (char*)&end);
				end.result = eRESULT::em_LOSE;
				SendMsg(checkRoom->GetSock(1), (char*)&end);
			}
			roomList.erase(roomItor);
			delete(checkRoom);
			break;
		}
		roomItor++;
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
