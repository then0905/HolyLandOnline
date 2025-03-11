using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2025/03/11
//  創建用途: 通用詢問視窗設定
//==========================================
public class CommonInquiryWindowSetting : MonoBehaviour
{
    [Header("遊戲物件")]
    [SerializeField] protected TMP_Text contentText;
    [SerializeField] protected TMP_Text titleText;
    [SerializeField] protected Button yesButton;
    [SerializeField] protected Button cancelButton;

    /// <summary>
    /// 初始化設定
    /// </summary>
    /// <param name="contentStr">提示框文字</param>
    /// <param name="button1">按鈕1執行內容</param>
    /// <param name="button2">按鈕2執行內容</param>
    public void Init(string titleStr, string contentStr, Action button1, Action button2)
    {
        //設定標題文字
        titleText.text = titleStr;
        //設定內容文字
        contentText.text = contentStr;
        //設定確定按鈕內容
        yesButton.onClick.AddListener(() =>
        {
            button1?.Invoke();
            DestroyWindow();
        });
        //設定取消按鈕內容
        cancelButton.onClick.AddListener(() =>
        {
            button2?.Invoke();
            DestroyWindow();
        });
    }

    /// <summary>
    /// 關閉視窗處理 刪除視窗物件
    /// </summary>
    protected void DestroyWindow()
    {
        if (gameObject != null)
            Destroy(this.gameObject);
    }
}
