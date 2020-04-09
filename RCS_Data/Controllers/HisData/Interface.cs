using System.Collections.Generic;
using RCS_Data.Models.DB;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace RCS_Data.Controllers.HisData
{
    public interface Interface
    {
        RESPONSE_MSG Search_io(ref List<IO_PUT> finalIO_PUTList, string io_sdate, string io_edate, IPDPatientInfo pat_info, IWebServiceParam iwp);
    }

    public interface IHisDataController
    {
        List<IO_PUT> search_io(FormSearch_io form);
    }
  
}
