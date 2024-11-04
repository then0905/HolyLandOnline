using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
                    if (equipData.PartID.Contains(cloneObjEquipment.EquipmentDatas.Weapon.TackHandID))
                        HintMethod(equipData.Hint, true, Color.green);
                    else
                        HintMethod(equipData.Hint, true, Color.red);
                }
                //檢查裝備欄
                else if (cloneObjEquipment.EquipmentDatas.Armor != null)
                {
                    if (equipData.PartID.Contains(cloneObjEquipment.EquipmentDatas.Armor.WearPartID))
                        HintMethod(equipData.Hint, true, Color.green);
                    else
                        HintMethod(equipData.Hint, true, Color.red);
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
        Equipment MovingItem = data.pointerCurrentRaycast.gameObject.GetComponent<Equipment>();
        //檢查空值
        if (MovingItem == null)
        {
            print("空值return");
            ReverseDragObj();
            return;
        }
        else
        {
            //若是裝備格
            if (MovingItem.tag == "Equip")
            {
                //檢查武器欄
                if (cloneItem.GetComponent<Equipment>().EquipmentDatas.Weapon != null)
                {
                    if (MovingItem.GetComponent<EquipData>().PartID.Contains(cloneItem.GetComponent<Equipment>().EquipmentDatas.Weapon.TackHandID))
                    {
                        print("裝備武器");
                        PutOnEquip(MovingItem);
                    }
                    else
                    {
                        print("雖然是武器 但穿戴部位錯誤return");
                        ReverseDragObj();
                        return;
                    }
                }
                //檢查裝備欄
                else if (cloneItem.GetComponent<Equipment>().EquipmentDatas.Armor != null)
                {

                    if (MovingItem.GetComponent<EquipData>().PartID.Contains(cloneItem.GetComponent<Equipment>().EquipmentDatas.Armor.WearPartID))
                    {
                        print("裝備防具");
                        PutOnEquip(MovingItem);
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
            else if (MovingItem.tag == "BagItem")
            {
                //若開始拖曳的欄位是裝備欄 則是脫裝 脫裝要檢查 若放開拖曳的格子有資料的話 判斷是否可以更換裝備
                if (OriginItemSeat.tag == "Equip")
                {
                    //若是空格的情況 直接更換
                    if (MovingItem.GetComponent<Equipment>().EquipmentDatas.Armor == null &&
                        MovingItem.GetComponent<Equipment>().EquipmentDatas.Weapon == null &&
                        MovingItem.GetComponent<Equipment>().EquipmentDatas.Item == null)
                    {
                        MovingItem.GetComponent<Equipment>().EquipImage.sprite = cloneItem.GetComponent<Equipment>().EquipImage.sprite;
                        MovingItem.GetComponent<Equipment>().EquipmentDatas = cloneItem.GetComponent<Equipment>().EquipmentDatas.Clone();
                        ReductionDrag();
                        OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite = BagItemOriginImage;
                        OriginItemSeat.GetComponent<Equipment>().EquipmentDatas.Armor = null;
                        OriginItemSeat.GetComponent<Equipment>().EquipmentDatas.Weapon = null;
                        OriginItemSeat.GetComponent<Equipment>().EquipmentDatas.Item = null;

                        //刷新能力值數據(脫裝)
                        StatusOperation.Instance.StatusMethod(true);
                        print("將裝備脫到背包內空的格子上");
                    }
                    //檢查該格 是否有武器資料
                    else if (MovingItem.GetComponent<Equipment>().EquipmentDatas.Weapon != null)
                    {
                        if (MovingItem.GetComponent<EquipData>().PartID.Contains(cloneItem.GetComponent<Equipment>().EquipmentDatas.Weapon.TackHandID))
                        {
                            print("從裝備欄脫下武器跟背包的武器交換 更換武器");
                            PutOnEquip(MovingItem);
                        }
                        else
                        {
                            print("錯的武器類型 return");
                            ReverseDragObj();
                            return;
                        }
                    }
                    //檢查該格 是否有裝備資料
                    else if (MovingItem.GetComponent<Equipment>().EquipmentDatas.Armor != null)
                    {
                        if (MovingItem.GetComponent<EquipData>().PartID.Contains(cloneItem.GetComponent<Equipment>().EquipmentDatas.Armor.WearPartID))
                        {
                            print("從裝備欄脫下防具跟背包的防具交換 更換防具");
                            PutOnEquip(MovingItem);
                        }
                        else
                        {
                            print("錯的防具類型 return");
                            ReverseDragObj();
                            return;
                        }
                    }
                    //若該格是道具 不可更換穿戴
                    else if (MovingItem.GetComponent<Equipment>().EquipmentDatas.Item != null)
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
                    ChangeSeat(MovingItem);
                    return;
                }
            }
            //都不是則回原位子
            else
            {
                print("不是背包也不是裝備(也不知道會不會有這情形) return");
                ReverseDragObj();
            }
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
        //CommonFunction.ChangeSameComponent(OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite, BagSeat.GetComponent<Equipment>().EquipImage.sprite);
        //CommonFunction.ChangeSameComponent(OriginItemSeat.GetComponent<Equipment>().EquipmentDatas, BagSeat.GetComponent<Equipment>().EquipmentDatas);
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
        //檢測目標格子是否已有穿戴物件(武器或防具)
        if (Equip.EquipmentDatas.Armor != null || Equip.EquipmentDatas.Weapon != null)
        {
            //刷新能力值數據(脫裝) 更換武器先把技能加成部分以原數據清除
            StatusOperation.Instance.StatusMethod(false);

            print("已有裝備武器進行更換");
            CommonFunction.ChangeSameComponent(OriginItemSeat.GetComponent<Equipment>().EquipmentDatas, Equip.EquipmentDatas);
            CommonFunction.ChangeSameComponent(OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite, Equip.EquipImage.sprite);

            //刷新能力值數據(穿裝) 再以新穿的武器計算裝備數據與技能加成
            StatusOperation.Instance.StatusMethod();

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
            print("無裝備 穿上裝備");
            //設定裝備資料
            Equip.EquipmentDatas = new Equipment.EquipmentData
            {
                Weapon = cloneItem.GetComponent<Equipment>().EquipmentDatas.Weapon,
                Armor = cloneItem.GetComponent<Equipment>().EquipmentDatas.Armor,
                Item = cloneItem.GetComponent<Equipment>().EquipmentDatas.Item
            };

            //OriginItemSeat.GetComponent<Image>().sprite = BagItemOriginImage;
            //設定目標裝備欄圖片
            Equip.EquipImage.sprite = cloneItem.GetComponent<Equipment>().EquipImage.sprite;
            //還原原始格子
            OriginItemSeat.GetComponent<Equipment>().EquipImage.sprite = BagItemOriginImage;
            OriginItemSeat.GetComponent<Equipment>().EquipmentDatas.Armor = null;
            OriginItemSeat.GetComponent<Equipment>().EquipmentDatas.Weapon = null;
            OriginItemSeat.GetComponent<Equipment>().EquipmentDatas.Item = null;

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
}
