using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//==========================================
//  創建者:家豪
//  創建日期:2025/03/16
//  創建用途: 頁籤常用行為
//==========================================
public class ToggleBehavior : MonoBehaviour
{
    [Header("Toggle Event")]
    [SerializeField] private UnityEvent unityEvent;

    public void EventGo(bool isOn)
    {
        if (isOn)
        {
            unityEvent?.Invoke();
        }
    }
}
