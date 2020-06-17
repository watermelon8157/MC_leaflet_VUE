using RCS_Data;
using RCS_Data.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models
{
    /// <summary>
    /// 計算醫院分數
    /// </summary>
    public class MCSource : BaseThread
    {
        string csName = "MCSource";
        private SQLProvider _DBLink { get; set; }
        protected SQLProvider DBLink
        {
            get
            {
                if (this._DBLink == null)
                {
                    this._DBLink = new SQLProvider();
                }
                return this._DBLink;
            }
        }

        public override void RunThread()
        {
            // 取得醫院資料
            if (MvcApplication.hospList.Count == 0)
            {
                this.gethospList();
            }
            // 計算分數相關變數 
            // 𝑺𝒄𝒐𝒓𝒆_𝒊𝒋=  (𝒙_𝒊𝒋) + ( 𝒚_𝒊𝒋 (𝟏−𝒛_𝒊𝒋 )+ 𝒚_𝒊𝒋 𝒘𝟐_𝒊𝒋 ) 

            // 計算 MC_HOSP_INFO_DTL
            // (  𝒚_𝒊𝒋 (𝟏−𝒛_𝒊𝒋 )+ 𝒚_𝒊𝒋 𝒘𝟐_𝒊𝒋 ) 

            // 計算 DB_MC_SITE_DRIVING_TIME_INFO
            //  (𝒙_𝒊𝒋) 

            // 計算 DB_MC_SOURCE_LIST
            //  𝑺𝒄𝒐𝒓𝒆_𝒊𝒋


        }

        private void gethospList()
        {
            string actionName = "gethospList";
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_HOSP_INFO + " WHERE DATASTATUS = '1';";
            MvcApplication.hospList = this.DBLink.DBA.getSqlDataTable<DB_MC_HOSP_INFO>(sql);
            if (this.DBLink.DBA.hasLastError)
            {
                MvcApplication.hospList = new List<DB_MC_HOSP_INFO>();
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }
        }

        /// <summary>
        /// 計算CV
        /// </summary>
        /// <param name="pDate">計算日期</param>
        public void runCV(DateTime pDate)
        {
            //檢查是否有計算過資料

        }

        // 𝑺𝒄𝒐𝒓𝒆_𝒊𝒋=𝒙_𝒊𝒋+𝒚_𝒊𝒋 (𝟏−𝒛_𝒊𝒋 )+ 𝒚_𝒊𝒋 𝒘𝟐_𝒊𝒋  
        // 𝑥_𝑖𝑗="Drive−Time"
        // 依據Google Maps- Distance Matrix API，計算從災害地點到每一家醫院的開車時間。
        // 將開車時間排序，並給予序號，如25, 27, 27,  27, 30, 30 >>> 1, 3, 3, 3, 6, 6 ，詳見p.4
        // 𝑦_𝑖𝑗 "(" 𝐴𝑑𝑒𝑞𝑢𝑎𝑐𝑦)=𝑤𝑒𝑖𝑔ℎ𝑡 𝑣𝑎𝑙𝑢𝑒 𝑖𝑛 𝑚𝑎𝑡𝑟𝑖𝑥 
        // 醫院對該傷患的醫療能力適當性，按照矩陣表對照數據
        // 1 ~ 10 represent hospital level, 1表示有急診病床且非ERH
        // 以1, 2,6,10 代表各程度傷勢對應各級醫院的適當性，
        // 𝑧_𝑖𝑗=(𝑤1_𝑖𝑗∗𝑅𝑒𝑐𝑒𝑖𝑣𝑒𝑑 𝐶𝑎𝑠𝑢𝑎𝑙𝑡𝑖𝑒𝑠(𝑙𝑜𝑎𝑑𝑖𝑛𝑔))/(ED B𝑒𝑑𝑠) ，詳見p.4
        // 重傷
        // 依據𝑐𝑟𝑖𝑡𝑖𝑐𝑎𝑙 ED 𝐵𝑒𝑑𝑠及Received Casualties變動
        // 𝑐𝑟𝑖𝑡𝑖𝑐𝑎𝑙 ED 𝐵𝑒𝑑𝑠臨界值=  ED B𝑒𝑑𝑠_𝑖𝑗/(𝑄1(=25))∗4
        // 全台重度急救責任醫院急診觀察床取Q1
        // 中傷或輕傷:𝑤1_𝑖𝑗 =2
        // 𝑤2_𝑖𝑗=內外科人力/急重症相關病床 < 1，詳見p.5


        #region 𝑺𝒄𝒐𝒓𝒆_𝒊𝒋=𝒙_𝒊𝒋+𝒚_𝒊𝒋 (𝟏−𝒛_𝒊𝒋 )+ 𝒚_𝒊𝒋 𝒘𝟐_𝒊𝒋  
        // 𝑺𝒄𝒐𝒓𝒆_𝒊𝒋=𝒙_𝒊𝒋+𝒚_𝒊𝒋 (𝟏−𝒛_𝒊𝒋 )+ 𝒚_𝒊𝒋 𝒘𝟐_𝒊𝒋  

        #region 𝑥_𝑖𝑗
        /// <summary> 
        /// 𝑥_𝑖𝑗 = "Drive−Time" 公式
        /// </summary>
        /// <param name="drivingTime">花費時間</param>
        /// <param name="maxDrinvingTime">花費時間最大值</param>
        /// <returns></returns>
        public int time_ordinal(double drivingTime, double maxDrinvingTime)
        {
            int x = 0;
            // (maxDrinvingTime+1)-drivingTime

            return x;
        }
        #endregion



        #endregion





    }



}