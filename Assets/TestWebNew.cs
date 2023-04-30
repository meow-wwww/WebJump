using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vuplex.WebView;

public class TestWebNew : MonoBehaviour
{
    public WebViewPrefab webView;
    TextMeshPro tmp;
    string debugInfo;

    // Start is called before the first frame update
    async void Start()
    {
        tmp = gameObject.AddComponent<TextMeshPro>();
        tmp.fontSize = 0.5f;
        tmp.transform.position = new Vector3(10f, -2.4f, 0.6f);

        string htmlUrl = "streaming-assets://Firepad Demo.html";

        // 宽是一个固定值，自己设置。
        float webViewBrowserUnityWidth = 0.55f;
        webView = WebViewPrefab.Instantiate(webViewBrowserUnityWidth, 3*webViewBrowserUnityWidth); //第二个值自己设，大一点无妨
        
        
        await webView.WaitUntilInitialized();
        webView.WebView.LoadUrl(htmlUrl);

        await webView.WebView.WaitForNextPageLoadToFinish();

        // debugInfo = "url:" + webView.WebView.Url;

        //string BrowserWidth = await wvp.WebView.ExecuteJavaScript("document.body.offsetWidth");
        int webViewBrowserPixelWidth = 1280; //本来是1080，但 1.跨平台可能不一致 2.大一点无妨
        int webViewBrowserPixelHeight;

        //调整分辨率
        webView.Resolution = webViewBrowserPixelWidth / webViewBrowserUnityWidth;

        //====================================================
        //获取元素宽高
        int elementPixelWidth = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetWidth"));//顺便，原html中true写错了
        //有时会有“最后一个按钮换行”的情况出现，猜测是前面的计算误差导致？
        //为了避免以上情况，这里把 元素宽度 增加5%-10%
        elementPixelWidth = (int)(elementPixelWidth * 1.10);
        int elementPixelHeight = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetHeight"));

        print("ui w/h:" + elementPixelWidth + "," + elementPixelHeight);

        // 计算比例下的高
        float webViewBrowserUnityHeight = (float)elementPixelHeight / elementPixelWidth * webViewBrowserUnityWidth;
        webView.Resize(webViewBrowserUnityWidth, webViewBrowserUnityHeight);

        print("web unity w/h:" + webViewBrowserUnityWidth + "," + webViewBrowserUnityHeight);

        //====================================================
        //获取元素宽高 2.0
        elementPixelWidth = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetWidth"));//顺便，原html中true写错了
        //有时会有“最后一个按钮换行”的情况出现，猜测是前面的计算误差导致？
        //为了避免以上情况，这里把 元素宽度 增加5%-10%
        elementPixelWidth = (int)(elementPixelWidth * 1.05);
        elementPixelHeight = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetHeight"));

        print("ui w/h 2.0:" + elementPixelWidth + "," + elementPixelHeight);

        // 计算比例下的高
        webViewBrowserUnityHeight = (float)elementPixelHeight / elementPixelWidth * webViewBrowserUnityWidth;
        webView.Resize(webViewBrowserUnityWidth, webViewBrowserUnityHeight);

        print("web unity w/h 2.0:" + webViewBrowserUnityWidth + "," + webViewBrowserUnityHeight);
        //====================================================

        // 获取网页原本的像素值
        webViewBrowserPixelWidth = webView.WebView.Size.x;

        // 修改网页的resolution，即原本的网页pixel用了X这个比例，那么现在只有y个pixel了，计算新的比例
        webView.Resolution = webView.Resolution / webViewBrowserPixelWidth * elementPixelWidth;

        // 调整位置
        Debug.Log("wvp pos before:" + webView.transform.position);
        webView.transform.position = new Vector3(0, 0.1f, 0.4f);
        Debug.Log("wvp pos after:" + webView.transform.position);
        webView.transform.eulerAngles = new Vector3(0, 180, 0);

        webView.Clicked += async (sender, eventArgs) =>
        {
            //Debug.Log("clicked at point: " + Math.Round(eventArgs.Point.x, 4) + ", " + Math.Round(eventArgs.Point.y, 4));

            int coordinationXofClick = (int)(elementPixelWidth * Math.Round(eventArgs.Point.x, 4));
            int coordinationYofClick = (int)(elementPixelHeight * Math.Round(eventArgs.Point.y, 4));
            //Debug.Log("Click at Pixel:" + coordinationXofClick + "," + coordinationYofClick);

            string query = "getXPath(document.elementFromPoint(" + coordinationXofClick + "," + coordinationYofClick + "))";
            string clickElement = await webView.WebView.ExecuteJavaScript(query);
            Debug.Log("Click element: " + clickElement);

            string queryColor = "document.elementFromPoint(" + coordinationXofClick + "," + coordinationYofClick + ").style.backgroundColor = 'red'";
            await webView.WebView.ExecuteJavaScript(queryColor);

            debugInfo = clickElement;
            tmp.text = debugInfo;
        };
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //debugInfo = Application.persistentDataPath;
        //tmp.text = debugInfo;
    }
}
