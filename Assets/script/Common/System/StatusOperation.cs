using UnityEngine;
using System;
using System.Linq;
using TMPro;
using System.Collections.Generic;
using System.Reflection;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/16
//  創建用途:  角色能力值計算
//==========================================

/// <summary>
/// 暫存基礎能力值
/// </summary>
public class TempBasalStatus
{
    public int MeleeATK;
    public int RemoteATK;
    public int MageATK;
    public int MaxHP;
    public int MaxMP;
    public int HP_Recovery;
    public int MP_Recovery;
    public int DEF;
    public int Avoid;
    public int MeleeHit;
    public int RemoteHit;
    public int MageHit;
    public int MDEF;
    public int DamageReduction;
    public float ElementDamageIncrease;
    public float ElementDamageReduction;
    public float BlockRate;
    public float Speed;
    public float Crt;
    public float AS;
    public float DisorderResistance;
}

public class StatusOperation : MonoBehaviour
{
    #region 全域靜態變數

    public static StatusOperation Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<StatusOperation>();
            return instance;
        }
    }

    private static StatusOperation instance;

    #endregion

    //暫存基礎能力值
    protected TempBasalStatus tempBasalStatus = new TempBasalStatus();
    //暫存效果影響的能力值
    protected TempBasalStatus tempEffectStatus = new TempBasalStatus();
    //暫存裝備效果影響的能力值
    protected TempBasalStatus tempEquipStatus = new TempBasalStatus();

    //武器資料清單
    private List<EquipmentData> weaponList = new List<EquipmentData>();
    //防具資料清單
    private List<EquipmentData> armorList = new List<EquipmentData>();

    //基礎能力值加成事件
    private static Action refreshStatus;
    //需等待基礎數值計算完後才計算的部分
    private static Action refreshAfterStatus;

    //處理技能效果計算能力值的字典 <能力值名稱,Action<技能能力值,基礎能力值,加成方式,加成值>>
    private readonly Dictionary<string, Action<TempBasalStatus, TempBasalStatus, bool, float>> statusModifyDic =
        new Dictionary<string, Action<TempBasalStatus, TempBasalStatus, bool, float>>
    {
            { "Speed", (effect, basal, isRate, value) => 
                    //設定技能效果屬性的數值 (移動速度的算法比較特別 "倍率"的話以速度基準值來計算 不會用穿上裝備的加總)
                    effect.Speed += (isRate ? (1 * value) : value)
            },
              { "ATK", (effect, basal, isRate, value) =>
        {
            //ATK為加總所有攻擊類型的能力值    
            effect.MeleeATK += (isRate ? (int)(basal.MeleeATK * value) : (int)value);
            effect.RemoteATK += (isRate ? (int)(basal.RemoteATK * value) : (int)value);
            effect.MageATK += (isRate ? (int)(basal.MageATK * value) : (int)value);
        }
    },
    { "MeleeATK", (effect, basal, isRate, value) => effect.MeleeATK += (isRate ? (int)(basal.MeleeATK * value) : (int)value) },
    { "RemoteATK", (effect, basal, isRate, value) => effect.RemoteATK += (isRate ? (int)(basal.RemoteATK * value) : (int)value) },
    { "MageATK", (effect, basal, isRate, value) => effect.MageATK += (isRate ? (int)(basal.MageATK * value) : (int)value) },
    { "MaxHP", (effect, basal, isRate, value) => effect.MaxHP += (isRate ? (int)(basal.MaxHP * value) : (int)value) },
    { "MaxMP", (effect, basal, isRate, value) => effect.MaxMP += (isRate ? (int)(basal.MaxMP * value) : (int)value) },
    { "HP_Recovery", (effect, basal, isRate, value) => effect.HP_Recovery += (isRate ? (int)(basal.HP_Recovery * value) : (int)value) },
    { "MP_Recovery", (effect, basal, isRate, value) => effect.MP_Recovery += (isRate ? (int)(basal.MP_Recovery * value) : (int)value) },
    { "DEF", (effect, basal, isRate, value) => effect.DEF += (isRate ? (int)(basal.DEF * value) : (int)value) },
    { "Avoid", (effect, basal, isRate, value) => effect.Avoid += (isRate ? (int)(basal.Avoid * value) : (int)value) },
    { "MeleeHit", (effect, basal, isRate, value) => effect.MeleeHit += (isRate ? (int)(basal.MeleeHit * value) : (int)value) },
    { "RemoteHit", (effect, basal, isRate, value) => effect.RemoteHit += (isRate ? (int)(basal.RemoteHit * value) : (int)value) },
    { "MageHit", (effect, basal, isRate, value) => effect.MageHit += (isRate ? (int)(basal.MageHit * value) : (int)value) },
    { "MDEF", (effect, basal, isRate, value) => effect.MDEF += (isRate ? (int)(basal.MDEF * value) : (int)value) },
    { "DamageReduction", (effect, basal, isRate, value) => effect.DamageReduction += (isRate ? (int)(basal.DamageReduction * value) : (int)value) },
    { "ElementDamageIncrease", (effect, basal, isRate, value) => effect.ElementDamageIncrease += (isRate ? (basal.ElementDamageIncrease * value) :value) },
    { "ElementDamageReduction", (effect, basal, isRate, value) => effect.ElementDamageReduction += (isRate ? (basal.ElementDamageReduction * value) : value) },
    { "BlockRate", (effect, basal, isRate, value) => effect.BlockRate += (isRate ? (basal.BlockRate * value) : value) },
    { "Crt", (effect, basal, isRate, value) => effect.Crt += (isRate ? (basal.Crt * value) : value) },
    { "AS", (effect, basal, isRate, value) => effect.AS += (isRate ? (basal.AS * value) : value) },
    { "DisorderResistance", (effect, basal, isRate, value) => effect.DisorderResistance += (isRate ? (basal.DisorderResistance * value) : value) }
    };

    /// <summary>
    /// 初始化訂閱事件內容
    /// </summary>
    private void InitSub()
    {
        refreshStatus += MeleeATK;
        refreshStatus += RemoteATK;
        refreshStatus += MageATK;
        refreshStatus += MaxHp;
        refreshStatus += MaxMp;
        refreshStatus += DEF;
        refreshStatus += Avoid;
        refreshStatus += MeleeHit;
        refreshStatus += RemoteHit;
        refreshStatus += MageHit;
        refreshStatus += MDEF;
        refreshStatus += AS;
        refreshStatus += DamageReduction;
        refreshStatus += ElementDamageIncrease;
        refreshStatus += ElementDamageReduction;
        refreshStatus += HP_RecoveryReduction;
        refreshStatus += MP_RecoveryReduction;
        refreshStatus += GetNormalAttackRange;
        refreshStatus += Speed;
        refreshAfterStatus += AttackSpeedTimer;
    }

    private void OnDisable()
    {
        refreshStatus -= MeleeATK;
        refreshStatus -= RemoteATK;
        refreshStatus -= MageATK;
        refreshStatus -= MaxHp;
        refreshStatus -= MaxMp;
        refreshStatus -= DEF;
        refreshStatus -= Avoid;
        refreshStatus -= MeleeHit;
        refreshStatus -= RemoteHit;
        refreshStatus -= MageHit;
        refreshStatus -= MDEF;
        refreshStatus -= AS;
        refreshStatus -= DamageReduction;
        refreshStatus -= ElementDamageIncrease;
        refreshStatus -= ElementDamageReduction;
        refreshStatus -= HP_RecoveryReduction;
        refreshStatus -= MP_RecoveryReduction;
        refreshStatus -= GetNormalAttackRange;
        refreshStatus -= Speed;
        refreshAfterStatus -= AttackSpeedTimer;
    }

    /// <summary>
    /// 職業的基礎加成能力值
    /// </summary>
    private void ClassStatus()
    {
        //武器清單
        weaponList = new List<EquipmentData>();
        foreach (var item in BagManager.Instance.EquipDataList)
        {
            if (item.EquipmentDatas.Weapon != null)
                weaponList.Add(item.EquipmentDatas);
        }

        //防具清單
        armorList = new List<EquipmentData>();
        foreach (var item in BagManager.Instance.EquipDataList)
        {
            if (item.EquipmentDatas.Armor != null)
                armorList.Add(item.EquipmentDatas);
        }

        //獲取武器資料
        int weaponDataSTR = weaponList.Sum(x => x.Weapon.STR + (x.ForceLv > 0 ? x.Weapon.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).STR : 0));
        int weaponDataDEX = weaponList.Sum(x => x.Weapon.DEX + (x.ForceLv > 0 ? x.Weapon.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).DEX : 0));
        int weaponDataINT = weaponList.Sum(x => x.Weapon.INT + (x.ForceLv > 0 ? x.Weapon.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).INT : 0));
        int weaponDataAGI = weaponList.Sum(x => x.Weapon.AGI + (x.ForceLv > 0 ? x.Weapon.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).AGI : 0));
        int weaponDataWIS = weaponList.Sum(x => x.Weapon.WIS + (x.ForceLv > 0 ? x.Weapon.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).WIS : 0));
        int weaponDataVIT = weaponList.Sum(x => x.Weapon.VIT + (x.ForceLv > 0 ? x.Weapon.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).VIT : 0));

        //獲取防具資料
        int armorDataSTR = armorList.Sum(x => x.Armor.STR + (x.ForceLv > 0 ? x.Armor.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).STR : 0));
        int armorDataDEX = armorList.Sum(x => x.Armor.DEX + (x.ForceLv > 0 ? x.Armor.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).DEX : 0));
        int armorDataINT = armorList.Sum(x => x.Armor.INT + (x.ForceLv > 0 ? x.Armor.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).INT : 0));
        int armorDataAGI = armorList.Sum(x => x.Armor.AGI + (x.ForceLv > 0 ? x.Armor.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).AGI : 0));
        int armorDataWIS = armorList.Sum(x => x.Armor.WIS + (x.ForceLv > 0 ? x.Armor.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).WIS : 0));
        int armorDataVIT = armorList.Sum(x => x.Armor.VIT + (x.ForceLv > 0 ? x.Armor.ForgeConfigList.Find(y => y.ForgeLv == x.ForceLv).VIT : 0));

        //獲取職業加成的六維 並加上裝備數據
        var jobBonusData = GameData.JobBonusDic[PlayerDataOverView.Instance.PlayerData_.Job];
        PlayerDataOverView.Instance.PlayerData_.STR = int.Parse(jobBonusData.STR) + weaponDataSTR + armorDataSTR;
        PlayerDataOverView.Instance.PlayerData_.DEX = int.Parse(jobBonusData.DEX) + weaponDataDEX + armorDataDEX;
        PlayerDataOverView.Instance.PlayerData_.INT = int.Parse(jobBonusData.INT) + weaponDataINT + armorDataINT;
        PlayerDataOverView.Instance.PlayerData_.AGI = int.Parse(jobBonusData.AGI) + weaponDataAGI + armorDataAGI;
        PlayerDataOverView.Instance.PlayerData_.WIS = int.Parse(jobBonusData.WIS) + weaponDataWIS + armorDataWIS;
        PlayerDataOverView.Instance.PlayerData_.VIT = int.Parse(jobBonusData.VIT) + weaponDataVIT + armorDataVIT;
        PlayerDataOverView.Instance.PlayerData_.MaxHP = int.Parse(jobBonusData.HP);
        PlayerDataOverView.Instance.PlayerData_.MaxMP = int.Parse(jobBonusData.MP);
    }

    /// <summary>
    /// 更新裝備資料後 發送技能檢查事件 更新被動技能與其他技能的啟用條件判斷
    /// </summary>
    private void SkillContitionEventFromEquipment()
    {
        //武器資料 技能事件發送
        foreach (var item in BagManager.Instance.EquipDataList)
        {
            if (item.EquipmentDatas.Weapon != null)
            {
                if (item.GetComponent<EquipData>().PartID.Any(x => x == "LeftHand"))
                    SkillController.Instance.SkillConditionCheckEvent?.Invoke("EquipLeft");
                SkillController.Instance.SkillConditionCheckEvent?.Invoke("EquipWeapon");
            }
            else
            {
                if (item.GetComponent<EquipData>().PartID.Any(x => x == "LeftHand"))
                    SkillController.Instance.SkillConditionCheckEvent?.Invoke("EquipLeft");

                SkillController.Instance.SkillConditionCheckEvent?.Invoke("EquipWeapon");
            }
        }

        //防具資料 技能事件發送
        SkillController.Instance.SkillConditionCheckEvent?.Invoke("EquipArmor");
    }

    /// <summary>
    ///  刷新能力值 (升等、初始化、穿脫裝備等等)
    /// <para> 技能效果處理 :</para>
    /// <para> False => 技能效果正常啟動順序 基礎加成>穿裝>技能效果 </para>
    /// <para> True => 技能效果優先啟動 以脫裝前的數據來關閉技能效果 </para>
    /// </summary>
    /// <param name="skillEffectProcessor">技能效果處理</param>
    public void StatusMethod(bool skillEffectProcessor = false)
    {
        if (refreshStatus == null)
            InitSub();          //重新訂閱

        if (!skillEffectProcessor)
        {
            ClassStatus();      //基礎加成
            refreshStatus.Invoke(); //刷新加成後的數值
            PlayerDataStatusOperation();    //將刷新後加成的值算入角色屬性
            refreshAfterStatus.Invoke();//需等待基礎數值計算完後才計算的部分
            //PassiveSkillManager.Instance.RestartPassiveSkill();      //重新啟動被動技能  
            SkillContitionEventFromEquipment();     //發送技能條件檢查事件
        }
        else
        {
            SkillContitionEventFromEquipment();   //發送技能條件檢查事件
            // PassiveSkillManager.Instance.RestartPassiveSkill();      //重新啟動被動技能  
            ClassStatus();      //基礎加成
            refreshStatus.Invoke(); //刷新加成後的數值
            PlayerDataStatusOperation();    //將刷新後加成的值算入角色屬性
            refreshAfterStatus.Invoke();//需等待基礎數值計算完後才計算的部分
        }
    }

    #region 能力值加成

    /// <summary>
    /// 近距離攻擊加成
    /// </summary>
    private void MeleeATK()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"MeleeATK_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取裝備能力值數據
        tempEquipStatus.MeleeATK = weaponList.Sum(x => x.Weapon.MeleeATK + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.MeleeATK).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.MeleeATK = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.STR * targetStatus.STR +
              PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition);
    }

    /// <summary>
    /// 遠距離攻擊加成
    /// </summary>
    private void RemoteATK()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"RemoteATK_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取裝備能力值數據
        tempEquipStatus.RemoteATK = weaponList.Sum(x => x.Weapon.RemoteATK + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.RemoteATK).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.RemoteATK = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.DEX * targetStatus.DEX +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition);
    }

    /// <summary>
    /// 魔法攻擊加成
    /// </summary>
    private void MageATK()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"MageATK_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取裝備能力值數據
        tempEquipStatus.MageATK = weaponList.Sum(x => x.Weapon.MageATK + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.MageATK).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.MageATK = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.INT * targetStatus.INT +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition);

    }

    /// <summary>
    /// 最大生命加成
    /// </summary>
    private void MaxHp()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"HP_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取武器、防具能力值數據
        tempEquipStatus.MaxHP = weaponList.Sum(x => x.Weapon.HP + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.HP).FirstOrDefault()) +
       armorList.Sum(x => x.Armor.HP + x.Armor.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.HP).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.MaxHP = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.VIT * targetStatus.VIT +
                PlayerDataOverView.Instance.PlayerData_.STR * targetStatus.STR +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + PlayerDataOverView.Instance.PlayerData_.MaxHP;
    }

    /// <summary>
    /// 最大魔力加成
    /// </summary>
    private void MaxMp()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"MP_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取武器、防具能力值數據
        tempEquipStatus.MaxMP = weaponList.Sum(x => x.Weapon.MP + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.MP).FirstOrDefault()) +
            armorList.Sum(x => x.Armor.MP + x.Armor.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.MP).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.MaxMP = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.INT * targetStatus.INT +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + PlayerDataOverView.Instance.PlayerData_.MaxMP;
    }

    /// <summary>
    /// 物理防禦加成
    /// </summary>
    private void DEF()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"DEF_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取武器、防具能力值數據
        tempEquipStatus.DEF = weaponList.Sum(x => x.Weapon.DEF + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.DEF).FirstOrDefault()) +
            armorList.Sum(x => x.Armor.DEF + x.Armor.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.DEF).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.DEF = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.VIT * targetStatus.VIT +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition);

    }

    /// <summary>
    /// 迴避值加成
    /// </summary>
    private void Avoid()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"Avoid_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取武器、防具能力值數據
        tempEquipStatus.Avoid = weaponList.Sum(x => x.Weapon.Avoid + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.Avoid).FirstOrDefault()) +
            armorList.Sum(x => x.Armor.Avoid + x.Armor.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.Avoid).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.Avoid = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.DEX * targetStatus.DEX +
                PlayerDataOverView.Instance.PlayerData_.AGI * targetStatus.AGI);
    }

    /// <summary>
    /// 進距離命中加成
    /// </summary>
    private void MeleeHit()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"MeleeHit_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取武器能力值數據
        tempEquipStatus.MeleeHit = weaponList.Sum(x => x.Weapon.MeleeHit + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.MeleeHit).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.MeleeHit = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.STR * targetStatus.STR +
                PlayerDataOverView.Instance.PlayerData_.AGI * targetStatus.AGI +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition);
    }

    /// <summary>
    /// 遠距離命中加成
    /// </summary>
    private void RemoteHit()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"RemoteHit_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取武器能力值數據
        tempEquipStatus.RemoteHit = weaponList.Sum(x => x.Weapon.RemoteHit + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.RemoteHit).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.RemoteHit = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.DEX * targetStatus.DEX +
                PlayerDataOverView.Instance.PlayerData_.AGI * targetStatus.AGI +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition);
    }

    /// <summary>
    /// 魔法命中加成
    /// </summary>
    private void MageHit()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"MageHit_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取武器能力值數據
        tempEquipStatus.MageHit = weaponList.Sum(x => x.Weapon.MageHit + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.MageHit).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.MageHit = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.INT * targetStatus.INT +
                PlayerDataOverView.Instance.PlayerData_.AGI * targetStatus.AGI +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition);
    }

    /// <summary>
    /// 魔法防禦值加成
    /// </summary>
    private void MDEF()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"MDEF_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取武器、防具能力值數據
        tempEquipStatus.MDEF = weaponList.Sum(x => x.Weapon.MDEF + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.MDEF).FirstOrDefault()) +
            armorList.Sum(x => x.Armor.MDEF + x.Armor.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.MDEF).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.MDEF = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.VIT * targetStatus.VIT +
                PlayerDataOverView.Instance.PlayerData_.WIS * targetStatus.WIS);
    }

    /// <summary>
    /// 移動速度
    /// </summary>
    private void Speed()
    {
        //獲取防具能力值數據
        tempEquipStatus.Speed = armorList.Sum(x => x.Armor.Speed + x.Armor.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.Speed).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.Speed = 1;
    }

    /// <summary>
    /// 攻擊速度值加成
    /// </summary>
    private void AS()
    {
        //獲取武器能力值數據
        tempEquipStatus.AS = weaponList.Select(x => GameData.GameSettingDic[x.Weapon.ASID].GameSettingValue).FirstOrDefault();

        //獲取基礎加成能力值數據
        tempBasalStatus.AS = 1;
    }

    /// <summary>
    /// 傷害減緩加成
    /// </summary>
    private void DamageReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"DamageReduction_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取防具能力值數據
        tempEquipStatus.DamageReduction = armorList.Sum(x => x.Armor.DamageReduction + x.Armor.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.DamageReduction).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.DamageReduction = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.VIT * targetStatus.VIT);
    }

    /// <summary>
    /// 屬性傷害增幅加成
    /// </summary>
    private void ElementDamageIncrease()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"ElementDamageIncrease_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取武器能力值數據
        tempEquipStatus.ElementDamageIncrease = (int)(weaponList.Sum(x => x.Weapon.ElementDamageIncrease + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.ElementDamageIncrease).FirstOrDefault()));

        //獲取基礎加成能力值數據
        tempBasalStatus.ElementDamageIncrease = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.INT * targetStatus.INT);
    }

    /// <summary>
    /// 屬性傷害抵抗加成
    /// </summary>
    private void ElementDamageReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"ElementDamageReduction_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取防具能力值數據
        tempEquipStatus.ElementDamageReduction = armorList.Sum(x => x.Armor.ElementDamageReduction + x.Armor.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.ElementDamageReduction).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.ElementDamageReduction = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.WIS * targetStatus.WIS);
    }

    /// <summary>
    /// 生命恢復加成
    /// </summary>
    private void HP_RecoveryReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"HP_Recovery_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取防具能力值數據
        tempEquipStatus.HP_Recovery = armorList.Sum(x => x.Armor.HpRecovery + x.Armor.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.HpRecovery).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.HP_Recovery =
            (int)Mathf.Round(targetStatus.VIT * PlayerDataOverView.Instance.PlayerData_.VIT + PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition +
            GameData.GameSettingDic[PlayerDataOverView.Instance.PlayerData_.Race + "BasalHpRecovery"].GameSettingValue);
    }

    /// <summary>
    /// 魔力恢復加成
    /// </summary>
    private void MP_RecoveryReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus = GameData.StatusFormulaDic[$"MP_Recovery_{PlayerDataOverView.Instance.PlayerData_.Race}"];

        //獲取防具能力值數據
        tempEquipStatus.MP_Recovery = armorList.Sum(x => x.Armor.MpRecovery + x.Armor.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.MpRecovery).FirstOrDefault());

        //獲取基礎加成能力值數據
        tempBasalStatus.MP_Recovery =
           (int)Mathf.Round(targetStatus.WIS * PlayerDataOverView.Instance.PlayerData_.WIS + PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition +
           GameData.GameSettingDic[PlayerDataOverView.Instance.PlayerData_.Race + "BasalMpRecovery"].GameSettingValue);
    }

    /// <summary>
    /// 取得裝備武器的普通攻擊範圍
    /// </summary>
    public void GetNormalAttackRange()
    {
        /*腳本下中斷點 經過  weaponType = "MeleeAttackRange"; 後 後面都不會執行 帶查驗修正*/

        string weaponType = "";
        //若沒有武器則為空手 視同近戰
        if (weaponList.Count == 0 || (!weaponList.Any(x => x.Weapon.TypeID.Contains("Bow"))))
        {
            weaponType = "MeleeAttackRange";
        }
        else
            //若有武器 檢查是否有物理攻擊資料 判斷是近距離型武器或是遠距離
            weaponType = (weaponList.Any(x => x.Weapon.MeleeATK + x.Weapon.ForgeConfigList.Where(y => y.ForgeLv.Equals(x.ForceLv)).Select(y => y.MeleeATK).FirstOrDefault() != 0) ? "MeleeAttackRange" : "RemoteAttackRange");

        //取得攻擊範圍值
        PlayerDataOverView.Instance.PlayerData_.NormalAttackRange = (int)GameData.GameSettingDic[weaponType].GameSettingValue;
    }

    #endregion

    #region 能力值加成完成後需要計算的機制

    /// <summary>
    /// 計算攻擊速度
    /// </summary>
    public void AttackSpeedTimer()
    {
        //每當攻擊速度重新設定 計算攻擊速度區間
        NormalAttackSystem.AttackSpeedTimer = 1 / PlayerDataOverView.Instance.PlayerData_.AS;
    }

    #endregion

    /// <summary>
    /// 技能效果運算
    /// <para>在計算完玩家穿戴裝備 種族 職業基本加成的詳細能力值(稱 基礎能力值)</para>
    /// <para>再將玩家當前所有的被動 buff debuff 以 基礎能力值 將技能影響數值做計算</para>
    /// </summary>
    public void SkillEffectStatusOperation(string statusType, bool isRate, float value)
    {
        if (statusModifyDic.TryGetValue(statusType, out var modifier))
        {
            modifier(tempEffectStatus, tempBasalStatus, isRate, value);
        }
        else
        {
            Debug.Log($"未宣告參數或是輸入了不知名參數: {statusType}");
        }
        //將效果影響的能力值進行運算
        PlayerDataStatusOperation();
        #region 映射版本 但太耗效能 故棄用
        ////取得 能力值裡對應的欄位參數(技能效果)
        //FieldInfo effectProperty = typeof(TempBasalStatus).GetField(statusType);
        ////取得 能力值裡對應的欄位參數(當前基礎屬性)
        //FieldInfo basalProperty = typeof(TempBasalStatus).GetField(statusType);

        ////檢查空值
        //if (effectProperty != null && basalProperty != null)
        //{

        //    //若效果為 移動速度 屬性
        //    if (statusType == "Speed")
        //    {
        //        //取得 技能效果 能力值裡對應的參數的數值
        //        float effectValue = (float)effectProperty.GetValue(tempBasalStatus);
        //        //設定技能效果屬性的數值 (移動速度的算法比較特別 "倍率"的話以速度基準值來計算 不會用穿上裝備的加總)
        //        effectProperty.SetValue(tempBasalStatus, effectValue + (isRate ? (1 * value) : value));
        //    }
        //    //其餘屬性正常運算
        //    else
        //    {
        //        //取得 技能效果 能力值裡對應的參數的數值
        //        if (effectProperty.GetValue(tempEffectStatus) is int)
        //        {
        //            int effectValue = (int)effectProperty.GetValue(tempEffectStatus);
        //            //取得 當前基礎屬性 能力值裡對應的參數的數值
        //            int basalValue = (int)basalProperty.GetValue(tempBasalStatus);
        //            //取得 當前裝備屬性 能力值裡對應的參數的數值
        //            int equipmentValue = (int)basalProperty.GetValue(tempEquipStatus);

        //            //依照加成或倍率計算數值
        //            effectValue += (isRate ? (int)((basalValue + equipmentValue) * value) : (int)value);

        //            //設定技能效果屬性的數值
        //            effectProperty.SetValue(tempEffectStatus, effectValue);
        //        }
        //        else if (effectProperty.GetValue(tempEffectStatus) is float)
        //        {
        //            float effectValue = (float)effectProperty.GetValue(tempEffectStatus);
        //            //取得 當前基礎屬性 能力值裡對應的參數的數值
        //            float basalValue = (float)basalProperty.GetValue(tempBasalStatus);
        //            //取得 當前裝備屬性 能力值裡對應的參數的數值
        //            float equipmentValue = (float)basalProperty.GetValue(tempEquipStatus);

        //            //依照加成或倍率計算數值
        //            effectValue += (isRate ? ((basalValue + equipmentValue) * value) : value);

        //            //設定技能效果屬性的數值
        //            effectProperty.SetValue(tempEffectStatus, effectValue);
        //        }


        //    }
        //}
        ////若效果為 全部攻擊 屬性
        //else if (statusType == "ATK")
        //{
        //    //取得所有攻擊屬性
        //    string[] atkTypes = { "MeleeATK", "RemoteATK", "MageATK" };
        //    //依次增加所有攻擊屬性
        //    foreach (string atkType in atkTypes)
        //    {
        //        //取得能力值對應的攻擊欄位參數(技能效果)
        //        FieldInfo effectATKProperty = typeof(TempBasalStatus).GetField(atkType);
        //        //取得能力值對應的攻擊欄位參數(當前基礎屬性)
        //        FieldInfo basalATKProperty = typeof(TempBasalStatus).GetField(atkType);

        //        //檢查空值
        //        if (effectProperty != null && basalProperty != null)
        //        {
        //            //取得 技能效果 能力值裡對應的攻擊參數的數值
        //            int effectValue = (int)effectATKProperty.GetValue(tempEffectStatus);
        //            //取得 當前基礎屬性 能力值裡對應的攻擊參數的數值
        //            int basalValue = (int)basalATKProperty.GetValue(tempBasalStatus);
        //            //取得 當前裝備屬性 能力值裡對應的參數的數值
        //            int equipmentValue = (int)basalProperty.GetValue(tempEquipStatus);

        //            //依照加成或倍率計算數值
        //            effectValue += ((isRate ? (int)(basalValue * value) : (int)value) + (isRate ? (int)(equipmentValue * value) : (int)value));

        //            //設定技能效果屬性的數值
        //            effectProperty.SetValue(tempEffectStatus, effectValue);
        //        }
        //    }
        //}
        //else
        //{
        //    Debug.Log($"未宣告參數或是輸入了不知名參數: {statusType}");
        //}
        #endregion
        #region 舊版運算 使用SwitchCase 已棄用 改用映射
        //switch (statusType)
        //{
        //    default:
        //    case "MeleeATK":
        //        tempEffectStatus.MeleeATK += (isRate ? (int)(tempBasalStatus.MeleeATK * value) : (int)value);
        //        break;
        //    case "MaxHP":
        //        tempEffectStatus.MaxHp += (isRate ? (int)(tempBasalStatus.MaxHp * value) : (int)value);
        //        break;
        //    case "DEF":
        //        tempEffectStatus.DEF += (isRate ? (int)(tempBasalStatus.DEF * value) : (int)value);
        //        break;
        //    case "MeleeHit":
        //        tempEffectStatus.MeleeHit += (isRate ? (int)(tempBasalStatus.MeleeHit * value) : (int)value);
        //        break;
        //    case "HP_Recovery":
        //        tempEffectStatus.HP_Recovery += (isRate ? (int)(tempBasalStatus.HP_Recovery * value) : (int)value);
        //        break;
        //    case "MP_Recovery":
        //        tempEffectStatus.MP_Recovery += (isRate ? (int)(tempBasalStatus.MP_Recovery * value) : (int)value);
        //        break;
        //    case "ATK":
        //        tempEffectStatus.MeleeATK += (isRate ? (int)(tempBasalStatus.MeleeATK * value) : (int)value);
        //        tempEffectStatus.RemoteATK += (isRate ? (int)(tempBasalStatus.RemoteATK * value) : (int)value);
        //        tempEffectStatus.MageATK += (isRate ? (int)(tempBasalStatus.MageATK * value) : (int)value);
        //        break;
        //    case "BlockRate":
        //        tempEffectStatus.BlockRate += (isRate ? (int)(tempBasalStatus.BlockRate * value) : (int)value);
        //        break;
        //    case "Speed":
        //        //倍率的話以速度基準值來計算 不會用穿上裝備的加總
        //        tempEffectStatus.Speed += (isRate ? (int)(1 * value) : (int)value);
        //        break;
        //    case "Crt":
        //        tempEffectStatus.Crt += (isRate ? (int)(tempBasalStatus.Crt * value) : (int)value);
        //        break;
        //    case "AS":
        //        tempEffectStatus.AS += (isRate ? (int)(tempBasalStatus.AS * value) : (int)value);
        //        break;
        //    case "DisorderResistance":
        //        tempEffectStatus.DisorderResistance += (isRate ? (int)(tempBasalStatus.DisorderResistance * value) : (int)value);
        //        break;
        //}
        #endregion
    }

    /// <summary>
    /// 將效果影響的能力值加成到PlayerDataOverView.Instance.PlayerData_的屬性
    /// </summary>
    public void PlayerDataStatusOperation()
    {
        //if (PlayerDataOverView.Instance && tempEffectStatus != null)
        //{
        PlayerDataOverView.Instance.PlayerData_.MeleeATK = (tempEffectStatus.MeleeATK + tempBasalStatus.MeleeATK + tempEquipStatus.MeleeATK);
        PlayerDataOverView.Instance.PlayerData_.RemoteATK = (tempEffectStatus.RemoteATK + tempBasalStatus.RemoteATK + tempEquipStatus.RemoteATK);
        PlayerDataOverView.Instance.PlayerData_.MageATK = (tempEffectStatus.MageATK + tempBasalStatus.MageATK + tempEquipStatus.MageATK);
        PlayerDataOverView.Instance.PlayerData_.MaxHP = (tempEffectStatus.MaxHP + tempBasalStatus.MaxHP + tempEquipStatus.MaxHP);
        PlayerDataOverView.Instance.PlayerData_.MaxMP = (tempEffectStatus.MaxMP + tempBasalStatus.MaxMP + tempEquipStatus.MaxMP);
        PlayerDataOverView.Instance.PlayerData_.HP_Recovery = (tempEffectStatus.HP_Recovery + tempBasalStatus.HP_Recovery + tempEquipStatus.HP_Recovery);
        PlayerDataOverView.Instance.PlayerData_.MP_Recovery = (tempEffectStatus.MP_Recovery + tempBasalStatus.MP_Recovery + tempEquipStatus.MP_Recovery);
        PlayerDataOverView.Instance.PlayerData_.DEF = (tempEffectStatus.DEF + tempBasalStatus.DEF + tempEquipStatus.DEF);
        PlayerDataOverView.Instance.PlayerData_.Avoid = (tempEffectStatus.Avoid + tempBasalStatus.Avoid + tempEquipStatus.Avoid);
        PlayerDataOverView.Instance.PlayerData_.MeleeHit = (tempEffectStatus.MeleeHit + tempBasalStatus.MeleeHit + tempEquipStatus.MeleeHit);
        PlayerDataOverView.Instance.PlayerData_.RemoteHit = (tempEffectStatus.RemoteHit + tempBasalStatus.RemoteHit + tempEquipStatus.RemoteHit);
        PlayerDataOverView.Instance.PlayerData_.MageHit = (tempEffectStatus.MageHit + tempBasalStatus.MageHit + tempEquipStatus.MageHit);
        PlayerDataOverView.Instance.PlayerData_.MDEF = (tempEffectStatus.MDEF + tempBasalStatus.MDEF + tempEquipStatus.MDEF);
        PlayerDataOverView.Instance.PlayerData_.DamageReduction = (tempEffectStatus.DamageReduction + tempBasalStatus.DamageReduction + tempEquipStatus.DamageReduction);
        PlayerDataOverView.Instance.PlayerData_.ElementDamageIncrease = (tempEffectStatus.ElementDamageIncrease + tempBasalStatus.ElementDamageIncrease + tempEquipStatus.ElementDamageIncrease);
        PlayerDataOverView.Instance.PlayerData_.ElementDamageReduction = (tempEffectStatus.ElementDamageReduction + tempBasalStatus.ElementDamageReduction + tempEquipStatus.ElementDamageReduction);
        PlayerDataOverView.Instance.PlayerData_.BlockRate = (tempEffectStatus.BlockRate + tempBasalStatus.BlockRate + tempEquipStatus.BlockRate);
        PlayerDataOverView.Instance.PlayerData_.Speed = (tempEffectStatus.Speed + tempBasalStatus.Speed + tempEquipStatus.Speed);
        PlayerDataOverView.Instance.PlayerData_.Crt = (tempEffectStatus.Crt + tempBasalStatus.Crt + tempEquipStatus.Crt);
        PlayerDataOverView.Instance.PlayerData_.AS = (tempEffectStatus.AS + tempBasalStatus.AS + tempEquipStatus.AS);
        PlayerDataOverView.Instance.PlayerData_.DisorderResistance = (tempEffectStatus.DisorderResistance + tempBasalStatus.DisorderResistance + tempEquipStatus.DisorderResistance);
        //}
        //else if (PlayerDataOverView.Instance)
        //{
        //    PlayerDataOverView.Instance.PlayerData_.MeleeATK = tempBasalStatus.MeleeATK;
        //    PlayerDataOverView.Instance.PlayerData_.RemoteATK = tempBasalStatus.RemoteATK;
        //    PlayerDataOverView.Instance.PlayerData_.MageATK = tempBasalStatus.MageATK;
        //    PlayerDataOverView.Instance.PlayerData_.MaxHP = tempBasalStatus.MaxHP;
        //    PlayerDataOverView.Instance.PlayerData_.MaxMP = tempBasalStatus.MaxMP;
        //    PlayerDataOverView.Instance.PlayerData_.HP_Recovery = tempBasalStatus.HP_Recovery;
        //    PlayerDataOverView.Instance.PlayerData_.MP_Recovery = tempBasalStatus.MP_Recovery;
        //    PlayerDataOverView.Instance.PlayerData_.DEF = tempBasalStatus.DEF;
        //    PlayerDataOverView.Instance.PlayerData_.Avoid = tempBasalStatus.Avoid;
        //    PlayerDataOverView.Instance.PlayerData_.MeleeHit = tempBasalStatus.MeleeHit;
        //    PlayerDataOverView.Instance.PlayerData_.RemoteHit = tempBasalStatus.RemoteHit;
        //    PlayerDataOverView.Instance.PlayerData_.MageHit = tempBasalStatus.MageHit;
        //    PlayerDataOverView.Instance.PlayerData_.MDEF = tempBasalStatus.MDEF;
        //    PlayerDataOverView.Instance.PlayerData_.DamageReduction = tempBasalStatus.DamageReduction;
        //    PlayerDataOverView.Instance.PlayerData_.ElementDamageIncrease = tempBasalStatus.ElementDamageIncrease;
        //    PlayerDataOverView.Instance.PlayerData_.ElementDamageReduction = tempBasalStatus.ElementDamageReduction;
        //    PlayerDataOverView.Instance.PlayerData_.BlockRate = tempBasalStatus.BlockRate;
        //    PlayerDataOverView.Instance.PlayerData_.Speed = tempBasalStatus.Speed;
        //    PlayerDataOverView.Instance.PlayerData_.Crt = tempBasalStatus.Crt;
        //    PlayerDataOverView.Instance.PlayerData_.AS = tempBasalStatus.AS;
        //    PlayerDataOverView.Instance.PlayerData_.DisorderResistance = tempBasalStatus.DisorderResistance;
        //}
        //刷新數據呈現
        //PlayerDataPanelProcessor.Instance.SetPlayerDataContent();
        PlayerDataOverView.Instance.UIrefresh?.Invoke();
    }

    /// <summary>
    /// 經驗值增加測試
    /// </summary>
    public void AddExpTest()
    {
        PlayerDataOverView.Instance.PlayerData_.Exp += 10;
        PlayerDataOverView.Instance.ExpProcessor();
    }
}
