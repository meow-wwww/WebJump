using QRTracking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Vuplex.WebView;

public class test : MonoBehaviour
{
    WebViewPrefab webView;

    // Start is called before the first frame update
    void Start()
    {
        webView = gameObject.GetComponent<OpenShimoUI>().webView;
        Invoke("RotateWebView", 5f);
    }

    void RotateWebView()
    {
        Debug.Log("=================will rotate webview:");
        webView.transform.rotation = Quaternion.Euler(0f, 0f, 180f) * webView.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
