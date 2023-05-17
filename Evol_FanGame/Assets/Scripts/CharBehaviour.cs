using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TMPro;

public class CharBehaviour : MonoBehaviour
{
    public GameObject charGFX;
    public int charIdx;
    public OverlayTile activeTile;
    public bool hasFoodInHand;
    public bool isBringingCustomerToTable;
    public bool isCarryingFood;
    public int foodIdx;
    public GameObject foodObj;
    public Transform foodContainer;
    public float distanceToTarget;
    public eIndividualObjs curTargetObj;
    public TMP_Text tTableNum;
    public GameObject activeIndicator;

    public sInteractiveObj curCustomer;
    //public Animator charAnimator;
    //bool isFlip;
    //bool isFront;

    LevelManager level;
    PlayerController player;

    [Space]
    [Header("AI Pathfinding")]
    [Space]

    public Transform target;




    private void Start()
    {
        level = LevelManager.level;
        player = PlayerController.player;
        GetComponent<AIDestinationSetter>().target = target;
        tTableNum.gameObject.SetActive(false);
        activeIndicator.SetActive(false);
        CheckWaiterTask();
    }

    public bool ReachedEndOfPath(eIndividualObjs _indiObj)
    {
        distanceToTarget = Mathf.Abs((target.position - transform.position).magnitude);
        if (distanceToTarget <= GetComponent<AIPath>().endReachedDistance + 0.1f)
        {
            curTargetObj = _indiObj;
            return true;
        }
        else return false;
    }

    void CheckWaiterTask()
    {
        StartCoroutine(CheckReachedTarget());
    }

    IEnumerator CheckReachedTarget()
    {
        
        while (level.levelStarted)
        {
            yield return new WaitForSeconds(0.01f);

            // if the waiter is at the target pos of an interactive object
            if (ReachedEndOfPath(player.taskObj[charIdx]))
            {
                // if the waiter hasn't performed any task yet
                if (!player.hasPerformedTaskAtCurTarget[charIdx])
                {
                    // perform task
                    player.WaiterTask(player.taskObj[charIdx], level.waiter[charIdx]);
                    player.hasPerformedTaskAtCurTarget[charIdx] = true;
                }
            }
        }
    }



}
