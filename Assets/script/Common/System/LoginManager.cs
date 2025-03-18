using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
//using Photon.Pun;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//==========================================
//  創建者:家豪
//  創建日期:2024/10/28
//  創建用途: 登入管理
//==========================================
public class LoginManager : MonoBehaviour /*MonoBehaviourPunCallbacks*/
{
    [Header("登入帳號"), SerializeField] private TMP_InputField accountText;
    [Header("登入密碼"), SerializeField] private TMP_InputField passwordText;
    [Header("註冊帳號"), SerializeField] private TMP_InputField registAccountText;
    [Header("註冊密碼"), SerializeField] private TMP_InputField registPasswordText;


    [Header("登入成功物件"), SerializeField] private GameObject loginSuccessObj;
    [Header("登入成功文字"), SerializeField] private TextMeshProUGUI loginSuccessText;
    [Header("登入成功按鈕"), SerializeField] private Button loginSuccessButton;
    [Header("登入失敗物件"), SerializeField] private GameObject loginFailObj;
    [Header("登入失敗文字"), SerializeField] private TextMeshProUGUI loginFailText;
    [Header("登入失敗按鈕"), SerializeField] private Button loginFailButton;

    private const string ChooseCharacterSceneName = "ChooseCharacterScene";

    /// <summary>
    /// 登入
    /// </summary>
    public async void Login()
    {
        string account = accountText.text;
        string password = passwordText.text;
        Debug.Log("帳號: " + account);
        Debug.Log("密碼: " + password);
        //StartCoroutine(LoginRequest(account, password));

        //呼叫 登入API
        await ApiManager.ApiGetFunc<LoginAccountRequest, LoginAccountResponse>(
           new LoginAccountRequest()
           {
               Account = account,
               Password = password
           },
            (response) =>
            {
                //將回傳資料儲存
                if (response != null)
                {
                    CharacterScene.UserAccountResponseData = response;
                    loginSuccessButton.onClick.RemoveAllListeners();
                    loginSuccessButton.onClick.AddListener(() =>
                    {
                        loginSuccessText.text = response.ApiMsg;
                        CloseLoginSuccess();
                        ClickLoginSuccessButton();
                    });
                    LoginSuccess();
                }
                else
                {
                    LoginFail();
                }
            },
             (failMsg) =>
             {
                 loginFailText.text = failMsg;
                 LoginFail();
             });
    }

    /// <summary>
    /// 註冊
    /// </summary>
    public async void Regist()
    {
        string account = registAccountText.text;
        string password = registPasswordText.text;
        Debug.Log("帳號: " + account);
        Debug.Log("密碼: " + password);
        //呼叫 登入API
        await ApiManager.ApiPostFunc<RegistRequest, RegistResponse>(
           new RegistRequest()
           {
               Account = account,
               Password = password
           },
            (response) =>
            {
                //將回傳資料儲存
                if (response != null)
                {
                    loginSuccessText.text = response.ApiMsg;
                    LoginSuccess();

                }
                else
                {
                    LoginFail();
                }
            },
             (failMsg) =>
             {
                 loginFailText.text = failMsg;
                 LoginFail();
             });
    }

    /// <summary>
    /// 登入成功
    /// </summary>
    public void LoginSuccess()
    {
        loginSuccessObj.SetActive(true);
    }

    /// <summary>
    /// 關閉登入成功面板
    /// </summary>
    public void CloseLoginSuccess()
    {
        loginSuccessObj.SetActive(false);
    }

    /// <summary>
    /// 點擊登入成功面板按鈕
    /// </summary>
    public void ClickLoginSuccessButton()
    {
        SceneManager.LoadScene(ChooseCharacterSceneName);
    }


    /// <summary>
    /// 登入失敗
    /// </summary>
    public void LoginFail()
    {
        loginFailObj.SetActive(true);
    }

    /// <summary>
    /// 關閉登入失敗面板
    /// </summary>
    public void CloseLoginFail()
    {
        loginFailObj.SetActive(false);
    }
    //private IEnumerator LoginRequest(string accountInput, string passwordInput)
    //{
    //    LoginDto loginData = new LoginDto
    //    {
    //        GUID = accountInput,
    //        Password = passwordInput
    //    };
    //    string jsonData = JsonUtility.ToJson(loginData);
    //    UnityWebRequest request = new UnityWebRequest("https://localhost:7110/api/member/login", "POST");
    //    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
    //    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //    request.downloadHandler = new DownloadHandlerBuffer();
    //    request.SetRequestHeader("Content-Type", "application/json");


    //    yield return request.SendWebRequest();
    //    print(request.result);

    //    string jsonResponse = request.downloadHandler.text;


    //    Debug.Log("Received JSON: " + jsonResponse);
    //    if (jsonResponse.Contains("login_successful"))
    //    {
    //        Debug.Log("登入成功！");
    //        LoginSuccess();
    //        PhotonNetwork.AutomaticallySyncScene = true;
    //        PhotonNetwork.ConnectUsingSettings();
    //    }

    //    else if (jsonResponse.Contains("incorrect_password"))
    //    {
    //        print("\n==================" + jsonResponse + "==============\n");
    //        Debug.Log("帳號或密碼錯誤");
    //        LoginFail();
    //    }
    //    else if (jsonResponse.Contains("member_not_found"))
    //    {
    //        print("\n==================" + jsonResponse + "==============\n");
    //        Debug.Log("會員不存在");
    //        LoginFail();
    //    }
    //    else
    //    {
    //        print("\n==================" + jsonResponse + "==============\n");
    //        Debug.LogError("登入失敗: " + request.error);
    //        LoginFail();
    //    }
    //}

    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Connected to Photon");
    //    SceneManager.LoadScene("FirstScene");
    //}
    //[System.Serializable]
    //public class LoginDto
    //{
    //    public string GUID;
    //    public string Password;
    //}
}
