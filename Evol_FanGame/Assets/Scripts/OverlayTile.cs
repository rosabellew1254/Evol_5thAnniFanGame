using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{


    void Update()
    {
        MouseClick();
    }

    void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideHighlight();
        }
    }

    public void ShowHighlight()
    {
        GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void HideHighlight()
    {
        GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

}
