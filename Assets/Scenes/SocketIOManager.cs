using UnityEngine;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net; 
using SocketIOClient.Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class SocketIOManager : MonoBehaviour
{
    private static SocketIOUnity client = null;
    static int connected_players = 0;

    static bool all_ready = false;

    private static int[] player_boat_ids = new int[5];

    public enum ControlEventTypes
    {
        BUTTON_UP,
        BUTTON_DOWN,
        BUTTON_FIRE
    }
    public struct ControlEvent
    {
        public int player_id;
        public ControlEventTypes type;
    }

    private class ButtonPressedData
    {
        public int player_id;
        public string button;
    }
    private class NewReadyData
    {
        public int player_id;
        public int boat_id;
    }

    static Queue<ControlEvent> control_events = new Queue<ControlEvent>();
    public static bool event_available()
    {
        return control_events.Count > 0;
    }
    public static ControlEvent get_event()
    {
        return control_events.Dequeue();
    }

    public static int getConnectedPlayers()
    {
        return connected_players;
    }

    public static bool areAllReady()
    {
        return all_ready;
    }

    public static int getPlayerBoatId(int player_id)
    {
        return player_boat_ids[player_id];
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

        client.On("all_ready", (data) =>
        {
            Debug.Log("All players are ready!");
            all_ready = true;
        });

        client.On("button_pressed", (data) =>
        {
            Debug.Log("Button pressed!");

            string parsed_data = data.ToString().Substring(1, data.ToString().Length - 2);
            Debug.Log(parsed_data);

            ButtonPressedData buttonPressedData = JsonConvert.DeserializeObject<ButtonPressedData>(parsed_data);

            ControlEvent controlEvent = new ControlEvent();
            controlEvent.player_id = buttonPressedData.player_id;
            if (buttonPressedData.button == "up")
            {
                controlEvent.type = ControlEventTypes.BUTTON_UP;
            }
            else if (buttonPressedData.button == "down")
            {
                controlEvent.type = ControlEventTypes.BUTTON_DOWN;
            }
            else if (buttonPressedData.button == "fire")
            {
                controlEvent.type = ControlEventTypes.BUTTON_FIRE;
            }
            control_events.Enqueue(controlEvent);
        });
        client.On("new_ready", (data) =>
        {
            Debug.Log("New player is ready!");
            string parsed_data = data.ToString().Substring(1, data.ToString().Length - 2);
            
            NewReadyData newReadyData = JsonConvert.DeserializeObject<NewReadyData>(parsed_data);
            player_boat_ids[newReadyData.player_id] = newReadyData.boat_id;
            print("Player " + newReadyData.player_id + " is ready with boat " + newReadyData.boat_id);
        });

        client.Connect();
    }

}

