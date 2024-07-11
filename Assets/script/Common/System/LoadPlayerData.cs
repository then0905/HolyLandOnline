using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using static Equipment;
using System.Linq;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/07 
//  創建用途:讀取玩家資料(本地暫存) 
//==========================================

[Serializable]
public class AccountPlayerData
{
    public string UID;
    public string PlayerName;
    public string Job;
    public string Race;
    public int Hp;
    public int Mp;
    public int Exp;
    public int Lv;
    public int Coin;
    public List<MissionData> missionDataList = new List<MissionData>();        //接取的任務與進度資料
    public List<string> finishedMissionDataList = new List<string>();          //已完成的任務資料清單
    public List<EquipmentDataToJson> BagItemList = new List<EquipmentDataToJson>();       //背包資料清單
}

public class LoadPlayerData : MonoBehaviour
{
    private static AccountPlayerData accountPlayerData = new AccountPlayerData();

    /// <summary>
    /// 讀取使用者資料
    /// </summary>
    public static void LoadUserData()
    {
        //存取路徑
        string SaveData = Application.persistentDataPath + "/Usersave.txt";

        //若有檔案 讀取
        if (File.Exists(SaveData))
        {
            FileStream fs = new FileStream(Application.persistentDataPath + "/Usersave.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string sjon = sr.ReadToEnd();
            sr.Close();
            accountPlayerData = JsonUtility.FromJson<AccountPlayerData>(sjon);

            //讀取資料完 先讀取玩家基本資料
            PlayerDataOverView.Instance.PlayerData_.PlayerName = accountPlayerData.PlayerName;
            PlayerDataOverView.Instance.PlayerData_.Job = accountPlayerData.Job;
            PlayerDataOverView.Instance.PlayerData_.Race = accountPlayerData.Race;
            PlayerDataOverView.Instance.PlayerData_.Exp = accountPlayerData.Exp;
            PlayerDataOverView.Instance.PlayerData_.Lv = accountPlayerData.Lv;
            ItemManager.Instance.PickUp(accountPlayerData.Coin,true);       //不從PlayerData裡帶入值是因為這個方法可以更新玩家資料及背包UI文字
            ItemManager.Instance.Init(accountPlayerData.BagItemList);       //背包物品資料
            MissionManager.Instance.MissionList = accountPlayerData.missionDataList;
            MissionManager.Instance.FinishedMissionList = accountPlayerData.finishedMissionDataList;
        }
        //若沒有檔案 生成
        else
        {
            SaveUserData();
        }
    }

    /// <summary>
    /// 儲存本地檔案
    /// </summary>
    public static void SaveUserData()
    {
        var test = ItemManager.Instance.BagItems.Select(x => x.EquipmentDatas).ToList();
        CommonFunction.SaveLocalData(Application.persistentDataPath, "/Usersave.txt", accountPlayerData = new AccountPlayerData()
        {
            UID = "",
            PlayerName = PlayerDataOverView.Instance.PlayerData_.PlayerName,
            Job = PlayerDataOverView.Instance.PlayerData_.Job,
            Race = PlayerDataOverView.Instance.PlayerData_.Race,
            Hp = PlayerDataOverView.Instance.PlayerData_.HP,
            Mp = PlayerDataOverView.Instance.PlayerData_.MP,
            Exp = PlayerDataOverView.Instance.PlayerData_.Exp,
            Lv = PlayerDataOverView.Instance.PlayerData_.Lv,
            Coin = PlayerDataOverView.Instance.PlayerData_.Coin,
            missionDataList = MissionManager.Instance.MissionList,
            finishedMissionDataList = MissionManager.Instance.FinishedMissionList,
            BagItemList = ItemManager.Instance.BagItems.Select(x => x.EquipmentDatas.EquipmentDataToJson_).ToList()
        });
    }

    /// <summary>
    /// 寫入玩家UI屬性資料
    /// </summary>
    public static void LoadUserUiData()
    {
        PlayerDataOverView.Instance.PlayerData_.HP = accountPlayerData.Hp;
        PlayerDataOverView.Instance.PlayerData_.MP = accountPlayerData.Mp;
    }
}
