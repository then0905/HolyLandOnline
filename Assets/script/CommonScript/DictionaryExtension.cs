using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//==========================================
//  �Ыت�:    �a��
//  �Ыؤ��:  2023/05/03
//  �Ыإγ~:  �r���X�R��k
//==========================================
public static class DictionaryExtension
{
    /// <summary>
    /// �r���X�R��k
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
