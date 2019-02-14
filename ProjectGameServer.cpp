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
};

struct sRoom //매칭된 방
{
	SOCKET userSock[2];
};
list<sRoom*> roomList; //방의 리스트
list<sRoom*>::iterator roomItor;

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

struct sCharInfo :sFlag //캐릭터 정보
{
	sCharInfo() :sFlag(flag = eMSG::em_CHARINFO) {};
	int weapon; //무기
	int cloth; //방어구
	int gender, hair, hairColor, face; //캐릭터 외형 정보
};

struct sMove :sFlag //움직임
{
	sMove() :sFlag(flag = eMSG::em_MOVE) {};
	float x, y, z; //위치 좌표
	float rotX, rotY, rotZ, rotW; //회전 좌표
};

struct sAttack :sFlag //공격
{
	sAttack() :sFlag(flag = eMSG::em_ATK) {};
	int atkNum;
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
void SendMsg(LPPER_IO_DATA ioInfo, SOCKET sendSock, char* data); 
void GameMsg(LPPER_HANDLE_DATA handleInfo, LPPER_IO_DATA ioInfo);
void ErrorHandling(const char* msg);

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
			else if (flag.flag == eMSG::em_CHARINFO || flag.flag == eMSG::em_MOVE||flag.flag== em_ATK)
				GameMsg(handleInfo,ioInfo);

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
		sRoom* newRoom = new sRoom(); //방을 만들어 넣음
		newRoom->userSock[0] = handleInfo->hClntSock;
		newRoom->userSock[1] = 0;
		roomList.push_back(newRoom);
		room.userNum = 0; //아직 매칭이 되지 않음(0)
		SendMsg(ioInfo, handleInfo->hClntSock, (char*)&room);
	}
	else
	{
		roomItor = roomList.begin();
		while (roomItor!=roomList.end()) //만들어진 방에 빈자리 체크
		{
			sRoom* checkRoom = *roomItor;
			roomItor++;
			if (checkRoom->userSock[1] == 0) //한 자리가 비어있을 경우
			{
				checkRoom->userSock[1] = handleInfo->hClntSock;
				for (int i = 0; i < 2; i++)
				{
					room.userNum = i + 1; //먼저 들어온 유저(1) 두번째로 들어온 유저(2) 전송
					SendMsg(ioInfo, checkRoom->userSock[i], (char*)&room);
				}
				break;
			}
			else if (roomItor == roomList.end()) //두 자리 다 차있는 경우
			{
				sRoom* newRoom = new sRoom();
				newRoom->userSock[0] = handleInfo->hClntSock;
				newRoom->userSock[1] = 0;
				roomList.push_back(newRoom);
				room.userNum = 0;
				SendMsg(ioInfo, handleInfo->hClntSock, (char*)&room);
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
		sRoom* checkRoom = *roomItor;
		roomItor++;
		if (checkRoom->userSock[0] == handleInfo->hClntSock || checkRoom->userSock[1] == handleInfo->hClntSock)
		{
			if (checkRoom->userSock[0] == handleInfo->hClntSock)
				SendMsg(ioInfo, checkRoom->userSock[1], (char*)&bufData.buffer);
			else if (checkRoom->userSock[1] == handleInfo->hClntSock)
				SendMsg(ioInfo, checkRoom->userSock[0], (char*)&bufData.buffer);
			break;
		}
	}
	ReleaseMutex(hMutex);
}

void SendMsg(LPPER_IO_DATA ioInfo, SOCKET sendSock, char* data) //메세지 보내기
{
	ioInfo = new PER_IO_DATA;
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

