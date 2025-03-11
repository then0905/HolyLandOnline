using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/06/07
//  創建用途:  背包物品相關腳本:裝備與拖曳
//==========================================

public class BagItem : MonoBehaviour
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
        Bagsitemintro.OnClick(BagWindowTransform.GetComponent<RectTransform>(), data);
        //Bagsitemintro.BagItemOnClickEvent.Invoke(this, data);
    }

    /// <summary>
    /// 開始拖曳
    /// </summary>
    /// <param name="data"></param>
    public void BeginDrag(BaseEventData baseEventData)
    {
        PointerEventData data = baseEventData as PointerEventData;

        Equipment thisItem = data.pointerCurrentRaycast.gameObject.GetComponent<Equipment>();
        //檢查空值
        if ((thisItem.EquipmentDatas.Armor != null ||
            thisItem.EquipmentDatas.Weapon != null ||
            thisItem.EquipmentDatas.Item != null) && thisItem != null)
        {
            OriginItemSeat = thisItem.gameObject;
            if (!cloneItemImage)
            {
                //生成物件
                cloneItemImage = true;  //設定防呆 防止重複生成
                cloneItem = Instantiate(CloneItem, transform.position, transform.rotation, BagWindowTransform);
                //設定生成物件的圖片與資料
                cloneItem.transform.GetComponent<Equipment>().EquipImage.sprite = thisItem.EquipImage.sprite;
                cloneItem.transform.GetComponent<Equipment>().EquipmentDatas = thisItem.EquipmentDatas.Clone();
                //背包原位的格子圖片設定為空背包圖 並取消raycast
                thisItem.EquipImage.sprite = BagItemOriginImage;
            }
        }
        else
        {
            Debug.Log(">>>>>>>>>>>> 拉到 沒資料的格子");
            return;
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
        if (!cloneItemImage) return;
        PointerEventData data = baseEventData as PointerEventData;

        GameObject MovingItem = data.pointerCurrentRaycast.gameObject;

        //複製的物件跟隨鼠標
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(BagWindowTransform.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out pos);

        //因為跟拖曳物品的圖片會造成Z軸超遠偏移(不知明原因) 所以背包物品拖曳的版本多寫了Z軸的控制
        cloneItem.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pos.y, BagWindowTransform.GetComponent<RectTransform>().position.z);

        //因為跟拖曳物品的圖片會造成旋轉(不知明原因) 所以背包物品拖曳的版本多了旋轉軸歸零
        cloneItem.transform.localRotation = Quaternion.identity;

        //關閉物件raycast
        if (cloneItem.transform.GetComponent<Image>().raycastTarget)
            cloneItem.transform.GetComponent<Image>().raycastTarget = false;

        //獲取複製物件的裝備資料
        Equipment cloneObjEquipment = cloneItem.transform.GetComponent<Equipment>();

        //檢查空值 拖曳物品到背包的任一欄位時
        if (MovingItem != null)
        {
            if (MovingItem.tag == "Equip")  //檢查滑鼠接觸的欄位是否為裝備欄
            {
                //獲取裝備欄腳本
                EquipData equipData = MovingItem.transform.GetComponent<EquipData>();

                /*檢查滑鼠偵測到的欄位是否為正確穿著位置*/
                //檢查武器欄
                if (cloneObjEquipment.EquipmentDatas.Weapon != null)
                {
                    HintMethod(equipData.Hint, true,
                        (equipData.PartID.Contains(cloneObjEquipment.EquipmentDatas.Weapon.TackHandID) &&
                        PlayerDataOverView.Instance.PlayerData_.Lv >= cloneObjEquipment.EquipmentDatas.Weapon.LV)
                        ? Color.green : Color.red);
                }
                //檢查裝備欄
                else if (cloneObjEquipment.EquipmentDatas.Armor != null)
                {
                    HintMethod(equipData.Hint, true,
                        (equipData.PartID.Contains(cloneObjEquipment.EquipmentDatas.Armor.WearPartID) &&
                        PlayerDataOverView.Instance.PlayerData_.Lv >= cloneObjEquipment.EquipmentDatas.Armor.NeedLv)
                        ? Color.green : Color.red);
                }
                else
                    HintMethod(equipData.Hint, true, Color.red);
            }
        }
        else
            //離開裝備欄 隱藏裝備欄提示物件
            HintMethod(null, false, Color.red);
    }

    /// <summary>
    /// 放開拖曳
    /// </summary>
    /// <param name="data"></param>
    public void EndDrag(BaseEventData baseEventData)
    {
        PointerEventData data = baseEventData as PointerEventData;

        //獲取放開拖曳後 滑鼠座標的物件
        GameObject endDragTargetObj = data.pointerCurrentRaycast.gameObject;

        //檢查空值
        if (endDragTargetObj == null)
        {
            print("空值return");
            ReverseDragObj();
            return;
        }
        else
        {
            MovingItemCheck(endDragTargetObj);
        }
    }

    #endregion

    /// <summary>
    /// 快捷鍵穿脫裝處理
    /// </summary>
    /// <param name="target"></param>
    public void HotKeyItemEquipProceoosr(Equipment target)
    {
        //物品正在穿戴中
        if (BagManager.Instance.EquipDataList.Any(x => x.EquipmentDatas == target.EquipmentDatas))
        {
            //找出穿戴資料
            var wearing = BagManager.Instance.EquipDataList.Find(x => x == target);
            //找出背包內空格的資料
            var emptyBagItem = BagManager.Instance.BagItems.Where(x => x.EquipmentDatas.Weapon == null &&
            x.EquipmentDatas.Armor == null &&
            x.EquipmentDatas.Item == null).FirstOrDefault();
            //設定背包內空格的資料與圖片
            emptyBagItem.EquipmentDatas = new EquipmentData(wearing.EquipmentDatas);
            emptyBagItem.EquipImage.sprite = wearing.EquipImage.sprite;

            //清空裝備欄資料並設定原圖
            wearing.EquipmentDatas = new EquipmentData();
            wearing.EquipImage.sprite = BagItemOriginImage;

            //快捷鍵穿戴效果框處理
            CheckHotKeyUseFrame();

            //刷新能力值數據(穿裝或脫裝)
            StatusOperation.Instance.StatusMethod();
            //跳出方法(此次為做好脫裝即可)
            return;
        }

        //找背包內的物品
        Equipment bagEquipment = BagManager.Instance.BagItems.Find(x => x.EquipmentDatas == target.EquipmentDatas);
        //穿戴裝備欄
        Equipment targetEquipment = null;
        //搜尋武器
        if (bagEquipment.EquipmentDatas.Weapon != null)
            //找出要裝備的裝備欄
            targetEquipment = BagManager.Instance.EquipDataList.Select(x => x.GetComponent<EquipData>()).ToList().
                 Where(x => x.PartID.Any(y => y.Contains(bagEquipment.EquipmentDatas.Weapon.TackHandID))).Select(x => x.GetComponent<Equipment>()).FirstOrDefault();
        //搜尋防具
        if (bagEquipment.EquipmentDatas.Armor != null)
            //找出要裝備的裝備欄
            targetEquipment = BagManager.Instance.EquipDataList.Select(x => x.GetComponent<EquipData>()).ToList().
                 Where(x => x.PartID.Any(y => y.Contains(bagEquipment.EquipmentDatas.Armor.WearPartID))).Select(x => x.GetComponent<Equipment>()).FirstOrDefault();

        //檢查裝備等級是否符合 
        if (bagEquipment)
        {
            //取得裝備等級
            int getItemLV = bagEquipment.EquipmentDatas.Weapon != null ? bagEquipment.EquipmentDatas.Weapon.LV : bagEquipment.EquipmentDatas.Armor.NeedLv;
            if (PlayerDataOverView.Instance.PlayerData_.Lv >= getItemLV)
            {
                //設定裝備欄
                CommonFunction.ChangeSameComponent(ref targetEquipment.EquipmentDatas,  ref bagEquipment.EquipmentDatas);
                var temp = targetEquipment.EquipImage.sprite;
                targetEquipment.EquipImage.sprite = bagEquipment.EquipImage.sprite;
                bagEquipment.EquipImage.sprite = temp;
                //設定武器
                if (targetEquipment.EquipmentDatas.Weapon != null)
                    TakeWeaponProcessor(targetEquipment.EquipmentDatas.Weapon.TackHandID);

                //快捷鍵穿戴效果框處理
                CheckHotKeyUseFrame();
            }
        }

        //刷新能力值數據(穿裝或脫裝)
        StatusOperation.Instance.StatusMethod();
    }

    /// <summary>
    /// 移動物品後確認處理
    /// </summary>
    /// <param name="endDragTargetObj">放開拖曳後的物件</param>
    public void MovingItemCheck(GameObject endDragTargetObj)
    {
        //從放開拖曳後的物件上取得物品資料
        Equipment endDragTargetEquipment = endDragTargetObj.GetComponent<Equipment>();
        //從拖曳中的物件上取得物品資料
        Equipment movingEquipment = cloneItem.GetComponent<Equipment>();
        //從放開拖曳後的物件上取得穿戴部位腳本
        EquipData equipdata = endDragTargetObj.GetComponent<EquipData>();

        if (endDragTargetObj.tag == "Equip")
        {
            //檢查武器欄
            if (movingEquipment.EquipmentDatas.Weapon != null)
            {
                if (equipdata.PartID.Contains(movingEquipment.EquipmentDatas.Weapon.TackHandID) &&
                    PlayerDataOverView.Instance.PlayerData_.Lv >= movingEquipment.EquipmentDatas.Weapon.LV)
                {
                    print("裝備武器");
                    TakeWeaponProcessor(movingEquipment.EquipmentDatas.Weapon.TackHandID);
                    PutOnEquip(endDragTargetEquipment);

                    //快捷鍵穿戴效果框處理
                    CheckHotKeyUseFrame();
                }
                else
                {
                    print("雖然是武器 但穿戴部位錯誤return");
                    ReverseDragObj();
                    return;
                }
            }
            //檢查裝備欄
            else if (movingEquipment.EquipmentDatas.Armor != null)
            {

                if (equipdata.PartID.Contains(movingEquipment.EquipmentDatas.Armor.WearPartID) &&
                     PlayerDataOverView.Instance.PlayerData_.Lv >= movingEquipment.EquipmentDatas.Armor.NeedLv)
                {
                    print("裝備防具");
                    PutOnEquip(endDragTargetEquipment);

                    //快捷鍵穿戴效果框處理
                    CheckHotKeyUseFrame();
                }
                else
                {
                    print("雖然是裝備 但穿戴部位錯誤return");
                    ReverseDragObj();
                    return;
                }
            }
            else
            {
                print("拖曳物品不是武器也不是裝備 是道具 所以也return");
                ReverseDragObj();
                return;
            }
        }
        //若是背包格       
        else if (endDragTargetObj.tag == "BagItem")
        {
            //若開始拖曳的欄位是裝備欄 則是脫裝 脫裝要檢查 若放開拖曳的格子有資料的話 判斷是否可以更換裝備
            if (OriginItemSeat.tag == "Equip")
            {
                EquipmentData endDrageTargetObjData = endDragTargetEquipment.EquipmentDatas;
                //從放開拖曳後的物件上取得物品資料
                Equipment originItemSeatEquipment = OriginItemSeat.GetComponent<Equipment>();
                //若是空格的情況 直接更換
                if (endDrageTargetObjData.Armor == null &&
                    endDrageTargetObjData.Weapon == null &&
                    endDrageTargetObjData.Item == null)
                {
                    endDragTargetEquipment.EquipImage.sprite = movingEquipment.EquipImage.sprite;
                    endDragTargetEquipment.EquipmentDatas = movingEquipment.EquipmentDatas.Clone();
                    ReductionDrag();
                    originItemSeatEquipment.EquipImage.sprite = BagItemOriginImage;
                    originItemSeatEquipment.EquipmentDatas.Armor = null;
                    originItemSeatEquipment.EquipmentDatas.Weapon = null;
                    originItemSeatEquipment.EquipmentDatas.Item = null;

                    //刷新能力值數據(脫裝)
                    StatusOperation.Instance.StatusMethod(true);

                    //快捷鍵穿戴效果框處理
                    CheckHotKeyUseFrame();
                    print("將裝備脫到背包內空的格子上");
                }
                //檢查該格 是否有武器資料
                else if (endDrageTargetObjData.Weapon != null)
                {
                    if (equipdata.PartID.Contains(originItemSeatEquipment.EquipmentDatas.Weapon.TackHandID))
                    {
                        print("從裝備欄脫下武器跟背包的武器交換 更換武器");
                        TakeWeaponProcessor(originItemSeatEquipment.EquipmentDatas.Weapon.TackHandID);
                        PutOnEquip(endDragTargetEquipment);

                        //快捷鍵穿戴效果框處理
                        CheckHotKeyUseFrame();
                    }
                    else
                    {
                        print("錯的武器類型 return");
                        ReverseDragObj();
                        return;
                    }
                }
                //檢查該格 是否有裝備資料
                else if (endDrageTargetObjData.Armor != null)
                {
                    if (equipdata.PartID.Contains(originItemSeatEquipment.EquipmentDatas.Armor.WearPartID))
                    {
                        print("從裝備欄脫下防具跟背包的防具交換 更換防具");
                        PutOnEquip(endDragTargetEquipment);

                        //快捷鍵穿戴效果框處理
                        CheckHotKeyUseFrame();
                    }
                    else
                    {
                        print("錯的防具類型 return");
                        ReverseDragObj();
                        return;
                    }
                }
                //若該格是道具 不可更換穿戴
                else if (endDrageTargetObjData.Item != null)
                {
                    print("若該格是道具 不可更換穿戴 return");
                    ReverseDragObj();
                    return;
                }
            }
            else if (OriginItemSeat.tag == "BagItem")
            {
                //若是背包格的話則是換位置
                print("背包格的內交換位置 return");
                ChangeSeat(endDragTargetEquipment);
                return;
            }
        }
        //若是快捷鍵
        else if (endDragTargetObj.tag == "HotKey")
        {
            SetItemEffectHotKey(endDragTargetObj.GetComponent<HotKeyData>(),
movingEquipment.EquipImage.sprite, CommonFunction.LoadItemEffectPrefab(movingEquipment.EquipmentDatas.EquipmentDataToJson_.CodedID, movingEquipment.EquipmentDatas.EquipmentDataToJson_.Type));

            //快捷鍵穿戴效果框處理
            CheckHotKeyUseFrame();
        }
        //都不是則回原位子
        else
        {
            print("不是背包也不是裝備(也不知道會不會有這情形) return");
            ReverseDragObj();
        }
    }

    /// <summary>
    /// 設定裝備欄提示狀態
    /// </summary>
    /// <param name="hint">提示物件</param>
    /// <param name="isEnable">顯示狀態</param>
    /// <param name="color">顏色</param>
    private void HintMethod(GameObject hint, bool isEnable, Color color)
    {
        if (hint != null)
        {
            hint.SetActive(isEnable);
            hint.GetComponent<Image>().color = color;
        }
        else
        {
            for (int i = 0; i < SetHints.Length; i++)
            {
                SetHints[i].gameObject.SetActive(false);
            }
            //ItemManager.Instance.EquipDataList.ForEach(x => x.Hint.SetActive(isEnable));
            //ItemManager.Instance.EquipDataList.ForEach(x => x.Hint.GetComponent<Image>().color = color);
        }
    }

    /// <summary>
    /// 背包格內物品位置互換
    /// </summary>
    /// <param name="BagSeat">交換位置的目標物件</param>
    public void ChangeSeat(Equipment BagSeat)
    {
        //原位置的物品資料與目標物品資料交換
        OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite = BagSeat.EquipImage.sprite;
        OriginItemSeat.GetComponent<Equipment>().EquipmentDatas = BagSeat.EquipmentDatas.Clone();

        BagSeat.EquipImage.sprite = cloneItem.GetComponent<Equipment>().EquipImage.sprite;
        BagSeat.EquipmentDatas = cloneItem.GetComponent<Equipment>().EquipmentDatas.Clone();
        print("物品交換");
        ReductionDrag();
    }

    /// <summary>
    /// 穿上裝備或是武器
    /// </summary>
    /// <param name="Equip">目標格子</param>
    public void PutOnEquip(Equipment Equip)
    {
        //取得拖曳中裝備的腳本資訊
        Equipment cloneItemData = cloneItem.GetComponent<Equipment>();

        //檢測目標格子是否已有穿戴物件(武器或防具)
        if (Equip.EquipmentDatas.Armor != null || Equip.EquipmentDatas.Weapon != null)
        {
            //刷新能力值數據(脫裝) 更換武器先把技能加成部分以原數據清除
            StatusOperation.Instance.StatusMethod(false);

            print("已有裝備武器進行更換");
            CommonFunction.ChangeSameComponent(ref OriginItemSeat.GetComponent<Equipment>().EquipmentDatas, ref Equip.EquipmentDatas);
            var temp = cloneItemData.EquipImage.sprite;
            OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite = Equip.EquipImage.sprite;
            Equip.EquipImage.sprite = temp;

            //刷新能力值數據(穿裝) 再以新穿的武器計算裝備數據與技能加成
            StatusOperation.Instance.StatusMethod();
        }
        //若沒有裝備 直接穿上
        else
        {
            print("無裝備 穿上裝備");
            //設定裝備資料
            Equip.EquipmentDatas = new EquipmentData(cloneItemData.EquipmentDatas);
            //設定目標裝備欄圖片
            Equip.EquipImage.sprite = cloneItemData.EquipImage.sprite;
            //還原原始格子
            OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite = BagItemOriginImage;
            OriginItemSeat.GetComponent<Equipment>().EquipmentDatas = new EquipmentData();

            //刷新能力值數據(穿裝)
            StatusOperation.Instance.StatusMethod();
        }
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
        //還原raycast
        OriginItemSeat.transform.GetComponent<Image>().raycastTarget = true;
    }

    /// <summary>
    /// 還原拖曳的物品(裝錯裝備欄 不是放到裝備欄 快捷鍵上的時候使用)
    /// </summary>
    public void ReverseDragObj()
    {
        //還原 原本位置的圖片
        OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite = cloneItem.GetComponent<Equipment>().EquipImage.sprite;
        OriginItemSeat.GetComponent<Equipment>().EquipmentDatas = cloneItem.GetComponent<Equipment>().EquipmentDatas.Clone();
        ReductionDrag();
    }

    /// <summary>
    /// 設置快捷鍵資料
    /// </summary>
    /// <param name="itemEffectHotkey"></param>
    /// <param name="itemEffectIcon"></param>
    /// <param name="itemID"></param>
    public void SetItemEffectHotKey(HotKeyData itemEffectHotkey, Sprite itemEffectIcon, IHotKey itemID)
    {
        itemEffectHotkey.SetItemEffect(itemEffectIcon, itemID, OriginItemSeat.GetComponent<Equipment>());

        //還原原始格子
        ReverseDragObj();
    }

    /// <summary>
    /// 拿取武器處理(解決不合理的拿取設定)
    /// </summary>
    /// <param name="currentWeaponTakeID"></param>
    private void TakeWeaponProcessor(string currentWeaponTakeID)
    {
        //當前武器為雙手武器 檢查副手是否有裝備武器
        if (currentWeaponTakeID == "BothHand")
        {
            //取得副手資料
            var targetEquipment = BagManager.Instance.EquipDataList.Select(x => x.GetComponent<EquipData>()).ToList().
                Where(x => x.PartID.Any(y => y.Contains("LeftHand"))).Select(x => x.GetComponent<Equipment>()).FirstOrDefault();
            if (targetEquipment.EquipmentDatas.Weapon != null)
            {
                var emptyBagItem = BagManager.Instance.BagItems.Where(x => x.EquipmentDatas.Weapon == null &&
          x.EquipmentDatas.Armor == null &&
          x.EquipmentDatas.Item == null).FirstOrDefault();
                //設定背包內空格的資料與圖片
                emptyBagItem.EquipmentDatas = new EquipmentData(targetEquipment.EquipmentDatas);
                emptyBagItem.EquipImage.sprite = targetEquipment.EquipImage.sprite;
                //清空裝備欄資料並設定原圖
                targetEquipment.EquipmentDatas = new EquipmentData();
                targetEquipment.EquipImage.sprite = BagItemOriginImage;
            }
        }
        //當前武器為副手武器 檢查主手是否是雙手武器
        if (currentWeaponTakeID == "LeftHand")
        {
            //取得主手資料
            var targetEquipment = BagManager.Instance.EquipDataList.Select(x => x.GetComponent<EquipData>()).ToList().
                Where(x => x.PartID.Any(y => y.Contains("BothHand"))).Select(x => x.GetComponent<Equipment>()).FirstOrDefault();
            if (targetEquipment.EquipmentDatas.Weapon != null && targetEquipment.EquipmentDatas.Weapon.TackHandID == "BothHand")
            {
                var emptyBagItem = BagManager.Instance.BagItems.Where(x => x.EquipmentDatas.Weapon == null &&
          x.EquipmentDatas.Armor == null &&
          x.EquipmentDatas.Item == null).FirstOrDefault();
                //設定背包內空格的資料與圖片
                emptyBagItem.EquipmentDatas = new EquipmentData(targetEquipment.EquipmentDatas);
                emptyBagItem.EquipImage.sprite = targetEquipment.EquipImage.sprite;
                //清空裝備欄資料並設定原圖
                targetEquipment.EquipmentDatas = new EquipmentData();
                targetEquipment.EquipImage.sprite = BagItemOriginImage;
            }
        }
    }

    /// <summary>
    /// 確認快捷鍵上放置裝備內容的使用狀態
    /// </summary>
    private void CheckHotKeyUseFrame()
    {
        //取得 擁有資料的所有快捷鍵
       var hotkeyItemList =  HotKeyManager.Instance.HotKeyArray.Where(x => x.TempHotKeyData != null).ToList();

        foreach (var hotkeyItem in hotkeyItemList)
        {
            //檢查是否為裝備類型效果
            if(hotkeyItem.TempHotKeyData is ItemEffectBase_Equip itemEffectBase)
            {
                //裝備類型效果 檢查是否正在裝備中 開關使用中效果框
                hotkeyItem.UseFrameEnable(itemEffectBase.CheckItemIsUse());
            }
        }
    }
}
