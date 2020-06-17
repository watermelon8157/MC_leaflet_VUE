using Newtonsoft.Json;
using RCSData.Models;
using RCS_Data;
using System.Collections.Generic;

namespace RCSData.Models
{
    public partial class WebMethod
    {
        /// <summary>取得院內床位區域清單</summary>
        /// <returns></returns>
        public List<BedArea> getHisBedAreaList(IWebServiceParam iwp)
        { 
            WS_HisBedAreaList ba = new WS_HisBedAreaList(iwp); 
            ServiceResult< BedArea> sr = HISData.getServiceResult(ba); 
            List<BedArea> dataList = new List<BedArea>();
            if (sr.datastatus == HISDataStatus.SuccessWithData)
            {
                dataList = JsonConvert.DeserializeObject<List<BedArea>>(sr.returnJson);
            }
            return dataList;
        }
    }


    /// <summary>
    /// 取得院內床位區域清單(成本中心)
    /// </summary>
    public class WS_HisBedAreaList : AwebMethod< BedArea>, IwebMethod< BedArea>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }
        public override string wsSession { get { return "RCS_WS_BASIC"; } }
        /// <summary>
        /// 取得院內床位區域清單(成本中心)
        /// </summary>
        /// <param name="pWsSession"></param>
        public WS_HisBedAreaList(IWebServiceParam pIwp)
        {
            this.iwp = pIwp; 
            this.setParam();
        }

        public void setParam()
        {
            this.ServiceResult = new ServiceResult< BedArea>(); 
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue; 
        }
    }

    public class BedArea
    {
        /// <summary> 區域代碼 </summary>
        public string area_code { set; get; }
        /// <summary> 區域名稱 </summary>
        public string area_name { set; get; }
        /// <summary> 顯示名稱 </summary>
        public string area_title { get { return string.Format("{0} {1}", area_code, area_name); } }
    }

}