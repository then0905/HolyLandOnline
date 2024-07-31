using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/07/29
//  創建用途: 總管理腳本
//==========================================
public class MainManager : Singleton<MainManager>
{
    protected override void Awake()
    {
        if (instance == null)
        {
            //StartCoroutine(InitSequenceManager.Instance.Init());
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // GameManager 特定的初始化代碼
    }
}
