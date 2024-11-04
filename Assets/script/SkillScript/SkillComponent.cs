using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/10/22
//  創建用途:技能組件接口繼承實例
//==========================================
public abstract class SkillComponent : ISkillComponent
{
    protected Skill_Base skillbase;
    public Skill_Base SkillBase => skillbase;
    protected SkillOperationData skillOperationData;
    public SkillOperationData SkillOperationData => skillOperationData;

    public abstract void Execute(ICombatant caster, ICombatant target);

    /// <summary>
    /// 確認條件
    /// </summary>
    /// <param name="skillOperationData"></param>
    /// <returns></returns>
    protected bool CheckCondition(List<SkillOperationData> skillOperationData, ICombatant caster, ICombatant target)
    {
        List<bool> checkResult = new List<bool>();
        //找出每個技能運算資料
        foreach (var item in skillOperationData)
        {
            //暫存技能施放條件資料
            List<string> condition = item.ConditionOR;
            //若不需要條件直接跳過
            if (condition == null || condition.Count <= 0) continue;
            foreach (var data in condition)
            {
                checkResult.Add(DetailConditionProcess(data, caster, target));
            }
        }
        //回傳條件結果
        return checkResult.All(x => x == true);
    }

    /// <summary>
    /// 詳細條件判斷處理
    /// </summary>
    /// <param name="key">條件名稱</param>
    /// <param name="value">條件判斷值</param>
    /// <returns></returns>
    protected bool DetailConditionProcess(string condition, ICombatant caster, ICombatant target)
    {
        //宣告 判斷條件清單
        List<bool> finalResult = new List<bool>();
        //宣告 儲存條件判斷字典
        Dictionary<string, object> conditionDetail = new Dictionary<string, object>();

        //處理 | 字元
        if (condition.Contains('|'))
        {
            List<string> conditionSplit = condition.Split('|').ToList();
            foreach (var item in conditionSplit)
            {
                //處理_字元
                if (item.Contains('_'))
                    conditionDetail.Add(item.Split('_')[0], item.Split('_')[0]);
                else
                    conditionDetail.Add(item, null);
            }
        }
        else
        {
            //處理_字元
            if (condition.Contains('_'))
                conditionDetail.Add(condition.Split('_')[0], condition.Split('_')[0]);
            else
                conditionDetail.Add(condition, null);
        }

        //根據處理完的字典資料 判斷條件是否達成
        foreach (var condtionData in conditionDetail)
        {
            switch (condtionData.Key)
            {
                default:
                //裝備指定類型道具
                case "Equip":
                    switch (condtionData.Value.ToString())
                    {
                        case "GiantMallet":
                        case "TwoHandedSword":
                        case "Shield":
                        case "Axe":
                            finalResult.Add(ItemManager.Instance.EquipDataList.Where(x => x.EquipmentDatas.Weapon != null).Any(x => x.EquipmentDatas.Weapon.TypeID == condtionData.Value.ToString()));
                            break;
                        case "HeavyArmor":
                            finalResult.Add(ItemManager.Instance.EquipDataList.Where(x => x.EquipmentDatas.Armor != null).All(x => x.EquipmentDatas.Armor.TypeID == condtionData.Value.ToString()));
                            break;
                    }
                    break;
                //在戰鬥狀態中
                case "InCombatStatus":
                    //缺少戰鬥狀態判斷
                    return false;
                //HP低於指定百分比
                //case "HpLess":
                //    float conditionHP = PlayerData.MaxHP * float.Parse(value);
                //    return conditionHP < PlayerData.HP;
                //HP低於指定百分比
                //case "HpMore":
                //    conditionHP = PlayerData.MaxHP * float.Parse(value);
                //    return conditionHP > PlayerData.HP;
                case "Close":
                    //建立靠近單位的判斷(朝單位移動? 雙方距離縮短? 單位判斷與距離多少?)
                    return false;
                case "Random":
                    //缺乏隨機條件(目前有的資料 禁衛軍的"回擊好禮")
                    return false;
            }
        }
        return finalResult.All(x => x);
    }
}

/// <summary>
/// 傷害技能組件 基底
/// </summary>
public abstract class DamageComponent : SkillComponent
{
}

/// <summary>
/// Buff技能組件 基底
/// </summary>
public abstract class BuffComponent : SkillComponent
{
    public abstract void ReverseExecute(params SkillOperationData[] skillOperationData);

    /// <summary>
    /// 玩家狀態效果提示物件 移除處理
    /// </summary>
    /// <param name="skillOperationDatas"></param>
    protected void CharacterStatusRemove(params SkillOperationData[] skillOperationDatas)
    {
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent?.Invoke(skillOperationDatas.ToArray());
    }

    /// <summary>
    /// 玩家狀態效果提示物件 增加除處理
    /// </summary>
    /// <param name="skillOperationDatas"></param>
    protected void CharacterStatusAdd(params SkillOperationData[] skillOperationDatas)
    {
        CharacterStatusManager.Instance.CharacterSatusAddEvent?.Invoke(this, skillOperationDatas.ToArray());
    }
}

/// <summary>
/// 傷害技能組件
/// </summary>
public class DamageSkillComponent : DamageComponent
{
    public DamageSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {
        if (SelectTarget.Instance.Targetgameobject != null/* && !SkillBeUpgrade*/)
        {
            Skill_Base_Attack skill_Base_Attack = skillbase as Skill_Base_Attack;
            BattleOperation.Instance.SkillAttackEvent?.Invoke(this, caster, target);
            PlayerDataOverView.Instance.CharacterMove.CharacterAnimator.SetTrigger(skillbase.SkillID);
        }
    }
}

/// <summary>
/// 多段傷害演出技能組件
/// </summary>
public class MultipleDamageSkillComponent : DamageComponent
{
    public MultipleDamageSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {
        if (SelectTarget.Instance.Targetgameobject != null/* && !SkillBeUpgrade*/)
        {
            Skill_Base_Attack skill_Base_Attack = skillbase as Skill_Base_Attack;
            BattleOperation.Instance.SkillAttackEvent?.Invoke(this, caster, target);
        }
    }
}

/// <summary>
/// 附帶屬性的傷害技能組件
/// </summary>
public class ElementtDamageSkillComponent : DamageComponent
{
    public ElementtDamageSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {
    }
}

/// <summary>
/// 持續傷害技能組件
/// </summary>
public class DotDamageSkill : DamageComponent
{
    public DotDamageSkill(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {

    }
}

/// <summary>
/// 控制狀態技能組件
/// </summary>
public class CrowdControlSkillComponent : SkillComponent
{

    public CrowdControlSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {

    }
}

/// <summary>
/// 按壓 引導型 技能組件
/// </summary>
public class ChanneledSkillComponent : SkillComponent
{
    public ChanneledSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {

    }
}

/// <summary>
/// 功能型技能組件
/// </summary>
public class UtilitySkillComponent : SkillComponent
{
    public UtilitySkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {

    }
}

/// <summary>
/// 位移 順移技能組件
/// </summary>
public class TeleportationSkillComponent : SkillComponent
{
    public TeleportationSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {

    }
}

/// <summary>
/// 位移 撲擊技能組件
/// </summary>
public class LungeSkillComponent : SkillComponent
{
    public LungeSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {

    }
}

/// <summary>
/// 位移 突進技能組件
/// </summary>
public class ChargeSkillComponent : SkillComponent
{
    public ChargeSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {

    }
}

/// <summary>
/// 升級指定技能組件
/// </summary>
public class UpgradeSkillComponent : BuffComponent
{
    private string targetSkillID;
    public UpgradeSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
        targetSkillID = skillOperationData.Bonus.ToString();
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {
        //尋找場景上所有SkillUI
        var findSkillUIResult = PassiveSkillManager.Instance.SkillUIList;
        if (findSkillUIResult.CheckAnyData())
        {
            var getSkillUIListOnScene = findSkillUIResult.Where(x => x.SkillID == targetSkillID).ToList();
            foreach (var skillUIData in getSkillUIListOnScene)
            {
                skillUIData.SkillBeUpgrade = true;
                skillUIData.SkillUpgradeID = skillbase.SkillID;
                skillUIData.SkillUpgradeIcon = CommonFunction.LoadSkillIcon(skillbase.SkillID);
            }
        }

        //更新快捷鍵上的技能圖片

        //獲取快捷鍵資料
        List<HotKeyData> hotKeyDataList = SkillController.Instance.SkillHotKey.ToList();
        //收尋快捷鍵上資料ID 找尋升級的指定技能ID
        HotKeyData item = hotKeyDataList.Find(x => x.TempHotKeyData != null && x.TempHotKeyData.KeyID == targetSkillID);
        if (item != null)
        {
            //更換圖片與更新升級技能ID資訊
            item.UpgradeSkillHotkeyDataProceoose(CommonFunction.LoadSkillIcon(skillbase.SkillID));
            item.UpgradeSkillID = skillbase.SkillID;
        }
        CharacterStatusAdd(skillOperationData);
    }

    public override void ReverseExecute(params SkillOperationData[] skillOperationDatas)
    {
        //尋找場景上所有SkillUI
        var findSkillUIResult = PassiveSkillManager.Instance.SkillUIList;
        if (findSkillUIResult.CheckAnyData())
        {
            var getSkillUIListOnScene = findSkillUIResult.Where(x => x.SkillID == targetSkillID).ToList();
            foreach (var skillUIData in getSkillUIListOnScene)
            {
                skillUIData.SkillBeUpgrade = false;
                skillUIData.SkillUpgradeID = "";
                skillUIData.SkillUpgradeIcon = CommonFunction.LoadSkillIcon(targetSkillID);
            }
        }

        //更新快捷鍵上的技能圖片

        //獲取快捷鍵資料
        List<HotKeyData> hotKeyDataList = SkillController.Instance.SkillHotKey.ToList();
        //收尋快捷鍵上資料ID 找尋升級的指定技能ID
        HotKeyData item = hotKeyDataList.Find(x => x.TempHotKeyData != null && x.TempHotKeyData.KeyID == targetSkillID);
        if (item != null)
        {
            //更換圖片與更新升級技能ID資訊
            item.UpgradeSkillHotkeyDataProceoose(CommonFunction.LoadSkillIcon(targetSkillID));
            item.UpgradeSkillID = "";
        }
        CharacterStatusRemove(skillOperationData);
    }
}

/// <summary>
/// 強化指定技能組件
/// </summary>
public class EnhanceSkillComponent : BuffComponent
{
    public EnhanceSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {

    }

    public override void ReverseExecute(params SkillOperationData[] skillOperationData)
    {
    }
}

/// <summary>
/// 傷害技能組件 但作用的技能效果繼承自別的技能
/// </summary>
public class InheritDamegeSkillComponent : SkillComponent
{
    private DamageSkillComponent skillComponent;
    public InheritDamegeSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {
        skillComponent.Execute(caster, target);
    }
}

/// <summary>
/// 被動技能組件
/// </summary>
public class PassiveBuffSkillComponent : BuffComponent
{
    public PassiveBuffSkillComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }
    private ICombatant tempCaster;
    private ICombatant tempTarget;
    public override void Execute(ICombatant caster, ICombatant target)
    {
        tempCaster = caster;
        tempTarget = target;
        //取得Buff技能詳細資料
        var buffData = skillbase.SkillData.SkillOperationDataList;
        //以相同的組件 與 相同條件分組
        var group = buffData.GroupBy(x => new
        {
            x.SkillComponentID,
            ConditionORKey = x.ConditionOR.CheckAnyData() ? string.Join(",", x.ConditionOR.OrderBy(c => c)) : null,
            ConditionANDKey = x.ConditionAND.CheckAnyData() ? string.Join(",", x.ConditionAND.OrderBy(c => c)) : null
        });
        foreach (var groupData in group)
        {
            CharacterStatusAdd(groupData.ToArray());

            foreach (var data in groupData)
            {
                tempCaster.GetBuffEffect(tempTarget, data);
            }
        }
    }
    public override void ReverseExecute(params SkillOperationData[] skillOperationData)
    {
        for (int i = 0; i < skillOperationData.Length; i++)
        {
            tempCaster.RemoveBuffEffect(tempTarget, skillOperationData[i]);
        }
    }
}

/// <summary>
/// 主動Buff技能組件
/// </summary>
public class ContinuanceBuffComponent : BuffComponent
{
    public ContinuanceBuffComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    private ICombatant tempCaster;
    private ICombatant tempTarget;

    public override void Execute(ICombatant caster, ICombatant target)
    {
        tempCaster = caster;
        tempTarget = target;
        //取得Buff技能詳細資料
        var buffData = skillbase.SkillData.SkillOperationDataList;
        //以相同的組件 與 持續時間分組
        var group = buffData.GroupBy(x => new
        {
            x.SkillComponentID,
            ConditionORKey = x.ConditionOR.CheckAnyData() ? string.Join(",", x.ConditionOR.OrderBy(c => c)) : null,
            ConditionANDKey = x.ConditionAND.CheckAnyData() ? string.Join(",", x.ConditionAND.OrderBy(c => c)) : null
        });
        foreach (var groupData in group)
        {
            CharacterStatusAdd(groupData.ToArray());

            foreach (var data in groupData)
            {
                tempCaster.GetBuffEffect(tempTarget, data);
            }
        }
    }

    public override void ReverseExecute(params SkillOperationData[] skillOperationData)
    {
        for (int i = 0; i < skillOperationData.Length; i++)
        {
            tempCaster.RemoveBuffEffect(tempTarget, skillOperationData[i]);
        }
    }
}

/// <summary>
/// 疊加型效果技能組件
/// </summary>
public class AdditiveBuffComponent : BuffComponent
{
    public AdditiveBuffComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    private ICombatant tempCaster;
    private ICombatant tempTarget;

    public override void Execute(ICombatant caster, ICombatant target)
    {
        tempCaster = caster;
        tempTarget = target;
        //取得Buff技能詳細資料
        var buffData = skillbase.SkillData.SkillOperationDataList;
        //以相同的組件 與 持續時間分組
        var group = buffData.GroupBy(x => new { x.SkillComponentID, x.EffectDurationTime });
        foreach (var groupData in group)
        {
            foreach (var data in groupData)
            {
                tempCaster.GetBuffEffect(tempTarget, data);
            }
        }
    }

    public override void ReverseExecute(params SkillOperationData[] skillOperationData)
    {
    }
}

/// <summary>
/// 負面Buff技能組件
/// </summary>
public class DebuffComponent : BuffComponent
{
    public DebuffComponent(Skill_Base skill_Base, SkillOperationData operationData)
    {
        skillbase = skill_Base;
        skillOperationData = operationData;
    }

    private ICombatant tempCaster;
    private ICombatant tempTarget;

    public override void Execute(ICombatant caster, ICombatant target)
    {
        tempCaster = caster;
        tempTarget = target;
        //取得Buff技能詳細資料
        var buffData = skillbase.SkillData.SkillOperationDataList;
        //以相同的組件 與 持續時間分組
        var group = buffData.GroupBy(x => new { x.SkillComponentID, x.EffectDurationTime });
        foreach (var groupData in group)
        {
            foreach (var data in groupData)
            {
                tempCaster.GetBuffEffect(tempTarget, data);
            }
        }
    }

    public override void ReverseExecute(params SkillOperationData[] skillOperationData)
    {
    }
}