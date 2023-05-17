using Pathfinding;
using System.Collections;
using UnityEngine;

public class sCharAnimation : MonoBehaviour
{

    bool isFlip;
    bool isFront;
    public GameObject charGFX;
    public Animator charAnimator;
    public float distanceToTarget;

    private void Start()
    {
        StartCoroutine(CheckMoving());
    }

    IEnumerator CheckMoving()
    {
        while (LevelManager.level.levelStarted)
        {
            yield return new WaitForSeconds(0.01f);

            // check if gfx matches direction
            CheckGFXMatchDirection();

            // update the moving bool
            if (ReachedEndOfPath())
            {
                if (charAnimator.GetBool("isMoving"))
                {
                    charAnimator.SetBool("isMoving", false);
                }
            }
        }
    }

    public bool ReachedEndOfPath()
    {
        distanceToTarget = Mathf.Abs((GetComponent<AIDestinationSetter>().target.position - transform.position).magnitude);
        if (distanceToTarget <= GetComponent<AIPath>().endReachedDistance + 0.1f)
        {
            return true;
        }
        else return false;
    }

    public void CheckGFXMatchDirection()
    {
        isFront = Mathf.Sign(GetComponent<AIPath>().velocity.y) == -1;
        isFlip = Mathf.Sign(GetComponent<AIPath>().velocity.x) == -1;
        // check flipping and do the flip
        if (GetComponent<AIPath>().velocity.magnitude >= 0.1f)
        {
            if (isFlip && charGFX.transform.localScale.x == 1)
            {
                //charGFX.GetComponent<SpriteRenderer>().flipX = true;
                charGFX.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (!isFlip && charGFX.transform.localScale.x == -1)
            {
                //charGFX.GetComponent<SpriteRenderer>().flipX = false;
                charGFX.transform.localScale = new Vector3(1, 1, 1);
            }
            // check front and back and do the sprite swap
            if (isFront && charAnimator.GetBool("isUp"))
            {
                charAnimator.SetBool("isUp", false);
            }
            else if (!isFront && !charAnimator.GetBool("isUp"))
            {
                charAnimator.SetBool("isUp", true);
            }

        }
    }
}
