using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public static LobbyController inst;
    public GameObject lobbyPanel;
    public GameObject roomPanel;

    public GameObject textCoutndown;
    public Text countDownNumber;
    public GameObject gameLobby;
    public int numberPlayerInRoom = 0;
    public Sprite[] playerIcon;
    public GameObject[] listSlotPlayer;
    //--top
    public Text txtPlayerName;
    public Image imgAvatar;
    //--dino info
    public GameObject[] dinos;
    private GameObject dino;
    public Text txtDinoName;
    public Image strengthBar;
    public Image speedBar;
    public Text txtStrength;
    public Text txtSpeed;

    public static int dinoid = -1;
    private void Awake()
    {
        if (inst == null)
            inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        txtPlayerName.text = "Player";// + NetworkMgr.inst.pid;
        imgAvatar.sprite = playerIcon[0];//[NetworkMgr.inst.avatarid];
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }

    //--select character -------------------------------------------------------
    public void OnNextDino()
    {
        dinoid++;
        if (dinoid >= dinos.Length)
        {
            dinoid = 0;
        }
        CreateDino();
    }
    public void OnPreviousDino()
    {
        dinoid--;
        if (dinoid < 0)
        {
            dinoid = dinos.Length - 1;
        }
        CreateDino();
    }
    private void CreateDino()
    {
        if (dino != null)
        {
            Destroy(dino);
        }
        dino = Instantiate(dinos[dinoid]);
        dino.transform.position = new Vector3(0, 0.64f, 0);
        DinoInfo info = GameMgr.inst.arrDino[dinoid];
        txtDinoName.text = info.name;

        return;
    }
    //--end select character----------------------------------------------------

    public void JoinRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        gameLobby.SetActive(false);
        dino.SetActive(false);
    }
    public void OnJoinRoom(List<PlayerInfo> players)
    {
       
        for (int i = 0; i < listSlotPlayer.Length; i++)
            if (i < players.Count)
            {
                listSlotPlayer[i].transform.GetChild(1).gameObject.SetActive(true);
                listSlotPlayer[i].transform.GetChild(2).GetComponent<Text>().text = "Player" + players[i].pid;
                listSlotPlayer[i].transform.GetChild(1).GetComponent<Image>().sprite = playerIcon[players[i].avatarid];
            }
            else
            {
                listSlotPlayer[i].transform.GetChild(1).gameObject.SetActive(false);
                listSlotPlayer[i].transform.GetChild(2).GetComponent<Text>().text = "";
            }
    }
    public void setCountDown(int cd)
    {
        countDownNumber.gameObject.SetActive(true);
        textCoutndown.SetActive(true);
        countDownNumber.text = cd + "";
    }

    public void SetDinoActive(bool isActive = true)
    {
        dino.SetActive(isActive);
    }
}
