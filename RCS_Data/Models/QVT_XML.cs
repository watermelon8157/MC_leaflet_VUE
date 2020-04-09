using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RCS_Data
{
    /// <summary>
    /// 呼吸照護資料上傳
    /// <para>資料庫資料表</para>
    /// </summary>
    public class DB_RCS_VPN_UPLOAD_TRAN: qvtrec
    {
        public string TRAN_ID { get; set; }
        public string CHART_NO { get; set; }
        public string IPD_NO { get; set; }

        /// <summary>
        /// 上傳狀態，0=未上傳、1=已上傳成功
        /// </summary>
        public string UPLOAD_STATUS { get; set; }

        public string UPLOAD_ID { get; set; }

        public string UPLOAD_NAME { get; set; }

        public string UPLOAD_DATE { get; set; }

        public string NEW_TRAN_ID { get; set; }

        public string UPLOAD_FILENAME { get; set; }

        public string CREATE_ID { get; set; }

        public string CREATE_NAME { get; set; }

        public string CREATE_DATE { get; set; }

        public string MODIFY_ID { get; set; }

        public string MODIFY_NAME { get; set; }

        public string MODIFY_DATE { get; set; }

        /// <summary>
        /// 資料狀態，1=正常、2=歷史資料、3=更新資料、9=刪除
        /// </summary>
        public string DATASTATUS { get; set; }

        /// <summary>
        ///脫離呼吸器但因病況需要繼續留住原病房
        /// </summary>
        private List<JSON_DATA> _is_WEANING_REMARK { get; set; }
        /// <summary>
        ///脫離呼吸器但因病況需要繼續留住原病房
        /// </summary>
        public List<JSON_DATA> is_WEANING_REMARK
        {
            get
            {
                return _is_WEANING_REMARK;
            }
            set
            {
                if (value == null)
                {
                    this.WEANING_REMARK = "";
                }
                else
                {
                    _is_WEANING_REMARK = value;
                    if (_is_WEANING_REMARK.Exists(x => x.chkd))
                    {
                        this.WEANING_REMARK = "Y";
                    }
                    else
                    {
                        this.WEANING_REMARK = "";
                    }
                }
               
            }
        }

    }

    /// <summary>
    /// 呼吸照護資料上傳XML
    /// <para>＊ 為必填欄位</para>
    /// </summary>
    public class qvtrec
    {
        /// <summary>
        /// TRAN_ID
        /// </summary>

        public string TRAN_ID { get; set; }
        /// <summary>
        /// ＊ 筆數序號
        /// </summary>
        [XmlElement(ElementName = "f1")]
        public string TRAN_NO { get; set; }

        /// <summary>
        /// ＊ 資料類別
        /// <para>1:個案轉入  2：個案轉出</para>
        /// </summary>
        [XmlElement(ElementName = "f2")]
        public string TRAN_TYPE { get; set; }

        /// <summary>
        /// ＊ 身分證字號
        /// <para>1:個案轉入  2：個案轉出</para>
        /// </summary>
        [XmlElement(ElementName = "f3")]
        public string ID_NO { get; set; }

        /// <summary>
        /// ＊ 出生日期
        /// <para>民國年月日(YYYMMDD)</para>
        /// </summary>
        [XmlElement(ElementName = "f4")]
        public string BIRTH_DAY { get; set; }

        /// <summary>
        /// ＊ 姓名
        /// <para></para>
        /// </summary>
        [XmlElement(ElementName = "f5")]
        public string PATIENT_NAME { get; set; }

        /// <summary>
        /// ＊ 性別
        /// <para>1:男性  2:女性</para>
        /// </summary>
        [XmlElement(ElementName = "f6")]
        public string GENDER { get; set; }

        /// <summary>
        /// ＊ 轉入/轉出日期
        /// <para>民國年月日(YYYMMDD)</para>
        /// </summary>
        [XmlElement(ElementName = "f7")]
        public string TRAN_DATE { get; set; }

        /// <summary>
        /// 病患來源
        /// <para>資料類別(TRAN_TYPE)=1 轉入，本欄位必填</para>
        /// <para>1：本院  2：他院  3：居家照護  4：其他(如護理之家或養護機構)</para>
        /// </summary>
        [XmlElement(ElementName = "f8")]
        public string PATIENT_SOURCE { get; set; }

        /// <summary>
        /// 病患來源病床類別
        /// <para>1. 資料類別(TRAN_TYPE)=1 轉入，且病患來源(PATIENT_SOURCE)='1'-本院、'2'-他院</para>
        /// <para>下列選項：1：門急診  2：ICU  3：RCC 4：RCW  5：一般病房  6：其他病房</para>
        /// <para>2. 病患來源 = '3' - 居家照護、'4'-其他，本欄位空白</para>
        /// <para>3. 資料類別(TRAN_TYPE)=2 轉出:本欄位空白</para>
        /// </summary>
        [XmlElement(ElementName = "f9")]
        public string STATION_TYPE { get; set; }

        /// <summary>
        /// 病患來源醫事機構代碼 /轉出醫事機構代碼
        /// <para>資料類別(TRAN_TYPE)=1 轉入</para>
        /// <para>1.  病患來源(PATIENT_SOURCE)='1' ，本欄位空白(系統自動設定病患來源醫事機構代碼=轉入醫事機構代號)</para>
        /// <para>2.  病患來源(PATIENT_SOURCE)='2'，'3'，本欄位必填</para>
        /// <para>3.  病患來源(PATIENT_SOURCE)='4'，本欄位可填寫或空白（ps.若填寫則必須是正確的醫事機構代號；若空白則"病患來源醫事機構名稱"不可空白）</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出</para>
        /// <para>1.  轉出目的醫院類別='1' ，本欄位空白(系統自動設定病患轉出醫事機構代碼=轉入醫事機構代號)</para>
        /// <para>2.  轉出目的醫院類別='2' ，本欄位必填</para>
        /// </summary>
        [XmlElement(ElementName = "f10")]
        public string HOSP_ID { get; set; }

        /// <summary>
        /// 病患來源醫事機構名稱 /轉出醫事機構名稱
        /// <para>資料類別(TRAN_TYPE)=1 轉入，病患來源='4'時，本欄位必填</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出，轉出目的醫院類別='2'時，本欄位必填</para>
        /// <para></para>
        /// </summary>
        [XmlElement(ElementName = "f11")]
        public string HOSP_NAME { get; set; }

        /// <summary>
        /// 開始使用呼吸器日期
        /// <para>民國年月日(YYYMMDD)</para>
        /// <para>資料類別(TRAN_TYPE)=1 轉入，本欄位必填</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出，本欄位空白</para>
        /// </summary>
        [XmlElement(ElementName = "f12")]
        public string MV_START_DATE { get; set; }

        /// <summary>
        /// 使用呼吸器原因
        /// <para>資料類別(TRAN_TYPE)=1 轉入，本欄位必填</para>
        /// <para>1：中樞神經病變(CVA,hypoxic encephalopathy,頭部外傷)</para>
        /// <para>2：重大病患(敗血症,肺炎,心肌梗塞,心衰竭等內科病患)</para>
        /// <para>3：開刀後</para>
        /// <para>4：慢性肺疾病患(COPD,asthma,pneumoconiosis、、、)</para>
        /// <para>5：肌肉神經元病變</para>
        /// <para>6：胸廓畸形</para>
        /// <para>7：其他</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出，本欄位空白</para>
        /// </summary>
        [XmlElement(ElementName = "f13")]
        public string MV_REASON { get; set; }

        /// <summary>
        /// 轉入醫事機構代號
        /// </summary>
        [XmlElement(ElementName = "f14")]
        public string TRAN_HOSP_ID { get; set; }

        /// <summary>
        /// 轉入醫院病床類別/轉出前醫院病床類別
        /// <para>1：ICU；2：RCC；3：RCW；4：居家 5：一般病房；6：其他病房</para>
        /// <para>資料類別(TRAN_TYPE)=1 轉入，填入轉入醫院病床類別</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出，填入轉出前醫院病床類別</para>
        /// </summary>
        [XmlElement(ElementName = "f15")]
        public string TRAN_STATION_TYPE { get; set; }

        /// <summary>
        /// 轉入理由
        /// <para>資料類別(TRAN_TYPE)=1 轉入，填入轉入醫院病床類別</para>
        /// <para>1：病況需要加強照護</para>
        /// <para>2：嚐試進行脫離呼吸器</para>
        /// <para>轉入醫院病床類別='1：ICU'時，本欄位必填</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出，本欄位空白</para>
        /// <para></para>
        /// </summary>
        [XmlElement(ElementName = "f16")]
        public string TRAN_REASON { get; set; }

        /// <summary>
        /// 氣道介面
        /// <para>1：氣管插管 2：氣切插管 3：面罩 4：鼻面罩 5：Nasal Prong 6：O2 Supply 7：無人工氣道</para>
        /// <para>當呼吸器=1 (使用呼吸器)，則欄值只能輸入1,2,3,4</para>
        /// <para>當呼吸器=2 (使用CPCA或Bi-PAP)，則本欄值只能輸入1,2,3,4,5</para>
        /// <para>當呼吸器=3 (未使用呼吸器)，則本欄值只能輸入1,2,5,6,7</para>
        /// <para>＊ 轉出時情形(TRAN_SITUATION)='3'(死亡)時本欄位空白，其他情形本欄位必填</para>
        /// </summary>
        [XmlElement(ElementName = "f17")]
        public string MV_MODE { get; set; }

        /// <summary>
        /// 意識形態
        /// <para>1：清醒  2：不清醒 </para>
        /// <para>＊ 轉出時情形='3'(死亡)時本欄位空，其他情形本欄位必填</para>
        /// </summary>
        [XmlElement(ElementName = "f18")]
        public string CONSCIOUS { get; set; }

        /// <summary>
        /// 呼吸器
        /// <para>1：使用呼吸器  2：使用CPCA或Bi-PAP  3：未使用呼吸器</para>
        /// <para>資料類別(TRAN_TYPE)=1 轉入，本欄位必填</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出</para>
        /// <para>＊ 轉出時情形(TRAN_SITUATION)='3'(死亡)時本欄位空白，其他情形本欄位必填</para>
        /// <para>＊ 轉出時情形(TRAN_SITUATION)='2'(脫離呼吸器成功)或'6'(嘗試脫離呼吸器中)時本欄只能填"3"</para>
        /// </summary>
        [XmlElement(ElementName = "f19")]
        public string MV { get; set; }

        /// <summary>
        /// 轉出目的醫院類別
        /// <para>資料類別(TRAN_TYPE)=1 轉入，本欄位必填</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出</para>
        /// <para>1：本院 2：他院 3：返家或接受居家照護 4：其他(如護理之家或養護機構)</para>
        /// <para>＊ 轉出時情形(TRAN_SITUATION)='1'、'2'、'4'、'5'、'6' 本欄位必填</para>
        /// <para>＊ 轉出時情形(TRAN_SITUATION)='3'空白</para>
        /// </summary>
        [XmlElement(ElementName = "f20")]
        public string TRAN_HOSP { get; set; }

        /// <summary>
        /// 轉出目的病床類別
        /// <para>資料類別(TRAN_TYPE)=1 轉入，本欄位空白</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出</para>
        /// <para>＊ 轉出目的醫院類別='1'-本院、'2'-他院</para>
        /// <para>1：ICU 2：RCC 3：RCW 4：一般病房 5：其他病房</para>
        /// <para>＊ 轉出時情形(TRAN_SITUATION)='1'，'6'本欄位必填</para>
        /// <para>＊ 轉出時情形(TRAN_SITUATION)='2'本欄位必填，但不可填 2-RCC、3-RCW</para>
        /// <para>＊ 轉出時情形(TRAN_SITUATION)='3'、'4'、'5'空白</para>
        /// </summary>
        [XmlElement(ElementName = "f21")]
        public string TRAN_BED { get; set; }

        /// <summary>
        /// 轉出時情形
        /// <para>1：未脫離呼吸器 2：脫離呼吸器成功 3：死亡 4：一般自動出院 5：病危自動出院 6：嘗試脫離呼吸器中</para>
        /// </summary>
        [XmlElement(ElementName = "f22")]
        public string TRAN_SITUATION { get; set; }

        /// <summary>
        /// 脫離呼吸器但因病況需要繼續留住原病房註記
        /// <para>資料類別(TRAN_TYPE)=1 轉入，本欄位空白</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出</para>
        /// <para>Y：留住原病房</para>
        /// <para>空白：轉出不留住原病房</para>
        /// <para>本欄位若填"Y'，則轉出時情形(TRAN_SITUATION)欄位值只能填 '2' (脫離呼吸器成功)</para>
        /// </summary>
        [XmlElement(ElementName = "f23")]
        public string WEANING_REMARK { get; set; }

        /// <summary>
        /// 脫離呼吸器成功/嘗試脫離呼吸器中日期
        /// <para>資料類別(TRAN_TYPE)=1 轉入，本欄位空白</para>
        /// <para>資料類別(TRAN_TYPE)=2 轉出</para>
        /// <para>轉出時情形(TRAN_SITUATION)= '2' (脫離呼吸器成功)，本欄位填入日期，格式為yyymmdd（yyy民國年）</para>
        /// <para>轉出時情形(TRAN_SITUATION)= '6' (嘗試脫離呼吸器中)，本欄位填入日期時間，格式為yyymmddhhmi（yyy民國年，hh為24小時制00~23，mi為分鐘00~59）</para>
        /// <para>其他轉出情形本欄為空白</para>
        /// </summary>
        [XmlElement(ElementName = "f24")]
        public string WEANING_DATE { get; set; }


    }

}
