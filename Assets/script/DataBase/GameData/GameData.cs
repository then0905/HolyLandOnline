using JsonDataModel;
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
    public static Dictionary<string, SkillUIModel> SkillsUIDic = new Dictionary<string, SkillUIModel>();
    //技能資料字典
    public static Dictionary<string, SkillDataModel> SkillsDataDic = new Dictionary<string, SkillDataModel>();
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
    #endregion

    /// <summary>
    /// 初始化GameData資料
    /// </summary>
    public static void Init()
    {
        CommonFunction.InitData(MonsterBootysDic, "Bootys", "Json");
        CommonFunction.InitData(ArmorsDic, "Armor", "Json");
        CommonFunction.InitData(SkillsUIDic, "SkillIntro", "Json");
        CommonFunction.InitData(SkillsDataDic, "SkillData", "Json");
        CommonFunction.InitData(WeaponsDic, "Weapon", "Json");
        CommonFunction.InitData(ItemsDic, "Item", "Json");
        CommonFunction.InitData(MonstersDataDic, "Monster", "Json");
        CommonFunction.InitData(JobBonusDic, "JobBonus", "Json");
        CommonFunction.InitData(StatusFormulaDic, "StatusFormula", "Json");
        CommonFunction.InitData(ExpAndLvDic, "LvAndExp", "Json");
        CommonFunction.InitData(GameSettingDic, "GameSetting", "Json");
    }
}
