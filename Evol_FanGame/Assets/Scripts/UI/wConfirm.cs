using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class wConfirm : MonoBehaviour
{
    public TMP_Text tTitle;
    public TMP_Text tMessage;
    string action;
    GameObject sendObject;
    public Button bConfirm;
    public Button bCancel;

    public void InitUI(string _tTitle, string _tMessage, string _action, GameObject _sendObject)
    {
        tTitle.text = _tTitle;
        tMessage.text = _tMessage;
        action = _action;
        sendObject = _sendObject;
        bConfirm.onClick.AddListener(OnConfirmClick);
        bCancel.onClick.AddListener(OnCancelClick);
    }

    void OnCancelClick()
    {
        Destroy(gameObject);
    }

    void OnConfirmClick()
    {
        sendObject.SendMessage(action);
        Destroy(gameObject);
    }
}