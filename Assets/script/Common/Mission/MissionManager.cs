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
    [SerializeField] private MissionItem missionItem;       //任務物件參考

    [Header("遊戲資料")]
    private List<MissionItem> tempMainMission = new List<MissionItem>();        //暫存接取的主線任務資料
    private List<MissionItem> tempSideMission = new List<MissionItem>();        //暫存接取的支線任務資料
    private List<MissionItem> tempDailyMission = new List<MissionItem>();        //暫存接取的每日任務資料
    public List<MissionItem> AlllMission
    {
        get
        {
            var totalMission = tempMainMission.Concat(tempSideMission).Concat(tempDailyMission).ToList();
            return totalMission;
        }
    }
    public List<MissionData> MissionList = new List<MissionData>();     //本地紀錄使用 紀錄已接取的任務進度
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
                tempMission = Instantiate(missionItem, missionItemSideOffset);
                tempSideMission.Add(tempMission);
                break;
            case "Daily":
                tempMission = Instantiate(missionItem, missionItemDailyOffset);
                tempDailyMission.Add(tempMission);
                break;
            default:
                break;
        }
        tempMission.QuestData = missionData;
        tempMission.MissionData = (missionRecordData);
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
        //任務進度刷新 本地紀錄資料刷新點
        LoadPlayerData.SaveUserData();
        Init();
    }

    /// <summary>
    /// 完成任務處理
    /// </summary>
    /// <param name="missionData"></param>
    public void FinishedMisstion(QuestDataModel missionData)
    {
        // 主線 支線 每日 任務清除
        var main = tempMainMission.Where(x => x.QuestData == missionData).FirstOrDefault();
        if (main?.gameObject != null) Destroy(main.gameObject);
        tempMainMission.Remove(main);
        var side = tempSideMission.Where(x => x.QuestData == missionData).FirstOrDefault();
        if (side?.gameObject != null) Destroy(side.gameObject);
        tempSideMission.Remove(side);
        var daily = tempDailyMission.Where(x => x.QuestData == missionData).FirstOrDefault();
        if (daily?.gameObject != null) Destroy(daily.gameObject);
        tempDailyMission.Remove(daily);

        //清除玩家身上接取任務的資料
        MissionList = MissionList.Where(x => x.MissionID != missionData.QuestID).ToList();
        //關閉任務詳細資訊物件
        missioninfo.gameObject.SetActive(false);
        //寫入任務完成紀錄清單
        FinishedMissionList.Add(missionData.QuestID);
        //任務完成 本地紀錄資料刷新點
        LoadPlayerData.SaveUserData();
    }

    /// <summary>
    /// 刪除當前預覽的任務
    /// </summary>
    public void DeleteMission()
    {
        QuestDataModel missionData = GameData.QuestDataDic[missioninfo.TempMissinoonID];
        // 主線 支線 每日 任務清除
        var main = tempMainMission.Where(x => x.QuestData == missionData).FirstOrDefault();
        if (main?.gameObject != null) Destroy(main.gameObject);
        tempMainMission.Remove(main);
        var side = tempSideMission.Where(x => x.QuestData == missionData).FirstOrDefault();
        if (side?.gameObject != null) Destroy(side.gameObject);
        tempSideMission.Remove(side);
        var daily = tempDailyMission.Where(x => x.QuestData == missionData).FirstOrDefault();
        if (daily?.gameObject != null) Destroy(daily.gameObject);
        tempDailyMission.Remove(daily);
        //清除玩家身上接取任務的資料
        MissionList = MissionList.Where(x => x.MissionID != missionData.QuestID).ToList();
        //關閉任務詳細資訊物件
        missioninfo.gameObject.SetActive(false);
        //刪除任務 本地紀錄資料刷新點
        LoadPlayerData.SaveUserData();
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
            foreach (var missionData in item.MissionData.MissionDataModel)
            {
                if (conditionID == missionData.MissionTarget)
                {
                    missionData.MissionSchedule = missionData.MissionSchedule + 1 >= item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount ?
                            item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount : missionData.MissionSchedule + 1;
                    //刷新任務詳細介面的資料
                    item.SetItem();
                    item.MissionBtn.onClick.Invoke();
                }
            }
        }

        //支線檢查
        foreach (var item in tempSideMission)
        {
            foreach (var missionData in item.MissionData.MissionDataModel)
            {
                if (conditionID == missionData.MissionTarget)
                {
                    missionData.MissionSchedule = missionData.MissionSchedule + 1 >= item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount ?
                            item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount : missionData.MissionSchedule + 1;
                    //刷新任務詳細介面的資料
                    item.SetItem();
                    item.MissionBtn.onClick.Invoke();
                }
            }
        }

        //每日檢查
        foreach (var item in tempDailyMission)
        {
            foreach (var missionData in item.MissionData.MissionDataModel)
            {
                {
                    if (conditionID == missionData.MissionTarget)
                    {
                        missionData.MissionSchedule = missionData.MissionSchedule + 1 >= item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount ?
                            item.QuestData.QuestConditionList.Find(x => x.ConditionID == conditionID).ConditionCount : missionData.MissionSchedule + 1;
                        //刷新任務詳細介面的資料
                        item.SetItem();
                        item.MissionBtn.onClick.Invoke();
                    }

                }
            }
            //任務進度刷新 本地紀錄資料刷新點
            LoadPlayerData.SaveUserData();
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
