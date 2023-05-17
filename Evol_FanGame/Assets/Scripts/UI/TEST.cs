using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TEST : MonoBehaviour
{
    public TMP_Text height;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            height.text = GetComponent<RectTransform>().rect.height.ToString();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(100, 500);
            height.text = GetComponent<RectTransform>().rect.height.ToString();
        }
    }

}
