using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/08
//  創建用途:腳本初始化順序管理
//==========================================
public class InitSequenceManager : MonoBehaviour
{
    private void Awake()
    {
        GameData.Init();//GameData資料優先
        LoadPlayerData.LoadUserData();//帶入使用者資料 再帶入離線時紀錄的血量等資料及運算能力值 需要GameData資料
        StatusOperation.Instance.StatusMethod();//使用者資料刷新
        ClassAndSkill.Instance.Init();//技能視窗初始化 需要玩家的等級與職業資料 以及gamedata
        PlayerValueManager.Instance.Init();//玩家UI屬性上的更新 血量 魔力 經驗值 等級
    }
}
