
//==========================================
//  創建者:家豪
//  創建日期:2025/03/10
//  創建用途: 道具使用效果基底_無功用、不可使用系列
//==========================================
public class ItemEffectBase_Unavailable : ItemEffectBase
{
    protected override void ItemEffectEnd(ICombatant caster = null, ICombatant receiver = null)
    {

    }

    protected override void ItemEffectStart(ICombatant caster = null, ICombatant receiver = null)
    {
        //if (ItemComponentList.CheckAnyData())
        //    //效果運行
        //    ItemComponentList[0].Execute(caster, receiver);

        ////執行冷卻時間判斷、扣除背包道具內容
        //base.ItemEffectStart(caster, receiver);
    }
}
