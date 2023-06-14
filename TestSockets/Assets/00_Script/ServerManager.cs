using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ServerManager : BaseManager
{
    #region private members

    private TcpListener tcpListener;

    private Thread tcpListenerThread;

//	private TcpClient connectedTcpClient; 	

    ConcurrentDictionary<int, TcpClient> _connectedClients = new ConcurrentDictionary<int, TcpClient>();
    private int counter = 0;

    #endregion


    void Start()
    {
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
        tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8052);
        tcpListener.Start();
        Debug.Log("Server is listening");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessage();
        }
    }


    private void ListenForIncomingRequests()
    {
        
        try
        {
        
          
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (var client = tcpListener.AcceptTcpClient())
                {


                    _connectedClients.TryAdd(counter, client);
                    Thread t = new Thread(HandleClients);
                    t.Start(counter);
                    counter++;

                    // using (NetworkStream stream = client.GetStream())
                    // {
                    //     int length;
                    //
                    //     while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    //     {
                    //         var incommingData = new byte[length];
                    //         Array.Copy(bytes, 0, incommingData, 0, length);
                    //         string clientMessage = Encoding.ASCII.GetString(incommingData);
                    //         Debug.Log("client message received as: " + clientMessage);
                    //     }
                    // }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
        
     
    }


    private void SendMessage()
    {
        // if (connectedTcpClient == null) {             
        // 	return;         
        // }  		

        try
        {
            // Get a stream object for writing. 	

            foreach (var item in _connectedClients)
            {
                NetworkStream stream = item.Value.GetStream();
                if (stream.CanWrite)
                {
                    string serverMessage = DateTime.Now.ToString();
                    // Convert string message to byte array.                 
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);

                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                    Debug.Log("Server sent his message - should be received by client");
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }



    void HandleClients(object input)
    {
        
    }




}