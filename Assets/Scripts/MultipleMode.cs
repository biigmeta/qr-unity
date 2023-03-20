using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.UI;

public class MultipleMode : MonoBehaviour
{
    [Header("Properties")]
    public string filePath;
    public string[] wordingToGenerate;

    [Header("UI")]
    public InputField filePathInputField;
    public Text lineCountText;
    public Button previewContentButton;


    public void Initialize()
    {
        previewContentButton.interactable = false;
    }

    public void SelectFilePath()
    {
        StartCoroutine(SelectFilePathCoroutine());
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

    private void ReadFile()
    {
        string[] lines = File.ReadAllLines(filePath);
        lineCountText.text = string.Format("Line count: {0}", lines.Length);
        
        if(lines.Length > 0)
        {
            previewContentButton.interactable = true;
        }
    }

    public void DisplayPath()
    {
        filePathInputField.text = string.Format("{0}", filePath);
    }


    IEnumerator CopyFile(string path)
    {
        byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(path);
        yield return null;

        string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(path));
        FileBrowserHelpers.CopyFile(path, destinationPath);
        Debug.Log(destinationPath);
    }
}
