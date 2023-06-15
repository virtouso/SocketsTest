using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Code.Shared;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace _00_Script
{
    public class UdpClientManager : MonoBehaviour, INetEventListener
    {
        private Action<DisconnectInfo> _onDisconnected;

        private NetManager _netManager;
        private NetDataWriter _writer;
        private NetPacketProcessor _packetProcessor;
        private NetPeer _server;

        private ServerState _cachedServerState;


        private void Awake()
        {
            _cachedServerState = new ServerState();
            _writer = new NetDataWriter();
            _packetProcessor = new NetPacketProcessor();
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
            _packetProcessor.RegisterNestedType<PlayerState>();
            _packetProcessor.SubscribeReusable<PlayerJoinedPacket>(OnPlayerJoined);
            _packetProcessor.SubscribeReusable<JoinAcceptPacket>(OnJoinAccept);
            _packetProcessor.SubscribeReusable<PlayerLeavedPacket>(OnPlayerLeaved);
            _netManager = new NetManager(this) { AutoRecycle = true, IPv6Enabled = false };
            _netManager.Start();
            Connect("localhost", OnDisconnected);
        }


        public void Connect(string ip, Action<DisconnectInfo> onDisconnected)
        {
            _onDisconnected = onDisconnected;
            _netManager.Connect(ip, 2424, "ExampleGame");
        }


        public void SendPacketSerializable<T>(PacketType type, T packet, DeliveryMethod deliveryMethod) where T : INetSerializable
        {
            if (_server == null)
                return;
            _writer.Reset();
            _writer.Put((byte)type);
            packet.Serialize(_writer);
            _server.Send(_writer, deliveryMethod);
        }

        public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new()
        {
            if (_server == null)
                return;
            _writer.Reset();
            _writer.Put((byte)PacketType.Serialized);
            _packetProcessor.Write(_writer, packet);
            _server.Send(_writer, deliveryMethod);
        }


        private void OnDisconnected(DisconnectInfo info)
        {
            Debug.Log($"disconnected:{info}");
        }


        private void OnPlayerJoined(PlayerJoinedPacket packet)
        {
            Debug.Log($"player joined:{packet}");
        }

        private void OnPlayerLeaved(PlayerLeavedPacket packet)
        {
            Debug.Log($"player left:{packet}");
        }


        private void OnJoinAccept(JoinAcceptPacket packet)
        {
            Debug.Log($"Server Join Accepted");
        }


        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[C] Connected to server: " + peer.EndPoint);
            _server = peer;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            Debug.Log($"server response:{Encoding.UTF8.GetString(reader.RawData)}");
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Reject();
        }


        private void Update()
        {
            _netManager.PollEvents();

            _writer.Reset();
            _writer.Put(Encoding.UTF8.GetBytes($"{DateTime.Now.ToString()}"));
            //   packet.Serialize(_writer);
            _server.Send(_writer, DeliveryMethod.ReliableOrdered);
        }
    }
}