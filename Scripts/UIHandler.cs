using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject endMenu;
    [SerializeField] GameObject ingameMenu;
    //--start game
    [SerializeField] Button btnStart;
    [SerializeField] Button btnSkin;

    //--in game
    [SerializeField] Text lbScore;
    [SerializeField] Text lbPlayerCollect;

    //--end game
    [SerializeField] Text lbBestScore;
    [SerializeField] Text lbNewScore;
    [SerializeField] Button btnSound;

    // Start is called before the first frame update
    void Start()
    {
        this.UpdateBtnSound();
        //--init button
        this.UpdateGameState();
        this.UpdateScore();

        EventDispatcher.AddEventListener("EVENT_UPDATE_SCORE", (message) =>
        {
            this.UpdateScore();
        });
        EventDispatcher.AddEventListener("EVENT_UPDATE_GAME_STATE", (message) =>
        {
            this.UpdateScore();
            this.UpdateGameState();
        });

    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        Debug.Log(GameMgr.inst.gameState);
    }

    // Update is called once per frame
    public void UpdateGameState()
    {
        this.startMenu.SetActive(false);
        this.ingameMenu.SetActive(false);
        this.endMenu.SetActive(false);
        switch (GameMgr.inst.gameState)
        {
            case GameMgr.GAME_STATE.START:
                this.startMenu.SetActive(true);
                break;
            case GameMgr.GAME_STATE.INGAME:
                this.ingameMenu.SetActive(true);
                EventDispatcher.DispatchEvent("EVENT_GAME_START", "");
                break;
            case GameMgr.GAME_STATE.GAMEOVER:
                NetworkMgr.inst.score = 0;
                this.endMenu.SetActive(true);
                EventDispatcher.DispatchEvent("EVENT_GAME_END", "");
                break;
        }
    }
    public void OnStart()
    {
        NetworkMgr.inst.score = 0;
        NetworkMgr.inst.numOfCollect = 0;
        AudioMgr.inst.PlayButton();
        GameMgr.inst.gameState = GameMgr.GAME_STATE.INGAME;
        this.UpdateGameState();
        this.UpdateScore();
    }
    public void OnSkin()
    {
        EventDispatcher.RemoveAllListener();
        AudioMgr.inst.PlayButton();
        SceneManager.LoadScene("Dino");
    }
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
    public void UpdateScore()
    {
        this.lbScore.text = NetworkMgr.inst.score + "";
        this.lbNewScore.text = NetworkMgr.inst.score + "";
        this.lbBestScore.text = NetworkMgr.inst.bestScore + "";
        this.lbPlayerCollect.text = NetworkMgr.inst.numOfCollect + "";
    }
    public void OnRestart()
    {
        OnStart();
    }
    public void OnHome()
    {
       EventDispatcher.RemoveAllListener();
        AudioMgr.inst.PlayButton();
        SceneManager.LoadScene("Home");
    }
}
