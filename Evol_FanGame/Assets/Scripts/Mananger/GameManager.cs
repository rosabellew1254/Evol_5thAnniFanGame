using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    [HideInInspector] public AudioManager am;
    PlayerData data;

    [NamedArray(typeof(eChar))] public int[] stamina;

    [NamedArray(typeof(eChar))] public ePosition[] positions; // indicating who is doing what

    [NamedArray(typeof(eIngredient))] public int[] ingredientAmt;

    [Space]
    [Header("Game Data")]
    public int money;
    public int progress;
    public bool isNewGame;
    public int[] moneyGoals;
    public bool haveReadTutorial;

    [Space]
    [Header("Prefabs")]
    public GameObject pInitWnd;
    public GameObject pSummaryWnd;
    [NamedArray(typeof(eChar))] public GameObject[] pWaiters;
    public GameObject pMessageWnd;
    public GameObject pTutorial;


    [Space]
    [Header("Original Data")]

    public int originalLevel;
    public int originalIngredient;
    public int originalMoney;
    public int originalStamina;

    [Space]
    [Header("Sprites")]
    public Sprite[] cakeLayers;

    public bool doneInit;

    private void Awake()
    {
        if (gm == null)
        {
            DontDestroyOnLoad(gameObject);
            gm = this;
        }
        else if (gm != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        data = FindObjectOfType<PlayerData>();
        am = AudioManager.am;
        LoadData();
    }

    public void LoadData()
    {
        StartCoroutine(InitData());

    }

    public void ResetData()
    {
        for (int i = 0; i < stamina.Length; i++)
        {
            stamina[i] = originalStamina;
            ingredientAmt[i] = originalIngredient;
        }
        progress = originalLevel;
        money = originalMoney;
        isNewGame = true;
        WriteData();
    }

    void WriteData()
    {
        data.progData.levelNum = progress;
        data.progData.isNewGame = isNewGame;
        data.charData.stamina = stamina;
        data.inventoryData.ingredients = ingredientAmt;
        data.inventoryData.money = money;
        FindObjectOfType<PlayerData>().SaveChar();
        FindObjectOfType<PlayerData>().SaveInventory();
        FindObjectOfType<PlayerData>().SaveProgress();
    }

    IEnumerator InitData()
    {
        // display loading screen
        //data = FindObjectOfType<PlayerData>();
        yield return new WaitUntil(() => data != null);
        doneInit = false;

        stamina = data.charData.stamina;
        progress = JsonUtility.FromJson<PlayerProgress>(System.IO.File.ReadAllText(Application.persistentDataPath + data.progDataFileName)).levelNum;
        isNewGame = JsonUtility.FromJson<PlayerProgress>(System.IO.File.ReadAllText(Application.persistentDataPath + data.progDataFileName)).isNewGame;
        haveReadTutorial = FindObjectOfType<PlayerData>().progData.haveReadTutorial;
        am.musVol = data.volData.musicVol;
        am.sfxVol = data.volData.SFXVol;
        am.ChangeMusicVolume(am.musVol);
        am.ChangeSFXVolume(am.sfxVol);
        //Debug.Log("progress: " + progress + ", data progress: " + data.progData.levelNum);
        ingredientAmt = data.inventoryData.ingredients;
        money = data.inventoryData.money;
        doneInit = true;
        // hide loading screen

        // play heartbeat music
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            am.StartMusic(eMusic.heartbeat);
        }

    }

    public void LoadScene(int _sceneIdx)
    {
        SceneManager.LoadScene(_sceneIdx);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 0:
                StartCoroutine(InitButton());
                if (am != null)
                {
                    am.StartMusic(eMusic.heartbeat);
                }
                break;
            case 1:
                am.StartMusic(eMusic.cafe);
                
                break;
            case 2:
                am.StartMusic(eMusic.heartbeat);
                break;
            default:
                break;
        }

    }

    IEnumerator InitButton()
    {
        data = FindObjectOfType<PlayerData>();
        data.LoadData(); 
        yield return new WaitUntil(() => doneInit);
        Debug.Log("is new game: " + data.progData.isNewGame);
        Debug.Log("is new game find data: " + FindObjectOfType<PlayerData>().progData.isNewGame);
        Debug.Log("is new game gm: " + isNewGame);
        if (!data.progData.isNewGame)
        {
            if (progress != 11)
            {
                FindObjectOfType<cMain>().buttonPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(580, 450);
                FindObjectOfType<cMain>().bContinue.gameObject.SetActive(true);
            }
        }
    }
    public int CakeLayer()
    {
        if (gm.money < gm.moneyGoals[0])
        {
            return 0;
        }
        else if (gm.money >= gm.moneyGoals[0] && gm.money < gm.moneyGoals[1])
        {
            return 1;
        }
        else if (gm.money >= gm.moneyGoals[1] && gm.money < gm.moneyGoals[2])
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }
}
