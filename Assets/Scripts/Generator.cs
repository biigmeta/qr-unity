using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using SimpleFileBrowser;
public class Generator : MonoBehaviour
{
    /* https://github.com/yasirkula/UnitySimpleFileBrowser */

    [Header("Properies")]
    public string filePath;
    public string savePath;
    public string prefixFileName;
    public bool isProcessing = false;
    public bool onGenerate = false;

    [Header("UI")]
    public InputField filePathInputField;
    public InputField savePathInputField;
    public InputField prefixInputField;

    /*file detail*/
    public Text lineCountText;
    public Text firstLineContentText;
    public Text lastLineContentText;
    public Text exampleFileNameText;
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

        /* reset line count text */
        lineCountText.text = string.Format("Line count: {0}", "-");
    }

    public void OnPrefixChanged()
    {
        prefixFileName = prefixInputField.text.Trim();
    }

    public void SelectFilePath()
    {
        StartCoroutine(SelectFilePathCoroutine());
    }

    public void SelectSavePath()
    {
        StartCoroutine(SelectSavePathCoroutine());
    }

    IEnumerator SelectFilePathCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Select Text File", "Select");

        if (FileBrowser.Success)
        {
            if (FileBrowser.Result.Length > 0)
            {
                filePath = FileBrowser.Result[0];
                ReadFile();
            }
        }
        else
        {
            filePath = string.Empty;
        }

        DisplayPath();
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

        DisplayPath();
    }

    IEnumerator CopyFile(string path)
    {
        byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(path);
        yield return null;

        string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(path));
        FileBrowserHelpers.CopyFile(path, destinationPath);
        Debug.Log(destinationPath);
    }

    private void DisplayPath()
    {
        filePathInputField.text = string.Format("{0}", filePath);
        savePathInputField.text = string.Format("{0}", savePath);

        /* set line count to - if file path is empty */
        if (string.IsNullOrEmpty(filePath))
        {
            lineCountText.text = string.Format("Line count: {0}", "-");
        }


        if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(savePath))
        {
            generateQRCodeButton.interactable = true;
        }
        else
        {
            generateQRCodeButton.interactable = false;
        }
    }

    private void ReadFile()
    {
        string[] lines = File.ReadAllLines(filePath);
        lineCountText.text = string.Format("Line count: {0}", lines.Length);
        firstLineContentText.text = string.Format("First line content: {0}", lines[0]);
        lastLineContentText.text = string.Format("Last line content: {0}", lines[lines.Length - 1]);
    }

    public void StartGenerate()
    {
        if (!isProcessing)
        {
            StartCoroutine(Co_Generate());
        }
    }

    IEnumerator Co_Generate()
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
