using Com.Mayaminer;
using RCS.Models.JAG;
using RCS_Data;
using RCS_Data.Controllers.Upload;
using RCS_Data.Models.JAG;
using RCS_Data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace RCS.Models.ViewModel
{
    public class RtAssess : RCS_Data.Controllers.RtAssess.Models , RCS_Data.Controllers.RtAssess.Interface
    { 
        /// <summary>
        /// 轉換JSON
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public override List<RCS_CPT_DTL_NEW_ITEMS> changeJson(List<RCS_CPT_DTL_NEW_ITEMS> List)
        {

            List<RCS_CPT_DTL_NEW_ITEMS> result = List;

            #region 轉換JSON

            foreach (var item in result)
            {
                #region brief_status
                if (item.brief_status != null && item.brief_status.Any())
                {
                    if (item.brief_status.Where(x => x.id == "tube_time").Any())
                    {
                        item.tube_time = item.brief_status.Where(x => x.id == "tube_time").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "tube_width").Any())
                    {
                        item.tube_width = item.brief_status.Where(x => x.id == "tube_width").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "tube_deep").Any())
                    {
                        item.tube_deep = item.brief_status.Where(x => x.id == "tube_deep").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "shiley_time").Any())
                    {
                        item.shiley_time = item.brief_status.Where(x => x.id == "shiley_time").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "shiley_width").Any())
                    {
                        item.shiley_width = item.brief_status.Where(x => x.id == "shiley_width").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "shiley_kind").Any())
                    {
                        item.shiley_kind = item.brief_status.Where(x => x.id == "shiley_kind").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "rt_start_time").Any())
                    {
                        item.rt_start_time = item.brief_status.Where(x => x.id == "rt_start_time").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "rt_start_respirator_model").Any())
                    {
                        item.rt_start_respirator_model = item.brief_status.Where(x => x.id == "rt_start_respirator_model").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "rt_start_if").Any())
                    {
                        item.rt_start_if = item.brief_status.Where(x => x.id == "rt_start_if").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "rt_start_model_1").Any())
                    {
                        item.rt_start_model = item.brief_status.Where(x => x.id == "rt_start_model_1").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "rt_start_model_2").Any())
                    {
                        item.rt_start_fio2 = item.brief_status.Where(x => x.id == "rt_start_model_2").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "rt_start_model_3").Any())
                    {
                        item.rt_start_PEEP = item.brief_status.Where(x => x.id == "rt_start_model_3").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "rt_start_model_4").Any())
                    {
                        item.rt_start_VT_PC = item.brief_status.Where(x => x.id == "rt_start_model_4").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "rt_start_model_5").Any())
                    {
                        item.rt_start_RR = item.brief_status.Where(x => x.id == "rt_start_model_5").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "rt_start_model_6").Any())
                    {
                        item.rt_start_IPAP = item.brief_status.Where(x => x.id == "rt_start_model_6").First().val;
                    }
                    if (item.brief_status.Where(x => x.id == "rt_start_model_7").Any())
                    {
                        item.rt_start_EPAP = item.brief_status.Where(x => x.id == "rt_start_model_7").First().val;
                    }
                }

                #endregion

                #region rt_reason

                if (item.rt_reason != null && item.rt_reason.Any())
                {
                    List<string> checkrt_reasonList = new List<string>();

                    checkrt_reasonList.AddRange(item.rt_reason.Where(y => y.id != "rt_reason_other" && y.id != "rt_reason_Post" && y.id != "Trauma" && y.chkd).Select(x => x.val));

                    item.rt_reason_data = String.Join(",", checkrt_reasonList);
                    if (item.rt_reason.Where(x => x.id == "rt_reason_other").Any())
                    {
                        item.rt_reason_other = item.rt_reason.Where(x => x.id == "rt_reason_other").First().val;
                    }
                    if (item.rt_reason.Where(x => x.id == "rt_reason_Post").Any())
                    {
                        item.rt_reason_postop = item.rt_reason.Where(x => x.id == "rt_reason_Post").First().val;
                    }
                    if (item.rt_reason.Where(x => x.id == "Trauma").Any())
                    {
                        item.rt_reason_trauma = item.rt_reason.Where(x => x.id == "Trauma").First().val;
                    }
                }

                #endregion

                #region from_unit_data

                if (item.from_unit != null && item.from_unit.Any())
                {

                    List<string> checkfrom_unit = new List<string>();

                    checkfrom_unit.AddRange(item.from_unit.Where(y => y.id != "from_unit_11"&& y.chkd).Select(x => x.val));

                    item.from_unit_data = String.Join(",", checkfrom_unit);

                    if (item.from_unit.Where(x => x.id == "from_unit_11").Any())
                    {
                        item.from_unit_other_txext = item.from_unit.Where(x => x.id == "from_unit_11").First().val;
                    }
                }

                #endregion

                #region cpt_history

                if (item.cpt_history != null && item.cpt_history.Any())
                {
                    List<string> checkcpt_history = new List<string>();

                    checkcpt_history.AddRange(item.cpt_history.Where(y => y.id != "cpt_ass_cpt_history_7" && y.chkd).Select(x => x.val));

                    item.cpt_history_data = String.Join(",", checkcpt_history);

                    if (item.cpt_history.Where(x => x.id == "cpt_ass_cpt_history_7").Any())
                    {
                        item.cpt_history_other_txext = item.cpt_history.Where(x => x.id == "cpt_ass_cpt_history_7").First().val;
                    }

                }

                #endregion

                #region smoke_history

                if (item.smoke_history != null && item.smoke_history.Any())
                {

                    if (item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_1").Any() && item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_1").First().chkd)
                    {
                        item.smoke_history_data = item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_1").First().val;
                    }
                    if (item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_2").Any() && item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_2").First().chkd)
                    {
                        item.smoke_history_data = item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_2").First().val;
                    }
                    if (item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_3").Any())
                    {
                        item.smoke_history_PPD = item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_3").First().val;
                    }
                    if (item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_4").Any())
                    {
                        item.smoke_history_year = item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_4").First().val;
                    }
                    if (item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_5").Any()&& item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_5").First().chkd)
                    {
                        item.smoke_history_end = "true";
                    }
                    if (item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_6").Any())
                    {
                        item.smoke_history_end_year = item.smoke_history.Where(x => x.id == "cpt_ass_smoke_history_6").First().val;
                    }
                }

                #endregion

                #region abg_data

                if (item.abg_data != null && item.abg_data.Any())
                {
                    if (item.abg_data.Where(x => x.id == "cpt_ass_abg_1").Any())
                    {
                        item.abg_date = item.abg_data.Where(x => x.id == "cpt_ass_abg_1").First().val;
                    }
                    if (item.abg_data.Where(x => x.id == "cpt_ass_abg_2").Any())
                    {
                        item.abg_data_pH = item.abg_data.Where(x => x.id == "cpt_ass_abg_2").First().val;
                    }
                    if (item.abg_data.Where(x => x.id == "cpt_ass_abg_10").Any())
                    {
                        item.abg_data_PaCO2 = item.abg_data.Where(x => x.id == "cpt_ass_abg_10").First().val;
                    }
                    if (item.abg_data.Where(x => x.id == "cpt_ass_abg_3").Any())
                    {
                        item.abg_data_PaO2 = item.abg_data.Where(x => x.id == "cpt_ass_abg_3").First().val;
                    }
                    if (item.abg_data.Where(x => x.id == "cpt_ass_abg_7").Any())
                    {
                        item.abg_data_HCO3 = item.abg_data.Where(x => x.id == "cpt_ass_abg_7").First().val;
                    }
                    if (item.abg_data.Where(x => x.id == "cpt_ass_abg_9").Any())
                    {
                        item.abg_data_BE = item.abg_data.Where(x => x.id == "cpt_ass_abg_9").First().val;
                    }
                    if (item.abg_data.Where(x => x.id == "cpt_ass_abg_8").Any())
                    {
                        item.abg_data_SaO2 = item.abg_data.Where(x => x.id == "cpt_ass_abg_8").First().val;
                    }
                    if (item.abg_data.Where(x => x.id == "cpt_ass_abg_12").Any())
                    {
                        item.abg_data_FiO2 = item.abg_data.Where(x => x.id == "cpt_ass_abg_12").First().val;
                    }
                    if (item.abg_data.Where(x => x.id == "cpt_ass_abg_13").Any())
                    {
                        item.abg_data_Device = item.abg_data.Where(x => x.id == "cpt_ass_abg_13").First().val;
                    }

                }

                #endregion

                #region conscious

                if (item.conscious != null && item.conscious.Any())
                {
                    if (item.conscious.Where(x => x.id == "cpt_ass_conscious_4").Any())
                    {
                        item.conscious_E = item.conscious.Where(x => x.id == "cpt_ass_conscious_4").First().val;
                    }
                    if (item.conscious.Where(x => x.id == "cpt_ass_conscious_5").Any())
                    {
                        item.conscious_V = item.conscious.Where(x => x.id == "cpt_ass_conscious_5").First().val;
                    }
                    if (item.conscious.Where(x => x.id == "cpt_ass_conscious_6").Any())
                    {
                        item.conscious_M = item.conscious.Where(x => x.id == "cpt_ass_conscious_6").First().val;
                    }
                    if (item.conscious.Where(x => x.id == "cpt_ass_conscious_1").Any())
                    {
                        item.conscious_data = item.conscious.Where(x => x.id == "cpt_ass_conscious_1").First().val;
                    }
                    if (item.conscious.Where(x => x.id == "body_height").Any())
                    {
                        item.body_height = item.conscious.Where(x => x.id == "body_height").First().val;
                    }
                    if (item.conscious.Where(x => x.id == "body_weight").Any())
                    {
                        item.body_weight = item.conscious.Where(x => x.id == "body_weight").First().val;
                    }
                    if (item.conscious.Where(x => x.id == "f_body_weight").Any())
                    {
                        item.body_IBW = item.conscious.Where(x => x.id == "f_body_weight").First().val;
                    }

                }

                #endregion

                #region patterns

                if (item.patterns != null && item.patterns.Any())
                {
                    List<string> checkpatterns = new List<string>();

                    checkpatterns.AddRange(item.patterns.Where(y => y.id != "cpt_ass_patterns_10"&& y.chkd).Select(x => x.val));

                    item.patterns_data = String.Join(",", checkpatterns);

                    if (item.patterns.Where(x => x.id == "cpt_ass_patterns_10").Any())
                    {
                        item.patterns_other = item.patterns.Where(x => x.id == "cpt_ass_patterns_10").First().val;
                    }
                }

                #endregion

                #region breath_sound

                if (item.breath_sound != null && item.breath_sound.Any())
                {

                    List<string> checkCoarse = new List<string>();

                    for (int j = 1; j < 7; j++)
                    {

                        string checksoundID = string.Concat("cpt_ass_sound_", j + 1);

                        if (item.breath_sound.Where(x => x.id == checksoundID).Any() && item.breath_sound.Where(x => x.id == checksoundID).First().chkd)
                        {
                            if (j == 1)
                            {
                                checkCoarse.Add("Bilateral");
                            }
                            else if (j == 2)
                            {
                                checkCoarse.Add("RUL");
                            }
                            else if (j == 3)
                            {
                                checkCoarse.Add("RML");
                            }
                            else if (j == 4)
                            {
                                checkCoarse.Add("RLL");
                            }
                            else if (j == 5)
                            {
                                checkCoarse.Add("LUL");
                            }
                            else if (j == 6)
                            {
                                checkCoarse.Add("LLL");
                            }
                        }
                    }

                    item.breath_sound_Coarse = String.Join(",", checkCoarse);

                    List<string> checkCrackle = new List<string>();

                    for (int j = 1; j < 7; j++)
                    {

                        string checksoundID = string.Concat("cpt_ass_sound_", j + 8);

                        if (item.breath_sound.Where(x => x.id == checksoundID).Any() && item.breath_sound.Where(x => x.id == checksoundID).First().chkd)
                        {
                            if (j == 1)
                            {
                                checkCrackle.Add("Bilateral");
                            }
                            else if (j == 2)
                            {
                                checkCrackle.Add("RUL");
                            }
                            else if (j == 3)
                            {
                                checkCrackle.Add("RML");
                            }
                            else if (j == 4)
                            {
                                checkCrackle.Add("RLL");
                            }
                            else if (j == 5)
                            {
                                checkCrackle.Add("LUL");
                            }
                            else if (j == 6)
                            {
                                checkCrackle.Add("LLL");
                            }
                        }
                    }

                    item.breath_sound_Crackle = String.Join(",", checkCrackle);

                    List<string> Wheezing = new List<string>();

                    for (int j = 1; j < 7; j++)
                    {

                        string checksoundID = string.Concat("cpt_ass_sound_", j + 15);

                        if (item.breath_sound.Where(x => x.id == checksoundID).Any() && item.breath_sound.Where(x => x.id == checksoundID).First().chkd)
                        {
                            if (j == 1)
                            {
                                Wheezing.Add("Bilateral");
                            }
                            else if (j == 2)
                            {
                                Wheezing.Add("RUL");
                            }
                            else if (j == 3)
                            {
                                Wheezing.Add("RML");
                            }
                            else if (j == 4)
                            {
                                Wheezing.Add("RLL");
                            }
                            else if (j == 5)
                            {
                                Wheezing.Add("LUL");
                            }
                            else if (j == 6)
                            {
                                Wheezing.Add("LLL");
                            }
                        }
                    }

                    item.breath_sound_Wheezing = String.Join(",", Wheezing);

                    List<string> Rhonchi = new List<string>();

                    for (int j = 1; j < 7; j++)
                    {

                        string checksoundID = string.Concat("cpt_ass_sound_", j + 22);

                        if (item.breath_sound.Where(x => x.id == checksoundID).Any() && item.breath_sound.Where(x => x.id == checksoundID).First().chkd)
                        {
                            if (j == 1)
                            {
                                Rhonchi.Add("Bilateral");
                            }
                            else if (j == 2)
                            {
                                Rhonchi.Add("RUL");
                            }
                            else if (j == 3)
                            {
                                Rhonchi.Add("RML");
                            }
                            else if (j == 4)
                            {
                                Rhonchi.Add("RLL");
                            }
                            else if (j == 5)
                            {
                                Rhonchi.Add("LUL");
                            }
                            else if (j == 6)
                            {
                                Rhonchi.Add("LLL");
                            }
                        }
                    }

                    item.breath_sound_Rhonchi = String.Join(",", Rhonchi);

                    List<string> Decreased = new List<string>();

                    for (int j = 1; j < 7; j++)
                    {

                        string checksoundID = string.Concat("cpt_ass_sound_", j + 33);

                        if (item.breath_sound.Where(x => x.id == checksoundID).Any() && item.breath_sound.Where(x => x.id == checksoundID).First().chkd)
                        {
                            if (j == 1)
                            {
                                Decreased.Add("Bilateral");
                            }
                            else if (j == 2)
                            {
                                Decreased.Add("RUL");
                            }
                            else if (j == 3)
                            {
                                Decreased.Add("RML");
                            }
                            else if (j == 4)
                            {
                                Decreased.Add("RLL");
                            }
                            else if (j == 5)
                            {
                                Decreased.Add("LUL");
                            }
                            else if (j == 6)
                            {
                                Decreased.Add("LLL");
                            }
                        }
                    }

                    item.breath_sound_Decreased = String.Join(",", Decreased);

                    List<string> Other = new List<string>();

                    for (int j = 1; j < 7; j++)
                    {

                        string checksoundID = string.Concat("cpt_ass_sound_", j + 40);

                        if (item.breath_sound.Where(x => x.id == checksoundID).Any() && item.breath_sound.Where(x => x.id == checksoundID).First().chkd)
                        {
                            if (j == 1)
                            {
                                Other.Add("Bilateral");
                            }
                            else if (j == 2)
                            {
                                Other.Add("RUL");
                            }
                            else if (j == 3)
                            {
                                Other.Add("RML");
                            }
                            else if (j == 4)
                            {
                                Other.Add("RLL");
                            }
                            else if (j == 5)
                            {
                                Other.Add("LUL");
                            }
                            else if (j == 6)
                            {
                                Other.Add("LLL");
                            }
                        }
                    }

                    item.breath_sound_other = String.Join(",", Other);


                    if (item.breath_sound.Where(x => x.id == "cpt_ass_sound_47").Any())
                    {
                        item.breath_sound_other_text = item.breath_sound.Where(x => x.id == "cpt_ass_sound_47").First().val;
                    }

                }

                #endregion

                #region cough
                if (item.cough != null && item.cough.Any())
                {

                    if (item.cough.Where(x => x.id == "cpt_ass_cough_1").Any() && item.cough.Where(x => x.id == "cpt_ass_cough_1").First().chkd)
                    {
                        item.cough_data = item.cough.Where(x => x.id == "cpt_ass_cough_1").First().val;
                    }

                    if (item.cough.Where(x => x.id == "cpt_ass_cough_2").Any() && item.cough.Where(x => x.id == "cpt_ass_cough_2").First().chkd)
                    {
                        item.cough_data = item.cough.Where(x => x.id == "cpt_ass_cough_2").First().val;
                    }

                    if (item.cough.Where(x => x.id == "cpt_ass_cough_3").Any() && item.cough.Where(x => x.id == "cpt_ass_cough_3").First().chkd)
                    {
                        item.cough_data = item.cough.Where(x => x.id == "cpt_ass_cough_3").First().val;
                    }

                }


                #endregion

                #region sputum_assess

                if (item.sputum_assess != null && item.sputum_assess.Any())
                {


                    if (item.sputum_assess.Where(x => x.id == "cpt_ass_sputum_ass_0").Any())
                    {
                        item.sputum_assess_way = item.sputum_assess.Where(x => x.id == "cpt_ass_sputum_ass_0").First().val;
                    }
                    if (item.sputum_assess.Where(x => x.id == "cpt_ass_sputum_ass_1").Any())
                    {
                        item.sputum_assess_amount = item.sputum_assess.Where(x => x.id == "cpt_ass_sputum_ass_1").First().val;
                    }
                    if (item.sputum_assess.Where(x => x.id == "cpt_ass_sputum_ass_2").Any())
                    {
                        item.sputum_assess_quality = item.sputum_assess.Where(x => x.id == "cpt_ass_sputum_ass_2").First().val;
                    }
                    if (item.sputum_assess.Where(x => x.id == "cpt_ass_sputum_ass_3").Any())
                    {
                        item.sputum_assess_color = item.sputum_assess.Where(x => x.id == "cpt_ass_sputum_ass_3").First().val;
                    }

                }

                #endregion

                // item.remark = item.other_history;

            }

            #endregion

            return result;
        }
 
    }

 
}