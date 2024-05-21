using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/26
//  創建用途: UI面板管理
//==========================================
[Serializable]
public class PanelData
{
    public string PanelName;
    public GameObject PanelObj;
}
public class PanelManager : MonoBehaviour
{
    #region 靜態變數
    private static PanelManager instance;
    public static PanelManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PanelManager>();
            return instance;
        }
    }
    #endregion 

    [Header("遊戲中所有面板資料"),SerializeField] List<PanelData> panelList = new List<PanelData>();
    public List<PanelData> PanelLsit
    {
        get
        {
            return panelList;
        }
    }
    /// <summary>
    /// 設定指定面板打開
    /// </summary>
    /// <param name="panelName">面板名稱</param>
    /// <param name="func">打開後是否附帶要執行的動作</param>
    public void SetPanelOpen(string panelName, Action func = null)
    {
        panelList.Where(x => x.PanelName.Contains(panelName)).FirstOrDefault().PanelObj.SetActive(true);
        if (func != null)
            func.Invoke();
    }
    /// <summary>
    /// 設定指定面板關閉
    /// </summary>
    /// <param name="panelName">面板名稱</param>
    /// <param name="func">打開後是否附帶要執行的動作</param>
    public void SetPanelClose(string panelName, Action func = null)
    {
        panelList.Where(x => x.PanelName.Contains(panelName)).FirstOrDefault().PanelObj.SetActive(false);
        if (func != null)
            func.Invoke();
    }
    /// <summary>
    /// 關閉所有面板
    /// </summary>
    public void CloseAllPanel()
    {
        PanelLsit.ForEach(x => x.PanelObj.SetActive(false));
    }

    /// <summary>
    /// 確認每個面板開關狀態
    /// </summary>
    /// <returns></returns>
    public bool CheckPanelStatus()
    {
        return PanelLsit.All(x => !x.PanelObj.activeSelf);
    }
}
