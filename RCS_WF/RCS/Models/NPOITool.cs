using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using RCS_Data;
using RCSData.Models;


namespace RCS.Models
{
    public class NPOITool
    {
        IWorkbook _WorkBook = new HSSFWorkbook();
        WebMethod _RcsWebMethod = new WebMethod();
        byte[] bytes;
        public string Lasterror = "";
        public byte[] ExportExcelTable(DataTable pDt, string titleName, string pFileName, string pSheetName, string pBindColName = "", string pColTitleName = "", string pExportActionName = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pExportActionName))
                    pExportActionName = "";
                switch (pExportActionName)
                {
                    case "ExportExcel_RT_Index":
                        return ExportExcel_RT_Index(pDt, titleName, pFileName, pSheetName, pBindColName, pColTitleName);
                    default:
                        #region 預設
                        if (pDt != null && pDt.Rows.Count > 0)
                        {
                            List<string> BindColName = pBindColName.Split(',').ToList();
                            List<string> ColTitleName = pColTitleName.Split(',').ToList();
                            MemoryStream MS = new MemoryStream();
                            ISheet WorkSheet = _WorkBook.CreateSheet(pSheetName);
                            IRow SheetHeadRow = WorkSheet.CreateRow(0);
                            ICellStyle SheetHeadCellStyle = _WorkBook.CreateCellStyle();
                            IRow SheetRow = default(HSSFRow);
                            ICell SheetCell = default(HSSFCell);
                            string GetHeadCellsWidth = Convert.ToString(20 * 140);
                            ICellStyle GetHeadCellsStyle = this.SetCellsStyle(SetCellsStyleType.Head);
                            ICellStyle GetRowCellsStyle = this.SetCellsStyle(SetCellsStyleType.Row);
                            ICellStyle GetRowCellsNumberStyle = this.SetCellsStyle(SetCellsStyleType.Row);//數字字體
                            GetRowCellsNumberStyle.SetFont(this.SetCellsFont(SetCellsFontType.TimeNewRoman));

                            //註解樣式
                            ICellStyle GetNoteCellsStyle = this.SetCellsStyle(SetCellsStyleType.Note);
                            //抬頭樣式
                            ICellStyle GetTitleCellsStyle = this.SetCellsStyle(SetCellsStyleType.Title);

                            //設定欄位寬度
                            for (int i = 1; i < BindColName.Count; i++)
                            {
                                WorkSheet.SetColumnWidth(i, 24 * 180);
                            }
                            int nowRow = 0;
                            int nowCell = 0;
                            int conbolCell = 0;
                            conbolCell = BindColName.Count - 1;
                            //抬頭
                            SheetRow = WorkSheet.CreateRow(nowRow);
                            SheetCell = SheetRow.CreateCell(nowCell);
                            SheetCell.CellStyle = GetTitleCellsStyle;
                            SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                            SheetCell.SetCellValue(titleName);
                            WorkSheet.AddMergedRegion(new CellRangeAddress(nowRow, nowCell, nowRow, conbolCell));  //合併儲存格

                            nowRow++;
                            nowCell = 0;
                            conbolCell = nowCell;
                            //自訂第一列
                            SheetRow = WorkSheet.CreateRow(nowRow);
                            for (int i = nowCell; i < ColTitleName.Count; i++)
                            {
                                SheetCell = SheetRow.CreateCell(i);
                                SheetCell.CellStyle = GetHeadCellsStyle;
                                //GetHeadCellsStyle.WrapText = true;//自動換行
                                SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                                SheetCell.SetCellValue(ColTitleName[i]);
                            }

                            for (int i = 0; i < pDt.Rows.Count; i++)
                            {
                                nowRow++;
                                nowCell = 0;
                                conbolCell = nowCell;
                                SheetRow = WorkSheet.CreateRow(nowRow);

                                for (int j = nowCell; j < BindColName.Count; j++)
                                {
                                    SheetCell = SheetRow.CreateCell(j);
                                    SheetCell.CellStyle = GetRowCellsStyle;
                                    SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                                    if (j == BindColName.Count - 1)
                                    {
                                        SheetCell.CellStyle.WrapText = true;
                                    }
                                    SheetCell.SetCellValue(pDt.Rows[i][BindColName[j]].ToString());
                                }
                            }

                            _WorkBook.Write(MS);

                            bytes = MS.ToArray();

                            _WorkBook = null;
                            MS.Close();
                            MS.Dispose();
                        }
                        #endregion
                        break;
                }

            }
            catch (Exception ex)
            {
                Lasterror = ex.ToString();
            }

            return bytes;
        }
        
        public byte[] ExportExcel_RT_Index(DataTable pDt, string titleName, string pFileName, string pSheetName, string pBindColName = "", string pColTitleName = "")
        {
            #region 預設
            if (pDt != null && pDt.Rows.Count > 0)
            {
                List<string> BindColName = pBindColName.Split(',').ToList();
                List<string> ColTitleName = pColTitleName.Split(',').ToList();
                MemoryStream MS = new MemoryStream();
                ISheet WorkSheet = _WorkBook.CreateSheet(pSheetName);
                IRow SheetHeadRow = WorkSheet.CreateRow(0);
                ICellStyle SheetHeadCellStyle = _WorkBook.CreateCellStyle();
                IRow SheetRow = default(HSSFRow);
                ICell SheetCell = default(HSSFCell);
                string GetHeadCellsWidth = Convert.ToString(20 * 140);
                ICellStyle GetHeadCellsStyle = this.SetCellsStyle(SetCellsStyleType.Head);
                ICellStyle GetRowCellsStyle = this.SetCellsStyle(SetCellsStyleType.Row);
                ICellStyle GetRowCellsNumberStyle = this.SetCellsStyle(SetCellsStyleType.Row);//數字字體
                GetRowCellsNumberStyle.SetFont(this.SetCellsFont(SetCellsFontType.TimeNewRoman));

                //註解樣式
                ICellStyle GetNoteCellsStyle = this.SetCellsStyle(SetCellsStyleType.Note);
                //抬頭樣式
                ICellStyle GetTitleCellsStyle = this.SetCellsStyle(SetCellsStyleType.Title);
                /*------------- Style 樣式 [下] -------------*/
                ICellStyle iCellStyle_RedNoneThin = SetCellsStyle_Text(NPOI.HSSF.Util.HSSFColor.Red.Index, (short)FontBoldWeight.None, BorderStyle.Thin, 12);
                ICellStyle iCellStyle_BlackNoneThin14 = SetCellsStyle_Text(NPOI.HSSF.Util.HSSFColor.Black.Index, (short)FontBoldWeight.None, BorderStyle.Thin, 14);
                    iCellStyle_BlackNoneThin14.Alignment = HorizontalAlignment.Center;
                    iCellStyle_BlackNoneThin14.VerticalAlignment = VerticalAlignment.Bottom;
                ICellStyle iCellStyle_BlackNoneThin1WrapText4 = SetCellsStyle_Text(NPOI.HSSF.Util.HSSFColor.Black.Index, (short)FontBoldWeight.None, BorderStyle.Thin, 14);
                    iCellStyle_BlackNoneThin1WrapText4.Alignment = HorizontalAlignment.Center;
                    iCellStyle_BlackNoneThin1WrapText4.VerticalAlignment = VerticalAlignment.Bottom;
                    iCellStyle_BlackNoneThin1WrapText4.WrapText = true; //自動換行
                ICellStyle iCellStyle_BlackNoneThin7 = SetCellsStyle_Text(NPOI.HSSF.Util.HSSFColor.Black.Index, (short)FontBoldWeight.None, BorderStyle.Thin, 7);
                    iCellStyle_BlackNoneThin7.Alignment = HorizontalAlignment.Left;
                    iCellStyle_BlackNoneThin7.VerticalAlignment = VerticalAlignment.Top;
                    iCellStyle_BlackNoneThin7.WrapText = true; //自動換行
                ICellStyle iCellStyle_EmptyThin = SetCellsStyle_Empty(BorderStyle.Thin);
                /*------------- Style 樣式[上] -------------*/
                int nowRow = 0;
                int nowCell = 0;
                int conbolCell = 0;
                conbolCell = BindColName.Count - 1;
                //新增，第1橫行 2018.11.09
                SheetRow = WorkSheet.CreateRow(0);
                WorkSheet.GetRow(0).Height = 18 * 20; //設置行高row.Height需要乘以20
                {
                    SheetCell = SheetRow.CreateCell(1); //新增，第1直列
                    SheetCell.CellStyle = iCellStyle_RedNoneThin; //紅色-細體-無底線
                    SheetCell.SetCellType(CellType.String);
                    SheetCell.SetCellValue(DateTime.Now.ToString("yyyy/MM/dd"));

                    SheetCell = SheetRow.CreateCell(5); //新增，第5直列
                    SheetCell.CellStyle = iCellStyle_RedNoneThin; //紅色-細體-無底線
                    SheetCell.SetCellType(CellType.String);
                    SheetCell.SetCellValue("病人清單");
                }

                nowRow++;
                nowCell = 0;
                conbolCell = nowCell;
                //自訂第一列
                SheetRow = WorkSheet.CreateRow(nowRow);
                WorkSheet.GetRow(nowRow).Height = 18 * 20; //設置行高row.Height需要乘以20
                for (int ii = nowCell; ii < ColTitleName.Count; ii++)
                {
                    SheetCell = SheetRow.CreateCell(ii);
                    /*------------- Style 樣式 2018.11.09 [下] -------------*/
                    SheetCell.CellStyle =
                        //有值，才能設定文字
                        !string.IsNullOrWhiteSpace(ColTitleName[ii])
                        ? iCellStyle_RedNoneThin
                        //沒值，單純設定外框
                        : iCellStyle_EmptyThin;
                    /*------------- Style 樣式 2018.11.09 [上] -------------*/
                    SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                    SheetCell.SetCellValue(ColTitleName[ii]);
                }

                //Rows-橫列ii
                for (int ii = 0; ii < pDt.Rows.Count; ii++)
                {
                    nowRow++;
                    nowCell = 0;
                    conbolCell = nowCell;
                    SheetRow = WorkSheet.CreateRow(nowRow);
                    WorkSheet.GetRow(nowRow).Height = 60 * 20; //設置行高row.Height需要乘以20
                    //Column-直行jj
                    for (int jj = nowCell; jj < BindColName.Count; jj++)
                    {
                        SheetCell = SheetRow.CreateCell(jj);
                        SheetCell.SetCellType(CellType.String);
                        SheetCell.SetCellValue(pDt.Rows[ii][BindColName[jj]].ToString());
                        /*------------- Style 樣式 [下] -------------*/
                        //Column-直行jj
                        switch (jj)
                        {
                            case 5:
                            case 7:
                            case 8:
                                SheetCell.CellStyle =
                                    //有值，才能設定文字
                                    !string.IsNullOrWhiteSpace(pDt.Rows[ii][BindColName[jj]].ToString())
                                    ? iCellStyle_BlackNoneThin1WrapText4
                                    //沒值，單純設定外框
                                    : iCellStyle_EmptyThin;
                                break;
                            case 10:
                                SheetCell.CellStyle =
                                    //有值，才能設定文字
                                    !string.IsNullOrWhiteSpace(pDt.Rows[ii][BindColName[jj]].ToString())
                                    ? iCellStyle_BlackNoneThin7
                                    //沒值，單純設定外框
                                    : iCellStyle_EmptyThin;
                                break;
                            default:
                                SheetCell.CellStyle =
                                    //有值，才能設定文字
                                    !string.IsNullOrWhiteSpace(pDt.Rows[ii][BindColName[jj]].ToString())
                                    ? iCellStyle_BlackNoneThin14
                                    //沒值，單純設定外框
                                    : iCellStyle_EmptyThin;
                                break;
                        }//switch
                        /*------------- Style 樣式[上] -------------*/
                    }//for-[Column-直行]
                }//for-[Rows-橫列]

                //獲取當前列的寬度，然後對比本列的長度，取最大值
                for (int columnNum = 0; columnNum <= BindColName.Count; columnNum++)
                {
                    int columnWidth = WorkSheet.GetColumnWidth(columnNum) / 256;
                    for (int rowNum = 1; rowNum <= WorkSheet.LastRowNum; rowNum++)
                    {
                        IRow currentRow;
                        //當前行，未被使用過
                        if (WorkSheet.GetRow(rowNum) == null)
                            currentRow = WorkSheet.CreateRow(rowNum);
                        else
                            currentRow = WorkSheet.GetRow(rowNum);
                        //當前內容，有值
                        if (currentRow.GetCell(columnNum) != null)
                        {
                            ICell currentCell = currentRow.GetCell(columnNum);
                            //依據 \n 轉換成 List
                            List<string> string_List = currentCell.ToString().Split('\n').ToList();
                            int length = System.Text.Encoding.Default.GetBytes(
                                string_List.OrderByDescending(s => s.Length).First()
                                ).Length;
                            if (columnWidth < length)
                                columnWidth = length;
                        }
                    }
                    
                }//獲取當前列的寬度，然後對比本列的長度，取最大值
 
                WorkSheet.SetColumnWidth(0, (int)((5.36 + 0.72) * 256)); // 在EXCEL文档中实际列宽为5.36
                WorkSheet.SetColumnWidth(1, (int)((8.09 + 0.72) * 256)); // 在EXCEL文档中实际列宽为5.36
                WorkSheet.SetColumnWidth(2, (int)((8.91 + 0.72) * 256)); // 在EXCEL文档中实际列宽为8.91
                WorkSheet.SetColumnWidth(3, (int)((12.09 + 0.72) * 256)); // 在EXCEL文档中实际列宽为12.09
                WorkSheet.SetColumnWidth(4, (int)((8.64 + 0.72) * 256)); // 在EXCEL文档中实际列宽为8.64
                WorkSheet.SetColumnWidth(5, (int)((30.09 + 0.72) * 256)); // 在EXCEL文档中实际列宽为30.09
                WorkSheet.SetColumnWidth(6, (int)((25.09 + 0.72) * 256)); // 在EXCEL文档中实际列宽为25.09
                WorkSheet.SetColumnWidth(7, (int)((8.18 + 0.72) * 256)); // 在EXCEL文档中实际列宽为8.18
                WorkSheet.SetColumnWidth(8, (int)((9.09 + 0.72) * 256)); // 在EXCEL文档中实际列宽为9.09
                WorkSheet.SetColumnWidth(9, (int)((12.64 + 0.72) * 256)); // 在EXCEL文档中实际列宽为12.64


                _WorkBook.Write(MS);

                bytes = MS.ToArray();

                _WorkBook = null;
                MS.Close();
                MS.Dispose();
            }
            #endregion

            return bytes;
        }


        public byte[] ExportExcelTable(DataTable pDt, string pFileName, string pSheetName, string titleName = "", bool showColname = true)
        {
            try
            {                
                int nowRow = 0;
                MemoryStream MS = new MemoryStream();
                ISheet WorkSheet = _WorkBook.CreateSheet(pSheetName);//產生一頁Sheet

                //在Sheet中新增一列(IRow)
                IRow SheetHeadRow = WorkSheet.CreateRow(0);// 插入新列
                IRow SheetRow = default(HSSFRow);

                //在該列(IRow)中新增一格(ICell)
                ICell SheetCell = default(HSSFCell);

                //設定Cell的樣式(ICellStyle)
                ICellStyle GetTitleCellsStyle = this.SetCellsStyle(SetCellsStyleType.Title);//抬頭樣式
                ICellStyle SheetHeadCellStyle = _WorkBook.CreateCellStyle();// 產生欄位樣式設定
                ICellStyle GetHeadCellsStyle = this.SetCellsStyle(SetCellsStyleType.Head);
                ICellStyle GetRowCellsStyle = this.SetCellsStyle(SetCellsStyleType.Row);

                if (!string.IsNullOrWhiteSpace(titleName))
                {
                    //抬頭
                    SheetRow = WorkSheet.CreateRow(nowRow);// 產生新列
                    SheetCell = SheetRow.CreateCell(0);// 產生欄位
                    SheetCell.CellStyle = GetTitleCellsStyle;
                    SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                    SheetCell.SetCellValue(titleName);
                    WorkSheet.AddMergedRegion(new CellRangeAddress(nowRow, 0, nowRow, pDt.Columns.Count));  //合併儲存格
                    nowRow++;
                }//if (!string.IsNullOrWhiteSpace(titleName))
                if (showColname)
                {
                    SheetRow = WorkSheet.CreateRow(nowRow);// 產生新列
                    for (int j = 0; j < pDt.Columns.Count; j++)
                    {
                        SheetCell = SheetRow.CreateCell(j);// 產生欄位
                        SheetCell.CellStyle = GetHeadCellsStyle;
                        SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                        SheetCell.SetCellValue(pDt.Columns[j].ColumnName.ToString());
                    }//for (int j = 0; j < pDt.Columns.Count; j++)
                    nowRow++;
                }//if (showColname)

                if (pDt != null && pDt.Rows.Count > 0)
                {
                    for (int i = 0; i < pDt.Rows.Count; i++)
                    {
                        SheetRow = WorkSheet.CreateRow(nowRow);// 產生新列
                        for (int j = 0; j < pDt.Columns.Count; j++)
                        {
                            SheetCell = SheetRow.CreateCell(j);// 產生欄位
                            SheetCell.CellStyle = GetRowCellsStyle;
                            SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                            SheetCell.SetCellValue(pDt.Rows[i][pDt.Columns[j].ColumnName].ToString());
                        }//for (int j = 0; j < pDt.Columns.Count; j++)
                        nowRow++;
                    }//for (int i = 0; i < pDt.Rows.Count; i++)
                }//if (pDt != null && pDt.Rows.Count > 0)
                _WorkBook.Write(MS);
                bytes = MS.ToArray();
                _WorkBook = null;
                MS.Close();
                MS.Dispose();
            }//try
            catch (Exception ex)
            {
                Lasterror = ex.ToString();
            }

            return bytes;
        }//public byte[] ExportExcelTable(DataTable pDt, string pFileName, string pSheetName, string titleName = "", bool showColname = true)

    
        public ICellStyle SetCellsStyle_Text(
            short input_HSSFColor
            , short input_BoldNone
            , BorderStyle input_BottomThinThick
            , short input_TextSize = 10
        ){
            //有值，才能設定文字
            ICellStyle CellsStyle = _WorkBook.CreateCellStyle();
            //畫格線
            CellsStyle.BorderBottom = input_BottomThinThick; //BorderStyle.Thick
            CellsStyle.BorderLeft = BorderStyle.Thin;
            CellsStyle.BorderRight = BorderStyle.Thin;
            CellsStyle.BorderTop = BorderStyle.Thin;
            CellsStyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            CellsStyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            CellsStyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            CellsStyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            CellsStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
            //調整文字水平
            CellsStyle.Alignment = HorizontalAlignment.Center;
            CellsStyle.VerticalAlignment = VerticalAlignment.Center;
            //CellsStyle.WrapText = true; //自動換行
            //設定字體.顏色
            IFont StyleFont = _WorkBook.CreateFont();
            StyleFont.Color = input_HSSFColor;
            //文字顏色
            StyleFont.FontName = "標楷體";
            StyleFont.FontHeightInPoints = input_TextSize; //設定文字大小為12pt
            StyleFont.Boldweight = input_BoldNone; //(short)FontBoldWeight.Bold;
            //文字字型
            CellsStyle.SetFont(StyleFont);
            return CellsStyle;
        }
        public ICellStyle SetCellsStyle_Empty(BorderStyle input_BottomThinThick)
        {
            //沒值，單純設定外框
            ICellStyle CellsStyle = _WorkBook.CreateCellStyle();
            //畫格線
            CellsStyle.BorderBottom = input_BottomThinThick;
            CellsStyle.BorderLeft = BorderStyle.Thin;
            CellsStyle.BorderRight = BorderStyle.Thin;
            CellsStyle.BorderTop = BorderStyle.Thin;
            CellsStyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            CellsStyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            CellsStyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            CellsStyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            CellsStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
            //調整文字水平
            CellsStyle.Alignment = HorizontalAlignment.Center;
            CellsStyle.VerticalAlignment = VerticalAlignment.Center;
            return CellsStyle;
        }

        public byte[] ExportShiftExcel(RCS_Data.scheduling pSchData)
        {
            Dictionary<string, string> pStr = new Dictionary<string, string>();
            pStr.Add("holiday","休假");
            pStr.Add("workday","工作");
            pStr.Add("real_off","實休");
            pStr.Add("vo_month","VO月休");
            pStr.Add("vo_left","VO餘");
            pStr.Add("overtime","加班");
            pStr.Add("pre_off","借休");
            pStr.Add("last_hr","上月時數");
            pStr.Add("this_hr","本月時數");
            try
            {
                if (pSchData.year > 0 && pSchData.month > 0 && pSchData.schedul_data.Count > 0)
                {
                    string yyyy = pSchData.year.ToString().PadLeft(4, '0');
                    string mm = pSchData.month.ToString().PadLeft(2, '0');
                    string yyyymm_date = yyyy + mm;

                    MemoryStream MS = new MemoryStream();
                    ISheet WorkSheet = _WorkBook.CreateSheet(yyyymm_date + "排班表");
                    IRow SheetHeadRow = WorkSheet.CreateRow(0);
                    ICellStyle SheetHeadCellStyle = _WorkBook.CreateCellStyle();
                    IRow SheetRow = default(HSSFRow);
                    ICell SheetCell = default(HSSFCell);
                    string GetHeadCellsWidth = Convert.ToString(20 * 140);
                    ICellStyle HeadCellsStyle = this.SetCellsStyle(SetCellsStyleType.Head);
                    ICellStyle RowCellsStyle = this.SetCellsStyle(SetCellsStyleType.Row);
                    ICellStyle GetRowCellsNumberStyle = this.SetCellsStyle(SetCellsStyleType.Row);//數字字體
                    GetRowCellsNumberStyle.SetFont(this.SetCellsFont(SetCellsFontType.TimeNewRoman));
                    //抬頭樣式
                    ICellStyle TitleCellsStyle = this.SetCellsStyle(SetCellsStyleType.Title);

                    SheetRow = WorkSheet.CreateRow(0);
                    SheetCell = SheetRow.CreateCell(0);
                    SheetCell.CellStyle = HeadCellsStyle;
                    SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                    SheetCell.SetCellValue(yyyymm_date+ "排班表");

                    SheetRow = WorkSheet.CreateRow(1);
                    SheetCell = SheetRow.CreateCell(0);
                    SheetCell.CellStyle = HeadCellsStyle;
                    SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                    SheetCell.SetCellValue("當月OF" + pSchData.month_off + "天");
                    SheetCell = SheetRow.CreateCell(1);
                    SheetCell.CellStyle = HeadCellsStyle;
                    SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                    SheetCell.SetCellValue("工時" + pSchData.working_hr + "小時");

                    int nowRow = 2;
                    int nowCell = 0;
                    IRow DayRow = null;
                    foreach (RCS_Data.SchedulData item in pSchData.schedul_data)
                    {
                        nowCell = 0;
                        if (nowRow == 2)
                        {
                            DayRow = WorkSheet.CreateRow(nowRow);//日期title row
                            nowRow++;
                        }
                        SheetRow = WorkSheet.CreateRow(nowRow);
                        SheetCell = SheetRow.CreateCell(nowCell);
                        SheetCell.CellStyle = HeadCellsStyle;
                        SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                        SheetCell.SetCellValue(item.op_name);
                        nowCell++;
                        foreach (string d in item.day_data.Keys)
                        {
                            if (nowRow == 3)
                            {
                                SheetCell = DayRow.CreateCell(nowCell);
                                SheetCell.CellStyle = RowCellsStyle;
                                SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                                SheetCell.SetCellValue(d);
                                if (pStr.ContainsKey(d))
                                    SheetCell.SetCellValue(pStr[d]);
                                else
                                    SheetCell.SetCellValue(d);
                            }
                            RCS_Data.DayData dd = item.day_data[d];
                            SheetCell = SheetRow.CreateCell(nowCell);
                            SheetCell.CellStyle = RowCellsStyle;
                            SheetCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                            if (pStr.ContainsKey(d))
                                SheetCell.SetCellValue(dd.area);
                            else
                                SheetCell.SetCellValue(dd.stype);
                            nowCell++;
                        }
                        nowRow++;
                    }

                    _WorkBook.Write(MS);
                    bytes = MS.ToArray();

                    _WorkBook = null;
                    MS.Close();
                    MS.Dispose();
                }
                
            }
            catch (Exception ex)
            {
                Lasterror = ex.ToString();
            }

            return bytes;
        }

        #region "設定[標題]儲存格的樣式(函數)"

        /// <summary>設定Cell樣式Type</summary>
        public enum SetCellsStyleType
        {
            /// <summary>抬頭</summary>
            Head,
            /// <summary>資料列</summary>
            Row,
            /// <summary>標題</summary>
            Title,
            /// <summary>註解</summary>
            Note
        }

        /// <summary>設定CellStyle樣式</summary>
        /// <param name="pType">Cell樣式Type</param>
        /// <returns></returns>
        public ICellStyle SetCellsStyle(SetCellsStyleType pType)
        {
            ICellStyle CellsStyle = _WorkBook.CreateCellStyle();
            IFont BlackBoldFont = _WorkBook.CreateFont();  // 粗體_黑字
            //建立Style
            switch (pType)
            {
                case SetCellsStyleType.Head:
                    #region

                    //CellsStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.WHITE.index     '顏色(一定要和圖樣同時存在才有作用)
                    //CellsStyle.FillPattern = SS.UserModel.FillPatternType.SOLID_FOREGROUND      '圖樣
                    //畫格線
                    CellsStyle.BorderBottom = BorderStyle.Thin;
                    CellsStyle.BorderLeft = BorderStyle.Thin;
                    CellsStyle.BorderRight = BorderStyle.Thin;
                    CellsStyle.BorderTop = BorderStyle.Thin;

                    CellsStyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    CellsStyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    CellsStyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    CellsStyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

                    //-----
                    //文字位置
                    CellsStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //調整文字水平
                    CellsStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    //調整文字垂直
                    //-----

                    #endregion
                    break;
                case SetCellsStyleType.Row:
                    #region

                    //畫格線
                    CellsStyle.BorderBottom = BorderStyle.Thin;
                    CellsStyle.BorderLeft = BorderStyle.Thin;
                    CellsStyle.BorderRight = BorderStyle.Thin;
                    CellsStyle.BorderTop = BorderStyle.Thin;
                    CellsStyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    CellsStyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    CellsStyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    CellsStyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    CellsStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;

                    #endregion
                    break;
                default:
                    break;
            }
            //調整文字水平
            CellsStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            CellsStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            //-----
            //設定字體.顏色
            IFont StyleFont = _WorkBook.CreateFont();
            StyleFont.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
            //文字顏色
            StyleFont.FontName = "新細明體";
            //文字字型
            CellsStyle.SetFont(StyleFont);
            //-----
            return CellsStyle;
        }
        #endregion

        #region "設定[標題]儲存格的文字樣式(函數)"

        /// <summary>設定Cell文字樣式Type</summary>
        public enum SetCellsFontType
        {
            /// <summary>粗體</summary>
            BlackBoldFont,
            /// <summary>牛肉麵字型(數字)</summary>
            TimeNewRoman
        }

        /// <summary>設定Cell文字樣式</summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        private IFont SetCellsFont(SetCellsFontType pType)
        {
            IFont CellsFont = _WorkBook.CreateFont();
            //建立Style
            switch (pType)
            {
                case SetCellsFontType.BlackBoldFont:
                    // 粗體_黑字
                    //字體顏色
                    CellsFont.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    //字體粗體
                    CellsFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    break;
                case SetCellsFontType.TimeNewRoman:
                    //牛肉麵字型
                    // 粗體_黑字
                    //字體顏色
                    CellsFont.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;    //字體顏色
                    //字型 TimeNewRoman
                    CellsFont.FontName = "TimeNewRoman";    //字型
                    break;
                default:
                    break;
            }
            
            return CellsFont;
        }
        #endregion

    }


    public class ExcelSetting
    {
        public bool isSetbindColName { get { return !string.IsNullOrWhiteSpace(bindColName); } }
        public string titleName { get; set; }

        public string sheetName { get; set; }

        public string bindColName { get; set; }

        public string colTitleName { get; set; }

        public string FileName { get; set; }
        /// <summary>
        /// 匯出使用方法
        /// </summary>
        public string exportActionName { get; set; }
    }
}