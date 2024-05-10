using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class HeroesLobbyManager : MonoBehaviourPunCallbacks
{
    public LobbyController lobbyController;
    public GameObject SelectionPanel;
    public GameObject CreateRoomPanel;
    public GameObject ListRoomPanel;
    public GameObject InRoomPanel;
    public GameObject CenterPanel;
    public GameObject ScrollContent;
    public GameObject listPlayerScrollContent;


    public Text progressStatus;
    public Text PlayerNameInput;
    public Text RoomNameInput;
    public Text roomMaxplayerTxt;
    public Button StartGameButton;
    public Button loginButton;
    public Text roomTitle;

    public GameObject itemRoomPrefab;
    public GameObject playerPrefab;

    Dictionary<string, GameObject> itemRoomLists = new Dictionary<string, GameObject>();
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        Screen.fullScreen = true;
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);

        PlayerNameInput.text = "Player" + Random.Range(1111, 9999);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Login
    public void OnPlayerLoginClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
            CenterPanel.SetActive(false);
            loginButton.gameObject.SetActive(false);
            lobbyController.SetDinoActive(false);
        }
        else
        {

            PhotonNetwork.LocalPlayer.NickName = "Player" + Random.Range(1111, 9999).ToString();
            PhotonNetwork.ConnectUsingSettings();
            CenterPanel.SetActive(false);
            loginButton.gameObject.SetActive(false);
            lobbyController.SetDinoActive(false);
        }
    }
    public override void OnConnectedToMaster()
    {
        SelectionPanel.SetActive(true);
        Debug.Log("OnConnectedToMaster");
    }


    //Create Room
    public void ShowCreateRoom()
    {
        CreateRoomPanel.SetActive(true);
        SelectionPanel.SetActive(false);
    }
    public void CreateRoomPanelBack()
    {
        CreateRoomPanel.SetActive(false);
        ListRoomPanel.SetActive(false);
        InRoomPanel.SetActive(false);
        SelectionPanel.SetActive(true);
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = RoomNameInput.text;
        int maxPlayer = 2;
        int.TryParse(roomMaxplayerTxt.text, out maxPlayer);
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayer, PlayerTtl = 10000 };
        PhotonNetwork.CreateRoom(roomName, options, null);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
        cachedRoomList.Clear();

        CreateRoomPanel.SetActive(false);
        InRoomPanel.SetActive(true);
        ListRoomPanel.SetActive(false);

        //SetActivePanel(InsideRoomPanel.name);


        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(playerPrefab);
            entry.transform.SetParent(listPlayerScrollContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerItemController>().Initialize(p.ActorNumber, p.NickName);

        }
        listPlayerScrollContent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, PhotonNetwork.PlayerList.Length * 100);
        roomTitle.text = "ROOM: " + PhotonNetwork.CurrentRoom.Name;
        StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
    public void InRoomBack()
    {
        CreateRoomPanel.SetActive(false);
        InRoomPanel.SetActive(false);
        ListRoomPanel.SetActive(false);
        SelectionPanel.SetActive(true);
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void ShowListRoom()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ListRoomPanel.SetActive(true);
    }
    public void ListRoomPanelBack()
    {
        ListRoomPanel.SetActive(false);
        CreateRoomPanel.SetActive(false);
        InRoomPanel.SetActive(false);
        SelectionPanel.SetActive(true);
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");

    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate " + roomList.Count);
        //ClearRoomListView
        foreach (GameObject entry in itemRoomLists.Values)
        {
            Destroy(entry.gameObject);
        }

        itemRoomLists.Clear();

        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(itemRoomPrefab);
            entry.transform.SetParent(ScrollContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            entry.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            entry.GetComponent<RoomItemController>().Initialize(info.Name, (byte)info.PlayerCount, (byte)info.MaxPlayers);

            itemRoomLists.Add(info.Name, entry);
        }
        ScrollContent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cachedRoomList.Values.Count * 100);

    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("NewPlayer Join");
        GameObject entry = Instantiate(playerPrefab);
        entry.transform.SetParent(listPlayerScrollContent.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerItemController>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.LoadLevel("multiplayerP");
    }
}
