using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class OpenBiliPlayerSlider : MonoBehaviour
{
    public GameObject prefab;
    public GameObject slider;
    SendString SendSliderValueScript;
    PinchSlider PinchSliderComponent;

    TextMeshPro tmp;

    private void Awake()
    {
        Invoke("GetSendSliderValueScript", 1f);
        InvokeRepeating("GetSliderPositionFromPC", 5f, 3f); // 从第5秒开始，每3秒更新一次slider的位置
    }

    void GetSendSliderValueScript()
    {
        // Network Communication
        SendSliderValueScript = GameObject.Find("WebManager").GetComponent<SendString>();
    }

    void GetSliderPositionFromPC()
    {
        try
        {
            string response = SendSliderValueScript.testClient.SendStringMessage("currentPosition");
            PinchSliderComponent.SliderValue = float.Parse(response);
        }
        catch {; }
    }

    // Start is called before the first frame update
    void Start()
    {
        slider = Instantiate(prefab);
        PinchSliderComponent = slider.GetComponent<PinchSlider>();

        // add EventListener
        PinchSliderComponent.OnInteractionEnded.AddListener(SendSliderValue);

        //slider.SetActive(false);

        tmp = gameObject.AddComponent<TextMeshPro>();
        tmp.fontSize = 1;
        tmp.transform.position = new Vector3(10f, -2.4f, 0.6f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //// 两个测试用的函数 没啥用
    //public void isDragging()
    //{
    //    tmp.text = "is dragging";
    //}
    //public void endDragging()
    //{
    //    tmp.text = "end dragging";
    //}
    public void SendSliderValue(SliderEventData data)
    {
        Debug.Log("data new value:" + data.NewValue);
        SendSliderValueScript.testClient.SendStringMessage(data.NewValue.ToString());
    }
}
