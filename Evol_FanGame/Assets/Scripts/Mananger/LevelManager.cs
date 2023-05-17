using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager level;
    PlayerController player;
    GameManager gm;
    PlayerData data;
    cInGame canvas;
    public IntroManager intro;

    [Space]
    [Header("Prefabs")]
    public GameObject[] pCustomer;
    public GameObject pCustomerFollower;
    [NamedArray(typeof(eChar))] public GameObject[] pFood;


    //public bool isLeadingCustomer;
    public List<sInteractiveObj> totalCustomers;
    public List<sInteractiveObj> customersInLine;
    public CharBehaviour[] waiter;
    public int totalTableNum;
    public Transform[] linePos;
    public Transform doorPos;
    public LayerMask interactiveObjLayer;

    [NamedArray(typeof(eCookingStatus))] public Color[] cookingProgress;
    public SpriteRenderer[] cookingProgressLights;


    [Space]
    [Header("In Game Data")]
    public int singleExploreAmt;
    public int oneCustomerEarning;
    public int numOfCustomersServed;
    public int[] startingIngredient;
    public eChar[] waiterChars;
    public eChar chefChar;
    public eChar[] restChar;
    public eChar exploreChar;

    [Space]
    [Header("Game Progress")]
    public bool[] linePosHasCustomer;
    public bool levelStarted;
    public bool levelCompleted;
    public int[] timeDigit;
    [NamedArray(typeof(eIndividualObjs))] public sInteractiveObj[] interactiveObjs;

    public bool introFinished;
    public bool tutorialFinished;

    private void Awake()
    {
        level = this;

    }

    void Start()
    {
        // arrange jobs
        gm = GameManager.gm;
        data = FindObjectOfType<PlayerData>();
        canvas = cInGame.canvas;
        player = PlayerController.player;

        StartCoroutine(CheckIntroAndInitLevel());


    }

    IEnumerator CheckIntroAndInitLevel()
    {
        gm.LoadData();
        yield return new WaitUntil(() => gm.doneInit);
        Debug.Log("gm new game: " + gm.isNewGame);
        Debug.Log("data new game: " + FindObjectOfType<PlayerData>().progData.isNewGame);
        if (gm.isNewGame)
        {
            introFinished = false;
            // do intro
            canvas.gameObject.SetActive(false);
            intro.gameObject.SetActive(true);


            yield return new WaitUntil(() => introFinished);
            intro.gameObject.SetActive(false);
            canvas.gameObject.SetActive(true);
        }
        canvas.InitCanvas();
        if (!gm.haveReadTutorial)
        {
            Instantiate(gm.pTutorial, canvas.transform);
            yield return new WaitUntil(() => tutorialFinished);
        }
        Instantiate(gm.pInitWnd, canvas.initPos);
        for (int i = 0; i < cookingProgressLights.Length; i++)
        {
            cookingProgressLights[i].color = cookingProgress[(int)eCookingStatus.none];
        }
        for (int i = 0; i < player.waiterTargetPos.Length; i++)
        {
            player.waiterTargetPos[i].GetComponent<SpriteRenderer>().enabled = false;
        }
    }


    public void StartGame()
    {

        StartCoroutine(StartLevel());
    }

    public string TimeRemain()
    {
        return timeDigit[0].ToString() + timeDigit[1].ToString() + ":" + timeDigit[2].ToString() + timeDigit[3].ToString();
    }

    IEnumerator StartLevel()
    {
        yield return new WaitUntil(() => levelStarted);
        //player.enabled = true;
        // start countdown
        DecorManager.decor.InitCounterFood();
        for (int i = 0; i < waiter.Length; i++)
        {
            waiter[i].charIdx = i;
        }
        StartCoroutine(StartCountdown());
        numOfCustomersServed = 0;
        startingIngredient = gm.ingredientAmt;
        InitLine();
        int waiterNum = 0;
        for (int j = 0; j < (int)eChar.none; j++)
        {
            if (gm.positions[j] == ePosition.waiter)
            {
                canvas.waiterTabsFull[waiterNum].GetComponent<Image>().sprite = canvas.spriteIDs[j];
                canvas.waiterTabs[waiterNum].GetComponent<Image>().sprite = canvas.spriteNames[j];
                canvas.tWaiterName[waiterNum].text = canvas.stringNames[j];
                canvas.tWaiterNameTag[waiterNum].text = canvas.stringNames[j];
                canvas.waiterTabs[waiterNum].SetActive(true);
                waiterNum++;
            }
        }

        // after game starts
        InvokeRepeating("SpawnCustomer", 3f, 5f);
    }

    IEnumerator StartCountdown()
    {
        while (!levelCompleted)
        {
            yield return new WaitForSeconds(1);
            timeDigit[3] -= 1;
            if (timeDigit[3] < 0)
            {
                timeDigit[3] = 9;
                timeDigit[2] -= 1;
                if (timeDigit[2] < 0)
                {
                    timeDigit[2] = 5;
                    timeDigit[1] -= 1;
                    if (timeDigit[1] < 0)
                    {
                        // game end
                        StopCoroutine(StartCountdown());
                        CompleteLevel();
                    }
                }
            }
            // update time display
            canvas.UpdateTimeDisplay();
        }

    }



    void InitLine()
    {
        linePosHasCustomer = new bool[(int)eChar.none];
        for (int i = 0; i < linePosHasCustomer.Length; i++)
        {
            linePosHasCustomer[i] = false;
        }
    }


    void SpawnCustomer()
    {
        if (totalCustomers.Count < totalTableNum && !levelCompleted && levelStarted) // check total customer number
        {
            // spawn customer at door
            int randNum = Random.Range(0, pCustomer.Length);
            sInteractiveObj customerObj = Instantiate(pCustomer[randNum], doorPos.position, Quaternion.identity).GetComponent<sInteractiveObj>();
            customerObj.cusStatus = eCustomerStatus.waitAtDoor;
            int followerRandNum = Random.Range(0, pCustomer.Length);
            customerObj.follower = Instantiate(pCustomer[followerRandNum], doorPos.position, Quaternion.identity);
            customerObj.follower.GetComponent<sInteractiveObj>().isFollower = true;
            AudioManager.am.PlaySFX(eSFX.doorbell);
            customerObj.follower.GetComponent<sInteractiveObj>().cusStatus = eCustomerStatus.waitAtDoor;
            customerObj.follower.GetComponent<AIDestinationSetter>().target = customerObj.followerInLinePos.transform;
            // maybe randomize customer to spawn


            // move to first available spot
            for (int i = 0; i < linePosHasCustomer.Length; i++)
            {
                if (!linePosHasCustomer[i])
                {
                    customerObj.GetComponent<AIDestinationSetter>().target = linePos[i];
                    linePosHasCustomer[i] = true;
                    totalCustomers.Add(customerObj);
                    customersInLine.Add(customerObj);
                    return;
                }
            }

        }
    }

    public void UpdateCustomerSetTarget(sInteractiveObj _customer, float _followerDistance, Transform _followerTarget, Transform _customerTarget)
    {
        _customer.follower.GetComponent<AIPath>().endReachedDistance = _followerDistance;
        _customer.follower.GetComponent<AIDestinationSetter>().target = _followerTarget;
        _customer.GetComponent<AIDestinationSetter>().target = _customerTarget;
    }

    public void MoveCustomerUp()
    {
        for (int i = 0; i < customersInLine.Count; i++)
        {
            customersInLine[i].GetComponent<AIDestinationSetter>().target = linePos[i];
            linePosHasCustomer[i] = true;
            linePosHasCustomer[i + 1] = false;
        }

    }

    public void CompleteLevel()
    {
        // end timer, end all coroutine and invoke
        CancelInvoke("SpawnCustomer");
        StopAllCoroutines();

        gm.progress++;
        Debug.Log("progress added");

        // record data
        RecordData();
        // display summary page
        levelCompleted = true;
        levelStarted = false;
        Instantiate(gm.pSummaryWnd, canvas.transform);

    }

    public void EndGameHalfWay()
    {
        CancelInvoke("SpawnCustomer");
        StopAllCoroutines();
        foreach (sInteractiveObj item in interactiveObjs)
        {
            item.StopAllCurCoroutine();
        }
        levelCompleted = false;
        levelStarted = false;
        // display summary page
        Instantiate(gm.pSummaryWnd, canvas.transform);

    }

    public void ServeOneCustomer()
    {
        numOfCustomersServed++;
        canvas.customerNum.text = numOfCustomersServed.ToString();
        canvas.tCurTurnMoney.text = (numOfCustomersServed * oneCustomerEarning).ToString();
    }

    void CalculateStamina()
    {
        for (int i = 0; i < gm.stamina.Length; i++)
        {
            int dStamina = 0;
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
            if (gm.stamina[i] + dStamina > gm.originalStamina)
            {
                gm.stamina[i] = gm.originalStamina;
            }
            else if (gm.stamina[i] + dStamina < 0)
            {
                gm.stamina[i] = 0;
            }
            else
            {
                gm.stamina[i] += dStamina;
            }
        }
    }

    public int[] CalculateCurTurnIngredient()
    {
        int[] ing = new int[6];
        for (int i = 0; i < gm.positions.Length; i++)
        {
            if (gm.positions[i] == ePosition.explorer)
            {
                ing[i] += singleExploreAmt;
            }
        }
        return ing;
    }

    public int CalculateMoneyEarned()
    {
        return numOfCustomersServed * oneCustomerEarning;
    }

    public void RecordData()
    {

        // write stamina
        CalculateStamina();
        data.charData.stamina = gm.stamina;
        data.SaveChar();
        // write ingredient
        for (int i = 0; i < gm.ingredientAmt.Length; i++)
        {
            gm.ingredientAmt[i] += CalculateCurTurnIngredient()[i];
        }
        data.inventoryData.ingredients = gm.ingredientAmt;
        // write money
        gm.money += CalculateMoneyEarned();
        data.inventoryData.money = gm.money;
        data.SaveInventory();
        // write progress
        if (gm.isNewGame)
        {
            gm.isNewGame = false;
            data.progData.isNewGame = gm.isNewGame;
        }
        data.progData.levelNum = gm.progress;
        data.progData.haveReadTutorial = gm.haveReadTutorial;
        data.SaveProgress();

    }

}
