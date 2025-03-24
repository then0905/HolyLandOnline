//==========================================
//  創建者:家豪
//  創建日期:2025/03/16
//  創建用途: 角色的背包(傳輸層用)
//==========================================

using System;

namespace HLO_Client
{
    public class InventoryDataDTO
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public string CodeId { get; set; }

        /// <summary>
        /// 物品類別ID
        /// </summary>
        public string ClassificationID { get; set; }

        /// <summary>
        ///  唯一物品識別碼（只對不可堆疊物品有效）
        /// </summary>
        public Guid? UniqueItemId { get; set; }

        /// <summary>
        /// 物品強化等級 (武器、防具適用)
        /// </summary>
        public int ForgeLv { get; set; }

        /// <summary>
        /// 穿戴部位 (在背包的話填Bag，若在裝備欄上則帶入對應裝備欄部位)
        /// </summary>
        public string WearingPart { get; set; }

        /// <summary>
        /// 物品數量（只對可堆疊物品有效）
        /// </summary>
        public int Qty { get; set; } = 1;

        /// <summary>
        /// 物品產生的時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 軟刪除標誌
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
