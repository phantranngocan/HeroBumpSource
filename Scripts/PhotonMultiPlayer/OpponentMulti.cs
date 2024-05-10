using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentMulti : MonoBehaviour
{
    public int pid;
    public int speed = 0;//speed append
    public int strength = 0;
    public int dinoid = 0;
    public int avatarId;
    public Vector3 target = Vector3.zero;
    public List<Vector3> listTarget = new List<Vector3>();
    private Animator animator;
    private string currentAnim = "";
    void Start()
    {
        animator = GetComponent < Animator>();
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
    public void EatItem(int item)
    {
        switch (item)
        {
            case 1://strength
                break;
            case 2://speed
                break;
            case 3://slow
                break;
        }
    }
    public void Run(Vector3 offset)
    {
        Vector3 direction = Vector3.ClampMagnitude(offset, 1.0f);
        transform.rotation = Quaternion.LookRotation(direction);
        float moveSpeed = GameMgr.inst.GetSpeed(speed, dinoid);
        transform.Translate(direction * moveSpeed * 0.5f, Space.World);
    }

    public void Respawn(Vector3 pos)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = pos;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (listTarget.Count > 0)
        {
            while (listTarget.Count > 0 && Vector3.Distance(listTarget[0], new Vector3(transform.position.x, listTarget[0].y, transform.position.z)) <= 0.05f)
            {
                listTarget.RemoveAt(0);
            }
            if (listTarget.Count > 0)
            {
                float moveSpeed = GameMgr.inst.GetSpeed(speed, dinoid);
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(listTarget[0].x, transform.position.y, listTarget[0].z), moveSpeed * Time.deltaTime);
            }
                
            SetAnimation("run");
        } else
        {
            SetAnimation("idle");
        }
    }
}
