using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;

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
    private ICombatant tempCaster;
    private ICombatant tempTarget;
    public override void Execute(ICombatant caster, ICombatant target)
    {
        tempCaster = caster;
        tempTarget = target;
        DebuffEffectBase_CrowdControl tempObj = UnityEngine.Object.Instantiate(CommonFunction.LoadObject<GameObject>(GameConfig.EffectPrefab, $"Effect_{skillOperationData.InfluenceStatus}")).GetComponent<DebuffEffectBase_CrowdControl>();
        Debug.Log($"被施放效果 {skillOperationData.InfluenceStatus} 施放者 {target.Obj.name} 被施放者 {caster.Obj.name}");
        tempObj.EffectStart(caster, target, this);
    }

    /// <summary>
    /// 移除效果處理
    /// </summary>
    /// <param name="debuffEffectBase"></param>
    public void ReverseExecute(DebuffEffectBase debuffEffectBase)
    {
        tempTarget.RemoveDebuff(debuffEffectBase);
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
        LungeMove((ActivityCharacterBase)caster, (ActivityCharacterBase)target);
    }

    /// <summary>
    /// 撲擊移動
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    private async void LungeMove(ActivityCharacterBase caster, ActivityCharacterBase target)
    {
        float jumpTime;
        //取得跳躍時間
        if (float.TryParse(SkillOperationData.Bonus.ToString(), out jumpTime))
        {
            float elapsedTime = 0f;
            //取得施法者位置
            Vector3 targetPosition = target.Povit.transform.position;
            //取得面相目標方向
            Vector3 direction = (targetPosition - caster.Povit.transform.position).normalized;
            //取得與目標相隔4單位的座標
            Vector3 destinationPosition = targetPosition - (direction * 4f);
            //取得與目標差4單位座標的距離
            float distance = Vector3.Distance(caster.Povit.transform.position, destinationPosition);
            float speed = distance / jumpTime;

            while (elapsedTime < jumpTime)
            {
                caster.Obj.transform.position = Vector3.Lerp(caster.Povit.transform.position, destinationPosition, 0.033f);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            // Ensure the caster reaches the exact destination position
            caster.Obj.transform.position = destinationPosition;
        }
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
            Skill_Base targetSkillbase = (Skill_Base)item.TempHotKeyData;
            targetSkillbase.InitSkillEffectData(item.UpgradeSkillID);
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