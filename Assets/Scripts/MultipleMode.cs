using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.UI;
using System.Linq;

public class MultipleMode : MonoBehaviour
{
    [Header("Properties")]
    public bool canGenerate = false;
    public string filePath;
    public string[] contents;
    public string[] textLines;
    public string[] fileNames;

    public string splitCharacter;
    public string prefixFileName;
    public int maximumDisplayLine = 1000;

    [Header("UI")]
    public Toggle canGenerateStatusToggle;
    public InputField filePathInputField;
    public InputField splitInputField;
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

    public void OnSplitCharacterChanged()
    {
        splitCharacter = splitInputField.text;
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

    public void ClearSplitCharacter()
    {
        splitCharacter = string.Empty;
        splitInputField.text = splitCharacter;
    }

    public void ClearPrefix()
    {
        prefixFileName = string.Empty;
        prefixInputField.text = prefixFileName;
    }

    public void ProcessContents()
    {
        if (textLines.Length == 0) return;

        int counter = 0;
        List<string> fileNameList = new List<string>();
        List<string> contentList = new List<string>();

        foreach (string a in textLines)
        {
            /* break loop when instantiate more than maximum lines */
            if (counter >= maximumDisplayLine) break;

            /* create content text object in preview container */
            ContentText contentText = Instantiate(textContentPrefab, contentPreviewRect.content.transform);

            string filename = "";
            string content = "";
            /* split text line to filename and content */
            if (splitCharacter != "")
            {
                string[] spliter = a.Split(splitCharacter.ToCharArray());

                if (spliter.Length < 2)
                {
                    Debug.Log("can not split content");
                    break;
                }

                filename = prefixFileName + spliter.First<string>();
                content = string.Join(" ", spliter.Skip(1));
            }
            else
            {
                filename = prefixFileName + "_" + (counter + 1).ToString();
                content = a;
            }

            fileNameList.Add(filename);
            contentList.Add(content);

            /* set text to display each content line */
            string displayContent = string.Format("Filename: {0}, Content: {1}", filename, content);
            contentText.SetElement(displayContent);
            counter++;
        }
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


        if (lines.Length == 0)
        {
            /* can not read content from this file path */
            return;
        }

        textLines = lines;
        contents = lines;
        lineCountText.text = string.Format("Line count: {0}", textLines.Length);

        ClearPreviewContainer();


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
