using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class DialogMessage : MonoBehaviour
{
    UnityAction confirmAction;
    UnityAction cancelAction;

    [Header("UI")]
    public Text messageText;
    public Button cancelButton;


    public void OpenMessage(string msg, UnityAction confirm = null, UnityAction cancel = null)
    {
        messageText.gameObject.SetActive(false);
        messageText.text = string.Format("{0}", msg);

        confirmAction = confirm;
        cancelAction = cancel;

        cancelButton.gameObject.SetActive(confirmAction != null ? true : false);

        messageText.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        if (confirmAction != null)
        {
            confirmAction();
        }

        Close();
    }

    public void Cancel()
    {

        if (cancelAction != null)
        {
            cancelAction();
        }

        Close();
    }
}
