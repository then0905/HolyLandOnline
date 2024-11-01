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
            SkillsPanelPage();
        }
        if (Input.GetKeyDown("b"))
        {
            BagPanelPage();
        }
        if (Input.GetKeyDown("c"))
        {
            PlayerDataPanelPage();
        }
        if (Input.GetKeyDown("q"))
        {
            MissionPanelPage();
        }
        if (Input.GetKeyDown("`"))
            OpenTargetPanel("TestItemDrop");

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
    /// <summary>
    /// 開關技能視窗
    /// </summary>
    public void SkillsPanelPage()
    {
        PanelData skillPanel = panelManager.PanelLsit.Where(x => x.PanelName.Contains("SkillPanel")).FirstOrDefault();
        if (!skillPanel.PanelObj.activeSelf)
        {
            panelManager.SetPanelOpen("SkillPanel");
        }
        else
        {
            panelManager.SetPanelClose("SkillPanel");
        }
    }
    /// <summary>
    /// 開關背包視窗
    /// </summary>
    public void BagPanelPage()
    {
        PanelData bagPanel = panelManager.PanelLsit.Where(x => x.PanelName.Contains("BagPanel")).FirstOrDefault();
        if (!bagPanel.PanelObj.activeSelf)
        {
            panelManager.SetPanelOpen("BagPanel");
        }
        else
        {
            panelManager.SetPanelClose("BagPanel");
        }
    }

    /// <summary>
    /// 開關角色資料視窗
    /// </summary>
    public void PlayerDataPanelPage()
    {
        PanelData playerDataPanel = panelManager.PanelLsit.Where(x => x.PanelName.Contains("PlayerDataPanel")).FirstOrDefault();
        if (!playerDataPanel.PanelObj.activeSelf)
        {
            panelManager.SetPanelOpen("PlayerDataPanel");
        }
        else
        {
            panelManager.SetPanelClose("PlayerDataPanel");
        }
    }
    /// <summary>
    /// 開關任務視窗
    /// </summary>
    public void MissionPanelPage()
    {
        PanelData playerDataPanel = panelManager.PanelLsit.Where(x => x.PanelName.Contains("MissionPanel")).FirstOrDefault();
        if (!playerDataPanel.PanelObj.activeSelf)
        {
            panelManager.SetPanelOpen("MissionPanel");
        }
        else
        {
            panelManager.SetPanelClose("MissionPanel");
        }
    }

    /// <summary>
    /// 開關指定視窗
    /// </summary>
    public void OpenTargetPanel(string panelName)
    {
        PanelData playerDataPanel = panelManager.PanelLsit.Where(x => x.PanelName.Contains(panelName)).FirstOrDefault();
        if (!playerDataPanel.PanelObj.activeSelf)
        {
            panelManager.SetPanelOpen(panelName);
        }
        else
        {
            panelManager.SetPanelClose(panelName);
        }
    }
}
