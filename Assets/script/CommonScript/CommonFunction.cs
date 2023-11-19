using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using JsonDataModel;
using Newtonsoft.Json;
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
    /// 輸入字典資料方法
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="name"></param>
    /// <param name="path"></param>
    public static void InitData<I, K>(Dictionary<I, K> dictionary, string name, string path)
        where I : IConvertible
        where K : IDictionaryData<I>
    {
        var list = DeserializeJson<K>(path, name);
        for (int i = 0; i < list.Count; i++)
        {
            var t = list[i];
            dictionary.TrySetValue(t.GetKey, t);
        }
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
    /// 呼叫系統訊息
    /// </summary>
    /// <param name="content"></param>
    public static void MessageHint(string content)
    {
       MessageHintSetting message =  LoadObject<MessageHintSetting>("Hint", "MessageHint").GetComponent<MessageHintSetting>();
        message.CallHintCanvas(content);
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
        var valueList = value.Split(',').ToList();
        valueList.ForEach(x => tempList.Add(float.Parse(x)));
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
        var valueList = value.Split(',').ToList();
        valueList.ForEach(x => tempList.Add(int.Parse(x)));
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
        tempList = value.Split(',').ToList();
        return tempList;
    }

}
