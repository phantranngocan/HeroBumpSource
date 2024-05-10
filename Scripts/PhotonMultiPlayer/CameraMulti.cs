using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMulti : MonoBehaviour
{
    [SerializeField] Transform target = null;
    [SerializeField] float smoothness = 0.3f;
    private Vector3 velocity = Vector3.zero;
    public Transform cam1st;
    public Transform cam2nd;
    private Vector3 offset = new Vector3(0, 2.0f, -2.2f);

    void Start()
    {
        transform.position = cam1st.position;
        transform.rotation = cam1st.rotation;
    }
    public void SetTarget(Transform _target)
    {
        target = _target;
    }
    public void SetStartGame(Transform _target)
    {
        target = _target;
    }
    public void SetEndGame(Transform _target)
    {
        target = _target;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position =  Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothness);
        }
    }
}
