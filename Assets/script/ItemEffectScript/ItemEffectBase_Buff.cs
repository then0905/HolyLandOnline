using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2025/01/21
//  創建用途:道具使用效果基底_Buff系列
//==========================================
public class ItemEffectBase_Buff : ItemEffectBase
{
    protected override void ItemEffectStart(ICombatant caster = null, ICombatant receiver = null)
    {
        var tempData = ItemData.ItemEffectDataList[0];

        if (ItemComponentList.CheckAnyData())
            //ItemComponentList.ForEach(x => BattleOperation.Instance.ItemRestorationEvent?.Invoke(x as ItemComponent, caster, receiver));
            ItemComponentList[0].Execute(caster, receiver);

        if (!CooldownTime.Equals(0))
            StartCoroutine(UpdateCooldown(CooldownTime));

        UseItemQtyProcessor();
    }
    protected override void ItemEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {

    }
}
