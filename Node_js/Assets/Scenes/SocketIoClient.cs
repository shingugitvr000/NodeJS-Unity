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

        // JSON �����͸� ��ü�� ������ȭ
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
            // TODO: ���� UI�� ǥ���ϴ� ���� �߰�
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
            // JSON ������ ����
            MyData sendData = new MyData { message = "�ȳ�" };
            string jsonData = JsonConvert.SerializeObject(sendData);

            // WebSocket���� JSON ������ ����
            m_WebSocket.Send(jsonData);
        }
    }
}

// JSON �����͸� ���� Ŭ����
public class MyData
{
    public string message;
}