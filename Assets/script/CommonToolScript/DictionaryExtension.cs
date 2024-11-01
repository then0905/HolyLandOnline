using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//==========================================
//  創建者:    家豪
//  創建日期:  2023/05/03
//  創建用途:  字典擴充方法
//==========================================
public static class DictionaryExtension
{
    /// <summary>
    /// 字典擴充方法
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TrySetValue<K, V>(this Dictionary<K, V> dictionary, K key, V value)
    {
        if (!dictionary.ContainsKey(key))
            dictionary.Add(key, value);
        else
            dictionary[key] = value;
        return true;
    }

    /// <summary>
    /// 確認清單是否有任何資料
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool CheckAnyData<T>(this List<T> list)
    {
        return list != null && list.Count > 0;
    }

    /// <summary>
    /// 確認字典是否有任何資料
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool CheckAnyData<K, V>(this Dictionary<K, V> dictionary)
    {
        return dictionary != null && dictionary.Count > 0;
    }
}
