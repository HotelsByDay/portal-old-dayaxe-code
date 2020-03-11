using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using DayaxeDal.Custom;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DayaxeDal.Ultility
{
    public class ExcelExportHelper
    {
        public static string ExcelContentType
        {
            get
            {
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
        }

        public static DataTable ListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();

            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public static byte[] ExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake)
        {

            byte[] result;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", heading));
                int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 3;

                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        item[0] = index;
                        index++;
                    }
                }


                // add the content into the Excel file  
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

                // autofit width of cells with small content  
                int columnIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                    if (maxLength < 150)
                    {
                        workSheet.Column(columnIndex).AutoFit();
                    }
                    columnIndex++;
                }

                // format header - bold, yellow on black  
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
                }

                // format cells - add borders  
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                // removed ignored columns  
                for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                {
                    if (i == 0 && showSrNo)
                    {
                        continue;
                    }
                    if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                    {
                        workSheet.DeleteColumn(i + 1);
                    }
                }

                if (!String.IsNullOrEmpty(heading))
                {
                    workSheet.Cells["A1"].Value = heading;
                    workSheet.Cells["A1"].Style.Font.Size = 20;

                    workSheet.InsertColumn(1, 1);
                    workSheet.InsertRow(1, 1);
                    workSheet.Column(1).Width = 5;
                }

                result = package.GetAsByteArray();
            }

            return result;
        }

        public static byte[] ExportExcel(List<KeyValuePair<string, DataTable>> keyValuePairs, params string[] columnsToTake)
        {
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                keyValuePairs.ForEach(keyValuePair =>
                {
                    int startRowFrom = 3;
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0}", keyValuePair.Key));
                    var dataTable = keyValuePair.Value;

                    // add the content into the Excel file  
                    workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

                    // autofit width of cells with small content  
                    int columnIndex = 1;
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                        int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                        if (maxLength < 150)
                        {
                            workSheet.Column(columnIndex).AutoFit();
                        }
                        columnIndex++;
                    }

                    workSheet.Row(startRowFrom).Height = 30;
                    // format header - bold, yellow on black  
                    using (ExcelRange r = workSheet.Cells[startRowFrom, 2, startRowFrom, dataTable.Columns.Count])
                    {
                        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        r.Style.Font.Bold = true;
                        r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#4f81bd"));
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        r.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    // Set Custom With Title
                    columnIndex = 2;
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        ExcelRange columnCells = workSheet.Cells[startRowFrom, columnIndex, startRowFrom, columnIndex];
                        columnCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        switch (columnIndex)
                        {
                            case 2:
                                columnCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                columnCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;

                                columnCells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                                columnCells.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);

                                columnCells.Value = "";
                                columnCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                columnCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ffffff"));
                                break;
                            case 4:
                                columnCells.Value = "Utilization %";
                                break;
                            case 5:
                                columnCells.Value = "Tickets Sold";
                                break;
                            case 6:
                                columnCells.Value = "Tickets Redeemed";
                                break;
                            case 7:
                                columnCells.Value = "Tickets Expired";
                                break;
                            case 8:
                                columnCells.Value = "Tickets Refunded";
                                break;
                            case 9:
                                columnCells.Value = "Gross Sales";
                                break;
                            case 10:
                                columnCells.Value = "Net Sales";
                                break;
                            case 11:
                                columnCells.Value = "Net Revenue";
                                break;
                            case 12:
                                columnCells.Value = "Avg Incremental Revenue";
                                break;
                            case 13:
                                columnCells.Value = "% sold";
                                break;
                            case 14:
                                columnCells.Value = "% redeemed";
                                break;
                            case 15:
                                columnCells.Value = "% expired";
                                break;
                            case 16:
                                columnCells.Value = "% refunds";
                                break;
                        }
                        columnIndex++;
                    }

                    // format cells - add borders  
                    using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                    {
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    }

                    // removed ignored columns  
                    for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                    {
                        if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                        {
                            workSheet.DeleteColumn(i + 1);
                        }
                    }

                    // Set Data with Formula
                    for (int rowIndex = 4; rowIndex < 16; rowIndex++)
                    {
                        for (int col = 8; col < 15; col++)
                        {
                            ExcelRange columnCells = workSheet.Cells[rowIndex, col, rowIndex, col];
                            switch (col)
                            {
                                case 8: // gross sales
                                    columnCells.Style.Numberformat.Format = "$0.00";

                                    // ultilization %
                                    ExcelRange ultilizationCells = workSheet.Cells[rowIndex, 3, rowIndex, 3];
                                    ultilizationCells.Formula = string.Format("IFERROR((E{0} * 100% / B{0}),0)", rowIndex);
                                    ultilizationCells.Style.Numberformat.Format = "#0%";
                                    break;
                                case 9: // net sales
                                    columnCells.Style.Numberformat.Format = "$0.00";
                                    break;
                                case 10: // net revenue
                                    columnCells.Style.Numberformat.Format = "$0.00";
                                    break;
                                case 11: // Avg Incremental Revenue
                                    columnCells.Style.Numberformat.Format = "$0.00";
                                    break;
                                case 12: // % sold
                                    columnCells.Formula = string.Format("IFERROR((D{0} * 100% / B{0}),0)", rowIndex);
                                    columnCells.Style.Numberformat.Format = "#0%";
                                    break;
                                case 13: // % redeemed
                                    columnCells.Formula = string.Format("IFERROR((E{0} * 100% / D{0}),0)", rowIndex);
                                    columnCells.Style.Numberformat.Format = "#0%";
                                    break;
                                case 14: // % expired
                                    columnCells.Formula = string.Format("IFERROR((F{0} * 100% / D{0}),0)", rowIndex);
                                    columnCells.Style.Numberformat.Format = "#0%";
                                    break;
                                case 15: // % refund
                                    columnCells.Formula = string.Format("IFERROR((G{0} * 100% / D{0}),0)", rowIndex);
                                    columnCells.Style.Numberformat.Format = "#0%";
                                    break;
                            }
                        }
                    }

                    // Add Total Row
                    for (columnIndex = 1; columnIndex < dataTable.Columns.Count; columnIndex++)
                    {
                        ExcelRange columnCells = workSheet.Cells[startRowFrom + 13, columnIndex, startRowFrom + 13, columnIndex];
                        columnCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        columnCells.Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                        columnCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        columnCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        columnCells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        columnCells.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        columnCells.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        columnCells.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                        switch (columnIndex)
                        {
                            case 1: // Total
                                columnCells.Value = "Total";
                                columnCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                columnCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ffffff"));
                                break;
                            case 2: // Inventory
                                columnCells.Formula = "SUM(B4:B15)";
                                break;
                            case 3: // Ultilization
                                columnCells.Formula = "AVERAGE(C4:C15)";
                                columnCells.Style.Numberformat.Format = "#0%";
                                break;
                            case 4: // Tickets Sold
                                columnCells.Formula = "SUM(D4:D15)";
                                break;
                            case 5: // Tickets Redeemed
                                columnCells.Formula = "SUM(E4:E15)";
                                break;
                            case 6: // Tickets Expired
                                columnCells.Formula = "SUM(F4:F15)";
                                break;
                            case 7: // Tickets Refunded
                                columnCells.Formula = "SUM(G4:G15)";
                                break;
                            case 8: // Gross Sale
                                columnCells.Formula = "SUM(H4:H15)";
                                columnCells.Style.Numberformat.Format = "$0.00";
                                break;
                            case 9: // Net Sales
                                columnCells.Formula = "SUM(I4:I15)";
                                columnCells.Style.Numberformat.Format = "$0.00";
                                break;
                            case 10: // Net Revenue
                                columnCells.Formula = "SUM(J4:J15)";
                                columnCells.Style.Numberformat.Format = "$0.00";
                                break;
                            case 11: // Avg Incremental Revenue
                                columnCells.Formula = "SUM(K4:K15)";
                                columnCells.Style.Numberformat.Format = "$0.00";
                                break;
                            case 12: // % Sold
                                columnCells.Formula = "IFERROR((D16 * 100% / B16), 0)";
                                columnCells.Style.Numberformat.Format = "#0%";
                                break;
                            case 13: // % Redeemed
                                columnCells.Formula = "IFERROR((E16 * 100% / D16), 0)";
                                columnCells.Style.Numberformat.Format = "#0%";
                                break;
                            case 14: // % expired
                                columnCells.Formula = "IFERROR((E16 * 100% / D16), 0)";
                                columnCells.Style.Numberformat.Format = "#0%";
                                break;
                            case 15: // % expired
                                columnCells.Formula = "IFERROR((G16 * 100% / D16), 0)";
                                columnCells.Style.Numberformat.Format = "#0%";
                                break;
                        }
                    }

                    if (!String.IsNullOrEmpty(keyValuePair.Key))
                    {
                        workSheet.Cells["A1"].Value = keyValuePair.Key;
                        workSheet.Cells["A1"].Style.Font.Size = 20;

                        workSheet.InsertColumn(1, 1);
                        workSheet.InsertRow(1, 1);
                        workSheet.Column(1).Width = 5;
                    }
                });

                result = package.GetAsByteArray();
            }

            return result;
        }

        public static byte[] ExportExcel<T>(List<T> data, string Heading = "", bool showSlno = false, params string[] ColumnsToTake)
        {
            return ExportExcel(ListToDataTable<T>(data), Heading, showSlno, ColumnsToTake);
        }

        public static byte[] ExportExcel(List<ProductSalesReportObject> data, params string[] columnsToTake)
        {
            var result = new List<KeyValuePair<string, DataTable>>();
            data.ForEach(item =>
            {
                result.Add(new KeyValuePair<string, DataTable>(item.ProductObject.ProductName, ListToDataTable(item.SalesReportObject)));
            });
            return ExportExcel(result, columnsToTake);
        }
    }
}
