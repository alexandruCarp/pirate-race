using UnityEngine;
using SocketIOClient;
﻿using System;
using System.Collections.Generic;
using SocketIOClient.Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SocketIOManager : MonoBehaviour
{
    private SocketIOUnity client;
    public GameObject barca;

    private Vector3 barcaPosition;

    private string textValue = "0";

    void Start()
    {
        barcaPosition = barca.transform.position;
    	var uri = new Uri("http://192.168.1.5:5000");
        client = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                }
            ,
            EIO = 4
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        client.JsonSerializer = new NewtonsoftJsonSerializer();
        
        client.On("connect", (data) =>
        {
            Debug.Log("Connected to server!");
        });

        client.On("buttonPressed", (data) =>
        {
            //string button = json.GetField("button").str;using System.Text;
            string button = data.ToString();

            if (button.Contains("1")) {
                Debug.Log("Button 1 was pressed!");

                barcaPosition += new Vector3(-1f, 0, 0);
            } else if (button.Contains("2")) {
                Debug.Log("Button 2 was pressed!");
                barcaPosition += new Vector3(1f, 0, 0);
            }
            
            Debug.Log("yoyo");
            
        });

        client.Connect();
    }

    void Update()
    {
        barca.transform.position = barcaPosition;
    }


}

