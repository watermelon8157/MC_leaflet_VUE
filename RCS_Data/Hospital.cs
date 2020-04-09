using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data
{

    /// <summary> 病房區域清單 </summary>
 
    public class SLC_GetExam_2
    {
        ///<summary>病歷號</summary>
        public string CHART { set; get; }
        ///<summary>來源</summary>
        public string SOURCE { set; get; }
        ///<summary>檢查單別</summary>
        public string TYPE { set; get; }
        ///<summary>檢查單別名稱</summary>
        public string TYPE_NAME { set; get; }
        ///<summary>狀態</summary>
        public string STAT { set; get; }
        ///<summary>狀態描述</summary>
        public string STATNAME { set; get; }
        ///<summary>開單醫師</summary>
        public string ORDERDR { set; get; }
        ///<summary>開單醫師姓名</summary>
        public string ORDERDRNAME { set; get; }
        ///<summary>開單時間</summary>
        public string ORDERTIME { set; get; }
        ///<summary>登錄時間</summary>
        public string LOGINTIME { set; get; }
        ///<summary>檢查序號</summary>
        public string SEQ { set; get; }
        ///<summary>工作序號</summary>
        public string E_SEQ { set; get; }
    }

    public class SLC_GetExam_3
    {
        ///<summary>病歷號</summary>
        public string CHART { set; get; }
        ///<summary>來源</summary>
        public string SOURCE { set; get; }
        ///<summary>檢查單別</summary>
        public string TYPE { set; get; }
        ///<summary>檢查單別名稱</summary>
        public string TYPE_NAM { set; get; }
        ///<summary>狀態</summary>        
        public string STAT { set; get; }
        ///<summary>狀態描述</summary>
        public string STATNAME { set; get; }
        ///<summary>開單醫師</summary>
        public string ORDERDR { set; get; }
        ///<summary>開單醫師姓名</summary>
        public string ORDERDRNAME { set; get; }
        ///<summary>開單時間</summary>
        public string ORDERTIME { set; get; }
        ///<summary>登錄時間</summary>
        public string LOGINTIME { set; get; }
        ///<summary>檢查序號</summary>
        public string SEQ { set; get; }
        ///<summary>工作序號</summary> 
        public string E_SEQ { set; get; }
        ///<summary>判讀醫師</summary>
        public string STUDYDR { set; get; }
        ///<summary>判讀醫師姓名</summary>
        public string STUDYDRNAME { set; get; }
        ///<summary>輸入人員</summary>
        public string ENTERDR { set; get; }
        ///<summary>輸入人員姓名</summary>
        public string ENTERDRNAME { set; get; }
        ///<summary>完成時間</summary>
        public string COMPLETTIME { set; get; }
        ///<summary>輸入時間</summary>
        public string ENTERTIME { set; get; }
        ///<summary>報告內容</summary>
        public string REPORT { set; get; }
    }

    public class SLC_GetExam_6
    {
        /// <summary>病歷號</summary>
        public string CHART { set; get; }
        /// <summary>檢查單別</summary>
        public string TYPE { set; get; }
        /// <summary>檢查單別名稱</summary>
        public string TYPE_NAM { set; get; }
        /// <summary>狀態</summary>
        public string STAT { set; get; }
        /// <summary>狀態描述</summary>
        public string STAT_DESC { set; get; }
        /// <summary>開單時間</summary>
        public string CHARGS { set; get; }
        /// <summary>登錄時間</summary>
        public string LOGIN_TIM { set; get; }
        /// <summary>檢查序號</summary>
        public string SEQ { set; get; }
        public string E_SEQ { set; get; }
    }

   

    /// <summary>
    /// 呼吸器使用保養紀錄
    /// </summary>
    public class DeviceDetail
    {
        /// <summary>流水號</summary>
        public string R_NO { set; get; }
        /// <summary>呼吸器編號</summary>
        public string DEVICE_NO { set; get; }
        /// <summary>對應主檔流水號</summary>
        public string DEVICE_SEQ { set; get; }
        /// <summary>狀態{1:使用中,2:維修,3:外借,4:停用}</summary>
        public string STATUS { set; get; }
        /// <summary>起始日</summary>
        public string START_DATE { set; get; }
        /// <summary>截止日</summary>
        public string END_DATE { set; get; }
        /// <summary>備註</summary>
        public string REMARK { set; get; }
        /// <summary>建立者ID</summary>
        public string CREATE_ID { set; get; }
        /// <summary>建立者名稱</summary>
        public string CREATE_NAME { set; get; }
        /// <summary>建立時間</summary>
        public string CREATE_DATE { set; get; }
        /// <summary>修改者ID</summary>
        public string MODIFY_ID { set; get; }
        /// <summary>修改者名稱</summary>
        public string MODIFY_NAME { set; get; }
        /// <summary>修改時間</summary>
        public string MODIFY_DATE { set; get; }
        /// <summary>運轉時數</summary>
        public string RUNNING_HR { set; get; }
        /// <summary>機器檢測代碼</summary>
        public string SYSTEM_TEST { set; get; }
        /// <summary>保養清潔代碼</summary>
        public string CLEAN_STATUS { set; get; }
    }


    /// <summary>
    /// 查詢ICD10
    /// </summary>
    public class ws_getIPHICD
    {
        public string DIAG_TYPE { set; get; }
        public double SEQ { set; get; }
        public string ICD { set; get; }
        public string C_NAME { set; get; }
        public string E_NAME { set; get; }
    }

    /// <summary>
    /// 插管資料
    /// </summary>
    public class WS_getLastON_ENDOResponse
    {
        /// <summary> RT_ACCOUNT </summary>
        public string RT_ACCOUNT { get; set; }
        /// <summary> 插管開始日期 </summary>
        public string IN_DATE { get; set; }
        /// <summary> 拔管日期 </summary>
        public string OUT_DATE { get; set; }
        /// <summary> 經由(經口,經鼻,氣管切開術) </summary>
        public string PATH { get; set; }
        /// <summary> 插管直徑 </summary>
        public string DIAMETER { get; set; }
        /// <summary> 插管深度 </summary>
        public string DEPTH { get; set; }
        /// <summary> GCS </summary>
        public string GCS { get; set; }
        /// <summary> IND_CODE </summary>
        public string IND_CODE { get; set; }
        /// <summary> CON_CODE </summary>
        public string CON_CODE { get; set; }
        /// <summary> REASON </summary>
        public string REASON { get; set; }
        /// <summary> PHYSICIAN </summary>
        public string PHYSICIAN { get; set; }
        /// <summary> ASSISTANT </summary>
        public string ASSISTANT { get; set; }
        /// <summary> CONSTRAINT </summary>
        public string CONSTRAINT { get; set; }
        /// <summary> SEDATIVE </summary>
        public string SEDATIVE { get; set; }
        /// <summary> SOLAXIN </summary>
        public string SOLAXIN { get; set; }
        /// <summary> COM_DATE1 </summary>
        public string COM_DATE1 { get; set; }
        /// <summary> COM_CODE1 </summary>
        public string COM_CODE1 { get; set; }
        /// <summary> COM_DATE2 </summary>
        public string COM_DATE2 { get; set; }
        /// <summary> COM_CODE2 </summary>
        public string COM_CODE2 { get; set; }
        /// <summary> COM_DATE3 </summary>
        public string COM_DATE3 { get; set; }
        /// <summary> COMPLICATION3 </summary>
        public string COMPLICATION3 { get; set; }
        /// <summary> RESULT </summary>
        public string RESULT { get; set; }
        /// <summary> 新增人員 </summary>
        public string RTP { get; set; }
        /// <summary> 新增時間 </summary>
        public string RTT { get; set; }
        /// <summary> 區域 </summary>
        public string LOC { get; set; }
        /// <summary> 房號 </summary>
        public string ROOM { get; set; }
        /// <summary> 床號 </summary>
        public string BED { get; set; }
    }

    /// <summary>
    /// 查詢最後一次呼吸器設定
    /// </summary>
    public class WS_getLastSetRespResult
    {
        /// <summary>RT帳號</summary>
        public string RT_ACCOUNT { set; get; }

        /// <summary>病歷號</summary>
        public string CHART { set; get; }

        /// <summary>住院帳號</summary>
        public string IPD_ACCOUNT { set; get; }

        /// <summary> 病人來源 </summary>
        public string SOURCE { set; get; }

        /// <summary>開始時間</summary>
        public string START_DATE { set; get; }

        /// <summary>結束時間</summary>
        public string STOP_DATE { set; get; }

        public string IND_CODE { set; get; }

        /// <summary>嘗試脫離時間</summary>
        public string START_WEANING { set; get; }

        /// <summary>結果</summary>
        public string RESULT { set; get; }

        public string COM_DATE1 { set; get; }

        public string COM_CODE1 { set; get; }

        public string COM_DATE2 { set; get; }

        public string COM_CODE2 { set; get; }

        public string COM_DATE3 { set; get; }

        public string COMPLICATION3 { set; get; }

        /// <summary>RT ID</summary>
        public string RTP { set; get; }

        public string RTT { set; get; }

        public string CORF_COD { set; get; }

        /// <summary>院區</summary>
        public string LOC { set; get; }

        /// <summary>房號</summary>
        public string ROOM { set; get; }

        /// <summary>床號</summary>
        public string BED { set; get; }

        /// <summary>更新時間</summary>
        public string UPDATE_DATE { set; get; }

        /// <summary>醫師ID(病患列印使用，RT可修改)</summary>
        public string SET_DR { set; get; }
    }

    /// <summary>
    /// 查詢最後一次氧氣治療設定
    /// </summary>
    public class ws_getLastOXYGEN_CUREResponse
    {
        /// <summary>RT帳號</summary>
        public string RT_ACCOUNT { set; get; }
        /// <summary>開始時間</summary>
        public string START_DATE { set; get; }
        /// <summary>結束時間</summary>
        public string END_DATE { set; get; }
        /// <summary>治療類型</summary>
        public string CURE_TYPE { set; get; }
        /// <summary>治療狀況</summary>
        public string CURE_STATE { set; get; }
        /// <summary>備註</summary>
        public string REMARKS { set; get; }
        /// <summary>新增人員</summary>
        public string RTP { set; get; }
        /// <summary>新增時間</summary>
        public string RTT { set; get; }
        /// <summary>區域</summary>
        public string LOC { set; get; }
        /// <summary>房號</summary>
        public string ROOM { set; get; }
        /// <summary>床號</summary>
        public string BED { set; get; }
    }

    /// <summary>
    /// 取得呼吸器編號
    /// </summary>
    public class ws_getRESP
    {
        /// <summary>呼吸器編號</summary>
        public string ID { set; get; }
        /// <summary>房號</summary>
        public string ROOM { set; get; }
        /// <summary>呼吸器型號</summary>
        public string TYPE { set; get; }
        /// <summary>日期</summary>
        public string EFF_DAT { set; get; }
        /// <summary>到期日</summary>
        public string EXP_DAT { set; get; }

        /// <summary> 下拉選單用值 </summary>
        public string field_value
        {
            get
            {
                return ID + "|" + TYPE;
            }
        }
    }

    /// <summary>
    /// 使用呼吸器患者資料
    /// </summary>
    public class WS_GetPtData
    {
        /// <summary>RT帳號</summary>
        public string RT_ACCOUNT { set; get; }
        /// <summary>住院帳號</summary>
        public string ACC_NO { set; get; }
        /// <summary>病歷號</summary>
        public string CHART_NO { set; get; }
        /// <summary>住院時間</summary>
        public string ADMIN_TIME { set; get; }
        /// <summary>出院時間</summary>
        public string DISCHARGE_TIME { set; get; }
        /// <summary>區域</summary>
        public string LOC { set; get; }
        /// <summary>房號</summary>
        public string ROOM { set; get; }
        /// <summary>床號</summary>
        public string BED { set; get; }
        /// <summary>主治醫生代碼</summary>
        public string VS1 { set; get; }
        /// <summary></summary>
        public string VS1_DEPART { set; get; }
        /// <summary>來源</summary>
        public string SOURCE { set; get; }
        /// <summary>性別</summary>
        public string SEX { set; get; }
        /// <summary>年齡</summary>
        public string AGE { set; get; }
        /// <summary>姓名</summary>
        public string NAME { set; get; }
        /// <summary>出生民國年</summary>
        public string BORN_YYMMDD { set; get; }
        /// <summary>主治醫生姓名</summary>
        public string EMP_NAM { set; get; }
        /// <summary>科別</summary>
        public string NAM_C { set; get; }
        /// <summary>身分證字號</summary>
        public string IDNO { set; get; }
    }


    /// <summary>
    /// 查詢是否需授權者
    /// </summary>
    public class WS_getRtGrantExist
    {
        public string GRANT_EXIST { get; set; }
    }

    /// <summary>
    /// 查詢授權者清單
    /// </summary>
    public class WS_getRtGrantList
    {
        /// <summary>
        /// 被授權者代碼
        /// </summary>
        public string GET_EMP_NO { get; set; }
        /// <summary>
        /// 被授權者名字
        /// </summary>
        public string GET_EMP_NAM { get; set; }
        /// <summary>
        /// 授權者代碼
        /// </summary>
        public string GIVE_EMP_NO { get; set; }
        /// <summary>
        /// 授權者名字
        /// </summary>
        public string GIVE_EMP_NAM { get; set; }
    }

    /// <summary>
    /// 查詢基本資料:呼吸治療的併發症
    /// </summary>
    public class GetCOMPLICATION
    {
        /// <summary>
        /// 回傳值
        /// </summary>
        public string CODE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TYPE { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string NAME { get; set; }
    }

    /// <summary>
    /// 查詢基本資料:ON ENDO 的禁忌症
    /// </summary>
    public class GetCONTRAINDICATIONS
    {
        /// <summary>
        /// 回傳值
        /// </summary>
        public string CODE { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string NAME { get; set; }
    }

    /// <summary>
    /// 查詢基本資料:引發呼吸哀竭的原因
    /// </summary>
    public class GetCORF
    {
        /// <summary>
        /// 回傳值
        /// </summary>
        public string CODE { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string CAUSE { get; set; }
    }

    /// <summary>
    /// 查詢基本資料:呼吸治療的適應症
    /// </summary>
    public class GetINDICATION
    {
        /// <summary>
        /// 回傳值
        /// </summary>
        public string CODE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TYPE { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string NAME { get; set; }
    }

}