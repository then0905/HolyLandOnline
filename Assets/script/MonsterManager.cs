using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

//==========================================
//  創建者:家豪
//  創建日期:2024/03/30
//  創建用途:怪物管理
//==========================================
public class MonsterManager : MonoBehaviour
{
    /// <summary>
    /// 測試方法 召喚指定怪物
    /// </summary>
    /// <param name="monsterName"></param>
    public void TestMethod_AddMonster(string monsterName)
    {
        GameObject monsterObj = CommonFunction.LoadObject<GameObject>(GameConfig.Monster, monsterName);
        Vector3 targetPos = new Vector3(Character_move.Instance.transform.position.x + 5, 3f, Character_move.Instance.transform.position.z + 5);
        Instantiate(monsterObj, targetPos, monsterObj.transform.rotation);
    }
}
