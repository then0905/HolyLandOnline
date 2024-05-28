using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking.PlayerConnection;
using System.Linq;

//==========================================
//  創建者:家豪
//  創建日期:2024/05/25
//  創建用途: 任務管理器
//==========================================
public class MissionManager : MonoBehaviour
{
    #region 全域靜態變數

    public static MissionManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MissionManager>();
            return instance;
        }
    }

    private static MissionManager instance;

    #endregion

    [Header("遊戲物件")]
    [SerializeField] private Transform missionItemMainOffset;       //主線任務物件生成的參考
    [SerializeField] private Transform missionItemSideOffset;       //支線任務物件生成的參考
    [SerializeField] private Transform missionItemDailyOffset;       //每日任務物件生成的參考
    [SerializeField] private MissionInfo missioninfo;       //任務詳細資料介面

    [Header("遊戲資料")]
    [SerializeField] private MissionItem missionItem;       //任務物件參考
    private List<MissionItem> tempMainMission = new List<MissionItem>();        //暫存接取的主線任務資料
    private List<MissionItem> tempSideMission = new List<MissionItem>();        //暫存接取的支線任務資料
    private List<MissionItem> tempDailyMission = new List<MissionItem>();        //暫存接取的每日任務資料
    public List<MissionData> MissionList = new List<MissionData>();
    public List<string> FinishedMissionList = new List<string>();       //紀錄已完成的任務清單

    //事件區 
    public EventHandler<MissionItem> MissionItemInfo;        //詳細任務事件
    public EventHandler<string> MissionScheduleCheck;        //任務進度檢查事件

    private void OnEnable()
    {
        MissionItemInfo += SetMissionInfo;
        MissionScheduleCheck += MissionSheduleCheck;
    }
    private void OnDisable()
    {
        MissionItemInfo -= SetMissionInfo;
        MissionScheduleCheck -= MissionSheduleCheck;
    }

    /// <summary>
    /// 任務內容初始化
    /// </summary>
    public void Init()
    {
        ResetMissionData();
        if (MissionList != null && MissionList.Count > 0)
        {
            foreach (var item in MissionList)
            {
                QuestDataModel missionData = GameData.QuestDataDic[item.MissionID];
                SpawnMissionItem(missionData, item);
            }
        }
    }

    /// <summary>
    /// 生成任務物件
    /// </summary>
    /// <param name="missionData"></param>
    public void SpawnMissionItem(QuestDataModel missionData, MissionData missionRecordData)
    {
        MissionItem tempMission = null;
        switch (missionData.QuestType)
        {
            case "Main":
                tempMission = Instantiate(missionItem, missionItemMainOffset);
                tempMainMission.Add(tempMission);
                break;
            case "Side":
                tempMission = Instantiate(missionItem, missionItemMainOffset);
                tempSideMission.Add(tempMission);
                break;
            case "Daily":
                tempMission = Instantiate(missionItem, missionItemMainOffset);
                tempDailyMission.Add(tempMission);
                break;
            default:
                break;
        }
        tempMission.QuestData = missionData;
        tempMission.MissionData.Add(missionRecordData);
        tempMission.SetItem();
    }

    /// <summary>
    /// 接受新任務
    /// </summary>
    /// <param name="id"></param>
    /// <param name="conditionDataList"></param>
    public void AcceptMisstion(string id, List<QuestConditionData> conditionDataList)
    {
        foreach (var item in conditionDataList)
        {
            if (!MissionList.Any(x => x.MissionID == id))
                MissionList.Add(new MissionData()
                {
                    MissionID = id,
                    MissionDataModel = new List<MissionDataModel>()
                });

            MissionList.Find(x => x.MissionID == id).MissionDataModel.Add(new MissionDataModel()
            {
                MissionType = item.ConditionType,
                MissionTarget = item.ConditionID,
                MissionSchedule = 0,
            });
        }
        Init();
    }

    public void FinishedMisstion(string id)
    {

        //    if (tempMainMission.Count > 0)
        //        tempMainMission.ForEach(x => Destroy(x.gameObject));
        //tempMainMission.Clear();
        //if (tempSideMission.Count > 0)
        //    tempSideMission.ForEach(x => Destroy(x.gameObject));
        //tempSideMission.Clear();
        //if (tempDailyMission.Count > 0)
        //    tempDailyMission.ForEach(x => Destroy(x.gameObject));
        //tempDailyMission.Clear();
    }


    public void SetMissionInfo(object o, MissionItem missionitem)
    {
        missioninfo.MissionInfoSetting(missionitem);
    }

    /// <summary>
    /// 任務進度處理
    /// </summary>
    /// <param name="o"></param>
    /// <param name="missionID"></param>
    public void MissionSheduleCheck(object o, string conditionID)
    {
        //主線檢查
        foreach (var item in tempMainMission)
        {
            foreach (var missionData in item.MissionData)
            {
                foreach (var conditaionData in missionData.MissionDataModel)
                {
                    if (conditionID == conditaionData.MissionTarget)
                    {
                        conditaionData.MissionSchedule = conditaionData.MissionSchedule + 1 >= item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount ?
                            item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount : conditaionData.MissionSchedule + 1;
                        //刷新任務詳細介面的資料
                        item.SetItem();
                        item.MissionBtn.onClick.Invoke();
                    }
                }
            }
        }

        //支線檢查
        foreach (var item in tempSideMission)
        {
            foreach (var missionData in item.MissionData)
            {
                foreach (var conditaionData in missionData.MissionDataModel)
                {
                    if (conditionID == conditaionData.MissionTarget)
                    {
                        conditaionData.MissionSchedule = conditaionData.MissionSchedule + 1 >= item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount ?
                            item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount : conditaionData.MissionSchedule + 1;
                        //刷新任務詳細介面的資料
                        item.SetItem();
                        item.MissionBtn.onClick.Invoke();
                    }
                }
            }
        }

        //每日檢查
        foreach (var item in tempDailyMission)
        {
            foreach (var missionData in item.MissionData)
            {
                foreach (var conditaionData in missionData.MissionDataModel)
                {
                    if (conditionID == conditaionData.MissionTarget)
                    {
                        conditaionData.MissionSchedule = conditaionData.MissionSchedule + 1 >= item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount ?
                            item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount : conditaionData.MissionSchedule + 1;
                        //刷新任務詳細介面的資料
                        item.SetItem();
                        item.MissionBtn.onClick.Invoke();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 重製任務資料
    /// </summary>
    private void ResetMissionData()
    {
        if (tempMainMission.Count > 0)
            tempMainMission.ForEach(x => Destroy(x.gameObject));
        tempMainMission.Clear();
        if (tempSideMission.Count > 0)
            tempSideMission.ForEach(x => Destroy(x.gameObject));
        tempSideMission.Clear();
        if (tempDailyMission.Count > 0)
            tempDailyMission.ForEach(x => Destroy(x.gameObject));
        tempDailyMission.Clear();
    }
}
