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
    //技能欄底圖
    public Image[] SkillsHotKeyBackground;
    //技能欄CD計時圖
    public Image[] SkillsHotKeyFill;
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
        for (int i = 0; i < 10; i++)
        {
            SkillsHotKeyFill[i].sprite = SkillsHotKeyBackground[i].sprite;
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
}
