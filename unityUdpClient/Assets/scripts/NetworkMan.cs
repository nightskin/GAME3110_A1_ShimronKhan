using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

public class NetworkMan : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<GameObject> p_objs;
    UdpClient udp;

    void Start()
    {
        udp = new UdpClient();
        p_objs = new List<GameObject>();

        udp.Connect("3.130.163.55", 12345);
        //udp.Connect("192.168.1.101", 12345);
        Byte[] sendBytes = Encoding.ASCII.GetBytes("connect");

        udp.Send(sendBytes, sendBytes.Length);

        udp.BeginReceive(new AsyncCallback(OnReceived), udp);
        InvokeRepeating("HeartBeat", 1, 1);
    }
    void OnDestroy()
    {
        udp.Dispose();
    }
    public enum commands
    {
        NEW_CLIENT,
        UPDATE,
        DISCONNECT_CLIENT,
    };
    [Serializable]
    public class Message
    {
        public commands cmd;
    }
    [Serializable]
    public class Player
    {
        [Serializable]
        public struct receivedPos
        {
            public float x;
            public float y;
            public float z;
        }
        public string id;
        public receivedPos pos;
    }
    [Serializable]
    public class GameState
    {
        public List<Player>players;
        public List<Player> playersDropped;
    }

    public Message latestMessage;
    public GameState latestGameState;

    void OnReceived(IAsyncResult result)
    {
        // this is what had been passed into BeginReceive as the second parameter:
        UdpClient socket = result.AsyncState as UdpClient;

        // points towards whoever had sent the message:
        IPEndPoint msgSrc = new IPEndPoint(0, 0);
        // get the actual message and fill out the source:
        byte[] message = socket.EndReceive(result, ref msgSrc);
        // do what you'd like with `message` here:
        string returnData = Encoding.ASCII.GetString(message);

        Debug.Log("Got this: " + returnData);
        latestMessage = JsonUtility.FromJson<Message>(returnData);
        try
        {
            switch (latestMessage.cmd)
            {
                case commands.NEW_CLIENT:
                    break;
                case commands.UPDATE:
                    latestGameState = JsonUtility.FromJson<GameState>(returnData);
                    break;
                case commands.DISCONNECT_CLIENT:
                    
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        // schedule the next receive operation once reading is done:
        socket.BeginReceive(new AsyncCallback(OnReceived), socket);
    }
    void SendPos(string txt)
    {
        //udp.Connect("192.168.1.101", 12345);
        udp.Connect("3.130.163.55", 12345);
        Byte[] sendBytes = Encoding.ASCII.GetBytes(txt);
        udp.Send(sendBytes, sendBytes.Length);
    }
    
    void SpawnPlayers()
    {
        if(latestGameState.players.Count > p_objs.Count)
        {
            p_objs.Add(Instantiate(playerPrefab));
        }
    }
    
    void UpdatePlayers()
    {
        for(int p = 0; p < p_objs.Count; p++)
        {
            p_objs[p].GetComponent<PlayerScript>().id = latestGameState.players[p].id;
            string pos = JsonUtility.ToJson(p_objs[p].transform.position);
            SendPos(pos);
            Debug.Log(pos);
        }
    }

    void DestroyPlayers()
    {
        
    }

    void HeartBeat()
    {
        Byte[] sendBytes = Encoding.ASCII.GetBytes("heartbeat");
        udp.Send(sendBytes, sendBytes.Length);
    }

    void Update()
    {
        SpawnPlayers();
        UpdatePlayers();
        DestroyPlayers();
    }
}
