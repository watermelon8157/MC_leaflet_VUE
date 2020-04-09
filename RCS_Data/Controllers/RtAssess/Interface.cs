using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Controllers.RtAssess
{
    public interface Interface
    {
        RESPONSE_MSG CPTRecordList(ref List<RCS_CPT_DTL_NEW_ITEMS> cpt_dtl_new_items, string pSDate, string pEDate, string ipd_no);

        RCS_CPT_DTL_NEW_ITEMS CPTLastData(IPDPatientInfo pat_info);
        RESPONSE_MSG CPTAssess_Save(Form_CPTAssess_Save form);

        RESPONSE_MSG CPTRecordDelete(List<string> RT_ID_LIST, UserInfo user_inf);

        List<RCS_CPT_DTL_NEW_ITEMS> changeJson(List<RCS_CPT_DTL_NEW_ITEMS> List);
    }

    public interface IRtAssessController
    {
        List<RCS_CPT_DTL_NEW_ITEMS> CPTRecordList(Form_CPTRecordList form);

        RESPONSE_MSG CPTAssess_Save(Form_CPTAssess_Save form);

        RESPONSE_MSG CPTRecordDelete(Form_CPTDetail form);

        object CPTDetail(Form_CPTDetail form);
        
    } 
    public interface IForm_CPTRecordList
    {
        string pSDate { get; set; } 
        string pEDate { get; set; }
        string pipd_no { get; set; }
        string pId { get; set; }
        string pLast { get; set; }
    }
}
