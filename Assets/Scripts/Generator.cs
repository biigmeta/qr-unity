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
    public string currentFileName;
    public bool isProcessing = false;
    public bool onGenerate = false;

    [Header("UI")]

    public InputField savePathInputField;
    public RawImage qrPreview;
    public Text progressText;
    public Text currentFileNameText;
    public Text fullPathText;
    public Image progressBarImage;


    public Button generateQRCodeButton;
    public Button resetButton;

    [Header("Component")]
    public QRCodeEncodeController qrCodeEncodeController;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {

        /* add event listener  ** do this once ** */
        qrCodeEncodeController.onQREncodeFinished += OnEncoded;
        // singleMode.m_OnInputFieldChanged += OnSingleModeInputChanged;
        singleMode.m_OnSingleCodeCangenerate += OnSingleCodeCangenerate;

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Text Files", ".txt"));
        FileBrowser.SetDefaultFilter(".txt");
        FileBrowser.SetExcludedExtensions(".jpg", ".png", ".lnk", ".tmp", ".zip", ".rar", ".exe");

        Reset();

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

    private void OnSingleCodeCangenerate(bool canGenerate)
    {
        /* to check generate wording is not empty */
        if (string.IsNullOrEmpty(savePath))
        {
            generateQRCodeButton.interactable = false;
            return;
        }

        generateQRCodeButton.interactable = canGenerate;
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

    public void Reset()
    {
        singleMode.Initialize();
        multipleMode.Initialize();

        progressBarImage.fillAmount = 0;
        progressText.text = string.Format("{0}/{1}", 0, 0);
        currentFileNameText.text = string.Format("File name: ", "-");
        fullPathText.text = string.Format("Path: ", "-");
        qrPreview.texture = new Texture2D(1, 1);

        generateQRCodeButton.interactable = false;
        resetButton.interactable = false;
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
        Debug.Log("start generate");
        yield return null;
        isProcessing = true;
        onGenerate = true;

        int maxContent = 1;
        int currentIndex = 0;

        progressText.text = string.Format("{0}/{1}", currentIndex, maxContent);

        if (!string.IsNullOrEmpty(singleMode.fileName))
        {
            currentFileName = singleMode.fileName + ".png";
        }
        else
        {
            currentFileName = System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
        }

        currentFileNameText.text = string.Format("File name: {0}", currentFileName);

        qrCodeEncodeController.Encode(singleMode.text);
        yield return new WaitUntil(() => onGenerate == false);

        currentIndex++;
        progressText.text = string.Format("{0}/{1}", currentIndex, maxContent);
        progressBarImage.fillAmount = (float)currentIndex / (float)maxContent;

        isProcessing = false;
        resetButton.interactable = true;
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
        resetButton.interactable = true;
    }

    private void OnEncoded(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        string fullPath = savePath + "/" + currentFileName;

        qrPreview.texture = texture;

        File.WriteAllBytes(fullPath, bytes);
        fullPathText.text = string.Format("Path: {0}", fullPath);

        onGenerate = false;
    }
}
