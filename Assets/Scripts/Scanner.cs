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
    public Toggle autoResetToggle;
    public GameObject scanFrame;
    public Button resetCameraButton;
    public int originQRCameraPreviewWidth;
    public int originQRCameraPreviewHeight;
    public bool onSetCameraPreviewSize = false;
    public bool isAutoReset = false;

    [Header("Texture Decode")]
    public string filePath;
    public RawImage decodeTexturePreview;
    public int originDecodeTexturePreviewWidth;
    public int originDecodeTexturePreviewHeight;

    private void Start()
    {
        if (qrCodeDecodeController == null) qrCodeDecodeController = GetComponent<QRCodeDecodeController>();

        qrCodeDecodeController.onQRScanFinished += OnQRScaned;
        qrCodeDecodeController.StopWork();
        scanFrame.SetActive(qrCodeDecodeController.e_DeviceController.isPlaying);
        resetCameraButton.gameObject.SetActive(false);

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Image Files", ".png", ".jpg", ".jpeg"));
        FileBrowser.SetDefaultFilter(".png");
        FileBrowser.SetExcludedExtensions(".txt", ".pdf", ".lnk", ".tmp", ".zip", ".rar", ".exe");

        originQRCameraPreviewWidth = (int)qrCameraPreview.rectTransform.sizeDelta.x;
        originQRCameraPreviewHeight = (int)qrCameraPreview.rectTransform.sizeDelta.y;

        originDecodeTexturePreviewWidth = (int)decodeTexturePreview.rectTransform.sizeDelta.x;
        originDecodeTexturePreviewHeight = (int)decodeTexturePreview.rectTransform.sizeDelta.y;

        isAutoReset = false;
        autoResetToggle.isOn = isAutoReset;
        Initialize();
    }

    void Initialize()
    {
        resultText.text = string.Format("{0}", string.Empty);
    }

    private void OnQRScaned(string msg)
    {
        DisplayResult(msg);

        if (autoResetToggle.isOn)
        {
            resetCameraButton.gameObject.SetActive(false);
            StartCoroutine(CountResetQR());
        }
        else
        {
            resetCameraButton.gameObject.SetActive(true);
        }
    }

    IEnumerator CountResetQR()
    {
        yield return new WaitForSeconds(1);
        ResetScan();
    }

    public void ToggleScanner()
    {
        if (!qrCodeDecodeController.e_DeviceController.isPlaying)
        {
            qrCodeDecodeController.StartWork();
            scanFrame.SetActive(true);
            StartCoroutine(StartQRCamera());
        }
        else
        {
            qrCodeDecodeController.StopWork();
            scanFrame.SetActive(false);
        }

        resetCameraButton.gameObject.SetActive(false);
    }

    public void ToggleAutoReset()
    {
        isAutoReset = autoResetToggle.isOn;
    }

    IEnumerator StartQRCamera()
    {
        yield return new WaitUntil(() => qrCodeDecodeController.e_DeviceController.dWebCam != null);

        if (!onSetCameraPreviewSize)
        {
            int textureWidth = qrCodeDecodeController.e_DeviceController.dWebCam.preview.width;
            int textureHeight = qrCodeDecodeController.e_DeviceController.dWebCam.preview.height;

            /* fixed height */
            int newWidth = (int)(((float)textureWidth / (float)textureHeight) * originQRCameraPreviewWidth);
            qrCameraPreview.rectTransform.sizeDelta = new Vector2(newWidth, originQRCameraPreviewHeight);
            onSetCameraPreviewSize = true;
        }

        qrCameraPreview.texture = qrCodeDecodeController.e_DeviceController.dWebCam.preview;
    }

    public void ResetScan()
    {
        qrCodeDecodeController.Reset();
        resetCameraButton.gameObject.SetActive(false);
    }

    /**********************************************************************************************************/
    /*************************************  DECODE BY FROM TEXTURE2D  *****************************************/
    /**********************************************************************************************************/

    public void ResetTexture()
    {
        
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
                ReadFile(filePath);
            }
        }
        else
        {
            filePath = string.Empty;
        }
    }

    private void ReadFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(1, 1);

        texture.LoadImage(bytes);
        texture.Apply();

        if (texture.width <= texture.height)
        {
            /* Protrait => fit height */
            int newWidth = (int)((float)originDecodeTexturePreviewHeight * ((float)texture.width / (float)texture.height));
            decodeTexturePreview.rectTransform.sizeDelta = new Vector2(newWidth, originDecodeTexturePreviewHeight);
        }
        else
        {
            /* Landscape => fit width */
            int newHeight = (int)((float)originDecodeTexturePreviewWidth * ((float)texture.height / (float)texture.width));
            decodeTexturePreview.rectTransform.sizeDelta = new Vector2(originDecodeTexturePreviewWidth, newHeight);
        }

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
