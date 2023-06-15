using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class ClientManager : BaseManager
{
    #region private members

    private TcpClient socketConnection;
    private Thread clientReceiveThread;

    #endregion

   
    async void StartClient()
    {
        await Task.Delay(1000);
        ConnectToTcpServer();
    }


    private void Start()
    {
        webClientButton.onClick.AddListener(StartClient);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessage();
        }
    }


    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
            base.webSocketMessageText.text = "client connect success:" ;
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
            base.webSocketMessageText.text = "client connect erro:" + e;
        }
    }


    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("127.0.0.1", 8052);
            Byte[] bytes = new Byte[100];
            while (true)
            {
           		
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                  				
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                     					
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);
                        
                        base.webSocketMessageText.text = $"received message: {serverMessage}";
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }


    private void SendMessage()
    {
        if (socketConnection == null)
        {
            return;
        }

        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = "client message:"+DateTime.Now;
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}