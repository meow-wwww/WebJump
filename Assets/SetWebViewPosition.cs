using QRTracking;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TMPro;
using UnityEngine;
using Vuplex.WebView;

public class SetWebViewPosition : MonoBehaviour
{
    public Vector3 anchorPosition = new Vector3(0, 0.4f, 0.4f);
    Vector3 anchorOffset = new Vector3(0.1f, 0.1f, 0.1f);
    TextMeshPro tmp;
    WebViewPrefab webView;

    Vector3 qrPosition;
    Vector3 qrRotation;

    GameObject startScanButton;

    private System.Collections.Generic.SortedDictionary<string, UnityEngine.GameObject> qrList;

    // Start is called before the first frame update
    void Start()
    {
        tmp = gameObject.GetComponent<TextMeshPro>();
        tmp.text = "hello";

        Invoke("GetQRRelated", 1f);

        webView = gameObject.GetComponent<TestWebNew>().webView;
    }

    void GetQRRelated()
    {
        qrList = GameObject.Find("QRCodesManager").GetComponent<QRCodesVisualizer>().qrCodesObjectsList;
        Debug.Log("=================get qrlist:" + qrList);

        startScanButton = GameObject.Find("QRCodePanel").transform.Find("ContentPanel/StartScanButton").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (!startScanButton.activeSelf) //只有在扫描二维码的时候才调整UI位置
            {
                GameObject qrCode = qrList.First().Value;
                qrPosition = qrCode.transform.position;
                qrRotation = qrCode.transform.rotation.eulerAngles;

                webView.transform.position = qrPosition + Quaternion.Euler(qrRotation) * anchorOffset; // qrCode.transform.rotation * anchorOffset;
                webView.transform.rotation = Quaternion.Euler(qrRotation) * Quaternion.Euler(0f, 0f, 180f); //Quaternion.Euler(0, 180, 0) * qrCode.transform.rotation;
                Debug.Log("qr rotation quaternion:" + Quaternion.Euler(qrRotation));
            }
        }
        catch
        {

        }
        
    }

    public void setAnchorPositon(Vector3 position)
    {
        anchorPosition = position;
        tmp.text = "pos:" + position;
    }
}