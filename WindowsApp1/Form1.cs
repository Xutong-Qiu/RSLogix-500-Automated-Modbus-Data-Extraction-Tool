using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApp1
{

    public partial class Form1
    {

        private List<Button> buttons = new List<Button>();
        private object logixApp = Interaction.CreateObject("RSLogix500.Application");
        private object logixObj;
        private object data_collection;
        private PLC_DB db;
        private Thread BGTask;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            buttons = new List<Button>() { Search, display_data_button, find_modbus_mapping_button, find_invalid_mapping_button, load_ref_table_button, modbus_doc, save_button, generate_modbus_doc };
            foreach (var btn in buttons) // When the software is open. Disable all buttons except load file button
                btn.Enabled = false;
            // PictureBox1.Image = My.Resources.blank_image
        }

        private void load_file_Click(object sender, EventArgs e)
        {
            // checking if rslogix500 is opened successfully
            if (logixApp is null)
            {
                MessageBox.Show("ERROR: Failed to open RSLogix500 software.", "ERROR: 001", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // Checking if a file is already loaded
            if (db is not null)
            {
                var result = MessageBox.Show("Are you sure to load a new file?", "A File Has Been Loaded", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    foreach (var btn in buttons)
                        btn.Enabled = false;
                    DataGridView1.Rows.Clear();
                    DataGridView1.Columns.Clear();
                    rss_path.Text = "Not Loaded";
                    if (logixObj is not null)
                    {
                        // change the second to true to save
                        logixObj.close(false, false);
                    }
                }
                else if (result == DialogResult.No)
                {
                    return;
                }
            }
            // open file load dialog
            var openFileDialog = new OpenFileDialog();
            string path;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
            }
            else
            {
                return;
            }
            logixObj = IOHandler.LoadRSSFile(path, logixApp);
            if (logixObj is null)
            {
                return;
            }
            // preparing a new database
            db = new PLC_DB(logixObj);
            // display rss file
            rss_path.Text = path;
            // buttons control
            foreach (var btn in buttons)
                btn.Enabled = true;
            generate_modbus_doc.Enabled = false;
            if (db.IsX())
            {
                generate_modbus_doc.Enabled = true;
            }
            else
            {
                // start background loading
                StartLoadMapping();
            }
            MessageBox.Show("PLC program successfully loaded.");
            Focus(); // Keep the window in the view after closing the message box
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (logixObj is not null)
            {
                // change the second to true to save
                logixObj.close((object)false, (object)true);
                logixObj = null;
            }
            if (logixApp is not null)
            {
                logixApp.Quit((object)true, (object)false);
                logixApp = null;
            }
        }


        private void Search_Click(object sender, EventArgs e)
        {
            string addr = TextBox1.Text;
            addr = addr.Trim(); // get rid of all leading and tailing spaces
            int rowIndex = -1;
            foreach (DataGridViewRow row in DataGridView1.Rows)
            {
                if (row.Cells[0].Value is not null && row.Cells[0].Value.ToString().Equals(addr))
                {
                    rowIndex = row.Index;
                    break;
                }
            }
            if (rowIndex != -1)
            {
                // Clear the selection first
                DataGridView1.ClearSelection();
                // Select the found row
                DataGridView1.Rows[rowIndex].Selected = true;
                // Scroll to the found row if it is not currently visible
                if (!DataGridView1.Rows[rowIndex].Displayed)
                {
                    DataGridView1.FirstDisplayedScrollingRowIndex = rowIndex;
                }
            }

            if (db.ContainEntry(addr))
            {
                // MessageBox.Show("Name: " + dataEntries(addr).Item1 + vbNewLine + "Description: " + dataEntries(addr).Item2)
                Label2.Text = db.GetTagName(addr);
            }
            else
            {
                MessageBox.Show("Address not found.");
            }
            if (db.ContainEntry(addr))
            {
                string s = "";
                var l = db.GetMappingTarget(addr);
                foreach (var i in l)
                    s = s + i + " ";
                Label6.Text = s;
            }
            else
            {
                Label6.Text = "Empty";
            }
        }

        private void find_exception_mapping_button_click(object sender, EventArgs e)
        {
            var content = db.GetExceptionMapping();
            string exePath = Application.StartupPath;
            string filePath = Path.Combine(exePath, "invalid.csv");
            if (File.Exists(filePath))
            {
                try
                {
                    using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        // If the file can be opened, it is not in use
                    }
                }
                catch (IOException ex)
                {
                    // If an IOException occurred, the file is in use
                    MessageBox.Show("File is being used by other applications.");
                }
            }
            else
            {
                var fs = File.Create(filePath);
                fs.Close();
            }
            IOHandler.WriteToCSV(content, new[] { "Address", "Name", "Source", "Description" }, filePath);
            DisplayList(content, new[] { "Address", "Name", "Source", "Description" }, true);
            DataGridView1.ReadOnly = false;
        }

        // Handle the KeyDown event for your textbox
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the pressed key is the Enter key
            if (e.KeyCode == Keys.Enter)
            {
                // Handle your search logic same as clicking on the button
                Search_Click(sender, e);
            }
        }

        private void display_data_button_click(object sender, EventArgs e)
        {
            DisplayList(db.DBtoList(), new[] { "Address", "Name", "Description" }, false);
            return;
        }

        private void find_mapping_Click(object sender, EventArgs e)
        {
            WaitForBGTask();
            var modifiedList = db.GetModbusList();
            var content = new List<string[]>();
            foreach (var addr in modifiedList)
            {
                string str = "";
                foreach (var src in db.GetMappingSrc(addr))
                    str += src + " ";
                content.Add(new[] { addr, db.GetTagName(addr), str, db.GetDescription(addr) });
            }
            content.Sort(new DataEntryComparer());
            DisplayList(content, new[] { "Address", "Name", "Source", "Description" }, true);
            DataGridView1.ReadOnly = false;
        }

        private void DisplayList(List<string[]> list, string[] cols, bool hightlight)
        {
            DataGridView1.Rows.Clear();
            DataGridView1.Columns.Clear();

            // Assuming the string arrays all have the same length.
            foreach (string colName in cols)
            {
                var col = new DataGridViewTextBoxColumn();
                col.Name = colName;
                DataGridView1.Columns.Add(col);
            }

            foreach (string[] item in list)
            {
                var row = DataGridView1.Rows[DataGridView1.Rows.Add(item)];
                if (hightlight)
                {
                    if (string.IsNullOrEmpty(item[1]))
                    {
                        row.Cells[1].Style.BackColor = Color.LightSalmon;
                    }
                    if (string.IsNullOrEmpty(item[3]))
                    {
                        row.Cells[3].Style.BackColor = Color.LightSalmon;
                    }
                }
            }

            DataGridView1.RowHeadersVisible = false;
            DataGridView1.AutoResizeColumns();
            DataGridView1.ReadOnly = true;
            DataGridView1.AllowUserToAddRows = false;
        }


        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Determine which column and row to allow editing
            int columnIndexToAllowEdit = 3; // Adjust this to the column index you want to allow editing (0-based index)
            string cellContent = Conversions.ToString(DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            if (e.ColumnIndex == columnIndexToAllowEdit && DataGridView1.Columns[columnIndexToAllowEdit].HeaderText == "Description")
            {
                // Allow editing for the specified cell
                if (cellContent is null || string.IsNullOrEmpty(cellContent))
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void DataGridView1_CanEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (DataGridView1.ReadOnly == true)
            {
                return;
            }
            var editableCols = new HashSet<int>(new[] { 1, 3 });
            if (editableCols.Contains(e.ColumnIndex) && DataGridView1.Columns[3].HeaderText == "Description")
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && !DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly)
                {
                    // Begin editing the clicked cell
                    DataGridView1.BeginEdit(false);
                }
            }
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var desp = DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            var addr = DataGridView1.Rows[e.RowIndex].Cells[0].Value;
            db.UpdateDescription(Conversions.ToString(addr), Conversions.ToString(desp));
            DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightBlue;
        }


        private void load_ref_table_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            string path;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
                string extension = Path.GetExtension(path);
                if (extension != ".xlsx")
                {
                    MessageBox.Show("The file must be an Excel file.");
                    return;
                }
            }
            else
            {
                return;
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
                    return;
                }
            }
            else
            {
                var fs = File.Create(path);
                fs.Close();
            }
            db.TagAbbrDictionary = IOHandler.LoadExcel(path);
            ref_file_status.Text = path;
            MessageBox.Show("Reference File Loaded.");
        }

        private bool toggle = false;
        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Check if the clicked column is the one you want to perform custom sorting on.
            if (e.ColumnIndex == 0)
            {
                DataGridView1.Sort(new DataGridEntryComparer(toggle));
                toggle = !toggle;
            }
        }


        private void StartLoadMapping()
        {
            BGTask = new Thread(db.LoadMapping);
            BGTask.Start();
        }

        private void WaitForBGTask()
        {
            // Use Join to wait for the background thread to complete
            if (BGTask is not null && BGTask.IsAlive)
            {
                BGTask.Join();
            }
        }

        private void modbus_doc_button_Click(object sender, EventArgs e)
        {
            WaitForBGTask();
            string exePath = Application.StartupPath;
            string filePath = Path.Combine(exePath, "Modbus Address.xlsx");
            var content = db.GetModbusData();
            DisplayList(content, new[] { "Address", "Tag Name", "Description" }, false);
            generate_modbus_doc.Enabled = true;
        }

        private void save_rss_button_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure to save changes to the RSS file?", "Save", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                WaitForBGTask();
                IOHandler.WriteToProject(logixObj, db);
                logixObj.Save((object)true, (object)true);
            }
            else if (result == DialogResult.No)
            {
                return;
            }
        }

        private void generate_modbus_doc_Click(object sender, EventArgs e)
        {
            WaitForBGTask();
            string exePath = Application.StartupPath;
            string filePath = Path.Combine(exePath, "Modbus Address.xlsx");
            var content = db.GetModbusData();
            IOHandler.GenerateModbusDoc(content, new[] { "Address", "Tag Name", "Description" }, filePath);
            Focus();
        }
    }
}