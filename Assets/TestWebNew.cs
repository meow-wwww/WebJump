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

        // ����һ���̶�ֵ���Լ����á�
        float webViewBrowserUnityWidth = 0.55f;
        webView = WebViewPrefab.Instantiate(webViewBrowserUnityWidth, 3*webViewBrowserUnityWidth); //�ڶ���ֵ�Լ��裬��һ���޷�
        
        
        await webView.WaitUntilInitialized();
        webView.WebView.LoadUrl(htmlUrl);

        await webView.WebView.WaitForNextPageLoadToFinish();

        // debugInfo = "url:" + webView.WebView.Url;

        //string BrowserWidth = await wvp.WebView.ExecuteJavaScript("document.body.offsetWidth");
        int webViewBrowserPixelWidth = 1280; //������1080���� 1.��ƽ̨���ܲ�һ�� 2.��һ���޷�
        int webViewBrowserPixelHeight;

        //�����ֱ���
        webView.Resolution = webViewBrowserPixelWidth / webViewBrowserUnityWidth;

        //====================================================
        //��ȡԪ�ؿ��
        int elementPixelWidth = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetWidth"));//˳�㣬ԭhtml��trueд����
        //��ʱ���С����һ����ť���С���������֣��²���ǰ��ļ������£�
        //Ϊ�˱����������������� Ԫ�ؿ�� ����5%-10%
        elementPixelWidth = (int)(elementPixelWidth * 1.10);
        int elementPixelHeight = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetHeight"));

        print("ui w/h:" + elementPixelWidth + "," + elementPixelHeight);

        // ��������µĸ�
        float webViewBrowserUnityHeight = (float)elementPixelHeight / elementPixelWidth * webViewBrowserUnityWidth;
        webView.Resize(webViewBrowserUnityWidth, webViewBrowserUnityHeight);

        print("web unity w/h:" + webViewBrowserUnityWidth + "," + webViewBrowserUnityHeight);

        //====================================================
        //��ȡԪ�ؿ�� 2.0
        elementPixelWidth = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetWidth"));//˳�㣬ԭhtml��trueд����
        //��ʱ���С����һ����ť���С���������֣��²���ǰ��ļ������£�
        //Ϊ�˱����������������� Ԫ�ؿ�� ����5%-10%
        elementPixelWidth = (int)(elementPixelWidth * 1.05);
        elementPixelHeight = int.Parse(await webView.WebView.ExecuteJavaScript("document.querySelector(\"div[jumptag = \\\"ture\\\"]\").offsetHeight"));

        print("ui w/h 2.0:" + elementPixelWidth + "," + elementPixelHeight);

        // ��������µĸ�
        webViewBrowserUnityHeight = (float)elementPixelHeight / elementPixelWidth * webViewBrowserUnityWidth;
        webView.Resize(webViewBrowserUnityWidth, webViewBrowserUnityHeight);

        print("web unity w/h 2.0:" + webViewBrowserUnityWidth + "," + webViewBrowserUnityHeight);
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
