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

    /// <summary>
    /// GameData任務資料
    /// </summary>
    public QuestDataModel QuestData { get; set; }

    /// <summary>
    /// 任務進行進度記錄資料
    /// </summary>
    public List<MissionData> MissionData { get; set; } = new List<MissionData>();

    /// <summary>
    /// 設定任務內容
    /// </summary>
    public void SetItem()
    {
        missionTitle.text = QuestData.QuestName;
        SetItemTargetText(QuestData.QuestConditionList);
        missionBtn.onClick.AddListener(() =>
        {
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
            var getMissionQueryResult = MissionData.Where(x => x.MissionID == conditionDataList[i].QuestID).Select(x => x.MissionDataModel).FirstOrDefault();
            switch (conditionDataList[i].ConditionType)
            {
                //狩獵類型
                case "Hunting":
                    actionString = (i > 0) ? "\n狩獵 " : "狩獵 ";
                    targetString = GameData.MonstersDataDic[conditionDataList[i].ConditionID].Name;
                    scheduleString = " (" + getMissionQueryResult.Find(x => x.MissionTarget == conditionDataList[i].ConditionID).MissionSchedule.ToString() + "/" + conditionDataList[i].ConditionCount + ")";
                    break;
                //收集類型
                case "Collect":
                    actionString = (i > 0) ? "\n收集 " : "收集 ";
                    targetString = GameData.ItemsDic[conditionDataList[i].ConditionID].Name;
                    scheduleString = " (" + getMissionQueryResult.Find(x => x.MissionTarget == conditionDataList[i].ConditionID).MissionSchedule.ToString() + "/" + conditionDataList[i].ConditionCount + ")";
                    break;
                //交付類型
                case "Items":
                    actionString = (i > 0) ? "\n交付 " : "交付 ";
                    targetString = GameData.ItemsDic[conditionDataList[i].ConditionID].Name + " 給";
                    scheduleString = " (" + getMissionQueryResult.Find(x => x.MissionTarget == conditionDataList[i].ConditionID).MissionSchedule.ToString() + "/" + conditionDataList[i].ConditionCount + ")";
                    break;
                //代話類型
                case "Relay":
                    actionString = (i > 0) ? "\n傳話給 " : "傳話給 ";
                    targetString = GameData.ItemsDic[conditionDataList[i].ConditionID].Name;
                    scheduleString = " (" + getMissionQueryResult.Find(x => x.MissionTarget == conditionDataList[i].ConditionID).MissionSchedule.ToString() + "/" + conditionDataList[i].ConditionCount + ")";
                    break;
            }
            missionTarget.text += (actionString + targetString + scheduleString);
        }
    }
}
