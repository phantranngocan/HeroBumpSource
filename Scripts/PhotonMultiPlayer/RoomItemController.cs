using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RoomItemController : MonoBehaviour
{
    public Text roomName;
    public Text RoomPlayersText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void JoinRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(roomName.text);
    }
    public void Initialize(string name, byte currentPlayers, byte maxPlayers)
    {
        roomName.text = name;

        RoomPlayersText.text = currentPlayers + " / " + maxPlayers;
    }
}
