using Com.Mayaminer;
using mayaminer.com.jxDB;
using Newtonsoft.Json;
using RCS.Models;
using RCS.Models.DB;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace RCS.Controllers
{
    public class SuppliesController : BaseController
    {
        //
        // GET: /Supplies/

        public ActionResult Index()
        {
            return View();
        }

        #region  取得耗材SQL
        public JsonResult supplies_DB()
        {
            //取得耗材SQL
            string sqlstring = "SELECT C_ID,C_CNAME,C_ENAME,VENDER_CODE,C_MEMO FROM  RCS_SYS_CONSUMABLE_LIST WHERE DATASTATUS = '1'";
            List<RCS_SYS_CONSUMABLE_LIST> list = new List<RCS_SYS_CONSUMABLE_LIST>();
            SQLProvider supplies_DBA = new SQLProvider();
            //結果
            list = supplies_DBA.DBA.getSqlDataTable<RCS_SYS_CONSUMABLE_LIST>(sqlstring);
   //         supplies_DBA.DBA.RESPONSE_MSG.status= RESPONSE_STATUS;
            return Json(list);
        }
        #endregion

        #region 新增修改耗材資料
        public JsonResult Supplies_Save(string viewstats, string suppliesID, string suppliesName, string suppliesName_en, string suppliesVenderCode, string suppliesRemark)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            Dapper.DynamicParameters item = new Dapper.DynamicParameters();
            string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string createid = user_info.user_id;
            string createname = user_info.user_name;        
            item.Add("suppliesID", suppliesID);
            item.Add("suppliesName", suppliesName);
            item.Add("suppliesName_en", suppliesName_en);
            item.Add("suppliesVenderCode", suppliesVenderCode);
            item.Add("suppliesRemark", suppliesRemark);
            item.Add("datetime", datetime);
            item.Add("createid", createid);
            item.Add("createname", createname);

            List<RCS_SYS_CONSUMABLE_LIST> list = new List<RCS_SYS_CONSUMABLE_LIST>();
            string sqlSave = @"SELECT C_ID,C_CNAME,C_ENAME,VENDER_CODE,C_MEMO,CREATE_DATE,CREATE_ID,
            CREATE_NAME,DATASTATUS FROM RCS_SYS_CONSUMABLE_LIST WHERE C_ID=@suppliesID";

            SQLProvider supplies_DBA = new SQLProvider();
            int num =0;
            list= supplies_DBA.DBA.getSqlDataTable<RCS_SYS_CONSUMABLE_LIST>(sqlSave,item);
            num = list.Count;
            if (num==0)
            {
            sqlSave = @"INSERT INTO RCS_SYS_CONSUMABLE_LIST (C_ID,C_CNAME,C_ENAME,VENDER_CODE
            ,C_MEMO,CREATE_DATE,CREATE_ID,CREATE_NAME,DATASTATUS) VALUES (@suppliesID,@suppliesName,@suppliesName_en,
            @suppliesVenderCode,@suppliesRemark,@datetime,@createid,@createname,'1')";
            }
            else
            {
                if (viewstats=="add"&& list.Exists(x=>x.DATASTATUS=="1"))
                {
                    rm.message = "耗材代碼重複";
                    rm.status = RESPONSE_STATUS.ERROR;
                    return Json(rm);
                }
            sqlSave = @"UPDATE RCS_SYS_CONSUMABLE_LIST SET C_ID=@suppliesID,C_CNAME=@suppliesName,
            C_ENAME=@suppliesName_en,VENDER_CODE=@suppliesVenderCode,C_MEMO=@suppliesRemark ,DATASTATUS='1',
            MODIFY_ID=@createid,MODIFY_NAME=@createname,MODIFY_DATE=@datetime WHERE C_ID=@suppliesID";
            }
            supplies_DBA.DBA.DBExecute(sqlSave, item);

            if (supplies_DBA.RESPONSE_MSG.status== RESPONSE_STATUS.SUCCESS)
            {
                rm.message = "耗材更新成功";
            }else
            {
                rm.message = "耗材更新失敗";
                LogTool.SaveLogMessage(supplies_DBA.RESPONSE_MSG.message, "Supplies_Save","SuppliesController");
            }
            return Json(rm);
        }
        #endregion
        
        #region 刪除耗材資料
        public JsonResult Supplies_Del(string JsonList)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            dbResultMessage dbMsg = new dbResultMessage();
            List<RCS_SYS_CONSUMABLE_LIST> list = new List<RCS_SYS_CONSUMABLE_LIST>();
            list = JsonConvert.DeserializeObject<List<RCS_SYS_CONSUMABLE_LIST>>(JsonList);
            string sqlDel = "UPDATE RCS_SYS_CONSUMABLE_LIST SET DATASTATUS = '9' WHERE C_ID = @C_ID";
            SQLProvider supplies_DBA = new SQLProvider();
            int num = 0;
            num= supplies_DBA.DBA.DBExecute<RCS_SYS_CONSUMABLE_LIST>(sqlDel, list);

            if (supplies_DBA.RESPONSE_MSG.status== RESPONSE_STATUS.SUCCESS)
            {
                rm.message = "刪除成功。";
            }else
            {
                rm.message = "刪除失敗。";
                LogTool.SaveLogMessage(supplies_DBA.RESPONSE_MSG.message, "Supplies_Del", "SuppliesController");
            }

            return Json(rm);
        }
        #endregion

        public ActionResult Supplies_package()
        {
            SuppliesViewModel vm = new SuppliesViewModel();
            string sqlCM_name = "SELECT CM_ID AS VALUE,CM_NAME AS TEXT FROM RCS_SYS_CONSUMABLE_PACKAGE_MASTER WHERE  DATASTATUS = '1'";
            SQLProvider CM_DBA = new SQLProvider() ;
            vm.package_name  = CM_DBA.DBA.getSqlDataTable<SelectListItem>(sqlCM_name);

            string sqlsupliesName = "SELECT C_ID AS VALUE,C_CNAME AS TEXT FROM RCS_SYS_CONSUMABLE_LIST WHERE DATASTATUS = '1'";
            SQLProvider C_DBA = new SQLProvider();
            vm.suplies_name = C_DBA.DBA.getSqlDataTable<SelectListItem>(sqlsupliesName);

            return View(vm);
        }

        #region 取得套餐SQL
        public JsonResult package_DB()
        {
            //取得耗材SQL
            string sqlstring = "SELECT CM_ID,C_CNAME,C_ENAME,VENDER_CODE,C_MEMO FROM  RCS_SYS_CONSUMABLE_PACKAGE_MASTER WHERE DATASTATUS = '1'";
            List<RCS_SYS_CONSUMABLE_PACKAGE_MASTER> list = new List<RCS_SYS_CONSUMABLE_PACKAGE_MASTER>();
            SQLProvider supplies_DBA = new SQLProvider();
            //結果
            list = supplies_DBA.DBA.getSqlDataTable<RCS_SYS_CONSUMABLE_PACKAGE_MASTER>(sqlstring);
            //         supplies_DBA.DBA.RESPONSE_MSG.status= RESPONSE_STATUS;
            return Json(list);
        }
        #endregion

        #region 新增修改套餐
        public JsonResult package_Save(string viewstats,string CM_name)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            Dapper.DynamicParameters item = new Dapper.DynamicParameters();
            string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string cmid = "CM"+DateTime.Now.ToString("yyyyMMddHHmmss");
            string createid = user_info.user_id;
            string createname = user_info.user_name;
            item.Add("CM_name", CM_name);
            item.Add("datetime", datetime);
            item.Add("cmid", cmid);
            item.Add("createid", createid);
            item.Add("createname", createname);

            List<RCS_SYS_CONSUMABLE_PACKAGE_MASTER> list = new List<RCS_SYS_CONSUMABLE_PACKAGE_MASTER>();
            string sqlSave = @"SELECT CM_NAME,DATASTATUS FROM RCS_SYS_CONSUMABLE_PACKAGE_MASTER WHERE CM_NAME=@CM_name AND DATASTATUS='1' ";

            SQLProvider supplies_DBA = new SQLProvider();
            int num = 0;
            list = supplies_DBA.DBA.getSqlDataTable<RCS_SYS_CONSUMABLE_PACKAGE_MASTER>(sqlSave, item);
            num = list.Count;
            if (num == 0)
            {
                sqlSave = @"INSERT INTO RCS_SYS_CONSUMABLE_PACKAGE_MASTER (CM_ID,CM_NAME
            ,CREATE_DATE,CREATE_ID,CREATE_NAME,DATASTATUS) VALUES (@cmid,@CM_name,
            @datetime,@createid,@createname,'1')";
            }
            else
            {
                    rm.message = "套餐名稱："+ CM_name + "重複";
                    rm.status = RESPONSE_STATUS.ERROR;
                    return Json(rm);
            }
            supplies_DBA.DBA.DBExecute(sqlSave, item);

            if (supplies_DBA.RESPONSE_MSG.status == RESPONSE_STATUS.SUCCESS)
            {
                rm.message = "套餐更新成功";
            }
            else
            {
                rm.message = "套餐更新失敗";
                LogTool.SaveLogMessage(supplies_DBA.RESPONSE_MSG.message, "Supplies_Save", "SuppliesController");
            }
            return Json(rm);
        }
        #endregion

        #region 刪除套餐項目
        public JsonResult package_Del(string CMid)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            dbResultMessage dbMsg = new dbResultMessage();
            Dapper.DynamicParameters item = new Dapper.DynamicParameters();
            item.Add("CMid",CMid);
            string sqlDel = "UPDATE RCS_SYS_CONSUMABLE_PACKAGE_MASTER SET DATASTATUS = '9' WHERE  CM_ID=@CMid;";
            sqlDel += "UPDATE RCS_SYS_CONSUMABLE_PACKAGE_DTL SET DATASTATUS = '9' WHERE  CM_ID=@CMid;";
            SQLProvider supplies_DBA = new SQLProvider();
            supplies_DBA.DBA.DBExecute(sqlDel, item);

            if (supplies_DBA.RESPONSE_MSG.status == RESPONSE_STATUS.SUCCESS)
            {
                if (CMid!="")
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "刪除成功。";
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "請選擇要刪除項目。";
                }
                
            }
            else
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "刪除失敗。";
                LogTool.SaveLogMessage(supplies_DBA.RESPONSE_MSG.message, "package_Del", "SuppliesController");
            }

            return Json(rm);
        }
        #endregion

        #region 新增套餐耗材內容
        public JsonResult suppliesPackage_Save(string objStr)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            objStr = HttpUtility.UrlDecode(objStr);
            dbResultMessage dbMsg = new dbResultMessage();
            CM_RCS_SYS_CONSUMABLE_PACKAGE_DTL list = new CM_RCS_SYS_CONSUMABLE_PACKAGE_DTL();
            list = JsonConvert.DeserializeObject<CM_RCS_SYS_CONSUMABLE_PACKAGE_DTL>(objStr);
            list.DETAIL.ForEach(x => { x.CM_ID = list.CM_ID; x.DATASTATUS = "1"; });
            string sqlSave = "SELECT * FROM RCS_SYS_CONSUMABLE_PACKAGE_DTL WHERE CM_ID=@CM_ID ";
            SQLProvider supplies_DBA = new SQLProvider();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            dp.Add("CM_ID",list.CM_ID);
            int num = 0;
            List<RCS_SYS_CONSUMABLE_PACKAGE_DTL> rpl = new List<RCS_SYS_CONSUMABLE_PACKAGE_DTL>();
            rpl = supplies_DBA.DBA.getSqlDataTable<RCS_SYS_CONSUMABLE_PACKAGE_DTL>(sqlSave, dp);
            num = rpl.Count;
            List<Dapper.DynamicParameters> bpl = new List<Dapper.DynamicParameters>();
            if (num == 0)
            {

                sqlSave = "INSERT INTO RCS_SYS_CONSUMABLE_PACKAGE_DTL (CM_ID,C_ID,C_QTY,DATASTATUS) VALUES(@CM_ID,@C_ID,@suplies_qty,@status) ";
                foreach (RCS_SYS_CONSUMABLE_PACKAGE_DTL item in list.DETAIL)
                {
                    dp = new Dapper.DynamicParameters();
                    dp.Add("CM_ID", item.CM_ID);
                    dp.Add("C_ID", item.C_ID);
                    dp.Add("suplies_qty", item.C_QTY);
                    dp.Add("status", item.DATASTATUS);
                    bpl.Add(dp);
                }
            }
            else
            {
                sqlSave = "DELETE RCS_SYS_CONSUMABLE_PACKAGE_DTL WHERE CM_ID = @CM_ID2 AND C_ID=@C_ID2;";
                foreach (RCS_SYS_CONSUMABLE_PACKAGE_DTL item in rpl)
                {
                    dp = new Dapper.DynamicParameters();
                    dp.Add("CM_ID2", item.CM_ID);
                    dp.Add("C_ID2", item.C_ID);
                    bpl.Add(dp);
                }
                supplies_DBA.DBA.DBExecute<Dapper.DynamicParameters>(sqlSave, bpl);
                bpl = new List<Dapper.DynamicParameters>();
                sqlSave = "INSERT INTO RCS_SYS_CONSUMABLE_PACKAGE_DTL (CM_ID,C_ID,C_QTY,DATASTATUS) VALUES(@CM_ID,@C_ID,@suplies_qty,@status) ";
                foreach (RCS_SYS_CONSUMABLE_PACKAGE_DTL item in list.DETAIL)
                {
                    dp = new Dapper.DynamicParameters();
                    dp.Add("CM_ID", item.CM_ID);
                    dp.Add("C_ID", item.C_ID);
                    dp.Add("suplies_qty", item.C_QTY);
                    dp.Add("status", item.DATASTATUS);
                    bpl.Add(dp);
                }
            }
            supplies_DBA.DBA.DBExecute<Dapper.DynamicParameters>(sqlSave, bpl);


            if (string.IsNullOrWhiteSpace(list.CM_ID) )
            {
                rm.message = "請選擇套餐。";
                rm.status = RESPONSE_STATUS.ERROR;
            }
            else { 
            if (supplies_DBA.RESPONSE_MSG.status == RESPONSE_STATUS.SUCCESS)
            {
                rm.message = "新增成功。";
            }
            else
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "新增失敗。";
                LogTool.SaveLogMessage(supplies_DBA.RESPONSE_MSG.message, "Supplies_Del", "SuppliesController");
            }
            }

            return Json(rm);

        }
        #endregion


        public ActionResult getPackage_View(string index ,string package)
        {
            
            ViewData["index"] = index;
            SuppliesViewModel vm = new SuppliesViewModel();
            string sqlCM_name = "SELECT CM_ID AS VALUE,CM_NAME AS TEXT FROM RCS_SYS_CONSUMABLE_PACKAGE_MASTER WHERE  DATASTATUS = '1'";
            SQLProvider CM_DBA = new SQLProvider();
            vm.package_name = CM_DBA.DBA.getSqlDataTable<SelectListItem>(sqlCM_name);

            string sqlsupliesName = "SELECT C_ID AS VALUE,C_CNAME AS TEXT FROM RCS_SYS_CONSUMABLE_LIST WHERE DATASTATUS = '1'";
            SQLProvider C_DBA = new SQLProvider();
            vm.suplies_name = C_DBA.DBA.getSqlDataTable<SelectListItem>(sqlsupliesName);
            Dapper.DynamicParameters item = new Dapper.DynamicParameters();
            item.Add("package", package);
           
            string sqlDTL = "SELECT * FROM RCS_SYS_CONSUMABLE_PACKAGE_DTL  WHERE DATASTATUS = '1' AND CM_ID=@package";
            SQLProvider DTL_DBA = new SQLProvider();
            vm.package_dtl = DTL_DBA.DBA.getSqlDataTable<RCS_SYS_CONSUMABLE_PACKAGE_DTL>(sqlDTL,item);

            return View("_supplies_view", vm);
        }
    }


}
