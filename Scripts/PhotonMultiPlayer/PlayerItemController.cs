using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerItemController : MonoBehaviour
{
    public Text PlayerNameText;
    public string playerName = "";
    private int ownerId;
    public void Initialize(int playerId, string playername)
    {
        ownerId = playerId;
        playerName = playername;
        Debug.Log("playername " + playerName);
        PlayerNameText.text = "Player:  " + playerName;

    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerName.Length > 0)
            PlayerNameText.text = playerName;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
