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
        //取得道具的使用效果 運算資料
        var tempData = ItemData.ItemEffectDataList[0];

        //檢查資料的空值
        if (ItemComponentList.CheckAnyData())
            ItemComponentList.ForEach(x => BattleOperation.Instance.ItemRestorationEvent?.Invoke(x as ItemComponent, caster, receiver));

        //執行冷卻時間判斷、扣除背包道具內容
        base.ItemEffectStart(caster, receiver);
    }

    protected override void ItemEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {

    }
}
