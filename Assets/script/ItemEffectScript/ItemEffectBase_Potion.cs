using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/11/25
//  創建用途:道具使用效果基底_回復藥水系列
//==========================================
public class ItemEffectBase_Potion : ItemEffectBase
{
    protected override void ItemEffectStart(ICombatant caster = null, ICombatant receiver = null)
    {
        var tempData = ItemData.ItemEffectDataList[0];

        if (ItemComponentList.CheckAnyData())
            ItemComponentList.ForEach(x => BattleOperation.Instance.ItemRestorationEvent?.Invoke(x as ItemComponent, caster, receiver));
       

        if (!CooldownTime.Equals(0))
            StartCoroutine(UpdateCooldown(CooldownTime));
        UseItemQtyProcessor();
    }
    protected override void ItemEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {

    }
}
