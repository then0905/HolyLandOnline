using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/06
//  創建用途:系統提示訊息
//==========================================
public class MessageHintSetting : MonoBehaviour
{
    public Canvas HintCanvas;
    public TextMeshProUGUI Text;

    /// <summary>
    /// 呼叫系統提示設定
    /// </summary>
    /// <param name="content">訊息內容</param>
    public void CallHintCanvas(string content)
    {
        HintCanvas.worldCamera = Camera.main;
        Text.text = content;
    }

    /// <summary>
    /// 系統提示結束(掛在AnimationEvent)
    /// </summary>
    public void HintEnd()
    {
        Destroy(this.gameObject);
    }
}
