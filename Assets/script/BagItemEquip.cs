using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JsonDataModel;
//==========================================
//  創建者:    家豪
//  翻修日期:  2023/06/07
//  創建用途:  背包物品的 裝備與拖曳
//==========================================
public class BagItemEquip : MonoBehaviour
{
    //生成預製物
    public GameObject CloneItem;
    //背包父級
    public Transform BagWindowTransform;
    //空背包圖
    public Sprite BagItemOriginImage;
    //儲存生成物件
    private GameObject cloneItem;
    //紀錄原始位置
    public GameObject OriginItemSeat;
    //是否已生成物品物件
    private bool cloneItemImage;
    //
    public GameObject[] SetHints = new GameObject[7];

    //物品資訊腳本
    public BagsItemIntro Bagsitemintro;

    #region 觸發事件

    /// <summary>
    /// 點擊事件
    /// </summary>
    /// <param name="data"></param>
    public void OnClick(BaseEventData baseEventData)
    {
        PointerEventData data = baseEventData as PointerEventData;
        Bagsitemintro.OnClick(BagWindowTransform.GetComponent<RectTransform>(),data);
    }

    /// <summary>
    /// 開始拖曳
    /// </summary>
    /// <param name="data"></param>
    public void BeginDrag(BaseEventData baseEventData)
    {
        PointerEventData data = baseEventData as PointerEventData;

        GameObject thisItem = data.pointerCurrentRaycast.gameObject;
        //檢查空值
        if (thisItem.GetComponent<Equipment>().EquipmentDatas != null)
        {
            OriginItemSeat = thisItem;
            if (!cloneItemImage)
            {
                //生成物件
                cloneItemImage = true;  //設定防呆 防止重複生成
                cloneItem = Instantiate(CloneItem, transform.position, transform.rotation, BagWindowTransform);
                //設定生成物件的圖片與資料
                cloneItem.transform.GetComponent<Equipment>().EquipImage.sprite = thisItem.GetComponent<Equipment>().EquipImage.sprite;
                cloneItem.transform.GetComponent<Equipment>().EquipmentDatas = thisItem.GetComponent<Equipment>().EquipmentDatas;
                //背包原位的格子圖片設定為空背包圖
                thisItem.GetComponent<Equipment>().EquipImage.sprite = BagItemOriginImage;
            }
        }
        //關閉物品介紹
        Bagsitemintro.gameObject.SetActive(false);
        Bagsitemintro.IntroItem = null;
    }

    /// <summary>
    /// 拖曳中
    /// </summary>
    /// <param name="data"></param>
    public void Dragging(BaseEventData baseEventData)
    {
        PointerEventData data = baseEventData as PointerEventData;

        GameObject MovingItem = data.pointerCurrentRaycast.gameObject;

        //複製的物件跟隨鼠標
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(BagWindowTransform.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out pos);

        cloneItem.GetComponent<RectTransform>().anchoredPosition = pos;

        //關閉物件raycast
        if (cloneItem.transform.GetComponent<Image>().raycastTarget)
            cloneItem.transform.GetComponent<Image>().raycastTarget = false;

        //獲取複製物件的裝備資料
        Equipment cloneObjEquipment = cloneItem.transform.GetComponent<Equipment>();
        //獲取裝備欄腳本
        EquipData equipData = MovingItem.transform.GetComponent<EquipData>();

        //檢查空值
        if (MovingItem.tag == "Equip" && MovingItem != null)//拖曳物品到裝備欄時
        {
            if (cloneObjEquipment.EquipmentDatas.Item == null)//檢查物品是否為道具類
                return;
            //檢查是否正確位置
            if (equipData.PartID.Contains(cloneItem.transform.GetComponent<Equipment>().EquipmentDatas.Weapon.TackHandID) && equipData != null)
                //顯示裝備欄綠色提示
                HintMethod(equipData.Hint, true, Color.green);
            else if (MovingItem.transform.GetComponent<EquipData>().PartID.Contains(cloneItem.transform.GetComponent<Equipment>().EquipmentDatas.Armor.WearPartID))
                //顯示裝備欄綠色提示
                HintMethod(equipData.Hint, true, Color.green);
            else
                //顯示裝備欄紅色提示
                HintMethod(equipData.Hint, false, Color.red);
        }
        else
            //離開裝備欄 隱藏裝備欄提示物件
            HintMethod(equipData.Hint, false, Color.red);
    }

    /// <summary>
    /// 放開拖曳
    /// </summary>
    /// <param name="data"></param>
    public void EndDrag(BaseEventData baseEventData)
    {
        PointerEventData data = baseEventData as PointerEventData;

        //獲取放開拖曳後 滑鼠座標的物件
        GameObject MovingItem = data.pointerCurrentRaycast.gameObject;
        //檢查空值
        if (MovingItem == null)
        {
            //還原 原本位置的圖片
            OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite = cloneItem.GetComponent<Equipment>().EquipImage.sprite;
            OriginItemSeat.GetComponent<Equipment>().EquipmentDatas = cloneItem.GetComponent<Equipment>().EquipmentDatas;
            ReductionDrag();
            return;
        }

        //若是裝備格
        if (MovingItem.tag == "Equip")
        {
            if (MovingItem.GetComponent<EquipData>().PartID.Contains(cloneItem.GetComponent<Equipment>().EquipmentDatas.Weapon.TackHandID))
            {
                print("裝備武器");
                PutOnEquip(MovingItem);
            }
            else if (MovingItem.GetComponent<EquipData>().PartID.Contains(cloneItem.GetComponent<Equipment>().EquipmentDatas.Armor.WearPartID))
            {
                print("裝備防具");
                PutOnEquip(MovingItem);
            }
        }
        //若是背包格
        else if (MovingItem.tag == "BagItem")
        {
            ChangeSeat(MovingItem);
        }
        //都不是則回原位子
        else
        {
            OriginItemSeat.GetComponent<Image>().sprite = cloneItem.GetComponent<Image>().sprite;
            ReductionDrag();
        }
    }
    #endregion

    /// <summary>
    /// 設定裝備欄提示狀態
    /// </summary>
    /// <param name="hint">提示物件</param>
    /// <param name="isEnable">顯示狀態</param>
    /// <param name="color">顏色</param>
    public void HintMethod(GameObject hint, bool isEnable, Color color)
    {
        hint.SetActive(isEnable);
        hint.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// 背包格內物品位置互換
    /// </summary>
    /// <param name="BagSeat">交換位置的目標物件</param>
    public void ChangeSeat(GameObject BagSeat)
    {
        //原位置的物品資料與目標物品資料交換
        CommonFunction.ChangeSameComponent(OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite, BagSeat.GetComponent<Equipment>().EquipImage.sprite);
        CommonFunction.ChangeSameComponent(OriginItemSeat.GetComponent<Equipment>().EquipmentDatas, BagSeat.GetComponent<Equipment>().EquipmentDatas);
        //OriginItemSeat.GetComponent<Image>().sprite = BagSeat.GetComponent<Image>().sprite;
        //OriginItemSeat.GetComponent<Equipment>().EquipImage = OriginItemSeat.GetComponent<Image>();
        //OriginItemSeat.GetComponent<Equipment>().EquipmentDatas = BagSeat.GetComponent<Equipment>().EquipmentDatas;

        //BagSeat.GetComponent<Image>().sprite = cloneItem.transform.GetChild(0).GetComponent<Image>().sprite;
        //BagSeat.GetComponent<Equipment>().EquipImage = BagSeat.GetComponent<Image>();
        //BagSeat.GetComponent<Equipment>().EquipmentDatas = cloneItem.transform.GetChild(0).GetComponent<Equipment>().EquipmentDatas;
        ReductionDrag();
    }

    /// <summary>
    /// 穿上裝備或是武器
    /// </summary>
    /// <param name="Equip">目標格子</param>
    public void PutOnEquip(GameObject Equip)
    {
        //檢測目標格子是否已有穿戴物件(武器或防具)
        if (Equip.GetComponent<Equipment>().EquipmentDatas != null)
        {
            print("已有裝備武器進行更換");
            CommonFunction.ChangeSameComponent(OriginItemSeat.GetComponent<Equipment>().EquipmentDatas, Equip.GetComponent<Equipment>().EquipmentDatas);
            CommonFunction.ChangeSameComponent(OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite, Equip.GetComponent<Equipment>().EquipImage.sprite);

            //OriginItemSeat.GetComponent<Equipment>().EquipmentDatas = Equip.GetComponent<Equipment>().EquipmentDatas;
            //Equip.GetComponent<Equipment>().EquipmentDatas = cloneItem.GetComponent<Equipment>().EquipmentDatas;
            //OriginItemSeat.GetComponent<Image>().sprite = Equip.GetComponent<Image>().sprite;
            //OriginItemSeat.GetComponent<Equipment>().EquipImage = OriginItemSeat.GetComponent<Image>();
            //Equip.GetComponent<Image>().sprite = cloneItem.GetComponent<Image>().sprite;
            //Equip.GetComponent<Equipment>().EquipImage = Equip.GetComponent<Image>();
        }
        //若沒有裝備 直接穿上
        else
        {
            print("無裝備");
            //設定裝備資料
            Equip.GetComponent<Equipment>().EquipmentDatas = cloneItem.GetComponent<Equipment>().EquipmentDatas;

            //OriginItemSeat.GetComponent<Image>().sprite = BagItemOriginImage;
            //還原原始格子
            OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite = BagItemOriginImage;
            OriginItemSeat.GetComponent<Equipment>().EquipmentDatas = null;

            //設定目標裝備欄圖片
            Equip.GetComponent<Equipment>().EquipImage.sprite = cloneItem.GetComponent<Equipment>().EquipImage.sprite;

        }
        //刷新能力值數據
        StatusOperation.Instance.StatusMethod();
        ReductionDrag();
    }

    /// <summary>
    /// 清空物件並刪除拖曳的複製物件
    /// </summary>
    public void ReductionDrag()
    {
        //還原裝備欄提示圖片
        for (int i = 0; i < SetHints.Length; i++)
        {
            SetHints[i].gameObject.SetActive(false);
        }
        Destroy(cloneItem);
        //還原防呆
        cloneItemImage = false;
    }
}
