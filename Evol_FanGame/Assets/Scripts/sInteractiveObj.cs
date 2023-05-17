using Pathfinding;
using System.Collections;
using UnityEngine;

public class sInteractiveObj : MonoBehaviour
{
    [Header("General")]

    public eIndividualObjs obj;

    [Space]
    [Header("Table")]
    [Space]

    public bool tableTaken;
    public Transform[] cusObjStandPos;
    public Transform[] seatPos;

    public sInteractiveObj seatedCustomer;
    public GameObject food;
    public GameObject foodPos;
    public GameObject plate;
    public GameObject tableNumberGFX;


    [Space]
    [Header("Customer")]
    [Space]
    public bool isWaitArea;
    public bool isFollower;
    public GameObject follower;
    public Transform followerInLinePos;
    public float customerEatCount;
    public float totalTimeCustomerEat = 10f;
    public float distanceToTarget;
    public GameObject sitFrontGFX;
    public GameObject sitBackGFX;
    public eCustomerStatus cusStatus;
    bool isMoving;

    [Space]
    [Header("Kitchen")]
    [Space]
    public float cookSpeed = 0.05f;
    public float orderProgress;
    public bool[] orderReady;

    public bool hasReadyOrder;
    LevelManager level;

    cInGame canvas;
    DecorManager decor;

    private void Start()
    {
        canvas = cInGame.canvas;
        level = LevelManager.level;
        decor = DecorManager.decor;
        orderReady = new bool[6];
        if (obj == eIndividualObjs.waitLine && !isWaitArea)
        {
            StartCoroutine(CheckCustomerMoving());
        }
    }

    IEnumerator CheckCustomerMoving()
    {
        while (level.levelStarted)
        {
            yield return new WaitForSeconds(0.01f);
            if (GetComponent<AIPath>().velocity.magnitude >= 0.1f)
            {
                // customer is moving, update animation
                GetComponent<sCharAnimation>().charAnimator.SetBool("isMoving", true);
                if (!isMoving)
                {
                    isMoving = true;
                }
            }
            else
            {
                if (isMoving)
                {
                    isMoving = false;
                }
                if (CustomerStopInLine())
                {
                    GetComponent<sCharAnimation>().charAnimator.SetBool("isUp", false);
                    GetComponent<sCharAnimation>().charGFX.transform.localScale = new Vector3(-1, 1, 1);
                }
            }

        }
    }

    // FROM KITCHEN OBJ
    public void CustomerWaitingForFood(int _tableIdx)
    {
        level.interactiveObjs[_tableIdx + 1].seatedCustomer.CustomerGFXUpdate(_tableIdx);
        level.interactiveObjs[_tableIdx + 1].seatedCustomer.follower.GetComponent<sInteractiveObj>().CustomerGFXUpdate(_tableIdx);
        // kitchen start making food
        StartCoroutine(StartCookingProgress(_tableIdx));
        // display ordered food at table
        // display food being made at kitchen
    }


    // FROM CUSTOMER OBJ
    public void CustomerGFXUpdate(int _tableIdx)
    {
        StartCoroutine(CheckCustomerSeated(_tableIdx));
    }

    IEnumerator CheckCustomerSeated(int _tableIdx)
    {
        yield return new WaitUntil(() => (GetComponent<AIPath>().reachedEndOfPath));
        yield return new WaitForSeconds(1.3f);
        StartCoroutine(UpdateCusGFX(true, _tableIdx));
    }

    IEnumerator UpdateCusGFX(bool _showSit, int _tableIdx)
    {
        // hide customer gfx
        GetComponent<sCharAnimation>().charGFX.GetComponent<SpriteRenderer>().enabled = !_showSit;

        // show sit gfx
        if (isFollower)
        {
            float timer = 0;
            while (timer < 2)
            {
                yield return new WaitForSeconds(0.1f);
                if (sitFrontGFX.transform.position != level.interactiveObjs[_tableIdx + 1].seatPos[0].position)
                {
                    sitFrontGFX.transform.position = level.interactiveObjs[_tableIdx + 1].seatPos[0].position;
                    if (!sitFrontGFX.activeSelf)
                    {
                        sitFrontGFX.SetActive(_showSit);
                    }
                }
                timer += 0.1f;
            }
        }
        else
        {
            float timer = 0;
            while (timer < 2)
            {
                yield return new WaitForSeconds(0.1f);
                if (sitBackGFX.transform.position != level.interactiveObjs[_tableIdx + 1].seatPos[1].position)
                {
                    sitBackGFX.transform.position = level.interactiveObjs[_tableIdx + 1].seatPos[1].position;
                    if (!sitBackGFX.activeSelf)
                    {
                        sitBackGFX.SetActive(_showSit);
                    }
                }
                timer += 0.1f;
            }
        }
    }

    public void CustomerStartEating()
    {
        StartCoroutine(CustomerEat());
    }

    public void StopAllCurCoroutine()
    {
        StopAllCoroutines();
    }

    IEnumerator CustomerEat()
    {
        customerEatCount = 0;
        while (customerEatCount < totalTimeCustomerEat)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            customerEatCount += Time.deltaTime;
        }
        if (!level.levelCompleted)
        {
            // customer done eating and leave
            plate.SetActive(false);
            // TODO: disable coffee images

            tableNumberGFX.SetActive(true);
            seatedCustomer.GetComponent<sCharAnimation>().charGFX.GetComponent<SpriteRenderer>().enabled = true;
            seatedCustomer.follower.GetComponent<sCharAnimation>().charGFX.GetComponent<SpriteRenderer>().enabled = true;
            seatedCustomer.follower.GetComponent<sInteractiveObj>().sitFrontGFX.SetActive(false);
            seatedCustomer.sitBackGFX.SetActive(false);


            level.ServeOneCustomer();
            LevelManager.level.UpdateCustomerSetTarget(seatedCustomer, 0.3f, seatedCustomer.transform, LevelManager.level.doorPos);
            Destroy(food);
            for (int i = 0; i < level.totalCustomers.Count; i++)
            {
                if (seatedCustomer == level.totalCustomers[i])
                {
                    level.totalCustomers.RemoveAt(i);
                }
            }
            tableTaken = false;
            yield return new WaitUntil(() => ReachedEndOfPath(seatedCustomer));
            Destroy(seatedCustomer.follower);
            Destroy(seatedCustomer.gameObject);
        }

    }

    IEnumerator StartCookingProgress(int _tableIdx)
    {
        float start = 0;
        float end = 1;
        float curPct = start;
        level.cookingProgressLights[_tableIdx].color = level.cookingProgress[(int)eCookingStatus.cooking];
        while (curPct < end)
        {
            curPct += cookSpeed;
            //canvas.UpdateCookingStatus(_tableIdx, curPct.ToString("p"));
            yield return new WaitForSeconds(1f);
        }
        orderReady[_tableIdx] = true;
        AudioManager.am.PlaySFX(eSFX.foodReady);
        decor.UpdateCounterFoodDisplay(_tableIdx, true);
        if (!hasReadyOrder)
        {
            hasReadyOrder = true;
        }
        //canvas.UpdateCookingStatus(_tableIdx, canvas.stringKitchenStatus[2]);
        level.cookingProgressLights[_tableIdx].color = level.cookingProgress[(int)eCookingStatus.done];

    }


    public bool ReachedEndOfPath(sInteractiveObj _checkedObj)
    {
        distanceToTarget = Mathf.Abs((_checkedObj.GetComponent<AIDestinationSetter>().target.position - _checkedObj.transform.position).magnitude);
        if (distanceToTarget <= _checkedObj.GetComponent<AIPath>().endReachedDistance + 0.6f)
        {
            return true;
        }
        else return false;
    }

    bool CustomerStopInLine()
    {
        return this != isWaitArea &&  !isMoving && ReachedEndOfPath(this);
    }

}
