using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorManager : MonoBehaviour
{
    public static DecorManager decor;

    [Header("Neon Sign")]
    [Space]
    [NamedArray(typeof(eChar))] public Color[] neonSignColor;
    [NamedArray(typeof(eChar))] public Color[] neonSignHaloColor;

    public SpriteRenderer neonSignExpand;
    public SpriteRenderer neonSignHalo;
    public float signColorSwitchTime;

    [Space]
    [Header("Window")]
    [Space]

    public GameObject window;

    [Space]
    [Header("Menu Sign")]
    [Space]
    public SpriteRenderer menuBoardInfo;
    [NamedArray(typeof(eChar))] public Sprite[] menuBoardContent;

    [Space]
    [Header("Counter")]
    [Space]
    public SpriteRenderer[] counterFood;
    public SpriteRenderer[] counterPlate;
    public Sprite[] spriteCake;
    public SpriteRenderer[] displayCake;

    LevelManager level;



    private void Start()
    {
        decor = this;
        level = LevelManager.level;
        neonSignExpand.color = Color.clear;
        neonSignHalo.color = Color.clear;

        StartCoroutine(ShowCar());
    }

    public void InitCounterFood()
    {
        for (int i = 0; i < counterFood.Length; i++)
        {
            counterFood[i].sprite = spriteCake[(int)level.chefChar];
            counterFood[i].enabled = false;
            counterPlate[i].enabled = false;
        }
        for (int i = 0; i < displayCake.Length; i++)
        {
            if (i < 2)
            {
                displayCake[i].sprite = spriteCake[(int)level.chefChar];
            }
            else
            {
                displayCake[i].sprite = spriteCake[(int)level.exploreChar];    
            }
            displayCake[i].enabled = true;
        }
    }


    public void SetNeonSignColor(eChar _waiter0, eChar _waiter1)
    {
        StartCoroutine(FlashNeonSign(_waiter0, _waiter1));
    }

    public void SetMenuSignContent(eChar _chef)
    {
        menuBoardInfo.sprite = menuBoardContent[(int)_chef];
    }

    IEnumerator FlashNeonSign(eChar _waiter0, eChar _waiter1)
    {
        while (level.levelStarted)
        {
            neonSignExpand.color = neonSignColor[(int)_waiter0];
            neonSignHalo.color = neonSignHaloColor[(int)_waiter0];
            yield return new WaitForSeconds(signColorSwitchTime);
            neonSignExpand.color = neonSignColor[(int)_waiter1];
            neonSignHalo.color = neonSignHaloColor[(int)_waiter1];
            yield return new WaitForSeconds(signColorSwitchTime);
        }
    }

    IEnumerator ShowCar()
    {
        Animator windowAnim = window.GetComponent<Animator>();
        while (true)
        {
            yield return new WaitForSeconds(3f);
            if (!windowAnim.GetBool("isRedCar") && !windowAnim.GetBool("isBlackCar"))
            {
                int randNum = Random.Range(0, 4);
                switch (randNum)
                {
                    case 2:
                        windowAnim.SetBool("isRedCar", true);
                        break;
                    case 3:
                        windowAnim.SetBool("isBlackCar", true);
                        break;
                    case 0:
                    case 1:
                    default:
                        break;
                }
            }

        }
    }

    public void UpdateCounterFoodDisplay(int _tableIdx, bool _turnOn)
    {
        counterFood[_tableIdx].enabled = _turnOn;
        counterPlate[_tableIdx].enabled = _turnOn;
    }

}
