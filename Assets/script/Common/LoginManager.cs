using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//==========================================
//  創建者:
//  創建日期:
//  創建用途: 
//==========================================
public class LoginManager : MonoBehaviour
{
    [Header("帳號"), SerializeField] private TMP_InputField accountText;
    [Header("密碼"), SerializeField] private TMP_InputField passwordText;
    
    
    [Header("登入失敗物件"), SerializeField] private GameObject loginSuccessObj;
    [Header("登入成功物件"), SerializeField] private GameObject loginFailObj;



    public void Login()
    {
        Debug.Log("帳號 : " + accountText.text + " & 密碼 : " + passwordText.text);
    }

    /// <summary>
    /// 登入成功
    /// </summary>
    public void LoginSuccess()
    {
        loginSuccessObj.SetActive(true);
    }


    /// <summary>
    /// 登入失敗
    /// </summary>
    public void LoginFail()
    {
        loginFailObj.SetActive(false);
    }
}
