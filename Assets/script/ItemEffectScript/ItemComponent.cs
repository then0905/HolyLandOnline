using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/12/03
//  創建用途:道具組件接口繼承實例
//==========================================
public abstract class ItemComponent : BaseComponent, IItemComponent
{
    protected ItemEffectBase itemBase;
    public ItemEffectBase ItemBase => itemBase;
    protected ItemEffectData itemEffectData;
    public ItemEffectData ItemEffectData => itemEffectData;

    public abstract void Execute(ICombatant caster, ICombatant target);
}

/// <summary>
/// 回復型道具效果組件
/// </summary>
public class RestorationItemComponent : ItemComponent
{
    public RestorationItemComponent(ItemEffectBase item_Base, ItemEffectData itemEffectData)
    {
        itemBase = item_Base;
        this.itemEffectData = itemEffectData;
    }
    public override void Execute(ICombatant caster, ICombatant target)
    {
        BattleOperation.Instance.ItemRestorationEvent?.Invoke(this, caster, target);
    }
}

/// <summary>
/// 持續效果道具效果組件
/// </summary>
public class BuffItemComponent : ItemComponent
{
    public BuffItemComponent(ItemEffectBase item_Base, ItemEffectData itemEffectData)
    {
        itemBase = item_Base;
        this.itemEffectData = itemEffectData;
    }

    private ICombatant tempCaster;
    private ICombatant tempTarget;

    public override void Execute(ICombatant caster, ICombatant target)
    {
        tempCaster = caster;
        tempTarget = target;

        tempCaster.GetBuffEffect(tempTarget, itemEffectData);
        CharacterStatusAdd(itemEffectData);
    }

    public void ReverseExecute(params OperationData[] itemEffectDatas)
    {
        for (int i = 0; i < itemEffectDatas.Length; i++)
        {
            tempCaster.RemoveBuffEffect(tempTarget, itemEffectDatas[i]);
        }
    }

    /// <summary>
    /// 玩家狀態效果提示物件 增加處理
    /// </summary>
    /// <param name="itemEffectDatas"></param>
    protected void CharacterStatusAdd(params ItemEffectData[] itemEffectDatas)
    {
        CharacterStatusManager.Instance.CharacterSatusAddEvent?.Invoke(this, itemEffectDatas.ToArray());
    }

    /// <summary>
    /// 玩家狀態效果提示物件 移除處理
    /// </summary>
    /// <param name="itemEffectDatas"></param>
    protected void CharacterStatusRemove(params ItemEffectData[] itemEffectDatas)
    {
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent?.Invoke(itemEffectDatas.ToArray());
    }
}


/// <summary>
/// 功能型道具效果組件
/// </summary>
public class UtilityItemComponent : ItemComponent
{
    /// <summary>功能型道具執行內容事件</summary>
    public Action<ICombatant, ICombatant> UtilityAct { get; set; }
    public UtilityItemComponent(ItemEffectBase item_Base, ItemEffectData itemEffectData)
    {
        itemBase = item_Base;
        this.itemEffectData = itemEffectData;
    }

    public override void Execute(ICombatant caster, ICombatant target)
    {
        UtilityAct?.Invoke(caster,target);
    }
}