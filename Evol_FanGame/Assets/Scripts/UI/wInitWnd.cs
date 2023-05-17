using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class wInitWnd : MonoBehaviour
{
    LevelManager level;
    GameManager gm;

    [Header("Buttons")]
    [Space]
    public Button bStartGame;
    public Button[] bPositions; // 0-1: waiter, 2: chef, 3-4: rest, 5: explorer
    [NamedArray(typeof(eChar))] public Button[] bNeedAssign; // 0-5: chars, 6: clear cur pos
    public Button bAssignBack;
    public Button bConfirm;

    public Image[] iPositions;
    //[NamedArray(typeof(eChar))] public TMP_Text[] tStamina;

    [Space]
    [Header("Info")]
    [Space]
    [NamedArray(typeof(eIngredient))] public TMP_Text[] tIngredientNum;
    public TMP_Text tTotalIncome;
    public TMP_Text tNextGoal;

    [Space]
    [Header("Assign")]
    [Space]

    public GameObject goInfo;
    public TMP_Text[] tIndiStamina;
    public TMP_Text tHint;
    public TMP_Text tName;
    public TMP_Text tStamina;
    public TMP_Text tIngredientName;
    public Image iCharHead;
    public Image[] iCake;
    public Image iIngredient;
    public string[] stringName;
    public string[] stringIngName;
    public Sprite[] spriteCake;
    public Sprite[] spriteIngre;

    bool willStartGame = false;
    bool nextRoundPlayable = false;


    eChar curViewChar;

    public Sprite[] spriteChars; // 0-5: chars, 6: +

    public GameObject assignPanel;

    bool[] charSufficientStamina;
    bool[] charNotInUse;

    int curSelectedPos;

    // for assigned positions
    eChar[] charsAtPosition;


    void Start()
    {
        level = LevelManager.level;
        gm = GameManager.gm;
        InitUI();
    }

    void InitUI()
    {
        InitBtn();
        InitIncome();
        InitCharData();

        for (int i = 0; i < tIngredientNum.Length; i++)
        {
            tIngredientNum[i].text = gm.ingredientAmt[i].ToString();
        }
        InitAssignPanel();
        //bStartGame.interactable = false;
    }

    void InitBtn()
    {
        bStartGame.onClick.AddListener(OnStartGameClick);
        bPositions[0].onClick.AddListener(delegate { OnPositionClick(0); });
        bPositions[1].onClick.AddListener(delegate { OnPositionClick(1); });
        bPositions[2].onClick.AddListener(delegate { OnPositionClick(2); });
        bPositions[3].onClick.AddListener(delegate { OnPositionClick(3); });
        bPositions[4].onClick.AddListener(delegate { OnPositionClick(4); });
        bPositions[5].onClick.AddListener(delegate { OnPositionClick(5); });
        bNeedAssign[0].onClick.AddListener(delegate { OnCharSelect(eChar.me); });
        bNeedAssign[1].onClick.AddListener(delegate { OnCharSelect(eChar.li); });
        bNeedAssign[2].onClick.AddListener(delegate { OnCharSelect(eChar.xu); });
        bNeedAssign[3].onClick.AddListener(delegate { OnCharSelect(eChar.bai); });
        bNeedAssign[4].onClick.AddListener(delegate { OnCharSelect(eChar.zhou); });
        bNeedAssign[5].onClick.AddListener(delegate { OnCharSelect(eChar.ling); });
        bNeedAssign[6].onClick.AddListener(delegate { OnCharSelect(eChar.none); });
        bConfirm.onClick.AddListener(OnConfirmClick);
        bAssignBack.onClick.AddListener(OnAssignBackClick);
    }

    void InitIncome()
    {
        tTotalIncome.text = gm.money.ToString();
        if (gm.money < gm.moneyGoals[0])
        {
            tNextGoal.text = gm.moneyGoals[0].ToString();
        }
        else if (gm.money >= gm.moneyGoals[0] && gm.money < gm.moneyGoals[1])
        {
            tNextGoal.text = gm.moneyGoals[1].ToString();
        }
        else if (gm.money >= gm.moneyGoals[1] && gm.money < gm.moneyGoals[2])
        {
            tNextGoal.text = gm.moneyGoals[2].ToString();
        }
        else
        {
            tNextGoal.text = "已完成目标";
        }
    }

    void InitCharData()
    {
        curViewChar = eChar.none;
        charsAtPosition = new eChar[(int)eChar.none];
        for (int i = 0; i < charsAtPosition.Length; i++)
        {
            charsAtPosition[i] = eChar.none;
        }
        charSufficientStamina = new bool[(int)eChar.none + 1];
        charNotInUse = new bool[(int)eChar.none];
        for (int i = 0; i < charSufficientStamina.Length; i++)
        {
            charSufficientStamina[i] = true;
        }
        for (int i = 0; i < charNotInUse.Length; i++)
        {
            charNotInUse[i] = true;
        }
    }

    void InitAssignPanel()
    {
        for (int i = 0; i < (int)eChar.none; i++)
        {
            tIndiStamina[i].text = "体力" + gm.stamina[i].ToString() + "/6";
        }
        CheckCharAvailability();
    }

    void InitAssignInfo(int _posIdx)
    {
        curViewChar = charsAtPosition[_posIdx];
        OnCharSelect(curViewChar);
    }

    void OnCharSelect(eChar _char)
    {
        curViewChar = _char;
        if (_char != eChar.none)
        {
            goInfo.SetActive(true);
            tHint.gameObject.SetActive(false);
            tName.text = stringName[(int)curViewChar];
            tStamina.text = "体力" + gm.stamina[(int)curViewChar].ToString() + "/6";
            iCharHead.sprite = spriteChars[(int)curViewChar];
            for (int i = 0; i < iCake.Length; i++)
            {
                iCake[i].gameObject.SetActive(false);
            }
            iCake[(int)curViewChar].gameObject.SetActive(true);
            iIngredient.sprite = spriteIngre[(int)curViewChar];
            tIngredientName.text = stringIngName[(int)curViewChar];
        }
        else
        {
            goInfo.SetActive(false);
            tHint.gameObject.SetActive(true);
        }
    }

    bool[] CheckCharAvailability()
    {
        bool[] availability = new bool[6];
        for (int i = 0; i < bNeedAssign.Length - 1; i++)
        {
            switch (curSelectedPos)
            {
                case 0:
                case 1:
                    availability[i] = gm.stamina[i] >= 2 && charNotInUse[i] ? true: false;
                    break;
                case 2:
                    availability[i] = gm.stamina[i] >= 3 && charNotInUse[i] ? true: false;
                    break;
                case 3:
                case 4:
                case 5:
                    availability[i] = charNotInUse[i] ? true : false;
                    break;
                default:
                    break;
            }
        }
        
        return availability;
    }

    void OnPositionClick(int _positionIdx)
    {
        curSelectedPos = _positionIdx;
        InitAssignInfo(_positionIdx);
        assignPanel.SetActive(true);

        // init assign panel
        InitAssignPanel();
        for (int i = 0; i < bNeedAssign.Length - 1; i++)
        {
            bNeedAssign[i].interactable = CheckCharAvailability()[i];
        }
    }

    void OnConfirmClick()
    {

        // when changing to another char, make the current char on the position available

        // then set the button back to interactable
        if (charsAtPosition[curSelectedPos] != eChar.none)
        {
            charNotInUse[(int)charsAtPosition[curSelectedPos]] = true;
            gm.positions[(int)charsAtPosition[curSelectedPos]] = ePosition.none;
            bNeedAssign[(int)charsAtPosition[curSelectedPos]].interactable = CheckCharAvailability()[(int)charsAtPosition[curSelectedPos]];
        }

        if ((int)curViewChar != 6)
        {
            charNotInUse[(int)curViewChar] = false;
            bNeedAssign[(int)curViewChar].interactable = CheckCharAvailability()[(int)curViewChar];
        }
        // update saved char array
        charsAtPosition[curSelectedPos] = curViewChar;

        if (charsAtPosition[curSelectedPos] != eChar.none)
        {
            switch (curSelectedPos)
            {
                case 0:
                case 1:
                    gm.positions[(int)charsAtPosition[curSelectedPos]] = ePosition.waiter;
                    break;
                case 2:
                    gm.positions[(int)charsAtPosition[curSelectedPos]] = ePosition.chef;
                    break;
                case 3:
                case 4:
                    gm.positions[(int)charsAtPosition[curSelectedPos]] = ePosition.rest;
                    break;
                case 5:
                    gm.positions[(int)charsAtPosition[curSelectedPos]] = ePosition.explorer;
                    break;
                default:
                    break;
            }
        }



        // update sprite on position
        iPositions[curSelectedPos].sprite = spriteChars[(int)curViewChar];
        bStartGame.interactable = true;

        for (int i = 0; i < gm.positions.Length - 1; i++)
        {
            if (gm.positions[i] != ePosition.none)
            {
                continue;
            }
            else
            {
                bStartGame.interactable = false;
                break;
            }
        }

        OnAssignBackClick();

    }

    void StartGame()
    {
        int waiterIdx = 0;
        int restIdx = 0;
        for (int i = 0; i < gm.positions.Length; i++)
        {
            if (gm.positions[i] == ePosition.waiter)
            {
                level.waiterChars[waiterIdx] = (eChar)i;
                waiterIdx++;
                GameObject waiter = Instantiate(gm.pWaiters[i], PlayerController.player.waiterTargetPos[waiterIdx - 1].transform.position, Quaternion.identity);
                level.waiter[waiterIdx - 1] = waiter.GetComponent<CharBehaviour>();
                level.waiter[waiterIdx - 1].target = PlayerController.player.waiterTargetPos[waiterIdx - 1].transform;
                //Debug.Log(level.waiter[waiterIdx - 1].transform.position);
            }
            if (gm.positions[i] == ePosition.chef)
            {
                level.chefChar = (eChar)i;
            }
            if (gm.positions[i] == ePosition.rest)
            {
                level.restChar[restIdx] = (eChar)i;
                restIdx++;
            }
            if (gm.positions[i] == ePosition.explorer)
            {
                level.exploreChar = (eChar)i;
            }
        }
        level.levelStarted = true;
        DecorManager.decor.SetNeonSignColor(level.waiterChars[0], level.waiterChars[1]);
        DecorManager.decor.SetMenuSignContent(level.chefChar);
        level.StartGame();
        gameObject.SetActive(false);
    }

    void OnStartGameClick()
    {
        CheckNextTurnPlayability();
        if (gm.progress < 10)
        {
            if (nextRoundPlayable)
            {
                StartGame();
            }
            else
            {
                DisplayConfirmMessage();
            }
        }
        else
        {
            StartGame();

        }
    }

    void DisplayConfirmMessage()
    {
        GameObject confirmWnd = Instantiate(gm.pMessageWnd, transform);
        confirmWnd.GetComponent<wConfirm>().bConfirm.gameObject.SetActive(false);
        confirmWnd.GetComponent<wConfirm>().InitUI("工作分配问题", "当前工作分配方式会造成下一轮游戏无法开始，请重新分配", "StartGame", gameObject);
    }

    void CheckNextTurnPlayability()
    {
        // each bool indicates the availability of one character being available to the specific position
        bool[] charAvailableForWaiter = new bool[6]; 
        bool[] charAvailableForChef = new bool[6];
        int[] predictedStamina = new int[6];
        int dStamina = 0;

        for (int i = 0; i < predictedStamina.Length; i++)
        {
            switch (gm.positions[i])
            {
                case ePosition.waiter:
                    dStamina = -2;
                    break;
                case ePosition.chef:
                    dStamina = -3;
                    break;
                case ePosition.explorer:
                    dStamina = 1;
                    break;
                case ePosition.rest:
                    dStamina = 3;
                    break;
                case ePosition.none:
                    break;
                default:
                    break;
            }
            predictedStamina[i] = gm.stamina[i] + dStamina;
        }

        // update available char arrays
        for (int i = 0; i < charAvailableForChef.Length; i++)
        {
            charAvailableForChef[i] = predictedStamina[i] - 3 < 0 ? false : true;
            charAvailableForWaiter[i] = predictedStamina[i] - 2 < 0 ? false : true;
        }

        // CHECK LOGIC:
        // chef available num should be <= waiter available num
        // 1. check if there is any available chef
        // - if >= 3: playable for the next round
        // - else if == 2 (which means only 2 char have stamina >= 3): take out these 2 idx and check for waiter availability
        // --- if waiter available >= 1 (have at least 1 char with stamina >= 2): playable for the next round
        // --- else: ASK TO REASSIGN
        // - else if == 1 (which means only 1 char have stamina >= 3): take out this 1 idx and check for waiter availability
        // --- if waiter available >= 2 (have at least 2 chars with stamina >= 2): playable for the next round 
        // --- else: ASK TO REASSIGN
        // - else: ASK TO REASSIGN

        int availableChefNum = 0;
        for (int i = 0; i < charAvailableForChef.Length; i++)
        {
            availableChefNum += charAvailableForChef[i] ? 1 : 0;
        }

        if (availableChefNum >= 3)
        {
            nextRoundPlayable = true;
        }
        else if (availableChefNum >= 2)
        {
            int[] availableChefIdx = new int[2];
            int filledChefNum = 0;
            for (int i = 0; i < charAvailableForChef.Length; i++)
            {
                if (charAvailableForChef[i])
                {
                    availableChefIdx[filledChefNum] = i;
                    filledChefNum++;
                }
            }
            int waiterAvailableNum = 0;
            for (int i = 0; i < charAvailableForWaiter.Length; i++)
            {
                if (i != availableChefIdx[0] && i != availableChefIdx[1])
                {
                    if (charAvailableForWaiter[i])
                    {
                        waiterAvailableNum++;
                    }
                }
            }
            nextRoundPlayable = waiterAvailableNum >= 1;
        }
        else if (availableChefNum >= 1)
        {
            int availableChefIdx = new int();
            for (int i = 0; i < charAvailableForChef.Length; i++)
            {
                if (charAvailableForChef[i])
                {
                    availableChefIdx = i;
                }
            }
            int waiterAvailableNum = 0;
            for (int i = 0; i < charAvailableForWaiter.Length; i++)
            {
                if (i != availableChefIdx)
                {
                    if (charAvailableForWaiter[i])
                    {
                        waiterAvailableNum++;
                    }
                }
            }
            nextRoundPlayable = waiterAvailableNum >= 2;
        }
        else
        {
            nextRoundPlayable = false;
        }

    }

    void OnAssignBackClick()
    {
        assignPanel.SetActive(false);
    }

}
