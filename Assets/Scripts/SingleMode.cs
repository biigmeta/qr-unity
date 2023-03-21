using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System;

public class SingleMode : MonoBehaviour
{
    public delegate void OnInputFieldChangedHandler(string text);
    public OnInputFieldChangedHandler m_OnInputFieldChanged;

    public delegate void OnSingleCodeCanGenerateHandler(bool canGenerate);
    public OnSingleCodeCanGenerateHandler m_OnSingleCodeCangenerate;

    [Header("Properties")]
    public bool canGenerate = false;
    public bool isURI = false;
    public string text;
    public string fileName;

    [Header("UI")]
    public Toggle canGenerateStatusToggle;
    public InputField wordingInputField;
    public InputField fileNameInputField;
    public Text textTypeText;

    public void Initialize()
    {
        isURI = false;
        canGenerate = false;
        textTypeText.text = string.Format("{0}", isURI ? "** This text is uri. You must provide filename before generate QR Code." : "");

        canGenerateStatusToggle.isOn = true;
        canGenerateStatusToggle.gameObject.SetActive(canGenerate);
        Clear();
    }
    public void OnInputFieldChanged()
    {
        text = wordingInputField.text.Trim();

        /* prevent error on typing */
        try
        {
            Uri uriChecker = new Uri(text);
            isURI = true;
        }
        catch
        {
            isURI = false;
        }

        textTypeText.text = string.Format("{0}", isURI ? "** This text is uri. You must provide filename before generate QR Code." : "");

        CheckCanGenerateCode();
    }

    public void OnFileNameChanged()
    {
        fileName = wordingInputField.text.Trim();
        CheckCanGenerateCode();
    }

    /*For single mode*/
    public void Clear()
    {
        text = string.Empty;
        wordingInputField.text = text;
    }

    private void CheckCanGenerateCode()
    {

        if (string.IsNullOrEmpty(text))
        {
            canGenerate = false;
            canGenerateStatusToggle.gameObject.SetActive(canGenerate);

            if (m_OnSingleCodeCangenerate != null) m_OnSingleCodeCangenerate(canGenerate);
            return;
        }

        /* can not set url as filename */
        if (isURI && string.IsNullOrEmpty(fileName))
        {

            canGenerate = false;
            canGenerateStatusToggle.gameObject.SetActive(canGenerate);

            if (m_OnSingleCodeCangenerate != null) m_OnSingleCodeCangenerate(canGenerate);
            return;
        }



        canGenerate = true;
        canGenerateStatusToggle.gameObject.SetActive(canGenerate);

        if (m_OnSingleCodeCangenerate != null) m_OnSingleCodeCangenerate(canGenerate);
    }
}
