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

    //武器資料清單
    private List<WeaponDataModel> weaponList = new List<WeaponDataModel>();
    //防具資料清單
    private List<ArmorDataModel> armorList = new List<ArmorDataModel>();

    //基礎能力值加成事件
    private static Action refreshStatus;
    //需等待基礎數值計算完後才計算的部分
    private static Action refreshAfterStatus;

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
        //發送空條件 讓不須條件的物件通過檢查
        SkillController.Instance.SkillConditionCheckEvent?.Invoke("", null);
        //武器清單
        weaponList = new List<WeaponDataModel>();
        foreach (var item in ItemManager.Instance.EquipDataList)
        {
            if (item.EquipmentDatas.Weapon != null)
            {
                weaponList.Add(item.EquipmentDatas.Weapon);
                SkillController.Instance.SkillConditionCheckEvent?.Invoke("EquipWeapon", item.EquipmentDatas.Weapon.TypeID);
            }
        }
        //防具清單
        armorList = new List<ArmorDataModel>();
        foreach (var item in ItemManager.Instance.EquipDataList)
        {
            if (item.EquipmentDatas.Armor != null)
            {
                armorList.Add(item.EquipmentDatas.Armor);
                SkillController.Instance.SkillConditionCheckEvent?.Invoke("EquipArmor", item.EquipmentDatas.Armor.TypeID);
            }
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
        PlayerDataOverView.Instance.PlayerData_.STR = int.Parse(GameData.JobBonusDic[PlayerDataOverView.Instance.PlayerData_.Job].STR) + weaponDataSTR + armorDataSTR;
        PlayerDataOverView.Instance.PlayerData_.DEX = int.Parse(GameData.JobBonusDic[PlayerDataOverView.Instance.PlayerData_.Job].DEX) + weaponDataDEX + armorDataDEX;
        PlayerDataOverView.Instance.PlayerData_.INT = int.Parse(GameData.JobBonusDic[PlayerDataOverView.Instance.PlayerData_.Job].INT) + weaponDataINT + armorDataINT;
        PlayerDataOverView.Instance.PlayerData_.AGI = int.Parse(GameData.JobBonusDic[PlayerDataOverView.Instance.PlayerData_.Job].AGI) + weaponDataAGI + armorDataAGI;
        PlayerDataOverView.Instance.PlayerData_.WIS = int.Parse(GameData.JobBonusDic[PlayerDataOverView.Instance.PlayerData_.Job].WIS) + weaponDataWIS + armorDataWIS;
        PlayerDataOverView.Instance.PlayerData_.VIT = int.Parse(GameData.JobBonusDic[PlayerDataOverView.Instance.PlayerData_.Job].VIT) + weaponDataVIT + armorDataVIT;
        PlayerDataOverView.Instance.PlayerData_.MaxHP = int.Parse(GameData.JobBonusDic[PlayerDataOverView.Instance.PlayerData_.Job].HP);
        PlayerDataOverView.Instance.PlayerData_.MaxMP = int.Parse(GameData.JobBonusDic[PlayerDataOverView.Instance.PlayerData_.Job].MP);
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
        }
        else
        {
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
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MeleeATK_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();
        //獲取裝備能力值數據
        int weaponData = weaponList.Sum(x => x.MeleeATK);

        tempBasalStatus.MeleeATK = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.STR * targetStatus.STR +
              PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + weaponData;
    }

    /// <summary>
    /// 遠距離攻擊加成
    /// </summary>
    private void RemoteATK()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("RemoteATK_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取裝備能力值數據
        int weaponData = weaponList.Sum(x => x.RemoteATK);
        tempBasalStatus.RemoteATK = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.DEX * targetStatus.DEX +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + weaponData;
    }

    /// <summary>
    /// 魔法攻擊加成
    /// </summary>
    private void MageATK()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MageATK_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取裝備能力值數據
        int weaponData = weaponList.Sum(x => x.MageATK);

        tempBasalStatus.MageATK = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.INT * targetStatus.INT +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + weaponData;

    }

    /// <summary>
    /// 最大生命加成
    /// </summary>
    private void MaxHp()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("HP_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.HP);
        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.HP);

        tempBasalStatus.MaxHP = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.VIT * targetStatus.VIT +
                PlayerDataOverView.Instance.PlayerData_.STR * targetStatus.STR +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + weaponData + armorData + PlayerDataOverView.Instance.PlayerData_.MaxHP;
    }

    /// <summary>
    /// 最大魔力加成
    /// </summary>
    private void MaxMp()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MP_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.MP);
        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.MP);

        tempBasalStatus.MaxMP = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.INT * targetStatus.INT +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + weaponData + armorData + PlayerDataOverView.Instance.PlayerData_.MaxMP;
    }

    /// <summary>
    /// 物理防禦加成
    /// </summary>
    private void DEF()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("DEF_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.DEF);
        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.DEF);


        tempBasalStatus.DEF = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.VIT * targetStatus.VIT +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + weaponData + armorData;

    }

    /// <summary>
    /// 迴避值加成
    /// </summary>
    private void Avoid()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("Avoid_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.Avoid);
        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.Avoid);

        tempBasalStatus.Avoid = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.DEX * targetStatus.DEX +
                PlayerDataOverView.Instance.PlayerData_.AGI * targetStatus.AGI) + weaponData + armorData;
    }

    /// <summary>
    /// 進距離命中加成
    /// </summary>
    private void MeleeHit()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MeleeHit_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.MeleeHit);

        tempBasalStatus.MeleeHit = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.STR * targetStatus.STR +
                PlayerDataOverView.Instance.PlayerData_.AGI * targetStatus.AGI +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + weaponData;
    }

    /// <summary>
    /// 遠距離命中加成
    /// </summary>
    private void RemoteHit()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("RemoteHit_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.RemoteHit);

        tempBasalStatus.RemoteHit = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.DEX * targetStatus.DEX +
                PlayerDataOverView.Instance.PlayerData_.AGI * targetStatus.AGI +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + weaponData;
    }

    /// <summary>
    /// 魔法命中加成
    /// </summary>
    private void MageHit()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MageHit_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.MageHit);

        tempBasalStatus.MageHit = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.INT * targetStatus.INT +
                PlayerDataOverView.Instance.PlayerData_.AGI * targetStatus.AGI +
                PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition) + weaponData;
    }

    /// <summary>
    /// 魔法防禦值加成
    /// </summary>
    private void MDEF()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MDEF_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = weaponList.Sum(x => x.MDEF);
        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.MDEF);

        tempBasalStatus.MDEF = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.VIT * targetStatus.VIT +
                PlayerDataOverView.Instance.PlayerData_.WIS * targetStatus.WIS) + weaponData + armorData;
    }

    /// <summary>
    /// 移動速度
    /// </summary>
    private void Speed()
    {
        //獲取防具能力值數據
        float armorData = armorList.Sum(x => x.Speed);

        tempBasalStatus.Speed = armorData.Equals(0) ? 1 : 1 + armorData;
    }

    /// <summary>
    /// 攻擊速度值加成
    /// </summary>
    private void AS()
    {
        //獲取武器能力值數據
        float weaponAS = weaponList.Select(x => GameData.GameSettingDic[x.ASID].GameSettingValue).FirstOrDefault();

        tempBasalStatus.AS = weaponAS.Equals(0) ? 1 : weaponAS;
    }

    /// <summary>
    /// 傷害減緩加成
    /// </summary>
    private void DamageReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("DamageReduction_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.DamageReduction);

        tempBasalStatus.DamageReduction = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.VIT * targetStatus.VIT) + armorData;
    }

    /// <summary>
    /// 屬性傷害增幅加成
    /// </summary>
    private void ElementDamageIncrease()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("ElementDamageIncrease_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取武器能力值數據
        int weaponData = (int)(weaponList.Sum(x => x.ElementDamageIncrease));

        tempBasalStatus.ElementDamageIncrease = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.INT * targetStatus.INT);
    }

    /// <summary>
    /// 屬性傷害抵抗加成
    /// </summary>
    private void ElementDamageReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("ElementDamageReduction_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取防具能力值數據
        float armorData = armorList.Sum(x => x.ElementDamageReduction);


        tempBasalStatus.ElementDamageReduction = (int)Mathf.Round(PlayerDataOverView.Instance.PlayerData_.WIS * targetStatus.WIS) + armorData;
    }

    /// <summary>
    /// 生命恢復加成
    /// </summary>
    private void HP_RecoveryReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("HP_Recovery_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.HpRecovery);

        tempBasalStatus.HP_Recovery =
            (int)Mathf.Round(targetStatus.VIT * PlayerDataOverView.Instance.PlayerData_.VIT + PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition +
            GameData.GameSettingDic[PlayerDataOverView.Instance.PlayerData_.Race + "BasalHpRecovery"].GameSettingValue) + armorData;
    }

    /// <summary>
    /// 魔力恢復加成
    /// </summary>
    private void MP_RecoveryReduction()
    {
        //獲取種族能力值的成長加成
        var targetStatus =
                GameData.StatusFormulaDic.Where(x => x.Key.Contains("MP_Recovery_" + PlayerDataOverView.Instance.PlayerData_.Race))
                .Select(x => x.Value).FirstOrDefault();

        //獲取防具能力值數據
        int armorData = armorList.Sum(x => x.MpRecovery);

        tempBasalStatus.MP_Recovery =
           (int)Mathf.Round(targetStatus.WIS * PlayerDataOverView.Instance.PlayerData_.WIS + PlayerDataOverView.Instance.PlayerData_.Lv * targetStatus.LvCodition +
           GameData.GameSettingDic[PlayerDataOverView.Instance.PlayerData_.Race + "BasalMpRecovery"].GameSettingValue) + armorData;
    }

    /// <summary>
    /// 取得裝備武器的普通攻擊範圍
    /// </summary>
    public void GetNormalAttackRange()
    {
        /*腳本下中斷點 經過  weaponType = "MeleeAttackRange"; 後 後面都不會執行 帶查驗修正*/

        string weaponType = "";
        //若沒有武器則為空手 視同近戰
        if (weaponList.Count == 0)
        {
            weaponType = "MeleeAttackRange";
        }
        else
            //若有武器 檢查是否有物理攻擊資料 判斷是近距離型武器或是遠距離
            weaponType = (weaponList.Any(x => x.MeleeATK != 0) ? "MeleeAttackRange" : "RemoteAttackRange");

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
        //取得 能力值裡對應的欄位參數(技能效果)
        FieldInfo effectProperty = typeof(TempBasalStatus).GetField(statusType);
        //取得 能力值裡對應的欄位參數(當前基礎屬性)
        FieldInfo basalProperty = typeof(TempBasalStatus).GetField(statusType);

        //檢查空值
        if (effectProperty != null && basalProperty != null)
        {
            //若效果為 全部攻擊 屬性
            if (statusType == "ATK")
            {
                //取得所有攻擊屬性
                string[] atkTypes = { "MeleeATK", "RemoteATK", "MageATK" };
                //依次增加所有攻擊屬性
                foreach (string atkType in atkTypes)
                {
                    //取得能力值對應的攻擊欄位參數(技能效果)
                    FieldInfo effectATKProperty = typeof(TempBasalStatus).GetField(atkType);
                    //取得能力值對應的攻擊欄位參數(當前基礎屬性)
                    FieldInfo basalATKProperty = typeof(TempBasalStatus).GetField(atkType);

                    //檢查空值
                    if (effectProperty != null && basalProperty != null)
                    {
                        //取得 技能效果 能力值裡對應的攻擊參數的數值
                        int effectValue = (int)effectATKProperty.GetValue(tempEffectStatus);
                        //取得 當前基礎屬性 能力值裡對應的攻擊參數的數值
                        int basalValue = (int)basalATKProperty.GetValue(tempBasalStatus);

                        //依照加成或倍率計算數值
                        effectValue += (isRate ? (int)(basalValue * value) : (int)value);

                        //設定技能效果屬性的數值
                        effectProperty.SetValue(tempEffectStatus, effectValue);
                    }
                }
            }
            //若效果為 移動速度 屬性
            else if (statusType == "Speed")
            {
                //取得 技能效果 能力值裡對應的參數的數值
                int effectValue = (int)effectProperty.GetValue(tempEffectStatus);
                //設定技能效果屬性的數值 (移動速度的算法比較特別 "倍率"的話以速度基準值來計算 不會用穿上裝備的加總)
                effectProperty.SetValue(tempEffectStatus, effectValue + (isRate ? (int)(1 * value) : (int)value));
            }
            //其餘屬性正常運算
            else
            {
                //取得 技能效果 能力值裡對應的參數的數值
                if (effectProperty.GetValue(tempEffectStatus) is int)
                {
                    int effectValue = (int)effectProperty.GetValue(tempEffectStatus);
                    //取得 當前基礎屬性 能力值裡對應的參數的數值
                    int basalValue = (int)basalProperty.GetValue(tempBasalStatus);
                    //依照加成或倍率計算數值
                    effectValue += (isRate ? (int)(basalValue * value) : (int)value);

                    //設定技能效果屬性的數值
                    effectProperty.SetValue(tempEffectStatus, effectValue);
                }
                else if (effectProperty.GetValue(tempEffectStatus) is float)
                {
                    float effectValue = (float)effectProperty.GetValue(tempEffectStatus);
                    //取得 當前基礎屬性 能力值裡對應的參數的數值
                    float basalValue = (float)basalProperty.GetValue(tempBasalStatus);
                    //依照加成或倍率計算數值
                    effectValue += (isRate ? (float)(basalValue * value) : (float)value);

                    //設定技能效果屬性的數值
                    effectProperty.SetValue(tempEffectStatus, effectValue);
                }


            }
        }
        else
        {
            Debug.Log($"未宣告參數或是輸入了不知名參數: {statusType}");
        }
        //將效果影響的能力值進行運算
        PlayerDataStatusOperation();
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
        if (PlayerDataOverView.Instance && tempEffectStatus != null)
        {
            PlayerDataOverView.Instance.PlayerData_.MeleeATK = (tempEffectStatus.MeleeATK + tempBasalStatus.MeleeATK);
            PlayerDataOverView.Instance.PlayerData_.RemoteATK = (tempEffectStatus.RemoteATK + tempBasalStatus.RemoteATK);
            PlayerDataOverView.Instance.PlayerData_.MageATK = (tempEffectStatus.MageATK + tempBasalStatus.MageATK);
            PlayerDataOverView.Instance.PlayerData_.MaxHP = (tempEffectStatus.MaxHP + tempBasalStatus.MaxHP);
            PlayerDataOverView.Instance.PlayerData_.MaxMP = (tempEffectStatus.MaxMP + tempBasalStatus.MaxMP);
            PlayerDataOverView.Instance.PlayerData_.HP_Recovery = (tempEffectStatus.HP_Recovery + tempBasalStatus.HP_Recovery);
            PlayerDataOverView.Instance.PlayerData_.MP_Recovery = (tempEffectStatus.MP_Recovery + tempBasalStatus.MP_Recovery);
            PlayerDataOverView.Instance.PlayerData_.DEF = (tempEffectStatus.DEF + tempBasalStatus.DEF);
            PlayerDataOverView.Instance.PlayerData_.Avoid = (tempEffectStatus.Avoid + tempBasalStatus.Avoid);
            PlayerDataOverView.Instance.PlayerData_.MeleeHit = (tempEffectStatus.MeleeHit + tempBasalStatus.MeleeHit);
            PlayerDataOverView.Instance.PlayerData_.RemoteHit = (tempEffectStatus.RemoteHit + tempBasalStatus.RemoteHit);
            PlayerDataOverView.Instance.PlayerData_.MageHit = (tempEffectStatus.MageHit + tempBasalStatus.MageHit);
            PlayerDataOverView.Instance.PlayerData_.MDEF = (tempEffectStatus.MDEF + tempBasalStatus.MDEF);
            PlayerDataOverView.Instance.PlayerData_.DamageReduction = (tempEffectStatus.DamageReduction + tempBasalStatus.DamageReduction);
            PlayerDataOverView.Instance.PlayerData_.ElementDamageIncrease = (tempEffectStatus.ElementDamageIncrease + tempBasalStatus.ElementDamageIncrease);
            PlayerDataOverView.Instance.PlayerData_.ElementDamageReduction = (tempEffectStatus.ElementDamageReduction + tempBasalStatus.ElementDamageReduction);
            PlayerDataOverView.Instance.PlayerData_.BlockRate = (tempEffectStatus.BlockRate + tempBasalStatus.BlockRate);
            PlayerDataOverView.Instance.PlayerData_.Speed = (tempEffectStatus.Speed + tempBasalStatus.Speed);
            PlayerDataOverView.Instance.PlayerData_.Crt = (tempEffectStatus.Crt + tempBasalStatus.Crt);
            PlayerDataOverView.Instance.PlayerData_.AS = (tempEffectStatus.AS + tempBasalStatus.AS);
            PlayerDataOverView.Instance.PlayerData_.DisorderResistance = (tempEffectStatus.DisorderResistance + tempBasalStatus.DisorderResistance);
        }
        else if (PlayerDataOverView.Instance)
        {
            PlayerDataOverView.Instance.PlayerData_.MeleeATK = tempBasalStatus.MeleeATK;
            PlayerDataOverView.Instance.PlayerData_.RemoteATK = tempBasalStatus.RemoteATK;
            PlayerDataOverView.Instance.PlayerData_.MageATK = tempBasalStatus.MageATK;
            PlayerDataOverView.Instance.PlayerData_.MaxHP = tempBasalStatus.MaxHP;
            PlayerDataOverView.Instance.PlayerData_.MaxMP = tempBasalStatus.MaxMP;
            PlayerDataOverView.Instance.PlayerData_.HP_Recovery = tempBasalStatus.HP_Recovery;
            PlayerDataOverView.Instance.PlayerData_.MP_Recovery = tempBasalStatus.MP_Recovery;
            PlayerDataOverView.Instance.PlayerData_.DEF = tempBasalStatus.DEF;
            PlayerDataOverView.Instance.PlayerData_.Avoid = tempBasalStatus.Avoid;
            PlayerDataOverView.Instance.PlayerData_.MeleeHit = tempBasalStatus.MeleeHit;
            PlayerDataOverView.Instance.PlayerData_.RemoteHit = tempBasalStatus.RemoteHit;
            PlayerDataOverView.Instance.PlayerData_.MageHit = tempBasalStatus.MageHit;
            PlayerDataOverView.Instance.PlayerData_.MDEF = tempBasalStatus.MDEF;
            PlayerDataOverView.Instance.PlayerData_.DamageReduction = tempBasalStatus.DamageReduction;
            PlayerDataOverView.Instance.PlayerData_.ElementDamageIncrease = tempBasalStatus.ElementDamageIncrease;
            PlayerDataOverView.Instance.PlayerData_.ElementDamageReduction = tempBasalStatus.ElementDamageReduction;
            PlayerDataOverView.Instance.PlayerData_.BlockRate = tempBasalStatus.BlockRate;
            PlayerDataOverView.Instance.PlayerData_.Speed = tempBasalStatus.Speed;
            PlayerDataOverView.Instance.PlayerData_.Crt = tempBasalStatus.Crt;
            PlayerDataOverView.Instance.PlayerData_.AS = tempBasalStatus.AS;
            PlayerDataOverView.Instance.PlayerData_.DisorderResistance = tempBasalStatus.DisorderResistance;
        }
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
