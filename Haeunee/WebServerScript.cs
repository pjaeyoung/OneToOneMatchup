using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text;

public class WebServerScript : MonoBehaviour {
    public string nick;

    void Start () {
        DontDestroyOnLoad(this);
        SocketServer.SingleTonServ();
	}

    private void Update()
    {
        SocketServer.SingleTonServ().WaitRecieve();
    }

    public string ConnectServer(string Url, StringBuilder Info) //웹서버와 연결
    {
        StringBuilder sendInfo = Info;

        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
        byte[] byteArr = UTF8Encoding.UTF8.GetBytes(sendInfo.ToString());
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentLength = byteArr.Length;

        Stream stream = httpWebRequest.GetRequestStream();
        stream.Write(byteArr, 0, byteArr.Length);

        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        Stream result = httpWebResponse.GetResponseStream();
        StreamReader readerResult = new StreamReader(result, Encoding.Default);

        return readerResult.ReadToEnd();
    }
    
}
