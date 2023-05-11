using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System;
using UnityEngine;
using Vuplex.WebView;

public class OpenBiliRecommendation : MonoBehaviour
{
    public WebViewPrefab webView;
    TextMeshPro tmp;
    string debugInfo;

    SendString SendXPathScript;

    private void Awake()
    {
        Invoke("GetSendXPath", 1f);
        //tmp = gameObject.AddComponent<TextMeshPro>();
        //tmp.fontSize = 0.5f;
        //tmp.transform.position = new Vector3(10f, -2.4f, 0.6f);
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

        // ����һ���̶�ֵ���Լ����á�
        float webViewBrowserUnityWidth = 0.1f;
        webView = WebViewPrefab.Instantiate(webViewBrowserUnityWidth, 3 * webViewBrowserUnityWidth); //�ڶ���ֵ�Լ��裬��һ���޷�

        await webView.WaitUntilInitialized();
        webView.WebView.LoadUrl(htmlUrl);
        await webView.WebView.WaitForNextPageLoadToFinish();

        //string BrowserWidth = await wvp.WebView.ExecuteJavaScript("document.body.offsetWidth");
        int webViewBrowserPixelWidth = 280; //������1080���� 1.��ƽ̨���ܲ�һ�� 2.��һ���޷�
        // �ĳ�280��Ī�������work��
        int webViewBrowserPixelHeight;

        // extract target UI
        string jsNewElement = "styleHide = document.createElement('style');";
        await webView.WebView.ExecuteJavaScript(jsNewElement);
        string jsSetStyle = "styleHide.innerHTML = '.rec-list{position: fixed;left: 0px;top: 0px;background: rgb(255, 255, 255);z-index: 100000000;width: 100vw;height: 100vh;overflow-y: scroll;} .left-container{display:none;} .bili-header{display:none;}'";
        await webView.WebView.ExecuteJavaScript(jsSetStyle);
        string jsAddElement = "document.head.appendChild(styleHide);";
        await webView.WebView.ExecuteJavaScript(jsAddElement);

        //�����ֱ���
        webView.Resolution = webViewBrowserPixelWidth / webViewBrowserUnityWidth;

        //====================================================
        //��ȡԪ�ؿ��
        int elementPixelWidth = 330; // int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetWidth"));//˳�㣬ԭhtml��trueд����
        //��ʱ���С����һ����ť���С���������֣��²���ǰ��ļ������£�
        //Ϊ�˱����������������� Ԫ�ؿ�� ����5%-10%
        elementPixelWidth = (int)(elementPixelWidth * 1.10);
        int elementPixelHeight = 690; // int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetHeight"));

        print("ui w/h:" + elementPixelWidth + "," + elementPixelHeight);

        // ��������µĸ�
        float webViewBrowserUnityHeight = (float)elementPixelHeight / elementPixelWidth * webViewBrowserUnityWidth;
        webView.Resize(webViewBrowserUnityWidth, webViewBrowserUnityHeight);

        print("web unity w/h:" + webViewBrowserUnityWidth + "," + webViewBrowserUnityHeight);

        ////====================================================
        ////��ȡԪ�ؿ�� 2.0
        //elementPixelWidth = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetWidth"));//˳�㣬ԭhtml��trueд����
        ////��ʱ���С����һ����ť���С���������֣��²���ǰ��ļ������£�
        ////Ϊ�˱����������������� Ԫ�ؿ�� ����5%-10%
        //elementPixelWidth = (int)(elementPixelWidth * 1.05);
        //elementPixelHeight = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetHeight"));

        //print("ui w/h 2.0:" + elementPixelWidth + "," + elementPixelHeight);

        //// ��������µĸ�
        //webViewBrowserUnityHeight = (float)elementPixelHeight / elementPixelWidth * webViewBrowserUnityWidth;
        //webView.Resize(webViewBrowserUnityWidth, webViewBrowserUnityHeight);

        //print("web unity w/h 2.0:" + webViewBrowserUnityWidth + "," + webViewBrowserUnityHeight);
        //====================================================

        // ��ȡ��ҳԭ��������ֵ
        webViewBrowserPixelWidth = webView.WebView.Size.x;

        // �޸���ҳ��resolution����ԭ������ҳpixel����X�����������ô����ֻ��y��pixel�ˣ������µı���
        webView.Resolution = webView.Resolution / webViewBrowserPixelWidth * elementPixelWidth;

        // ����λ��
        Debug.Log("wvp pos before:" + webView.transform.position);
        webView.transform.position = new Vector3(0, 0.1f, 0.4f);
        Debug.Log("wvp pos after:" + webView.transform.position);
        webView.transform.eulerAngles = new Vector3(0, 180, 0);

        webView.Clicked += async (sender, eventArgs) =>
        {
            //Debug.Log("clicked at point: " + Math.Round(eventArgs.Point.x, 4) + ", " + Math.Round(eventArgs.Point.y, 4));

            int coordinationXofClick = (int)(elementPixelWidth * Math.Round(eventArgs.Point.x, 4));
            int coordinationYofClick = (int)(elementPixelHeight * Math.Round(eventArgs.Point.y, 4));
            Debug.Log("Click at Pixel:" + coordinationXofClick + "," + coordinationYofClick);

            string query = "document.elementFromPoint(" + coordinationXofClick + "," + coordinationYofClick + ").closest('.video-page-card-small').querySelector('a:first-child').getAttribute('href')";
            string clickElementUrl = await webView.WebView.ExecuteJavaScript(query);
            Debug.Log("Click element: " + clickElementUrl);


            // ����url����ҳ�棬��ֻ��ʾUI
            webView.WebView.LoadUrl("https://www.bilibili.com" + clickElementUrl);
            await webView.WebView.WaitForNextPageLoadToFinish();
            string jsNewElement = "styleHide = document.createElement('style');";
            await webView.WebView.ExecuteJavaScript(jsNewElement);
            string jsSetStyle = "styleHide.innerHTML = '.rec-list{position: fixed;left: 0px;top: 0px;background: rgb(255, 255, 255);z-index: 100000000;width: 100vw;height: 100vh;overflow-y: scroll;} .left-container{display:none;} .bili-header{display:none;}'";
            await webView.WebView.ExecuteJavaScript(jsSetStyle);
            string jsAddElement = "document.head.appendChild(styleHide);";
            await webView.WebView.ExecuteJavaScript(jsAddElement);

            //string query = "getXPath(document.elementFromPoint(" + coordinationXofClick + "," + coordinationYofClick + "))";
            //string clickElement = await webView.WebView.ExecuteJavaScript(query);
            //Debug.Log("Click element: " + clickElement);

            // Network Communication
            //SendXPathScript.testClient.SendString(clickElement);

            //string queryColor = "document.elementFromPoint(" + coordinationXofClick + "," + coordinationYofClick + ").style.backgroundColor = 'red'";
            //await webView.WebView.ExecuteJavaScript(queryColor);

            //debugInfo = clickElement;
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
