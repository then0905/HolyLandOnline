using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

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

            //讀取資料完 載入玩家資料
            PlayerData.PlayerName = accountPlayerData.PlayerName;
            PlayerData.Job = accountPlayerData.Job;
            PlayerData.Race = accountPlayerData.Race;
            PlayerData.HP = accountPlayerData.Hp;
            PlayerData.MP = accountPlayerData.Mp;
            PlayerData.Exp = accountPlayerData.Exp;
            PlayerData.Lv = accountPlayerData.Lv;
        }
        //若沒有檔案 生成
        else
        {
            accountPlayerData = new AccountPlayerData()
            {
                UID = "",
                PlayerName = "",
                Job = PlayerData.Job,
                Race = PlayerData.Race,
                Hp = PlayerData.HP,
                Mp = PlayerData.MP,
                Exp = PlayerData.Exp,
                Lv = PlayerData.Lv,
            };
            string usersave = JsonUtility.ToJson(accountPlayerData);
            FileStream fs = new FileStream(Application.persistentDataPath + "/Usersave.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(usersave);
            sw.Close();
        }
    }

}
