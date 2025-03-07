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

    //快捷鍵底圖原圖
    public Sprite HotKeyBackgroundSprite;
    //面板管理器
    private PanelManager panelManager;

    [Header("快捷鍵清單")]
    public HotKeyData[] HotKeyArray = new HotKeyData[10];

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
        //按下快捷鍵
        if (Input.GetKeyDown("1"))
        {
            HotKeyArray[0].HotKeyDataUse();
        }
        if (Input.GetKeyDown("2"))
        {
            HotKeyArray[1].HotKeyDataUse();
        }
        if (Input.GetKeyDown("3"))
        {
            HotKeyArray[2].HotKeyDataUse();
        }
        if (Input.GetKeyDown("4"))
        {
            HotKeyArray[3].HotKeyDataUse();
        }
        if (Input.GetKeyDown("5"))
        {
            HotKeyArray[4].HotKeyDataUse();
        }
        if (Input.GetKeyDown("6"))
        {
            HotKeyArray[5].HotKeyDataUse();
        }
        if (Input.GetKeyDown("7"))
        {
            HotKeyArray[6].HotKeyDataUse();
        }
        if (Input.GetKeyDown("8"))
        {
            HotKeyArray[7].HotKeyDataUse();
        }
        if (Input.GetKeyDown("9"))
        {
            HotKeyArray[8].HotKeyDataUse();
        }
        if (Input.GetKeyDown("0"))
        {
            HotKeyArray[9].HotKeyDataUse();
        }
        // 未選取任何目標 按下esc時 關閉所有面板
        if (Input.GetKeyDown(KeyCode.Escape) && !SelectTarget.Instance.CatchTarget)
        {
            panelManager.CloseAllPanel();
        }
    }

    /// <summary>
    /// 設定快捷鍵時的前置處理(檢查有無已設定的相同資料並清除)
    /// </summary>
    public void PrepareHotKeySetting(string id)
    {
        //先檢查快捷鍵上是否已有此資料 有的話清除
        bool queryResult = HotKeyArray.Any(x => x.TempHotKeyData?.KeyID == id);

        if (queryResult)
        {
            var targetQueryResult = HotKeyArray.Where(x => x.TempHotKeyData?.KeyID == id).ToList();
            targetQueryResult.ForEach(x => x.ClearHotKeyData());
        }
    }
}
