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
    public int maximumDisplayLine = 1000;

    [Header("UI")]
    public Toggle canGenerateStatusToggle;
    public InputField filePathInputField;
    public InputField prefixInputField;
    public Text lineCountText;

    [Header("Preview")]
    public ScrollRect contentPreviewRect;
    public ContentText textContentPrefab;


    public void Initialize()
    {
        canGenerateStatusToggle.isOn = true;
        canGenerateStatusToggle.gameObject.SetActive(canGenerate);

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Text Files", ".txt"));
        FileBrowser.SetDefaultFilter(".txt");
        FileBrowser.SetExcludedExtensions(".jpg", ".png", ".lnk", ".tmp", ".zip", ".rar", ".exe");

        Reset();
    }

    public void Reset()
    {
        lineCountText.text = string.Format("Count: {0}", 0);
        ClearPreviewContainer();
        ClearPrefix();
    }

    public void OnPrefixChanged()
    {
        prefixFileName = prefixInputField.text.Trim();
    }

    /* clear any object in preview rect */
    private void ClearPreviewContainer()
    {
        foreach (Transform a in contentPreviewRect.content)
        {
            Destroy(a.gameObject);
        }
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

        ClearPreviewContainer();

        if (lines.Length > 0)
        {
            int counter = 0;
            foreach (string a in lines)
            {
                /* break loop when instantiate more than maximum lines */
                if (counter >= maximumDisplayLine) break;

                /* create content text object in preview container */
                ContentText contentText = Instantiate(textContentPrefab, contentPreviewRect.content.transform);
                /* set text to display each content line */
                contentText.SetElement(a);
                counter++;
            }
        }else{
            /* do something if can not read data from file */
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
