using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.IO;

public class Scanner : MonoBehaviour
{
    public QRCodeDecodeController qrCodeDecodeController;
    public Text resultText;

    [Header("Camera Decode")]
    public RawImage qrCameraPreview;
    public bool onSetCameraPreviewSize = false;

    [Header("Texture Decode")]
    public string filePath;
    public InputField filePathInputField;
    public RawImage decodeTexturePreview;

    private void Start()
    {
        if (qrCodeDecodeController == null) qrCodeDecodeController = GetComponent<QRCodeDecodeController>();

        qrCodeDecodeController.onQRScanFinished += OnQRScaned;
        qrCodeDecodeController.StopWork();

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Image Files", ".png", ".jpg", ".jpeg"));
        FileBrowser.SetDefaultFilter(".png");
        FileBrowser.SetExcludedExtensions(".txt", ".pdf", ".lnk", ".tmp", ".zip", ".rar", ".exe");
    }

    private void OnQRScaned(string msg)
    {
        DisplayResult(msg);
    }

    public void ToggleScanner()
    {
        if (!qrCodeDecodeController.e_DeviceController.isPlaying)
        {
            qrCodeDecodeController.StartWork();
            StartCoroutine(StartQRCamera());
        }
        else
        {
            qrCodeDecodeController.StopWork();
        }
    }

    IEnumerator StartQRCamera()
    {
        yield return new WaitUntil(() => qrCodeDecodeController.e_DeviceController.dWebCam != null);

        if (!onSetCameraPreviewSize)
        {
            int textureWidth = qrCodeDecodeController.e_DeviceController.dWebCam.preview.width;
            int textureHeight = qrCodeDecodeController.e_DeviceController.dWebCam.preview.height;

            int previewWidth = (int)qrCameraPreview.rectTransform.sizeDelta.x;
            int previewHeight = (int)qrCameraPreview.rectTransform.sizeDelta.y;

            /* fixed height */
            int newWidth = (int)(((float)textureWidth / (float)textureHeight) * previewWidth);
            qrCameraPreview.rectTransform.sizeDelta = new Vector2(newWidth, previewHeight);
            onSetCameraPreviewSize = true;
        }

        qrCameraPreview.texture = qrCodeDecodeController.e_DeviceController.dWebCam.preview;
    }

    public void ResetScan()
    {
        qrCodeDecodeController.Reset();
    }

    /**********************************************************************************************************/
    /*************************************  DECODE BY FROM TEXTURE2D  *****************************************/
    /**********************************************************************************************************/

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
                ReadFile(filePath);
            }
        }
        else
        {
            filePath = string.Empty;
        }

        filePathInputField.text = filePath;
    }

    private void ReadFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(1, 1);
        Debug.Log(bytes.Length);

        texture.LoadImage(bytes);
        texture.Apply();

        decodeTexturePreview.texture = texture;
        TextureDecode(texture);
    }

    private void TextureDecode(Texture2D texture)
    {
        string msg = QRCodeDecodeController.DecodeByStaticPic(texture);
        DisplayResult(msg);
    }


    private void DisplayResult(string msg)
    {
        resultText.text = string.Format("{0}", msg);
    }
}
