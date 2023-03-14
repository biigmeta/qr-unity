using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using SimpleFileBrowser;
public class Generator : MonoBehaviour
{
    [Header("Properies")]
    public string filePath;
    public bool isProcessing = false;
    public bool onGenerate = false;

    [Header("UI")]
    public InputField filePathInputField;

    [Header("Component")]
    public QRCodeEncodeController qrCodeEncodeController;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        qrCodeEncodeController.onQREncodeFinished += OnEncoded;
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Text Files", ".txt"));
        FileBrowser.SetDefaultFilter(".txt");
        FileBrowser.SetExcludedExtensions(".jpg", ".png", ".lnk", ".tmp", ".zip", ".rar", ".exe");
    }

    public void SelectFile()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: both, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, null, null, "Select Text File", "Select");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            if (FileBrowser.Result.Length > 0)
            {
                string path = FileBrowser.Result[0];
                 DisplayFilePath(path);
            }
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            // for (int i = 0; i < FileBrowser.Result.Length; i++)
            //     Debug.Log(FileBrowser.Result[i]);

            // Read the bytes of the first file via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            // byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);

            // Or, copy the first file to persistentDataPath
            // string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(FileBrowser.Result[0]));
            // FileBrowserHelpers.CopyFile(FileBrowser.Result[0], destinationPath);
            // Debug.Log(destinationPath);
        }
        else
        {
            DisplayFilePath(string.Empty);
        }
    }

    private void DisplayFilePath(string path)
    {
        filePathInputField.text = string.Format("{0}", path);
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
