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
}
