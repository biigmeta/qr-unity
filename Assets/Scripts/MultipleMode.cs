using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.UI;

public class MultipleMode : MonoBehaviour
{
    [Header("Properties")]
    public bool canGenerate = false;
    public string filePath;
    public string[] wordingToGenerate;
    public string prefixFileName;

    [Header("UI")]
    public Toggle canGenerateStatusToggle;
    public InputField filePathInputField;
    public InputField prefixInputField;
    public Text lineCountText;


    public void Initialize()
    {
        canGenerateStatusToggle.isOn = true;
        canGenerateStatusToggle.gameObject.SetActive(canGenerate);
        ClearPrefix();
    }

    public void OnPrefixChanged()
    {
        prefixFileName = prefixInputField.text.Trim();
    }

    public void ClearPrefix()
    {
        prefixFileName = string.Empty;
        prefixInputField.text = prefixFileName;
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

        if (lines.Length > 0)
        {

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
