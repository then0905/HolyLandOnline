using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2025/03/11
//  創建用途: 道具數量操作相關視窗設定
//==========================================
public class ItemQtyControlSetting : CommonInquiryWindowSetting
{
    [SerializeField] protected TMP_Text itemName;
    [SerializeField] protected TMP_Text itemQty;
    [SerializeField] protected Image itemImage;

    [Header("遊戲資料")]
    protected Equipment equipment;
    protected int tempQty;      //暫存選擇的數量資料

    /// <summary>
    /// 初始化設定
    /// </summary>
    /// <param name="contentStr">提示框文字</param>
    /// <param name="button1">按鈕1執行內容(但已包含設定丟棄物品數量)</param>
    /// <param name="button2">按鈕2執行內容</param>
    public void Init(Equipment data, string titleStr, string contentStr, Action button1, Action button2)
    {
        //設定全域資料
        equipment = data;
        //設定物品名稱
        itemName.text = equipment.EquipmentDatas.ItemCommonData.Name.GetText();
        //設定物品圖示
        itemImage.sprite = CommonFunction.GetItemSprite(equipment.EquipmentDatas);
        //初始物品數量
        tempQty = 1;
        itemQty.text = tempQty.ToString();
        //設定標題
        titleText.text = titleStr;
        //設定內容
        contentText.text = contentStr;
        //設定確認按鈕
        yesButton.onClick.AddListener(() =>
        {
            button1?.Invoke();
            BootysHandle.Instance.DropSpecifiedItem(equipment.EquipmentDatas.ItemCommonData.CodeID, tempQty);
            BagManager.Instance.RemoveItem(equipment.EquipmentDatas.ItemCommonData.CodeID, tempQty);
            DestroyWindow();
        });
        //設定關閉按鈕
        cancelButton.onClick.AddListener(() =>
        {
            button2?.Invoke();
            DestroyWindow();
        });
    }

    /// <summary>
    /// 增加數量
    /// </summary>
    public void AddQty()
    {
        if(tempQty +1 <= equipment.EquipmentDatas.Qty)
        {
            tempQty += 1;
            itemQty.text = tempQty.ToString();
        }   
    }

    /// <summary>
    /// 減少數量
    /// </summary>
    public void ReduceQty()
    {
        if (tempQty - 1 >= 0)
        {
            tempQty -= 1;
            itemQty.text = tempQty.ToString();
        }
    }
}
