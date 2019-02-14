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
enum eMSG //메세지 종류
{
    em_ENTER = 1,
    em_CHARINFO,
    em_MOVE,
    em_ATK,
};

struct sGameRoom //매칭 정보
{
    public int flag;
    public int userNum;
    public sGameRoom(int f, int u)
    {
        flag = f;
        userNum = 0;
    }
}

public struct sCharInfo //획득한 아이템 정보
{
    private int flag;
    public int weapon; //무기
    public int cloth; //방어구
    public int gender, hair, hairColor, face; //캐릭터 외형 정보

    public sCharInfo(int f, int w, int c, int gen, int inputHair, int inputColor, int inputFace)
    {
        flag = f;
        weapon = w;
        cloth = c;
        gender = gen;
        hair = inputHair;
        hairColor = inputColor;
        face = inputFace;
    }
}

public struct sMove //움직임, 회전
{
    private int flag;
    public float x, y, z;
    public float rotX, rotY, rotZ, rotW;

    public sMove(int f, float inputX, float inputY, float inputZ, float inrotX, float inrotY, float inrotZ, float inrotW)
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
    public sAtk(int f, int ani)
    {
        flag = f;
        atkAni = ani;
    }
}

public class RecvData //받은 데이터와 소켓을 저장하는 클래스
{
    public byte[] buffer = new byte[100];
    public Socket socket;
}

public class SocketServer : MonoBehaviour {
    private static ManualResetEvent conn = new ManualResetEvent(false); //커넥트 스레드
    private static ManualResetEvent recv = new ManualResetEvent(false); //리시브 스레드
    Socket sock; //소켓
    GameObject enemyObj; //현재 스폰된 적 오브젝트
    static EnemyScript eScript;
    bool gameScene = false; //게임씬에 입장했는지 여부
    static GameEnterScript gScript; 
    static SpawnScript sScript; 
    RecvData rd = new RecvData();
    static sGameRoom room;
    static int enterNum;

    void Start()
    {
        DontDestroyOnLoad(this); //서버 오브젝트 파괴되지 않게 함

        gScript = GetComponent<GameEnterScript>();
        sScript = GetComponent<SpawnScript>();
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //소켓생성
        var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10001); //연결할 서버 정보
        var connResult = sock.BeginConnect(ep, new AsyncCallback(ConnectCallBack), sock); //커넥트
        bool connSucc = connResult.AsyncWaitHandle.WaitOne(5, true); //커넥트 성공여부 체크
        if (connSucc) //성공했을 때
            sock.EndConnect(connResult); //커넥트를 마무리함
        else //성공 못했을 때
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        rd.socket = sock;
    }

    // Update is called once per frame
    void Update () {
        sock.BeginReceive(rd.buffer, 0, rd.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), rd);
        //리시브 기다리기
        recv.Set();
        if (sScript.nowEnemy!=null && gameScene == false) //게임 씬에서 적이 스폰된 후 적 정보 가져오기
        {
            enemyObj = sScript.nowEnemy;
            eScript = enemyObj.GetComponent<EnemyScript>();
            gameScene = true;
        }
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
}
