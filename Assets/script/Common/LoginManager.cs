using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using System.Text;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
//==========================================
//  創建者:
//  創建日期:
//  創建用途: 
//==========================================
public class LoginManager : MonoBehaviourPunCallbacks
{
    [Header("帳號"), SerializeField] private TMP_InputField accountText;
    [Header("密碼"), SerializeField] private TMP_InputField passwordText;
    
    
    [Header("登入成功物件"), SerializeField] private GameObject loginSuccessObj;
    [Header("登入失敗物件"), SerializeField] private GameObject loginFailObj;



    public void Login()
    {
        string account = accountText.text;
        string password = passwordText.text;
        Debug.Log("帳號: "+account);
        Debug.Log("密碼: " + password);
        StartCoroutine(LoginRequest(account, password));
    }
    private IEnumerator LoginRequest(string accountInput, string passwordInput)
    {
        LoginDto loginData = new LoginDto
        {
            GUID = accountInput,
            Password = passwordInput
        };
        

        string jsonData = JsonUtility.ToJson(loginData);
        UnityWebRequest request = new UnityWebRequest("https://localhost:7110/api/member/login", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");


        yield return request.SendWebRequest();
        print(request.result);
        
        string jsonResponse = request.downloadHandler.text;
            
            
        Debug.Log("Received JSON: " + jsonResponse);
        if (jsonResponse.Contains("login_successful"))
        {
            Debug.Log("登入成功！");
            LoginSuccess();
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }

        else if (jsonResponse.Contains("incorrect_password"))
        {
            print("\n==================" + jsonResponse + "==============\n");
            Debug.Log("帳號或密碼錯誤");
            LoginFail();
        }
        else if (jsonResponse.Contains("member_not_found"))
        {
            print("\n==================" + jsonResponse + "==============\n");
            Debug.Log("會員不存在");
            LoginFail();
        }
        else
        {
            print("\n==================" + jsonResponse + "==============\n");
            Debug.LogError("登入失敗: " + request.error);
            LoginFail();
        }
        
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon");
        SceneManager.LoadScene("FirstScene");
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
        loginFailObj.SetActive(true);
    }


    [System.Serializable]
    public class LoginDto
    {
        public string GUID;
        public string Password;
    }
}
