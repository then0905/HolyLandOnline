using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
//==========================================
//  創建者:家豪
//  創建日期:2024/05/25
//  創建用途: 任務視窗子物件
//==========================================

/// <summary>
/// 任務進度資料細部結構
/// </summary>
[Serializable]
public class MissionDataModel
{
    /// <summary>
    /// 任務類型
    /// </summary>
    public string MissionType;
    /// <summary>
    /// 任務目標
    /// </summary>
    public string MissionTarget;
    /// <summary>
    /// 任務進度
    /// </summary>
    public int MissionSchedule;
}

/// <summary>
/// 任務進度資料 記錄用
/// </summary>
[Serializable]
public class MissionData
{
    /// <summary>
    /// 任務ID
    /// </summary>
    public string MissionID;
    /// <summary>
    /// 
    /// </summary>
    public List<MissionDataModel> MissionDataModel;
}

public class MissionItem : MonoBehaviour
{
    [Header("遊戲物件")]
    [SerializeField] private TextMeshProUGUI missionTitle;       //任務物件標題
    [SerializeField] private TextMeshProUGUI missionTarget;       //任務物件目標
    [SerializeField] private Button missionBtn;       //任務按鈕功能
    //進度文字
    public string MissionScheduleText => missionTarget.text;
    public Button MissionBtn => missionBtn;

    /// <summary> 取得任務是否 是完成的bool </summary>
    public bool GetMissionFinishStatus
    {
        get
        {
            return MissionData.MissionDataModel.All(z => QuestData.QuestConditionList.All(y => (y.ConditionID == z.MissionTarget && y.ConditionCount == z.MissionSchedule) || y.ConditionType == "Relay"));
        }
    }

    /// <summary>
    /// GameData任務資料
    /// </summary>
    public QuestDataModel QuestData { get; set; }

    /// <summary>
    /// 任務進行進度記錄資料
    /// </summary>
    public MissionData MissionData { get; set; }

    /// <summary>
    /// 設定任務內容
    /// </summary>
    public void SetItem()
    {
        missionTitle.text = QuestData.QuestName.GetText();
        SetItemTargetText(QuestData.QuestConditionList);
        missionBtn.onClick.AddListener(() =>
        {
            //設定任務按鈕內容 點擊後 執行任務詳細內容介紹
            MissionManager.Instance.MissionItemInfo?.Invoke(null, this);
        });
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 任務條件描述文字處理
    /// </summary>
    /// <param name="conditionDataList"></param>
    public void SetItemTargetText(List<QuestConditionData> conditionDataList)
    {
        missionTarget.text = "";
        string actionString = "";       //動作詞
        string targetString = "";       //任務目標
        string scheduleString = "";     //進度文字

        for (int i = 0; i < conditionDataList.Count; i++)
        {
            var getMissionQueryResult = MissionData.MissionDataModel;
            var str = string.Empty;
            switch (conditionDataList[i].ConditionType)
            {
                //狩獵類型
                case "Hunting":
                    str = "TM_Hunting".GetText();
                    actionString = (i > 0) ? string.Format("\n{0} ", str) : (str + " ");
                    targetString = GameData.MonstersDataDic[conditionDataList[i].ConditionID].Name.GetText();
                    scheduleString = " (" + getMissionQueryResult.Find(x => x.MissionTarget == conditionDataList[i].ConditionID).MissionSchedule.ToString() + "/" + conditionDataList[i].ConditionCount + ")";
                    break;
                //收集類型
                case "Collect":
                    str = "TM_Collect".GetText();
                    actionString = (i > 0) ? string.Format("\n{0} ", str) : (str + " ");
                    targetString = GameData.ItemsDic[conditionDataList[i].ConditionID].Name.GetText();
                    scheduleString = " (" + getMissionQueryResult.Find(x => x.MissionTarget == conditionDataList[i].ConditionID).MissionSchedule.ToString() + "/" + conditionDataList[i].ConditionCount + ")";
                    break;
                //交付類型
                case "Items":
                    str = "TM_Deliver".GetText();
                    actionString = (i > 0) ? string.Format("\n{0} ", str) : (str + " ");
                    targetString = GameData.ItemsDic[conditionDataList[i].ConditionID].Name.GetText() + " 給";
                    scheduleString = " (" + getMissionQueryResult.Find(x => x.MissionTarget == conditionDataList[i].ConditionID).MissionSchedule.ToString() + "/" + conditionDataList[i].ConditionCount + ")";
                    break;
                //代話類型
                case "Relay":
                    str = "TM_SendMessageTo".GetText();
                    actionString = (i > 0) ? string.Format("\n{0} ", str) : (str + " ");
                    targetString = GameData.NpcDataDic[conditionDataList[i].ConditionID].NpcName.GetText();
                    scheduleString = " (" + getMissionQueryResult.Find(x => x.MissionTarget == conditionDataList[i].ConditionID).MissionSchedule.ToString() + "/" + conditionDataList[i].ConditionCount + ")";
                    break;
            }
            missionTarget.text += (actionString + targetString + scheduleString);
        }
    }
}
