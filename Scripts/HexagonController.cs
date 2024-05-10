using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshRenderer))]
public class HexagonController : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {

    }
    public void PlayEffect()
    {
        GetComponent<Animator>().Play("ChangeColor");
    }
    public void SetActive()
    {
        GetComponent<Animator>().Play("ScaleZoomIn");
    }
    public bool IsHidden(){
        return transform.lossyScale.x<0.1;
    }
}
