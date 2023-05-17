using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cInGame : MonoBehaviour
{
    public static cInGame canvas;
    PlayerController player;

    [Header("for Gameplay")]
    public GameObject[] waiterTabs;
    public GameObject[] waiterTabsFull;
    public TMP_Text[] tWaiterName;
    public TMP_Text[] tWaiterNameTag;
    public TMP_Text[] tWaiterStatus;
    [NamedArray(typeof(eCarrying))] public string[] stringStatus;
    //public Transform[] waiterTabsInactive;
    //public Transform[] waiterTabsActive;
    [NamedArray(typeof(eChar))] public Sprite[] spriteIDs;
    [NamedArray(typeof(eChar))] public Sprite[] spriteNames;
    [NamedArray(typeof(eChar))] public string[] stringNames;
    public Image[] iWaiterStatusInactive;
    public Image[] iWaiterStatusActive;
    [NamedArray(typeof(eCarrying))] public Sprite[] spriteStatus;
    public Transform initPos;

    public eChar activeChar;


    [Space]
    [Header("HUD Info")]
    public TMP_Text tCountdown;
    public TMP_Text tCurTurn;
    public TMP_Text tCurTurnMoney;

    [Space]
    [Header("Pause Menu")]
    public GameObject pauseWnd;
    public Button bResume;
    public Button bBackToMain;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Space]
    [Header("for prototype use")]
    public Button bComplete;
    public Button bFail;
    public Button bAddCustomer;
    public Button bSettings;
    public TMP_Text customerNum;
    public TMP_Text[] tKitchenStatus;
    public string[] stringKitchenStatus; // 0: no order, 1: cooking, 2: ready

    LevelManager level;
    AudioManager am;

    private void Awake()
    {
        canvas = this;
    }

    void Start()
    {

    }

    public void InitCanvas()
    {
        level = LevelManager.level;
        player = PlayerController.player;
        am = AudioManager.am;
        customerNum.text = 0.ToString();
        bComplete.onClick.AddListener(OnCompleteClick);
        bFail.onClick.AddListener(OnFailClick);
        bAddCustomer.onClick.AddListener(OnAddCustomerClick);
        bSettings.onClick.AddListener(OnSettingsClick);
        bResume.onClick.AddListener(OnResumeClick);
        bBackToMain.onClick.AddListener(OnBackToMainClick);
        waiterTabs[0].GetComponent<Button>().onClick.AddListener(delegate { OnWaiterTabClick(0); });
        waiterTabs[1].GetComponent<Button>().onClick.AddListener(delegate { OnWaiterTabClick(1); });
        StartCoroutine(InitUI());
        InitPauseMenu();
    }

    IEnumerator InitUI()
    {
        yield return new WaitUntil(() => level != null);
        tCurTurnMoney.text = 0.ToString();
        Debug.Log("gm progress: " + GameManager.gm.progress);
        tCurTurn.text = "距离蛋糕节还有" + (10 - GameManager.gm.progress).ToString() + "天";
        // init tab status
        for (int i = 0; i < waiterTabsFull.Length; i++)
        {
            tWaiterStatus[i].text = stringStatus[(int)eCarrying.none];
        }

        UpdateTimeDisplay();
    }

    public void OnWaiterTabClick(int _waiterIdx)
    {
        // check if the waiter is currently active
        // set waiter to be active/inactive according to the current state
        player.activeWaiterIdx = _waiterIdx;
        // update the tab's position according to the current state
        switch (_waiterIdx)
        {
            case 0:
                UpdateTabStatus(true);
                break;
            case 1:
                UpdateTabStatus(false);
                break;
            default:
                break;
        }

        level.waiter[player.activeWaiterIdx].activeIndicator.SetActive(true);

    }

    public void UpdateWaiterTabDisplay(int _charIdx, eCarrying _targetState)
    {
        iWaiterStatusActive[_charIdx].sprite = spriteStatus[(int)_targetState];
        iWaiterStatusInactive[_charIdx].sprite = spriteStatus[(int)_targetState];
        tWaiterStatus[_charIdx].text = stringStatus[(int)_targetState];
    }


    void UpdateTabStatus(bool _waiter0Active)
    {
        // turn on/off the inactive tabs
        waiterTabs[0].SetActive(!_waiter0Active);
        waiterTabs[1].SetActive(_waiter0Active);

        level.waiter[0].activeIndicator.SetActive(_waiter0Active);
        level.waiter[1].activeIndicator.SetActive(!_waiter0Active);

        // turn on/off the active tabs
        waiterTabsFull[0].SetActive(_waiter0Active);
        waiterTabsFull[1].SetActive(!_waiter0Active);
    }


    void InitPauseMenu()
    {
        musicSlider.value = SetSliderValues(musicSlider);
        sfxSlider.value = SetSliderValues(sfxSlider);
    }

    float SetSliderValues(Slider _slider)
    {
        return _slider == musicSlider ? am.musVol : am.sfxVol;
    }


    void OnCompleteClick()
    {
        level.CompleteLevel();
    }

    void OnFailClick()
    {
        level.EndGameHalfWay();
    }

    void OnAddCustomerClick()
    {
        level.ServeOneCustomer();
    }

    void OnSettingsClick()
    {
        // pause game
        Time.timeScale = 0;
        player.enabled = false;

        // open up pause menu
        pauseWnd.SetActive(true);
    }

    void OnResumeClick()
    {
        // resume time
        Time.timeScale = 1;
        player.enabled = true;
        // close pause menu
        pauseWnd.SetActive(false);
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

    void OnBackToMainClick()
    {
        // show confirm window
        GameObject confirmWnd = Instantiate(GameManager.gm.pMessageWnd, transform);
        confirmWnd.GetComponent<wConfirm>().InitUI("真的要离开吗？", "中途退出游戏将不会储存任何游戏进度。确定返回主界面？", "GoToMain", gameObject);
    }

    void GoToMain()
    {
        Time.timeScale = 1f;
        GameManager.gm.LoadScene(0);
    }

    public void UpdateTimeDisplay()
    {
        tCountdown.text = level.TimeRemain();
    }

}
