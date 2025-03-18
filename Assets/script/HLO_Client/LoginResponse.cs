using System;
using System.Collections.Generic;
//==========================================
//  創建者:家豪
//  創建日期:2025/03/14
//  創建用途: 登入帳號後的回應結構
//==========================================
public class LoginAccountResponse : HLO_ClientResponse
{ 
    /// <summary>
    /// 帳號
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 創建日期
    /// </summary>
    public DateTime CreatDateTime { get; set; }

    /// <summary>
    /// 最後登入日期
    /// </summary>
    public DateTime LastLoginTime { get; set; }

    /// <summary>
    /// 帳戶擁有的角色資料
    /// </summary>
    public List<CharacterDataModel>? CharacterList { get; set; } = new List<CharacterDataModel>();
}

/// <summary>
/// 角色資料結構
/// </summary>
public class CharacterDataModel
{
    /// <summary>
    /// 角色名稱
    /// </summary>
    public string CharacterName { get; set; }

    /// <summary>
    /// 等級
    /// </summary>
    public int Lv { get; set; }

    /// <summary>
    /// 最大血量
    /// </summary>
    public int MaxHP { get; set; }

    /// <summary>
    /// 當前血量
    /// </summary>
    public int HP { get; set; }

    /// <summary>
    /// 最大魔力
    /// </summary>
    public int MaxMP { get; set; }

    /// <summary>
    /// 當前魔力
    /// </summary>
    public int MP { get; set; }

    /// <summary>
    /// 當前經驗值
    /// </summary>
    public int EXP { get; set; }

    /// <summary>
    /// 職業
    /// </summary>
    public string Job { get; set; }

    /// <summary>
    /// 所屬種族
    /// </summary>
    public string Race { get; set; }

    /// <summary>
    /// 擁有的金錢
    /// </summary>
    public int Coin { get; set; }


    /// <summary>
    /// 角色創立日期
    /// </summary>
    public DateTime CharacterCreateTime { get; set; }

    /// <summary>
    /// 最後登入日期
    /// </summary>
    public DateTime LastLogintTime { get; set; }

    /// <summary>
    /// 最後遊玩的地圖
    /// </summary>
    public string? LastMapID { get; set; }

    /// <summary>最後存在的座標 X</summary>
    public float? LastPosX { get; set; }
    /// <summary>最後存在的座標 Y</summary>
    public float? LastPosY { get; set; }
    /// <summary>最後存在的座標 Z</summary>
    public float? LastPosZ { get; set; }

    /// <summary>
    /// 背包格數
    /// </summary>
    public int BagCount { get; set; }

    /// <summary>
    /// 背包資料
    /// </summary>
    public List<BagDataModel>? BagDataList { get; set; } = new List<BagDataModel>();

    /// <summary>
    /// 正穿著的裝備資料
    /// </summary>
    public List<EquipDataModel>? EquipDataList { get; set; } = new List<EquipDataModel>();

    /// <summary>
    /// 角色是否正在線上
    /// </summary>
    public bool IsOnline { get; set; }
}

/// <summary>
/// 角色背包物品資料結構
/// </summary>
public class BagDataModel
{

    /// <summary>
    /// 物品ID
    /// </summary>
    public string ItemID { get; set; }

    /// <summary>
    /// 物品類型 (武器、防具、道具....)
    /// </summary>
    public string Type { get; set; }


    /// <summary>
    /// 物品數量
    /// </summary>
    public int Qty { get; set; }

    /// <summary>
    /// 物品強化等級 (武器、防具適用)
    /// </summary>
    public int ForgeLv { get; set; }
}

/// <summary>
/// 裝備物品資料
/// </summary>
public class EquipDataModel
{
    /// <summary>
    /// 物品ID
    /// </summary>
    public string ItemID { get; set; }

    /// <summary>
    /// 物品類型 (武器、防具、道具....)
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 裝備穿戴部位
    /// </summary>
    public string wearingPart { get; set; }

    /// <summary>
    /// 物品數量
    /// </summary>
    public int Qty { get; set; }

    /// <summary>
    /// 物品強化等級 (武器、防具適用)
    /// </summary>
    public int ForgeLv { get; set; }
}
