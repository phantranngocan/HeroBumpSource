using UnityEngine;
using Photon.Pun;
using System;

public class PUN2_PlayerSync : MonoBehaviourPun, IPunObservable
{
    public event Action<byte[]> OnReceiveObstacleEvent;
    public event Action<byte[]> OnReceiveFoodEvent;
    public event Action<int[]> OnGetFoodEvent;
    public PhotonGameManager photonGameMng;

    //List of the scripts that should only be active for the local player (ex. PlayerController, MouseLook etc.)
    public MonoBehaviour[] localScripts;
    //List of the GameObjects that should only be active for the local player (ex. Camera, AudioListener etc.)
    public GameObject[] localObjects;
    //Values that will be synced over network
    Vector3 latestPos;
    Quaternion latestRot;

    // Use this for initialization
    void Start()
    {
        if (photonView.IsMine)
        {
            //Player is local
        }
        else
        {
            //Player is Remote, deactivate the scripts and object that should only be enabled for the local player
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObjects.Length; i++)
            {
                localObjects[i].SetActive(false);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            if ((transform.position - latestPos).magnitude > 10)
                transform.position = latestPos;
            else
                transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            if (latestRot.eulerAngles != Vector3.zero)
                transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
                GetComponent<Rigidbody>().AddForce(new Vector3(-1888f, 0, -207));
        }
    }

    [PunRPC]
    void AddForce(Vector3 force, int playerID)
    {
        Debug.Log("AddForce  (" + force + ")  to  " + playerID + " and my ID " + GetComponent<PhotonView>().ViewID);

        if (playerID == GetComponent<PhotonView>().ViewID)
            GetComponent<Rigidbody>().AddForce(force);
        else
            Debug.Log("Send RPC but not match");

    }
    public void SendForce(Vector3 force, int id)
    {
        photonView.RPC("AddForce", RpcTarget.Others, force, id);
    }


    [PunRPC]
    private void ReceiveItemIDs(byte[] itemIDs)
    {
        //Debug.Log("PUN2 Received obstaclee list: " + BitConverter.ToString(itemIDs));
        if (GetComponent<PhotonView>().IsMine)
        {
            //OnReceiveObstacleEvent(itemIDs);
            photonGameMng.ReceiveObstacle(itemIDs);
        }
    }
    public void sendMasterObstacle(byte[] itemIDs)
    {
        photonView.RPC("ReceiveItemIDs", RpcTarget.All, itemIDs);
    }


    [PunRPC]
    private void ReceiveFoodIDs(byte[] listfoods)
    {
        //Debug.Log(gameObject.name + " Received food IDs: " + BitConverter.ToString(listfoods));
        if (GetComponent<PhotonView>().IsMine)
        {
            photonGameMng.ReceiveFood(listfoods);
        }
    }
    public void sendMasterFood(byte[] itemIDs)
    {
        photonView.RPC("ReceiveFoodIDs", RpcTarget.All, itemIDs);
    }


    [PunRPC]
    private void EatFoodID(int[] foodInfo)
    {
        Debug.Log(gameObject.name + " Received Eat Food: " + foodInfo[0] + " ");
        photonGameMng.GetFood(foodInfo);
    }
    public void sendEatFood(int[] foodInfo)
    {
        photonView.RPC("EatFoodID", RpcTarget.All, foodInfo);
    }
    public void OnPlayerCollisionFood(GameObject foodObj)
    {
        EventDispatcher.DispatchEvent("EVENT_SHOW_EFFECT");
    }
}