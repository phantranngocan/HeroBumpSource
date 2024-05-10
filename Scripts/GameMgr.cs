using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameMgr : MonoBehaviour
{
    private static GameMgr _inst;
    public float OBSTACLE_FORCE = 3000;
    public float PLAYER_FORCE = 100;
    public int NUMBER_OBSTACLE = 15;
    public int NUMBER_FOOD = 10;

    public enum GAME_STATE
    {
        START = 0,
        INGAME,
        GAMEOVER,
    }
    public static GameMgr inst
    {
        get
        {
            if (_inst == null)
                _inst = new GameMgr();
            return _inst;
        }
    }
    private void Awake()
    {
        if (_inst == null)
            _inst = this;
        DontDestroyOnLoad(gameObject);
    }
    public GAME_STATE gameState;
    void Start()
    {
        this.gameState = GAME_STATE.START;
        GenerateDinoInfo();
        GenerateFoodInfo();
        LoadPrefs();
        GetComponent<LobbyController>().OnNextDino();
    }
    public double GetDistance(Vector3 p1, Vector3 p2)
    {
        return Math.Sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y));
    }
    public List<DinoInfo> arrDino;
    public void GenerateDinoInfo()
    {
        Debug.Log("Gen Dino Info");
        arrDino = new List<DinoInfo>();
        arrDino.Add(new DinoInfo(0, "Dino", "Dinosaurs are a diverse group of reptiles[note 1] of the clade Dinosauria. They first appeared during the Triassic period, between 243 and 233.23 million years ago (mya), although the exact origin and timing of the evolution of dinosaurs is a subject of active research.", 4, 6));
        arrDino.Add(new DinoInfo(1, "Shark", "Sharks are a group of elasmobranch fish characterized by a cartilaginous skeleton, five to seven gill slits on the sides of the head, and pectoral fins that are not fused to the head. Modern sharks are classified within the clade Selachimorpha (or Selachii) and are the sister group to the Batoidea (rays and kin).", 4, 6));
        arrDino.Add(new DinoInfo(2, "Rock", "In geology, rock (or stone) is any naturally occurring solid mass or aggregate of minerals or mineraloid matter. It is categorized by the minerals included, its chemical composition, and the way in which it is formed.", 3, 7));
        arrDino.Add(new DinoInfo(3, "Penguin", "Penguins are a group of aquatic flightless birds from the family Spheniscidae of the order Sphenisciformes. They live almost exclusively in the Southern Hemisphere: only one species, the Gal?pagos penguin, is found north of the Equator.", 7, 3));
        arrDino.Add(new DinoInfo(4, "Ice Monster", "A monster is a type of fictional creature found in horror, fantasy, science fiction, folklore, mythology and religion. Monsters are very often depicted as dangerous and aggressive, with a strange or grotesque appearance that causes terror and fear, often in humans.", 3, 7));
        arrDino.Add(new DinoInfo(5, "Cats", "The cat (Felis catus), commonly referred to as the domestic cat or house cat, is the only domesticated species in the family Felidae. Recent advances in archaeology and genetics have shown that the domestication of the cat occurred in the Near East around 7500 BC.", 7, 3));
        arrDino.Add(new DinoInfo(6, "Human", "Humans (Homo sapiens) or modern humans are the most common and widespread species of primate, and the last surviving species of the genus Homo. They are great apes characterized by their hairlessness, bipedalism, and high intelligence.", 5, 5));
    }
    public List<FoodInfo> arrFood;
    public void GenerateFoodInfo()
    {
        arrFood = new List<FoodInfo>();
        arrFood.Add(new FoodInfo(0, "MagicUp", "", 2, 5));//not use
        arrFood.Add(new FoodInfo(1, "MagicUp", "", 2, 5));
        arrFood.Add(new FoodInfo(2, "Repel", "", 2, 5));
        arrFood.Add(new FoodInfo(3, "MagicHit", "", 0, 3));
        arrFood.Add(new FoodInfo(4, "StunFire", "", 0, 5));
        arrFood.Add(new FoodInfo(5, "StunNormal", "", 0, 5));
        arrFood.Add(new FoodInfo(6, "MagicalCast", "", 0, 0));
        
    }

    //--game settings
    public int firstPlay = 0;
    public void SavePrefs()
    {
        PlayerPrefs.SetInt("firstPlay", 1);
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        firstPlay = PlayerPrefs.GetInt("firstPlay", 0);
        
    }
    public float GetSpeed(float speed, int dinoid)
    {
        //4 <-> 1
        //speed ->?
        float baseSpeed = arrDino[dinoid].speed;
        return (speed + baseSpeed) / 4;
    }
    public float GetStrength(float startSize, float strength, int dinoid)
    {
        float baseStrength = arrDino[dinoid].strength;
        float newStrength = baseStrength + strength;
        //base <---> startSize
        //new  <--->  ?
        return (newStrength * startSize) / baseStrength;
    }
}

public class FoodInfo
{
    public int id;
    public string name;
    public string decs;
    public int val;
    public int countdown;
    public FoodInfo(int _id,string _name, string _decs, int _val, int _countdown)
    {
        id = _id;
        name = _name;
        decs = _decs;
        val = _val;
        countdown = _countdown;
    }
}


public class DinoInfo
{
    public int id;
    public string name;
    public string decs;
    public int speed;
    public int strength;
    public DinoInfo(int _id,string _name, string _decs, int _speed, int _strength)
    {
        id = _id;
        name = _name;
        decs = _decs;
        speed = _speed;
        strength = _strength;
    }
}

public class PlayerInfo
{
    public int pid;
    public string name;
    public int avatarid;
    public int dinoid;
    public int score;
    public int skilledcount;
    public int cellid;//start pos
    public PlayerInfo(int _pid, int _avatarid,int _dinoid) {
        pid = _pid;
        avatarid = _avatarid;
        dinoid = _dinoid;
    }
}