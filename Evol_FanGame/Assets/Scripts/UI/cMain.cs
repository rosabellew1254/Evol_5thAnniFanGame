using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cMain : MonoBehaviour
{
    GameManager gm;
    AudioManager am;
    PlayerData data;

    public Button bNewGame;
    public Button bContinue;
    public Button bOptions;
    public TMP_Text tVersion;
    public TMP_Text tLoading;
    public GameObject buttonPanel;

    [Space]
    [Header("Options")]
    public GameObject optionsWnd;
    public GameObject creditWnd;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button bCredit;
    public Button bOptionsBack;
    public Button bCreditBack;
    public Button bTutorial;


    void Start()
    {
        gm = GameManager.gm;
        am = FindObjectOfType<AudioManager>();
        data = FindObjectOfType<PlayerData>();
        bNewGame.onClick.AddListener(OnNewGameClick);
        bContinue.onClick.AddListener(OnContinueClick);
        bOptions.onClick.AddListener(OnOptionsClick);

        bCredit.onClick.AddListener(OnCreditClick);
        bOptionsBack.onClick.AddListener(OnOptionsBack);
        bCreditBack.onClick.AddListener(OnCreditBack);
        bTutorial.onClick.AddListener(OnTutorialClick);
        tVersion.text = "版本号：v" + Application.version;
        
    }

    void InitOptions()
    {
        musicSlider.value = SetSliderValues(musicSlider);
        sfxSlider.value = SetSliderValues(sfxSlider);
    }

    float SetSliderValues(Slider _slider)
    {
        return _slider == musicSlider ? am.musVol : am.sfxVol;

    }


    void StartNewGame()
    {
        StartCoroutine(NewGame());
    }

    void OnNewGameClick()
    {
        if (!data.progData.isNewGame)
        {
            if (gm.progress != 11)
            {
                Instantiate(gm.pMessageWnd, transform).GetComponent<wConfirm>().InitUI("新游戏", "将会抹去旧数据。确认开启新游戏？", "StartNewGame", gameObject);
            }
            else
            {
                StartNewGame();

            }
        }
        else
        {
            StartNewGame();
        }
    }

    IEnumerator NewGame()
    {
        gm.ResetData();
        bNewGame.gameObject.SetActive(false);
        bContinue.gameObject.SetActive(false);
        bOptions.gameObject.SetActive(false);
        tLoading.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        gm.LoadScene(1); // load inGame scene

    }

    void OnContinueClick()
    {
        gm.LoadScene(1); // load inGame scene

    }

    void OnOptionsClick()
    {
        // open options menu
        optionsWnd.SetActive(true);
        InitOptions();
    }

    void OnCreditClick()
    {
        creditWnd.SetActive(true);
    }

    void OnOptionsBack()
    {
        optionsWnd.SetActive(false);
    }

    void OnCreditBack()
    {
        creditWnd.SetActive(false);
    }

    public void OnMusicVolChange()
    {
        am.ChangeMusicVolume(musicSlider.value);
        am.musVol = musicSlider.value;
        FindObjectOfType<PlayerData>().volData.musicVol = musicSlider.value;
        FindObjectOfType<PlayerData>().SavePrefData();
    }

    public void OnSFXVolChange()
    {
        am.ChangeSFXVolume(sfxSlider.value);
        am.sfxVol = sfxSlider.value;
        FindObjectOfType<PlayerData>().volData.SFXVol = sfxSlider.value;
        FindObjectOfType<PlayerData>().SavePrefData();
    }

    void OnTutorialClick()
    {
        Instantiate(gm.pTutorial, transform);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.O) && Input.GetKeyDown(KeyCode.X))
        {
            gm.LoadScene(2);
        }
    }

}
