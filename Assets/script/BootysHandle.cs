using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JsonDataModel;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/05/27
//  創建用途:  怪物掉落物處理
//==========================================

public class BootysHandle : MonoBehaviour
{
    protected int Coin;            //金幣量
    public GameObject BootyCoin;        //金幣物件
    public GameObject BootyItem;        //掉落物物件
    BootysPresent bootyItem;        //紀錄生成的掉落物

    /// <summary>
    /// 取得掉落物資料
    /// </summary>
    /// <param name="MonsterID"></param>
    /// <param name="MonsterTransform"></param>
    public void GetBootysData(string MonsterID, Transform monsterTransform)
    {
        var bootysData = GameData.BootysDic.Where(x => x.Key.Contains(MonsterID)).Select(x => x.Value).FirstOrDefault();
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
    private void GenerateCoin(BootyDataModel bootysData, Transform monsterTransform)
    {
        // 運算金幣量
        Coin = Random.Range(bootysData.CoinMin, bootysData.CoinMax + 1);
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
    private void GenerateItem(BootyDataModel bootysData, Transform monsterTransform)
    {
        //分割掉落物清單
        List<string> chanceList = bootysData.BootysChance.Split(':').ToList();
        List<string> bootyList = bootysData.Bootys.Split(':').ToList();

        foreach (var chance in chanceList)
        {
            // 取得隨機值
            int RandomVaule = Random.Range(1, 101);
            if (RandomVaule <= int.Parse(chance))//如果骰完小於該戰利品的機率則生成物件
            {
                //獲取掉落物資料
                var booty = bootyList[chanceList.IndexOf(chance)];
                //生成掉落物
                bootyItem = Instantiate(BootyItem, RandomTransform(monsterTransform), transform.rotation).GetComponent<BootysPresent>();
                //從資料庫抓出防具資料 或是空值
                bootyItem.EquipmentDatas.Armor = GameData.ArmorsDic.Where(x => x.Value.Name.Contains(booty)).FirstOrDefault().Value;
                //從資料庫抓出武器資料 或是空值
                bootyItem.EquipmentDatas.Weapon = GameData.WeaponsDic.Where(x => x.Value.Name.Contains(booty)).FirstOrDefault().Value;
                //從資料庫抓出道具資料 或是空值
                bootyItem.EquipmentDatas.Item = GameData.ItemsDic.Where(x => x.Value.Name.Contains(booty)).FirstOrDefault().Value;
            }
        }
    }
}
