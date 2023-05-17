using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class wSummary : MonoBehaviour
{
    GameManager gm;
    LevelManager level;

    [Header("Goal Gauge")]
    public GameObject gaugeBG;
    public GameObject gaugeMask;
    public GameObject[] gaugeGoals;
    public float[] goals;

    // summary data
    int incomeCurTurn;
    int incomeTotal;
    int customersServed;
    int[] ingredientEarned;

    float[] goalPercent;

    [Space]
    [Header("Summary Display")]
    public TMP_Text tIncomeCurTurn;
    public TMP_Text tIncomeTotal;
    public TMP_Text tCustomersServed;
    public TMP_Text[] tIngredientEarned;
    public TMP_Text[] tIngredientTotal;
    public TMP_Text[] tMoneyGoals;

    public Button bContinue;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.gm;
        level = LevelManager.level;

        InitUI();

    }

    void InitUI()
    {
        bContinue.onClick.AddListener(OnContinueClick);
        InitNumbers();
        InitTexts();
        InitGauge();

    }

    void InitNumbers()
    {
        incomeCurTurn = level.CalculateMoneyEarned();
        incomeTotal = gm.money;
        customersServed = level.numOfCustomersServed;
        ingredientEarned = level.CalculateCurTurnIngredient();
    }

    void InitTexts()
    {
        tIncomeCurTurn.text = incomeCurTurn.ToString();
        tIncomeTotal.text = incomeTotal.ToString();
        tCustomersServed.text = customersServed.ToString();
        for (int i = 0; i < tIngredientEarned.Length; i++)
        {
            tIngredientEarned[i].text = "(+" + ingredientEarned[i].ToString() + ")";
            tIngredientTotal[i].text = level.startingIngredient[i].ToString();
        }
    }

    void InitGauge()
    {
        float curPercent = 0;

        for (int i = 0; i < tMoneyGoals.Length; i++)
        {
            tMoneyGoals[i].text = gm.moneyGoals[i].ToString();
        }
        switch (gm.CakeLayer())
        {
            case 0:
                curPercent = goals[0] / 100 * gm.money / gm.moneyGoals[0] * 100;
                break;
            case 1:
                curPercent = goals[1] / 100 * (gm.money - gm.moneyGoals[0]) / gm.moneyGoals[1] * 100 + goals[0];
                break;
            case 2:
                curPercent = goals[2] / 100 * (gm.money - gm.moneyGoals[1]) / gm.moneyGoals[2] * 100 + goals[1];
                break;
            case 3:
                curPercent = curPercent > gaugeBG.GetComponent<RectTransform>().rect.height ? gaugeBG.GetComponent<RectTransform>().rect.height : goals[3] / 100 * (gm.money - gm.moneyGoals[2]) / (gm.moneyGoals[2] + 500) * 100 + goals[2];
                break;
            default:
                break;
        }
        gaugeMask.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, curPercent);
    }



    void OnContinueClick()
    {
        gm.LoadScene(2);
    }

}
