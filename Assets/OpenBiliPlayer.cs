using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using Vuplex.WebView;
using System.Threading;

/// <summary>
/// 想实现的功能：打开哔哩哔哩的某个视频，并抽取出进度条
/// 问题：如果直接打开网站，再用js实现抽取进度条
///         在PC上，WebView打开的浏览器不支持html5播放器（因此也无法播放视频），因此也就拿不到进度条对应的html元素
///         在HoloLens上，WebView打开的浏览器能播放视频，但是执行js有点问题，而且调试太麻烦，所以暂时弃疗了= =。
/// </summary>

public class OpenBiliPlayer : MonoBehaviour
{
    public WebViewPrefab webView;
    TextMeshPro tmp;
    string debugInfo;

    SendString SendXPathScript;

    private void Awake()
    {
        Invoke("GetSendXPath", 1f);
        tmp = gameObject.AddComponent<TextMeshPro>();
        tmp.fontSize = 0.5f;
        tmp.transform.position = new Vector3(10f, -2.4f, 0.6f);
    }

    void GetSendXPath()
    {
        // Network Communication
        SendXPathScript = GameObject.Find("WebManager").GetComponent<SendString>();
    }

    // Start is called before the first frame update
    async void Start()
    {
        string htmlUrl = "https://www.bilibili.com/video/BV1H44y1t75x/?spm_id_from=333.999.0.0&vd_source=91fd13debe34aa346c970638ae4473f6";
        Debug.Log("htmlUrl:" + htmlUrl);


        // 宽是一个固定值，自己设置。
        float webViewBrowserUnityWidth = 0.35f;
        webView = WebViewPrefab.Instantiate(webViewBrowserUnityWidth, 3 * webViewBrowserUnityWidth); //第二个值自己设，大一点无妨

        await webView.WaitUntilInitialized();
        webView.WebView.LoadUrl(htmlUrl);
        await webView.WebView.WaitForNextPageLoadToFinish();

        Debug.Log("!!!!!!!!!!!!!!!Load WebPage Finish");

        //string BrowserWidth = await wvp.WebView.ExecuteJavaScript("document.body.offsetWidth");
        int webViewBrowserPixelWidth = 1280; //本来是1080，但 1.跨平台可能不一致 2.大一点无妨
        // 改成280，莫名其妙地work了
        int webViewBrowserPixelHeight;

        //调整分辨率
        webView.Resolution = webViewBrowserPixelWidth / webViewBrowserUnityWidth;

        //====================================================
        //获取元素宽高
        int elementPixelWidth = 1280;// int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetWidth"));//顺便，原html中true写错了
        //有时会有“最后一个按钮换行”的情况出现，猜测是前面的计算误差导致？
        //为了避免以上情况，这里把 元素宽度 增加5%-10%
        elementPixelWidth = (int)(elementPixelWidth * 1.10);
        int elementPixelHeight = 500; // int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetHeight"));

        print("ui w/h:" + elementPixelWidth + "," + elementPixelHeight);

        // 计算比例下的高
        float webViewBrowserUnityHeight = (float)elementPixelHeight / elementPixelWidth * webViewBrowserUnityWidth;
        webView.Resize(webViewBrowserUnityWidth, webViewBrowserUnityHeight);

        print("web unity w/h:" + webViewBrowserUnityWidth + "," + webViewBrowserUnityHeight);

        ////====================================================
        ////获取元素宽高 2.0
        //elementPixelWidth = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetWidth"));//顺便，原html中true写错了
        ////有时会有“最后一个按钮换行”的情况出现，猜测是前面的计算误差导致？
        ////为了避免以上情况，这里把 元素宽度 增加5%-10%
        //elementPixelWidth = (int)(elementPixelWidth * 1.05);
        //elementPixelHeight = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetHeight"));

        //print("ui w/h 2.0:" + elementPixelWidth + "," + elementPixelHeight);

        //// 计算比例下的高
        //webViewBrowserUnityHeight = (float)elementPixelHeight / elementPixelWidth * webViewBrowserUnityWidth;
        //webView.Resize(webViewBrowserUnityWidth, webViewBrowserUnityHeight);

        //print("web unity w/h 2.0:" + webViewBrowserUnityWidth + "," + webViewBrowserUnityHeight);
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

        // set the background to transparent
        string transparentBackground = "metaTransparent = document.createElement('meta');metaTransparent.name='transparent';metaTransparent.content='true';document.head.appendChild(metaTransparent);";
        await webView.WebView.ExecuteJavaScript(transparentBackground);
        Debug.Log("set bg to transparent");
        tmp.text = "set bg to transparent";

        // extract target UI
        string freezeProgressBar = "pbTop=document.querySelector('.bpx-player-control-top');pbTop.style.visibility='visible'; pbTop.style.opacity=1;";
        Debug.Log(await webView.WebView.ExecuteJavaScript(freezeProgressBar));
        Debug.Log("set the pb opacity to be visible");
        tmp.text = "set the pb opacity to be visible";
        Thread.Sleep(1000);
        string extractProgressBar = "styleHideProgressBar = document.createElement('style');styleHideProgressBar.innerHTML = '.bpx-player-control-top{position: fixed;left: 0px;top: 0px;background: rgb(255, 255, 255);z-index: 100000000;width: 100vw;height: 100vh;overflow-y: scroll;} .bili-header{display:none;}';document.head.appendChild(styleHideProgressBar);";
        Debug.Log(await webView.WebView.ExecuteJavaScript(extractProgressBar));
        Debug.Log("extracted pb");
        tmp.text = "extracted pb";

        //webView.Clicked += async (sender, eventArgs) =>
        //{
        //    //Debug.Log("clicked at point: " + Math.Round(eventArgs.Point.x, 4) + ", " + Math.Round(eventArgs.Point.y, 4));

        //    int coordinationXofClick = (int)(elementPixelWidth * Math.Round(eventArgs.Point.x, 4));
        //    int coordinationYofClick = (int)(elementPixelHeight * Math.Round(eventArgs.Point.y, 4));
        //    Debug.Log("Click at Pixel:" + coordinationXofClick + "," + coordinationYofClick);

        //    string query = "document.elementFromPoint(" + coordinationXofClick + "," + coordinationYofClick + ").closest('.video-page-card-small').querySelector('a:first-child').getAttribute('href')";
        //    string clickElementUrl = await webView.WebView.ExecuteJavaScript(query);
        //    Debug.Log("Click element: " + clickElementUrl);


        //    // 按照url打开新页面，并只显示UI
        //    webView.WebView.LoadUrl("https://www.bilibili.com" + clickElementUrl);
        //    await webView.WebView.WaitForNextPageLoadToFinish();
        //    string jsNewElement = "styleHide = document.createElement('style');";
        //    await webView.WebView.ExecuteJavaScript(jsNewElement);
        //    string jsSetStyle = "styleHide.innerHTML = '.rec-list{position: fixed;left: 0px;top: 0px;background: rgb(255, 255, 255);z-index: 100000000;width: 100vw;height: 100vh;overflow-y: scroll;} .left-container{display:none;} .bili-header{display:none;}'";
        //    await webView.WebView.ExecuteJavaScript(jsSetStyle);
        //    string jsAddElement = "document.head.appendChild(styleHide);";
        //    await webView.WebView.ExecuteJavaScript(jsAddElement);

        //    //string query = "getXPath(document.elementFromPoint(" + coordinationXofClick + "," + coordinationYofClick + "))";
        //    //string clickElement = await webView.WebView.ExecuteJavaScript(query);
        //    //Debug.Log("Click element: " + clickElement);

        //    // Network Communication
        //    //SendXPathScript.testClient.SendString(clickElement);

        //    //string queryColor = "document.elementFromPoint(" + coordinationXofClick + "," + coordinationYofClick + ").style.backgroundColor = 'red'";
        //    //await webView.WebView.ExecuteJavaScript(queryColor);

        //    //debugInfo = clickElement;
        //    tmp.text = debugInfo;
        //};


    }

    // Update is called once per frame
    void Update()
    {
        //debugInfo = Application.persistentDataPath;
        //tmp.text = debugInfo;
    }
}
