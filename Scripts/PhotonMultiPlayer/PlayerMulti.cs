using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMulti : MonoBehaviour
{
    private Vector3 pointA;
    private Vector3 pointB;
    private bool touchStart = false;
    private Animator animator;
    private string currentAnim = "";

    //--
    public int pid;
    public int speed = 0; //speed append
    public int strength = 0;  //strength append
    public int dinoid = 0;
    private float startSize;


    const float updateInterval = 1.0f;//seconds
    private double lastInterval;
    //--countdonw item
    

    
    int item1cd = 0;
    private List<GameObject> item1list = new List<GameObject>();
    
    private List<GameObject> item2list = new List<GameObject>();

    int item3cd = 0;
    private List<GameObject> item3list = new List<GameObject>();

    int item4cd = 0;
    private List<GameObject> item4list = new List<GameObject>();

    int item5cd = 0;
    private List<GameObject> item5list = new List<GameObject>();

    private List<GameObject> food6 = new List<GameObject>();
    const float baseForce = 500;
    public void Start()
    {
        animator = GetComponent<Animator>();
        startSize = transform.localScale.x;
        SetAnimation("idle");
    }
    public void SetAnimation(string str)
    {
        if (currentAnim.CompareTo(str) == 0)
        {
            return;
        }
        currentAnim = str;
        switch (dinoid)
        {
            case 0:
            case 1://dino
                if (str == "idle")
                {
                    animator.SetInteger("animation", 1);
                }
                else if (str == "run")
                {
                    animator.SetInteger("animation", 18);
                }
                else if (str == "attack")
                {
                    animator.SetInteger("animation", 6);
                }
                else if (str == "die")
                {
                    animator.SetInteger("animation", 8);
                }
                break;
            case 2: //desert monster: mega_golem_A
            case 3: //ice monster: brother_penguin_A, Mega_ice_Golem
            case 4:
                if (str == "idle")
                {
                    animator.SetInteger("animation", 1);
                }
                else if (str == "run")
                {
                    animator.SetInteger("animation", 2);
                }
                else if (str == "attack")
                {
                    animator.SetInteger("animation", 3);
                }
                else if (str == "die")
                {
                    animator.SetInteger("animation", 8);
                }
                break;
            case 5: //cat
                if (str == "idle")
                {
                    animator.SetInteger("animation", 1);
                }
                else if (str == "run")
                {
                    animator.SetInteger("animation", 18);
                }
                else if (str == "attack")
                {
                    animator.SetInteger("animation", 41);
                }
                else if (str == "die")
                {
                    animator.SetInteger("animation", 6);
                }
                break;
            case 6: //human
                if (str == "idle")
                {
                    animator.SetInteger("animation", 13);
                }
                else if (str == "run")
                {
                    animator.SetInteger("animation", 20);
                }
                else if (str == "attack")
                {
                    animator.SetInteger("animation", 23);
                }
                else if (str == "die")
                {
                    animator.SetInteger("animation", 11);
                }
                break;
        }
    }
    public void Respawn(Vector3 pos)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = pos;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        item1cd = 0;
        foreach (var item in item1list)
        {
            Destroy(item);
        }
        item1list.Clear();

        item3cd = 0;
        foreach (var item in item3list)
        {
            Destroy(item);
        }
        item3list.Clear();

        item4cd = 0;
        foreach (var item in item4list)
        {
            Destroy(item);
        }
        item4list.Clear();

        item5cd = 0;
        foreach (var item in item5list)
        {
            Destroy(item);
        }
        item5list.Clear();
    }
    void Update()
    {
        CheckInputs();
        RotateAndMove();

        //--update 1s -> item countdown
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            lastInterval = timeNow;
            if (item1cd > 0)
            {
                item1cd--;
                if (item1cd == 0)
                {
                    strength = 0;
                    float newSize = GameMgr.inst.GetStrength(startSize, strength, dinoid);
                    transform.localScale = new Vector3(newSize, newSize, newSize);
                    GetComponent<Rigidbody>().mass = GameMgr.inst.arrDino[dinoid].strength;//revert
                    //--clear item
                    foreach(var item in item1list)
                    {
                        Destroy(item);
                    }
                    item1list.Clear();
                }
            }
            if (item3cd > 0)
            {
                item3cd--;
                if (item3cd == 0)
                {
                    //--send effect to all player
                    NetworkMgr.inst.effectType = 3;
                    NetworkMgr.inst.effectPos = transform.position;
                    EventDispatcher.DispatchEvent("EVENT_SHOW_EFFECT");
                    float f1 = baseForce + GetComponent<Rigidbody>().mass * 200;
                    foreach (var item in item3list)
                    {
                        Destroy(item);
                    }
                    item3list.Clear();
                }
            }
            if (item4cd > 0)
            {
                item4cd--;
                if (item4cd == 0)
                {
                    foreach (var item in item4list)
                    {
                        Destroy(item);
                    }
                    item4list.Clear();
                }
            }
            if (item5cd > 0)
            {
                item5cd--;
                if (item5cd == 0)
                {
                    foreach (var item in item5list)
                    {
                        Destroy(item);
                    }
                    item5list.Clear();
                }
            }
        }
        if (item4cd > 0)
        {
            Vector3 offset = pointB - pointA;
            Vector3 direction = Vector3.ClampMagnitude(offset, 1.0f);
            transform.rotation = Quaternion.LookRotation(direction);
            float newSpeed = GameMgr.inst.GetSpeed(speed, dinoid);
            transform.Translate(direction * newSpeed * 2 * Time.deltaTime, Space.World);
        }
    }
    public void ApplyForce(float f, Vector3 dir) {
        GetComponent<Rigidbody>().AddForce(f * dir);
    }
    private void CheckInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = true;
            pointA = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y);
            SetAnimation("run");
        }

        if (Input.GetMouseButton(0))
        {
            pointB = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y);
        }
        else
        {
            touchStart = false;
            SetAnimation("idle");
        }
    }

    private void RotateAndMove()
    {
        if (touchStart)
        {
            Vector3 offset = pointB - pointA;
            Vector3 direction = Vector3.ClampMagnitude(offset, 1.0f);
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
            float newSpeed = GameMgr.inst.GetSpeed(speed,dinoid);
            if(item5cd>0){
                transform.Translate(direction * newSpeed * Time.deltaTime  * -2, Space.World);
            } else {
                transform.Translate(direction * newSpeed * Time.deltaTime, Space.World);
            }
        }
    }
    //--check collision with opponent
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            Destroy(collision.gameObject.GetComponent<BoxCollider>());
            int type = collision.gameObject.GetComponent<FoodMulti>().type;
            if (type == 0) //not use
            {

            }
            else if (type == 1)//strength
            {
                strength = GameMgr.inst.arrFood[type].val;
                GetComponent<Rigidbody>().mass = GameMgr.inst.arrDino[dinoid].strength + strength;
                float newSize = GameMgr.inst.GetStrength(startSize, strength, dinoid);
                transform.localScale = new Vector3(newSize, newSize, newSize);
                item1cd += GameMgr.inst.arrFood[type].countdown;
                NetworkMgr.inst.effectType = type;
                NetworkMgr.inst.effectPos = collision.transform.position;
                EventDispatcher.DispatchEvent("EVENT_SHOW_EFFECT");
                if (item1list.Count == 0)
                {
                    item1list.Add(collision.gameObject);
                    collision.transform.parent = transform;
                    collision.transform.localScale = collision.transform.localScale * 0.5f;
                    collision.transform.localPosition = new Vector3(0, 1.0f, 0);
                }
                else
                {
                    Destroy(collision.gameObject);
                }
            }
            else if (type == 2)//repel
            {

            }
            else if (type == 3)//magic hit
            {
                item3cd += GameMgr.inst.arrFood[type].countdown;
                if (item3list.Count == 0)
                {
                    item3list.Add(collision.gameObject);
                    collision.transform.parent = transform;
                    collision.transform.localScale = collision.transform.localScale * 0.5f;
                    collision.transform.localPosition = new Vector3(0, 1.0f, 0);
                }
                else
                {
                    Destroy(collision.gameObject);
                }
            }
            else if (type == 4)//magic fire
            {
                item4cd += GameMgr.inst.arrFood[type].countdown;
                NetworkMgr.inst.effectType = type;
                NetworkMgr.inst.effectPos = collision.transform.position;
                EventDispatcher.DispatchEvent("EVENT_SHOW_EFFECT");
                if (item4list.Count == 0)
                {
                    item4list.Add(collision.gameObject);
                    collision.transform.parent = transform;
                    collision.transform.localScale = collision.transform.localScale * 0.5f;
                    collision.transform.localPosition = new Vector3(0, 1.0f, 0);
                }
                else
                {
                    Destroy(collision.gameObject);
                }
            }
            else if (type == 5)//stun normal
            {
                item5cd = GameMgr.inst.arrFood[type].countdown;
                NetworkMgr.inst.effectType = type;
                NetworkMgr.inst.effectPos = collision.transform.position;
                EventDispatcher.DispatchEvent("EVENT_SHOW_EFFECT");
                if (item5list.Count == 0)
                {
                    item5list.Add(collision.gameObject);
                    collision.transform.parent = transform;
                    collision.transform.localScale = collision.transform.localScale * 0.5f;
                    collision.transform.localPosition = new Vector3(0, 1.0f, 0);
                }
                else
                {
                    Destroy(collision.gameObject);
                }
            }
            else if (type == 6)//multi cast
            {

            }
            else
            {
                Destroy(collision.gameObject);
            }
            int cell = collision.gameObject.GetComponent<FoodMulti>().cell;
            int playerId = GetComponent<PhotonView>().ViewID;
            AudioMgr.inst.PlayGotItem();
            GetComponent<PUN2_PlayerSync>().sendEatFood(new int[] { collision.gameObject.GetComponent<FoodMulti>().type, cell, playerId });
            NetworkMgr.inst.score += 100;
            EventDispatcher.DispatchEvent("EVENT_UPDATE_SCORE");
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Vector3 direction = transform.forward.normalized;
            GetComponent<Rigidbody>().AddForce(direction * GameMgr.inst.OBSTACLE_FORCE);
            StartCoroutine(ReleaseForceEff(0.8f));
        }
        else if (collision.gameObject.CompareTag("Opponent") || collision.gameObject.CompareTag("Player"))
        {
            int otherStrength = 7;
            Vector3 localDir = Quaternion.Inverse(transform.rotation) * (collision.transform.position - transform.position);
            float f1 = baseForce + GetComponent<Rigidbody>().mass * 200;
            float f2 = baseForce + otherStrength * 200;
            Vector3 d1 = transform.forward.normalized;
            Vector3 d2 = collision.transform.forward.normalized;
            GetComponent<Rigidbody>().AddForce(d1 * f2 * -1);
            GetComponent<PUN2_PlayerSync>().SendForce(d1 * f2, GetComponent<PhotonView>().ViewID);
            AudioMgr.inst.PlayBump();
        }
        else if (collision.gameObject.CompareTag("wall"))//player die
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPlayerCollisionFood(GameObject foodObj)
    {
        int type = foodObj.GetComponent<FoodMulti>().type;
        if (type == 0) //not use
        {
        }
        else if (type == 1)//strength
        {
            strength = GameMgr.inst.arrFood[type].val;
            GetComponent<Rigidbody>().mass = GameMgr.inst.arrDino[dinoid].strength + strength;
            float newSize = GameMgr.inst.GetStrength(startSize, strength, dinoid);
            transform.localScale = new Vector3(newSize, newSize, newSize);
            item1cd += GameMgr.inst.arrFood[type].countdown;
            NetworkMgr.inst.effectType = type;
            NetworkMgr.inst.effectPos = foodObj.transform.position;
            EventDispatcher.DispatchEvent("EVENT_SHOW_EFFECT");
            if (item1list.Count == 0)
            {
                item1list.Add(foodObj);
                foodObj.transform.parent = transform;
                foodObj.transform.localScale = foodObj.transform.localScale * 0.5f;
                foodObj.transform.localPosition = new Vector3(0, 1.0f, 0);
            }
        }
        else if (type == 2)//repel
        {

        }
        else if (type == 3)//magic hit
        {
            item3cd += GameMgr.inst.arrFood[type].countdown;
            if (item3list.Count == 0)
            {
                item3list.Add(foodObj);
                foodObj.transform.parent = transform;
                foodObj.transform.localScale = foodObj.transform.localScale * 0.5f;
                foodObj.transform.localPosition = new Vector3(0, 1.0f, 0);
            }
        }
        else if (type == 4)//magic fire
        {
            item4cd += GameMgr.inst.arrFood[type].countdown;
            NetworkMgr.inst.effectType = type;
            NetworkMgr.inst.effectPos = foodObj.transform.position;
            EventDispatcher.DispatchEvent("EVENT_SHOW_EFFECT");
            if (item4list.Count == 0)
            {
                item4list.Add(foodObj);
                foodObj.transform.parent = transform;
                foodObj.transform.localScale = foodObj.transform.localScale * 0.5f;
                foodObj.transform.localPosition = new Vector3(0, 1.0f, 0);
            }
        }
        else if (type == 5)//stun normal
        {
            item5cd = GameMgr.inst.arrFood[type].countdown;
            NetworkMgr.inst.effectType = type;
            NetworkMgr.inst.effectPos = foodObj.transform.position;
            EventDispatcher.DispatchEvent("EVENT_SHOW_EFFECT");
            if (item5list.Count == 0)
            {
                item5list.Add(foodObj);
                foodObj.transform.parent = transform;
                foodObj.transform.localScale = foodObj.transform.localScale * 0.5f;
                foodObj.transform.localPosition = new Vector3(0, 1.0f, 0);
            }
        }
        else if (type == 6)//multi cast
        {

        }

        AudioMgr.inst.PlayGotItem();
        NetworkMgr.inst.score += 100;
    }
    IEnumerator ReleaseForceEff(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    IEnumerator ReleaseStrength(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        strength = 0;
    }
}
