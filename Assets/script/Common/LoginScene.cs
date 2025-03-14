using System.Collections;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/08/01
//  創建用途: 登入場景
//==========================================
public class LoginScene : MonoBehaviour
{
    public static UserAccountResponse UserAccountResponseData = null;

    private void Start()
    {
        //InitSequenceManager.Instance.Init();
        //StartCoroutine(InitSequenceManager.Instance.Init());
        //StartCoroutine(Init());

        if (UserAccountResponseData.CharacterList.CheckAnyData())
            SetChooseCharacterWindow();
        else
            SetCreateCharacterWindow();
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

    /// <summary>
    /// 設定創造角色畫面
    /// </summary>
    private void SetCreateCharacterWindow()
    {
        print("呼叫創角");
    }

    /// <summary>
    /// 設定選擇角色畫面
    /// </summary>
    private void SetChooseCharacterWindow()
    {
        print("呼叫選角");
    }
}
