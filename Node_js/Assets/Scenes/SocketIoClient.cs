using UnityEngine;
using WebSocketSharp;
using System.Text;
using Newtonsoft.Json;

public class SocketIoClient : MonoBehaviour
{
    private WebSocket m_WebSocket;
    private bool m_IsConnected = false;
    private int m_ConnectionAttempt = 0;
    private const int MaxConnectionAttempts = 3;

    void Start()
    {
        ConnectWebSocket();
    }

    void ConnectWebSocket()
    {
        m_WebSocket = new WebSocket("ws://localhost:8000");
        m_WebSocket.OnOpen += OnWebSocketOpen;
        m_WebSocket.OnMessage += OnWebSocketMessage;
        m_WebSocket.OnClose += OnWebSocketClose;

        m_WebSocket.ConnectAsync();
    }

    void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connected");
        m_IsConnected = true;
        m_ConnectionAttempt = 0;
    }

    void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string jsonData = Encoding.Default.GetString(e.RawData);
        Debug.Log("Received JSON data: " + jsonData);

        // JSON 데이터를 객체로 역직렬화
        MyData receivedData = JsonConvert.DeserializeObject<MyData>(jsonData);
        Debug.Log("Received Data: " + receivedData.message);
    }

    void OnWebSocketClose(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket connection closed");
        m_IsConnected = false;

        if (m_ConnectionAttempt < MaxConnectionAttempts)
        {
            m_ConnectionAttempt++;
            Debug.Log("Attempting to reconnect. Attempt: " + m_ConnectionAttempt);
            ConnectWebSocket();
        }
        else
        {
            Debug.Log("Failed to connect after " + MaxConnectionAttempts + " attempts. Showing connection UI.");
            // TODO: 접속 UI를 표시하는 로직 추가
        }
    }

    void Update()
    {
        if (m_WebSocket == null || !m_IsConnected)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // JSON 데이터 생성
            MyData sendData = new MyData { message = "안녕" };
            string jsonData = JsonConvert.SerializeObject(sendData);

            // WebSocket으로 JSON 데이터 전송
            m_WebSocket.Send(jsonData);
        }
    }
}

// JSON 데이터를 위한 클래스
public class MyData
{
    public string message;
}