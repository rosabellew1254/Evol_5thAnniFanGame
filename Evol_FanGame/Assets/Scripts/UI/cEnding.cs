using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cEnding : MonoBehaviour
{
    public GameObject endingTextFrame;
    public TMP_Text caption;
    public string[] cakeTextContent;
    public TMP_Text tLoading;
    public Button bBackToMain;
    public GameObject tableImage;
    public GameObject particles;

    GameManager gm;

    void Start()
    {
        gm = GameManager.gm;
        if (gm.progress == 11)
        {
            bBackToMain.onClick.AddListener(OnBackToMainClick);
            Debug.Log("added listener on cEnding");
            InitUI();

        }
    }

    void InitUI()
    {
        caption.text = cakeTextContent[gm.CakeLayer()];
    }

    void OnBackToMainClick()
    {
        StartCoroutine(ResetData());
    }

    IEnumerator ResetData()
    {
        tLoading.gameObject.SetActive(true);
        FindObjectOfType<PlayerData>().ResetGameData();
        FindObjectOfType<cPrep>().bBackToMain.gameObject.SetActive(false);
        Debug.Log("new game: " + FindObjectOfType<PlayerData>().progData.isNewGame);
        yield return new WaitForSeconds(1f);
        gm.LoadScene(0);
    }

}
