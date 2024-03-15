using System;
using UnityEngine;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  角色基礎資訊
//==========================================

[Serializable]
public static partial class PlayerData
{
    [Header("玩家名稱")]
    public static string PlayerName;
    [Header("等級")]
    public static int Lv;
    [Header("最大血量")]
    public static int MaxHP;
    [Header("當前血量")]
    public static int HP;
    [Header("血量回復")]
    public static int HP_Recovery;
    [Header("最大魔力")]
    public static int MaxMP;
    [Header("當前魔力")]
    public static int MP;
    [Header("魔力回復")]
    public static int MP_Recovery;
    [Header("經驗量")]
    public static int Exp;
    [Header("最大經驗值")]
    public static int MaxExp;
    [Header("近距離攻擊力")]
    public static int MeleeATK;
    [Header("遠距離攻擊力")]
    public static int RemoteATK;
    [Header("魔法攻擊力")]
    public static int MageATK;
    [Header("總防禦值")]
    public static int DEF;
    [Header("總魔法防禦值")]
    public static int MDEF;
    [Header("傷害減緩")]
    public static int DamageReduction;
    [Header("總迴避值")]
    public static int Avoid;
    [Header("近距離命中值")]
    public static int MeleeHit;
    [Header("遠距離命中值")]
    public static int RemoteHit;
    [Header("魔法命中值")]
    public static int MageHit;
    [Header("暴擊率")]
    public static float Crt;
    [Header("暴擊抵抗")]
    public static float CrtResistance;
    [Header("暴擊傷害")]
    public static int CrtDamage;
    [Header("移動速度")]
    public static float Speed;
    [Header("職業")]
    public static string Job;
    [Header("種族")]
    public static string Race;
    [Header("力量")]
    public static int STR;
    [Header("敏捷")]
    public static int DEX;
    [Header("智慧")]
    public static int INT;
    [Header("靈巧")]
    public static int AGI;
    [Header("體力")]
    public static int VIT;
    [Header("感知")]
    public static int WIS;
    [Header("攻擊速度")]
    public static float AS;
    [Header("詠唱速度")]
    public static float CS;
    [Header("屬性傷害增幅")]
    public static float ElementDamageIncrease;
    [Header("屬性傷害抗性")]
    public static float ElementDamageReduction;
    [Header("異常狀態抗性")]
    public static float DisorderResistance;
    [Header("格檔率")]
    public static float BlockRate;
    [Header("玩家金幣量")]
    public static int Coin;
}
