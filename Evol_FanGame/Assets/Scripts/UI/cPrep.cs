using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cPrep : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject inventoryPanel;
    public GameObject charPanel;
    public Image cakeBG;
    public GameObject cakeTitle;
    public GameObject prepBG;
    

    [Header("Inventory")]
    public TMP_Text tDate;
    public TMP_Text tIncome;
    public TMP_Text tNextGoal;
    [NamedArray(typeof(eIngredient))] public TMP_Text[] tIngredient;

    [Space]
    [Header("Character")]
    [NamedArray(typeof(eChar))] public Button[] bChar;
    [NamedArray(typeof(eChar))] public TMP_Text[] tStamina;
    public GameObject charBodyContainer;
    public TMP_Text tCharDialog;
    public TMP_Text tCharBodyHint;
    //[NamedArray(typeof(eChar))] public GameObject[] pCharBody;
    public Image iChar;
    public GameObject[] iCharCake;
    public Image iIng;
    public Sprite[] spriteChars;
    public Sprite[] spriteIng;


    // 0-1 2-4 5-6
    [TextArea]
    public string[] myText;
    [TextArea]
    public string[] lzyText;
    [TextArea]
    public string[] xmText;
    [TextArea]
    public string[] bqText;
    [TextArea]
    public string[] zqlText;
    [TextArea]
    public string[] lxText;
    string[][] dialogText;

    [Space]
    [Header("Cake")]
    public Image iCake;

    public Transform[] cakeTierPos; // 0: 1 layer, 1: 2 layers, 3: 0 and 3 layers
    public Transform cakeParentTransform;
    public GameObject[] plainCake;
    public GameObject noCake;
    GameObject[][] ingBot;
    GameObject[][] ingMid;
    GameObject[][] ingTop;

    [Space]
    [Header("bot")]
    [Space]
    public GameObject[] decorBot;
    public GameObject[] candyBot;
    public GameObject[] icBot;
    public GameObject[] cookieBot;
    public GameObject[] chocoBot;
    public GameObject[] creamBot;

    [Space]
    [Header("mid")]
    [Space]
    public GameObject[] decorMid;
    public GameObject[] candyMid;
    public GameObject[] icMid;
    public GameObject[] cookieMid;
    public GameObject[] chocoMid;
    public GameObject[] creamMid;

    [Space]
    [Header("top")]
    [Space]
    public GameObject[] decorTop;
    public GameObject[] candyTop;
    public GameObject[] icTop;
    public GameObject[] cookieTop;
    public GameObject[] chocoTop;
    public GameObject[] creamTop;



    [Space]
    [Header("Navigation")]
    public Button bContinue;
    public Button bBackToMain;

    GameManager gm;

    void Start()
    {
        gm = GameManager.gm;
        // check if is the last level
        // if so, check the total income
        // if total income is more than first goal, show confetti
        dialogText = new string[6][];
        dialogText[0] = myText;
        dialogText[1] = lzyText;
        dialogText[2] = xmText;
        dialogText[3] = bqText;
        dialogText[4] = zqlText;
        dialogText[5] = lxText;

        InitIngredientArrays();
        if (gm.progress <= 10)
        {
            InitBtn();
            InitData();
            InitLargeCake();
        }
        else
        {
            inventoryPanel.SetActive(false);
            charPanel.SetActive(false);
            cakeBG.enabled = false;
            cakeTitle.SetActive(false);
            FindObjectOfType<cEnding>().endingTextFrame.SetActive(true);
            FindObjectOfType<cEnding>().tableImage.SetActive(true);
            prepBG.SetActive(false);
            // TODO: shoot confetti
            if (gm.CakeLayer() != 0)
            {
                FindObjectOfType<cEnding>().particles.SetActive(true);
                // play sound?
            }

            InitLargeCake();
            bContinue.gameObject.SetActive(false);

        }
    }

    void InitIngredientArrays()
    {
        ingBot = new GameObject[6][];
        ingMid = new GameObject[6][];
        ingTop = new GameObject[6][];

        ingBot[0] = decorBot;
        ingBot[1] = candyBot;
        ingBot[4] = chocoBot;
        ingBot[5] = creamBot;

        ingMid[0] = decorMid;
        ingMid[1] = candyMid;
        ingMid[4] = chocoMid;
        ingMid[5] = creamMid;

        ingTop[0] = decorTop;
        ingTop[1] = candyTop;
        ingTop[2] = icTop;
        ingTop[3] = cookieTop;
        ingTop[4] = chocoTop;
        ingTop[5] = creamTop;

    }

    void InitData()
    {
        tDate.text = (gm.progress - 1).ToString();
        tIncome.text = gm.money.ToString();
        tCharDialog.text = "";
        InitMoneyGoal();
        for (int i = 0; i < tIngredient.Length; i++)
        {
            tIngredient[i].text = gm.ingredientAmt[i].ToString();
        }
        for (int i = 0; i < tStamina.Length; i++)
        {
            tStamina[i].text = gm.stamina[i].ToString() + "/" + gm.originalStamina.ToString();
        }
    }

    void InitMoneyGoal()
    {
        switch (gm.CakeLayer())
        {
            case 0:
                tNextGoal.text = gm.moneyGoals[0].ToString();
                break;
            case 1:
                tNextGoal.text = gm.moneyGoals[1].ToString();
                break;
            case 2:
                tNextGoal.text = gm.moneyGoals[2].ToString();
                break;
            case 3:
                tNextGoal.text = "已完成目标";
                break;
            default:
                break;
        }
    }

    void InitLargeCake()
    {
        int cakeLayer = gm.CakeLayer();
        // 0 layer stay the same as 3 layer
        switch (cakeLayer)
        {
            case 0:
            case 3:
                // set the position of the parent object
                cakeParentTransform.position = cakeTierPos[2].position;
                if (cakeLayer == 0)
                {
                    noCake.SetActive(true);
                    if (gm.progress == 11)
                    {
                        noCake.GetComponentInChildren<TMP_Text>().gameObject.SetActive(false);
                    }
                    //also display some text telling the player to earn more money
                }
                else
                {
                    for (int i = 0; i < plainCake.Length; i++)
                    {
                        plainCake[i].SetActive(true);
                    }
                }
                break;
            case 1:
                // set the position of the parent object
                cakeParentTransform.position = cakeTierPos[0].position;
                for (int i = 0; i < cakeLayer; i++)
                {
                    plainCake[i].SetActive(true);
                }
                break;
            case 2:
                // set the position of the parent object
                cakeParentTransform.position = cakeTierPos[1].position;
                for (int i = 0; i < cakeLayer; i++)
                {
                    plainCake[i].SetActive(true);
                }
                break;

            default:
                break;
        }

        DisplayLayerIng(cakeLayer);
    }


    // _layerNum:
    // 0: no cake
    // 1: layer 1
    // 2: layer 2
    // 3: layer 3
    void DisplayLayerIng(int _layerNum) // only shows the specific layer's ingredient
    {
        int[] ingAmt = gm.ingredientAmt;
        for (int j = 0; j < _layerNum; j++)
        {
            plainCake[j].SetActive(true);
            for (int i = 0; i < ingAmt.Length; i++) // i: ing type, ingAmt[i]: amount of this type
            {
                switch (ingAmt[i])
                {
                    case 0: // if no this type of ing
                        break;
                    case 1: // if 1 of these types
                    case 2:
                    case 3:
                        Debug.Log("layer: " + j + ", ing: " + (eIngredient)i + ", amt: " + ingAmt[i]);
                        ShowIndiLayer(j + 1, i, ingAmt[i]);
                        break;
                    default: // more than 3
                        ShowIndiLayer(j + 1, i, 3);
                        break;
                }
            }

        }
    }

    void ShowIndiLayer(int _layerNum, int _ingIdx, int _ingAmt)
    {
        switch (_layerNum)
        {
            case 1:
                for (int i = 0; i < _ingAmt; i++)
                {
                    if (_ingIdx != (int)eIngredient.decor)
                    {
                        ingTop[_ingIdx][i].SetActive(true);
                    }
                    else
                    {
                        if (_ingAmt >= 1 && _ingAmt < 3)
                        {
                            ingTop[_ingIdx][0].SetActive(true);
                        }
                        else if (_ingAmt >= 3)
                        {
                            ingTop[_ingIdx][0].SetActive(true);
                            ingTop[_ingIdx][1].SetActive(true);

                        }
                    }
                }
                break;
            case 2:
                if (_ingIdx != (int)eIngredient.iceCream && _ingIdx != (int)eIngredient.cookie)
                {
                    for (int i = 0; i < _ingAmt; i++)
                    {
                        if (_ingIdx != (int)eIngredient.decor)
                        {
                            ingMid[_ingIdx][i].SetActive(true);
                        }
                        else
                        {
                            if (_ingAmt >= 3)
                            {
                                ingMid[_ingIdx][0].SetActive(true);
                            }
                        }
                    }
                }
                break;
            case 3:
                if (_ingIdx != (int)eIngredient.iceCream && _ingIdx != (int)eIngredient.cookie)
                {
                    for (int i = 0; i < _ingAmt; i++)
                    {
                        if (_ingIdx != (int)eIngredient.decor)
                        {
                            ingBot[_ingIdx][i].SetActive(true);
                        }
                        else
                        {
                            if (_ingAmt <= 2)
                            {
                                ingBot[_ingIdx][0].SetActive(true);
                            }
                            else
                            {
                                ingBot[_ingIdx][0].SetActive(true);
                                ingBot[_ingIdx][1].SetActive(true);

                            }
                        }
                    }
                }

                break;
            default:
                break;
        }
    }

    void InitBtn()
    {
        bChar[0].onClick.AddListener(delegate { OnCharClick(0); });
        bChar[1].onClick.AddListener(delegate { OnCharClick(1); });
        bChar[2].onClick.AddListener(delegate { OnCharClick(2); });
        bChar[3].onClick.AddListener(delegate { OnCharClick(3); });
        bChar[4].onClick.AddListener(delegate { OnCharClick(4); });
        bChar[5].onClick.AddListener(delegate { OnCharClick(5); });
        bContinue.onClick.AddListener(OnContinueClick);
        if (gm.progress != 11)
        {
            bBackToMain.onClick.AddListener(OnBackToMainClick);
        }
    }

    void OnCharClick(int _charIdx)
    {
        // update and show char image
        iChar.sprite = spriteChars[_charIdx];
        iChar.gameObject.SetActive(true);

        // hide hint text
        if (tCharBodyHint.gameObject.activeSelf)
        {
            tCharBodyHint.gameObject.SetActive(false);
        }

        // update and show char cake and ingredient
        for (int i = 0; i < iCharCake.Length; i++)
        {
            iCharCake[i].SetActive(false);
        }
        iCharCake[_charIdx].SetActive(true);
        iIng.sprite = spriteIng[_charIdx];
        iIng.gameObject.SetActive(true);

        //charBody = Instantiate(pCharBody[_charIdx], charBodyContainer.transform);
        switch (gm.stamina[_charIdx])
        {
            case 0:
            case 1:
                tCharDialog.text = dialogText[_charIdx][0];
                break;
            case 2:
            case 3:
            case 4:
                tCharDialog.text = dialogText[_charIdx][1];
                break;
            case 5:
            case 6:
                tCharDialog.text = dialogText[_charIdx][2];
                break;
            default:
                break;
        }
    }

    void OnContinueClick()
    {
        gm.LoadScene(1);
    }

    void OnBackToMainClick()
    {
        gm.LoadScene(0);

    }
}