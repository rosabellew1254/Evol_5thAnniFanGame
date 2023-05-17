using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Button bPrev;
    public Button bNext;
    public Button bClose;

    public GameObject[] tutorialPages;

    int curPage;

    private void Start()
    {
        curPage = 0;
        bPrev.onClick.AddListener(OnPrevClick);
        bNext.onClick.AddListener(OnNextClick);
        bClose.onClick.AddListener(OnCloseClick);
        tutorialPages[0].SetActive(true);
        bPrev.gameObject.SetActive(false);
    }


    void OnPrevClick()
    {
        tutorialPages[curPage].SetActive(false);
        curPage--;
        tutorialPages[curPage].SetActive(true);
        if (curPage == 0)
        {
            bPrev.gameObject.SetActive(false);
        }
        if (!bNext.gameObject.activeSelf)
        {
            bNext.gameObject.SetActive(true);
        }
    }

    void OnNextClick()
    {
        tutorialPages[curPage].SetActive(false);
        curPage++;
        tutorialPages[curPage].SetActive(true);

        if (curPage == tutorialPages.Length - 1)
        {
            bNext.gameObject.SetActive(false);
        }
        if (!bPrev.gameObject.activeSelf)
        {
            bPrev.gameObject.SetActive(true);
        }

    }

    void OnCloseClick()
    {
        if (LevelManager.level != null)
        {
            LevelManager.level.tutorialFinished = true;
            GameManager.gm.haveReadTutorial = true;
            FindObjectOfType<PlayerData>().progData.haveReadTutorial = GameManager.gm.haveReadTutorial;
            FindObjectOfType<PlayerData>().SaveProgress();
        }
        Destroy(gameObject);
    }


}
