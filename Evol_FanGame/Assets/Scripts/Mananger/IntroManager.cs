using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public Button bSkip;
    public Button bContinue;
    public GameObject introBG;
    Animator introAnimator;

    public TMP_Text introText;

    [Space]
    [Header("Type Writer")]
    [Space]
    public float timeBtwCharacters;
    public string content;



    private void Start()
    {
        introAnimator = GetComponent<Animator>();
        bSkip.onClick.AddListener(ReadyToStart);
        bContinue.onClick.AddListener(OnContinueClick);
        if (introText != null)
        {
            content = introText.text;
            introText.text = "";
            StartCoroutine(TypeText());
        }
    }

    IEnumerator TypeText()
    {
        yield return new WaitForSeconds(0.6f);

        foreach (char c in content)
        {
            introText.text += c;
            if (c == '\n')
            {
                Debug.Log("has return");
                yield return new WaitForSeconds(0.2f);
            }
            if (c != '\n'/* && c != '£¬' && c != '¡£'*/)
            {
                AudioManager.am.PlaySFX(eSFX.typing);
            }
            yield return new WaitForSeconds(timeBtwCharacters);
        }
        yield return new WaitForSeconds(1f);
        bContinue.gameObject.SetActive(true);
    }

    public void ReadyToStart()
    {
        FindObjectOfType<LevelManager>().introFinished = true;
    }

    void OnContinueClick()
    {
        introBG.SetActive(false);
        introAnimator.enabled = true;
    }


}
