using UnityEngine;
using System;
using System.Linq;
using TMPro;

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

    public ItemManager Itemmanager;
    private static Action refreshStatus;

    //暫存基礎能力值
    protected TempBasalStatus tempBasalStatus = new TempBasalStatus();
    //暫存效果影響的能力值
    protected TempBasalStatus tempEffectStatus = new TempBasalStatus();

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
    /// 職業基礎加成能力值
    /// </summary>
    private void ClassStatus()
    {
        PlayerData.STR = int.Parse(GameData.JobBonusDic[PlayerData.Job].STR);
        PlayerData.DEX = int.Parse(GameData.JobBonusDic[PlayerData.Job].DEX);
        PlayerData.INT = int.Parse(GameData.JobBonusDic[PlayerData.Job].INT);
        PlayerData.AGI = int.Parse(GameData.JobBonusDic[PlayerData.Job].AGI);
        PlayerData.WIS = int.Parse(GameData.JobBonusDic[PlayerData.Job].WIS);
        PlayerData.VIT = int.Parse(GameData.JobBonusDic[PlayerData.Job].VIT);
        PlayerData.MaxHP = int.Parse(GameData.JobBonusDic[PlayerData.Job].HP);
        PlayerData.MaxMP = int.Parse(GameData.JobBonusDic[PlayerData.Job].MP);
    }

    /// <summary>
    /// 刷新能力值 (升等、初始化、穿脫裝備等等)
    /// </summary>
    public void StatusMethod()
    {
        InitSub();
        ClassStatus();
        refreshStatus.Invoke();
        PlayerDataStatusOperation();
    }

    #region 能力值加成

    /// <summary>
    /// 近距離攻擊加成
    /// </summary>
    private void MeleeATK()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MeleeATK_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //PlayerData.MeleeATK = (int)Mathf.Round(PlayerData.STR * targetStatus.STR +
        //        PlayerData.Lv * targetStatus.LvCodition);
        tempBasalStatus.MeleeATK = (int)Mathf.Round(PlayerData.STR * targetStatus.STR +
              PlayerData.Lv * targetStatus.LvCodition);


        //裝備效果計算
        //if (Itemmanager.Weapons[1] != null)
        //{
        //    PlayerData.MeleeATK += (int)Mathf.Round((float)(Itemmanager.Weapons[1].GetComponent<Equipment>().Weapon.WeaponCATK * 0.15));
        //}
        //if (Itemmanager.Weapons[0] != null)
        //{
        //    PlayerData.MeleeATK += Itemmanager.Weapons[0].GetComponent<Equipment>().Weapon.WeaponCATK;
        //}
    }
    /// <summary>
    /// 遠距離攻擊加成
    /// </summary>
    private void RemoteATK()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("RemoteATK_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //PlayerData.RemoteATK = (int)Mathf.Round(PlayerData.DEX * targetStatus.DEX +
        //        PlayerData.Lv * targetStatus.LvCodition);
        tempBasalStatus.RemoteATK = (int)Mathf.Round(PlayerData.DEX * targetStatus.DEX +
                PlayerData.Lv * targetStatus.LvCodition);

        //if (Itemmanager.Weapons[0] != null)
        //{
        //    DataBase.Instance.Playerattributes.LATK += Itemmanager.Weapons[0].GetComponent<Equipment>().Weapon.WeaponLATK;
        //}
    }
    /// <summary>
    /// 魔法攻擊加成
    /// </summary>
    private void MageATK()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MageATK_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //PlayerData.MageATK = (int)Mathf.Round(PlayerData.INT * targetStatus.INT +
        //        PlayerData.Lv * targetStatus.LvCodition);
        tempBasalStatus.MageATK = (int)Mathf.Round(PlayerData.INT * targetStatus.INT +
                PlayerData.Lv * targetStatus.LvCodition);
        //if (Itemmanager.Weapons[0] != null)
        //{
        //    DataBase.Instance.Playerattributes.MATK += Itemmanager.Weapons[0].GetComponent<Equipment>().Weapon.WeaponMATK;
        //}
    }
    /// <summary>
    /// 最大生命加成
    /// </summary>
    private void MaxHp()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("HP_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        //PlayerData.MaxHP = (int)Mathf.Round(PlayerData.VIT * targetStatus.VIT +
        //        PlayerData.STR * targetStatus.STR +
        //        PlayerData.Lv * targetStatus.LvCodition);
        tempBasalStatus.MaxHp = (int)Mathf.Round(PlayerData.VIT * targetStatus.VIT +
                PlayerData.STR * targetStatus.STR +
                PlayerData.Lv * targetStatus.LvCodition);
        //for (int i = 0; i < Itemmanager.Equipments.Length; i++)
        //{
        //    if (Itemmanager.Equipments[i] != null) DataBase.Instance.Playerattributes.MaxHp += Itemmanager.Equipments[i].GetComponent<Equipment>().Armor.ArmorHp;
        //}
        //if (Itemmanager.Weapons[1] != null)
        //{
        //    DataBase.Instance.Playerattributes.MaxHp += Itemmanager.Weapons[1].GetComponent<Equipment>().Weapon.ShieldHP;
        //}
    }
    /// <summary>
    /// 最大魔力加成
    /// </summary>
    private void MaxMp()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MP_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.MaxMp = (int)Mathf.Round(PlayerData.INT * targetStatus.INT +
                PlayerData.Lv * targetStatus.LvCodition);

        //for (int i = 0; i < Itemmanager.Equipments.Length; i++)
        //{
        //    if (Itemmanager.Equipments[i] != null) DataBase.Instance.Playerattributes.MaxMp += Itemmanager.Equipments[i].GetComponent<Equipment>().Armor.ArmorMp;
        //}
        //if (Itemmanager.Weapons[1] != null)
        //{
        //    DataBase.Instance.Playerattributes.MaxMp += Itemmanager.Weapons[1].GetComponent<Equipment>().Weapon.ShieldMP;
        //}
    }
    /// <summary>
    /// 物理防禦加成
    /// </summary>
    private void DEF()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("DEF_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.DEF = (int)Mathf.Round(PlayerData.VIT * targetStatus.VIT +
                PlayerData.Lv * targetStatus.LvCodition);

        //for (int i = 0; i < Itemmanager.Equipments.Length; i++)
        //{
        //    if (Itemmanager.Equipments[i] != null) DataBase.Instance.Playerattributes.Defense += Itemmanager.Equipments[i].GetComponent<Equipment>().Armor.ArmorDEF;
        //}
        //if (Itemmanager.Weapons[1] != null)
        //{
        //    DataBase.Instance.Playerattributes.Defense += Itemmanager.Weapons[1].GetComponent<Equipment>().Weapon.ShieldDEF;
        //}
    }
    /// <summary>
    /// 迴避值加成
    /// </summary>
    private void Avoid()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("Avoid_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.Avoid = (int)Mathf.Round(PlayerData.DEX * targetStatus.DEX +
                PlayerData.AGI * targetStatus.AGI);

        //for (int i = 0; i < Itemmanager.Equipments.Length; i++)
        //{
        //    if (Itemmanager.Equipments[i] != null) DataBase.Instance.Playerattributes.Avoid += Itemmanager.Equipments[i].GetComponent<Equipment>().Armor.ArmorAvoid;
        //}
        //if (Itemmanager.Weapons[1] != null)
        //{
        //    DataBase.Instance.Playerattributes.Avoid += Itemmanager.Weapons[1].GetComponent<Equipment>().Weapon.ShieldAvoid;
        //}
    }
    /// <summary>
    /// 進距離命中加成
    /// </summary>
    private void MeleeHit()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MeleeHit_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.MeleeHit = (int)Mathf.Round(PlayerData.STR * targetStatus.STR +
                PlayerData.AGI * targetStatus.AGI +
                PlayerData.Lv * targetStatus.LvCodition);

        //if (Itemmanager.Weapons[1] != null)
        //{
        //    DataBase.Instance.Playerattributes.C_Hit += Itemmanager.Weapons[1].GetComponent<Equipment>().Weapon.WeaponCHIT;
        //}
        //if (Itemmanager.Weapons[0] != null)
        //{
        //    DataBase.Instance.Playerattributes.C_Hit += Itemmanager.Weapons[0].GetComponent<Equipment>().Weapon.WeaponCHIT;
        //}
    }
    /// <summary>
    /// 遠距離命中加成
    /// </summary>
    private void RemoteHit()
    {

        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("RemoteHit_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.RemoteHit = (int)Mathf.Round(PlayerData.DEX * targetStatus.DEX +
                PlayerData.AGI * targetStatus.AGI +
                PlayerData.Lv * targetStatus.LvCodition);

        //if (Itemmanager.Weapons[0] != null)
        //{
        //    DataBase.Instance.Playerattributes.L_Hit += Itemmanager.Weapons[0].GetComponent<Equipment>().Weapon.WeaponLHIT;
        //}
    }
    /// <summary>
    /// 魔法命中加成
    /// </summary>
    private void MageHit()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MageHit_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.MageHit = (int)Mathf.Round(PlayerData.INT * targetStatus.INT +
                PlayerData.AGI * targetStatus.AGI +
                PlayerData.Lv * targetStatus.LvCodition);

        //if (Itemmanager.Weapons[0] != null)
        //{
        //    DataBase.Instance.Playerattributes.M_Hit += Itemmanager.Weapons[0].GetComponent<Equipment>().Weapon.WeaponMHIT;
        //}
    }
    /// <summary>
    /// 魔法防禦值加成
    /// </summary>
    private void MDEF()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MDEF_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.MDEF = (int)Mathf.Round(PlayerData.VIT * targetStatus.VIT +
                PlayerData.WIS * targetStatus.WIS);

        //for (int i = 0; i < Itemmanager.Equipments.Length; i++)
        //{
        //    if (Itemmanager.Equipments[i] != null) DataBase.Instance.Playerattributes.M_Defense += Itemmanager.Equipments[i].GetComponent<Equipment>().Armor.ArmorMDEF;
        //}
        //if (Itemmanager.Weapons[1] != null)
        //{
        //    DataBase.Instance.Playerattributes.M_Defense += Itemmanager.Weapons[1].GetComponent<Equipment>().Weapon.ShieldMDEF;
        //}
    }
    /// <summary>
    /// 傷害減緩加成
    /// </summary>
    private void DamageReduction()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("DamageReduction_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.DamageReduction = (int)Mathf.Round(PlayerData.VIT * targetStatus.VIT);

        //for (int i = 0; i < Itemmanager.Equipments.Length; i++)
        //{
        //    if (Itemmanager.Equipments[i] != null) DataBase.Instance.Playerattributes.DmgMitigation += Itemmanager.Equipments[i].GetComponent<Equipment>().Armor.ArmorDmgM;
        //}
    }
    /// <summary>
    /// 屬性傷害增幅加成
    /// </summary>
    private void ElementDamageIncrease()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("ElementDamageIncrease_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.ElementDamageIncrease = (int)Mathf.Round(PlayerData.INT * targetStatus.INT);
    }
    /// <summary>
    /// 屬性傷害抵抗加成
    /// </summary>
    private void ElementDamageReduction()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("ElementDamageReduction_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.ElementDamageReduction = (int)Mathf.Round(PlayerData.WIS * targetStatus.WIS);
    }

    /// <summary>
    /// 生命恢復加成
    /// </summary>
    private void HP_RecoveryReduction()
    {
        var targetStatus =          
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("HP_Recovery_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.HP_Recovery =
            (int)Mathf.Round(targetStatus.VIT * PlayerData.VIT + PlayerData.Lv * targetStatus.LvCodition + 
            GameData.GameSettingDic[PlayerData.Race + "BasalHpRecovery"].GameSettingValue);
    }

    /// <summary>
    /// 魔力恢復加成
    /// </summary>
    private void MP_RecoveryReduction()
    {
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MP_Recovery_" + PlayerData.Race))
                .Select(x => x.Value).FirstOrDefault();

        tempBasalStatus.MP_Recovery =
           (int)Mathf.Round(targetStatus.WIS * PlayerData.WIS + PlayerData.Lv * targetStatus.LvCodition +
           GameData.GameSettingDic[PlayerData.Race + "BasalMpRecovery"].GameSettingValue);
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
                tempEffectStatus.MeleeATK += Rate ? (int)(tempBasalStatus.MeleeATK * value) : (int)value;
                break;
            case "MaxHP":
                tempEffectStatus.MaxHp += Rate ? (int)(tempBasalStatus.MaxHp * value) : (int)value;
                break;
            case "DEF":
                tempEffectStatus.DEF += Rate ? (int)(tempBasalStatus.DEF * value) : (int)value;
                break;
            case "MeleeHit":
                tempEffectStatus.MeleeHit += Rate ? (int)(tempBasalStatus.MeleeHit * value) : (int)value;
                break;
            case "HP_Recovery":
                tempEffectStatus.HP_Recovery += Rate ? (int)(tempBasalStatus.HP_Recovery * value) : (int)value;
                break;
            case "MP_Recovery":
                tempEffectStatus.MP_Recovery += Rate ? (int)(tempBasalStatus.MP_Recovery * value) : (int)value;
                break;
            case "ATK":
                tempEffectStatus.MeleeATK += Rate ? (int)(tempBasalStatus.MeleeATK * value) : (int)value;
                tempEffectStatus.RemoteATK += Rate ? (int)(tempBasalStatus.RemoteATK * value) : (int)value;
                tempEffectStatus.MageATK += Rate ? (int)(tempBasalStatus.MageATK * value) : (int)value;
                break;
            case "BlockRate":
                tempEffectStatus.BlockRate += Rate ? (int)(tempBasalStatus.BlockRate * value) : (int)value;
                break;
            case "Speed":
                tempEffectStatus.Speed += Rate ? (int)(tempBasalStatus.Speed * value) : (int)value;
                break;
            case "Crt":
                tempEffectStatus.Crt += Rate ? (int)(tempBasalStatus.Crt * value) : (int)value;
                break;
            case "AS":
                tempEffectStatus.AS += Rate ? (int)(tempBasalStatus.AS * value) : (int)value;
                break;
            case "DisorderResistance":
                tempEffectStatus.DisorderResistance += Rate ? (int)(tempBasalStatus.DisorderResistance * value) : (int)value;
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
            PlayerData.MeleeATK += (tempEffectStatus.MeleeATK + tempBasalStatus.MeleeATK);
            PlayerData.RemoteATK += (tempEffectStatus.RemoteATK + tempBasalStatus.RemoteATK);
            PlayerData.MageATK += (tempEffectStatus.MageATK + tempBasalStatus.MageATK);
            PlayerData.MaxHP += (tempEffectStatus.MaxHp + tempBasalStatus.MaxHp);
            PlayerData.MaxMP += (tempEffectStatus.MaxMp + tempBasalStatus.MaxMp);
            PlayerData.HP_Recovery += (tempEffectStatus.HP_Recovery + tempBasalStatus.HP_Recovery);
            PlayerData.MP_Recovery += (tempEffectStatus.MP_Recovery + tempBasalStatus.MP_Recovery);
            PlayerData.DEF += (tempEffectStatus.DEF + tempBasalStatus.DEF);
            PlayerData.Avoid += (tempEffectStatus.Avoid + tempBasalStatus.Avoid);
            PlayerData.MeleeHit += (tempEffectStatus.MeleeHit + tempBasalStatus.MeleeHit);
            PlayerData.RemoteHit += (tempEffectStatus.RemoteHit + tempBasalStatus.RemoteHit);
            PlayerData.MageHit += (tempEffectStatus.MageHit + tempBasalStatus.MageHit);
            PlayerData.MDEF += (tempEffectStatus.MDEF + tempBasalStatus.MDEF);
            PlayerData.DamageReduction += (tempEffectStatus.DamageReduction + tempBasalStatus.DamageReduction);
            PlayerData.ElementDamageIncrease += (tempEffectStatus.ElementDamageIncrease + tempBasalStatus.ElementDamageIncrease);
            PlayerData.ElementDamageReduction += (tempEffectStatus.ElementDamageReduction + tempBasalStatus.ElementDamageReduction);
            PlayerData.BlockRate += (tempEffectStatus.BlockRate + tempBasalStatus.BlockRate);
            PlayerData.Speed += (tempEffectStatus.Speed + tempBasalStatus.Speed);
            PlayerData.Crt += (tempEffectStatus.Crt + tempBasalStatus.Crt);
            PlayerData.AS += (tempEffectStatus.AS + tempBasalStatus.AS);
            PlayerData.DisorderResistance += (tempEffectStatus.DisorderResistance + tempBasalStatus.DisorderResistance);
        }
        else
        {
            PlayerData.MeleeATK += tempBasalStatus.MeleeATK;
            PlayerData.RemoteATK += tempBasalStatus.RemoteATK;
            PlayerData.MageATK += tempBasalStatus.MageATK;
            PlayerData.MaxHP += tempBasalStatus.MaxHp;
            PlayerData.MaxMP += tempBasalStatus.MaxMp;
            PlayerData.HP_Recovery += tempBasalStatus.HP_Recovery;
            PlayerData.MP_Recovery += tempBasalStatus.MP_Recovery;
            PlayerData.DEF += tempBasalStatus.DEF;
            PlayerData.Avoid += tempBasalStatus.Avoid;
            PlayerData.MeleeHit += tempBasalStatus.MeleeHit;
            PlayerData.RemoteHit += tempBasalStatus.RemoteHit;
            PlayerData.MageHit += tempBasalStatus.MageHit;
            PlayerData.MDEF += tempBasalStatus.MDEF;
            PlayerData.DamageReduction += tempBasalStatus.DamageReduction;
            PlayerData.ElementDamageIncrease += tempBasalStatus.ElementDamageIncrease;
            PlayerData.ElementDamageReduction += tempBasalStatus.ElementDamageReduction;
            PlayerData.BlockRate += tempBasalStatus.BlockRate;
            PlayerData.Speed += tempBasalStatus.Speed;
            PlayerData.Crt += tempBasalStatus.Crt;
            PlayerData.AS += tempBasalStatus.AS;
            PlayerData.DisorderResistance += tempBasalStatus.DisorderResistance;
        }

        //刷新數據呈現
        PlayerDataPanelProcessor.Instance.SetPlayerDataContent();
    }
}
