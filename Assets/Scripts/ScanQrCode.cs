using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;

public class ScanQrCode : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    private bool _isCameraInitialized;
    private WebCamTexture _webCamTexture;
    private IBarcodeReader barcodeReader = new BarcodeReader();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupCamera() {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0) {
            Debug.Log("No camera detected");
            _isCameraInitialized = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++) {
            if (!devices[i].isFrontFacing) {
                _webCamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                break;
            }
        }

        _isCameraInitialized = _webCamTexture != null;

        if (_isCameraInitialized) {
            _webCamTexture.Play();
        }

        Debug.Log("Camera setup done. _isCameraInitialized: " + _isCameraInitialized);
    }

    public void ScanCode() {
        if (!_isCameraInitialized) {
            SetupCamera();
        }

        try {
            if (_isCameraInitialized) {
                var result = barcodeReader.Decode(_webCamTexture.GetPixels32(), _webCamTexture.width, _webCamTexture.height);
                if (result != null && result.Text != null) {
                    Debug.Log("DECODED TEXT FROM QR: " + result.Text);
                    _text.text = result.Text;
                } else {
                    Debug.Log("DECODE FAILED");
                    _text.text = "";
                }
            } else {
                Debug.Log("ScanCode: _isCameraInitialized is false");
            }
        } catch (System.Exception ex) {
            Debug.LogWarning(ex.Message);
            _text.text = "Error scanning QR code";
        }
    }
}
