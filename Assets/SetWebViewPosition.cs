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
    public Vector3 anchorOffset = new Vector3(0.35f, 0f, -0.2f);
    TextMeshPro tmp;
    WebViewPrefab webView;

    Vector3 qrPosition;
    Vector3 qrRotation;

    public GameObject startScanButton;

    public GameObject QRCodesManagerRef;
    private System.Collections.Generic.SortedDictionary<string, UnityEngine.GameObject> qrList;

    private void Awake()
    {
        tmp = gameObject.AddComponent<TextMeshPro>();
        tmp.fontSize = 0.5f;
        tmp.transform.position = new Vector3(10f, -2.2f, 0.6f);
        tmp.text = "hello";
    }

    void Start()
    {
        Invoke("GetQRRelated", 1f);
        webView = gameObject.GetComponent<OpenBiliRecommendationImg>().webView;
    }

    void GetQRRelated()
    {
        qrList = QRCodesManagerRef.GetComponent<QRCodesVisualizer>().qrCodesObjectsList;
        Debug.Log("=================get qrlist:" + qrList);

        //startScanButton = GameObject.Find("QRCodePanel").transform.Find("ContentPanel/StartScanButton").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (webView == null)//获取对webView的引用
        {
            webView = gameObject.GetComponent<OpenBiliRecommendationImg>().webView;
        }

        if (!startScanButton.activeSelf) //只有在扫描二维码的时候才调整UI位置
        {
            tmp.text = "scanning, qrListLen=" + qrList.Count;
            try
            {
                GameObject qrCode = qrList.First().Value;
                qrPosition = qrCode.transform.position;
                qrRotation = qrCode.transform.rotation.eulerAngles;
            }
            catch
            {
                qrPosition = new Vector3(0, 0, 5);
                qrRotation = webView.transform.rotation.eulerAngles;
            }

            try
            {
                webView.transform.position = qrPosition + Quaternion.Euler(qrRotation) * anchorOffset;
                webView.transform.rotation = Quaternion.Euler(qrRotation) * Quaternion.Euler(0f, 0f, 180f);
            }
            catch {; }
        }
        else
        {
            tmp.text = "not scanning";
        }
        
    }
}