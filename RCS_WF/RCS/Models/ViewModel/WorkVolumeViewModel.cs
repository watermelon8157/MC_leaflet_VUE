using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCS.Models
{
    /// <summary>
    /// ViewModel
    /// </summary>
    public class WorkVolumeViewModel
    {
        /// <summary>
        /// 查詢起日期
        /// </summary>
        public string sDate { get; set; }
        /// <summary>
        /// 查詢訖日期
        /// </summary>
        public string eDate { get; set; }
        /// <summary>
        /// 顯示方式(1:護理站2:呼吸治療師)
        /// </summary>
        public string showType { get; set; }
        /// <summary>
        /// 顯示全部選項
        /// </summary>
        public bool showAll { get; set; }
        /// <summary>
        /// 顯示項目清單(至少選擇一項，不選擇則全部顯示)
        /// </summary>
        public List<string> itemList { get; set; }
    }

    /// <summary>
    /// 工作量統計
    /// </summary>
    public class WorkVolume
    {
        public List<Dictionary<string, object>> WorkVolumeList { get; private set; }
        private WorkVolumeViewModel set { get; set; }
        public WorkVolume(WorkVolumeViewModel pSet)
        {
            set = pSet;
        }

        public List<IWorkItemFunc> getWorkVolume()
        {
            List<IWorkItemFunc> data = new List<IWorkItemFunc>();
            if(set.itemList.Contains("MV"))
                data.Add(setWorkVolume(new WorkVolume_MV()));//MV
            return data;
        }

        private IWorkItemFunc setWorkVolume(IWorkItemFunc item)
        {
            item.additionalFunc();
            return item;
        }

    }

    #region IWorkItemFunc 工作列介面類別


    public interface IWorkItemFunc
    {
        string csName { get; }
        /// <summary>
        /// 項目名稱
        /// </summary>
        string itemName { get; }
        /// <summary>
        /// 項目代碼
        /// </summary>
        string itemCode { get; }
        /// <summary>
        /// 結果類型
        /// </summary>
        WorkItemResultType resultType { get; }
        /// <summary>
        /// 取得SQL
        /// </summary>
        /// <returns></returns>
        string getSQL();
        /// <summary>
        /// 額外方法
        /// </summary>
        void additionalFunc();
        /// <summary>
        /// 結果
        /// </summary>
        List<WorkItem> result { get; set; }

    }
    /// <summary>
    /// 項目結果類型
    /// </summary>
    public enum WorkItemResultType
    {
        _int, //數值
        _str, //字串
    }

    public class WorkItem
    {
        /// <summary>
        /// 護理站代碼
        /// </summary>
        public string cost_code { get; set; }
        /// <summary>
        /// 護理站名稱
        /// </summary>
        public string cost_desc { get; set; }
        /// <summary>
        /// 科別代碼
        /// </summary>
        public string dept_code { get; set; }
        /// <summary>
        /// 科別名稱
        /// </summary>
        public string dept_desc { get; set; }
        /// <summary>
        /// 床號
        /// </summary>
        public string bed_no { get; set; }
        /// <summary>
        /// 記錄單流水號PK
        /// </summary>
        public string record_id { get; set; }
        /// <summary>
        /// 記錄者員編
        /// </summary>
        public string op_id { get; set; }
        /// <summary>
        /// 記錄者姓名
        /// </summary>
        public string op_name { get; set; }
        /// <summary>
        /// 顯示結果
        /// </summary>
        public object result { get; set; }
    }

    #endregion

    #region interface 實作介面
    public abstract class WorkVolume_Basic : IWorkItemFunc
    {
        public virtual string csName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string itemCode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string itemName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public List<WorkItem> result
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual WorkItemResultType resultType
        {
            get
            {
                return WorkItemResultType._int;
            }
        }

        public virtual void additionalFunc()
        {
            string actionNmae = "additionalFunc";
            string sql = this.getSQL();
            SQLProvider SQL = new SQLProvider();
            this.result = SQL.DBA.getSqlDataTable<WorkItem>(sql);
            if (SQL.DBA.hasLastError)
            {
                this.result = new List<WorkItem>();
                Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, actionNmae, this.csName);
            }
        }

        public virtual string getSQL()
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// MV
    /// </summary>
    public class WorkVolume_MV : WorkVolume_Basic, IWorkItemFunc
    {
        public override string csName
        {
            get
            {
                return "WorkVolume_MV";
            }
        }

        public override string itemCode
        {
            get
            {
                return "MV";
            }
        }

        public override string itemName
        {
            get
            {
                return "MV";
            }
        }



        public override string getSQL()
        {
            return string.Concat("","");
        }
    }

    /// <summary>
    /// 脫離MV(拔管)
    /// </summary>
    public class WorkVolume_Weaning_MV_Intube : WorkVolume_Basic, IWorkItemFunc
    {
        public override string csName
        {
            get
            {
                return "WorkVolume_Weaning_MV_Intube";
            }
        }


        public override string itemCode
        {
            get
            {
                return "Weaning_MV_Intube";
            }
        }

        public override string itemName
        {
            get
            {
                return "脫離MV(拔管)";
            }
        }

        public override string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// 脫離MV
    /// </summary>
    public class WorkVolume_Weaning_MV : WorkVolume_Basic, IWorkItemFunc
    {
        public override string csName
        {
            get
            {
                return "WorkVolume_Weaning_MV";
            }
        }

        public override string itemCode
        {
            get
            {
                return "Weaning_MV";
            }
        }

        public override string itemName
        {
            get
            {
                return "脫離MV";
            }
        }

        public override string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// 負壓N-MV
    /// </summary>
    public class WorkVolume_N_MV : WorkVolume_Basic,IWorkItemFunc
    {
        public override string csName
        {
            get
            {
                return "WorkVolume_N_MV";
            }
        }

        public override string itemCode
        {
            get
            {
                return "N_MV";
            }
        }

        public override string itemName
        {
            get
            {
                return "負壓N-MV";
            }
        }

        public override string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// 脫離N-MV
    /// </summary>
    public class WorkVolume_Weaning_N_MV : WorkVolume_Basic, IWorkItemFunc
    {
        public override string csName
        {
            get
            {
                return "WorkVolume_Weaning_N_MV";
            }
        }

        public override string itemCode
        {
            get
            {
                return "Weaning_N_MV";
            }
        }

        public override string itemName
        {
            get
            {
                return "脫離N-MV";
            }
        }

        public override string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// Bipap-M
    /// </summary>
    public class WorkVolume_Bipap_M : IWorkItemFunc
    {
        public string csName
        {
            get
            {
                return "WorkVolume_Bipap_M";
            }
        }
        public List<WorkItem> result { get; set; }

        public string itemCode
        {
            get
            {
                return "Bipap_M";
            }
        }

        public string itemName
        {
            get
            {
                return "Bipap-M";
            }
        }

        public WorkItemResultType resultType
        {
            get
            {
                return WorkItemResultType._int;
            }
        }

        public void additionalFunc()
        {
            string actionNmae = "additionalFunc";
            string sql = this.getSQL();
            SQLProvider SQL = new SQLProvider();
            this.result = SQL.DBA.getSqlDataTable<WorkItem>(sql);
            if (SQL.DBA.hasLastError)
            {
                this.result = new List<WorkItem>();
                Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, actionNmae, this.csName);
            }
        }

        public string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// 脫離Bipap
    /// </summary>
    public class WorkVolume__Weaning_Bipap_M : IWorkItemFunc
    {
        public string csName
        {
            get
            {
                return "WorkVolume__Weaning_Bipap_M";
            }
        }
        public List<WorkItem> result { get; set; }

        public string itemCode
        {
            get
            {
                return "Weaning_Bipap_M";
            }
        }

        public string itemName
        {
            get
            {
                return "脫離Bipap";
            }
        }

        public WorkItemResultType resultType
        {
            get
            {
                return WorkItemResultType._int;
            }
        }

        public void additionalFunc()
        {
            string actionNmae = "additionalFunc";
            string sql = this.getSQL();
            SQLProvider SQL = new SQLProvider();
            this.result = SQL.DBA.getSqlDataTable<WorkItem>(sql);
            if (SQL.DBA.hasLastError)
            {
                this.result = new List<WorkItem>();
                Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, actionNmae, this.csName);
            }
        }

        public string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// Wean.Parameter 
    /// </summary>
    public class WorkVolume__Weaning_Parameter : IWorkItemFunc
    {
        public string csName
        {
            get
            {
                return "WorkVolume__Weaning_Parameter";
            }
        }
        public List<WorkItem> result { get; set; }

        public string itemCode
        {
            get
            {
                return "Weaning_Parameter";
            }
        }

        public string itemName
        {
            get
            {
                return "Wean.Parameter";
            }
        }

        public WorkItemResultType resultType
        {
            get
            {
                return WorkItemResultType._int;
            }
        }

        public void additionalFunc()
        {
            string actionNmae = "additionalFunc";
            string sql = this.getSQL();
            SQLProvider SQL = new SQLProvider();
            this.result = SQL.DBA.getSqlDataTable<WorkItem>(sql);
            if (SQL.DBA.hasLastError)
            {
                this.result = new List<WorkItem>();
                Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, actionNmae, this.csName);
            }
        }

        public string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// O2
    /// </summary>
    public class WorkVolume_O2 : IWorkItemFunc
    {
        public string csName
        {
            get
            {
                return "WorkVolume_O2";
            }
        }
        public List<WorkItem> result { get; set; }

        public string itemCode
        {
            get
            {
                return "O2";
            }
        }

        public string itemName
        {
            get
            {
                return "O2";
            }
        }

        public WorkItemResultType resultType
        {
            get
            {
                return WorkItemResultType._int;
            }
        }

        public void additionalFunc()
        {
            string actionNmae = "additionalFunc";
            string sql = this.getSQL();
            SQLProvider SQL = new SQLProvider();
            this.result = SQL.DBA.getSqlDataTable<WorkItem>(sql);
            if (SQL.DBA.hasLastError)
            {
                this.result = new List<WorkItem>();
                Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, actionNmae, this.csName);
            }
        }

        public string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// 脫離O2
    /// </summary>
    public class WorkVolume_Weaning_O2 : IWorkItemFunc
    {
        public string csName
        {
            get
            {
                return "WorkVolume_Weaning_O2";
            }
        }
        public List<WorkItem> result { get; set; }

        public string itemCode
        {
            get
            {
                return "Weaning_O2";
            }
        }

        public string itemName
        {
            get
            {
                return "脫離O2";
            }
        }

        public WorkItemResultType resultType
        {
            get
            {
                return WorkItemResultType._int;
            }
        }

        public void additionalFunc()
        {
            string actionNmae = "additionalFunc";
            string sql = this.getSQL();
            SQLProvider SQL = new SQLProvider();
            this.result = SQL.DBA.getSqlDataTable<WorkItem>(sql);
            if (SQL.DBA.hasLastError)
            {
                this.result = new List<WorkItem>();
                Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, actionNmae, this.csName);
            }
        }

        public string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// O2分析
    /// </summary>
    public class WorkVolume_O2_Analysis : IWorkItemFunc
    {
        public string csName
        {
            get
            {
                return "WorkVolume_O2_Analysis";
            }
        }
        public List<WorkItem> result { get; set; }

        public string itemCode
        {
            get
            {
                return "O2_Analysis";
            }
        }

        public string itemName
        {
            get
            {
                return "O2分析";
            }
        }

        public WorkItemResultType resultType
        {
            get
            {
                return WorkItemResultType._int;
            }
        }

        public void additionalFunc()
        {
            string actionNmae = "additionalFunc";
            string sql = this.getSQL();
            SQLProvider SQL = new SQLProvider();
            this.result = SQL.DBA.getSqlDataTable<WorkItem>(sql);
            if (SQL.DBA.hasLastError)
            {
                this.result = new List<WorkItem>();
                Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, actionNmae, this.csName);
            }
        }

        public string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// O2分析
    /// </summary>
    public class WorkVolume_Inh_times : IWorkItemFunc
    {
        public string csName
        {
            get
            {
                return "WorkVolume_Inh_times";
            }
        }
        public List<WorkItem> result { get; set; }

        public string itemCode
        {
            get
            {
                return "Inh_times";
            }
        }

        public string itemName
        {
            get
            {
                return "Inh/次";
            }
        }

        public WorkItemResultType resultType
        {
            get
            {
                return WorkItemResultType._int;
            }
        }

        public void additionalFunc()
        {
            string actionNmae = "additionalFunc";
            string sql = this.getSQL();
            SQLProvider SQL = new SQLProvider();
            this.result = SQL.DBA.getSqlDataTable<WorkItem>(sql);
            if (SQL.DBA.hasLastError)
            {
                this.result = new List<WorkItem>();
                Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, actionNmae, this.csName);
            }
        }

        public string getSQL()
        {
            return string.Concat("", "");
        }
    }

    /// <summary>
    /// O2分析
    /// </summary>
    public class WorkVolume_Inh_day : IWorkItemFunc
    {
        public string csName
        {
            get
            {
                return "WorkVolume_Inh_day";
            }
        }
        public List<WorkItem> result { get; set; }

        public string itemCode
        {
            get
            {
                return "Inh_day";
            }
        }

        public string itemName
        {
            get
            {
                return "Inh/天";
            }
        }

        public WorkItemResultType resultType
        {
            get
            {
                return WorkItemResultType._int;
            }
        }

        public void additionalFunc()
        {
            string actionNmae = "additionalFunc";
            string sql = this.getSQL();
            SQLProvider SQL = new SQLProvider();
            this.result = SQL.DBA.getSqlDataTable<WorkItem>(sql);
            if (SQL.DBA.hasLastError)
            {
                this.result = new List<WorkItem>();
                Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, actionNmae, this.csName);
            }
        }

        public string getSQL()
        {
            return string.Concat("", "");
        }
    }
    #endregion


    public class VM_mainBreathReport
    {
        public string month { get; set; }
        public string year { get; set; }
        public List<SelectListItem> yearList { get; set; }
        public List<SelectListItem> monthList { get; set; }
    }
}