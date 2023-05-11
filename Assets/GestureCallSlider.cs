using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using System;

public class GestureCallSlider : MonoBehaviour
{
    TextMeshPro tmp;

    GameObject slider;

    MixedRealityPose thumbTip, indexTip;

    public float distanceThreshhold = 0.02f;
    float distanceThumbIndex;
    DateTime disappearTime;

    private void Awake()
    {
        tmp = gameObject.AddComponent<TextMeshPro>();
        tmp.fontSize = 0.5f;
        tmp.transform.position = new Vector3(10f, -2.4f, 0.6f);
    }
    // Start is called before the first frame update
    void Start()
    {
        slider = GameObject.Find("SliderManager").GetComponent<OpenBiliPlayerSlider>().slider;
    }

    // Update is called once per frame
    void Update()
    {
        bool isThumbPresent = HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out thumbTip);
        bool isIndexPresent = HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out indexTip);
        distanceThumbIndex = Vector3.Distance(thumbTip.Position, indexTip.Position);
        tmp.text = "dis:" + distanceThumbIndex;
        Vector3 gesturePositon = (thumbTip.Position + indexTip.Position) / 2;

        DateTime nowTime = DateTime.Now;
        if ((distanceThumbIndex < distanceThreshhold) && isThumbPresent && isIndexPresent)
        {
            disappearTime = nowTime.AddSeconds(5);
            if (slider.activeSelf == false)//如果现在slider还没出现，就让它出现，并调整位置
            {
                slider.SetActive(true);
                slider.transform.position = gesturePositon + new Vector3(0, 0, 0.03f);
                // slider的方向：slider的z方向与camera的z方向对齐，且x轴水平
                Vector3 headsetForward = CameraCache.Main.transform.forward;
                float targetAngle = Mathf.Atan2(headsetForward.x, headsetForward.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
                slider.transform.rotation = targetRotation;
            }
        }
        else
        {
            if (nowTime > disappearTime) //如果已经一段时间没有出现该手势了，进度条就消失
                slider.SetActive(false);
        }
    }
}
