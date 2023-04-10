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
    public DialogMessage dialogMessage;

    [Header("Panel")]
    public GameObject singlePanel;
    public GameObject multiPanel;

    [Header("Properies")]
    public string savePath;
    public string currentFileName;
    public bool isProcessing = false;
    public bool onGenerate = false;


    [Header("UI")]
    public Dropdown dropdownMode;
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
        singleMode.m_OnSingleCodeCangenerate += OnSingleCodeCangenerate;

        if (dialogMessage != null) dialogMessage.Close();

        Reset();
        OnChangeMode();
    }

    public void QuitApp()
    {
        dialogMessage.OpenMessage("Do you want to quit?", () =>
        {
            Application.Quit();
        });

    }


    public void OnChangeMode()
    {
        if (dropdownMode.value == 0)
        {
            mode = MODE.SINGLE;

        }

        if (dropdownMode.value == 1)
        {
            mode = MODE.MULTI;
        }

        singleMode.gameObject.SetActive(mode == MODE.SINGLE);
        multipleMode.gameObject.SetActive(mode == MODE.MULTI);
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
            if (multipleMode.contentList.Count == 0)
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

        currentFileName = string.Empty;

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
        isProcessing = true;
        onGenerate = true;

        yield return null;

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

        int code = qrCodeEncodeController.Encode(singleMode.text);
        yield return new WaitUntil(() => onGenerate == false);
        Debug.Log(code);
        
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

        int maxContent = multipleMode.contentList.Count;
        int currentIndex = 0;

        while (currentIndex < maxContent)
        {
            onGenerate = true;
            currentFileName = multipleMode.fileNameList[currentIndex] + ".png";
            qrCodeEncodeController.Encode(multipleMode.contentList[currentIndex]);


            yield return new WaitUntil(() => onGenerate == false);
            currentIndex++;
            progressText.text = string.Format("{0}/{1}", currentIndex, maxContent);
            progressBarImage.fillAmount = (float)currentIndex / (float)maxContent;
        }

        isProcessing = false;
        resetButton.interactable = true;
    }

    private void OnEncoded(Texture2D texture)
    {
        string fullPath = savePath + "/" + currentFileName;
        Texture2D rotatedTexture = RotateTexture(texture, true);
        byte[] bytes = rotatedTexture.EncodeToPNG();
        qrPreview.texture = rotatedTexture;

        File.WriteAllBytes(fullPath, bytes);
        fullPathText.text = string.Format("Path: {0}", fullPath);

        onGenerate = false;
    }

    /* http://answers.unity.com/answers/1401997/view.html */
    Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
}
