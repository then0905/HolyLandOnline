using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using HLO_Client;
//==========================================
//  創建者:家豪
//  創建日期:2025/03/14
//  創建用途:API 管理器
//==========================================
public class ApiManager : MonoBehaviour
{
    //宣告Http需求
    private static readonly HttpClient httpClient = new HttpClient();
    //後端url
    private const string backendurl = "https://hlobackend-production.up.railway.app/api";

    /// <summary>
    /// API POST 請求方法
    /// </summary>
    /// <typeparam name="T1">需求結構</typeparam>
    /// <typeparam name="T2">回傳結構</typeparam>
    /// <param name="request">發送需求資料</param>
    /// <param name="successAct">資料成功回傳後 執行的內容</param>
    /// <param name="failAct">API 失敗時執行的內容 (通常帶入錯誤訊息檢視)</param>
    /// <param name="enableBlock">是否啟動全屏遮罩</param>
    /// <returns></returns>
    public static async Task<bool> ApiPostFunc<T1, T2>(T1 request, Action<T2> successAct, Action<string> failAct = null, bool enableBlock = true)
        where T1 : HLO_ClientRequest
        where T2 : HLO_ClientResponse
    {
        //呼叫等待讀取視窗
        int key = CommonFunction.CallLoadingWindowSetting();
        try
        {
            // 準備請求URL
            string url = $"{backendurl}{request.Path}";

            // 將請求對象序列化為JSON
            string jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // 發送POST請求
            HttpResponseMessage response = await httpClient.PostAsync(url, content);

            // 檢查請求是否成功
            if (response.IsSuccessStatusCode)
            {
                // 讀取並反序列化響應
                string jsonResponse = await response.Content.ReadAsStringAsync();
                T2 responseObject = JsonConvert.DeserializeObject<T2>(jsonResponse);

                // 執行成功回調
                successAct?.Invoke(responseObject);

                if (enableBlock)
                    //關閉等待讀取視窗
                    CommonFunction.CloseLoadingWindowSetting(key);
                return true;
            }
            else
            {
                // 執行失敗回調
                CommonFunction.MessageHint($"API請求失敗，狀態碼: {await response.Content.ReadAsStringAsync()}", HintType.Warning);
                failAct?.Invoke(await response.Content.ReadAsStringAsync());

                if (enableBlock)
                    //關閉等待讀取視窗
                    CommonFunction.CloseLoadingWindowSetting(key);
                return false;
            }
        }
        catch (Exception ex)
        {
            // 處理任何異常
            Debug.LogException(ex);
            failAct?.Invoke(ex.ToString());

            if (enableBlock)
                //關閉等待讀取視窗
                CommonFunction.CloseLoadingWindowSetting(key);
            return false;
        }
    }

    /// <summary>
    /// API GET 請求方法
    /// </summary>
    /// <typeparam name="T1">需求結構</typeparam>
    /// <typeparam name="T2">回傳結構</typeparam>
    /// <param name="request">發送需求資料</param>
    /// <param name="successAct">資料成功回傳後 執行的內容</param>
    /// <param name="failAct">API 失敗時執行的內容 (通常帶入錯誤訊息檢視)</param> 
    /// <param name="enableBlock">是否啟動全屏遮罩</param>
    /// <returns></returns>
    public static async Task<bool> ApiGetFunc<T1, T2>(T1 request, Action<T2> successAct, Action<string> failAct = null, bool enableBlock = true)
        where T1 : HLO_ClientRequest
        where T2 : HLO_ClientResponse
    {
        //呼叫等待視窗
        int key = CommonFunction.CallLoadingWindowSetting();
        try
        {
            // 構建 URL + Query String 參數
            string url = $"{backendurl}{request.Path}?{ToQueryString(request)}";

            // 發送 GET 請求（移除 content）
            HttpResponseMessage response = await httpClient.GetAsync(url);

            // 檢查請求是否成功
            if (response.IsSuccessStatusCode)
            {
                // 讀取並反序列化響應
                string jsonResponse = await response.Content.ReadAsStringAsync();
                T2 responseObject = JsonConvert.DeserializeObject<T2>(jsonResponse);
                // 執行成功回調
                successAct?.Invoke(responseObject);

                if (enableBlock)
                    //關閉等待讀取視窗
                    CommonFunction.CloseLoadingWindowSetting(key);
                return true;
            }
            else
            {
                // 執行失敗回調
                CommonFunction.MessageHint($"API請求失敗，狀態碼: {await response.Content.ReadAsStringAsync()}", HintType.Warning);
                failAct?.Invoke(await response.Content.ReadAsStringAsync());

                if (enableBlock)
                    //關閉等待讀取視窗
                    CommonFunction.CloseLoadingWindowSetting(key);
                return false;
            }
        }
        catch (Exception ex)
        {
            // 處理任何異常
            Debug.LogException(ex);

            if (enableBlock)
                //關閉等待讀取視窗
                CommonFunction.CloseLoadingWindowSetting(key);
            //failAct?.Invoke();
            return false;
        }
    }

    private static string ToQueryString<T>(T obj)
    {
        var properties = from p in typeof(T).GetProperties()
                         where p.GetValue(obj, null) != null
                         select $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.GetValue(obj, null).ToString())}";

        return string.Join("&", properties);
    }

    //public async Task<bool> GetAsyncFuncion<T1,T2>(T1 request, Action<T2> successAct, Action failAct = null)
    //    where T1 : HLO_ClientRequest
    //    where T2 : HLO_ClientResponse
    //{
    //    Func<Task<bool>> asyncResult = async () =>
    //    {
    //        var response = await SendPostAsync<T1, T2>(request);

    //        var result = false;

    //        if (response != null && response.IsSuccess)
    //        {
    //            successAct?.Invoke(response);
    //            result = true;
    //        }
    //        else
    //        {
    //            if (response == null)
    //                Debug.LogErrorFormat("{0} Get Null...", request.Path);
    //            else
    //                Debug.LogErrorFormat("{0} Not Success...", request.Path);

    //            HintFrame.TriggerMessagePopup_ForID("TM_BearClientFail");
    //            failAct?.Invoke();
    //        }

    //        return result;
    //    };
    //    return asyncResult;
    //    using (UnityWebRequest request = UnityWebRequest.Get($"{url}/Players/{playerId}"))
    //    {
    //        return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.Success)
    //        {
    //            Player player = JsonUtility.FromJson<Player>(request.downloadHandler.text);
    //            callback(player);
    //        }
    //        else
    //        {
    //            Debug.LogError($"Error: {request.error}");
    //            callback(null);
    //        }
    //    }
    //}

    //public static async Task<O> SendPostAsync<I, O>(I Input, Action FailCallBack = null, bool IsReloadScene = true)
    // where I : HLO_ClientRequest
    // where O : HLO_ClientResponse
    //{
    //    PrintNormalAPILog(null, Input.Path);
    //    StartAPITimer(Input.Path, timeCacheDic);

    //    try
    //    {
    //        HTTPRequest request = new HTTPRequest(new Uri(BearClientConfig.serverPath + Input.Path), HTTPMethods.Post);
    //        request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Input));
    //        request.SetHeader("Content-Type", "application/json; charset=UTF-8");

    //        StartAPITimer(Input.Path, timeCacheDic_OnlyHttp);
    //        await request.Send();
    //        StopAPITimer(Input.Path, timeCacheDic_OnlyHttp);
    //        PrintAPITimerLog(timeCacheDic_OnlyHttp, "_OnlyHttp");

    //        if (request.State == HTTPRequestStates.TimedOut || request.State == HTTPRequestStates.ConnectionTimedOut || request.State == HTTPRequestStates.Error)
    //        {
    //            Debug.LogErrorFormat("{0} Exception : {1}", Input.Path, request.State);
    //            //Debug.LogWarning(string.Format("Exception Request  : {0}", Input.Path));
    //            //Debug.LogWarning(string.Format("Exception SendPostAsync : {0}", request.State));

    //            FailAction(IsReloadScene, FailCallBack, string.Format("Exception SendPostAsync : {0}", request.State));
    //        }

    //        if (request.Response.StatusCode == 403)
    //        {
    //            Debug.LogErrorFormat("{0} Exception : {1}", Input.Path, request.Response.StatusCode);
    //            //Debug.LogWarning(string.Format("Exception SendPostAsync : {0}", request.Response.StatusCode));

    //            CommonAppController.Instance.GetComponent<CommonAppController>().ResumeConnect();
    //            FailAction(IsReloadScene, FailCallBack, string.Format("Exception SendPostAsync : {0}", request.Response.StatusCode));
    //        }

    //        O o = JsonConvert.DeserializeObject<O>(request.Response.DataAsText);

    //        StopAPITimer(Input.Path, timeCacheDic);
    //        PrintNormalAPILog(null, Input.Path, timeCacheDic);
    //        PrintAPITimerLog(timeCacheDic);

    //        return o;
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogErrorFormat("{0} Exception : {1}", Input.Path, ex.Message);
    //        //Debug.LogWarning(string.Format("Exception Request  : {0}", Input.Path));
    //        //Debug.LogWarning(string.Format("Exception SendPostAsync : {0}", ex.Message));

    //        FailAction(IsReloadScene, FailCallBack, "[" + Input.Path.Replace("/Authenticate/", string.Empty) + " ]\n\n" + ex.Message);
    //    }

    //    return null;
    //}
}
