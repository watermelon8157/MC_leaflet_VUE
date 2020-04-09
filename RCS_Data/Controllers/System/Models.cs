using Com.Mayaminer;
using mayaminer.com.library;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RCS_Data.Controllers.System
{
    public class Models : BaseModels,RCS_Data.Controllers.System.Interface
    { 
        string csName { get { return "System.Models"; } }

        public RESPONSE_MSG saveUser(IUSER_FORM_BODY form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.status = RESPONSE_STATUS.ERROR;
            bool isNew = form.model.P_ID == null;
            if (string.IsNullOrWhiteSpace(form.model.P_VALUE))
            {
                rm.message = "請填寫ID!"; 
            }
            if (string.IsNullOrWhiteSpace(form.model.P_NAME))
            {
                rm.message = "請填寫姓名!"; 
            }
            if (string.IsNullOrWhiteSpace(form.model.P_GROUP))
            {
                rm.message = "請填寫使用者身分!"; 
            }

            this.DBLink.DBA.Open();
            if (isNew)
            {
                string getP_ID = this.DBLink.GetFixedStrSerialNumber(form.payload.user_id);

                DB_RCS_SYS_PARAMS addData = new DB_RCS_SYS_PARAMS()
                {
                    P_ID = getP_ID,
                    P_MODEL = GetP_MODEL.user.ToString(),
                    P_GROUP = form.model.P_GROUP,
                    P_NAME = form.model.P_NAME,
                    P_VALUE = form.model.P_VALUE,
                    P_LANG = "zh-tw",
                    P_MEMO = form.model.P_MEMO,
                    P_STATUS = "1",
                    P_MANAGE = "0",
                    MODIFY_ID = form.payload.user_id,
                    MODIFY_NAME = form.payload.user_name,
                    MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                };
                this.DBLink.DBA.DBExecInsert<DB_RCS_SYS_PARAMS>(new List<DB_RCS_SYS_PARAMS>() { addData });
            }
            else
            {
                string sql = "";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                sql = "select * from RCS_SYS_PARAMS where P_ID = @P_ID";
                dp.Add("P_ID", form.model.P_ID);

                var updateData = this.DBLink.DBA.getSqlDataTable<DB_RCS_SYS_PARAMS>(sql, dp);

                foreach (var item in updateData)
                {
                    item.MODIFY_ID = form.payload.user_id;
                    item.MODIFY_NAME = form.payload.user_name;
                    item.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    item.P_VALUE = form.model.P_VALUE;
                    item.P_NAME = form.model.P_NAME;
                    item.P_GROUP = form.model.P_GROUP;
                    item.P_MEMO = form.model.P_MEMO;
                    item.P_STATUS = form.model.P_STATUS;
                }

                this.DBLink.DBA.DBExecUpdate<DB_RCS_SYS_PARAMS>(updateData);

            }

            this.DBLink.DBA.Close();
            if (!rm.hasLastError)
            {
                rm.status = RESPONSE_STATUS.SUCCESS;
                rm.attachment = "儲存成功";
            } 
            return rm;
        }

        public RESPONSE_MSG getUserMaintainList(IGetUserMaintain_List_BODY from)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG(); 
            List<DB_RCS_SYS_PARAMS> _sysList = new List<DB_RCS_SYS_PARAMS>();
            try
            {
                List<DB_RCS_SYS_PARAMS> sysList = this.getRCS_SYS_PARAMS_GName(GetP_MODEL.role.ToString(), "role_type", GetP_MODEL.user.ToString(), @pStatus: "1");
                if (sysList.Exists(x => x.P_VALUE == from.payload.user_id) && from.isYourSelfExcept)
                    sysList.Remove(sysList.Find(x => x.P_VALUE == from.payload.user_id));
                _sysList = sysList.FindAll(x => x.P_MANAGE == "0");

                if (!string.IsNullOrWhiteSpace(from.P_ID))
                {
                    _sysList = _sysList.Where(x => x.P_ID == from.P_ID).ToList();
                }
                rm.attachment = _sysList; 
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "查詢失敗!"; 
            }
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = this.DBLink.DBA.lastError;
            }
            return rm;
        } 

        public RESPONSE_MSG getUserGroupList()
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<DB_RCS_SYS_PARAMS> list = this.getRCS_SYS_PARAMS(GetP_MODEL.role.ToString(), "role_type");
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = this.DBLink.DBA.lastError;
            }
            rm.attachment = list;
            return rm; 
        } 

        public RESPONSE_MSG getDevice(string pDeviceNo)
        {
            // 用getDeviceList 用不到getDevice方法
            throw new NotImplementedException();
        }

        public RESPONSE_MSG getDeviceType()
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<DB_RCS_SYS_PARAMS> result = new List<DB_RCS_SYS_PARAMS>();
            string sql = "";
            sql = "select * from RCS_SYS_PARAMS where P_MODEL = 'Index_device' and P_GROUP='device_model' order by P_SORT"; 
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters(); 
            result = this.DBLink.DBA.getSqlDataTable<DB_RCS_SYS_PARAMS>(sql, dp);
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "查詢失敗!" + this.DBLink.DBA.lastError;
            } 
            rm.attachment = result;
            return rm;
        }
        /// <summary>
        /// 儲存呼吸器編號
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG saveVENTILATOR(IVENTILATOR_FORM_BODY form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            bool isNew = form.model.DEVICE_SEQ == null; 
            if (string.IsNullOrWhiteSpace(form.model.DEVICE_NO))
            {
                rm.messageList.Add("請填寫財產編號!"); 
            }
            if (string.IsNullOrWhiteSpace(form.model.DEVICE_MODEL))
            {
                rm.messageList.Add("請填寫呼吸器類別!"); 
            }
            if (!rm.hasLastError)
            {
                string sql = "";
                sql = "select * from RCS_VENTILATOR_SETTINGS where DEVICE_NO = @DEVICE_NO AND USE_STATUS = 'Y'";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("DEVICE_NO", form.model.DEVICE_NO);

                if (!isNew)
                {
                    sql += " AND DEVICE_SEQ <> @DEVICE_SEQ";
                    dp.Add("DEVICE_SEQ", form.model.DEVICE_SEQ);
                } 
                var checkData = this.DBLink.DBA.getSqlDataTable<DB_RCS_VENTILATOR_SETTINGS>(sql, dp);

                if (checkData.Count() > 0)
                {
                    rm.messageList.Add("呼吸器財產編號重複類別!");
                }else
                {
                    if (isNew)
                    {
                        string getDEVICE_SEQ = this.DBLink.GetFixedStrSerialNumber();

                        DB_RCS_VENTILATOR_SETTINGS addData = new DB_RCS_VENTILATOR_SETTINGS()
                        {
                            DEVICE_SEQ = getDEVICE_SEQ,
                            DEVICE_NO = form.model.DEVICE_NO,
                            ROOM = form.model.ROOM,
                            DEVICE_MODEL = form.model.DEVICE_MODEL,
                            CREATE_ID = form.payload.user_id,
                            CREATE_NAME = form.payload.user_name,
                            CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                            MODIFY_ID = form.payload.user_id,
                            MODIFY_NAME = form.payload.user_name,
                            MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                            USE_STATUS = "Y",
                            PURCHASE_DATE = form.model.PURCHASE_DATE,
                        };

                        this.DBLink.DBA.DBExecInsert<DB_RCS_VENTILATOR_SETTINGS>(new List<DB_RCS_VENTILATOR_SETTINGS>() { addData });

                    }
                    else
                    {
                        sql = "select * from RCS_VENTILATOR_SETTINGS where DEVICE_SEQ = @DEVICE_SEQ";
                        dp = new Dapper.DynamicParameters();
                        dp.Add("DEVICE_SEQ", form.model.DEVICE_SEQ);

                        var updateData = this.DBLink.DBA.getSqlDataTable<DB_RCS_VENTILATOR_SETTINGS>(sql, dp);

                        foreach (var item in updateData)
                        {
                            item.MODIFY_ID = form.payload.user_id;
                            item.MODIFY_NAME = form.payload.user_name;
                            item.MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss);
                            item.USE_STATUS = form.model.USE_STATUS;
                            item.DEVICE_NO = form.model.DEVICE_NO;
                            item.DEVICE_MODEL = form.model.DEVICE_MODEL;
                            item.ROOM = form.model.ROOM;
                            item.PURCHASE_DATE = form.model.PURCHASE_DATE;
                        }
                        this.DBLink.DBA.DBExecUpdate<DB_RCS_VENTILATOR_SETTINGS>(updateData);
                    } 
                } 
            } 
            rm.attachment = "儲存成功";
            return rm;
        }
                
        public RESPONSE_MSG ParamData(string p_model, string p_group, string P_ID = "")
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<DB_RCS_SYS_PARAMS> rsp_collection = new List<DB_RCS_SYS_PARAMS>();
            try
            {
                string sql = "";
                sql = "select * from rcs_sys_params where p_model = @p_model and p_group = @p_group"; 
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("p_model", p_model);
                dp.Add("p_group", p_group);

                if (!string.IsNullOrWhiteSpace(P_ID))
                {
                    sql += " AND P_ID = @P_ID";
                    dp.Add("P_ID", P_ID);
                } 
                rsp_collection = this.DBLink.DBA.getSqlDataTable<DB_RCS_SYS_PARAMS>(sql, dp); 
                if (this.DBLink.DBA.hasLastError)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "查詢失敗!" + this.DBLink.DBA.lastError;
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!" + this.DBLink.DBA.lastError;
                LogTool.SaveLogMessage(ex, "ParamData");
            }
            rm.attachment = rsp_collection;
            return rm;
        }

        public RESPONSE_MSG saveParamSetting(IUSER_FORM_BODY form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            bool isNew = form.model.P_ID == null; 
            if (string.IsNullOrWhiteSpace(form.model.P_NAME))
            {
                rm.messageList.Add("請輸入名稱!"); 
            }
            if (string.IsNullOrWhiteSpace(form.model.P_VALUE))
            {
                rm.messageList.Add("請輸入值!"); 
            }
            if (!rm.hasLastError)
            {
                if (isNew)
                {
                    string getP_ID = this.DBLink.GetFixedStrSerialNumber(form.payload.user_id); 
                    DB_RCS_SYS_PARAMS addData = new DB_RCS_SYS_PARAMS()
                    {
                        P_ID = getP_ID,
                        P_MODEL = form.model.P_MODEL,
                        P_GROUP = form.model.P_GROUP,
                        P_NAME = form.model.P_NAME,
                        P_VALUE = form.model.P_VALUE,
                        P_SORT = form.model.P_SORT,
                        P_LANG = "zh-tw",
                        P_MEMO = form.model.P_MEMO,
                        P_STATUS = form.model.P_STATUS,
                        MODIFY_ID = form.payload.user_id,
                        MODIFY_NAME = form.payload.user_name,
                        MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    };
                    this.DBLink.DBA.DBExecInsert<DB_RCS_SYS_PARAMS>(new List<DB_RCS_SYS_PARAMS>() { addData });
                }
                else
                {
                    string sql = "";
                    Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                    sql = "select * from RCS_SYS_PARAMS where P_ID = @P_ID";
                    dp.Add("P_ID", form.model.P_ID); 
                    var updateData = this.DBLink.DBA.getSqlDataTable<DB_RCS_SYS_PARAMS>(sql, dp); 
                    foreach (var item in updateData)
                    {
                        item.MODIFY_ID = form.payload.user_id;
                        item.MODIFY_NAME = form.payload.user_name;
                        item.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        item.P_VALUE = form.model.P_VALUE;
                        item.P_NAME = form.model.P_NAME;
                        item.P_GROUP = form.model.P_GROUP;
                        item.P_SORT = form.model.P_SORT;
                        item.P_STATUS = form.model.P_STATUS;
                        item.P_MEMO = form.model.P_MEMO;
                    } 
                    this.DBLink.DBA.DBExecUpdate<DB_RCS_SYS_PARAMS>(updateData); 
                }
            }
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "查詢失敗!" + this.DBLink.DBA.lastError;
            }
            rm.attachment = "儲存成功";
            return rm;
        }

        public RESPONSE_MSG saveParamSettingSort(IParam_Setting_FORM_BODY form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();  
            string sql = "";
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            sql = "select * from RCS_SYS_PARAMS where P_ID in @P_ID";
            dp.Add("P_ID", form.list.Select(x => x.P_ID)); 
            var updateData = this.DBLink.DBA.getSqlDataTable<DB_RCS_SYS_PARAMS>(sql, dp); 
            int count = 0;
            foreach (var item in form.list)
            {
                foreach (var updateitem in updateData)
                {
                    if (updateitem.P_ID == item.P_ID)
                    {
                        updateitem.MODIFY_ID = form.payload.user_id;
                        updateitem.MODIFY_NAME = form.payload.user_name;
                        updateitem.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        updateitem.P_SORT = count.ToString();
                    }
                }
                count++;
            } 
            this.DBLink.DBA.DBExecUpdate<DB_RCS_SYS_PARAMS>(updateData); 
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "查詢失敗!" + this.DBLink.DBA.lastError;
            }
            rm.attachment = "儲存成功";
            return rm;
        }

        public RESPONSE_MSG GetVENTILATOR(string sDate, string eDate)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<VENTILATORGroupModel> result = new List<VENTILATORGroupModel>();
            string sql = "";
            sql = "SELECT * FROM RCS_VENTILATOR_SCHEDULING_CHECKLIST WHERE RECORD_DATE BETWEEN  @sDate AND @eDate AND DATASTATUS !='9'"; 
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            dp.Add("sDate", sDate);
            dp.Add("eDate", eDate); 
            var list = this.DBLink.DBA.getSqlDataTable<VENTILATORModel>(sql, dp);
            var MV_NOList = list.Select(x => x.MV_NO).Distinct();
            sql = "SELECT * FROM RCS_VENTILATOR_SETTINGS WHERE DEVICE_NO IN @DEVICE_NO ";
            dp = new Dapper.DynamicParameters();
            dp.Add("DEVICE_NO", MV_NOList); 
            var getCheckMV_NOList = this.DBLink.DBA.getSqlDataTable<RCS_VENTILATOR_SETTINGSModel>(sql, dp); 
            foreach (var data in list)
            {
                data.DEVICE_MODEL = getCheckMV_NOList.Where(x => x.DEVICE_NO == data.MV_NO).FirstOrDefault()?.DEVICE_MODEL ?? "";
            }
            result = list.GroupBy(x => new { x.DEVICE_MODEL, x.RECORD_DATE }).ToList().Select(x => new VENTILATORGroupModel
            {
                dataLists = x.ToList(),
                DEVICE_MODEL = x.Key.DEVICE_MODEL,
                RECORD_DATE = x.Key.RECORD_DATE,
                hasDatastatus = x.ToList().Exists(y => y.DATASTATUS == "0")
            }).ToList();

            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = this.DBLink.DBA.lastError;
            }
            rm.attachment = result;
            return rm;
        }

        public RESPONSE_MSG GetSelectList()
        {
            RESPONSE_MSG rm = new RESPONSE_MSG(); 
            List<DB_RCS_VENTILATOR_SETTINGS> result = new List<DB_RCS_VENTILATOR_SETTINGS>(); 
            string sql = "";
            sql = "select DISTINCT DEVICE_NO , DEVICE_MODEL  from  RCS_VENTILATOR_SETTINGS"; 
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters(); 
            //取呼吸器下拉
            result = this.DBLink.DBA.getSqlDataTable<DB_RCS_VENTILATOR_SETTINGS>(sql, dp); 
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = this.DBLink.DBA.lastError;
            }
            rm.attachment = result;
            return rm;
        } 

        public RESPONSE_MSG GetVENTILATORCheckList(string id)
        {

            RESPONSE_MSG rm = new RESPONSE_MSG();
            VENTILATORViewModel vm = new VENTILATORViewModel();

            List<DB_RCS_SYS_PARAMS> CheckNameList = this.getRCS_SYS_PARAMS(pModel: "SCHEDULING_CHECKLIST", pGroup: "CONTENT");
            List<DB_RCS_SYS_PARAMS> CheckTypeList = this.getRCS_SYS_PARAMS(pModel: "SCHEDULING_CHECKLIST", pGroup: "VALUE_TYPE");

            List<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL> getCheckTypeList = new List<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL>();

            var CheckDATASTATUS = true;

            if (string.IsNullOrWhiteSpace(id))
            {
                vm.MV_NO = "新增檢核表";
                vm.V_TYPE = "";
                vm.V_STATUS = "";
            }
            else
            {
                string sql = "";
                sql = "SELECT * FROM RCS_VENTILATOR_SCHEDULING_CHECKLIST WHERE V_ID = @V_ID"; 
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp = new Dapper.DynamicParameters();
                dp.Add("V_ID", id);

                var getData = this.DBLink.DBA.getSqlDataTable<VENTILATORModel>(sql, dp).FirstOrDefault();

                if (getData.DATASTATUS == "9")
                {
                    vm.MV_NO = "新增檢核表";
                    vm.V_TYPE = "";
                    vm.V_STATUS = "";

                    CheckDATASTATUS = false;
                }
                else
                {
                    sql = "SELECT * FROM RCS_VENTILATOR_SETTINGS WHERE DEVICE_NO =  @DEVICE_NO";
                    dp = new Dapper.DynamicParameters();
                    dp.Add("DEVICE_NO", getData.MV_NO);
                    var getDEVICE_MODEL = this.DBLink.DBA.getSqlDataTable<DB_RCS_VENTILATOR_SETTINGS>(sql, dp).FirstOrDefault()?.DEVICE_MODEL ?? "";

                    vm.MV_NO = getData.MV_NO;
                    vm.V_TYPE = getData.V_TYPE;
                    vm.V_STATUS = getData.V_STATUS;
                    vm.V_MODE = getData.V_MODE;
                    vm.RECORD_DATE = getData.RECORD_DATE;
                    vm.DEVICE_MODEL = getDEVICE_MODEL;

                    sql = "SELECT * FROM RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL WHERE V_ID = @V_ID";
                    dp = new Dapper.DynamicParameters();
                    dp.Add("V_ID", id);

                    getCheckTypeList = this.DBLink.DBA.getSqlDataTable<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL>(sql, dp).ToList();

                    if (getData.DATASTATUS == "0")
                    {
                        CheckDATASTATUS = false;
                    }
                }
            }
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = this.DBLink.DBA.lastError;
            }
            else
            {
                vm.CheckListDatas = new List<CheckListDetailModel>();
                foreach (var nameData in CheckNameList)
                {
                    CheckListDetailModel addData = new CheckListDetailModel();

                    addData.ITEM_NAME = nameData.P_NAME;
                    addData.ITEM_NAME_CODE = nameData.P_VALUE;
                    addData.ITEM_TYPE = int.TryParse(CheckTypeList.Where(x => x.P_NAME == nameData.P_VALUE).FirstOrDefault()?.P_VALUE, out addData.ITEM_TYPE) ? addData.ITEM_TYPE : 0;
                    if (string.IsNullOrWhiteSpace(id) || !CheckDATASTATUS)
                    {
                        addData.ITEM_VALUE = "";

                    }
                    else
                    {
                        addData.ITEM_VALUE = getCheckTypeList.Where(x => x.ITEM_NAME == nameData.P_VALUE).FirstOrDefault().ITEM_VALUE;

                        if (addData.ITEM_TYPE == 4)
                        {
                            if (addData.ITEM_VALUE == "N/A")
                            {
                                addData.Check = "N/A";
                            }
                            else
                            {
                                addData.Check = "是";
                            }
                        }

                    }
                    vm.CheckListDatas.Add(addData);
                }
            } 

            rm.attachment = vm;
            return rm;
        }
 
        public RESPONSE_MSG saveVENTILATORSCHEDULING(IVENTILATOR_DETAIL_FORM_BODY form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "saveVENTILATORSCHEDULING";
            VENTILATORModel getModel = form.model;
            bool is_MODIFY = string.IsNullOrWhiteSpace(getModel.V_ID) ? false : true; 
            string sql = "";
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters(); 
            List<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST> addMinList = new List<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST>();
            List<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST> updateMinList = new List<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST>();
            #region 主表

            if (!is_MODIFY)
            { 
                getModel.V_ID = this.DBLink.GetFixedStrSerialNumber(); 
                DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST addData = new DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST()
                {
                    V_ID = getModel.V_ID,
                    RECORD_DATE = getModel.RECORD_DATE,
                    MV_NO = form.model.MV_NO,
                    V_TYPE = form.model.V_TYPE,
                    V_STATUS = form.model.V_STATUS,
                    V_MODE = form.model.V_MODE,
                    DATASTATUS = form.model.DATASTATUS,
                    CREATE_ID = form.payload.user_id,
                    CREATE_NAME = form.payload.user_name,
                    CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                    MODIFY_ID = form.payload.user_id,
                    MODIFY_NAME = form.payload.user_name,
                    MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                }; 
                addMinList.Add(addData);  
            }
            else
            { 
                sql = "";
                dp = new Dapper.DynamicParameters();
                sql = "select * from RCS_VENTILATOR_SCHEDULING_CHECKLIST where V_ID = @V_ID";
                dp.Add("V_ID", form.model.V_ID); 
                var updateData = this.DBLink.DBA.getSqlDataTable<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST>(sql, dp); 
                foreach (var item in updateData)
                {
                    item.MODIFY_ID = form.payload.user_id;
                    item.MODIFY_NAME = form.payload.user_name;
                    item.MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss);
                    item.V_ID = getModel.V_ID;
                    item.MV_NO = form.model.MV_NO;
                    item.V_TYPE = form.model.V_TYPE;
                    item.V_STATUS = form.model.V_STATUS;
                    item.V_MODE = form.model.V_MODE;
                    item.DATASTATUS = form.model.DATASTATUS; 
                }
                updateMinList.AddRange(updateData); 
            }
            #endregion 
            #region 明細


            List<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL> list_DTL = new List<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL>();

            foreach (var data in getModel.RCS_VENTILATOR_SCHEDULING_CHECKLIST)
            {
                DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL addData = new DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL();

                addData.V_ID = getModel.V_ID;
                addData.ITEM_VALUE = data.ITEM_VALUE;
                addData.ITEM_NAME = data.ITEM_NAME_CODE;

                list_DTL.Add(addData);
            }
            #endregion 
            this.DBLink.DBA.BeginTrans();
            #region 儲存資料
            this.DBLink.DBA.DBExecInsert<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST>(addMinList);
            this.DBLink.DBA.DBExecUpdate<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST>(updateMinList);
            if (is_MODIFY)
            {
                this.DBLink.DBA.DBExecDelete<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL>(list_DTL);
            }
            this.DBLink.DBA.DBExecInsert<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL>(list_DTL);

            if (this.DBLink.DBA.hasLastError)
            {
                LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
                this.DBLink.DBA.Rollback();
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = this.DBLink.DBA.lastError;
            }
            else
            {
                this.DBLink.DBA.Commit();
            }
            #endregion
            this.DBLink.DBA.Close(); 
            rm.attachment = getModel;
            return rm;
        }

        public RESPONSE_MSG delVENTILATORSCHEDULING(IVENTILATOR_DETAIL_FORM_BODY form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "delVENTILATORSCHEDULING";  
            this.DBLink.DBA.BeginTrans(); 
            this.DBLink.DBA.DBExecDelete<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST>(form.model); 
            DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL addData = new DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL()
            {
                V_ID = form.model.V_ID
            };
            this.DBLink.DBA.DBExecDelete<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL>(new List<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL>() { addData });
            if (this.DBLink.DBA.hasLastError)
            {
                LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
                this.DBLink.DBA.Rollback();
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "刪除失敗!" + this.DBLink.DBA.lastError; 
            }
            else
            {
                this.DBLink.DBA.Commit(); 
            }
            this.DBLink.DBA.Close();  
            rm.attachment = "刪除成功!";
            return rm;
        }

        public RESPONSE_MSG exportVENTILATORExcel(IVENTILATORExcel_FORM_BODY form)
        {
            throw new NotImplementedException();
        }
    }

    public class USER_FORM_BODY : AUTH, IUSER_FORM_BODY
    {
        public DB_RCS_SYS_PARAMS model { get; set; }
    }

    public class getUserMaintain_List_BODY : AUTH, IGetUserMaintain_List_BODY
    {
        public bool isYourSelfExcept { get; set; }

        public string P_ID { get; set; }
    }

    public class VENTILATOR_FORM_BODY : AUTH, IVENTILATOR_FORM_BODY
    {
        public DB_RCS_VENTILATOR_SETTINGS model { get; set; }
    }

    public class Param_Setting_FORM_BODY : AUTH, IParam_Setting_FORM_BODY
    {
        public DB_RCS_SYS_PARAMS model { get; set; }

        public List<DB_RCS_SYS_PARAMS> list { get; set; }
    }

    public class VENTILATOR_DETAIL_FORM_BODY : AUTH, IVENTILATOR_DETAIL_FORM_BODY
    {
        public VENTILATORModel model { get; set; }

    }

    public class VENTILATORExcel_FORM_BODY : IVENTILATORExcel_FORM_BODY
    {
        public string year { get; set; }
        public string month { get; set; }
    }

}
