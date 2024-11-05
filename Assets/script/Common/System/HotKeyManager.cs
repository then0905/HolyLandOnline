using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/07/05
//  創建用途:  快捷鍵管理器
//==========================================

public class HotKeyManager : MonoBehaviour
{
    #region 靜態變數
    private static HotKeyManager instance;
    public static HotKeyManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<HotKeyManager>();
            return instance;
        }
    }
    #endregion 

    //快捷鍵底圖Img
    public Image[] HotKeyBackgroundArray;
    //快捷鍵計時圖Img
    public Image[] HotKeyFillArray;
    //快捷鍵底圖原圖
    public Sprite HotKeyBackgroundSprite;
    //面板管理器
    private PanelManager panelManager;
    private void Start()
    {
        panelManager = PanelManager.Instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            panelManager.OpenTargetPanel("SkillPanel");
        }
        if (Input.GetKeyDown("b"))
        {
            panelManager.OpenTargetPanel("BagPanel");
        }
        if (Input.GetKeyDown("c"))
        {
            panelManager.OpenTargetPanel("PlayerDataPanel");
        }
        if (Input.GetKeyDown("q"))
        {
            panelManager.OpenTargetPanel("MissionPanel");
        }
        if (Input.GetKeyDown("`"))
            panelManager.OpenTargetPanel("TestItemDrop");

        // 未選取任何目標 按下esc時 關閉所有面板
        if (Input.GetKeyDown(KeyCode.Escape) && !SelectTarget.Instance.CatchTarget)
        {
            panelManager.CloseAllPanel();
        }
        for (int i = 0; i < 10; i++)
        {
            HotKeyFillArray[i].sprite = HotKeyBackgroundArray[i].sprite;
        }
    }
}
