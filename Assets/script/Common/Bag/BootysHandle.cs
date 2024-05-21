using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    /// <param name="MonsterID"></param>
    /// <param name="MonsterTransform"></param>
    public void GetBootysData(string MonsterID, Transform monsterTransform)
    {
        var bootysData = GameData.MonsterBootysDic.Where(x => x.Key.Contains(MonsterID)).Select(x => x.Value).FirstOrDefault();
        GenerateCoin(bootysData, monsterTransform);
        GenerateItem(bootysData, monsterTransform);
    }

    /// <summary>
    /// 回傳座標
    /// </summary>
    /// <param name="monsterTransform"></param>
    /// <returns></returns>
    protected Vector3 RandomTransform(Transform monsterTransform)
    {
        return new Vector3(Random.Range(monsterTransform.position.x - 3, monsterTransform.position.x + 3)
            , monsterTransform.position.y
            , (Random.Range(monsterTransform.position.z - 3, monsterTransform.position.z + 3)));
    }

    /// <summary>
    /// 掉落金幣運算與實現
    /// </summary>
    /// <param name="bootysData">掉落物資料</param>
    /// <param name="monsterTransform">生成父級</param>
    private void GenerateCoin(MonsterBootyDataModel bootysData, Transform monsterTransform)
    {
        // 運算金幣量
        Coin = Random.Range(bootysData.MinCoin, bootysData.MaxCoin + 1);
        // 生成物件
        bootyItem = Instantiate(BootyItem, RandomTransform(monsterTransform), transform.rotation).GetComponent<BootysPresent>();
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
    private void GenerateItem(MonsterBootyDataModel bootysData, Transform monsterTransform)
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
            //從資料庫抓出防具資料 或是空值
            bootyItem.EquipmentDatas.Armor = GameData.ArmorsDic.Where(x => x.Key.Contains(tempBootyListData.BootyID)).FirstOrDefault().Value;
            //從資料庫抓出武器資料 或是空值
            bootyItem.EquipmentDatas.Weapon = GameData.WeaponsDic.Where(x => x.Key.Contains(tempBootyListData.BootyID)).FirstOrDefault().Value;
            //從資料庫抓出道具資料 或是空值
            bootyItem.EquipmentDatas.Item = GameData.ItemsDic.Where(x => x.Key.Contains(tempBootyListData.BootyID)).FirstOrDefault().Value;
            //取得掉落數量
            bootyItem.Qty = UnityEngine.Random.Range(tempBootyListData.DropCountMin,tempBootyListData.DropCountMax);
        }
    }
}
