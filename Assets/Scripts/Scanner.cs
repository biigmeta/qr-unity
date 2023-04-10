using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.IO;

public class Scanner : MonoBehaviour
{
    [Header("Components")]
    public QRCodeDecodeController qrCodeDecodeController;
    public DialogMessage dialogMessage;

    [Header("Camera Decode")]
    public RawImage qrCameraPreview;
    public Toggle autoResetToggle;
    public GameObject scanFrame;
    public Button toggleCameraButton;
    public Button resetButton;
    public int originQRCameraPreviewWidth;
    public int originQRCameraPreviewHeight;
    public bool onSetCameraPreviewSize = false;
    public bool isAutoReset = false;

    [Header("Image Decode")]
    public string filePath;
    public RawImage decodeTexturePreview;
    public int originDecodeTexturePreviewWidth;
    public int originDecodeTexturePreviewHeight;

    [Header("UI")]
    public Text resultText;

    [Header("Texture")]
    public Texture2D cameraTexture;
    public Texture2D previewTexture;

    private void Start()
    {
        if (qrCodeDecodeController == null) qrCodeDecodeController = GetComponent<QRCodeDecodeController>();

        qrCodeDecodeController.onQRScanFinished += OnQRScaned;

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Image Files", ".png", ".jpg", ".jpeg"));
        FileBrowser.SetDefaultFilter(".png");
        FileBrowser.SetExcludedExtensions(".txt", ".pdf", ".lnk", ".tmp", ".zip", ".rar", ".exe");

        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {

        if (WebCamTexture.devices.Length == 0)
        {
            yield break;
        }

        yield return null;

        if (qrCodeDecodeController.e_DeviceController.isPlaying) qrCodeDecodeController.StopWork();

        toggleCameraButton.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", qrCodeDecodeController.e_DeviceController.isPlaying ? "Turn off camera." : "Turn on camera.");

        originQRCameraPreviewWidth = (int)qrCameraPreview.rectTransform.sizeDelta.x;
        originQRCameraPreviewHeight = (int)qrCameraPreview.rectTransform.sizeDelta.y;

        originDecodeTexturePreviewWidth = (int)decodeTexturePreview.rectTransform.sizeDelta.x;
        originDecodeTexturePreviewHeight = (int)decodeTexturePreview.rectTransform.sizeDelta.y;

        resultText.text = string.Format("{0}", string.Empty);
        scanFrame.SetActive(false);
        resetButton.gameObject.SetActive(false);

        isAutoReset = false;
        autoResetToggle.isOn = isAutoReset;

    }

    private void OnQRScaned(string msg)
    {
        DisplayResult(msg);

        if (autoResetToggle.isOn)
        {
            resetButton.gameObject.SetActive(false);
            StartCoroutine(CountdownResetQR());
        }
        else
        {
            resetButton.gameObject.SetActive(true);
        }
    }

    IEnumerator CountdownResetQR()
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
            toggleCameraButton.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", qrCodeDecodeController.e_DeviceController.isPlaying ? "Turn off camera." : "Turn on camera.");

            /* reset camera preview texture */
            qrCameraPreview.texture = null;
        }

        resetButton.gameObject.SetActive(false);
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

        toggleCameraButton.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", qrCodeDecodeController.e_DeviceController.isPlaying ? "Turn off camera." : "Turn on camera.");

        qrCameraPreview.texture = qrCodeDecodeController.e_DeviceController.dWebCam.preview;
    }

    public void ResetScan()
    {
        qrCodeDecodeController.Reset();
        decodeTexturePreview.texture = null;
        resetButton.gameObject.SetActive(false);
        resultText.text = string.Empty;
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
        resetButton.gameObject.SetActive(true);
    }
}
