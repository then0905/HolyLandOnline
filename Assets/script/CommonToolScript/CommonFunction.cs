using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Newtonsoft.Json;
using System.IO;
using static Equipment;
using System.Security;

//==========================================
//  創建者:    家豪
//  創建日期:  2023/05/03
//  創建用途:  通用方法
//==========================================
public static class CommonFunction
{
    /// <summary>
    /// 用來交換兩個相同物件資料
    /// </summary>
    /// <typeparam name="T">交換的內容</typeparam>
    /// <param name="t1">目標1</param>
    /// <param name="t2">目標2</param>
    public static void ChangeSameComponent<T>(T t1, T t2)
    {
        T temporaryT = t1;
        t1 = t2;
        t2 = temporaryT;
    }

    /// <summary>
    /// 讀取Json方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static List<T> DeserializeJson<T>(string path, string name)
    {
        TextAsset binAsset = Resources.Load<TextAsset>(path + "/" + name);
        string jsonText = binAsset.text;
        var jsonString = JsonConvert.DeserializeObject<List<T>>(jsonText);
        // var jsonString = JsonConvert.DeserializeObject<string>(jsonText);


        return jsonString;
    }

    /// <summary>
    /// 儲存本地資料
    /// </summary>
    /// <typeparam name="T">帶入資料結構</typeparam>
    /// <param name="path">路徑</param>
    /// <param name="name">檔案名稱</param>
    /// <param name="item">資料結構</param>
    public static void SaveLocalData<T>(string path, string name, T item)
    {
        string usersave = JsonUtility.ToJson(item);
        FileStream fs = new FileStream(path + name, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(usersave);
        sw.Close();
    }

    /// <summary>
    /// 輸入字典資料方法
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="name"></param>
    /// <param name="path"></param>
    public static void InitData<I, K>(Dictionary<I, K> dictionary, string path, params string[] name)
        where I : IConvertible
        where K : IDictionaryData<I>
    {
        for (int j = 0; j < name.Length; j++)
        {
            var list = DeserializeJson<K>(path, name[j]);
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i];
                dictionary.TrySetValue(t.GetKey, t);
            }
        }
    }

    /// <summary>
    /// 依物品資料獲取圖片資源
    /// </summary>
    /// <param name="path">圖片路徑</param>
    /// <param name="name">圖片檔名</param>
    /// <returns></returns>
    public static Sprite GetItemSprite(EquipmentData tempData)
    {
        Sprite sprite = null;
        if (tempData.Weapon != null)
            sprite = LoadObject<Sprite>(tempData.Weapon.Path, tempData.Weapon.FileName);
        if (tempData.Armor != null)
            sprite = LoadObject<Sprite>(tempData.Armor.Path, tempData.Armor.FileName);
        if (tempData.Item != null)
            sprite = LoadObject<Sprite>(tempData.Item.Path, tempData.Item.FileName);
        return sprite;
    }

    /// <summary>
    /// Resorces讀取物件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T LoadObject<T>(string path, string name)
        where T : UnityEngine.Object
    {
        return Resources.Load<T>(path + "/" + name);
    }

    /// <summary>
    /// 讀取技能圖示專用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Sprite LoadSkillIcon(string name)
    {
        return Resources.LoadAll<Sprite>("").FirstOrDefault(x => x.name == name);
    }

    /// <summary>
    /// 讀取技能預製物專用
    /// </summary>
    /// <param name="skillID">技能ID</param>
    /// <returns></returns>
    public static Skill_Base LoadSkillPrefab(string skillID)
    {
        return LoadObject<GameObject>(GameConfig.SkillPrefab, "SkillEffect_" + skillID).GetComponent<Skill_Base>();
    }

    /// <summary>
    /// 取得文字內容
    /// </summary>
    /// <param name="id">帶入文字ID</param>
    /// <param name="addColon">是否需要冒號</param>
    /// <returns></returns>
    public static string GetText(this string id, bool addColon = false)
    {
        //試圖取得文字資料
        if (GameData.GameTextDataDic.TryGetValue(id, out GameText value))
            //回傳文字內容並且根據是否需求設定冒號
            return (GameData.GameTextDataDic[id].TextContent + (addColon ? ": " : string.Empty));
        else
            //未尋找到文字資料
            return "???";
    }

    /// <summary>
    /// 呼叫系統訊息
    /// </summary>
    /// <param name="content"></param>
    public static void MessageHint(string content, HintType hintType)
    {
        MessageHintSetting message = LoadObject<MessageHintSetting>("Hint", "MessageHint").GetComponent<MessageHintSetting>();
        MessageHintSetting instanceMessage = UnityEngine.Object.Instantiate(message);
        instanceMessage.CallHintCanvas(content, hintType);
    }

    /// <summary>
    /// 轉換參數(List<float>)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<float> SetFloatList(this string value)
    {
        List<float> tempList = new List<float>();
        if (string.IsNullOrEmpty(value)) return null;

        if (!value.Contains(","))
        {
            tempList.Add(float.Parse(value));
        }
        else
        {
            var valueList = value.Split(',').ToList();
            valueList.ForEach(x => tempList.Add(float.Parse(x)));
        }
        return tempList;
    }
    /// <summary>
    /// 轉換參數(List<Int>)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> SetIntList(this string value)
    {
        List<int> tempList = new List<int>();
        if (string.IsNullOrEmpty(value)) return null;

        if (!value.Contains(","))
        {
            tempList.Add(int.Parse(value));
        }
        else
        {
            var valueList = value.Split(',').ToList();
            valueList.ForEach(x => tempList.Add(int.Parse(x)));
        }
        return tempList;
    }
    /// <summary>
    /// 轉換參數(List<float>)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<string> SetStringList(this string value)
    {
        List<string> tempList = new List<string>();
        if (string.IsNullOrEmpty(value)) return null;

        if (!value.Contains(","))
        {
            tempList.Add(value);
        }
        else
        {
            tempList = value.Split(',').ToList();
        }
        return tempList;
    }

    /// <summary>
    /// 依權重隨機抽取內容(必出一個結果)
    /// </summary>
    /// <param name="weights">權重清單</param>
    /// <returns>抽中的內容是清單中的第幾位</returns>
    public static int WeightExtraction(List<float> weights)
    {
        float totalWeight = 0;

        foreach (float weight in weights)
        {
            totalWeight += weight;
        }

        float randomValue = UnityEngine.Random.Range(1, totalWeight + 1);

        float tempWeight = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            tempWeight += weights[i];
            if (randomValue <= tempWeight)
                return i;
        }
        return weights.Count;
    }

    /// <summary>
    /// 依照物品掉落率計算隨機值
    /// </summary>
    /// <param name="weights">權重機率列表</param>
    /// <returns>回傳抽中的結果</returns>
    public static List<int> BootyRandomDrop(List<float> weights)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < weights.Count; i++)
        {
            float randomValue = UnityEngine.Random.Range(0f, 100f);
            if (weights[i] >= randomValue)
                result.Add(i);
        }
        return result;
    }

    /// <summary>
    /// 通用計時器
    /// </summary>
    /// <param name="time">輸入秒數</param>
    /// <param name="realtimeAct">進度更新時的事件</param>
    /// <param name="finishAct">完成計時的事件</param>
    /// <returns></returns>
    public static IEnumerator Timer(float time, Action realtimeAct = null, Action finishAct = null)
    {
        float tempTime = time;
        while (time > 0)
        {
            time -= Time.deltaTime;
            realtimeAct?.Invoke();
            yield return null;
        }
        finishAct?.Invoke();
    }

    /// <summary>
    ///  偵測到達指定範圍後做指定的事
    /// </summary>
    /// <param name="tempDistance">輸入的範圍</param>
    /// <param name="distance">達成範圍</param>
    /// <param name="updateProgress">未達成範圍執行的事情</param>
    /// <param name="finishAct">完成後執行的事情</param>
    /// <returns></returns>
    public static IEnumerator DetectionRangeMethod(float tempDistance, float distance, Action updateProgress, Action finishAct)
    {
        while (tempDistance > distance)
        {
            updateProgress?.Invoke();
            Debug.Log("當前相差距離:" + tempDistance + " 目標距離: " + distance);
            yield return new WaitForEndOfFrame();
        }
        finishAct?.Invoke();
    }

    /// <summary>
    ///  偵測到達指定範圍後做指定的事
    /// </summary>
    /// <param name="tempDistance">輸入的範圍</param>
    /// <param name="distance">達成範圍</param>
    /// <param name="updateProgress">未達成範圍執行的事情</param>
    /// <param name="finishAct">完成後執行的事情</param>
    /// <returns></returns>
    public static IEnumerator DetectionRangeMethod(Vector3 selfV3, Vector3 targetV3, float distance, Action updateProgress, Action finishAct)
    {
        float tempDistance = Vector3.Distance(selfV3, targetV3);
        while (tempDistance > distance)
        {
            tempDistance = Vector3.Distance(selfV3, targetV3);
            updateProgress?.Invoke();
            Debug.Log("當前相差距離:" + tempDistance + " 目標距離: " + distance);
            yield return new WaitForEndOfFrame();
        }
        finishAct?.Invoke();
    }

    /// <summary>
    ///  偵測到達指定範圍後做指定的事
    /// </summary>
    /// <param name="tempDistance">輸入的範圍</param>
    /// <param name="distance">達成範圍</param>
    /// <param name="updateProgress">未達成範圍執行的事情</param>
    /// <param name="finishAct">完成後執行的事情</param>
    /// <returns></returns>
    public static IEnumerator DetectionRangeMethod(GameObject selfObj, GameObject targetObj, float distance, Action updateProgress, Action finishAct)
    {
        float tempDis = Vector3.Distance(selfObj.transform.position, targetObj.transform.position);
        while (tempDis > distance)
        {
            tempDis = Vector3.Distance(selfObj.transform.position, targetObj.transform.position);
            updateProgress?.Invoke();
            yield return new WaitForEndOfFrame();
        }
        finishAct?.Invoke();
    }
}
