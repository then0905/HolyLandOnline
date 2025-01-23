using System;
using System.Collections.Generic;

//==========================================
//  創建者:    家豪
//  創建日期:  2023/05/03
//  創建用途:  Json資料結構
//==========================================
public interface IDictionaryData<T> where T : IConvertible
{
    T GetKey { get; }
}

/// <summary>
/// 物品基本屬性介面
/// </summary>
public interface IItemBasal
{
    public string Name { get; set; }
    public string CodeID { get; set; }            // 道具ID
    public string Path { get; set; }            //圖片路徑
    public string FileName { get; set; }            //圖片檔名
    public bool Stackability { get; set; }            // 可堆疊性
    public int Price { get; set; }            // 道具在商店販賣的價格
    public int Redeem { get; set; }            // 道具賣給商店的價格
}

public class GameDataModel
{

}

/// <summary>
/// 計算資料共用基底
/// </summary>
public class OperationData
{

}

/// <summary>
/// 基本屬性資料結構
/// </summary>
public class BasalAttributesDataModel
{
    public int HP { get; set; }            // HP加成
    public int MP { get; set; }            // MP加成
    public int HpRecovery { get; set; }            // HP自然回復
    public int MpRecovery { get; set; }            // MP自然回復
    public int MeleeATK { get; set; }            // 近距離攻擊
    public int MeleeHit { get; set; }            // 近距離命中
    public int RemoteATK { get; set; }            // 遠距離攻擊
    public int RemoteHit { get; set; }            // 遠距離命中
    public int MageATK { get; set; }            // 魔法攻擊力
    public int MageHit { get; set; }            // 魔法命中
    public int Avoid { get; set; }            // 迴避值
    public int DEF { get; set; }            // 物理防禦值
    public int MDEF { get; set; }            // 魔法防禦值
    public int DamageReduction { get; set; }            // 傷害減緩
    public float Speed { get; set; }            // 移動速度影響
    public float Crt { get; set; }            // 暴擊率
    public int CrtDamage { get; set; }            // 暴擊傷害
    public float CrtResistance { get; set; }            // 暴擊抵抗
    public float BlockRate { get; set; }            // 盾牌格檔率
    public float DisorderResistance { get; set; }            // 異常狀態抗性
    public float ElementDamageReduction { get; set; }            // 屬性傷害抵抗
    public float ElementDamageIncrease { get; set; }            // 屬性傷害增幅
    public int STR { get; set; }            // STR加成
    public int DEX { get; set; }            // DEX加成
    public int INT { get; set; }            // INT加成
    public int AGI { get; set; }            // AGI加成
    public int VIT { get; set; }            // VIT加成
    public int WIS { get; set; }            // WIS加成
}

#region Json資料結構

/// <summary>
/// 怪物掉落物資料
/// </summary>結構
public class MonsterBootyDataModel : IDictionaryData<string>
{
    public string Area { get; set; }            //地區
    public string AreaID { get; set; }            //地區ID
    public string Name { get; set; }            //怪物名稱
    public string MonsterCodeID { get; set; }          //怪物流水號
    public int MinCoin { get; set; }            //金幣最小值
    public int MaxCoin { get; set; }            //金幣最大值
    public List<BootyDataList> BootyList { get; set; }     //怪物所有掉落物資料
    public string GetKey { get { return MonsterCodeID; } }
}

#region 物品

/// <summary>
/// 防具資料結構
/// </summary>
public class ArmorDataModel : BasalAttributesDataModel, IDictionaryData<string>, IItemBasal
{
    public int NeedLv { get; set; }            // 所需等級
    public string Name { get; set; }            // 防具名稱
    public string CodeID { get; set; }            // 防具ID
    public string Path { get; set; }            //圖片路徑
    public string FileName { get; set; }            //圖片檔名
    public string Classification { get; set; }            // 道具分類
    public string ClassificationID { get; set; }            // 道具分類ID
    public string WearPart { get; set; }            // 穿戴部位
    public string WearPartID { get; set; }            // 穿戴部位ID
    public string Type { get; set; }            // 防具類型
    public string TypeID { get; set; }            // 防具類型ID      
    public string Intro { get; set; }            // 防具介紹
    public bool Stackability { get; set; }            // 可堆疊性
    public int Redeem { get; set; }            // 販售價格
    public int Price { get; set; }          //商店購買價格
    public List<ForgeData> ForgeConfigList { get; set; }         //強化效果配置
    public string GetKey
    {
        get { return CodeID; }
    }
}

/// <summary>
/// 武器資料
/// </summary>結構
public class WeaponDataModel : BasalAttributesDataModel, IDictionaryData<string>, IItemBasal
{
    public int LV { get; set; }            // 所需等級
    public string Name { get; set; }            // 武器名稱
    public string CodeID { get; set; }            // 武器ID
    public string Path { get; set; }            //圖片路徑
    public string FileName { get; set; }            //圖片檔名
    public string Classification { get; set; }            // 道具分類
    public string ClassificationID { get; set; }            // 道具分類ID
    public string TackHand { get; set; }            // 拿取部位
    public string TackHandID { get; set; }            // 拿取部位ID
    public string Type { get; set; }            // 武器類型
    public string TypeID { get; set; }            // 武器類型ID
    public string Intro { get; set; }            // 武器介紹
    public bool Stackability { get; set; }            // 可堆疊性
    public string AS { get; set; }            // 武器攻擊速度
    public string ASID { get; set; }            // 武器攻擊速度(編碼用)    
    public int Redeem { get; set; }            // 販售價格
    public int Price { get; set; }          //商店購買價格
    public List<ForgeData> ForgeConfigList { get; set; }         //強化效果配置
    public string GetKey { get { return CodeID; } }
}

/// <summary>
/// 道具資料
/// </summary>
public class ItemDataModel : BasalAttributesDataModel, IDictionaryData<string>, IItemBasal
{
    public int LV { get; set; }            // 所需等級
    public string Name { get; set; }            // 道具名稱
    public string CodeID { get; set; }            // 道具ID
    public string Path { get; set; }            //圖片路徑
    public string FileName { get; set; }            //圖片檔名
    public string TakeHand { get; set; }            // 拿取部位
    public string TakeHandID { get; set; }            // 拿取部位ID
    public string Classification { get; set; }            // 道具分類
    public string ClassificationID { get; set; }            // 道具分類ID
    public string Type { get; set; }            // 道具類型
    public string TypeID { get; set; }            // 道具類型ID
    public string Intro { get; set; }            // 道具介紹
    public bool Stackability { get; set; }            // 可堆疊性
    public List<ItemEffectData> ItemEffectDataList { get; set; }            // 道具使用效果資料清單
    public float CD { get; set; }            // 道具冷卻時間
    public int Price { get; set; }            // 道具在商店販賣的價格
    public int Redeem { get; set; }            // 道具賣給商店的價格
    public string GetKey { get { return CodeID; } }
}

/// <summary>
/// 裝備強化屬性資料
/// </summary>
public class ForgeData : BasalAttributesDataModel
{
    public string CodeID { get; set; }            // 武器ID
    public int ForgeLv { get; set; }            // 強化等級
    public float SuccessProbability { get; set; }            // 一般強化成功率
    public float SuperiorSuccessProbability { get; set; }            // 高純度強化成功率
    public float DestroyedProbability { get; set; }            // 強化失敗時物品損壞率
    public int Redeem { get; set; }            // 強化後道具回收價格(加上原本物品的售價)
    public string GetKey { get { return CodeID; } }
}

/// <summary>
/// 道具使用後的效果資料
/// </summary>
public class ItemEffectData : OperationData
{
    public string CodeID { get; set; }      // 道具ID
    public string ItemComponentID { get; set; }        //道具組件ID
    public float EffectValue { get; set; }        // 效果值 
    public string InfluenceStatus { get; set; }     // 效果影響的屬性 (Buff)   
    public string AddType { get; set; }      // 加成運算的方式 Rate:乘法、Value:加法   
    public List<string> ConditionOR { get; set; }       //條件清單 (OR判斷 滿足其中一項即可)
    public List<string> ConditionAND { get; set; }      //條件清單 (AND判斷 滿足所有條件)
    public float EffectDurationTime { get; set; }       // 效果持續時間
    public int EffectRecive { get; set; }       //效果接收方-3:隊友 -2:所有友軍(包含自己) -1:怪物、玩家 0:自身 1:敵軍
    public int TargetCount { get; set; }        // 目標數量 -4:範圍內所有怪物-3:範圍內所有敵軍、-2:範圍內所有敵方目標、-1:隊友與自身、0:自己
    public object Bonus { get; set; }       //配合道具組件規則的額外設定
}

#endregion

#region 技能

/// <summary>
/// 技能資料結構
/// </summary>
public class SkillData : IDictionaryData<string>
{
    public string Job { get; set; }               // 職業
    public string Name { get; set; }            // 技能名稱
    public string SkillID { get; set; }                     // 技能ID
    public int NeedLv { get; set; }            // 技能所需等級
    public bool Characteristic { get; set; }            // 技能特性
    public int CastMage { get; set; }            // 技能花費魔力
    public float CD { get; set; }            // 技能冷卻時間
    public float ChantTime { get; set; }                    // 詠唱時間
    public int AnimaTrigger { get; set; }            // 技能動畫 混合樹的數值
    public string Type { get; set; }            // 技能類型
    public string EffectTarget { get; set; }            // 特效物件生成參考
    public float Distance { get; set; }            // 技能施放距離
    public float Width { get; set; }            // 技能命中寬度
    public float Height { get; set; }            // 技能命中長度
    public float CircleDistance { get; set; }            // 圓圈型技能命中範圍
    public float Damage { get; set; }            // 技能傷害倍率
    public string AdditionMode { get; set; }            // 技能攻擊模式
    public string Intro { get; set; }            // 技能說明
    public List<SkillOperationData> SkillOperationDataList { get; set; }        //技能實際運作所需資料清單
    public string GetKey { get { return SkillID; } }
}


/// <summary>
/// 技能實際運作所需資料
/// </summary>
public class SkillOperationData : OperationData
{
    public string SkillID { get; set; }      // 技能ID
    public string SkillComponentID { get; set; }        //技能組件ID
    public float EffectValue { get; set; }        // 效果值 
    public string InfluenceStatus { get; set; }     // 效果影響的屬性 (Buff)   
    public string AddType { get; set; }      // 加成運算的方式 Rate:乘法、Value:加法   
    public List<string> ConditionOR { get; set; }       //條件清單 (OR判斷 滿足其中一項即可)
    public List<string> ConditionAND { get; set; }      //條件清單 (AND判斷 滿足所有條件)
    public float EffectDurationTime { get; set; }       // 效果持續時間
    public int EffectRecive { get; set; }       //效果接收方-3:隊友 -2:所有友軍(包含自己) -1:怪物、玩家 0:自身 1:敵軍
    public int TargetCount { get; set; }        // 目標數量 -4:範圍內所有怪物-3:範圍內所有敵軍、-2:範圍內所有敵方目標、-1:隊友與自身、0:自己
    public object Bonus { get; set; }       //配合技能組件規則的額外設定
}

#endregion

#region 怪物

/// <summary>
/// 怪物資料
/// </summary>
public class MonsterDataModel : BasalAttributesDataModel, IDictionaryData<string>
{
    public string MonsterCodeID { get; set; }            // 怪物ID
    public string Name { get; set; }            // 怪物名稱
    public string AreaID { get; set; }            // 所在區域
    public string Area { get; set; }            // 所在區域
    public int Lv { get; set; }            // 怪物等級
    public string Class { get; set; }            // 怪物階級
    public string ClassID { get; set; }            // 怪物階級ID
    public string Type { get; set; }            // 怪物類型
    public string TypeID { get; set; }            // 怪物類型ID
    public int SpawnAmount { get; set; }            // 怪物生成量
    public string SpawnPos { get; set; }            // 怪物生成座標
    public int RebirthCD { get; set; }            // 怪物重生時間
    public int EXP { get; set; }            // 怪物經驗值
    public bool ExpSplit { get; set; }            // 經驗值處理 True:造成傷害者依照比例分割、False: 造成傷害者都一樣

    /// <summary>
    /// 掉落物開放權限
    /// 0:無權限 任何玩家皆可撿取
    /// 1:第一階段權限 傷害第一人
    /// 2:第二階段權限 戰鬥資料裡的所有玩家
    /// </summary>
    public int BootysLockSetting { get; set; }

    public string AttackMode { get; set; }            // 怪物攻擊模式
    public bool Habit { get; set; }            // 怪物是否被動
    public float ActivityScope { get; set; }            // 怪物可活動範圍的半徑
    public float DetectionScope { get; set; }            // 怪物若為主動 所偵測的範圍
    public bool UseSkill { get; set; }            // 怪物是否使用技能
    public int ATK { get; set; }            // 怪物攻擊力
    public int Hit { get; set; }            // 怪物命中值
    public float AtkSpeed { get; set; }            // 怪物攻擊速度
    public float AttackRange { get; set; }            // 怪物攻擊距離
    public float PursueRange { get; set; }            // 怪物最遠可追級距離距離
    public float WalkSpeed { get; set; }            // 怪物走路速度
    public float RunSpeed { get; set; }            // 怪物跑步速度
    public List<MonsterSpwanDataModel> MonsterSpawnPosList { get; set; }     //怪物生成資料
    public List<MonsterSkillDataModel> MonsterSkillList { get; set; }     //怪物技能資料
    public string GetKey { get { return MonsterCodeID; } }
}

/// <summary>
/// 怪物生成資料
/// </summary>
public class MonsterSpwanDataModel
{
    public string MonsterCodeID { get; set; }            // 怪物ID
    public float SpawnPosX { get; set; }            // 怪物生成座標X
    public float SpawnPosY { get; set; }            // 怪物生成座標Y
    public float SpawnPosZ { get; set; }            // 怪物生成座標Z

}

/// <summary>
/// 怪物技能資料
/// </summary>
public class MonsterSkillDataModel
{
    public string MonsterCodeID { get; set; }            // 怪物ID
    public string SkillEffect { get; set; }            // 怪物技能說明文字
    public string SkilTypeID { get; set; }            // 怪物技能ID
    public int Countdown { get; set; }            // 怪物施放技能讀條時間 為0則不需讀條立即施放
    public float SkillVolume { get; set; }            // 技能作用值
    public float SklillCD { get; set; }            // 技能施放冷卻時間
    public int CostMana { get; set; }            // 花費怪物的魔力量
    public string AddType { get; set; }            // 技能本身作用值值倍率方式
    public string EffectTarget { get; set; }            // 特效物件生成參考
    public string AdditionalEffect { get; set; }            // 技能額外附加效果種類
    public int AdditionalEffectValue { get; set; }            // 技能額外附加效果作用值
    public string InfluenceStatus { get; set; }            // 技能影響的屬性
    public string Condition { get; set; }            // 技能本身施放條件
    public int EffectRecive { get; set; }            // 技能效果接受方
    public float Distance { get; set; }            // 技能施放有效距離(圓圈)
    public float Width { get; set; }            // 技能施放面積 矩形寬度
    public float Height { get; set; }            // 技能施放面積 矩形長度
    public float CircleDistance { get; set; }            // 技能施放面積 圓形
}

/// <summary>
/// 掉落物詳情資料
/// </summary>
public class BootyDataList
{
    public string MonsterCodeID { get; set; }            //怪物ID
    public string BootyID { get; set; }            //掉落物ID
    public float DropProbability { get; set; }            //機率 小數

    public int DropCountMax { get; set; }       //掉落數量最大值
    public int DropCountMin { get; set; }       //掉落數量最小值
}

#endregion

#region 遊戲設定相關

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

#region NPC

/// <summary>
/// NPC 資料
/// </summary>
public class NpcDataModel : BasalAttributesDataModel, IDictionaryData<string>
{
    public string NpcID { get; set; }           //NPC ID
    public string AreaID { get; set; }           //NPC所屬地區 ID
    public string NpcName { get; set; }           //NPC 名稱
    public string NpcAvatarPath { get; set; }           //NPC 頭像路徑
    public string NpcAvatarName { get; set; }           //NPC 頭像檔案名稱
    public float NpcPosX { get; set; }           //NPC 座標X
    public float NpcPosY { get; set; }           //NPC 座標Y
    public float NpcPosZ { get; set; }           //NPC 座標Z
    public int ATK { get; set; }            // NPC 攻擊力
    public int Hit { get; set; }            // NPC 命中
    public List<string> NpcChatContent { get; set; }           //NPC 對話內容文字清單
    public List<NpcButtonFunc> NpcButtonFuncList { get; set; }           //NPC 按鈕功能清單
    public List<string> QuestIDList { get; set; } // NPC 任務清單
    public List<ShopInventoryData> ShopInventoryList { get; set; } // NPC 的商店物品資料
    public string GetKey
    {
        get { return NpcID; }
    }
}

/// <summary>
/// NPC 按鈕功能
/// </summary>
public class NpcButtonFunc
{
    public string NpcID { get; set; }           //NPC ID
    public string ButtonActionID { get; set; }           //按鈕內容ID
    public string ButtonName { get; set; }           //按鈕名稱
}

/// <summary>
/// NPC 商店資料
/// </summary>
public class ShopInventoryData
{
    public string NpcID { get; set; }           //NPC ID
    public string ProductID { get; set; }           //道具 ID
    public int LimitQty { get; set; }           //限制販賣數量(0為不限制)
}

#endregion

#region 任務

/// <summary>
/// 任務資料結構
/// </summary>
public class QuestDataModel : IDictionaryData<string>
{
    public string QuestID { get; set; }         //任務ID
    public string QuestName { get; set; }         //任務名稱
    public int NeedLv { get; set; }         //需求等級
    public List<string> QuestChatContent { get; set; }    //任務對話內容清單
    public string StartNpcID { get; set; }      //接取任務NpcID
    public string EndNpcID { get; set; }      //回報任務NpcID
    public string QuestType { get; set; }         //任務類型 主線 支線 每日 戰利品
    public List<QuestConditionData> QuestConditionList { get; set; }      //任務條件清單
    public List<QuestRewardData> QuestRewardList { get; set; }      //任務條件清單
    public List<PrerequisiteData> PrerequisiteList { get; set; }      //前置條件清單
    public List<FinishQuestData> QuestFinishList { get; set; }      //任務完成的對話清單
    public int Exp { get; set; }        //經驗值
    public int Coin { get; set; }      //金幣

    public string GetKey
    {
        get { return QuestID; }
    }
}

/// <summary>
/// 任務條件資料
/// </summary>
public class QuestConditionData
{
    public string QuestID { get; set; }         //任務ID
    public string ConditionType { get; set; }       //任務目標類型
    public string ConditionID { get; set; }      //任務目標ID
    public int ConditionCount { get; set; }     //任務目標數量
}

/// <summary>
/// 任務獎勵資料
/// </summary>
public class QuestRewardData
{
    public string QuestID { get; set; }         //任務ID
    public string RewardID { get; set; }      //獎勵物品ID
    public int RewardQty { get; set; }        //獎勵物品數量
}

/// <summary>
/// 前置任務資料
/// </summary>
public class PrerequisiteData
{
    public string QuestID { get; set; }         //任務ID
    public string PrerequisiteType { get; set; }         //前置類型 (須達成指定任務ID,須獲得成就,需求道具等等)
    public string PrerequisiteQty { get; set; }         //前置任務數量(道具向)
}

/// <summary>
/// 完成任務資料
/// </summary>
public class FinishQuestData
{
    public string QuestID { get; set; }         //任務ID
    public List<string> QuestChatContent { get; set; }         //完成任務對話
}

#endregion

#region 教學

/// <summary>
/// 教學系統資料
/// </summary>
public class TutorialSystemData : IDictionaryData<string>
{
    public string TutorialID { get; set; }          //教學ID
    public string SaveTutorialID { get; set; }      //教學完成儲存使用的ID
    public string NextTutorialID { get; set; }      //接著執行的教學ID
    public List<TutorialBasalSettingData> TutorialBasalSettingList { get; set; }         //教學基礎設定清單
    public List<TutorialIDData> TutorialIDList { get; set; }         //教學ID判斷組合
    public List<TutorialActorData> TutorialActorList { get; set; }         //教學ID演出物件清單

    public string GetKey
    {
        get { return TutorialID; }
    }
}

/// <summary>
/// 教學基礎設定資料
/// </summary>
public class TutorialBasalSettingData
{
    public string TutorialDialog { get; set; }         //對話框內容
    public List<string> TutorialDialogFormat { get; set; }         //對話框需合併的文字內容
    public float DiaologPosX { get; set; }      //對話框生成X座標
    public float DiaologPosY { get; set; }      //對話框生成Y座標
    public bool ApertureEnable { get; set; }      //是否啟用光圈
    public bool ApertureIsSquare { get; set; }      //光圈是否為正方形 True:方形 False:圓形
    public bool ClickLock { get; set; }      //是否鎖定點擊範圍
    public float ClickRadio { get; set; }      //設定點擊範圍(半徑)
    public bool TimeScaleStop { get; set; }      //是否需要暫停時間
}

/// <summary>
/// 教學演出物件生成資料
/// </summary>
public class TutorialActorData
{
    public string TutorialID { get; set; }          //教學ID
    public string Path { get; set; }          //生成路徑
    public string Name { get; set; }          //生成檔名
    public float PosX { get; set; }      //生成座標X
    public float PosY { get; set; }      //生成座標Y
    public string LayoutName { get; set; }      //物件圖層名稱
    public int LayoutOrder { get; set; }      //物件圖層值
    public int TutorialIndex { get; set; }      //此物件是該教學的第幾段內容做使用(使用數字為清單索引)
}

/// <summary>
/// 教學ID檢查資料
/// </summary>
public class TutorialIDData
{
    public string TutorialID { get; set; }          //教學ID
    public List<string> NotIncludedID { get; set; }          //此教學必須不包含的ID清單
    public List<string> IncludedID { get; set; }          //此教學必須包含的ID清單
}

#endregion

/// <summary>
/// 遊戲文字資料
/// </summary>
public class GameText : IDictionaryData<string>
{
    public string TextID { get; set; }          //文字ID
    public string TextContent { get; set; }      //文字內容

    public string GetKey
    {
        get { return TextID; }
    }
}

#endregion