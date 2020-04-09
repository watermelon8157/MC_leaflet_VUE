using Com.Mayaminer;
using Newtonsoft.Json;
using RCS.Models;
using RCS.Models.DB;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using mayaminer.com.library;
using mayaminer.com.jxDB;
using System.Reflection;
using Dapper;
using RCSData.Models;
using RCS_Data.Models.ViewModels;

namespace RCS.Controllers
{
    public class SystemManageController : BaseController
    {
        private SystemManage SystemManageModel { get; set; }
        public SystemManageController()
        {
            SystemManageModel = new SystemManage();
        }

        //主頁面
        public ActionResult Index()
        {
            ViewData["user_info"] = user_info;
            //測試撈取HIS WebService用
            if (user_info.user_id == "mayaAdmin")
                user_info.authority = "maya";
            return View();
        }

        #region 呼吸器維護
        /// <summary>取得呼吸器類型</summary>
        /// <returns>string</returns>
        private void getDeviceModel()
        {
            List<SelectListItem> ModelList = new List<SelectListItem>();

            DataTable dt = this.SystemManageModel.getDeviceType();
            if (BaseModel.DTNotNullAndEmpty(dt))
            {
                ModelList.Add(new SelectListItem { Text = "請選擇", Value = "0" });
                foreach (DataRow ro in dt.Rows)
                {
                    ModelList.Add(new SelectListItem { Text = ro["P_NAME"].ToString(), Value = ro["P_VALUE"].ToString() });
                }
            }
            ViewBag.ModelList = ModelList;
        }

        /// <summary>呼吸器維護 </summary>
        /// <returns></returns>
        public ActionResult Index_device()
        {
            DeviceModel vm = new DeviceModel();
            try
            {
                List<SelectListItem> ModelList = new List<SelectListItem>();
                DataTable dt = this.SystemManageModel.getDeviceType();
                vm.modeltypelist = setDropDownListData(dt);
                dt = BaseModel.getRCS_SYS_PARAMS_DT(@pModel: "Index_device", @pGroup: "system_test", pStatus: "1");
                vm.systemtestlist = setDropDownListData(dt);
                dt = BaseModel.getRCS_SYS_PARAMS_DT(@pModel: "Index_device", @pGroup: "device_status", pStatus: "1");
                vm.statuslist = setDropDownListData(dt);
                dt = BaseModel.getRCS_SYS_PARAMS_DT(@pModel: "Index_device", @pGroup: "clean_status", pStatus: "1");
                vm.cleanstatuslist = setDropDownListData(dt);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name,
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }

            ViewBag.OpenRights = user_info.authority == "RT_admin" || user_info.user_id == "maya" || user_info.authority == "admin";
            return View(vm);
        }
        private List<SelectListItem> setDropDownListData(DataTable pDT)
        {
            List<SelectListItem> rc = new List<SelectListItem>();
            if (BaseModel.DTNotNullAndEmpty(pDT))
            {
                rc.Add(new SelectListItem { Text = "請選擇", Value = "0" });
                foreach (DataRow ro in pDT.Rows)
                {
                    rc.Add(new SelectListItem { Text = ro["P_NAME"].ToString(), Value = ro["P_VALUE"].ToString() });
                }
            }
            return rc;
        }
        public ActionResult getDeviceList(bool pShowDel)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.message = "";
            rm.status = RESPONSE_STATUS.ERROR;
            List<DeviceMaster> list = null;
            try
            {
                list = SystemManageModel.getDeviceList(pShowDel);
                if (list.Count > 0)
                {
                    rm.attachment = list;
                    rm.status = RESPONSE_STATUS.SUCCESS;
                }
                else
                {
                    rm.attachment = new  List<DeviceMaster>();
                    rm.message = "查無資料";
                }
            }
            catch (Exception ex)
            {
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name,
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return Json(rm, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getDevice(bool pShowDel, string pDeviceNo)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.message = "";
            rm.status = RESPONSE_STATUS.ERROR;
            try
            {
                List<DeviceMaster> list = SystemManageModel.getDeviceList(pShowDel, pDeviceNo);
                if (list.Count > 0)
                {
                    rm.attachment = list[0];
                    rm.status = RESPONSE_STATUS.SUCCESS;
                }
                else
                {
                    rm.message = "查無資料";
                } 
            }
            catch (Exception ex)
            {
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name,
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return Json(rm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 取得檢核紀錄及保養紀錄清單
        /// </summary>
        /// <param name="pDeviceNo"></param>
        /// <param name="pSdate"></param>
        /// <param name="pEdate"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public ActionResult getDetailList(List<string> pDeviceNo, string pSdate, string pEdate, string pType)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.status = RESPONSE_STATUS.ERROR;
            try
            {
                if (pDeviceNo == null || pDeviceNo.Count == 0)
                {
                    rm.message = "請選擇至少一筆資料";
                }
                else
                {
                    if (pType == "checklist")
                    {
                        List<DeviceChecklist> list = SystemManageModel.getVRChecklist(pDeviceNo, pSdate, pEdate);
                        rm.attachment = list;
                    }
                    if (pType == "maintain")
                    {
                        List<DeviceDetail> list = SystemManageModel.getMaintainList(pDeviceNo, pSdate, pEdate);
                        rm.attachment = list;
                    }
                    rm.message = SystemManageModel.LastError;
                    rm.status = rm.message != "" ? RESPONSE_STATUS.ERROR : RESPONSE_STATUS.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name,
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return Json(rm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增修改刪除Device
        /// </summary>
        /// <param name="pOriDeviceNo"></param>
        /// <param name="pDeviceNo"></param>
        /// <param name="pModel"></param>
        /// <param name="pBuyDate"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public ActionResult DeviceSave(string pOriDeviceNo, List<string> pDeviceNo, string pModel, string pBuyDate, string pType)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.status = RESPONSE_STATUS.ERROR;
            string recordStr = "CREATE";
            string modeStr = "新增";
            DataTable dt = null;
            DataRow dr = null;
            try
            {
                ErrStrBuilder errBuilder = new ErrStrBuilder();
                if (pType == "delete" || pType == "restore")
                {
                    bool restore = pType == "restore";
                    modeStr = (restore ? "恢復" : "") + "停用儀器";
                    recordStr = "MODIFY";
                    if (pDeviceNo.Count == 0)
                    {
                        errBuilder.Add("請選擇至少一筆資料");
                    }
                    else
                    {
                        dt = SystemManageModel.getDeviceDT(pDeviceNo, restore);
                        if (BaseModel.DTNotNullAndEmpty(dt))
                        {
                            foreach (DataRow ro in dt.Rows)
                            {
                                ro["USE_STATUS"] = (restore ? "Y" : "N");
                                if (!restore) ro["ROOM"] = "";
                            }
                        }
                    }
                }
                else
                {
                    string newDeviceNo = (pDeviceNo.Count > 0) ? pDeviceNo[0] : "";
                    if (string.IsNullOrEmpty(newDeviceNo)) errBuilder.Add("請輸入財產編號");
                    if (string.IsNullOrEmpty(pModel) || pModel == "0") errBuilder.Add("請選擇呼吸器類別");
                    if (!errBuilder.HasError)
                    {
                        if (pType == "new")
                        {//新增
                            dt = SystemManageModel.getDeviceDT(pDeviceNo, true);
                            if (!string.IsNullOrEmpty(newDeviceNo) && BaseModel.DTNotNullAndEmpty(dt))
                            {
                                if(dt.Rows[0]["USE_STATUS"].ToString() == "N")
                                {
                                    dr = dt.Rows[0];
                                    dr["USE_STATUS"] = "Y";
                                    dr["MODIFY_ID"] = user_info.user_id;
                                    dr["MODIFY_NAME"] = user_info.user_name;
                                    dr["MODIFY_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                else
                                {
                                    errBuilder.Add("財產編號重複");
                                }
                            }
                            else
                            {
                                dt = SystemManageModel.getNewDeviceDT();
                                dr = dt.NewRow();
                                dr["DEVICE_NO"] = pDeviceNo[0];
                                dt.Rows.Add(dr);
                            }
                        }
                        else
                        {//修改
                            if (pOriDeviceNo.Trim() != newDeviceNo.Trim())
                            {
                                dt = SystemManageModel.getDeviceDT(pDeviceNo, true);
                                if (!string.IsNullOrEmpty(newDeviceNo) && BaseModel.DTNotNullAndEmpty(dt))
                                {
                                    errBuilder.Add("財產編號已存在，不可重複");
                                }
                            }
                            if (!errBuilder.HasError)
                            {
                                dt = SystemManageModel.getDeviceDT(new List<string> { pOriDeviceNo });
                                modeStr = "修改";
                                recordStr = "MODIFY";
                                if (BaseModel.DTNotNullAndEmpty(dt))
                                {
                                    dr = dt.Rows[0];
                                }
                                else
                                {
                                    errBuilder.Add("取得資料失敗");
                                    if (this.DBA.LastError != "")
                                    {
                                        errBuilder.Add(this.DBA.LastError);
                                        LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name,
                                            MethodInfo.GetCurrentMethod().DeclaringType.Name);
                                    }
                                }
                            }
                        }
                        if (!errBuilder.HasError)
                        {
                            dr["DEVICE_SEQ"] = pModel + "-" + pDeviceNo[0];
                            dr["DEVICE_MODEL"] = pModel;
                            dr["DEVICE_NO"] = pDeviceNo[0];
                            dr["USE_STATUS"] = "Y";
                            dr["PURCHASE_DATE"] = pBuyDate;
                            dr[recordStr + "_ID"] = user_info.user_id;
                            dr[recordStr + "_NAME"] = user_info.user_name;
                            dr[recordStr + "_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
                if (!errBuilder.HasError)
                {
                    dt.TableName = GetTableName.RCS_VENTILATOR_SETTINGS.ToString();
                    if (this.DBA.UpdateResult(dt).State == enmDBResultState.Success)
                    {
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = modeStr + "成功";
                    }
                    else
                    {
                        rm.message = modeStr + "失敗";
                    }
                    if (this.DBA.LastError != "")
                    {
                        rm.message += this.DBA.LastError;
                        LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
                    }
                }
                else
                {
                    rm.message = errBuilder.errStr;
                }
            }
            catch(Exception ex)
            {
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name,
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }

            return Json(rm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 列印月報表
        /// </summary>
        /// <param name="pYearMonth">起始年月</param>
        /// <param name="pYearMonth">起訖年月</param>
        /// <returns></returns>
        public ActionResult ExportMonthlyStatisticsExcel(string pYearMonth, string pEndYearMonth)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.status = RESPONSE_STATUS.ERROR;
            if (string.IsNullOrEmpty(pYearMonth))
            {
                rm.message = "請選擇月份";
                return Json(rm);
            }
            else
            {
                DataTable dt = SystemManageModel.getMaintainDT(pYearMonth, pEndYearMonth);
                try
                {
                    ExcelSetting em = new ExcelSetting();
                    em.titleName = pYearMonth + "月至" + pEndYearMonth + "月統計表";
                    em.sheetName = "月統計表";
                    em.FileName = pYearMonth + "月至" + pEndYearMonth + "月統計表.xls";
                    exportFile exportFile = new exportFile( em.FileName);
                    byte[] buf = exportFile.exportExcel(dt, em);
                    return new FileContentResult(buf, exportFile.fileContentType)
                    {
                        FileDownloadName = exportFile.FileDownloadName
                    };
                }
                catch (Exception ex)
                {
                    rm.message = "系統發生錯誤" + ex.Message;
                    LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, GetLogToolCS.RTController);
                }
            }
            return Json(rm);
        }
        /// <summary>
        /// 取得保養紀錄單資料
        /// </summary>
        /// <param name="pDeviceNo"></param>
        /// <param name="pCreateDate"></param>
        /// <returns></returns>
        public ActionResult getMaintain(string pDeviceNo, string pCreateDate)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.message = "";
            rm.status = RESPONSE_STATUS.ERROR;
            try
            {
                if (!string.IsNullOrEmpty(pDeviceNo) && !string.IsNullOrEmpty(pCreateDate))
                {
                    DataTable dt = SystemManageModel.getMaintainDT(pDeviceNo, new List<string> { pCreateDate });
                    if (BaseModel.DTNotNullAndEmpty(dt))
                    {
                        rm.attachment = new DeviceDetail()
                        {
                            DEVICE_NO = BaseModel.checkDataColumn(dt.Rows[0], "DEVICE_NO"),
                            START_DATE = BaseModel.checkDataColumn(dt.Rows[0], "START_DATE"),
                            END_DATE = BaseModel.checkDataColumn(dt.Rows[0], "END_DATE"),
                            STATUS = BaseModel.checkDataColumn(dt.Rows[0], "STATUS"),
                            RUNNING_HR = BaseModel.checkDataColumn(dt.Rows[0], "RUNNING_HR"),
                            SYSTEM_TEST = BaseModel.checkDataColumn(dt.Rows[0], "SYSTEM_TEST"),
                            CLEAN_STATUS = BaseModel.checkDataColumn(dt.Rows[0], "CLEAN_STATUS"),
                            REMARK = BaseModel.checkDataColumn(dt.Rows[0], "REMARK"),
                            CREATE_DATE = BaseModel.checkDataColumn(dt.Rows[0], "CREATE_DATE"),
                            CREATE_NAME = BaseModel.checkDataColumn(dt.Rows[0], "CREATE_NAME"),
                            CREATE_ID = BaseModel.checkDataColumn(dt.Rows[0], "CREATE_ID"),
                            MODIFY_DATE = BaseModel.checkDataColumn(dt.Rows[0], "MODIFY_DATE"),
                            MODIFY_NAME = BaseModel.checkDataColumn(dt.Rows[0], "MODIFY_NAME"),
                            MODIFY_ID = BaseModel.checkDataColumn(dt.Rows[0], "MODIFY_ID")
                        };
                        rm.status = RESPONSE_STATUS.SUCCESS;
                    }
                    else
                    {
                        rm.message = "查無資料";
                    }
                }
            }
            catch (Exception ex)
            {
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name,
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return Json(rm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增修改保養紀錄
        /// </summary>
        /// <param name="pMaintainData"></param>
        /// <param name="pIsNew"></param>
        /// <returns></returns>
        public ActionResult MaintainSave(string pMaintainData, bool pIsNew)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.status = RESPONSE_STATUS.ERROR;
            if (string.IsNullOrEmpty(pMaintainData))
            {
                rm.message = "取得資料失敗";
            }
            else
            {
                DeviceDetail MaintainData = JsonConvert.DeserializeObject<DeviceDetail>(pMaintainData);
                string recordStr = "CREATE";
                string modeStr = "新增";

                DataTable dt = null;
                DataRow dr = null;

                ErrStrBuilder errBuilder = new ErrStrBuilder();
                try
                {
                    if (!string.IsNullOrEmpty(MaintainData.START_DATE) && !DateHelper.isDate(MaintainData.START_DATE, "yyyy-MM-dd"))
                    {
                        errBuilder.Add("開始日期格式錯誤");
                    }
                    if (!string.IsNullOrEmpty(MaintainData.END_DATE) && !DateHelper.isDate(MaintainData.END_DATE, "yyyy-MM-dd"))
                    {
                        errBuilder.Add("結束日期格式錯誤");
                    }
                    if (!errBuilder.HasError)
                    {
                        if (!string.IsNullOrEmpty(MaintainData.START_DATE) && !string.IsNullOrEmpty(MaintainData.END_DATE) &&
                            DateTime.Parse(MaintainData.START_DATE) > DateTime.Parse(MaintainData.END_DATE))
                        {
                            errBuilder.Add("結束日期不可小於開始日期");
                        }
                    }
                    if (!string.IsNullOrEmpty(MaintainData.RUNNING_HR) && !NumberHelper.isNumber(MaintainData.RUNNING_HR))
                    {
                        errBuilder.Add("運轉時數請輸入數字");
                    }
                    if (!errBuilder.HasError)
                    {
                        if (pIsNew)
                        {//新增
                            dt = SystemManageModel.getNewMaintainDT();
                            dr = dt.NewRow();
                            dr["DEVICE_NO"] = MaintainData.DEVICE_NO;
                            dt.Rows.Add(dr);
                        }
                        else
                        {//修改 
                            dt = SystemManageModel.getMaintainDT(MaintainData.DEVICE_NO, new List<string> { MaintainData.CREATE_DATE });

                            modeStr = "修改";
                            recordStr = "MODIFY";
                            if (BaseModel.DTNotNullAndEmpty(dt))
                            {
                                dr = dt.Rows[0];
                            }
                            else
                            {
                                errBuilder.Add("取得資料失敗");
                                if (this.DBA.LastError != "")
                                {
                                    errBuilder.Add(this.DBA.LastError);
                                    LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name,
                                        MethodInfo.GetCurrentMethod().DeclaringType.Name);
                                }
                            }
                        }
                        if (!errBuilder.HasError)
                        {
                            dr["STATUS"] = MaintainData.STATUS;
                            dr["START_DATE"] = MaintainData.START_DATE;
                            dr["END_DATE"] = MaintainData.END_DATE;
                            dr["RUNNING_HR"] = MaintainData.RUNNING_HR;
                            dr["SYSTEM_TEST"] = MaintainData.SYSTEM_TEST;
                            dr["CLEAN_STATUS"] = MaintainData.CLEAN_STATUS;
                            dr["REMARK"] = MaintainData.REMARK;
                            dr[recordStr + "_ID"] = user_info.user_id;
                            dr[recordStr + "_NAME"] = user_info.user_name;
                            dr[recordStr + "_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            dt.TableName = GetTableName.RCS_VENTILATOR_MAINTAIN.ToString();
                            if (this.DBA.UpdateResult(dt).State == enmDBResultState.Success)
                            {
                                rm.status = RESPONSE_STATUS.SUCCESS;
                                rm.message = modeStr + "成功";
                            }
                            else
                            {
                                rm.message = modeStr + "失敗";
                            }
                            if (this.DBA.LastError != "")
                            {
                                rm.message += this.DBA.LastError;
                                LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name,
                                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
                            }
                        }
                    }
                    if (errBuilder.HasError) rm.message = errBuilder.errStr;
                }
                catch (Exception ex)
                {
                    rm.message = ex.Message;
                    LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name,
                        MethodInfo.GetCurrentMethod().DeclaringType.Name);
                }
            }

            return Json(rm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 刪除保養紀錄
        /// </summary>
        /// <param name="pMaintainData"></param>
        /// <returns></returns>
        public ActionResult MaintainDelete(string pDeviceNo, List<string> pCreateDate)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.status = RESPONSE_STATUS.ERROR;

            ErrStrBuilder errBuilder = new ErrStrBuilder();
            try
            {
                //-----2016/12/30 Vanda Mod
                //DataTable dt = SystemManageModel.getMaintainDT(pDeviceNo, pCreateDateList);
                DataTable dt = SystemManageModel.getMaintainDT(pDeviceNo, pCreateDate);
                //-----                
                if (BaseModel.DTNotNullAndEmpty(dt))
                {
                    dt.TableName = GetTableName.RCS_VENTILATOR_MAINTAIN.ToString();
                    foreach (DataRow ro in dt.Rows)
                    {
                        ro.Delete();
                    }
                    if (this.DBA.UpdateResult(dt).State == enmDBResultState.Success)
                    {
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = "刪除成功";
                    }
                    else
                    {
                        rm.message = "刪除失敗";
                    }
                    if (this.DBA.LastError != "")
                    {
                        rm.message += this.DBA.LastError;
                        LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name,
                            MethodInfo.GetCurrentMethod().DeclaringType.Name);
                    }
                }
                else
                {
                    errBuilder.Add("取得資料失敗");
                    if (this.DBA.LastError != "")
                    {
                        errBuilder.Add(this.DBA.LastError);
                        LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name,
                            MethodInfo.GetCurrentMethod().DeclaringType.Name);
                    }
                }
                if (errBuilder.HasError) rm.message = errBuilder.errStr;
            }
            catch (Exception ex)
            {
                rm.message = ex.Message;
                LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name, 
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return Json(rm, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 使用者管理

        public ActionResult UserMaintain()
        {
            try
            {
                List<Dictionary<string, string>> Dt = new List<Dictionary<string, string>>();
                Dictionary<string, string> Temp = null;
                List<SysParams> list = BaseModel.getRCS_SYS_PARAMS(GetP_MODEL.role.ToString(), "role_type");
                foreach (SysParams item in list)
                {
                    Temp = new Dictionary<string, string>();
                    Temp.Add("NAME", item.P_NAME);
                    Temp.Add("VALUE", item.P_VALUE);
                    Dt.Add(Temp);
                }
                ViewData["UserTypeList"] = Dt;
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, 
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }

            return View();
        }

        [HttpPost]
        public string UserMaintain_List()
        {
            List<Dictionary<string, string>> Dt = new List<Dictionary<string, string>>();
            Dt = SystemManageModel.UserMaintain_List(true);
            return JsonConvert.SerializeObject(Dt);
        }

        //儲存
        [HttpPost]
        public JsonResult UserMaintain_Save(string P_ID, string UserID, string UserName, string UserType, string UserRemark)
        {
            dbResultMessage dbMsg = new dbResultMessage();
            RESPONSE_MSG rm = new RESPONSE_MSG();
            DataRow dr = null;
            try
            {
                DataTable checkdt = BaseModel.getRCS_SYS_PARAMS_DT(@pValue: UserID, @pModel: "user");
                if (this.DBA.LastError != "")
                    LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name, 
                        MethodInfo.GetCurrentMethod().DeclaringType.Name);
                if (BaseModel.DTNotNullAndEmpty(checkdt) && checkdt.AsEnumerable().ToList().Exists(x => x["P_STATUS"].ToString() == "9"))
                    dr = checkdt.AsEnumerable().ToList().Find(x => x["P_STATUS"].ToString() == "9");
                if (!string.IsNullOrWhiteSpace(P_ID) && checkdt.AsEnumerable().ToList().Exists(x => x["P_ID"].ToString() == P_ID))
                    dr = checkdt.AsEnumerable().ToList().Find(x => x["P_ID"].ToString() == P_ID);
                if (dr == null)
                {
                    dr = checkdt.NewRow();
                    dr["P_ID"] = DateTime.Now.ToString("yyyyMMddHHmmss") + user_info.user_id;
                    dr["P_MODEL"] = GetP_MODEL.user.ToString();
                    checkdt.Rows.Add(dr);
                }
                dr["P_GROUP"] = UserType;
                dr["P_NAME"] = UserName;
                dr["P_VALUE"] = UserID;
                dr["P_LANG"] = "zh-tw";
                dr["P_MEMO"] = UserRemark;
                dr["P_STATUS"] = "1";
                dr["P_MANAGE"] = "0";

                dbMsg = this.DBA.UpdateResult(checkdt, GetTableName.RCS_SYS_PARAMS.ToString());
                if (dbMsg.State == enmDBResultState.Success)
                    rm.status = RESPONSE_STATUS.SUCCESS;
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    LogTool.SaveLogMessage(dbMsg.dbErrorMessage, MethodInfo.GetCurrentMethod().Name,
                        MethodInfo.GetCurrentMethod().DeclaringType.Name);
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, 
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return Json(rm);
        }

        #endregion

        #region 表單管理

        public ActionResult PhraseMaintain()
        {
            try
            {
                List<SysParams> list = BaseModel.getRCS_SYS_PARAMS(pModel: GetP_MODEL.rt_form.ToString());
                List<Dictionary<string, string>> Dt = new List<Dictionary<string, string>>();
                Dictionary<string, string> Temp = null;
                foreach (SysParams item in list)
                {
                    if (item.P_STATUS != "9")
                    {
                        Temp = new Dictionary<string, string>();
                        Temp.Add("NAME", item.P_NAME);
                        Temp.Add("VALUE", item.P_VALUE);
                        Dt.Add(Temp);
                    }
                }
                ViewData["FuncList"] = Dt;

                Dt = new List<Dictionary<string, string>>();
                list = BaseModel.getRCS_SYS_PARAMS("", "phrase");
                foreach (SysParams item in list)
                {
                    if (item.P_STATUS != "9")
                    {
                        Temp = new Dictionary<string, string>();
                        Temp.Add("MODEL", item.P_MODEL);
                        Temp.Add("NAME", item.P_NAME);
                        Temp.Add("VALUE", item.P_VALUE);
                        Dt.Add(Temp);
                    }
                }
                ViewData["PhraseList"] = Dt;

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, 
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }

            return View();
        }

        [HttpPost]
        public string PhraseMaintain_List(string P_Model)
        {
            List<Dictionary<string, string>> Dt = new List<Dictionary<string, string>>();
            Dt = SystemManageModel.PhraseMaintain_List(P_Model);
            return JsonConvert.SerializeObject(Dt);
        }

        [HttpPost]
        public JsonResult PhraseMaintain_Save(string P_ID, string FuncType, string PhraseType, string ItemName, string ItemValue, string ItemSort, string ItemRemark)
        {
            DataTable dtMode_memo = new DataTable();
            dbResultMessage dbMsg = new dbResultMessage();
            RESPONSE_MSG rm = new RESPONSE_MSG();
            bool IsNew = P_ID == "";
            try
            {
                DataTable dt = BaseModel.getRCS_SYS_PARAMS_DT("", FuncType, PhraseType, @pValue: ItemValue, @pStatus: "1");
                if (this.DBA.LastError != "")
                    LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name,
                        MethodInfo.GetCurrentMethod().DeclaringType.Name);
                DataTable savedt = BaseModel.getRCS_SYS_PARAMS_DT(P_ID, @pStatus: "1");
                if (this.DBA.LastError != "")
                    LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name, 
                        MethodInfo.GetCurrentMethod().DeclaringType.Name);
                DataRow dr = null;
                if (BaseModel.DTNotNullAndEmpty(dt) && IsNew)
                {
                    rm.status = RESPONSE_STATUS.DUPLICATE;
                    rm.message = "選項值重複,請重新輸入";
                    return Json(rm);
                }
                if (!IsNew && BaseModel.DTNotNullAndEmpty(savedt))
                {
                    dr = savedt.Rows[0];
                }
                else
                {
                    dr = savedt.NewRow();
                    dr["P_ID"] = DateTime.Now.ToString("yyyyMMddHHmmss") + user_info.user_id;
                    savedt.Rows.Add(dr);
                }
                dr["P_MODEL"] = FuncType;
                dr["P_GROUP"] = PhraseType;
                dr["P_VALUE"] = ItemValue;
                dr["P_NAME"] = ItemName;
                dr["P_SORT"] = ItemSort;
                dr["P_LANG"] = "zh-tw";
                dr["P_MEMO"] = ItemRemark;
                dr["P_STATUS"] = "1";
                dr["P_MANAGE"] = "0";
                //如果新增ICD10的資料，自動ON ICD10的病例註記
                if (!string.IsNullOrWhiteSpace(FuncType) && !string.IsNullOrWhiteSpace(PhraseType) && !string.IsNullOrWhiteSpace(ItemRemark))
                {
                    if (FuncType.Trim() == "RTRecord_Detail" && PhraseType.Trim() == "ventilation_mode")
                    {
                        string sql = "SELECT * FROM " + GetTableName.RCS_RECORD_DETAIL + " WHERE RECORD_ID in( SELECT RECORD_ID FROM RCS_RECORD_DETAIL WHERE ITEM_NAME = 'mode' AND ITEM_VALUE = '"+ ItemValue + "') AND ITEM_NAME = 'mode_memo'";
                        dtMode_memo = this.DBA.getSqlDataTable(sql);
                        if (BaseModel.DTNotNullAndEmpty(dtMode_memo))
                        {
                            foreach (DataRow drMode_memo in dtMode_memo.Rows)
                            {
                                drMode_memo["ITEM_VALUE"] = ItemRemark.Trim();
                            }
                        }
                    }
                }
                this.DBA.BeginTrans();

                dbMsg = this.DBA.UpdateResult(savedt, GetTableName.RCS_SYS_PARAMS.ToString());
                if (dbMsg.State == enmDBResultState.Success)
                {
                    dbMsg = this.DBA.UpdateResult(dtMode_memo, GetTableName.RCS_RECORD_DETAIL.ToString());
                } 
                if (dbMsg.State == enmDBResultState.Success)
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    this.DBA.Commit();
                }
                else
                {
                    this.DBA.Rollback();
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "程式發生錯誤!請洽資訊人員!";
                    LogTool.SaveLogMessage(dbMsg.dbErrorMessage, MethodInfo.GetCurrentMethod().Name, 
                        MethodInfo.GetCurrentMethod().DeclaringType.Name);
                }
                this.DBA.Close();
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "程式發生錯誤!請洽資訊人員!";
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, 
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }

            return Json(rm);
        }

        /*  以下為新版本  */
        /// <summary> 參數設定頁面 </summary>
        /// <returns></returns>
        public ActionResult ParamSetting()
        {
            return View();
        }

        /// <summary> 參數資料來源 </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ParamData(string p_model, string p_group)
        {
            string sqlstr = " select * from rcs_sys_params where p_model = '{0}' and p_group = '{1}' ";
            List<RCS_SYS_PARAMS> rsp_collection = new List<RCS_SYS_PARAMS>();
            try
            {
                sqlstr = string.Format(sqlstr, p_model, p_group);
                rsp_collection = this.DBA.Connection.Query<RCS_SYS_PARAMS>(sqlstr).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "PrarmSetting");
            }
            return Json(rsp_collection);
        }

        /// <summary> 參數儲存 </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ParamSave(RCS_SYS_PARAMS rsp)
        {

            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                SQLProvider SQL = new SQLProvider();
                //資料
                DataTable dt = null;// RCS_SYS_PARAMS的資料
                DataTable dtMode_memo = new DataTable();//如果新增ICD10的資料，自動ON ICD10的病例註記
                bool checkMode_memo = funSetting.checkMode_memo;

                string sql = "";
                //資料
                #region sql

                if (string.IsNullOrWhiteSpace(rsp.p_id) || string.IsNullOrEmpty(rsp.p_id))
                {
                    sql = "SELECT * FROM " + GetTableName.RCS_SYS_PARAMS + " WHERE 1<>1";
                }
                else
                {
                    sql = string.Format("SELECT * FROM " + GetTableName.RCS_SYS_PARAMS + " WHERE p_id = '{0}'", rsp.p_id);
                }
                #endregion
                dt = this.DBA.getSqlDataTable(sql);

                if (dt != null)
                {
                    DataRow dr = null;
                    //工作
                    #region 整理儲存資料
                    #region RCS_SYS_PARAMS的資料
                    if (string.IsNullOrWhiteSpace(rsp.p_id) || string.IsNullOrEmpty(rsp.p_id))
                    {
                        rsp.p_id = SQL.GetFixedStrSerialNumber();
                        dr = dt.NewRow();
                        dr["p_id"] = rsp.p_id;
                        dt.Rows.Add(dr);
                        dr = dt.AsEnumerable().ToList().Find(x => x["p_id"].ToString() == rsp.p_id);
                    }
                    else
                    {
                        dr = dt.AsEnumerable().ToList().Find(x => x["p_id"].ToString() == rsp.p_id);
                    }
                    dr["p_model"] = rsp.p_model;
                    dr["p_group"] = rsp.p_group;
                    dr["p_name"] = rsp.p_name;
                    dr["p_value"] = rsp.p_value;
                    dr["p_sort"] = rsp.p_sort.ToString();
                    dr["p_lang"] = "zh-tw";
                    dr["p_memo"] = rsp.p_memo;
                    dr["p_status"] = "1";
                    #endregion

                    #region 如果新增ICD10的資料，自動ON ICD10的病例註記
                    if (checkMode_memo)
                    {
                        //如果新增ICD10的資料，自動ON ICD10的病例註記
                        if (!string.IsNullOrWhiteSpace(rsp.p_model) && !string.IsNullOrWhiteSpace(rsp.p_group) && !string.IsNullOrWhiteSpace(rsp.p_memo) && !string.IsNullOrWhiteSpace(rsp.p_value))
                        {
                            if (rsp.p_model.Trim() == "RTRecord_Detail" && rsp.p_group.Trim() == "ventilation_mode" && rsp.p_value.Trim() != "")
                            {
                                sql = "SELECT * FROM " + GetTableName.RCS_RECORD_DETAIL + " WHERE RECORD_ID in( SELECT RECORD_ID FROM RCS_RECORD_DETAIL WHERE ITEM_NAME = 'mode' AND ITEM_VALUE = '" + rsp.p_value + "') AND ITEM_NAME = 'mode_memo'";
                                dtMode_memo = this.DBA.getSqlDataTable(sql);
                                if (BaseModel.DTNotNullAndEmpty(dtMode_memo))
                                {
                                    foreach (DataRow drMode_memo in dtMode_memo.Rows)
                                    {
                                        drMode_memo["ITEM_VALUE"] = rsp.p_memo.Trim();
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #endregion

                    //結果
                    dbResultMessage dbMsg = new dbResultMessage();
                    #region 儲存結果
                    this.DBA.BeginTrans();
                    dbMsg = this.DBA.UpdateResult(dt, GetTableName.RCS_SYS_PARAMS.ToString());
                    if (dbMsg.State == enmDBResultState.Success)
                    {
                        dbMsg = this.DBA.UpdateResult(dtMode_memo, GetTableName.RCS_RECORD_DETAIL.ToString());
                    }
                    if (dbMsg.State != enmDBResultState.Success)
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "程式發生錯誤!請洽資訊人員!";
                        LogTool.SaveLogMessage(dbMsg.dbErrorMessage, "ParamSave", GetLogToolCS.SystemManageController);
                        this.DBA.Rollback();
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = "儲存成功!";
                        this.DBA.Commit();
                    }
                    this.DBA.Close();
                    #endregion
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "儲存失敗";
                    LogTool.SaveLogMessage(this.DBA.LastError, "ParamSave", GetLogToolCS.SystemManageController);
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = ex.Message.ToString();
                LogTool.SaveLogMessage(ex, "ParamSave");
            }
            return Json(rm);
        }

        /*---------------- DragDrop [下] ----------------*/
        public ActionResult SysParams_DragDrop_EditTable_ActionCtrl() //2018.10.17
        {
            // 1.需先預載入，主畫面 [Views\SystemManage\ParamSetting.cshtml] 預載入 [RcsSysParams_DragDrop_DivTagID]
            // 2.$('#RcsSysParams_DragDrop_DivTagID').load('@Url.Content("~/SystemManage/SysParams_DragDrop_EditTable_ActionCtrl")');
            // 3.DragDrop 拖曳編輯頁面 [Views\SystemManage\SysParams_DragDrop_EditTable.cshtml] 
            return View("SysParams_DragDrop_EditTable");
        }

        [HttpPost]
        public JsonResult RcsSysParams_DragDropEdit_Save(List<RCS_SYS_PARAMS> input_RcsSysParams_List) //2018.10.17
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            DataTable RcsSysParams_DTable = new DataTable();// RCS_SYS_PARAMS的資料
            dbResultMessage dbResultMsg = new dbResultMessage();
            List<string> Pid_strList = new List<string>();
            try
            {
                //1.獲得 SQL 現有的資料庫內容
                string sqlCmdStr = string.Format(
                    @"SELECT TOP 0 * FROM {0}"
                    , GetTableName.RCS_SYS_PARAMS
                    );
                RcsSysParams_DTable = this.DBA.getSqlDataTable(sqlCmdStr);
                //2.更新 DataTable 暫存資料
                if (input_RcsSysParams_List != null 
                    && input_RcsSysParams_List.Count > 0
                ){
                    int ii = 0;
                    foreach (RCS_SYS_PARAMS inputRSP_Node in input_RcsSysParams_List)
                    {
                        if (inputRSP_Node != null
                            && !string.IsNullOrWhiteSpace(inputRSP_Node.p_id)
                        ){
                            ii++;
                            //待刪除 p_id 資料暫存
                            Pid_strList.Add(SQLDefend.SQLString(inputRSP_Node.p_id.Trim()));
                            DataRow RcsSysParams_DRow = RcsSysParams_DTable.NewRow();
                                RcsSysParams_DRow["P_ID"] = inputRSP_Node.p_id;
                                RcsSysParams_DRow["P_MODEL"] = inputRSP_Node.p_model;
                                RcsSysParams_DRow["P_GROUP"] = inputRSP_Node.p_group;
                                RcsSysParams_DRow["P_NAME"] = inputRSP_Node.p_name;
                                RcsSysParams_DRow["P_VALUE"] = inputRSP_Node.p_value;
                                RcsSysParams_DRow["P_LANG"] = inputRSP_Node.p_lang;
                                //RcsSysParams_DRow["P_SORT"] = inputRSP_Node.p_sort.ToString();
                                RcsSysParams_DRow["P_SORT"] = ii.ToString("000"); //前面補0數字
                                RcsSysParams_DRow["P_MEMO"] = inputRSP_Node.p_memo;
                                RcsSysParams_DRow["P_STATUS"] = inputRSP_Node.p_status;
                            RcsSysParams_DTable.Rows.Add(RcsSysParams_DRow);
                        }//if
                    }//foreach
                    // 4.先刪除 [Cxr_CJID流水號] 舊資料
                    String Pid_strStr = String.Join(",", Pid_strList.ToArray());
                    this.DBA.BeginTrans(); //是 Rollback 的起點，復原使用 Update 方法所儲存的變更
                    dbResultMsg = this.DBA.ExecuteNonQuery(
                        "DELETE " + GetTableName.RCS_SYS_PARAMS.ToString() +
                        " WHERE p_id in (" + Pid_strStr + ")"
                        );
                    //3.SQL儲存更新
                    if (dbResultMsg.State == enmDBResultState.Success)
                    {
                        dbResultMsg = this.DBA.UpdateResult(
                            RcsSysParams_DTable, 
                            GetTableName.RCS_SYS_PARAMS.ToString()
                            );
                    }
                    //4.錯誤訊息顯示
                    if (dbResultMsg.State == enmDBResultState.Success)
                    {
                        //成功不做任何事情
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = "儲存成功";
                        this.DBA.Commit();
                    } else {
                        rm.message = "程式發生錯誤!請洽資訊人員!";
                        LogTool.SaveLogMessage(dbResultMsg.dbErrorMessage, "RcsSysParams_DragDropEdit_Save", GetLogToolCS.SystemManageController);
                        this.DBA.Rollback();
                    }
                    this.DBA.Close();
                }//if
            }//try
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = ex.Message.ToString();
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }//catch
            return Json(rm);
        }//RcsSysParams_DragDropEdit_Save
        /*---------------- DragDrop [上] ----------------*/

        /// <summary> 參數刪除 </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ParamDelete(RCS_SYS_PARAMS rsp)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                string sql = string.Format("SELECT * FROM " + GetTableName.RCS_SYS_PARAMS + " WHERE p_id = '{0}'", rsp.p_id);
                DataTable dt = this.DBA.getSqlDataTable(sql);
                if(dt != null && dt.Rows.Count>0)
                {
                    //刪除資料
                    dt.Rows[0]["p_status"]= rsp.p_status;
                }
                dbResultMessage dbMsg = new dbResultMessage();
                dbMsg = this.DBA.UpdateResult(dt, GetTableName.RCS_SYS_PARAMS.ToString());
                if (dbMsg.State != enmDBResultState.Success)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "設定失敗，程式發生錯誤!請洽資訊人員!";
                    LogTool.SaveLogMessage(dbMsg.dbErrorMessage, "ParamDelete", GetLogToolCS.SystemManageController);
                }
                else
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "設定成功!";
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = ex.Message.ToString();
                LogTool.SaveLogMessage(ex, "ParamDelete");
            }
            return Json(rm);
        }

        #region 排班管理

        //取得排班表(View or Json)
        public ActionResult Scheduling(string year = "", string month = "", string data_type = "")
        {
            if (data_type.ToLower() == "json")
            {
                scheduling sch_data = SystemManageModel.getSchedulingData(year, month);
                return Content(JsonConvert.SerializeObject(sch_data));
            }
            else
            {
                return View();
            }
        }

        public ActionResult Scheduling_Save(string scheduling_data = "")
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.status = RESPONSE_STATUS.ERROR;
            try
            {
                if (string.IsNullOrEmpty(scheduling_data))
                {
                    rm.message = "傳入存檔資料錯誤";
                    return Json(rm);
                }
                else
                {
                    scheduling sch_data = JsonConvert.DeserializeObject<scheduling>(scheduling_data);
                    if (sch_data.year > 0 && sch_data.month > 0 && sch_data.schedul_data.Count > 0)
                    {
                        var yyyy = sch_data.year.ToString().PadLeft(4, '0');
                        var mm = sch_data.month.ToString().PadLeft(2, '0');
                        var yyyymm_date = yyyy + mm;

                        string table_name = GetTableName.RCS_SYS_SCHEDULING.ToString();
                        DataTable dt = SystemManageModel.getSchedulingDT(yyyy, mm);
                        Dictionary<string, DataRow> dtdic = new Dictionary<string, DataRow>();
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dtdic = dt.AsEnumerable().ToDictionary(key => key["YYYYMM_DATE"].ToString() + "_" + key["RT_ID"].ToString(), value => value);
                        }
                        foreach (SchedulData item in sch_data.schedul_data)
                        {
                            DataRow dr = null;
                            string key = yyyymm_date + "_" + item.op_id;
                            if (dtdic != null && dtdic.ContainsKey(key))
                            {
                                dr = dtdic[key];
                            }
                            else
                            {
                                dr = dt.NewRow();
                                dr["YYYYMM_DATE"] = yyyymm_date;
                                dr["RT_ID"] = item.op_id;
                                dt.Rows.Add(dr);
                            }

                            dr["RT_NAME"] = item.op_name;
                            dr["REMARK"] = "{\"month_off\": \"" + sch_data.month_off + "\", \"working_hr\": \"" + sch_data.working_hr + "\"}";
                            string day_data = JsonConvert.SerializeObject(item.day_data);
                            dr["WORK_DATA"] = day_data;
                        }
                        dbResultMessage dbMsg = this.DBA.UpdateResult(dt);
                        if (dbMsg.State == enmDBResultState.Success)
                        {
                            rm.status = RESPONSE_STATUS.SUCCESS;
                        }
                        if (dbMsg.dbErrorMessage != "")
                        {
                            rm.message = dbMsg.dbErrorMessage;
                            LogTool.SaveLogMessage(dbMsg.dbErrorMessage, MethodInfo.GetCurrentMethod().Name,
                                MethodInfo.GetCurrentMethod().DeclaringType.Name);
                        }
                    }
                    else
                    {
                        rm.message = "傳入存檔資料錯誤";
                    }
                }
            }
            catch (Exception ex)
            {
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name,
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return Json(rm);
        }

        public ActionResult ExportScheduleExcel(string pJsonDataStr)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.status = RESPONSE_STATUS.ERROR;
            if (string.IsNullOrEmpty(pJsonDataStr))
            {
                rm.message = "傳入存檔資料錯誤";
                return Json(rm);
            }
            else
            {
                scheduling sch_data = JsonConvert.DeserializeObject<scheduling>(pJsonDataStr);
                try
                {
                    exportFile exportFile = new exportFile(  sch_data.year.ToString() + "年" + sch_data.month.ToString() + "月排班表.xls");
                    return exportFile.ExportShiftExcel(sch_data);
                }
                catch (Exception ex)
                {
                    rm.message = "系統發生錯誤" + ex.Message;
                    LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, GetLogToolCS.RTController);
                }
            }
            return Json(rm);
        }
        #endregion

        #region 病患統計(7190)

        public ActionResult Count_7190()
        {
            ViewData["user_info"] = user_info;
            return View();
        }

        //取得病患7190資料
        public ActionResult Count7190Data(string group_v)
        {
            List<object> return_obj = new List<object>();
            string sql_str = "WHERE SYNC_STATUS = 'L' ";
            if (!string.IsNullOrEmpty(group_v))
            {
                string[] group_arr = group_v.Split(',');
                if (group_arr.Count() > 0)
                {
                    sql_str += string.Format("AND NS in ('{0}')  ", string.Join("','", group_arr));
                }
            }
            DataTable Dt = this.SystemManageModel.getOrderTemp(sql_str);
            foreach (DataRow Dr in Dt.Rows)
            {
                string room_loc = Dr["ROOM_LOC"].ToString().Trim();
                string room_no = Dr["ROOM_NO"].ToString().Trim();
                string bed_no = Dr["BED_NO"].ToString().Trim();
                string bed_no_str = bed_no == "" ? "" : "-" + bed_no;
                return_obj.Add(new
                {
                    t_id = Dr["ODR_ID"].ToString().Trim(),
                    bed_no = bed_no,
                    room_loc = room_loc,
                    room_no = room_no,
                    patient_name = Dr["PATIENT_NAME"].ToString().Trim(),
                    ipd_no = Dr["IPD_NO"].ToString().Trim(),
                    chart_no = Dr["CHART_NO"].ToString().Trim(),
                    on_7190 = Dr["ORDER_DATA"].ToString().Trim(),
                    on_status = Dr["ON_STATUS"].ToString().Trim(),
                    sync_date = Convert.ToDateTime(Dr["SYNC_DATE"]).ToString("yyyy-MM-dd HH:mm:ss").Trim()
                }
                );
            }
            return Content(JsonConvert.SerializeObject(return_obj));
        } 
        /// <summary>
        /// 取得前3天(預設)會診資料
        /// </summary>
        /// <returns></returns>
        public JsonResult getCount7190Data(string pSDate, string pEDate)
        {
            Count7190List boostrapList = new Count7190List();
            try
            {
                WebMethod wm = new WebMethod();
                pSDate = pSDate ?? ""; pEDate = pEDate ?? "";
                if (!(pSDate != "" && pEDate != ""))
                {
                    pSDate = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm");
                    pEDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                }
                boostrapList.listCount7190 = wm.get7190OrderList(this.hospFactory.webService.RCSConsultList(), pSDate, pEDate);
                boostrapList.RESPONSE_MSG = wm.rm;
                //依執行時間反向排序
                if (boostrapList.listCount7190.Count > 0)
                {
                    boostrapList.listCount7190 = boostrapList.listCount7190.OrderByDescending(x => x.START_DATE).ToList();
                }
                
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getCount7190Data", GetLogToolCS.SystemManageController);
            }
            return Json(boostrapList);
        }
        #endregion
         

        //刪除
        [HttpPost]
        public bool SysParams_Del(string JsonList)
        {
            dbResultMessage dbMsg = new dbResultMessage();
            bool Success = false;
            try
            {
                List<Dictionary<string, string>> DtList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(JsonList);
                dbMsg = SystemManageModel.SysParams_Del(DtList);
                if (dbMsg.State == enmDBResultState.Success)
                    Success = true;
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return Success;
        }

        #region 病歷號變更作業
        public ActionResult chartChange()
        {
            return View();
        }

        /// <summary>
        /// 檢核病歷號
        /// </summary>
        /// <param name="pOldChart">畫面輸入之原病歷號</param>
        /// <param name="pNewChart">畫面輸入之新病歷號</param>
        /// <returns></returns>
        [HttpPost]
        public string CheckChartNo(string pOldChart, string pNewChart)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            WebMethod wm = new WebMethod();
            //RESPONSE_MSG _result = wm.ws_get_merge_chart_no(pOldChart, pNewChart, user_info.user_id, user_info.user_name);

            rm.status = RESPONSE_STATUS.SUCCESS;
            rm.message = pNewChart;
            return rm.get_json();
        }

        /// <summary>
        /// 更新病歷號
        /// </summary>
        /// <param name="pOldChart">原病歷號</param>
        /// <param name="pNewChart">WS帶回來的資料</param>
        /// <returns></returns>
        [HttpPost]
        public string UpdateChartNo(string pOldChart, string pNewChart)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<string> UpdList = new List<string>();
            DataTable logDt = this.DBA.getDataTable("RCS_CHANGE_CHARTNO_LOG", "WHERE 1<>1");
            string update_status = "";
            string[] UpdArray = new string[] { "RCS_CPT_ASS_MASTER", "RCS_RECORD_MASTER", "RCS_PATIENT_ASSESS", "RCS_DATA_RtGrantList", "RCS_ORDER_TEMP", "RT_MEASURES_FORM_MASTER", "RCS_RT_CASE", "RCS_RT_CARE_SCHEDULING", "RCS_CPT_RECORD", "RCS_VENTILATOR_CHECKLIST", "RCS_WEANING_ASSESS" };
            try
            {
                UpdArray.ToList().ForEach(x => UpdList.Add(string.Format("UPDATE {0} SET CHART_NO={1} WHERE CHART_NO={2}", x.ToString(), SQLDefend.SQLString(pNewChart), SQLDefend.SQLString(pOldChart))));
                //UpdList.Add(string.Format("UPDATE {0} SET CHID={1} WHERE CHID={2}", "RCS_DATA_RtGrantList", SQLDefend.SQLString(pNewChart), SQLDefend.SQLString(pOldChart)));
                //UpdList.Add(string.Format("UPDATE {0} SET PATIENT_ID={1} WHERE PATIENT_ID={2}", "VIPRTTBL_CCH", SQLDefend.SQLString(pNewChart), SQLDefend.SQLString(pOldChart)));
                //UpdList.Add(string.Format("UPDATE {0} SET CHR_NO={1} WHERE CHR_NO={2}", "RT_MEASURES_FORM_MASTER", SQLDefend.SQLString(pNewChart), SQLDefend.SQLString(pOldChart)));
                //UpdList.Add(string.Format("UPDATE {0} SET CHR_NO={1} WHERE CHR_NO={2}", "RT_USER_RECORD_MASTER", SQLDefend.SQLString(pNewChart), SQLDefend.SQLString(pOldChart)));
                UpdList.Add(string.Format(@"UPDATE {0} SET S_VALUE=REPLACE(CAST(S_VALUE AS NVARCHAR(MAX)),'""data"":""{1}""','""data"":""{2}""') WHERE S_VALUE LIKE '%""data"":""{1}""%'", "RCS_RT_ISBAR_SHIFT", pOldChart, pNewChart));

                this.DBA.BeginTrans();
                UpdList.ForEach(x => this.DBA.ExecuteNonQuery(x.ToString().Trim()));
                if (this.DBA.LastError != "")
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "病歷號更新失敗!" + this.DBA.LastError;
                    update_status = "2";
                    this.DBA.Rollback();
                }
                else
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    update_status = "1";
                }

                //儲存病歷號更新紀錄
                DataRow logDr = logDt.NewRow();
                logDr["OLD_CHART_NO"] = pOldChart;
                logDr["NEW_CHART_NO"] = pNewChart;
                logDr["MODIFY_ID"] = user_info.user_id;
                logDr["MODIFY_NAME"] = user_info.user_name;
                logDr["MODIFY_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                logDr["UPDATE_STATUS"] = update_status;
                logDr["ERROR_MESSAGE"] = rm.message ?? "";
                logDt.Rows.Add(logDr);
                mayaminer.com.jxDB.dbResultMessage rc = this.DBA.UpdateResult(logDt, "RCS_CHANGE_CHARTNO_LOG");
                if (rc.State == mayaminer.com.jxDB.enmDBResultState.Success)
                {
                    this.DBA.Commit();
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    this.DBA.Rollback();
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "病歷號更新失敗!" + ex.Message;
            }
            return rm.get_json();
        }
        #endregion

        #region paint

        public ActionResult paint()
        {

            return PartialView();
        }
        #endregion
    }
    /// <summary>
    /// RT照護記錄統計，臨時使用，上線後刪除
    /// </summary>
    public class RTWORKCOUNT
    {
        public string analysis_date { get; set; }
        public string updateuser { get; set; }
        public string work_count { get; set; }
    }

}
#endregion