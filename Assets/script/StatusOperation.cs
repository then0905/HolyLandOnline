using UnityEngine;
using System;
using System.Linq;
using TMPro;
using System.Collections.Generic;

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
    public int MaxHp;
    public int MaxMp;
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

    private static Action refreshStatus;

    //暫存基礎能力值
    protected TempBasalStatus tempBasalStatus = new TempBasalStatus();
    //暫存效果影響的能力值
    protected TempBasalStatus tempEffectStatus = new TempBasalStatus();

    //武器資料清單
    private List<JsonDataModel.WeaponDataModel> weaponList = new List<JsonDataModel.WeaponDataModel>();
    //防具資料清單
    private List<JsonDataModel.ArmorDataModel> armorList = new List<JsonDataModel.ArmorDataModel>();
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
        refreshStatus += DamageReduction;
        refreshStatus += ElementDamageIncrease;
        refreshStatus += ElementDamageReduction;
        refreshStatus += HP_RecoveryReduction;
        refreshStatus += MP_RecoveryReduction;
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
        refreshStatus -= DamageReduction;
        refreshStatus -= ElementDamageIncrease;
        refreshStatus -= ElementDamageReduction;
        refreshStatus -= HP_RecoveryReduction;
        refreshStatus -= MP_RecoveryReduction;
    }

    /// <summary>
    /// 職業的基礎加成能力值
    /// </summary>
    private void ClassStatus()
    {
        //武器清單
        weaponList = new List<JsonDataModel.WeaponDataModel>();
        foreach (var item in ItemManager.Instance.EquipDataList)
        {
            if (item.EquipmentDatas.Weapon != null)
                weaponList.Add(item.EquipmentDatas.Weapon);
        }
        //防具清單
        armorList = new List<JsonDataModel.ArmorDataModel>();
        foreach (var item in ItemManager.Instance.EquipDataList)
        {
            if (item.EquipmentDatas.Armor != null)
                armorList.Add(item.EquipmentDatas.Armor);
        }

        //獲取武器資料
        int weaponDataSTR = weaponList.Sum(x => x.STR);
        int weaponDataDEX = weaponList.Sum(x => x.DEF);
        int weaponDataINT = weaponList.Sum(x => x.INT);
        int weaponDataAGI = weaponList.Sum(x => x.AGI);
        int weaponDataWIS = weaponList.Sum(x => x.WIS);
        int weaponDataVIT = weaponList.Sum(x => x.VIT);

        //獲取防具資料
        int armorDataSTR = armorList.Sum(x => x.STR);
        int armorDataDEX = armorList.Sum(x => x.DEF);
        int armorDataINT = armorList.Sum(x => x.INT);
        int armorDataAGI = armorList.Sum(x => x.AGI);
        int armorDataWIS = armorList.Sum(x => x.WIS);
        int armorDataVIT = armorList.Sum(x => x.VIT);

        //獲取職業加成的六維 並加上裝備數據
        PlayerData.STR = int.Parse(GameData.JobBonusDic[PlayerData.Job].STR) + weaponDataSTR + armorDataSTR;
        PlayerData.DEX = int.Parse(GameData.JobBonusDic[PlayerData.Job].DEX) + weaponDataDEX + armorDataDEX;
        PlayerData.INT = int.Parse(GameData.JobBonusDic[PlayerData.Job].INT) + weaponDataINT + armorDataINT;
        PlayerData.AGI = int.Parse(GameData.JobBonusDic[PlayerData.Job].AGI) + weaponDataAGI + armorDataAGI;
        PlayerData.WIS = int.Parse(GameData.JobBonusDic[PlayerData.Job].WIS) + weaponDataWIS + armorDataWIS;
        PlayerData.VIT = int.Parse(GameData.JobBonusDic[PlayerData.Job].VIT) + weaponDataVIT + armorDataVIT;
        PlayerData.MaxHP = int.Parse(GameData.JobBonusDic[PlayerData.Job].HP);
        PlayerData.MaxMP = int.Parse(GameData.JobBonusDic[PlayerData.Job].MP);
    }

    /// <summary>
    /// 刷新能力值 (升等、初始化、穿脫裝備等等)
    /// </summary>
    public void StatusMethod()
    {
        InitSub();          //重新訂閱
        ClassStatus();      //基礎加成
        refreshStatus.Invoke(); //刷新加成後的數值
        PlayerDataStatusOperation();    //將刷新後加成的值算入角色屬性
        PassiveSkillManager.Instance.RestartPassiveSkill();      //重新啟動被動技能 
    }

    #region 能力值加成

    /// <summary>
    /// 近距離攻擊加成
    /// </summary>
    private void MeleeATK()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MeleeATK_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();
        //獲取裝備能力值數據
        int weaponData = weaponList.Sum(x => x.MeleeATK);

        tempBasalStatus.MeleeATK = (int)Mathf.Round(PlayerData.STR * targetStatus.STR +
              PlayerData.Lv * targetStatus.LvCodition) + weaponData;
    }
    /// <summary>
    /// 遠距離攻擊加成
    /// </summary>
    private void RemoteATK()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("RemoteATK_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取裝備能力值數據
        int weaponData = weaponList.Sum(x => x.RemoteATK);
        tempBasalStatus.RemoteATK = (int)Mathf.Round(PlayerData.DEX * targetStatus.DEX +
                PlayerData.Lv * targetStatus.LvCodition) + weaponData;
    }
    /// <summary>
    /// 魔法攻擊加成
    /// </summary>
    private void MageATK()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MageATK_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取裝備能力值數據
        int weaponData = weaponList.Sum(x => x.MageATK);

        tempBasalStatus.MageATK = (int)Mathf.Round(PlayerData.INT * targetStatus.INT +
                PlayerData.Lv * targetStatus.LvCodition) + weaponData;

    }
    /// <summary>
    /// 最大生命加成
    /// </summary>
    private void MaxHp()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("HP_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.HP);
        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.HP);

        tempBasalStatus.MaxHp = (int)Mathf.Round(PlayerData.VIT * targetStatus.VIT +
                PlayerData.STR * targetStatus.STR +
                PlayerData.Lv * targetStatus.LvCodition) + weaponData + armorData;
    }
    /// <summary>
    /// 最大魔力加成
    /// </summary>
    private void MaxMp()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MP_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.MP);
        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.MP);

        tempBasalStatus.MaxMp = (int)Mathf.Round(PlayerData.INT * targetStatus.INT +
                PlayerData.Lv * targetStatus.LvCodition) + weaponData + armorData;
    }
    /// <summary>
    /// 物理防禦加成
    /// </summary>
    private void DEF()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("DEF_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.DEF);
        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.DEF);


        tempBasalStatus.DEF = (int)Mathf.Round(PlayerData.VIT * targetStatus.VIT +
                PlayerData.Lv * targetStatus.LvCodition) + weaponData + armorData;

    }
    /// <summary>
    /// 迴避值加成
    /// </summary>
    private void Avoid()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("Avoid_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.Avoid);
        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.Avoid);

        tempBasalStatus.Avoid = (int)Mathf.Round(PlayerData.DEX * targetStatus.DEX +
                PlayerData.AGI * targetStatus.AGI) + weaponData + armorData;
    }
    /// <summary>
    /// 進距離命中加成
    /// </summary>
    private void MeleeHit()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MeleeHit_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.MeleeHit);

        tempBasalStatus.MeleeHit = (int)Mathf.Round(PlayerData.STR * targetStatus.STR +
                PlayerData.AGI * targetStatus.AGI +
                PlayerData.Lv * targetStatus.LvCodition) + weaponData;
    }
    /// <summary>
    /// 遠距離命中加成
    /// </summary>
    private void RemoteHit()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("RemoteHit_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.RemoteHit);

        tempBasalStatus.RemoteHit = (int)Mathf.Round(PlayerData.DEX * targetStatus.DEX +
                PlayerData.AGI * targetStatus.AGI +
                PlayerData.Lv * targetStatus.LvCodition) + weaponData;
    }
    /// <summary>
    /// 魔法命中加成
    /// </summary>
    private void MageHit()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MageHit_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.MageHit);

        tempBasalStatus.MageHit = (int)Mathf.Round(PlayerData.INT * targetStatus.INT +
                PlayerData.AGI * targetStatus.AGI +
                PlayerData.Lv * targetStatus.LvCodition) + weaponData;
    }
    /// <summary>
    /// 魔法防禦值加成
    /// </summary>
    private void MDEF()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MDEF_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.MDEF);
        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.MDEF);

        tempBasalStatus.MDEF = (int)Mathf.Round(PlayerData.VIT * targetStatus.VIT +
                PlayerData.WIS * targetStatus.WIS) + weaponData + armorData;
    }
    /// <summary>
    /// 傷害減緩加成
    /// </summary>
    private void DamageReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("DamageReduction_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.DamageReduction);

        tempBasalStatus.DamageReduction = (int)Mathf.Round(PlayerData.VIT * targetStatus.VIT) + armorData;
    }
    /// <summary>
    /// 屬性傷害增幅加成
    /// </summary>
    private void ElementDamageIncrease()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("ElementDamageIncrease_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.ElementDamageIncrease);

        tempBasalStatus.ElementDamageIncrease = (int)Mathf.Round(PlayerData.INT * targetStatus.INT);
    }
    /// <summary>
    /// 屬性傷害抵抗加成
    /// </summary>
    private void ElementDamageReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("ElementDamageReduction_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取防具能力值數據
        float armorData = armorList.Sum(x => x.ElementDamageReduction);


        tempBasalStatus.ElementDamageReduction = (int)Mathf.Round(PlayerData.WIS * targetStatus.WIS) + armorData;
    }

    /// <summary>
    /// 生命恢復加成
    /// </summary>
    private void HP_RecoveryReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("HP_Recovery_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.HpRecovery);

        tempBasalStatus.HP_Recovery =
            (int)Mathf.Round(targetStatus.VIT * PlayerData.VIT + PlayerData.Lv * targetStatus.LvCodition +
            GameData.GameSettingDic[PlayerData.Race + "BasalHpRecovery"].GameSettingValue) + armorData;
    }

    /// <summary>
    /// 魔力恢復加成
    /// </summary>
    private void MP_RecoveryReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MP_Recovery_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.MpRecovery);

        tempBasalStatus.MP_Recovery =
           (int)Mathf.Round(targetStatus.WIS * PlayerData.WIS + PlayerData.Lv * targetStatus.LvCodition +
           GameData.GameSettingDic[PlayerData.Race + "BasalMpRecovery"].GameSettingValue) + armorData;
    }

    #endregion

    /// <summary>
    /// 技能效果運算
    /// <para>在計算完玩家穿戴裝備 種族 職業基本加成的詳細能力值(稱 基礎能力值)</para>
    /// <para>再將玩家當前所有的被動 buff debuff 以 基礎能力值 將技能影響數值做計算</para>
    /// </summary>
    public void SkillEffectStatusOperation(string statusType, bool Rate, float value)
    {
        switch (statusType)
        {
            default:
            case "MeleeATK":
                tempEffectStatus.MeleeATK +=( Rate ? (int)(tempBasalStatus.MeleeATK * value) : (int)value);
                break;
            case "MaxHP":
                tempEffectStatus.MaxHp +=( Rate ? (int)(tempBasalStatus.MaxHp * value) : (int)value);
                break;
            case "DEF":
                tempEffectStatus.DEF += (Rate ? (int)(tempBasalStatus.DEF * value) : (int)value);
                break;
            case "MeleeHit":
                tempEffectStatus.MeleeHit += (Rate ? (int)(tempBasalStatus.MeleeHit * value) : (int)value);
                break;
            case "HP_Recovery":
                tempEffectStatus.HP_Recovery += (Rate ? (int)(tempBasalStatus.HP_Recovery * value) : (int)value);
                break;
            case "MP_Recovery":
                tempEffectStatus.MP_Recovery += (Rate ? (int)(tempBasalStatus.MP_Recovery * value) : (int)value);
                break;
            case "ATK":
                tempEffectStatus.MeleeATK += (Rate ? (int)(tempBasalStatus.MeleeATK * value) : (int)value);
                tempEffectStatus.RemoteATK += (Rate ? (int)(tempBasalStatus.RemoteATK * value) : (int)value);
                tempEffectStatus.MageATK += (Rate ? (int)(tempBasalStatus.MageATK * value) : (int)value);
                break;
            case "BlockRate":
                tempEffectStatus.BlockRate += (Rate ? (int)(tempBasalStatus.BlockRate * value) : (int)value);
                break;
            case "Speed":
                tempEffectStatus.Speed += (Rate ? (int)(tempBasalStatus.Speed * value) : (int)value);
                break;
            case "Crt":
                tempEffectStatus.Crt += (Rate ? (int)(tempBasalStatus.Crt * value) : (int)value);
                break;
            case "AS":
                tempEffectStatus.AS += (Rate ? (int)(tempBasalStatus.AS * value) : (int)value);
                break;
            case "DisorderResistance":
                tempEffectStatus.DisorderResistance += (Rate ? (int)(tempBasalStatus.DisorderResistance * value) : (int)value);
                break;
        }
        PlayerDataStatusOperation();
    }

    /// <summary>
    /// 將效果影響的能力值加成到PlayerData的屬性
    /// </summary>
    public void PlayerDataStatusOperation()
    {
        if (tempEffectStatus != null)
        {
            PlayerData.MeleeATK = (tempEffectStatus.MeleeATK + tempBasalStatus.MeleeATK);
            PlayerData.RemoteATK = (tempEffectStatus.RemoteATK + tempBasalStatus.RemoteATK);
            PlayerData.MageATK = (tempEffectStatus.MageATK + tempBasalStatus.MageATK);
            PlayerData.MaxHP = (tempEffectStatus.MaxHp + tempBasalStatus.MaxHp);
            PlayerData.MaxMP = (tempEffectStatus.MaxMp + tempBasalStatus.MaxMp);
            PlayerData.HP_Recovery = (tempEffectStatus.HP_Recovery + tempBasalStatus.HP_Recovery);
            PlayerData.MP_Recovery = (tempEffectStatus.MP_Recovery + tempBasalStatus.MP_Recovery);
            PlayerData.DEF = (tempEffectStatus.DEF + tempBasalStatus.DEF);
            PlayerData.Avoid = (tempEffectStatus.Avoid + tempBasalStatus.Avoid);
            PlayerData.MeleeHit = (tempEffectStatus.MeleeHit + tempBasalStatus.MeleeHit);
            PlayerData.RemoteHit = (tempEffectStatus.RemoteHit + tempBasalStatus.RemoteHit);
            PlayerData.MageHit = (tempEffectStatus.MageHit + tempBasalStatus.MageHit);
            PlayerData.MDEF = (tempEffectStatus.MDEF + tempBasalStatus.MDEF);
            PlayerData.DamageReduction = (tempEffectStatus.DamageReduction + tempBasalStatus.DamageReduction);
            PlayerData.ElementDamageIncrease = (tempEffectStatus.ElementDamageIncrease + tempBasalStatus.ElementDamageIncrease);
            PlayerData.ElementDamageReduction = (tempEffectStatus.ElementDamageReduction + tempBasalStatus.ElementDamageReduction);
            PlayerData.BlockRate = (tempEffectStatus.BlockRate + tempBasalStatus.BlockRate);
            PlayerData.Speed = (tempEffectStatus.Speed + tempBasalStatus.Speed);
            PlayerData.Crt = (tempEffectStatus.Crt + tempBasalStatus.Crt);
            PlayerData.AS = (tempEffectStatus.AS + tempBasalStatus.AS);
            PlayerData.DisorderResistance = (tempEffectStatus.DisorderResistance + tempBasalStatus.DisorderResistance);
        }
        else
        {
            PlayerData.MeleeATK = tempBasalStatus.MeleeATK;
            PlayerData.RemoteATK = tempBasalStatus.RemoteATK;
            PlayerData.MageATK = tempBasalStatus.MageATK;
            PlayerData.MaxHP = tempBasalStatus.MaxHp;
            PlayerData.MaxMP = tempBasalStatus.MaxMp;
            PlayerData.HP_Recovery = tempBasalStatus.HP_Recovery;
            PlayerData.MP_Recovery = tempBasalStatus.MP_Recovery;
            PlayerData.DEF = tempBasalStatus.DEF;
            PlayerData.Avoid = tempBasalStatus.Avoid;
            PlayerData.MeleeHit = tempBasalStatus.MeleeHit;
            PlayerData.RemoteHit = tempBasalStatus.RemoteHit;
            PlayerData.MageHit = tempBasalStatus.MageHit;
            PlayerData.MDEF = tempBasalStatus.MDEF;
            PlayerData.DamageReduction = tempBasalStatus.DamageReduction;
            PlayerData.ElementDamageIncrease = tempBasalStatus.ElementDamageIncrease;
            PlayerData.ElementDamageReduction = tempBasalStatus.ElementDamageReduction;
            PlayerData.BlockRate = tempBasalStatus.BlockRate;
            PlayerData.Speed = tempBasalStatus.Speed;
            PlayerData.Crt = tempBasalStatus.Crt;
            PlayerData.AS = tempBasalStatus.AS;
            PlayerData.DisorderResistance = tempBasalStatus.DisorderResistance;
        }

        //刷新數據呈現
        PlayerDataPanelProcessor.Instance.SetPlayerDataContent();
    }
}
