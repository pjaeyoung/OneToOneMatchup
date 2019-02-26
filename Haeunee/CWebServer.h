#pragma once
class CWebServer
{
public:
	CWebServer();
	~CWebServer();

	void ServerConn();

	void OnBegin(void* userData);
	void OnData(void* userData, const char* data);
	void OnComplete(void* userData);
};

