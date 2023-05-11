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
            if (slider.activeSelf == false)//�������slider��û���֣����������֣�������λ��
            {
                slider.SetActive(true);
                slider.transform.position = gesturePositon + new Vector3(0, 0, 0.03f);
                // slider�ķ���slider��z������camera��z������룬��x��ˮƽ
                Vector3 headsetForward = CameraCache.Main.transform.forward;
                float targetAngle = Mathf.Atan2(headsetForward.x, headsetForward.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
                slider.transform.rotation = targetRotation;
            }
        }
        else
        {
            if (nowTime > disappearTime) //����Ѿ�һ��ʱ��û�г��ָ������ˣ�����������ʧ
                slider.SetActive(false);
        }
    }
}
