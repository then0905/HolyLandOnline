using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/07/27
//  創建用途: NPC管理器
//==========================================
public class NpcManager : MonoBehaviour
{
    #region 靜態變數

    private static NpcManager instance;
    public static NpcManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<NpcManager>(true);
            return instance;
        }
    }

    #endregion

    //暫存此地圖生成的npc資料 <NpcID,生成座標>
    private Dictionary<string, Vector3> tempMapNpcSpawnData = new Dictionary<string, Vector3>();
    //暫存已生成的npc物件
    private List<NpcBehavior> tempMapNpcSpawnObj = new List<NpcBehavior>();

    /// <summary>
    /// 初始化
    /// </summary>
    public void InitNpcManger(string mapName)
    {
        //獲取地圖的NPC資訊
        tempMapNpcSpawnData = GameData.NpcDataDic.Values.Where(x => x.AreaID == mapName).ToDictionary(x => x.NpcID, x => new Vector3(x.NpcPosX, x.NpcPosY, x.NpcPosZ));
        SpawnNpc(tempMapNpcSpawnData);
    }

    /// <summary>
    /// 生成NPC物件
    /// </summary>
    /// <param name="tempMapNpcSpawnData"></param>
    public void SpawnNpc(Dictionary<string, Vector3> tempMapNpcSpawnData)
    {
        ClearNpcObj();
        foreach (var item in tempMapNpcSpawnData)
        {
            NpcBehavior npcLegacy = CommonFunction.LoadObject<GameObject>(GameConfig.NpcPrefab, item.Key).GetComponent<NpcBehavior>();
            Vector3 getNpcSpwanPos = item.Value;
            NpcBehavior npcObj = Instantiate(npcLegacy, getNpcSpwanPos, npcLegacy.transform.rotation);
            tempMapNpcSpawnObj.Add(npcObj);
            //monsterObj.Init();
        }
    }

    /// <summary>
    /// 清空生成的NPC物件
    /// </summary>
    public void ClearNpcObj()
    {
        if (tempMapNpcSpawnObj.CheckAnyData())
        {
            try
            {
                foreach (var item in tempMapNpcSpawnObj)
                {
                    if (item.gameObject != null) Destroy(item.gameObject);
                }
            } catch (Exception e)
            {

            }

            tempMapNpcSpawnObj.Clear();
        }
    }

    /// <summary>
    /// 測試方法 召喚指定NPC
    /// </summary>
    /// <param name="npcName"></param>
    public void TestMethod_AddNpc(string npcName)
    {
        GameObject npcObj = CommonFunction.LoadObject<GameObject>(GameConfig.NpcPrefab, npcName);
        Vector3 targetPos = new Vector3(PlayerDataOverView.Instance.CharacterMove.transform.position.x + 5, 3f, PlayerDataOverView.Instance.CharacterMove.transform.position.z + 5);
        Instantiate(npcObj, targetPos, npcObj.transform.rotation);
    }
}
