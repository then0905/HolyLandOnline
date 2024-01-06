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
        LoadPlayerData.LoadUserData();//帶入使用者資料 先帶入血量魔力以外的資料 
        StatusOperation.Instance.StatusMethod();//使用者資料刷新
        ClassAndSkill.Instance.Init();//技能視窗初始化 需要玩家的等級與職業資料 以及gamedata

        //(等PlayerData資料讀取完再讀取血量魔力的原因是因為
        //避免本地紀錄可能儲存buff之後的血量 大於沒有buff技能下的血量)

        LoadPlayerData.LoadUserUiData(); //再帶入本地紀錄的血量魔力 
        PlayerValueManager.Instance.Init();//玩家UI屬性上的更新 血量 魔力 經驗值 等級


        PlayerDataPanelProcessor.Instance.Init();   //角色介面資料初始
    }
}
