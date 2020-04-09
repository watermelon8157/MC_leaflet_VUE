using RCS_Data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models.ViewModel
{
    public class RtCptAssess : RCS_Data.Controllers.RtCptAss.Models, RCS_Data.Controllers.RtCptAss.Interface
    {

        private string setList(string str, List<string> data)
        {
            string result = str;

            if (data != null && data.Any())
            {
                result = string.Join(",", data);
            }

            return result;
        }


        public CPTNewRecord CPTNewRecord(CPTNewRecord data)
        {
            data.ReconditioningExercise_str = setList(data.ReconditioningExercise_str, data.ReconditioningExercise);
            data.treatment_check_str = setList(data.treatment_check_str, data.treatment_check);
            data.Guardian_str = setList(data.Guardian_str, data.Guardian);
            data.Guardian_Aims_str = setList(data.Guardian_Aims_str, data.Guardian_Aims);
            data.Sport_Training_str = setList(data.Sport_Training_str, data.Sport_Training);
            data.Sport_Training_Aims_str = setList(data.Sport_Training_Aims_str, data.Sport_Training_Aims);
            data.Respiratory_Muscle_str = setList(data.Respiratory_Muscle_str, data.Respiratory_Muscle);
            data.Respiratory_Muscle_Aims_str = setList(data.Respiratory_Muscle_Aims_str, data.Respiratory_Muscle_Aims);
            data.Lung_Expansion_str = setList(data.Lung_Expansion_str, data.Lung_Expansion);
            data.Lung_Expansion_Aims_str = setList(data.Lung_Expansion_Aims_str, data.Lung_Expansion_Aims);
            data.cough_aims_str = setList(data.cough_aims_str, data.cough_aims);
            data.coucough_device_aimsgh_str = setList(data.coucough_device_aimsgh_str, data.coucough_device_aimsgh);
            data.coucough_aimsgh_str = setList(data.coucough_aimsgh_str, data.coucough_aimsgh);
            data.Breathing_Training_str = setList(data.Breathing_Training_str, data.Breathing_Training);
            data.Breathing_Training_Aims_str = setList(data.Breathing_Training_Aims_str, data.Breathing_Training_Aims);
            data.Drug_Inhalation_str = setList(data.Drug_Inhalation_str, data.Drug_Inhalation);
            data.Drug_Inhalation_Aims_str = setList(data.Drug_Inhalation_Aims_str, data.Drug_Inhalation_Aims);
            data.cpt_reason_str = setList(data.cpt_reason_str, data.cpt_reason);
            data.Activity_ability_after_str = setList(data.Activity_ability_after_str, data.Activity_ability_after);
            data.Activity_ability_str = setList(data.Activity_ability_str, data.Activity_ability);
            data.ass_ev_str = setList(data.ass_ev_str, data.ass_ev);
            data.breathe_type_str = setList(data.breathe_type_str, data.breathe_type);


            if (data.breathe_sound != null && data.breathe_sound.Any())
            {

                List<string> breath_sound = new List<string>();
                breath_sound.AddRange(data.breathe_sound.Where(y => y == "Normal" || y == "Stridor"));

                data.breathe_sound_str = String.Join(",", breath_sound);

                List<string> Fine_Crackles = new List<string>();

                for (int j = 1; j < 3; j++)
                {
                    string checksoundID = string.Concat("Fine Crackles_");
                    string addData = "";
                    if (j == 1)
                    {
                        addData = "L";
                        checksoundID += "L";
                    }
                    else
                    {
                        addData = "R";
                        checksoundID += "R";
                    }

                    if (data.breathe_sound.Where(x => x == checksoundID).Any())
                    {
                        Fine_Crackles.Add(addData);
                    }
                }

                data.breath_sound_Fine_Crackles = String.Join(",", Fine_Crackles);

                List<string> breath_sound_Crackle = new List<string>();

                for (int j = 1; j < 3; j++)
                {
                    string checksoundID = string.Concat("Coarse Crackle_");
                    string addData = "";
                    if (j == 1)
                    {
                        addData = "L";
                        checksoundID += "L";
                    }
                    else
                    {
                        addData = "R";
                        checksoundID += "R";
                    }

                    if (data.breathe_sound.Where(x => x == checksoundID).Any())
                    {
                        breath_sound_Crackle.Add(addData);
                    }
                }

                data.breath_sound_Crackle = String.Join(",", breath_sound_Crackle);



                List<string> breath_sound_Wheezing = new List<string>();

                for (int j = 1; j < 3; j++)
                {
                    string checksoundID = string.Concat("Wheezing_");
                    string addData = "";
                    if (j == 1)
                    {
                        addData = "L";
                        checksoundID += "L";
                    }
                    else
                    {
                        addData = "R";
                        checksoundID += "R";
                    }
                    if (data.breathe_sound.Where(x => x == checksoundID).Any())
                    {
                        breath_sound_Wheezing.Add(addData);
                    }
                }

                data.breath_sound_Wheezing = String.Join(",", breath_sound_Wheezing);

                List<string> breath_sound_Decrease = new List<string>();

                for (int j = 1; j < 3; j++)
                {
                    string checksoundID = string.Concat("Decrease_");
                    string addData = "";
                    if (j == 1)
                    {
                        addData = "L";
                        checksoundID += "L";
                    }
                    else
                    {
                        addData = "R";
                        checksoundID += "R";
                    }

                    if (data.breathe_sound.Where(x => x == checksoundID).Any())
                    {
                        breath_sound_Decrease.Add(addData);
                    }
                }

                data.breath_sound_Decrease = String.Join(",", breath_sound_Decrease);

                List<string> breath_sound_Absent = new List<string>();

                for (int j = 1; j < 3; j++)
                {
                    string checksoundID = string.Concat("Absent_");
                    string addData = "";
                    if (j == 1)
                    {
                        addData = "L";
                        checksoundID += "L";
                    }
                    else
                    {
                        addData = "R";
                        checksoundID += "R";
                    }

                    if (data.breathe_sound.Where(x => x == checksoundID).Any())
                    {
                        breath_sound_Absent.Add(addData);
                    }
                }

                data.breath_sound_Absent = String.Join(",", breath_sound_Absent);

                List<string> breath_sound_other_str = new List<string>();

                for (int j = 1; j < 3; j++)
                {
                    string checksoundID = string.Concat("其他_");
                    string addData = "";
                    if (j == 1)
                    {
                        addData = "L";
                        checksoundID += "L";
                    }
                    else
                    {
                        addData = "R";
                        checksoundID += "R";
                    }

                    if (data.breathe_sound.Where(x => x == checksoundID).Any())
                    {
                        breath_sound_other_str.Add(addData);
                    }
                }

                data.breath_sound_other_str = String.Join(",", breath_sound_other_str);
            }

            #region smoke_history


            if (data.cpt_ass_smoke_history_1)
            {
                data.smoke_history_data = "無";
            }
            if (data.cpt_ass_smoke_history_2)
            {
                data.smoke_history_data = "有";
            }
            data.smoke_history_PPD = data.cpt_ass_smoke_history_3;

            data.smoke_history_year = data.cpt_ass_smoke_history_4;
            if (data.cpt_ass_smoke_history_5)
            {
                data.smoke_history_end = "true";
            }
            data.smoke_history_end_year = data.cpt_ass_smoke_history_6;


            #endregion



            data.tube_pat_str = setList(data.tube_pat_str, data.tube_pat);
            data.cough_str = setList(data.cough_str, data.cough);
            data.sputum_str = setList(data.sputum_str, data.sputum);
            data.rt_reason_str = setList(data.rt_reason_str, data.rt_reason);
            data.cpt_tip_limbs_str = setList(data.cpt_tip_limbs_str, data.cpt_tip_limbs);
            data.cpt_tip_temperature_str = setList(data.cpt_tip_temperature_str, data.cpt_tip_temperature);
            data.cpt_tip_color_str = setList(data.cpt_tip_color_str, data.cpt_tip_color);
            data.sputum_draw_str = setList(data.sputum_draw_str, data.sputum_draw);
            data.ass_patterns_Breathe_str = setList(data.ass_patterns_Breathe_str, data.ass_patterns_Breathe);
            data.ass_patterns_speed_str = setList(data.ass_patterns_speed_str, data.ass_patterns_speed);
            data.ass_patterns_str = setList(data.ass_patterns_str, data.ass_patterns);
            data.sputum_count_str = setList(data.sputum_count_str, data.sputum_count);
            data.sputum_status_str = setList(data.sputum_status_str, data.sputum_status);
            data.sputum_small_list_str = setList(data.sputum_small_list_str, data.sputum_small_list);
            data.sputum_color_str = setList(data.sputum_color_str, data.sputum_color);
            data.treatment_str = setList(data.treatment_str, data.treatment);
            data.treatment_tube_pat_str = setList(data.treatment_tube_pat_str, data.treatment_tube_pat);
            data.treatment_PD_side_str = setList(data.treatment_PD_side_str, data.treatment_PD_side);
            data.treatment_PD_lobe_str = setList(data.treatment_PD_lobe_str, data.treatment_PD_lobe);
            data.conscious_after_str = setList(data.conscious_after_str, data.conscious_after);
            data.patterns_str = setList(data.patterns_str, data.patterns);
            data.breathe_sound_after_str = setList(data.breathe_sound_after_str, data.breathe_sound_after);
            data.LUNG_EXTEND_REGULAR_str = setList(data.LUNG_EXTEND_REGULAR_str, data.LUNG_EXTEND_REGULAR);
            data.sputum_after_str = setList(data.sputum_after_str, data.sputum_after);
            data.sputum_count_after_str = setList(data.sputum_count_after_str, data.sputum_count_after);
            data.sputum_status_after_str = setList(data.sputum_status_after_str, data.sputum_status_after);
            data.sputum_small_after_list_str = setList(data.sputum_small_after_list_str, data.sputum_small_after_list);
            data.sputum_color_after_str = setList(data.sputum_color_after_str, data.sputum_color_after);
            data.pat_compatibility_str = setList(data.pat_compatibility_str, data.pat_compatibility);
            data.pat_effect_str = setList(data.pat_effect_str, data.pat_effect);

            return data;

        }


        /// <summary>
        /// 轉換JSON
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public override List<CPTNewRecord> changeData(List<CPTNewRecord> List)
        {

            List<CPTNewRecord> result = new List<CPTNewRecord>();

            foreach (var item in List)
            {
                result.Add(CPTNewRecord(item));
            }


            return result;
        }
    }
}