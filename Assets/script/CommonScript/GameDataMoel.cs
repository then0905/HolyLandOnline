
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
//==========================================
//  創建者:    家豪
//  創建日期:  2023/05/03
//  創建用途:  Json資料結構
//==========================================
namespace JsonDataModel
{
    public interface IDictionaryData<T> where T : IConvertible
    {
        T GetKey { get; }
    }

    #region 資料結構
    /// <summary>
    /// 掉落物資料
    /// </summary>結構
    public class BootyDataModel : IDictionaryData<string>
    {
        public string Area { get; set; }            //地區
        public string AreaID { get; set; }            //地區ID
        public string Name { get; set; }            //怪物名稱
        public string CodeID { get; set; }          //怪物流水號
        public int CoinMin { get; set; }            //金幣最小值
        public int CoinMax { get; set; }            //金幣最大值
        public string Bootys { get; set; }            //掉落物品清單
        public string BootysChance { get; set; }            //掉落物品機率清單
        public string GetKey { get { return CodeID; } }
    }
    /// <summary>
    /// 防具資料結構
    /// </summary>
    public class ArmorDataModel : IDictionaryData<string>
    {
        public int NeedLv { get; set; }            // 所需等級
        public string Name { get; set; }            // 防具名稱
        public string CodeID { get; set; }            // 防具ID
        public string Classification { get; set; }            // 道具分類
        public string ClassificationID { get; set; }            // 道具分類ID
        public string WearPart { get; set; }            // 穿戴部位
        public string WearPartID { get; set; }            // 穿戴部位ID
        public string Type { get; set; }            // 防具類型
        public string TypeID { get; set; }            // 防具類型ID      
        public string Intro { get; set; }            // 防具介紹
        public int DEF { get; set; }            // 防具物理防禦值
        public int Avoid { get; set; }            // 防具迴避值
        public int MDEF { get; set; }            // 防具魔法防禦值
        public float Speed { get; set; }            // 防具移動速度影響
        public float CrtResistance { get; set; }            // 防具暴擊抵抗
        public int DamageReduction { get; set; }            // 防具傷害減緩
        public int HP { get; set; }            // 防具HP加成
        public int MP { get; set; }            // 防具MP加成
        public int HpRecovery { get; set; }            // 防具HP自然回復
        public int MpRecovery { get; set; }            // 防具MP自然回復
        public int STR { get; set; }            // 防具STR加成
        public int DEX { get; set; }            // 防具DEX加成
        public int INT { get; set; }            // 防具INT加成
        public int AGI { get; set; }            // 防具AGI加成
        public int VIT { get; set; }            // 防具VIT加成
        public int WIS { get; set; }            // 防具WIS加成
        public float ElementDamageReduction { get; set; }            // 防具屬性傷害抵抗
        public float DisorderResistance { get; set; }            // 防具異常狀態抗性
        public string GetKey
        {
            get { return CodeID; }
        }
    }
    /// <summary>
    /// 技能頁面上的資料結構
    /// </summary>
    public class SkillUIModel : IDictionaryData<string>
    {
        public string Job { get; set; }               // 職業
        public string Name { get; set; }            // 技能名稱
        public string SkillID { get; set; }                     // 技能ID
        public int NeedLv { get; set; }            // 技能所需等級
        public bool Characteristic { get; set; }            // 技能特性
        public int CastMage { get; set; }            // 技能花費魔力
        public float CD { get; set; }            // 技能冷卻時間
        public string AnimaTrigger { get; set; }            // 技能動畫名稱
        public string Type { get; set; }            // 技能類型
        public float Distance { get; set; }            // 技能施放距離
        public float Width { get; set; }            // 技能命中寬度
        public float Height { get; set; }            // 技能命中長度
        public float CircleDistance { get; set; }            // 圓圈型技能命中範圍
        public float Damage { get; set; }            // 技能傷害倍率
        public string AdditionMode { get; set; }            // 技能攻擊模式
        public string Intro { get; set; }            // 技能說明
        public string GetKey { get { return SkillID; } }
    }

    /// <summary>
    /// 技能施放進行運算所需的資料結構
    /// </summary>
    public class SkillDataModel : IDictionaryData<string>
    {
        public string Name { get; set; }            // 技能名稱
        public string SkillID { get; set; }                     // 技能ID
        public bool Characteristic { get; set; }                // True:主動、False:被動
        public int MultipleValue { get; set; }                  // 多段傷害的次數 次數大於1需要填

        public string EffectValue { get; set; }            // 效果值 
        public string InfluenceStatus { get; set; }       // 效果影響的屬性 (Buff)   
        public string AddType { get; set; }               // 加成運算的方式 Rate:乘法、Value:加法   
        public string EffectCategory { get; set; }              // 標籤類型    
        public string AdditionalEffect { get; set; }      // 額外附加效果標籤
        public string AdditionalEffectValue { get; set; }  // 額外附加效果的值
        public string AdditionalEffectTime { get; set; }   // 額外附加效果持續時間
        public string Condition { get; set; }             // 執行技能需要的條件 不需要不用填 
        public int EffectRecive { get; set; }
        public int TargetCount { get; set; }                    // 目標數量 -4:範圍內所有怪物-3:範圍內所有敵軍、-2:範圍內所有敵方目標、-1:隊友與自身、0:自己
        public float EffectDurationTime { get; set; }           // 效果持續時間
        public float ChantTime { get; set; }                    // 詠唱時間

        public string AdditionMode { get; set; }                // 攻擊模式 戰鬥計算防禦方面使用 (近距離物理、遠距離物理找物防:魔法找魔防)
        public float Distance { get; set; }                     // 技能範圍(施放者與目標間的距離值)
        public float Width { get; set; }                        // 矩形範圍的寬
        public float Height { get; set; }                       // 矩形範圍的長度
        public float CircleDistance { get; set; }               // 圓形範圍 
        public string GetKey { get { return SkillID; } }
    }
    /// <summary>
    /// 武器資料
    /// </summary>結構
    public class WeaponDataModel : IDictionaryData<string>
    {
        public int LV { get; set; }            // 所需等級
        public string Name { get; set; }            // 武器名稱
        public string CodeID { get; set; }            // 武器ID
        public string Classification { get; set; }            // 道具分類
        public string ClassificationID { get; set; }            // 道具分類ID
        public string TackHand { get; set; }            // 拿取部位
        public string TackHandID { get; set; }            // 拿取部位ID
        public string Type { get; set; }            // 武器類型
        public string TypeID { get; set; }            // 武器類型ID
        public string Intro { get; set; }            // 武器介紹
        public string AS { get; set; }            // 武器攻擊速度
        public int MeleeATK { get; set; }            // 武器近距離攻擊
        public int MeleeHit { get; set; }            // 武器近距離命中
        public int RemoteATK { get; set; }            // 武器遠距離攻擊
        public int RemoteHit { get; set; }            // 武器遠距離命中
        public int MageATK { get; set; }            // 武器魔法攻擊力
        public int MageHit { get; set; }            // 武器魔法命中
        public int DEF { get; set; }            // 盾牌物理防禦值
        public int MDEF { get; set; }            // 盾牌魔法防禦值
        public int Avoid { get; set; }            // 盾牌迴避值
        public int HP { get; set; }            // 盾牌最大生命值
        public int MP { get; set; }            // 盾牌最大魔法值
        public int BlockRate { get; set; }            // 盾牌格檔率
        public float Crt { get; set; }            // 武器暴擊率
        public int CrtDamage { get; set; }            // 武器暴擊傷害
        public int STR { get; set; }            // 武器STR
        public int DEX { get; set; }            // 武器DEX
        public int INT { get; set; }            // 武器INT
        public int AGI { get; set; }            // 武器AGI
        public int VIT { get; set; }            // 武器VIT
        public int WIS { get; set; }            // 武器WIS
        public string GetKey { get { return CodeID; } }
    }
    /// <summary>
    /// 道具資料
    /// </summary>
    public class ItemDataModel : IDictionaryData<string>
    {
        public int LV { get; set; }            // 所需等級
        public string Name { get; set; }            // 道具名稱
        public string CodeId { get; set; }            // 道具ID
        public string TakeHand { get; set; }            // 拿取部位
        public string TakeHandID { get; set; }            // 拿取部位ID
        public string Classification { get; set; }            // 道具分類
        public string ClassificationID { get; set; }            // 道具分類ID
        public string Type { get; set; }            // 道具類型
        public string TypeID { get; set; }            // 道具類型ID
        public string Intro { get; set; }            // 道具介紹
        public string Volume { get; set; }            // 道具作用值
        public float CD { get; set; }            // 道具冷卻時間
        public int ActionTime { get; set; }            // 道具持續時間
        public int Price { get; set; }            // 道具在商店販賣的價格
        public int Redeem { get; set; }            // 道具賣給商店的價格
        public string GetKey { get { return Name; } }
    }
    /// <summary>
    /// 怪物資料
    /// </summary>
    public class MonsterDataModel : IDictionaryData<string>
    {
        public string Area { get; set; }            // 所在區域
        public string AreaID { get; set; }            // 所在區域
        public string Name { get; set; }            // 怪物名稱
        public string CodeID { get; set; }            // 怪物ID
        public int Lv { get; set; }            // 怪物等級
        public string Class { get; set; }            // 怪物階級
        public string ClassID { get; set; }            // 怪物階級ID
        public string Type { get; set; }            // 怪物類型
        public string TypeID { get; set; }            // 怪物類型ID
        public int SpawnAmount { get; set; }            // 怪物生成量
        public string SpawnPos { get; set; }            // 怪物生成座標
        public int RebirthCD { get; set; }            // 怪物重生時間
        public int EXP { get; set; }            // 怪物經驗值
        public int HP { get; set; }            // 怪物血量
        public int MP { get; set; }            // 怪物魔力
        public string AttackMode { get; set; }            // 怪物攻擊模式
        public string Habit { get; set; }            // 怪物是否被動
        public string UseSkill { get; set; }            // 怪物是否使用技能
        public string SkillEffect { get; set; }            // 怪物技能效果
        public string SkilType { get; set; }            // 怪物技能類型
        public string SkillVolume { get; set; }            // 怪物技能數值
        public string SklillCD { get; set; }            // 怪物技能冷卻時間
        public int ATK { get; set; }            // 怪物攻擊力
        public int DEF { get; set; }            // 怪物防禦力
        public int Crt { get; set; }            // 怪物暴擊率
        public int CrtResistance { get; set; }            // 怪物暴擊抵抗
        public int Avoid { get; set; }            // 怪物迴避值
        public int Hit { get; set; }            // 怪物命中值
        public float AtkSpeed { get; set; }            // 怪物攻擊速度
        public string GetKey { get { return CodeID; } }
    }
    /// <summary>
    /// 職業能力值加成
    /// </summary>
    public class JobBonusDataModel : IDictionaryData<string>
    {
        public string Job { get; set; }     //職業
        public string STR { get; set; }     //Str
        public string DEX { get; set; }     //Dex
        public string INT { get; set; }     //Int
        public string AGI { get; set; }     //Agi
        public string WIS { get; set; }     //Wis
        public string VIT { get; set; }     //Vit
        public string HP { get; set; }      //Hp
        public string MP { get; set; }      //Mp
        public string GetKey
        {
            get { return Job; }
        }
    }

    /// <summary>
    /// 種族能力值加成
    /// </summary>
    public class StatusFormulaDataModel : IDictionaryData<string>
    {
        public string TargetStatus { get; set; }        //目標屬性值
        public string Race { get; set; }                //種族
        public float STR { get; set; }                  //STR
        public float DEX { get; set; }                  //DEX
        public float INT { get; set; }                  //INT
        public float AGI { get; set; }                  //AGI
        public float VIT { get; set; }                  //VIT
        public float WIS { get; set; }                  //WIS
        public float LvCodition { get; set; }           //等級加成

        public string GetKey
        {
            get { return TargetStatus + "_" + Race; }
        }
    }

    /// <summary>
    /// 等級與經驗值的數值
    /// </summary>
    public class LvAndExpDataModel : IDictionaryData<string>
    {
        public int Lv { get; set; }                //等級
        public int EXP { get; set; }               //經驗值

        public string GetKey
        {
            get { return Lv.ToString(); }
        }
    }

    /// <summary>
    /// 遊戲設定資料
    /// </summary>
    public class GameSettingDataModel : IDictionaryData<string>
    {
        public string GameSettingID { get; set; }                //遊戲設定項目ID
        public float GameSettingValue { get; set; }               //設定值

        public string GetKey
        {
            get { return GameSettingID; }
        }
    }
    #endregion
}
