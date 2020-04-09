using System.Collections.Generic;
using RCS_Data.Models.DB;
using RCSData.Models;
using RCS_Data.Models.ViewModels;

namespace RCS_Data.Controllers.System
{
    public interface Interface
    {
        /// <summary>
        /// 儲存使用者
        /// </summary>
        /// <returns></returns>
        RESPONSE_MSG saveUser(IUSER_FORM_BODY form);
        /// <summary>
        /// 取得使用者清單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        RESPONSE_MSG getUserMaintainList(IGetUserMaintain_List_BODY form);
        /// <summary>
        /// 使用者群組清單
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG getUserGroupList();  
        /// <summary>
        /// getDeviceType
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG getDeviceType( );
        /// <summary>
        /// saveVENTILATOR
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG saveVENTILATOR(IVENTILATOR_FORM_BODY form);
        /// <summary>
        /// getDeviceList
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG getDeviceList(bool pShowDel, string pDeviceNo = "");
        /// <summary>
        /// 參數資料來源
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG ParamData(string p_model, string p_group, string P_ID = "");
        /// <summary>
        /// 參數資料來源
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG saveParamSetting(IUSER_FORM_BODY form);
        /// <summary>
        /// 儲存表單管理排序
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG saveParamSettingSort(IParam_Setting_FORM_BODY form);
        /// <summary>
        /// 參數資料來源
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG GetVENTILATOR(string sDate, string eDate);
        /// <summary>
        /// GetSelectList
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG GetSelectList( );
        /// <summary>
        /// 取得呼吸器表單詳細
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG GetVENTILATORCheckList(string id);
        /// <summary>
        /// 取得呼吸器表單詳細
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG saveVENTILATORSCHEDULING(IVENTILATOR_DETAIL_FORM_BODY form);
        /// <summary>
        /// DEL呼吸器表單詳細
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG delVENTILATORSCHEDULING(IVENTILATOR_DETAIL_FORM_BODY form);
        /// <summary>
        /// exportVENTILATORExcel
        /// </summary> 
        /// <returns></returns>
        RESPONSE_MSG exportVENTILATORExcel(IVENTILATORExcel_FORM_BODY form);
    }

    public interface ISystemController
    {
        /// <summary>
        /// 儲存使用者
        /// </summary>
        /// <returns></returns>
        string saveUser(USER_FORM_BODY form);
        /// <summary>
        /// 取得使用者清單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        List<DB_RCS_SYS_PARAMS> getUserMaintainList(getUserMaintain_List_BODY form);
        /// <summary>
        /// 使用者群組清單
        /// </summary> 
        /// <returns></returns>
        List<DB_RCS_SYS_PARAMS> getUserGroupList();
        /// <summary>
        /// getDevice
        /// </summary> 
        /// <returns></returns>
        List<DeviceMaster> getDevice(string pDeviceNo);
        /// <summary>
        /// getDeviceType
        /// </summary> 
        /// <returns></returns>
        List<DB_RCS_SYS_PARAMS> getDeviceType();
        /// <summary>
        /// saveVENTILATOR
        /// </summary> 
        /// <returns></returns>
        string saveVENTILATOR(VENTILATOR_FORM_BODY form);
        /// <summary>
        /// 呼吸器清單
        /// </summary> 
        /// <param name="pDeviceNo">呼吸器編號DEVICE_NO</param>
        /// <returns></returns>
        List<DeviceMaster> getDeviceList(bool pShowDel, string pDeviceNo = "");
        /// <summary>
        /// 參數資料來源
        /// </summary> 
        /// <returns></returns>
        List<DB_RCS_SYS_PARAMS> ParamData(string p_model, string p_group, string P_ID = "");
        /// <summary>
        /// 參數資料來源
        /// </summary> 
        /// <returns></returns>
        string saveParamSetting(USER_FORM_BODY form);
        /// <summary>
        /// 儲存表單管理排序
        /// </summary> 
        /// <returns></returns>
        string saveParamSettingSort(Param_Setting_FORM_BODY form);
        /// <summary>
        /// 參數資料來源
        /// </summary> 
        /// <returns></returns>
        List<VENTILATORGroupModel> GetVENTILATOR(string sDate, string eDate);
        /// <summary>
        /// GetSelectList
        /// </summary> 
        /// <returns></returns>
        List<DB_RCS_VENTILATOR_SETTINGS> GetSelectList();
        /// <summary>
        /// 取得呼吸器表單詳細
        /// </summary> 
        /// <returns></returns>
        VENTILATORViewModel GetVENTILATORCheckList(string id);
        /// <summary>
        /// 取得呼吸器表單詳細
        /// </儲存呼吸器表單詳細> 
        /// <returns></returns>
        VENTILATORModel saveVENTILATORSCHEDULING(VENTILATOR_DETAIL_FORM_BODY form);
        /// <summary>
        /// DEL呼吸器表單詳細
        /// </summary> 
        /// <returns></returns>
        string delVENTILATORSCHEDULING(VENTILATOR_DETAIL_FORM_BODY form);
        /// <summary>
        /// 取得會診資料
        /// </summary> 
        /// <returns></returns>
        List<PatProgress> getCount(string pSDate, string pEDate);
    }

    public interface IUSER_FORM_BODY
    {
        UserInfo user_info { get;  }

        IPDPatientInfo pat_info { get; set; }

        PAYLOAD payload { get; set; }

        DB_RCS_SYS_PARAMS model { get; set; }
    }

    public interface IGetUserMaintain_List_BODY
    {
        UserInfo user_info { get; }

        IPDPatientInfo pat_info { get; set; }

        PAYLOAD payload { get; set; }

        bool isYourSelfExcept { get; set; }

        string P_ID { get; set; }
    }

    public interface IVENTILATOR_FORM_BODY
    {
        UserInfo user_info { get; }

        IPDPatientInfo pat_info { get; set; }

        PAYLOAD payload { get; set; }

        DB_RCS_VENTILATOR_SETTINGS model { get; set; }
    }

    public interface IParam_Setting_FORM_BODY
    {
        UserInfo user_info { get; }

        IPDPatientInfo pat_info { get; set; }

        PAYLOAD payload { get; set; }

        DB_RCS_SYS_PARAMS model { get; set; }

        List<DB_RCS_SYS_PARAMS> list { get; set; }
    }

    public interface IVENTILATOR_DETAIL_FORM_BODY
    {
        UserInfo user_info { get; }

        IPDPatientInfo pat_info { get; set; }

        PAYLOAD payload { get; set; }

        VENTILATORModel model { get; set; }

    }

    public interface IVENTILATORExcel_FORM_BODY
    {
        string year { get; set; }
        string month { get; set; }
    }
  
}
