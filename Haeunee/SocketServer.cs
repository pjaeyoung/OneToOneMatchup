using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

//게임안에서 사용할 소켓서버

public class RecvData //받은 데이터와 소켓을 저장하는 클래스
{
    public byte[] buffer = new byte[100];
    public Socket socket;
}

public class SocketServer {
    private static SocketServer socketServer = null;
    public static SocketServer SingleTonServ()
    {
        if(socketServer==null)
        {
            socketServer = new SocketServer();
            socketServer.MakeServer();
        }
        return socketServer;
    }

    private static ManualResetEvent conn = new ManualResetEvent(false); //커넥트 스레드
    private static ManualResetEvent recv = new ManualResetEvent(false); //리시브 스레드
    private static Socket sock; //소켓
    static EnemyScript eScript;
    static PlayerScript pScript;
    static GameEnterScript gScript; 
    static SpawnScript sScript; 
    RecvData rd = new RecvData();
    static sGameRoom room;
    static int enterNum;
    static int gameResult;

    private void MakeServer()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //소켓생성
        var ep = new IPEndPoint(IPAddress.Parse("192.168.0.22"), 10001); //연결할 서버 정보
        var connResult = sock.BeginConnect(ep, new AsyncCallback(ConnectCallBack), sock); //커넥트
        bool connSucc = connResult.AsyncWaitHandle.WaitOne(5, true); //커넥트 성공여부 체크
        if (connSucc) //성공했을 때
            sock.EndConnect(connResult); //커넥트를 마무리함
        else //성공 못했을 때
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            //Application.Quit();
#endif
        }
        rd.socket = sock;
    }

    private static void ConnectCallBack(IAsyncResult ar) //서버와 연결되었을 때
    {
        Socket socket = (Socket)ar.AsyncState;
        Debug.Log("Socket connected to " + socket.RemoteEndPoint.ToString());
        conn.Set();
    }

    private static void ReceiveCallBack(IAsyncResult ar) //서버에게 신호를 받았을 때
    {
        RecvData recvData = (RecvData)ar.AsyncState; //리시브된 데이터
        int receive = recvData.socket.EndReceive(ar); //리시브된 정보가 있는지 체크
        if (receive <= 0) //서버 종료
        {
            recvData.socket.Close(); //소켓을 닫음
            return;
        }
        IntPtr buff = Marshal.AllocHGlobal(recvData.buffer.Length); //받은 byte 데이터를 struct로 변환
        Marshal.Copy(recvData.buffer, 0, buff, recvData.buffer.Length);
        Type type = typeof(sGameRoom);
        room = (sGameRoom)Marshal.PtrToStructure(buff, type);

        if (room.flag == (int)eMSG.em_ENTER) //매칭 버튼을 눌렀다는 정보
        {
            if(room.userNum==0) //상대가 없음
                gScript.Matching();
            else //상대가 접속
            {
                enterNum = room.userNum;
                gScript.MatchSucc();
            }
        }
        else if(room.flag==(int)eMSG.em_CHARINFO) //아이템 선택이 끝났다는 정보
        {
            Type m_type = typeof(sCharInfo);
            sCharInfo charInfo = (sCharInfo)Marshal.PtrToStructure(buff, m_type);
            if (enterNum == 1) //내가 먼저 입장
                sScript.SpawnInfo("Spawn1", "Spawn2", charInfo);
            else if (enterNum == 2) //상대가 먼저 입장
                sScript.SpawnInfo("Spawn2", "Spawn1", charInfo);
        }
        else if (eScript != null && room.flag == (int)eMSG.em_MOVE) //적의 좌표, 회전 정보
        {
            Type m_type = typeof(sMove);
            sMove move = (sMove)Marshal.PtrToStructure(buff, m_type);
            Vector3 enemyPos = new Vector3(move.x, move.y, move.z);
            Quaternion enemyRot = new Quaternion(move.rotX, move.rotY, move.rotZ, move.rotW);
            eScript.EnemyMove(enemyPos, enemyRot);
        }
        else if(room.flag==(int)eMSG.em_ATK) //적이 공격했다는 정보
        {
            Type m_type = typeof(sAtk);
            sAtk atk = (sAtk)Marshal.PtrToStructure(buff, m_type);
            eScript.EnemyAtk(atk.atkAni);
        }
        else if(room.flag==(int)eMSG.em_HIT) //내가 공격을 성공함
        {
            Type m_type = typeof(sHit);
            sHit hit = (sHit)Marshal.PtrToStructure(buff, m_type);
            pScript.ChangePlayerHp(hit.hp);
            pScript.PlayerDamage(hit);
        }
        else if (room.flag == (int)eMSG.em_INFO) //적의 hp정보가 변한경우
        {
            Type m_type = typeof(sChangeInfo);
            sChangeInfo hpInfo = (sChangeInfo)Marshal.PtrToStructure(buff, m_type);
            eScript.ChangeEnemyHp(hpInfo.hp);
        }
        else if(room.flag == (int)eMSG.em_ITEMSPAWN)
        {
            Type m_type = typeof(sItemSpawn);
            sItemSpawn itemSpawn = (sItemSpawn)Marshal.PtrToStructure(buff, m_type);
        }
        else if (room.flag == (int)eMSG.em_USEITEM) //아이템 사용
        {
            Type m_type = typeof(sEndItem);
            sEndItem endItem = (sEndItem)Marshal.PtrToStructure(buff, m_type);
            pScript.ChangeItemImg(endItem.itemNum, true);
            pScript.ChangePlayerHp(endItem.hp);
            pScript.ChangePlayerSpeed(endItem.speed);
        }
        else if (room.flag == (int)eMSG.em_ENDITEM) //아이템 시간 끝
        {
            Type m_type = typeof(sUseItem);
            sUseItem useItem = (sUseItem)Marshal.PtrToStructure(buff, m_type);
            pScript.ChangeItemImg(useItem.itemNum, false);
            pScript.ChangePlayerSpeed(useItem.speed);
        }
        else if (room.flag == (int)eMSG.em_END)
        {
            Type m_type = typeof(sEnd);
            sEnd esc = (sEnd)Marshal.PtrToStructure(buff, m_type);
            gameResult = esc.result;
            pScript.ChangeWaitScene();
        }
    }

    public void GetEnterScript(GameEnterScript enter)
    {
        gScript = enter;
    }

    public void GetSpawnScript(SpawnScript spawn)
    {
        sScript = spawn;
    }

    public void GetCharScripts(PlayerScript player, EnemyScript enemy)
    {
        eScript = enemy;
        pScript = player;
    }

    public void WaitRecieve()
    {
        sock.BeginReceive(rd.buffer, 0, rd.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), rd);
        //리시브 기다리기
        recv.Set();
    }

    public void SendMsg(object obj)//메세지 보내기 함수
    {
        int byteSize = Marshal.SizeOf(obj); //구조체를 바이트로 변환
        byte[] byteData = new byte[byteSize];
        IntPtr ptr = Marshal.AllocHGlobal(byteSize);
        Marshal.StructureToPtr(obj, ptr, true);
        Marshal.Copy(ptr, byteData, 0, byteSize);
        Marshal.FreeHGlobal(ptr);

        sock.Send(byteData);
    }

    public int GetResult()
    {
        return gameResult;
    }

}
