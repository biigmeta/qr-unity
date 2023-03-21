using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using SimpleFileBrowser;
public class Generator : MonoBehaviour
{
    /* https://github.com/yasirkula/UnitySimpleFileBrowser */

    public enum MODE { SINGLE, MULTI }
    public MODE mode = MODE.SINGLE;
    public Text modeText;

    [Header("Script Component")]
    public SingleMode singleMode;
    public MultipleMode multipleMode;

    [Header("Panel")]
    public GameObject singlePanel;
    public GameObject multiPanel;


    [Header("Properies")]
    public string savePath;
    public bool isProcessing = false;
    public bool onGenerate = false;

    [Header("UI")]

    public InputField savePathInputField;
    public RawImage qrPreview;
    public Text progressText;
    public Text currentFileNameText;
    public Button generateQRCodeButton;

    [Header("Component")]
    public QRCodeEncodeController qrCodeEncodeController;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        qrCodeEncodeController.onQREncodeFinished += OnEncoded;
        generateQRCodeButton.interactable = false;

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Text Files", ".txt"));
        FileBrowser.SetDefaultFilter(".txt");
        FileBrowser.SetExcludedExtensions(".jpg", ".png", ".lnk", ".tmp", ".zip", ".rar", ".exe");


        singleMode.Initialize();
        singleMode.m_OnInputFieldChanged += OnSingleModeInputChanged;

        multipleMode.Initialize();

        ChangeMode("single");
    }


    public void ChangeMode(string mode)
    {
        this.mode = mode == "single" ? MODE.SINGLE : MODE.MULTI;
        singlePanel.SetActive(this.mode == MODE.SINGLE);
        multiPanel.SetActive(this.mode == MODE.MULTI);

        /* display mode */
        modeText.text = string.Format("{0} MODE", this.mode.ToString());
    }

    private void OnSingleModeInputChanged(string text)
    {
        if (mode != MODE.SINGLE) return;

        /* to check generate wording is not empty */
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(savePath))
        {
            generateQRCodeButton.interactable = false;
            return;
        }

        generateQRCodeButton.interactable = true;
    }

    public void SelectSavePath()
    {
        StartCoroutine(SelectSavePathCoroutine());
    }

    public void OpenSavePath()
    {
        Application.OpenURL(savePath);
    }

    IEnumerator SelectSavePathCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, true, null, null, "Select Directory", "Select");

        if (FileBrowser.Success)
        {
            if (FileBrowser.Result.Length > 0)
            {
                savePath = FileBrowser.Result[0];
            }
        }
        else
        {
            savePath = string.Empty;
        }

        DisplaySavePath();
    }



    public void DisplaySavePath()
    {
        savePathInputField.text = string.Format("{0}", savePath);

        if (string.IsNullOrEmpty(savePath))
        {
            generateQRCodeButton.interactable = false;
            return;
        }

        /* check can generate */
        if (mode == MODE.SINGLE)
        {
            if (string.IsNullOrEmpty(singleMode.text))
            {
                generateQRCodeButton.interactable = false;
                return;
            }
        }

        if (mode == MODE.MULTI)
        {
            if (multipleMode.wordingToGenerate.Length == 0)
            {
                generateQRCodeButton.interactable = false;
                return;
            }
        }

        generateQRCodeButton.interactable = true;
    }


    public void StartGenerate()
    {
        if (!isProcessing)
        {
            if (mode == MODE.SINGLE)
            {
                StartCoroutine(Co_GenerateSingle());
            }

            if (mode == MODE.MULTI)
            {
                StartCoroutine(Co_GenerateMultiple());
            }
        }
    }

    IEnumerator Co_GenerateSingle()
    {
        yield return null;
        isProcessing = true;

        // currentNumber = startNumber;

        // while (currentNumber <= endNumber)
        // {
        //     onGenerate = true;
        //     currentFileName = currentNumber + ".png";
        //     string content = "https://acmecs-businessmatching.com/match/?uid=" + currentNumber + "&openExternalBrowser=1";
        //     qrCodeEncodeController.Encode(content);
        //     currentNumber++;
        //     yield return new WaitUntil(() => onGenerate == false);
        // }

        isProcessing = false;
    }

    IEnumerator Co_GenerateMultiple()
    {
        yield return null;
        isProcessing = true;

        // currentNumber = startNumber;

        // while (currentNumber <= endNumber)
        // {
        //     onGenerate = true;
        //     currentFileName = currentNumber + ".png";
        //     string content = "https://acmecs-businessmatching.com/match/?uid=" + currentNumber + "&openExternalBrowser=1";
        //     qrCodeEncodeController.Encode(content);
        //     currentNumber++;
        //     yield return new WaitUntil(() => onGenerate == false);
        // }

        isProcessing = false;
    }

    private void OnEncoded(Texture2D texture)
    {
        // byte[] bytes = texture.EncodeToPNG();
        // string path = savePath + currentFileName;
        // File.WriteAllBytes(path, bytes);
        // onGenerate = false;
    }
}
