using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    enum DINO_TYPE
    {
        DINO = 0,
        SHARK,
        ROCK,//2
        PENGUIN,
        ICE_MONSTER,
        CAT,
        HUMAN//6
    }
    [SerializeField] GameObject opponents;
    [SerializeField] CameraMulti mainCam;
    [SerializeField] GameObject ringTarget;
    [SerializeField] GameObject respawnEff;
    [SerializeField] PhysicMaterial physicMat;
    [SerializeField] GameObject[] dinos;
    [SerializeField] GameObject hexagons;
    [SerializeField] GameObject foods;
    [SerializeField] GameObject[] pfFood;
    [SerializeField] Text countDownNumber;
    //--end game
    public GameObject inGameUI;
    public GameObject endGameUI;
    //--end game
    [SerializeField] Text lbScore;
    [SerializeField] Text lbNewScore;
    [SerializeField] Button btnSound;
    public GameObject[] pfEffects;
    private List<GameObject> effectList = new List<GameObject>();

    public GameObject bots;
    public GameObject player;
    GameObject[] listPlayer;

    // Start is called before the first frame update
    void Start()
    {
        GameMgr.inst.gameState = GameMgr.GAME_STATE.INGAME;

        LoadPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SendObstacle());
            StartCoroutine(SendFood());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.PlayerList.Length != listPlayer.Length)
        {
            listPlayer = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject pler in listPlayer)
                pler.GetComponent<PUN2_PlayerSync>().photonGameMng = this;

        }
        if (Input.GetMouseButtonDown(1))
            Debug.Log("Player Number : " + listPlayer.Length);
    }
    void LoadPlayer()
    {
        AudioMgr.inst.PlayMusic();
        inGameUI.SetActive(true);
        endGameUI.SetActive(false);
        UpdateBtnSound();
        GameObject playerObj = PhotonNetwork.Instantiate(dinos[LobbyController.dinoid].name, gameObject.transform.position, Quaternion.identity, 0);
        listPlayer = GameObject.FindGameObjectsWithTag("Player");

        //load all player
        foreach (GameObject pler in listPlayer)
        {
            CreatePlayer(pler, 1, 1, 0, true);
            if (pler == playerObj)//main player
            {
                //CreatePlayer(pler, 1, 1, 0, true);
                pler.GetComponent<PUN2_PlayerSync>().photonGameMng = this;
            }
        }
        NetworkMgr.inst.score = 0;
        EventDispatcher.AddEventListener("EVENT_UPDATE_SCORE", (message) =>
        {
            lbScore.text = NetworkMgr.inst.score + "";
            lbNewScore.text = NetworkMgr.inst.score + "";
        });

        EventDispatcher.AddEventListener("EVENT_SHOW_EFFECT", (message) =>
        {
            GameObject playerEff = message as GameObject;
            GameObject eff = Instantiate(pfEffects[NetworkMgr.inst.effectType]);
            eff.transform.position = NetworkMgr.inst.effectPos;
            int idx = effectList.Count;
            effectList.Add(eff);
            Debug.Log("CallEvent id " + NetworkMgr.inst.effectType +"  "+ (pfEffects[NetworkMgr.inst.effectType].name));
            if (NetworkMgr.inst.effectType == 4 || NetworkMgr.inst.effectType == 5)
            {
                eff.transform.parent = playerEff.transform;
                StartCoroutine(RemoveEff(5.0f, idx));
            }
            else
            {
                StartCoroutine(RemoveEff(0.5f, idx));
            }

        });
    }
    IEnumerator RemoveEff(float seconds, int idx)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(effectList[idx]);
    }
    //--test local
    

    public GameObject CreatePlayer(GameObject playerObj, byte pid, byte type, int cell, bool isMain = false)
    {
        playerObj.transform.parent = transform;
        Debug.Log("Create " + playerObj.name);

        //--collider
        if (type == 2 || type == 4)
        {
            playerObj.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.1f, 0);
            playerObj.GetComponent<CapsuleCollider>().radius = 0.2f;
            playerObj.GetComponent<CapsuleCollider>().height = 0.2f;
        }
        else if (type == 5)
        {
            playerObj.GetComponent<CapsuleCollider>().center = new Vector3(0, 1.0f, 0);
            playerObj.GetComponent<CapsuleCollider>().radius = 0.5f;
            playerObj.GetComponent<CapsuleCollider>().height = 2;
        }
        else
        {
            playerObj.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.4f, 0);
            playerObj.GetComponent<CapsuleCollider>().radius = 0.4f;
        }
        playerObj.GetComponent<CapsuleCollider>().material = physicMat;
        Vector3 pos = hexagons.transform.GetChild(cell).transform.position;
        pos.y = 0.5f;
        playerObj.transform.position = pos;
        playerObj.GetComponent<Rigidbody>().mass = GameMgr.inst.arrDino[type].strength;
        playerObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        if (isMain)
        {
            //--rigidbody

            playerObj.tag = "Player";
            player = playerObj;
            playerObj.AddComponent<PlayerMulti>();
            playerObj.GetComponent<PlayerMulti>().dinoid = type;
            ringTarget.transform.parent = playerObj.transform;
            ringTarget.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            mainCam.GetComponent<CameraMulti>().SetTarget(playerObj.transform);
            Vector3 effPos = player.transform.position;
            effPos.y = respawnEff.transform.position.y;
            respawnEff.transform.position = effPos;
            respawnEff.SetActive(true);
            StartCoroutine(HideRespawnEffect(0.5f));
        }
        else
        {
            playerObj.AddComponent<OpponentMulti>().pid = pid;
            playerObj.GetComponent<OpponentMulti>().dinoid = type;
            playerObj.transform.parent = opponents.transform;
            playerObj.tag = "Opponent";
        }
        return playerObj;
    }
    public void CreateFood(int type, int cell, int count)
    {
        GameObject food = Instantiate(pfFood[type]);
        food.AddComponent<FoodMulti>().type = type;
        food.GetComponent<FoodMulti>().cell = cell;
        food.GetComponent<FoodMulti>().idx = count;
        food.tag = "Food";
        //--opponent
        food.AddComponent<BoxCollider>();
        Vector3 pos = hexagons.transform.GetChild(cell).transform.position;
        pos.y = -0.1241631f;
        if (type == 1 || type == 2)
        {
            food.GetComponent<BoxCollider>().center = new Vector3(0, 0.2f, 0);
            food.GetComponent<BoxCollider>().size = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if (type == 3)//bomb
        {
            food.GetComponent<BoxCollider>().center = new Vector3(0.0f, 0.5f, 0.0f);
            food.GetComponent<BoxCollider>().size = new Vector3(0.8f, 0.8f, 0.8f);
            pos.y = -0.179163f;
        }
        else if (type == 6)//chest
        {
            food.GetComponent<BoxCollider>().center = new Vector3(0.0f, 0.25f, 0.0f);
            food.GetComponent<BoxCollider>().size = new Vector3(1, 0.5f, 0.5f);
        }
        food.transform.position = pos;
        food.transform.parent = foods.transform;
    }
    IEnumerator HideRespawnEffect(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ringTarget.SetActive(true);
        respawnEff.SetActive(false);
    }

    public void ReceiveObstacle(byte[] listObstacle)
    {
        UpdateObstacle(listObstacle);
    }
    private IEnumerator SendObstacle()
    {
        while (true)
        {
            if (listPlayer != null && listPlayer.Length > 0)
            {
                byte[] obsIDs = GenerateObstacleIDs();
                foreach (GameObject go in listPlayer)
                    go.GetComponent<PUN2_PlayerSync>().sendMasterObstacle(obsIDs);
            }
            GameMgr.inst.NUMBER_OBSTACLE += GameMgr.inst.NUMBER_OBSTACLE < 40 ? 2 : 0;
            yield return new WaitForSeconds(4f);
        }
    }
    private byte[] GenerateObstacleIDs()
    {
        List<byte> bytesList = new List<byte>();
        for (byte i = 7; i <= 102; i++)
        {
            bytesList.Add(i);
        }
        
        for (int i = 0; i < bytesList.Count; i++)
        {
            byte temp = bytesList[i];
            int randomIndex = UnityEngine.Random.Range(i, bytesList.Count);
            bytesList[i] = bytesList[randomIndex];
            bytesList[randomIndex] = temp;
        }

        byte[] result = new byte[GameMgr.inst.NUMBER_OBSTACLE];
        for (int i = 0; i < GameMgr.inst.NUMBER_OBSTACLE; i++)
        {
            result[i] = bytesList[i];
        }
        return result;
    }
    public void UpdateObstacle(byte[] listObstacle = null)
    {
        int childcount = hexagons.transform.childCount;

        for (int i = 0; i < childcount; i++)
            if (hexagons.transform.GetChild(i).GetComponent<HexagonController>().IsHidden())
                hexagons.transform.GetChild(i).GetComponent<HexagonController>().SetActive();

        for (int i = 0; i < listObstacle.Length; i++)
        {
            if (!hexagons.transform.GetChild(listObstacle[i]).GetComponent<HexagonController>().IsHidden())
            {
                hexagons.transform.GetChild(listObstacle[i]).GetComponent<HexagonController>().PlayEffect();
            }
        }
    }

    //Update Food Region
    public void ReceiveFood(byte[] listFoods)
    {
        UpdateFood(listFoods);
    }
    private IEnumerator SendFood()
    {
        while (true)
        {
            if (listPlayer != null && listPlayer.Length > 0)
            {
                byte[] foodsIDs = GenerateFoodIDs();
                foreach (GameObject go in listPlayer)
                    go.GetComponent<PUN2_PlayerSync>().sendMasterFood(foodsIDs);
            }
            yield return new WaitForSeconds(4f);
        }
    }
    private byte[] GenerateFoodIDs()
    {
        byte[] foodsList = new byte[GameMgr.inst.NUMBER_FOOD * 2];
        int hexacount = hexagons.transform.childCount;
        List<byte> listRemainHexa = new List<byte>();

        for (byte i = 0; i < hexacount; i++)
            listRemainHexa.Add(i);
            for (int i = 0; i < GameMgr.inst.NUMBER_FOOD * 2; i+= 2)
        {
            foodsList[i] = (byte)UnityEngine.Random.Range(0, pfFood.Length);

            byte hexaFoodIndex = (byte)UnityEngine.Random.Range(0, listRemainHexa.Count);
            foodsList[i + 1] = listRemainHexa[hexaFoodIndex];
            listRemainHexa.RemoveAt(hexaFoodIndex);
        }
        return foodsList;
    }
    public void UpdateFood(byte[] listFood = null)
    {
        foreach (Transform child in foods.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0, count = 0; i < listFood.Length; i+= 2)
        {
            int val = listFood[i];
            if (val > 0)
            {
                CreateFood(val, listFood[i + 1], count);
                count++;
            }
        }
    }
    
    public void GetFood(int[] foodInfo)
    {
        Debug.Log("Someone Get : " + foodInfo[0] +" " + foodInfo[1] + "  " + foodInfo[2]);
        UpdateFood(foodInfo[0], foodInfo[1], foodInfo[2]);
    }
    public void UpdateFood(int foodIdx, int cellIdx, int playerId)
    {
        GameObject food = null;
        foreach (Transform child in foods.transform)
            if (child == foods.transform.GetChild(foodIdx))
        {
                food = child.gameObject;
                break;
        }

        if (food != null)
        {
            foreach (GameObject player in listPlayer)
                if (player.GetComponent<PhotonView>().ViewID == playerId)
                {
                    int type = food.GetComponent<FoodMulti>().type;
                    NetworkMgr.inst.effectType = type;
                    NetworkMgr.inst.effectPos = food.transform.position;
                    if (type == 1 || type == 4 || type == 5)
                        EventDispatcher.DispatchEvent("EVENT_SHOW_EFFECT", player);
                    break;
                }
            Destroy(food);

        }
    }

    public void setInactiveFood(int index)
    {
        Debug.Log("Received eat-food: " + index);
        foods.transform.GetChild(index).gameObject.SetActive(false);
    }

    public void setCountDown(int cd)
    {
        countDownNumber.text = cd + "";
    }
    public void UpdateOtherPlayers(int pid, Vector3 pos, Quaternion quaternion, Vector3 scale, int strength, int speed)
    {
        for (int i = 0; i < opponents.transform.childCount; i++)
        {
            if (opponents.transform.GetChild(i).GetComponent<OpponentMulti>().pid == pid)
            {
                OpponentMulti playerOther = opponents.transform.GetChild(i).GetComponent<OpponentMulti>();
                if (playerOther.listTarget.Count > 2)
                {
                    playerOther.listTarget.Clear();
                }
                playerOther.listTarget.Add(pos);
                playerOther.transform.rotation = quaternion;
                playerOther.transform.localScale = scale;
                playerOther.strength = strength;
                playerOther.speed = speed;
                break;
            }
        }
    }
    public void SetFallen(int pid)
    {
        for (int i = 0; i < opponents.transform.childCount; i++)
        {
            if (opponents.transform.GetChild(i).GetComponent<OpponentMulti>().pid == pid)
            {
                opponents.transform.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
    }
    public void SetRespawn(int pid, int cellid)
    {
        Vector3 pos = hexagons.transform.GetChild(cellid).transform.position;
        pos.y = 0.5f;
        if (pid == NetworkMgr.inst.pid)
        {
            player.GetComponent<PlayerMulti>().Respawn(pos);
            Vector3 effPos = player.transform.position;
            effPos.y = respawnEff.transform.position.y;
            respawnEff.transform.position = effPos;
            respawnEff.SetActive(true);
            StartCoroutine(HideRespawnEffect(0.5f));
        }
        else
        {
            for (int i = 0; i < opponents.transform.childCount; i++)
            {
                if (opponents.transform.GetChild(i).GetComponent<OpponentMulti>().pid == pid)
                {
                    opponents.transform.GetChild(i).GetComponent<OpponentMulti>().Respawn(pos);
                    break;
                }
            }
        }
    }

    //--UI
    public void OnSound()
    {
        AudioMgr.inst.PlayButton();
        AudioMgr.inst.isSoundOn = !AudioMgr.inst.isSoundOn;
        UpdateBtnSound();
    }
    public void UpdateBtnSound()
    {
        AudioMgr.inst.PlayButton();
        if (AudioMgr.inst.isSoundOn)
        {
            AudioMgr.inst.SetSoundOn(true);
            this.btnSound.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            this.btnSound.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            AudioMgr.inst.SetSoundOn(false);
            this.btnSound.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            this.btnSound.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    public void OnHome()
    {
        EventDispatcher.RemoveAllListener();
        AudioMgr.inst.PlayButton();
        SceneManager.LoadScene("Lobby");
    }
}
