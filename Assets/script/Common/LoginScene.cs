using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//==========================================
//  創建者:家豪
//  創建日期:2024/08/01
//  創建用途: 登入場景
//==========================================
public class LoginScene : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(InitSequenceManager.Instance.Init());
        StartCoroutine(Init());
    }
    public IEnumerator Init()
    {
        yield return new WaitForSeconds(2);
        MapManager.Instance.NextMap("DevelopmentTest");
    }
}
