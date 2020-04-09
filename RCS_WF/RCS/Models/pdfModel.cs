using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCS.Models
{
    public class pdfModel
    {
        /// <summary>
        /// itextSharpHtml列印資料
        /// </summary>
        [AllowHtml]
        public string HtmlStr { get; set;}

        /// <summary>脫離評估列印</summary>
        [AllowHtml]
        public string HtmlStrWeaning { get; set; }

        /// <summary>胸腔復健列印-FROM Patient Assess</summary>
        [AllowHtml]
        public string HtmlStrCXR { get; set; }

        /// <summary>胸腔復健列印-FROM CPT</summary>
        [AllowHtml]
        public string HtmlStrCXR2 { get; set; }
    }
    //https://stackoverflow.com/questions/19389999/can-itextsharp-xmlworker-render-embedded-images

}