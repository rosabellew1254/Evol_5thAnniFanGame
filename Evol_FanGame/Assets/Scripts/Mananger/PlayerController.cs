using Pathfinding;
using System.Collections;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController player;
    public CharBehaviour activeChar;
    [SerializeField] LayerMask obstacle;
    [SerializeField] LayerMask interactiveObjLayer;
    [NamedArray(typeof(eIndividualObjs))] public Transform[] waiterAtObjPos;
    public GameObject[] waiterTargetPos;
    public sInteractiveObj[] customersFollowing;

    LevelManager level;
    cInGame canvas;

    public int activeWaiterIdx;
    GameObject overlayTile;
    public eIndividualObjs[] taskObj;
    public bool[] hasPerformedTaskAtCurTarget = new bool[2];


    private void Awake()
    {
        player = this;
    }

    private void Start()
    {
        level = LevelManager.level;
        canvas = cInGame.canvas;
        customersFollowing = new sInteractiveObj[2];
        activeWaiterIdx = -1;
        taskObj = new eIndividualObjs[2];
        for (int i = 0; i < taskObj.Length; i++)
        {
            taskObj[i] = eIndividualObjs.none;
        }
    }

    //============================ waiter moving start =================================
    public RaycastHit2D? GetFocusedOnTile() // 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D interactiveObj = new RaycastHit2D();
        RaycastHit2D ground = new RaycastHit2D();
        RaycastHit2D obstacle = new RaycastHit2D();
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);
        int[] existingHits = new int[hits.Length]; // none: 0, interactive: 1, obstacle: 2, ground: 3
        for (int i = 0; i < existingHits.Length; i++)
        {
            existingHits[i] = 0;
        }
        if (activeWaiterIdx != -1)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log("hit layer: " + hits[i].collider.gameObject.layer);
                if (hits[i].collider.gameObject.layer == 7) // hit interactive obj
                {
                    waiterTargetPos[activeWaiterIdx].transform.position = waiterAtObjPos[(int)hits[i].collider.GetComponent<sInteractiveObj>().obj].transform.position;
                    hasPerformedTaskAtCurTarget[activeWaiterIdx] = false;
                    interactiveObj = hits[i];
                    existingHits[i] = 1;

                }
                else if (hits[i].collider.gameObject.layer == 6) // obstacle
                {
                    obstacle = hits[i];
                    existingHits[i] = 2;
                }
                else if (hits[i].collider.gameObject.layer == 9) // ground
                {
                    ground = hits[i];
                    existingHits[i] = 3;

                    continue;
                }
                else
                {
                    continue;
                }
            }
            /*foreach (RaycastHit2D hit in hits)
            {            
                Debug.Log("hit layer: " + hit.collider.gameObject.layer);
                if (hit.collider.gameObject.layer == 7) // hit interactive obj
                {
                    waiterTargetPos[activeWaiterIdx].transform.position = waiterAtObjPos[(int)hit.collider.GetComponent<sInteractiveObj>().obj].transform.position;
                    hasPerformedTaskAtCurTarget[activeWaiterIdx] = false;
                    return hit;
                }
                /*else if (hit.collider.gameObject.layer == 6) // hit obstacle
                {
                    Debug.Log("hit obstacle");
                    continue;
                    //return null;
                }
                else
                {
                    ground = hit;
                }
                else if (hit.collider.gameObject.layer == 6) // obstacle
                {
                    obstacle = hit;
                }
                else if (hit.collider.gameObject.layer == 9) // ground
                {
                    ground = hit;
                    continue;
                }
                else
                {
                    continue;
                }
            }*/

            int interactiveObjAmt = 0;
            int obstacleAmt = 0;
            int groundAmt = 0;
            for (int i = 0; i < existingHits.Length; i++)
            {
                switch (existingHits[i])
                {
                    case 1:
                        interactiveObjAmt++;
                        break;
                    case 2:
                        obstacleAmt++;
                        break;
                    case 3:
                        groundAmt++;
                        break;
                    default:
                        break;
                }
            }
            if (interactiveObjAmt >= 1)
            {
                return interactiveObj;
            }
            else if (obstacleAmt >= 1)
            {
                return obstacle;
            }
            else if (groundAmt >= 1)
            {
                return ground;
            }
            else return null;


            /*if ()
            {

            }
            else if (hits.Length > 0)
            {
                return ground;
            }
            else
            {
                return null;
            }*/
        }
        return null;
    }

    void ClickOnTile()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var focusedTileHit = GetFocusedOnTile();
            if (focusedTileHit != null && focusedTileHit.HasValue)
            {
                level.waiter[activeWaiterIdx].GetComponent<sCharAnimation>().charAnimator.SetBool("isMoving", true);
                overlayTile = focusedTileHit.Value.collider.gameObject;
                Debug.Log("layer num: " + overlayTile.layer);
                if (overlayTile.layer == 6) // obstacle
                {
                    Debug.Log("hit obstacle");
                }
                else if (overlayTile.layer == 7) // interactive obj
                {
                    // do a check through of the current status of the waiter and determine whether or not to move the waiter

                    taskObj[activeWaiterIdx] = overlayTile.GetComponent<sInteractiveObj>().obj;
                    //WaiterActionOutput(obj);
                    if (overlayTile.GetComponent<OverlayTile>() != null)
                    {
                        overlayTile.GetComponent<OverlayTile>().ShowHighlight();
                    }
                }
                else
                {
                    taskObj[activeWaiterIdx] = eIndividualObjs.none;

                    waiterTargetPos[activeWaiterIdx].transform.position = overlayTile.transform.position;
                    if (overlayTile.GetComponent<OverlayTile>() != null)
                    {
                        overlayTile.GetComponent<OverlayTile>().ShowHighlight();
                    }
                }
            }

        }
    }
    //============================ waiter moving end =================================

    //============================ waiter interacting start =================================
    public sInteractiveObj GetInteractingObj() // target obj
    {
        return overlayTile.GetComponent<sInteractiveObj>();
    }

    

    public void WaiterTask(eIndividualObjs _obj, CharBehaviour _curWaiter) // what to do when the waiter reaches the target
    {
        switch (_obj)
        {
            case eIndividualObjs.waitLine:
                // grab the first customer in line
                if (level.customersInLine.Count >= 1 && !_curWaiter.isCarryingFood && !_curWaiter.isBringingCustomerToTable)
                {
                    _curWaiter.curCustomer = level.customersInLine[0];
                    _curWaiter.curCustomer.cusStatus = eCustomerStatus.toTable;
                    _curWaiter.curCustomer.follower.GetComponent<sInteractiveObj>().cusStatus = eCustomerStatus.toTable;
                    level.customersInLine.RemoveAt(0);
                    _curWaiter.curCustomer.GetComponent<AIPath>().endReachedDistance = 1f;
                    level.UpdateCustomerSetTarget(_curWaiter.curCustomer, 1f, _curWaiter.curCustomer.transform, _curWaiter.transform);
                    // move customers up
                    level.linePosHasCustomer[0] = false;
                    level.MoveCustomerUp();
                    // update waiter bringing customer status: true
                    _curWaiter.isBringingCustomerToTable = true;
                    canvas.UpdateWaiterTabDisplay(_curWaiter.charIdx, eCarrying.customer);

                }
                else
                {
                    Debug.Log("can't do that now");
                }
                break;
            case eIndividualObjs.table0:
            case eIndividualObjs.table1:
            case eIndividualObjs.table2:
            case eIndividualObjs.table3:
            case eIndividualObjs.table4:
            case eIndividualObjs.table5:
                // if bringing food:
                if (_curWaiter.isCarryingFood)
                {
                    // ----update customer status: eating
                    if (_curWaiter.foodIdx == (int)_obj - 1)
                    {
                        _curWaiter.GetComponent<sCharAnimation>().charAnimator.SetBool("hasFood", false);
                        _curWaiter.foodObj.transform.SetParent(level.interactiveObjs[(int)_obj].foodPos.transform);
                        level.interactiveObjs[(int)_obj].tableNumberGFX.SetActive(false);
                        // TODO: enable coffee images

                        level.interactiveObjs[(int)_obj].plate.SetActive(true);
                        level.interactiveObjs[(int)_obj].food = Instantiate(_curWaiter.foodObj, level.interactiveObjs[(int)_obj].foodPos.transform.position, Quaternion.identity, level.interactiveObjs[(int)_obj].foodPos.transform);
                        Destroy(_curWaiter.foodObj);
                        level.interactiveObjs[(int)_obj].CustomerStartEating();
                        _curWaiter.isCarryingFood = false;
                        _curWaiter.tTableNum.gameObject.SetActive(false);
                        canvas.UpdateWaiterTabDisplay(_curWaiter.charIdx, eCarrying.none);
                    }
                }
                // ----update waiter carrying food status: false
                // else if bringing customer:
                else if (_curWaiter.isBringingCustomerToTable && !level.interactiveObjs[(int)_obj].tableTaken)
                {
                    // ----set the customer target to be the seats
                    _curWaiter.curCustomer.GetComponent<AIPath>().endReachedDistance = 0.2f;
                    level.UpdateCustomerSetTarget(_curWaiter.curCustomer, 0.2f, level.interactiveObjs[(int)_obj].cusObjStandPos[0], level.interactiveObjs[(int)_obj].cusObjStandPos[1]);
                    _curWaiter.isBringingCustomerToTable = false;
                    canvas.UpdateWaiterTabDisplay(_curWaiter.charIdx, eCarrying.none);
                    level.interactiveObjs[(int)_obj].tableTaken = true;
                    level.interactiveObjs[(int)_obj].seatedCustomer = _curWaiter.curCustomer;
                    // start cooking food at kitchen
                    level.interactiveObjs[(int)eIndividualObjs.kitchen].CustomerWaitingForFood((int)_obj - 1);

                }
                // ----update customer status: waiting for food
                // ----update waiter bringing customer status: false
                break;
            case eIndividualObjs.kitchen:
                if (!_curWaiter.isCarryingFood && !_curWaiter.isBringingCustomerToTable)
                {
                    for (int i = 0; i < level.interactiveObjs[(int)_obj].orderReady.Length; i++)
                    {
                        if (level.interactiveObjs[(int)_obj].orderReady[i])
                        {
                            level.interactiveObjs[(int)_obj].orderReady[i] = false;
                            _curWaiter.GetComponent<sCharAnimation>().charAnimator.SetBool("hasFood", true);
                            _curWaiter.foodObj = Instantiate(level.pFood[(int)level.chefChar], _curWaiter.foodContainer);
                            _curWaiter.isCarryingFood = true;
                            _curWaiter.foodIdx = i;
                            _curWaiter.tTableNum.text = (i + 1).ToString();
                            _curWaiter.tTableNum.gameObject.SetActive(true);
                            DecorManager.decor.UpdateCounterFoodDisplay(i, false);
                            canvas.UpdateWaiterTabDisplay(_curWaiter.charIdx, eCarrying.food);
                            //canvas.UpdateCookingStatus(i, "ÎÞ¶©µ¥");
                            level.cookingProgressLights[i].color = level.cookingProgress[(int)eCookingStatus.none];
                            break;
                        }
                    }
                }
                // update waiter carrying food status: true
                // update kitchen display
                break;
            case eIndividualObjs.none:
                break;
            default:
                break;
        }
    }

    //============================ waiter interacting end =================================




    private void LateUpdate()
    {
        if (level.levelStarted && !level.levelCompleted)
        {
            ClickOnTile();
        }


    }

    private void Update()
    {
        if (level.levelStarted)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                cInGame.canvas.OnWaiterTabClick(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                cInGame.canvas.OnWaiterTabClick(1);
            }
        }
    }


}
