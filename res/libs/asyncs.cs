using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Stolovaya_1._0.res.libs
{
    internal class asyncs
    {
        public static async Task<bool> ExportToWord(DataTable dataTable)
        {
            await Task.Run(() =>
            {
                if (dataTable.Rows.Count > 0)
                {
                    Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                    word.Application.Documents.Add(Type.Missing);

                    Microsoft.Office.Interop.Word.Table table = word.Application.ActiveDocument.Tables.Add(word.Selection.Range, dataTable.Rows.Count, dataTable.Columns.Count, Type.Missing, Type.Missing);
                    table.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                    table.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;

                    /*for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        table.Cell(1, i + 1).Range.Text = dataTable.Columns[i].ColumnName;
                    }*/

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            table.Cell(i + 1, j + 1).Range.Text = dataTable.Rows[i][j].ToString();
                        }
                    }

                    word.Visible = true;
                    return true;
                }
                else
                {
                    MessageBox.Show("No data to export!");
                    return false;
                }
            });
            return false;
        }
        public static DataTable DataGridtoDataTable(DataGrid dg)
        {
            try
            {
                dg.SelectAllCells();
                dg.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, dg);
                dg.UnselectAllCells();
                String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                string[] Lines = result.Split(new string[] { "\r\n", "\n" },
                StringSplitOptions.None);
                string[] Fields;
                Fields = Lines[0].Split(new char[] { ',' });
                int Cols = Fields.GetLength(0);
                DataTable dt = new DataTable();
                //1st row must be column names; force lower case to ensure matching later on.
                for (int i = 0; i < Cols; i++)
                    dt.Columns.Add(Fields[i].ToUpper(), typeof(string));
                DataRow Row;
                for (int i = 1; i < Lines.GetLength(0) - 1; i++)
                {
                    Fields = Lines[i].Split(new char[] { ',' });
                    Row = dt.NewRow();
                    for (int f = 0; f < Cols; f++)
                    {
                        Row[f] = Fields[f];
                    }
                    dt.Rows.Add(Row);
                }
                return dt;
            }
            catch
            {
                return new DataTable();
            }
        }
    }
}
