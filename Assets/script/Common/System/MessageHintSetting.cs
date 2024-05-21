using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/06
//  創建用途:系統提示訊息
//==========================================

/// <summary>
/// 提示框類型
/// </summary>
public enum HintType
{
    /// <summary>
    /// 獲取普通物品
    /// </summary>
    NormalItem,
    /// <summary>
    /// 警告訊息
    /// </summary>
    Warning,
    /// <summary>
    /// 獲取特殊物品
    /// </summary>
    SpecailItem,
    /// <summary>
    /// 恭賀訊息
    /// </summary>
    Congraduation
}

public class MessageHintSetting : MonoBehaviour
{
    public Canvas HintCanvas;
    public TextMeshProUGUI Text;

    /// <summary>
    /// 呼叫系統提示設定
    /// </summary>
    /// <param name="content">訊息內容</param>
    public void CallHintCanvas(string content, HintType hintType)
    {
        HintCanvas.worldCamera = Camera.main;
        Text.text = content;
        switch (hintType)
        {
            case HintType.NormalItem:
                Text.color = new Color(255,255,255);
                break;
            case HintType.Warning:
                Text.color = new Color(255, 0, 0);
                break;
            case HintType.SpecailItem:
                Text.color = new Color(229, 224, 126);
                break;
            case HintType.Congraduation:
                Text.color = new Color(225, 255, 0);
                break;
        }
    }

    /// <summary>
    /// 系統提示結束(掛在AnimationEvent)
    /// </summary>
    public void HintEnd()
    {
        if (this.gameObject != null)
            Destroy(this.gameObject);
    }
}
