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
        if (ItemComponentList.CheckAnyData())
            //效果運行
            ItemComponentList[0].Execute(caster, receiver);

        //執行冷卻時間判斷、扣除背包道具內容
        base.ItemEffectStart(caster, receiver);
    }
    protected override void ItemEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {
    }
}
