using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
public class Generator : MonoBehaviour
{
    [Header("Properies")]
    public string savePath = @"C:\QRGenerate\ACMECS\";
    public string filePath;
    public string[] contents;
    public bool isProcessing = false;
    public bool onGenerate = false;

    [Header("UI")]
    public InputField filePathInputField;

    [Header("Component")]
    public QRCodeEncodeController qrCodeEncodeController;

    public int startNumber = 1;
    public int endNumber = 1000;
    public int currentNumber = 0;
    public string currentFileName = "";

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        qrCodeEncodeController.onQREncodeFinished += OnEncoded;
    }

    public void ReadFile()
    {
        filePath = filePathInputField.text;

        if(string.IsNullOrEmpty(filePath))
        {
            Debug.Log("Please enter file path.");
            return;
        }

        if (File.Exists(filePath))
        {
            contents = File.ReadAllLines(filePath);
            Debug.Log(contents.Length);
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }

    public void Generate()
    {
        if(!isProcessing)
        {
            StartCoroutine(Co_Generate());
        }
    }

    IEnumerator Co_Generate()
    {
        isProcessing = true;
        currentNumber = startNumber;

        while(currentNumber <= endNumber)
        {
            onGenerate = true;
            currentFileName = currentNumber+".png";
            string content = "https://acmecs-businessmatching.com/match/?uid="+currentNumber+"&openExternalBrowser=1";
            qrCodeEncodeController.Encode(content);
            currentNumber++;
            yield return new WaitUntil(()=> onGenerate == false);
        }
        yield return null;



        isProcessing = false;
    }

    private void OnEncoded(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        string path = savePath+currentFileName;
        File.WriteAllBytes(path,bytes);
        Debug.Log(currentFileName+" Saved");
        onGenerate = false;
    }
}
