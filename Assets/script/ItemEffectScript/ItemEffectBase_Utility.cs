using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2025/02/06
//  創建用途:道具使用效果基底_功能型系列
//==========================================
public class ItemEffectBase_Utility : ItemEffectBase
{
    public override void InitItemEffectData()
    {
        base.InitItemEffectData();

        //功能型資料 根據需求將執行內容帶入組件的事件
        for (int i = 0; i < ItemComponentList.Count; i++)
        {
            if (ItemComponentList[i] is UtilityItemComponent utilityItemComponent)
            {
                switch (ItemData.ItemEffectDataList[i].InfluenceStatus)
                {
                    case "RemovePoison":
                        utilityItemComponent.UtilityAct += RemovePoison;
                        break;

                    case "Record":
                        utilityItemComponent.UtilityAct += Record;
                        break;
                }
            }
        }
    }

    protected override void ItemEffectStart(ICombatant caster = null, ICombatant receiver = null)
    {
        //檢查資料的空值
        if (ItemComponentList.CheckAnyData())
            //效果運行
            ItemComponentList[0].Execute(caster, receiver);

        //執行冷卻時間判斷、扣除背包道具內容
        base.ItemEffectStart(caster, receiver);
    }

    protected override void ItemEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {

    }

    #region 委派的功能內容區塊

    /// <summary>
    /// 回城
    /// </summary>
    /// <param name="caster">施放者</param>
    /// <param name="target">被施放者</param>
    private void Record(ICombatant caster, ICombatant target)
    {
        //進行回城處理
        MapManager.Instance?.RecordProcessor(target);
    }

    /// <summary>
    /// 移除中毒
    /// </summary>
    /// <param name="caster">施放者</param>
    /// <param name="target">被施放者</param>
    private void RemovePoison(ICombatant caster, ICombatant target)
    {
        //找出目標身上正在運行的debuff
        //是否有中毒
    }

    #endregion
}
