using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    /// <summary>
    /// 病患清單
    /// </summary>
    public class PatientListItem : IPDPatientInfo
    {

        /// <summary> 是否轉或是切帳 </summary>
        public bool is_traned
        {
            get
            {
                if ((!string.IsNullOrWhiteSpace(this.new_bed_no) && this.bed_no != this.new_bed_no)
                    || (!string.IsNullOrWhiteSpace(this.new_cost_code) && this.cost_code != this.new_cost_code)
                    || (!string.IsNullOrWhiteSpace(this.new_vs_doc) && this.vs_doc != this.new_vs_doc)
                    || (!string.IsNullOrWhiteSpace(this.new_vs_id) && this.vs_id != this.new_vs_id)
                    || (!string.IsNullOrWhiteSpace(this.new_dept_code) && this.dept_code != this.new_dept_code)
                    || (!string.IsNullOrWhiteSpace(this.new_ipd_no) && this.ipd_no != this.new_ipd_no)
                    )
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary> 新的床號 </summary>
        public string new_bed_no { set; get; }

        public string new_cost_desc { set; get; }
        public string new_cost_code { set; get; }

        public string new_vs_doc { set; get; }

        public string new_vs_id { set; get; }

        public string new_dept_code { set; get; }

        public string new_dept_desc { set; get; }

        /// <summary> 治療項目 </summary>
        public string TREATMENT { get; set; }
        /// <summary> 呼吸設備 </summary>
        public string respid { get; set; }
        /// <summary> device </summary>
        public string device { get; set; }
        /// <summary> Ventilation Mode </summary>
        public string mode { get; set; }
        /// <summary>O2  device</summary>
        public string o2_device { get; set; }
        public Ventilator machine = new Ventilator();

        /// <summary>
        /// ON呼吸機時間 List
        /// </summary>
        public List<string> _on_mode { get; set; }

        /// <summary>
        /// ON 呼吸機時間String
        /// </summary>
        public string on_mode { get; set; }

        public void setOnMode(string pVal = "&#10;")
        {

            if (_on_mode == null)
                this.on_mode = "";
            else
            {
                _on_mode = _on_mode.OrderByDescending(x => DateTime.Parse(x)).ToList();
                this.on_mode = _on_mode.Count > 0 ? string.Join(pVal, _on_mode) : "";
            }
        }

        //2018/12/02加入
        public string _is_humidifier { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string memo { get; set; }

        /// <summary>
        /// 氣道介面 E.T. Tube
        /// </summary>
        public string show_ETTube { get; set; }
        /// <summary>
        /// 氧療 O2 equipment
        /// </summary>
        public string show_DeviceO2 { get; set; }
        /// <summary>
        /// 機型 呼吸器編號 
        /// </summary>
        public string show_respid { get; set; }
        /// <summary>
        /// 治療 
        /// </summary>
        public string show_treatment { get; set; }
    }
}
