using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

//==========================================
//  創建者:家豪
//  創建日期:2024/03/30
//  創建用途:怪物管理腳本
//==========================================
public class MonsterManager : MonoBehaviour
{
    #region 靜態變數

    private static MonsterManager instance;
    public static MonsterManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MonsterManager>(true);
            return instance;
        }
    }

    #endregion

    //暫存此地圖生成的怪物資料
    private List<MonsterSpwanDataModel> tempMapMonsterSpawnData = new List<MonsterSpwanDataModel>();
    //暫存已生成的怪物物件
    private List<MonsterBehaviour> tempMapMonsterSpawnObj = new List<MonsterBehaviour>();

    public void Awake()
    {
        //InitMonsterManger();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void InitMonsterManger(string mapName)
    {
        //獲取地圖的怪物資訊
        tempMapMonsterSpawnData = GameData.MonstersDataDic.Values.Where(x => x.AreaID == mapName).SelectMany(x => x.MonsterSpawnPosList).ToList();
        SpawnMonster(tempMapMonsterSpawnData);
    }

    /// <summary>
    /// 生成怪物物件
    /// </summary>
    /// <param name="tempMapMonsterSpawnData"></param>
    public void SpawnMonster(List<MonsterSpwanDataModel> tempMapMonsterSpawnData)
    {
        ClearMonsterObj();
        foreach (var item in tempMapMonsterSpawnData)
        {
            MonsterBehaviour monsterLegacy = CommonFunction.LoadObject<GameObject>(GameConfig.Monster, item.MonsterCodeID).GetComponent<MonsterBehaviour>();
            Vector3 getMonsterSpwanPos = new Vector3(item.SpawnPosX, item.SpawnPosY, item.SpawnPosZ);
            MonsterBehaviour monsterObj = Instantiate(monsterLegacy, getMonsterSpwanPos, monsterLegacy.transform.rotation);
            tempMapMonsterSpawnObj.Add(monsterObj);
            monsterObj.Init();
        }
    }

    /// <summary>
    /// 清空生成的怪物物件
    /// </summary>
    public void ClearMonsterObj()
    {
        if (tempMapMonsterSpawnObj != null && tempMapMonsterSpawnObj.Count > 0)
        {
            try
            {
                foreach (var item in tempMapMonsterSpawnObj)
                {
                    if (item.gameObject != null) Destroy(item.gameObject);
                }
            }
            catch(Exception e)
            {

            }
            tempMapMonsterSpawnObj.Clear();
        }
    }

    /// <summary>
    /// 測試方法 召喚指定怪物
    /// </summary>
    /// <param name="monsterName"></param>
    public void TestMethod_AddMonster(string monsterName)
    {
        GameObject monsterObj = CommonFunction.LoadObject<GameObject>(GameConfig.Monster, monsterName);
        Vector3 targetPos = new Vector3(PlayerDataOverView.Instance.CharacterMove.transform.position.x + 5, 3f, PlayerDataOverView.Instance.CharacterMove.transform.position.z + 5);
        Instantiate(monsterObj, targetPos, monsterObj.transform.rotation);
    }
}
