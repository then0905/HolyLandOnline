using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/27
//  創建用途:  怪物掉落物處理
//==========================================

public class BootysHandle : MonoBehaviour
{
    #region 全域靜態變數

    private static BootysHandle instance;
    public static BootysHandle Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BootysHandle>();
            }
            return instance;
        }
    }

    #endregion
    protected int Coin;            //金幣量
    public GameObject BootyCoin;        //金幣物件
    public GameObject BootyItem;        //掉落物物件
    private BootysPresent bootyItem;        //紀錄生成的掉落物

    /// <summary>
    /// 取得掉落物資料
    /// </summary>
    /// <param name="MonsterID">怪物ID</param>
    /// <param name="monsterTransform">怪物Trans</param>
    /// <param name="battleTargetList">戰鬥資料清單</param>
    public void GetBootysData(string MonsterID, Transform monsterTransform, List<ICombatant> battleTargetList)
    {
        //獲取該怪物的所有掉落物資料
        var bootysData = GameData.MonsterBootysDic.Where(x => x.Key.Contains(MonsterID)).Select(x => x.Value).FirstOrDefault();
        //獲取該怪物的掉落物設定權限
        int bootysLockSetting = GameData.MonstersDataDic[MonsterID].BootysLockSetting;
        GenerateCoin(bootysData, monsterTransform, battleTargetList, bootysLockSetting);
        GenerateItem(bootysData, monsterTransform, battleTargetList, bootysLockSetting);
    }

    /// <summary>
    /// 回傳座標
    /// </summary>
    /// <param name="monsterTransform"></param>
    /// <returns></returns>
    protected Vector3 RandomTransform(Transform monsterTransform)
    {
        return new Vector3(Random.Range(monsterTransform.position.x - 3, monsterTransform.position.x + 3)
            , 0
            , (Random.Range(monsterTransform.position.z - 3, monsterTransform.position.z + 3)));
    }

    /// <summary>
    /// 掉落金幣運算與實現
    /// </summary>
    /// <param name="bootysData">掉落物資料</param>
    /// <param name="monsterTransform">生成父級</param>
    /// <param name="battleTargetList">戰鬥資料清單</param>
    private void GenerateCoin(MonsterBootyDataModel bootysData, Transform monsterTransform, List<ICombatant> battleTargetList, int bootysLockSetting)
    {
        // 運算金幣量
        Coin = Random.Range(bootysData.MinCoin, bootysData.MaxCoin + 1);
        // 生成物件
        bootyItem = Instantiate(BootyItem, RandomTransform(monsterTransform), transform.rotation).GetComponent<BootysPresent>();
        //掉落物初始化設定
        bootyItem.Init(bootysLockSetting, battleTargetList);
        // 設定金幣量
        bootyItem.Coins = Coin;
        //bootyItem.Item.Type = "金幣";
        //bootyItem.Item.Name = "金幣";
    }

    /// <summary>
    /// 生成掉落物資料
    /// </summary>
    /// <param name="bootysData">掉落物資料</param>
    /// <param name="monsterTransform">掉落物位置</param>
    /// <param name="battleTargetList">戰鬥資料清單</param>
    private void GenerateItem(MonsterBootyDataModel bootysData, Transform monsterTransform, List<ICombatant> battleTargetList, int bootysLockSetting)
    {
        //該怪物的掉落物清單
        var bootyList = bootysData.BootyList.Select(x => x.DropProbability).ToList();

        //獲取抽中的掉落物資料
        var bootys = CommonFunction.BootyRandomDrop(bootyList);

        foreach (var booty in bootys)
        {
            var tempBootyListData = bootysData.BootyList[booty];
            //生成掉落物
            bootyItem = Instantiate(BootyItem, RandomTransform(monsterTransform), transform.rotation).GetComponent<BootysPresent>();
            //掉落物初始化設定
            bootyItem.Init(bootysLockSetting, battleTargetList);
            //從資料庫抓出防具資料 或是空值
            bootyItem.EquipmentDatas.Armor = GameData.ArmorsDic.Where(x => x.Key.Contains(tempBootyListData.BootyID)).FirstOrDefault().Value;
            //從資料庫抓出武器資料 或是空值
            bootyItem.EquipmentDatas.Weapon = GameData.WeaponsDic.Where(x => x.Key.Contains(tempBootyListData.BootyID)).FirstOrDefault().Value;
            //從資料庫抓出道具資料 或是空值
            bootyItem.EquipmentDatas.Item = GameData.ItemsDic.Where(x => x.Key.Contains(tempBootyListData.BootyID)).FirstOrDefault().Value;
            //設定物品圖片
            bootyItem.ThisEquipmentImage = CommonFunction.GetItemSprite(bootyItem.EquipmentDatas);
            //取得掉落數量
            bootyItem.Qty = UnityEngine.Random.Range(tempBootyListData.DropCountMin, tempBootyListData.DropCountMax);
        }
    }

    /// <summary>
    /// 測試 掉落指定道具到玩家附近
    /// </summary>
    /// <param name="itemID"></param>
    public void TestDropBooty(TMP_InputField itemID)
    {
        //生成掉落物
        bootyItem = Instantiate(BootyItem, RandomTransform(PlayerDataOverView.Instance.Obj.transform), transform.rotation).GetComponent<BootysPresent>();
        //掉落物初始化設定
        bootyItem.Init(0, new List<ICombatant>() { PlayerDataOverView.Instance });
        //從資料庫抓出防具資料 或是空值
        bootyItem.EquipmentDatas.Armor = GameData.ArmorsDic.Where(x => x.Key.Contains(itemID.text)).FirstOrDefault().Value;
        //從資料庫抓出武器資料 或是空值
        bootyItem.EquipmentDatas.Weapon = GameData.WeaponsDic.Where(x => x.Key.Contains(itemID.text)).FirstOrDefault().Value;
        //從資料庫抓出道具資料 或是空值
        bootyItem.EquipmentDatas.Item = GameData.ItemsDic.Where(x => x.Key.Contains(itemID.text)).FirstOrDefault().Value;
        //設定物品圖片
        bootyItem.ThisEquipmentImage = CommonFunction.GetItemSprite(bootyItem.EquipmentDatas);
        //取得掉落數量
        bootyItem.Qty = 1;
    }
}
