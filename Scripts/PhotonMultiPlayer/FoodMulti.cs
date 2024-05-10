using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMulti : MonoBehaviour
{
    public int type = 0;
    public int idx = 0;//index on foods
    public int cell = 0; //food put above this cell
    float rotatingSpeed = 30f;
    Vector3 rotatingAxis = new Vector3(0,1,0);
    void Update()
    {
        if(type==4)//banana
            transform.Rotate(rotatingAxis, rotatingSpeed * Time.deltaTime);
    }
    public void PlayEffect()
    {
        Destroy(gameObject);
    }
}
