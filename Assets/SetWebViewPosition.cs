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
    Vector3 anchorOffset = new Vector3(0.15f, 0.1f, 0.1f);
    TextMeshPro tmp;
    WebViewPrefab webView;

    Vector3 qrPosition = new Vector3(-0.3f, -0.2f, 0.4f);
    Vector3 qrRotation = new Vector3(210f, 45f, 0f);

    private System.Collections.Generic.SortedDictionary<string, UnityEngine.GameObject> qrList;

    // Start is called before the first frame update
    void Start()
    {
        tmp = gameObject.GetComponent<TextMeshPro>();
        tmp.text = "hello";

        Invoke("GetQRList", 1f);

        webView = gameObject.GetComponent<TestWebNew>().webView;
    }

    void GetQRList()
    {
        qrList = GameObject.Find("QRCodesManager").GetComponent<QRCodesVisualizer>().qrCodesObjectsList;
        Debug.Log("=================get qrlist:" + qrList);
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            GameObject qrCode = qrList.First().Value;
            qrPosition = qrCode.transform.position;
            qrRotation = qrCode.transform.rotation.eulerAngles;

            tmp.text = "qr:" + ", pos:" + qrPosition + "rot:" + qrRotation;
            webView.transform.position = qrPosition + Quaternion.Euler(qrRotation) * anchorOffset; // qrCode.transform.rotation * anchorOffset;
            webView.transform.rotation = Quaternion.Euler(qrRotation) * Quaternion.Euler(0f, 0f, 180f); //Quaternion.Euler(0, 180, 0) * qrCode.transform.rotation;
            Debug.Log("qr rotation quaternion:" + Quaternion.Euler(qrRotation));
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