
//==========================================
//  創建者:家豪
//  創建日期:2025/03/14
//  創建用途: HLO API 回應基底
//==========================================
namespace HLO_Client
{
    public class HLO_ClientResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 傳輸訊息
        /// </summary>
        public string ApiMsg { get; set; }
    }
}
