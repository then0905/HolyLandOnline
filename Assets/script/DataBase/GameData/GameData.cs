using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:    家豪
//  創建日期:  2023/05/03
//  創建用途:  遊戲資料
//==========================================

public partial class GameData : MonoBehaviour
{
    #region 資料結構

    //怪物的掉落物字典
    public static Dictionary<string, MonsterBootyDataModel> MonsterBootysDic = new Dictionary<string, MonsterBootyDataModel>();
    //防具字典
    public static Dictionary<string, ArmorDataModel> ArmorsDic = new Dictionary<string, ArmorDataModel>();
    //技能UI版字典
    public static Dictionary<string, SkillData> SkillDataDic = new Dictionary<string, SkillData>();
    //技能資料字典
    //public static Dictionary<string, SkillDataModel> SkillsDataDic = new Dictionary<string, SkillDataModel>();
    //武器字典
    public static Dictionary<string, WeaponDataModel> WeaponsDic = new Dictionary<string, WeaponDataModel>();
    //道具字典
    public static Dictionary<string, ItemDataModel> ItemsDic = new Dictionary<string, ItemDataModel>();
    //怪物資料字典
    public static Dictionary<string, MonsterDataModel> MonstersDataDic = new Dictionary<string, MonsterDataModel>();
    //職業加成字典
    public static Dictionary<string, JobBonusDataModel> JobBonusDic = new Dictionary<string, JobBonusDataModel>();
    //種族能力值字典
    public static Dictionary<string, StatusFormulaDataModel> StatusFormulaDic = new Dictionary<string, StatusFormulaDataModel>();
    //等級與經驗值的數值
    public static Dictionary<string, LvAndExpDataModel> ExpAndLvDic = new Dictionary<string, LvAndExpDataModel>();
    //遊戲設定的數值
    public static Dictionary<string, GameSettingDataModel> GameSettingDic = new Dictionary<string, GameSettingDataModel>();
    //Npc資料字典
    public static Dictionary<string, NpcDataModel> NpcDataDic = new Dictionary<string, NpcDataModel>();
    //任務資料字典
    public static Dictionary<string, QuestDataModel> QuestDataDic = new Dictionary<string, QuestDataModel>();
    //教學資料字典
    public static Dictionary<string, TutorialSystemData> TutorialDataDic = new Dictionary<string, TutorialSystemData>();
    //地區資料字典
    public static Dictionary<string, AreaData> AreaDataDic = new Dictionary<string, AreaData>();
    //遊戲文字字典
    public static Dictionary<string, GameText> GameTextDataDic = new Dictionary<string, GameText>();
    #endregion

    /// <summary>
    /// 初始化GameData資料
    /// </summary>
    public static void Init()
    {
        CommonFunction.InitData(GameTextDataDic, "Json",
            "GameText",
            "StatusText", 
            "CommonText",
            "EffectText",
            "AreaText",
            "ArmorText",
            "ItemText",
            "JobText",
            "MonsterText",
            "NpcText",
            "QuestText",
            "SkillText",
            "TutorialText",
            "WeaponText");
        CommonFunction.InitData(MonsterBootysDic,  "Json", "Bootys");
        CommonFunction.InitData(ArmorsDic, "Json", "Armor");
        CommonFunction.InitData(SkillDataDic, "Json", "SkillData");
        //CommonFunction.InitData(SkillDataDic, "Json", "SkillData");
        CommonFunction.InitData(WeaponsDic, "Json", "Weapon");
        CommonFunction.InitData(ItemsDic, "Json", "Item");
        CommonFunction.InitData(MonstersDataDic, "Json", "Monster");
        CommonFunction.InitData(JobBonusDic, "Json", "JobBonus");
        CommonFunction.InitData(StatusFormulaDic, "Json", "StatusFormula");
        CommonFunction.InitData(ExpAndLvDic, "Json", "LvAndExp");
        CommonFunction.InitData(GameSettingDic, "Json", "GameSetting");
        CommonFunction.InitData(NpcDataDic, "Json", "NpcData");
        CommonFunction.InitData(QuestDataDic, "Json", "Quest");
        CommonFunction.InitData(TutorialDataDic, "Json", "TutorialSystem");
        CommonFunction.InitData(AreaDataDic, "Json", "Area");
    }
}
