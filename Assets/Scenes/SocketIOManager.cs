using UnityEngine;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;   
using System.Net; 
using SocketIOClient.Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SocketIOManager : MonoBehaviour
{
    private static SocketIOUnity client = null;
    static int connected_players = 0;

    public static int getConnectedPlayers()
    {
        return connected_players;
    }

    public static void emit(string target,string data)
    {
        if (client == null) {
            init();
            Thread.Sleep(1000);
        }
        client.Emit(target, data);
        Debug.Log("EMITTED" + target + " " + data);
    }
    static void init()
    {
        connected_players = 0;
        string hostName = Dns.GetHostName(); 
        string IP = Dns.GetHostByName(hostName).AddressList[0].ToString();
    	var uri = new Uri("http://" + IP + ":5000");
        client = new SocketIOUnity(uri);
        client.JsonSerializer = new NewtonsoftJsonSerializer();
        
        client.On("connect", (data) =>
        {
            Debug.Log("Connected to server!");
        });

        client.On("new_connection", (data) =>
        {
            Debug.Log("New player connected!");
            connected_players++;
        });

        client.Connect();
    }

}

