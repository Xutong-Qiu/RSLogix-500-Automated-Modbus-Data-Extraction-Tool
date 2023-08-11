using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApp1
{

    public static class IOHandler
    {
        /// <summary>
    /// This function writes data to a CSV file whose path is spcified by the caller function.
    /// </summary>
    /// <param name="data">Data to be written to the file</param>
    /// <param name="path">The path of the target file</param>
    /// <returns>A boolean that indicates whether the operation is successful or not.</returns>
        public static bool WriteToCSV(List<string[]> data, string[] columnNames, string path)
        {
            using (var sw = new StreamWriter(path))
            {
                sw.WriteLine(string.Join(",", columnNames));

                foreach (string[] row in data)
                    sw.WriteLine(string.Join(",", row));
            }
            MessageBox.Show("Successfully written to " + path);
            return true;
        }


        public static void GenerateModbusDoc(List<string[]> content, string[] columnNames, string filePath)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            // Add a new sheet at the specified index
            Worksheet coilSheet = (Worksheet)workbook.Sheets.Add();
            Worksheet regSheet = (Worksheet)workbook.Sheets.Add();
            coilSheet.Name = "Coil";
            regSheet.Name = "Register";
            // Write column names
            for (int colIndex = 0, loopTo = columnNames.Length - 1; colIndex <= loopTo; colIndex++)
                regSheet.Cells[1, colIndex + 1].Value = columnNames[colIndex];
            for (int colIndex = 0, loopTo1 = columnNames.Length - 1; colIndex <= loopTo1; colIndex++)
                coilSheet.Cells[1, colIndex + 1].Value = columnNames[colIndex];

            int regRowIndex = 2;
            int coilRowIndex = 2;
            // Write content
            for (int rowIndex = 0, loopTo2 = content.Count - 1; rowIndex <= loopTo2; rowIndex++)
            {
                string[] rowData = content[rowIndex];
                if (Conversions.ToSingle(rowData[0]) < 1000f)
                {
                    for (int colIndex = 0, loopTo3 = rowData.Length - 1; colIndex <= loopTo3; colIndex++)
                        regSheet.Cells[regRowIndex, colIndex + 1].Value = rowData[colIndex];
                    regRowIndex += 1;
                }
                else
                {
                    for (int colIndex = 0, loopTo4 = rowData.Length - 1; colIndex <= loopTo4; colIndex++)
                        coilSheet.Cells[coilRowIndex, colIndex + 1].Value = rowData[colIndex];
                    coilRowIndex += 1;
                }
            }


            Worksheet defaultSheet = (Worksheet)workbook.Sheets["Sheet1"];
            defaultSheet.Delete();

            // Save the workbook
            try
            {
                workbook.SaveAs(filePath);
                MessageBox.Show("Successfully written to " + filePath);
            }
            catch (COMException ex)
            {
                MessageBox.Show("File not saved.");
            }

            workbook.Close(false);
            excelApp.Quit();

            ReleaseObject(regSheet);
            ReleaseObject(coilSheet);
            ReleaseObject(workbook);
            ReleaseObject(excelApp);

        }

        /// <summary>
    /// This function applies all changes in the database to the RSS project file.
    /// </summary>
    /// <param name="proj">The RSS project</param>
    /// <param name="db">The database</param>
        public static void WriteToProject(object proj, PLC_DB db)
        {
            var modifiedList = db.GetModbusList();
            foreach (var addr in modifiedList)
            {
                var record = proj.AddrSymRecords.GetRecordViaAddrOrSym(addr, (object)0);
                if (record is null)
                {
                    record = proj.AddrSymRecords.add();
                    record.SetAddress(addr);
                    record.SetScope((object)0);
                } // got the des addr instance
                if (!record.SetSymbol(db.GetTagName(addr)) && db.GetTagName(addr) == "")
                {
                    MessageBox.Show("Unable to set Name: " + db.GetTagName(addr) + "Address: " + addr);
                }
                if (!record.SetDescription(db.GetDescription(addr)) && db.GetDescription(addr) == "")
                {
                    MessageBox.Show("Unable to set Description: " + db.GetDescription(addr) + "Address: " + addr);
                }
            }
        }

        /// <summary>
    /// This function loads a RSS file
    /// </summary>
    /// <param name="rslogixApp">The RsLogix 500 Pro Application object that is used to open an RSS file</param>
    /// <param name="path">Thepath to the RSS file to be loaded</param>
    /// <returns>A RSS project object obtained from the RSS file</returns>
        public static object LoadRSSFile(string path, object rslogixApp)
        {
            // Obtain path chose by the user
            var openFileDialog = new OpenFileDialog();
            string extension = Path.GetExtension(path);
            if (extension != ".RSS")
            {
                MessageBox.Show("The file must be an RSS file.");
                return null;
            }
            // check if file is in use
            if (File.Exists(path))
            {
                try
                {
                    using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show("File is being used by other applications.");
                    return null;
                }
            }
            else
            {
                var fs = File.Create(path);
                fs.Close();
            }
            // open the file
            var rslogixProj = rslogixApp.FileOpen(path, (object)false, (object)false, (object)true);
            if (rslogixProj is null)
            {
                MessageBox.Show("ERROR: Failed to open the file.");
                return null;
            }
            return rslogixProj;
        }

        public static Dictionary<string, string> LoadExcel(string path)
        {

            var excelApp = new Microsoft.Office.Interop.Excel.Application();

            // Open the workbook (file) in read-only mode
            var workbook = excelApp.Workbooks.Open(path, ReadOnly: true);

            // Assuming the data is in the first worksheet (you can adjust the sheet index as needed)
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];
            var range = worksheet.UsedRange;

            int numRows = range.Rows.Count;
            var dataDictionary = new Dictionary<string, string>();

            // Loop through the data and add to the dictionary
            for (int row = 1, loopTo = numRows; row <= loopTo; row++)
            {
                string key = Conversions.ToString(range.Cells[row, 1].Value); // Assuming the key is in the first column
                string value = Conversions.ToString(range.Cells[row, 2].Value); // Assuming the value is in the second column

                if (!string.IsNullOrEmpty(key)) // Ignore rows with empty keys
                {
                    dataDictionary.Add(key, value);
                }
            }

            // Close the workbook and release resources
            workbook.Close();
            excelApp.Quit();
            ReleaseObject(worksheet);
            ReleaseObject(workbook);
            ReleaseObject(excelApp);

            return dataDictionary;
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

    }
}