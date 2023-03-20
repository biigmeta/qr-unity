using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleMode : MonoBehaviour
{
    public delegate void OnInputFieldChangedHandler(string text);
    public OnInputFieldChangedHandler m_OnInputFieldChanged;
    public void Initialize()
    {
        Clear();
    }

    [Header("Single Mode Properties")]
    public string wordingToGenerate;

    [Header("Single Mode UI")]
    public InputField wordingInputField;

    public void OnInputFieldChanged()
    {
        wordingToGenerate = wordingInputField.text.Trim();
        if(m_OnInputFieldChanged != null) m_OnInputFieldChanged(wordingToGenerate);
    }

    /*For single mode*/
    public void Clear()
    {
        wordingToGenerate = string.Empty;
        wordingInputField.text = wordingToGenerate;
    }
}
