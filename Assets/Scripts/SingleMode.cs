using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SingleMode : MonoBehaviour
{
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

        ClearText();
        ClearFileName();
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

    public void ClearText()
    {
        text = string.Empty;
        wordingInputField.text = text;
    }

    public void OnFileNameChanged()
    {
        fileName = fileNameInputField.text.Trim();
        CheckCanGenerateCode();
    }

    public void ClearFileName()
    {
        fileName = string.Empty;
        fileNameInputField.text = fileName;
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
