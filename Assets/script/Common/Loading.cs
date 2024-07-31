using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//==========================================
//  創建者:家豪
//  創建日期:2024/07/24
//  創建用途: 場景Loading腳本
//==========================================
public class Loading : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;   //Loading畫面
    [SerializeField] private Slider progressBar;        //Loading進度Bar
    [SerializeField] private TextMeshProUGUI progressText;      //Loading進度Bar文字

    /// <summary>
    /// 顯示 Loading 加載中...
    /// </summary>
    public void Show()
    {
        loadingPanel.SetActive(true);
    }

    /// <summary>
    /// 關閉 Loading 加載中...
    /// </summary>
    public void Hide()
    {
        loadingPanel.SetActive(false);
    }

    /// <summary>
    /// 更新 Loaing進度Bar
    /// </summary>
    /// <param name="progress"></param>
    public void UpdateProgress(float progress)
    {
        progressBar.value = progress;
        progressText.text = $"Loading... {progress * 100:F0}%";
    }
}
