// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace DataAnalysisPlugin.Utils
{
    public class NPOIUtil
    {
        /// <summary>
        /// 获取excel内容
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <returns><see cref="DataTable"/></returns>
        public static DataTable ImportExcel(string filePath)
        {
            DataTable dt = new DataTable();
            using (FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                //获取后缀名
                string extension = filePath.Substring(filePath.LastIndexOf(".")).ToLower();
                dt = ImportExcel(fileStream, extension);
            }
            return dt;
        }

        /// <summary>
        /// 获取excel内容
        /// </summary>
        /// <param name="stream">读取的流</param>
        /// <param name="extension">excel文件扩展名 .xlsx .xls</param>
        /// <returns><see cref="DataTable"/></returns>
        public static DataTable ImportExcel(Stream stream, string extension)
        {
            DataTable dt = new DataTable();

            IWorkbook wk = null;

            //判断是否是excel文件
            if (extension == ".xlsx" || extension == ".xls")
            {
                //判断excel的版本
                if (extension == ".xlsx")
                {
                    wk = new XSSFWorkbook(stream);
                }
                else
                {
                    wk = new HSSFWorkbook(stream);
                }

                //获取第一个sheet
                ISheet sheet = wk.GetSheetAt(0);
                //获取第一行
                IRow headrow = sheet.GetRow(0);
                //创建列
                for (int i = headrow.FirstCellNum; i < headrow.Cells.Count; i++)
                {
                    DataColumn datacolum = new DataColumn(headrow.GetCell(i).StringCellValue);
                    //DataColumn datacolum = new DataColumn("F" + (i + 1));
                    dt.Columns.Add(datacolum);
                }
                //读取每行,从第二行起
                for (int r = 1; r <= sheet.LastRowNum; r++)
                {
                    bool result = false;
                    DataRow dr = dt.NewRow();
                    //获取当前行
                    IRow row = sheet.GetRow(r);
                    //读取每列
                    for (int j = 0; j < row.Cells.Count; j++)
                    {
                        ICell cell = row.GetCell(j); //一个单元格
                        dr[j] = GetCellValue(cell); //获取单元格的值
                                                    //全为空则不取
                        if (dr[j].ToString() != "")
                        {
                            result = true;
                        }
                    }
                    if (result == true)
                    {
                        dt.Rows.Add(dr); //把每行追加到DataTable
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// 对单元格进行判断取值
        /// </summary>
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank: //空数据类型 这里类型注意一下，不同版本NPOI大小写可能不一样,有的版本是Blank（首字母大写)
                    return string.Empty;
                case CellType.Boolean: //bool类型
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric: //数字类型
                    if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else //其它数字
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Unknown: //无法识别类型
                default: //默认类型
                    return cell.ToString();//
                case CellType.String: //string 类型
                    return cell.StringCellValue;
                case CellType.Formula: //带公式类型
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }


        #region ITodays

        public static DataTable ImportExcelForITodays(Stream stream, string extension)
        {
            DataTable dt = new DataTable();

            IWorkbook wk = null;

            //判断是否是excel文件
            if (extension == ".xlsx" || extension == ".xls")
            {
                //判断excel的版本
                if (extension == ".xlsx")
                {
                    wk = new XSSFWorkbook(stream);
                }
                else
                {
                    wk = new HSSFWorkbook(stream);
                }

                //获取第一个sheet
                ISheet sheet = wk.GetSheet("爱今天");
                //ISheet sheet = wk.GetSheetAt(0);

                int recordStartRowNum = 0;
                // 跳过前面: "晨音与总结："， 到达 "每天记录："
                for (int i = 0; i < sheet.LastRowNum; i++)
                {
                    // 获取当前行
                    IRow row = sheet.GetRow(i);
                    // 第一个单元格
                    ICell cell = row?.GetCell(0);
                    // 获取单元格的值
                    string cellValue = GetCellValue(cell);
                    if (cellValue.Contains("每天记录"))
                    {
                        recordStartRowNum = i;
                        break;
                    }
                }

                //获取 "每天记录" 的下一行
                IRow headrow = sheet.GetRow(recordStartRowNum + 1);
                //创建列
                for (int i = headrow.FirstCellNum; i < headrow.Cells.Count; i++)
                {
                    DataColumn datacolum = new DataColumn(headrow.GetCell(i).StringCellValue);
                    //DataColumn datacolum = new DataColumn("F" + (i + 1));
                    dt.Columns.Add(datacolum);
                }
                // 读取每行,从 "每天记录" 的 数据行开始
                for (int r = recordStartRowNum + 2; r <= sheet.LastRowNum; r++)
                {
                    bool result = false;
                    DataRow dr = dt.NewRow();
                    //获取当前行
                    IRow row = sheet.GetRow(r);
                    //读取每列
                    for (int j = 0; j < row.Cells.Count; j++)
                    {
                        ICell cell = row?.GetCell(j); //一个单元格
                        dr[j] = GetCellValue(cell); //获取单元格的值
                        //全为空则不取
                        if (dr[j].ToString() != "")
                        {
                            result = true;
                        }
                    }
                    if (result == true)
                    {
                        dt.Rows.Add(dr); //把每行追加到DataTable
                    }
                }
            }

            return dt;
        }
        
        #endregion
    }
}
