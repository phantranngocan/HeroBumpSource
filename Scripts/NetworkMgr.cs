using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Jobs;

public class NetworkMgr : MonoBehaviour
{
    public int pid = 0;
    public int score = 0;
    public int bestScore = 0;
    public int numOfCollect = 0;
    public int dinoid = 0;
    public List<PlayerInfo> players = new List<PlayerInfo>();
    public List<Vector3> foodsPosition = new List<Vector3>();
    public int effectType = 0;
    public Vector3 effectPos = Vector3.zero;

    private static NetworkMgr _inst;
    public static NetworkMgr inst
    {
        get
        {
            if (_inst == null)
                _inst = FindObjectOfType<NetworkMgr>();
            return _inst;
        }
    }
    private void Awake()
    {
        if (_inst == null)
            _inst = this;
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    async void Start()
    {
        _inst = FindObjectOfType<NetworkMgr>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OpenSingleMainScene()
    {
        EventDispatcher.RemoveAllListener();
        UnityEngine.SceneManagement.SceneManager.LoadScene("singleplayer");
    }
    public void OpenMultiMainScene()
    {
        DontDestroyOnLoad(this.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene("multiplayer");
    }
}
