//==========================================
//  創建者:家豪
//  創建日期:2025/03/14
//  創建用途: 登入帳號後的回應結構
//==========================================
using System;
using System.Collections.Generic;

namespace HLO_Client
{
    public class LoginAccountResponse : HLO_ClientResponse
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 創建日期
        /// </summary>
        public DateTime CreatDateTime { get; set; }

        /// <summary>
        /// 最後登入日期
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 帳戶擁有的角色資料
        /// </summary>
        public List<CharacterDataDTO>? CharacterList { get; set; } = new List<CharacterDataDTO>();
    }
}
