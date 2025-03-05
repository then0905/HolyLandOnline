using System.Collections;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/08/01
//  創建用途: 登入場景
//==========================================
public class LoginScene : MonoBehaviour
{
    private void Start()
    {
        InitSequenceManager.Instance.Init();
        //StartCoroutine(InitSequenceManager.Instance.Init());
        //StartCoroutine(Init());
    }

    /// <summary>
    /// 呼叫初始化
    /// </summary>
    /// <returns></returns>
    public IEnumerator Init()
    {
        yield return new WaitForSeconds(2);
        MapManager.Instance.NextMap("DevelopmentTest");
    }
}
