using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sDecorAnim : MonoBehaviour
{
    
    public void SetBlackCarFalse()
    {
        GetComponent<Animator>().SetBool("isBlackCar", false);
    }

    public void SetRedCarFalse()
    {
        GetComponent<Animator>().SetBool("isRedCar", false);
    }

}
