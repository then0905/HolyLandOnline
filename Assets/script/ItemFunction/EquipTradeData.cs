using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2024/06/24
//  創建用途: 背包格物品資料 交易版
//==========================================
public class EquipTradeData : Equipment
{
    [Header("選擇框素材"), SerializeField] private Image selectFrame;
    private void OnEnable()
    {
        //訂閱事件
        TradeManager.Instance.SelectItemEvent += OnSelect;
    }

    private void OnDisable()
    {
        //反訂閱事件
        TradeManager.Instance.SelectItemEvent -= OnSelect;
    }

    /// <summary>
    /// 選取物品
    /// </summary>
    public void OnSelect(object o, EquipTradeData equipTradeData)
    {
        if (equipTradeData == this)
        {
            TradeManager tradeManager = TradeManager.Instance;
            tradeManager.ButtonSetting(this.EquipmentType);
            tradeManager.TempSelectItem = this;
            if (selectFrame)
                selectFrame.enabled = true;
        }
        else
        {
            if (selectFrame)
                selectFrame.enabled = false;
        }
    }
}
