using Com.Mayaminer;
using RCS_Data;
using RCS_Data.Controllers.RtRecord;
using RCS_Data.Controllers.Upload;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RCS.Models.ViewModel
{
    public class RtRecord: RCS_Data.Controllers.RtRecord.Models, RCS_Data.Controllers.RtRecord.Interface
    {
        public override RESPONSE_MSG GetPhraseData(string pType)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<DB_RCS_SYS_PARAMS> RTRecordAllList = new List<DB_RCS_SYS_PARAMS>();
            if (pType != null)
            {
                if (pType.EndsWith("memo") && !pType.EndsWith("drug_memo") )
                { 
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "HR:   ", P_NAME = "HR:   ", P_GROUP = "R_Memo", P_SORT = "1" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "RR:   ", P_NAME = "RR:   ", P_GROUP = "R_Memo", P_SORT = "2" });
                    // RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Pattern smooth", P_NAME = "Pattern smooth", P_GROUP = "R_Memo", P_SORT = "3" });  
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "SOB", P_NAME = "SOB", P_GROUP = "R_Memo", P_SORT = "6" });
                    // RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Apnea frequently", P_NAME = "Apnea frequently", P_GROUP = "R_Memo", P_SORT = "7" });
                    // RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Patient con's clear can obey.", P_NAME = "Patient con's clear can obey.", P_GROUP = "R_Memo", P_SORT = "8" });
                    // RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Patient unclear cannot obey.", P_NAME = "Patient unclear cannot obey.", P_GROUP = "R_Memo", P_SORT = "9" });
                    // RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Patient agitation cannot obey.", P_NAME = "Patient agitation cannot obey.", P_GROUP = "R_Memo", P_SORT = "10" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Paradoxical movement.", P_NAME = "Paradoxical movement.", P_GROUP = "R_Memo", P_SORT = "11" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Increased WOB.", P_NAME = "Increased WOB.", P_GROUP = "R_Memo", P_SORT = "12" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "SpO2↓   .", P_NAME = "SpO2↓   .", P_GROUP = "R_Memo", P_SORT = "13" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "HR↑   .", P_NAME = "HR↑   .", P_GROUP = "R_Memo", P_SORT = "14" });
                    // RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Vital sign stable.", P_NAME = "Vital sign stable.", P_GROUP = "R_Memo", P_SORT = "15" });
                    // RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Cuff leak test without leakage sound.", P_NAME = "Cuff leak test without leakage sound.", P_GROUP = "R_Memo", P_SORT = "16" });
                    // RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Leak sound only when auscultation.", P_NAME = "Leak sound only when auscultation.", P_GROUP = "R_Memo", P_SORT = "17" });
                    // TODO:上線後需進資料庫修改 

                    // 2020/01/07 新增
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Frequently show apnea.", P_NAME = "Frequently show apnea.", P_GROUP = "R_Memo", P_SORT = "18" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Cuff leak test failed without leakage sounds.", P_NAME = "Cuff leak test failed without leakage sounds.", P_GROUP = "R_Memo", P_SORT = "19" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Leak sound(+) only when auscultation.", P_NAME = "Leak sound(+) only when auscultation.", P_GROUP = "R_Memo", P_SORT = "20" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Patient was too agitative to obey orders.", P_NAME = "Patient was too agitative to obey orders.", P_GROUP = "R_Memo", P_SORT = "21" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Patient was con’s clear and could obey orders.", P_NAME = "Patient was con’s clear and could obey orders.", P_GROUP = "R_Memo", P_SORT = "22" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Patient was too unclear to obey orders.", P_NAME = "Patient was too unclear to obey orders.", P_GROUP = "R_Memo", P_SORT = "23" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Breathing pattern was smooth.", P_NAME = "Breathing pattern was smooth.", P_GROUP = "R_Memo", P_SORT = "24" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Vital sign was stable.", P_NAME = "Vital sign was stable.", P_GROUP = "R_Memo", P_SORT = "25" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Patient was intubated with Tr. , so doesn’t need to do cuff leak test.", P_NAME = "Patient was intubated with Tr. , so doesn’t need to do cuff leak test.", P_GROUP = "R_Memo", P_SORT = "26" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "     extubation", P_NAME = "     extubation", P_GROUP = "R_Memo", P_SORT = "26" });

                    // 2020/01/15 新增 
                    // 更換加熱潮濕管路
                    // 更換RT380管路
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "更換加熱潮濕管路", P_NAME = "更換加熱潮濕管路", P_GROUP = "R_Memo", P_SORT = "0" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "更換RT380管路", P_NAME = "更換RT380管路", P_GROUP = "R_Memo", P_SORT = "0" });


                }
                if (pType.EndsWith("drug_memo"))
                { 
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Fentanyl :  ml/hr", P_NAME = "Fentanyl :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "0" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Dormicum :  ml/hr", P_NAME = "Dormicum :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "1" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Precedex :  ml/hr", P_NAME = "Precedex :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "2" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Nimbex :  ml/hr", P_NAME = "Nimbex :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "3" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Levophed :  ml/hr", P_NAME = "Levophed :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "4" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Pitressin :  ml/hr", P_NAME = "Pitressin :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "5" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Dopamine :  ml/hr", P_NAME = "Dopamine :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "6" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Dobutamine :  ml/hr", P_NAME = "Dobutamine :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "7" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Bosmin :  ml/hr", P_NAME = "Bosmin :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "8" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Perdipine :  ml/hr", P_NAME = "Perdipine :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "9" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Cordarone :  ml/hr", P_NAME = "Cordarone :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "10" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "N.T.G :  ml/hr", P_NAME = "N.T.G :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "11" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Heparin :  ml/hr", P_NAME = "Heparin :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "12" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Somatosan :  ml/hr", P_NAME = "Somatosan :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "13" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Jusomin", P_NAME = "Jusomin", P_GROUP = "R_Drug_Memo", P_SORT = "14" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Lasix :  ml/hr", P_NAME = "Lasix :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "15" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Propofol :  ml/hr", P_NAME = "Propofol :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "16" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Xylocaine :  ml/hr", P_NAME = "Xylocaine :  ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "17" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "ICP:   mmHg", P_NAME = "ICP:   mmHg", P_GROUP = "R_Drug_Memo", P_SORT = "18" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "EVD:   mmHg", P_NAME = "EVD:   mmHg", P_GROUP = "R_Drug_Memo", P_SORT = "19" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "TcPCO2:   mmHg/ RHP:   mW", P_NAME = "TcPCO2:   mmHg/ RHP:   mW", P_GROUP = "R_Drug_Memo", P_SORT = "20" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "呼氣末二氧化碳分壓:   mmHg", P_NAME = "呼氣末二氧化碳分壓:   mmHg", P_GROUP = "R_Drug_Memo", P_SORT = "21" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "H/D/CVVH(體液減輕量):   kg/   ml/hr", P_NAME = "H/D/CVVH(體液減輕量):   kg/   ml/hr", P_GROUP = "R_Drug_Memo", P_SORT = "22" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "IABP:   ", P_NAME = "IABP:   ", P_GROUP = "R_Drug_Memo", P_SORT = "23" }); 
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Delta P：   cmH2O，PEEP：   cmH2O，VT：   ml，C：   ，Spo2 ：   %，HR：   bpm，SBP：   mmHg，DBP：   mmHg", P_NAME = "Delta P：   cmH2O，PEEP：   cmH2O，VT：   ml，C：   ，Spo2 ：   %，HR：   bpm，SBP：   mmHg，DBP：   mmHg", P_GROUP = "R_Drug_Memo", P_SORT = "24" }); 
                    // TODO:上線後需進資料庫修改 
                }
                if (pType.EndsWith("Remark"))
                {
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Duration：     mins", P_NAME = "Duration：     mins", P_SORT = "0" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Head Up：     度", P_NAME = "Head Up：     度", P_SORT = "1" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Duration：     Cycle,      Sec/Cycle", P_NAME = "Duration：     Cycle,      Sec/Cycle", P_SORT = "2" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "衛教          拍痰技巧", P_NAME = "衛教          拍痰技巧", P_SORT = "3" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "衛教          拍痰器使用", P_NAME = "衛教          拍痰器使⽤", P_SORT = "4" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Training Course：       ", P_NAME = "Training Course：       ", P_SORT = "5" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "Training Frequency：       ", P_NAME = "Training Frequency：       ", P_SORT = "6" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "VT (Before):     /VT(After):     ", P_NAME = "VT (Before):     /VT(After):     ", P_SORT = "6" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "For PLT：     (x10^3/ul), hold therapy once", P_NAME = "For PLT：    (x10^3/ul), hold therapy once", P_SORT = "6" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "For vital sign was unstable, hold therapy once", P_NAME = "For vital sign was unstable, hold therapy once", P_SORT = "6" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "未帶Triflow", P_NAME = "未帶Triflow", P_SORT = "6" });
                    RTRecordAllList.Add(new DB_RCS_SYS_PARAMS() { P_VALUE = "教導深呼吸運動", P_NAME = "教導深呼吸運動", P_SORT = "6" });
                    // TODO:上線後需進資料庫修改 
                }
            } 
            RTRecordAllList = RTRecordAllList.OrderBy(x => x.P_VALUE).ToList();
            rm.attachment = RTRecordAllList;
            return rm;
        }

        public override RT_RECORD_DATA<RT_RECORD_MUST_DATA> changeRTRecordData(RT_RECORD_DATA<RT_RECORD_MUST_DATA> List)
        {
            #region 帶入上一筆，以下變數都是萬芳醫院不想帶入的資料

            RT_RECORD_MUST_DATA pData = new RT_RECORD_MUST_DATA();

            //Total / insp.TV
            List.model.exp_tv = pData.exp_tv;
            List.model.vt = pData.vt;
            //Ventilation Rate Total
            List.model.vr = pData.vr;
            //MV Total
            List.model.mv = pData.mv;
            //I / E Ratio
            List.model.ie_ratio = pData.ie_ratio;
            //Peak Pr.
            List.model.pressure_peak = pData.pressure_peak;
            //Mean
            List.model.pressure_mean = pData.pressure_mean;
            //Raw / C
            List.model.resistance = pData.resistance;
            List.model.Compliance = pData.Compliance;
            //評估時間
            List.model.evaluation_time = pData.evaluation_time;
            //VT(ml) / RR(bpm)
            List.model.vt_value = pData.vt_value;
            List.model.rsi_srr = pData.rsi_srr;
            //RSBI
            List.model.rsbi = pData.rsbi;
            //Pi / E max(cmH2O)
            List.model.pi_max = pData.pi_max;
            List.model.pe_max = pData.pe_max;
            //Cuff - leak test(ml)
            List.model.cuff_leak_ml = pData.cuff_leak_ml;
            //SvO2(%)
            List.model.ecmo_svo2 = pData.ecmo_svo2;
            //Cuff pressure(cmH2O) ml
            List.model.cuff = pData.cuff;
            //Breath sound
            List.model.breath_sound = pData.breath_sound;
            List.model.breath_sound_location = pData.breath_sound_location;
            //PR / Temp
            List.model.pulse = pData.pulse;
            List.model.vital_signs_temp = pData.vital_signs_temp;
            //BP(S / D)(mmHg)
            List.model.bps = pData.bps;
            List.model.bpd = pData.bpd;
            //Con's level(E.V.M.)
            List.model.gcse = pData.gcse;
            List.model.gcsv = pData.gcsv;
            List.model.gcsm = pData.gcsm;
            //意識
            List.model.conscious = pData.conscious;            
            //備註
            List.model.memo = pData.memo;
            #endregion

            return List;
        }

    }
}