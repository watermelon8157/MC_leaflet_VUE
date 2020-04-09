using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.DB
{
    public interface IDBA<T>
    {
        List<T> InsertTenpMaster(string record_id, string record_date, IPDPatientInfo pat_info, UserInfo user_info);
    }
}
