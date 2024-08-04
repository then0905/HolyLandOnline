using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    private Dictionary<Vector3, MonsterBehaviour> tempMapMonsterSpawnObj = new Dictionary<Vector3, MonsterBehaviour>();


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
            //獲取資源
            MonsterBehaviour monsterLegacy = CommonFunction.LoadObject<GameObject>(GameConfig.Monster, item.MonsterCodeID).GetComponent<MonsterBehaviour>();
            //取得座標
            Vector3 getMonsterSpwanPos = new Vector3(item.SpawnPosX, item.SpawnPosY, item.SpawnPosZ);
            //生成物件
            MonsterBehaviour monsterObj = Instantiate(monsterLegacy, getMonsterSpwanPos, monsterLegacy.transform.rotation);
            //設定紀錄資料
            tempMapMonsterSpawnObj.Add(getMonsterSpwanPos, monsterObj);
            //怪物初始化
            monsterObj.Init(getMonsterSpwanPos);
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
                    if (item.Value.gameObject != null) Destroy(item.Value.gameObject);
                }
            }
            catch (Exception e)
            {

            }
            tempMapMonsterSpawnObj.Clear();
        }
    }

    /// <summary>
    ///  設定怪物重生
    /// </summary>
    /// <param name="vector3">生成座標</param>
    /// <param name="monsterBehaviour">怪物腳本</param>
    /// <param name="delay">是否延遲生成</param>
    public async void SetMonsterRebirth(Vector3 vector3, MonsterBehaviour monsterBehaviour, bool delay = true)
    {
        //獲取重生時間
        float rebirthDC = monsterBehaviour.MonsterValue.RebirthCD;

        //是否設定重生延遲
        if (delay)
            await Task.Delay((int)(rebirthDC * 1000));

        //尋找要生成的怪物
        var queryResult = tempMapMonsterSpawnObj.Where(x => x.Key == vector3).FirstOrDefault();
        //獲取資源
        MonsterBehaviour monsterLegacy = CommonFunction.LoadObject<GameObject>(GameConfig.Monster, monsterBehaviour.MonsterValue.MonsterCodeID).GetComponent<MonsterBehaviour>();
        //生成物件
        MonsterBehaviour monsterObj = Instantiate(monsterLegacy, vector3, monsterLegacy.transform.rotation);
        //設定紀錄資料
        tempMapMonsterSpawnObj.TrySetValue(vector3, monsterObj);
        //設定該怪物的初始化
        monsterObj.Init(vector3);
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
