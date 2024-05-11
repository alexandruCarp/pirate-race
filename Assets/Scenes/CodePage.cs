using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CodePage : MonoBehaviour
{
    public TMP_Text text_player_count;
    public TMP_Text text_code;
    string total_players;
    string connected_players;
    // Start is called before the first frame update
    void Start()
    {
        total_players = NumarJucatori.NumJucatori;
        connected_players = SocketIOManager.getConnectedPlayers().ToString();
        text_player_count.text = connected_players + "/" + total_players + " players connected";
    }

    // Update is called once per frame
    void Update()
    {
        connected_players = SocketIOManager.getConnectedPlayers().ToString();
        text_player_count.text = connected_players + "/" + total_players + " players connected";   
    }
}
