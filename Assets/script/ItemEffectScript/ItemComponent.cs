using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/12/03
//  創建用途:道具組件接口繼承實例
//==========================================
public abstract class ItemComponent : IItemComponent
{
    protected ItemEffectBase itemBase;
    public ItemEffectBase ItemBase => itemBase;
    protected ItemEffectData itemEffectData;
    public ItemEffectData ItemEffectData => itemEffectData;

    public abstract void Execute(ICombatant caster, ICombatant target);
}

public class ItemRestorationComponent : ItemComponent
{
    public ItemRestorationComponent(ItemEffectBase item_Base, ItemEffectData itemEffectData)
    {
        itemBase = item_Base;
        this.itemEffectData = itemEffectData;
    }
    public override void Execute(ICombatant caster, ICombatant target)
    {
        BattleOperation.Instance.ItemRestorationEvent?.Invoke(this,caster,target);
    }
}