using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scanner : MonoBehaviour
{
    public QRCodeDecodeController qrCodeDecodeController;
    public RawImage qrCameraPreview;

    private void Start()
    {
        if (qrCodeDecodeController == null) qrCodeDecodeController = GetComponent<QRCodeDecodeController>();

        qrCodeDecodeController.onQRScanFinished += OnQRScaned;
        StartScanner();
    }

    private void OnQRScaned(string msg)
    {
        Debug.Log(msg);
    }

    public void StartScanner()
    {
        qrCodeDecodeController.StartWork();
        StartCoroutine(StartQRCamera());
    }

    IEnumerator StartQRCamera()
    {
       yield return new WaitUntil(()=> qrCodeDecodeController.e_DeviceController.dWebCam != null);
        qrCameraPreview.texture = qrCodeDecodeController.e_DeviceController.dWebCam.preview;
    }

    public void StopScan()
    {
       qrCodeDecodeController.StopWork();
        
    }

    public void ResetScan()
    {
        qrCodeDecodeController.Reset();
    }
}
