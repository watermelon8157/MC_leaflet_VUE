using System.Collections.Generic;
using RCS_Data.Models.DB;
using RCSData.Models;
using RCS_Data.Models.ViewModels;

namespace RCS_Data.Controllers.RtTakeoff
{
    public interface Interface
    {
        RESPONSE_MSG RtTakeOff_Save(Form_RtTakeOff_Save form);
        List<VM_RTTakeoffAssess> changeJson(List<VM_RTTakeoffAssess> List);
    }

    public interface IRtTakeoffController
    {
        RESPONSE_MSG RtTakeOff_Save(Form_RtTakeOff_Save form);
        object RtTakeOffDetail(Form_RtTakeOffDetail form);
    }
    public interface IForm_RtTakeOffDetail
    {
        string TK_ID { get; set; }
        string chart_no { get; set; }
        string ipd_no { get; set; }

    }
    public interface IForm_RtTakeoffList
    {
        string pSDate { get; set; }
        string pEDate { get; set; }
        string pipd_no { get; set; }
        string pId { get; set; }
    }
}
