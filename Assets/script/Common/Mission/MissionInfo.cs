using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//==========================================
//  創建者:家豪
//  創建日期:2024/05/25
//  創建用途: 任務介面詳細呈現
//==========================================

public class MissionInfo : MonoBehaviour
{
    [Header("遊戲物件")]
    [SerializeField] private TextMeshProUGUI misssionTitle;       //任務標題
    [SerializeField] private TextMeshProUGUI missionInfoText;       //任務詳細描述
    [SerializeField] private TextMeshProUGUI missionExp;       //任務經驗值資料
    [SerializeField] private TextMeshProUGUI missionCoin;       //任務金幣量資料
    [SerializeField] private TextMeshProUGUI missionSchedule;       //任務進度資料

    private string tempMissionID;
    public string TempMissinoID { get { return tempMissionID; } }

    /// <summary>
    /// 任務詳細資訊呈現設定
    /// </summary>
    /// <param name="missionitem"></param>
    public void MissionInfoSetting(MissionItem missionitem)
    {
        gameObject.SetActive(true);
        misssionTitle.text = missionitem.QuestData.QuestName;
        missionInfoText.text = "";
        missionitem.QuestData.QuestChatContent.ForEach(x => missionInfoText.text += x);
        missionSchedule.text = missionitem.MissionScheduleText;
        missionExp.text = "經驗值: " + missionitem.QuestData.Exp.ToString();
        missionCoin.text = "金幣: " + missionitem.QuestData.Coin.ToString();
        tempMissionID = missionitem.QuestData.QuestID;
    }
}
