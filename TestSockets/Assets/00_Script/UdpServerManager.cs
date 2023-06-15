using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Code.Shared;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace _00_Script
{
    public class UdpServerManager : MonoBehaviour, INetEventListener
    {
        private NetManager _netManager;
        private NetPacketProcessor _packetProcessor;

        public const int MaxPlayers = 64;

        private readonly NetDataWriter _cachedWriter = new NetDataWriter();
        private ushort _serverTick;

        private PlayerInputPacket _cachedCommand = new PlayerInputPacket();
        private ServerState _serverState;
        public ushort Tick => _serverTick;


        public void StartServer()
        {
            if (_netManager.IsRunning)
                return;
            _netManager.Start(2424);
        }

        private void Awake()
        {
            _packetProcessor = new NetPacketProcessor();


            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), r => r.GetVector2());


            _packetProcessor.RegisterNestedType<PlayerState>();
            _packetProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);
            _netManager = new NetManager(this) { AutoRecycle = true };
            StartServer();
        }

        private void Update()
        {
            _netManager.PollEvents();
            
        }
        
        
        private void OnJoinReceived(JoinPacket joinPacket, NetPeer peer)
        {
            Debug.Log("[S] Join packet received: " + joinPacket.UserName);
        }


        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[S] Player connected: " + peer.EndPoint);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[S] Player disconnected: " + disconnectInfo.Reason);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log("network error happended on the server ");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            Debug.Log($"data received:{Encoding.ASCII.GetString(reader.RawData)}");

            _netManager.SendToAll(WriteSerializable(PacketType.Shoot, new ShootPacket { CommandId = 1 }), DeliveryMethod.ReliableUnordered);
        }


        private NetDataWriter WriteSerializable<T>(PacketType type, T packet) where T : struct, INetSerializable
        {
            _cachedWriter.Reset();
            _cachedWriter.Put((byte)type);
            packet.Serialize(_cachedWriter);
            return _cachedWriter;
        }


        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            Debug.Log($"connection request received:{request}");
            request.AcceptIfKey("ExampleGame");
        }
    }
}