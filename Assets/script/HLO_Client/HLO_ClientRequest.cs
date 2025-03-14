using Newtonsoft.Json;
//==========================================
//  創建者:家豪
//  創建日期:2025/03/14
//  創建用途: HLO API 呼叫請求基底
//==========================================
public class HLO_ClientRequest
{
    /// <summary>
    /// API 路由
    /// </summary>
    [JsonIgnore]
    public string Path { set; get; }
}
