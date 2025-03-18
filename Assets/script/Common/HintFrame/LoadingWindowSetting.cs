using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2025/03/18
//  創建用途: Loading等待視窗
//==========================================
public class LoadingWindowSetting : MonoBehaviour
{
    public static LoadingWindowSetting Instance;

    //紀錄鑰匙數量(呼叫等待讀取視窗的次數)
    public static int Key;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 呼叫讀取視窗
    /// </summary>
    public int CallLoadingWindow()
    {
        //呼叫一次 產生一把鑰匙
        Key += 1;
        return 1;
    }


    /// <summary>
    /// 關閉讀取視窗
    /// </summary>
    /// <param name="keyQty">此次歸還的鑰匙數量</param>
    public void CloseLoadingWindow(int keyQty)
    {
        //扣掉歸還的鑰匙
        Key -= keyQty;

        //若鑰匙已全數歸還 刪除物件
        if (Key.Equals(0))
        {
            Destroy(Instance.gameObject);
        }
    }
}
