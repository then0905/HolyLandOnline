using System;
using UnityEngine;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/10
//  創建用途:  角色基礎資訊
//==========================================

[Serializable]
public partial class PlayerData
{
    [Header("玩家名稱")]
    public string PlayerName;
    [Header("等級")]
    public int Lv;
    [Header("最大血量")]
    public int MaxHP;
    [Header("當前血量")]
    public int HP;
    [Header("血量回復")]
    public int HP_Recovery;
    [Header("最大魔力")]
    public int MaxMP;
    [Header("當前魔力")]
    public int MP;
    [Header("魔力回復")]
    public int MP_Recovery;
    [Header("經驗量")]
    public int Exp;
    [Header("最大經驗值")]
    public int MaxExp;
    [Header("近距離攻擊力")]
    public int MeleeATK;
    [Header("遠距離攻擊力")]
    public int RemoteATK;
    [Header("魔法攻擊力")]
    public int MageATK;
    [Header("總防禦值")]
    public int DEF;
    [Header("總魔法防禦值")]
    public int MDEF;
    [Header("傷害減緩")]
    public int DamageReduction;
    [Header("總迴避值")]
    public int Avoid;
    [Header("近距離命中值")]
    public int MeleeHit;
    [Header("遠距離命中值")]
    public int RemoteHit;
    [Header("魔法命中值")]
    public int MageHit;
    [Header("暴擊率")]
    public float Crt;
    [Header("暴擊抵抗")]
    public float CrtResistance;
    [Header("暴擊傷害")]
    public int CrtDamage;
    [Header("移動速度")]
    public float Speed;
    [Header("職業")]
    public string Job;
    [Header("種族")]
    public string Race;
    [Header("力量")]
    public int STR;
    [Header("敏捷")]
    public int DEX;
    [Header("智慧")]
    public int INT;
    [Header("靈巧")]
    public int AGI;
    [Header("體力")]
    public int VIT;
    [Header("感知")]
    public int WIS;
    [Header("攻擊速度")]
    public float AS;
    [Header("詠唱速度")]
    public float CS;
    [Header("屬性傷害增幅")]
    public float ElementDamageIncrease;
    [Header("屬性傷害抗性")]
    public float ElementDamageReduction;
    [Header("異常狀態抗性")]
    public float DisorderResistance;
    [Header("格檔率")]
    public float BlockRate;
    [Header("玩家金幣量")]
    public int Coin;
}
