using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/11/25
//  創建用途:道具使用效果基底(快捷鍵上使用)
//==========================================

/// <summary>
/// 「道具」的效果接口
/// </summary>
public interface IItemEffect
{
    /// <summary>
    /// 道具ID
    /// </summary>
    string ItemID { get; }

    /// <summary>
    /// 道具名稱
    /// </summary>
    string ItemName { get; }

    /// <summary>
    /// 道具效果描述
    /// </summary>
    string ItemIntro { get; }

    /// <summary>
    /// 冷卻時間
    /// </summary>
    float CooldownTime { get; }
    /// <summary>
    /// 道具資料
    /// </summary>
    ItemDataModel ItemData { get; set; }

    /// <summary>
    /// 道具施放 效果實現
    /// </summary>
    /// <param name="caster">施放者</param>
    /// <param name="target">被施放者</param>
    void ItemEffect(ICombatant caster, ICombatant target);

    /// <summary>
    /// 暫存已進入的冷卻時間
    /// </summary>
    float TempCooldownTime { get; }

    /// <summary>
    /// 更新冷卻時間
    /// </summary>
    /// <param name="deltaTime">冷卻時間</param>
    IEnumerator UpdateCooldown(float deltaTime);
}

/// <summary>
/// 道具組件接口
/// </summary>
public interface IItemComponent
{
    /// <summary>
    /// 道具效果實現
    /// </summary>
    /// <param name="caster">施放者</param>
    /// <param name="target">被施放者</param>
    void Execute(ICombatant caster, ICombatant target);
}

public abstract class ItemEffectBase : MonoBehaviour, IItemEffect, IHotKey
{
    public string ItemID { get { return itemID; } }
    public string KeyID => ItemID;

    public ItemDataModel ItemData { get; set; }

    public string ItemName { get; set; }

    public string ItemIntro { get; set; }

    public float CooldownTime { get; set; }

    public float TempCooldownTime { get; set; }

    public List<IItemComponent> ItemComponentList { get; set; } = new List<IItemComponent>();

    public virtual bool ItemEffectCanUse()
    {
        bool qtyBool = BagManager.Instance.GetItemQtyFromBag(ItemID) > 0;
        bool CooldownBool = TempCooldownTime <= 0;
        //背包物品數量不足
        if (!qtyBool)
            CommonFunction.MessageHint(string.Format("TM_ItemEffectQtyError".GetText(), ItemName), HintType.Warning);
        //是否正在冷卻時間
        else if (TempCooldownTime > 0)
            CommonFunction.MessageHint(string.Format("TM_ItemEffectCoolDownError".GetText(), ItemName), HintType.Warning);

        return qtyBool& CooldownBool;
    }

    [Header("道具ID 用來從GameData找資料輸入"), SerializeField] protected string itemID;

    [Header("生成的動畫特效物件"), SerializeField] protected GameObject effectObj;

    /// <summary>
    /// 初始化技能資料
    /// </summary>
    /// <param name="skillUpgradeID">是否升級技能</param>
    /// <param name="caster">施放者</param>
    /// <param name="receiver">接收者</param>
    public virtual void InitItemEffectData(ICombatant caster = null, ICombatant receiver = null)
    {
        //獲取GameData技能資料
        ItemData = GameData.ItemsDic[itemID];

        CooldownTime = ItemData.CD;

        ItemName = ItemData.Name.GetText();
        ItemIntro = ItemData.Intro.GetText();

        ItemData.ItemEffectDataList.ForEach(x => ItemComponentList.Add(ItemComponent(x)));

        //設定生成特效參考
        if (effectObj != null) InitItemEffect();

        //是否需要判斷條件 需要的話 訂閱事件
        //if (UseSkillCheck)
        //    SkillController.Instance.SkillConditionCheckEvent += SkillBuffSub;
        //else
        //    //不需要判斷條件 直接允許技能使用
        //    skillCondtionCheck = true;
    }

    public void ItemEffect(ICombatant caster, ICombatant target)
    {
        if (!CooldownTime.Equals(0))
            StartCoroutine(UpdateCooldown(CooldownTime));
        ItemEffectStart(caster, target);
    }

    public IEnumerator UpdateCooldown(float deltaTime)
    {
        //暫存冷卻時間
        TempCooldownTime = deltaTime;
        while (TempCooldownTime > 0)
        {
            TempCooldownTime -= 0.1f;
            //更新技能冷卻讀條
            //skillCdSlider.value = CooldownTime - TempCooldownTime;
            var hotkeyData = SkillController.Instance.SkillHotKey.Where(x => (x.TempHotKeyData is ItemEffectBase) && ((object)x.TempHotKeyData == this)).FirstOrDefault();
            if (hotkeyData == null)
            {
                Debug.Log("快捷鍵上沒有正在施放的道具 :" + ItemName);
                yield break;
            }
            hotkeyData.HotKeyCdProcessor(CooldownTime - TempCooldownTime);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 實例化技能特效物件
    /// </summary>
    /// <param name="targetReference">生成目標參考</param>
    protected void InitItemEffect()
    {
        //GameObject obj;
        //switch (targetReference)
        //{
        //    default:
        //    case "Self":
        //        obj = Instantiate(effectObj, PlayerDataOverView.Instance.CharacterMove.Character.transform);
        //        break;
        //    case "Target":
        //        obj = Instantiate(effectObj, SelectTarget.Instance.Targetgameobject.transform);
        //        break;
        //        //case "TargetArea":
        //        //case "Team":
        //}
        //obj.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 道具施放開始
    /// </summary>
    /// <param name="caster">施放者</param>
    /// <param name="receiver">被施放者</param>
    protected virtual void ItemEffectStart(ICombatant caster = null, ICombatant receiver = null)
    {
    }

    /// <summary>
    /// 道具施放結束
    /// </summary>
    protected abstract void ItemEffectEnd(ICombatant caster = null, ICombatant receiver = null);

    /// <summary>
    /// 使用物品處理背包內的數量
    /// </summary>
    /// <param name="qty">使用數量 預設為1</param>
    protected void UseItemQtyProcessor(int qty = 1)
    {
        BagManager.Instance.RemoveItem(ItemID, qty);
    }


    public IItemComponent ItemComponent(ItemEffectData itemComponent)
    {
        switch (itemComponent.ItemComponentID)
        {
            case "Restoration":
                ItemRestorationComponent x = new ItemRestorationComponent(this, itemComponent);
                return x;

            default:
                return null;
        }
    }
}
