using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsApp1
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class Form1 : Form
    {

        internal Button load_file_button;
        internal TextBox TextBox1;
        internal Button Search;
        internal Label Label1;
        internal Label Label2;
        internal Label Label5;
        internal Label Label6;
        internal GroupBox GroupBox1;
        internal Button find_invalid_mapping_button;
        internal Button display_data_button;
        internal DataGridView DataGridView1;
        internal Button find_modbus_mapping_button;

        // Form overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            load_file_button = new Button();
            load_file_button.Click += new EventHandler(load_file_Click);
            TextBox1 = new TextBox();
            TextBox1.KeyDown += new KeyEventHandler(TextBox1_KeyDown);
            Search = new Button();
            Search.Click += new EventHandler(Search_Click);
            Label1 = new Label();
            Label2 = new Label();
            Label5 = new Label();
            Label6 = new Label();
            GroupBox1 = new GroupBox();
            find_invalid_mapping_button = new Button();
            find_invalid_mapping_button.Click += new EventHandler(find_exception_mapping_button_click);
            display_data_button = new Button();
            display_data_button.Click += new EventHandler(display_data_button_click);
            DataGridView1 = new DataGridView();
            DataGridView1.CellBeginEdit += new DataGridViewCellCancelEventHandler(DataGridView1_CellBeginEdit);
            DataGridView1.CellClick += new DataGridViewCellEventHandler(DataGridView1_CanEdit);
            DataGridView1.CellEnter += new DataGridViewCellEventHandler(DataGridView1_CanEdit);
            DataGridView1.CellEndEdit += new DataGridViewCellEventHandler(DataGridView1_CellEndEdit);
            DataGridView1.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(DataGridView1_ColumnHeaderMouseClick);
            find_modbus_mapping_button = new Button();
            find_modbus_mapping_button.Click += new EventHandler(find_mapping_Click);
            load_ref_table_button = new Button();
            load_ref_table_button.Click += new EventHandler(load_ref_table_Click);
            RSS_file = new Label();
            rss_path = new Label();
            GroupBox2 = new GroupBox();
            ref_file_status = new Label();
            ref_file = new Label();
            modbus_doc = new Button();
            modbus_doc.Click += new EventHandler(modbus_doc_button_Click);
            save_button = new Button();
            save_button.Click += new EventHandler(save_rss_button_Click);
            generate_modbus_doc = new Button();
            generate_modbus_doc.Click += new EventHandler(generate_modbus_doc_Click);
            GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridView1).BeginInit();
            GroupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // load_file_button
            // 
            load_file_button.Location = new Point(12, 12);
            load_file_button.Name = "load_file_button";
            load_file_button.Size = new Size(157, 60);
            load_file_button.TabIndex = 0;
            load_file_button.Text = "Load File";
            load_file_button.UseVisualStyleBackColor = true;
            // 
            // TextBox1
            // 
            TextBox1.Location = new Point(6, 41);
            TextBox1.Name = "TextBox1";
            TextBox1.Size = new Size(64, 20);
            TextBox1.TabIndex = 1;
            // 
            // Search
            // 
            Search.Location = new Point(76, 39);
            Search.Name = "Search";
            Search.Size = new Size(75, 23);
            Search.TabIndex = 2;
            Search.Text = "Search";
            Search.UseVisualStyleBackColor = true;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Location = new Point(7, 76);
            Label1.Name = "Label1";
            Label1.Size = new Size(38, 13);
            Label1.TabIndex = 3;
            Label1.Text = "Name:";
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Location = new Point(51, 76);
            Label2.Name = "Label2";
            Label2.Size = new Size(0, 13);
            Label2.TabIndex = 4;
            // 
            // Label5
            // 
            Label5.AutoSize = true;
            Label5.Location = new Point(7, 104);
            Label5.Name = "Label5";
            Label5.Size = new Size(51, 13);
            Label5.TabIndex = 7;
            Label5.Text = "Mapping:";
            // 
            // Label6
            // 
            Label6.AutoSize = true;
            Label6.Location = new Point(64, 104);
            Label6.Name = "Label6";
            Label6.Size = new Size(0, 13);
            Label6.TabIndex = 8;
            // 
            // GroupBox1
            // 
            GroupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            GroupBox1.Controls.Add(Label6);
            GroupBox1.Controls.Add(Label5);
            GroupBox1.Controls.Add(Label2);
            GroupBox1.Controls.Add(Label1);
            GroupBox1.Controls.Add(Search);
            GroupBox1.Controls.Add(TextBox1);
            GroupBox1.Location = new Point(12, 457);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Size = new Size(157, 132);
            GroupBox1.TabIndex = 10;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "Search by Address";
            // 
            // find_invalid_mapping_button
            // 
            find_invalid_mapping_button.Location = new Point(12, 153);
            find_invalid_mapping_button.Name = "find_invalid_mapping_button";
            find_invalid_mapping_button.Size = new Size(157, 33);
            find_invalid_mapping_button.TabIndex = 11;
            find_invalid_mapping_button.Text = "Display Exception Mappings";
            find_invalid_mapping_button.UseVisualStyleBackColor = true;
            // 
            // display_data_button
            // 
            display_data_button.Location = new Point(12, 78);
            display_data_button.Name = "display_data_button";
            display_data_button.Size = new Size(157, 30);
            display_data_button.TabIndex = 12;
            display_data_button.Text = "Display Database";
            display_data_button.UseVisualStyleBackColor = true;
            // 
            // DataGridView1
            // 
            DataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridView1.Location = new Point(175, 12);
            DataGridView1.Name = "DataGridView1";
            DataGridView1.RowHeadersWidth = 45;
            DataGridView1.Size = new Size(682, 490);
            DataGridView1.TabIndex = 13;
            // 
            // find_modbus_mapping_button
            // 
            find_modbus_mapping_button.Location = new Point(12, 114);
            find_modbus_mapping_button.Name = "find_modbus_mapping_button";
            find_modbus_mapping_button.Size = new Size(157, 33);
            find_modbus_mapping_button.TabIndex = 14;
            find_modbus_mapping_button.Text = "Display Modbus Mappings";
            find_modbus_mapping_button.UseVisualStyleBackColor = true;
            // 
            // load_ref_table_button
            // 
            load_ref_table_button.Location = new Point(12, 340);
            load_ref_table_button.Name = "load_ref_table_button";
            load_ref_table_button.Size = new Size(157, 22);
            load_ref_table_button.TabIndex = 15;
            load_ref_table_button.Text = "Load Ref Table";
            load_ref_table_button.UseVisualStyleBackColor = true;
            // 
            // RSS_file
            // 
            RSS_file.AutoSize = true;
            RSS_file.Location = new Point(6, 16);
            RSS_file.Name = "RSS_file";
            RSS_file.Size = new Size(51, 13);
            RSS_file.TabIndex = 16;
            RSS_file.Text = "RSS File:";
            // 
            // rss_path
            // 
            rss_path.AutoSize = true;
            rss_path.Location = new Point(63, 16);
            rss_path.Name = "rss_path";
            rss_path.Size = new Size(63, 13);
            rss_path.TabIndex = 17;
            rss_path.Text = "Not Loaded";
            // 
            // GroupBox2
            // 
            GroupBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GroupBox2.Controls.Add(ref_file_status);
            GroupBox2.Controls.Add(ref_file);
            GroupBox2.Controls.Add(RSS_file);
            GroupBox2.Controls.Add(rss_path);
            GroupBox2.Location = new Point(175, 509);
            GroupBox2.Name = "GroupBox2";
            GroupBox2.Size = new Size(682, 80);
            GroupBox2.TabIndex = 18;
            GroupBox2.TabStop = false;
            GroupBox2.Text = "Software Status";
            // 
            // ref_file_status
            // 
            ref_file_status.AutoSize = true;
            ref_file_status.Location = new Point(63, 34);
            ref_file_status.Name = "ref_file_status";
            ref_file_status.Size = new Size(41, 13);
            ref_file_status.TabIndex = 19;
            ref_file_status.Text = "Default";
            // 
            // ref_file
            // 
            ref_file.AutoSize = true;
            ref_file.Location = new Point(6, 34);
            ref_file.Name = "ref_file";
            ref_file.Size = new Size(46, 13);
            ref_file.TabIndex = 18;
            ref_file.Text = "Ref File:";
            // 
            // modbus_doc
            // 
            modbus_doc.Location = new Point(12, 192);
            modbus_doc.Name = "modbus_doc";
            modbus_doc.Size = new Size(157, 32);
            modbus_doc.TabIndex = 19;
            modbus_doc.Text = "Modbus Doc";
            modbus_doc.UseVisualStyleBackColor = true;
            // 
            // save_button
            // 
            save_button.Location = new Point(12, 368);
            save_button.Name = "save_button";
            save_button.Size = new Size(157, 26);
            save_button.TabIndex = 20;
            save_button.Text = "Save PLC";
            save_button.UseVisualStyleBackColor = true;
            // 
            // generate_modbus_doc
            // 
            generate_modbus_doc.Location = new Point(12, 400);
            generate_modbus_doc.Name = "generate_modbus_doc";
            generate_modbus_doc.Size = new Size(157, 26);
            generate_modbus_doc.TabIndex = 21;
            generate_modbus_doc.Text = "Generate Doc";
            generate_modbus_doc.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(869, 601);
            Controls.Add(generate_modbus_doc);
            Controls.Add(save_button);
            Controls.Add(modbus_doc);
            Controls.Add(GroupBox2);
            Controls.Add(load_ref_table_button);
            Controls.Add(find_modbus_mapping_button);
            Controls.Add(DataGridView1);
            Controls.Add(display_data_button);
            Controls.Add(find_invalid_mapping_button);
            Controls.Add(GroupBox1);
            Controls.Add(load_file_button);
            MinimumSize = new Size(885, 642);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Automated Modbus Extraction";
            GroupBox1.ResumeLayout(false);
            GroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridView1).EndInit();
            GroupBox2.ResumeLayout(false);
            GroupBox2.PerformLayout();
            Load += new EventHandler(Form1_Load);
            FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            ResumeLayout(false);

        }

        internal Button load_ref_table_button;
        internal Label RSS_file;
        internal Label rss_path;
        internal GroupBox GroupBox2;
        internal Label ref_file_status;
        internal Label ref_file;
        internal Button modbus_doc;
        internal Button save_button;
        internal Button generate_modbus_doc;
    }
}