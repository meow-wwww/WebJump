using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vuplex.WebView;
using System;

public class OpenBiliRecommendationImg : MonoBehaviour
{
    public WebViewPrefab webView;

    SendString SendStringScript;

    private void Awake()
    {
        Invoke("GetSendXPath", 1f);
    }

    void GetSendXPath()
    {
        // Network Communication
        SendStringScript = GameObject.Find("WebManager").GetComponent<SendString>();
    }

    // Start is called before the first frame update
    async void Start()
    {
        string htmlUrl = "streaming-assets://biliRecommend/recommend.html";

        // 宽是一个固定值，自己设置。
        float webViewBrowserUnityWidth = 0.15f;
        webView = WebViewPrefab.Instantiate(webViewBrowserUnityWidth, 1294f / 714f * webViewBrowserUnityWidth); //第二个值自己设，大一点无妨

        await webView.WaitUntilInitialized();
        webView.WebView.LoadUrl(htmlUrl);
        await webView.WebView.WaitForNextPageLoadToFinish();

        int webViewBrowserPixelWidth = 714;

        //调整分辨率
        webView.Resolution = webViewBrowserPixelWidth / webViewBrowserUnityWidth;

        // 调整位置
        Debug.Log("wvp pos before:" + webView.transform.position);
        webView.transform.position = new Vector3(0, 0.1f, 0.4f);
        Debug.Log("wvp pos after:" + webView.transform.position);
        webView.transform.eulerAngles = new Vector3(0, 180, 0);

        webView.Clicked += async (sender, eventArgs) =>
        {
            //float x = (float)Math.Round(eventArgs.Point.x, 4);
            float y = (float)Math.Round(eventArgs.Point.y, 4);
            //Debug.Log("clicked at point: " + Math.Round(eventArgs.Point.x, 4) + ", " + Math.Round(eventArgs.Point.y, 4));

            int videoNumber = (int)(y * 7f) + 1;
            Debug.Log("click Video Number " + videoNumber + "~~~~~~~~~");

            if (videoNumber == 1 || videoNumber == 2 || videoNumber == 3)
            {
                SendStringScript.testClient.SendStringMessage("openVideo" + videoNumber);

                htmlUrl = "streaming-assets://biliRecommend/recommend" + videoNumber + ".html";
                webView.WebView.LoadUrl(htmlUrl);
                await webView.WebView.WaitForNextPageLoadToFinish();
            }
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
