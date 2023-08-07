<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    Friend WithEvents load_file_button As Button
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Search As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents find_invalid_mapping_button As Button
    Friend WithEvents display_data_button As Button
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents perform_mapping As Button

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.load_file_button = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Search = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.find_invalid_mapping_button = New System.Windows.Forms.Button()
        Me.display_data_button = New System.Windows.Forms.Button()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.perform_mapping = New System.Windows.Forms.Button()
        Me.load_ref_table_button = New System.Windows.Forms.Button()
        Me.RSS_file = New System.Windows.Forms.Label()
        Me.rss_path = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.ref_file_status = New System.Windows.Forms.Label()
        Me.ref_file = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'load_file_button
        '
        Me.load_file_button.Location = New System.Drawing.Point(12, 12)
        Me.load_file_button.Name = "load_file_button"
        Me.load_file_button.Size = New System.Drawing.Size(157, 60)
        Me.load_file_button.TabIndex = 0
        Me.load_file_button.Text = "Load File"
        Me.load_file_button.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(6, 41)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(64, 20)
        Me.TextBox1.TabIndex = 1
        '
        'Search
        '
        Me.Search.Location = New System.Drawing.Point(76, 39)
        Me.Search.Name = "Search"
        Me.Search.Size = New System.Drawing.Size(75, 23)
        Me.Search.TabIndex = 2
        Me.Search.Text = "Search"
        Me.Search.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 76)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(38, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Name:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(51, 76)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(0, 13)
        Me.Label2.TabIndex = 4
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(7, 104)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(51, 13)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "Mapping:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(64, 104)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(0, 13)
        Me.Label6.TabIndex = 8
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Search)
        Me.GroupBox1.Controls.Add(Me.TextBox1)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 413)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(157, 176)
        Me.GroupBox1.TabIndex = 10
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Search by Address"
        '
        'find_invalid_mapping_button
        '
        Me.find_invalid_mapping_button.Location = New System.Drawing.Point(12, 136)
        Me.find_invalid_mapping_button.Name = "find_invalid_mapping_button"
        Me.find_invalid_mapping_button.Size = New System.Drawing.Size(157, 22)
        Me.find_invalid_mapping_button.TabIndex = 11
        Me.find_invalid_mapping_button.Text = "Find Invalid Mapping"
        Me.find_invalid_mapping_button.UseVisualStyleBackColor = True
        '
        'display_data_button
        '
        Me.display_data_button.Location = New System.Drawing.Point(12, 78)
        Me.display_data_button.Name = "display_data_button"
        Me.display_data_button.Size = New System.Drawing.Size(157, 23)
        Me.display_data_button.TabIndex = 12
        Me.display_data_button.Text = "Display Database"
        Me.display_data_button.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        Me.DataGridView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(175, 12)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersWidth = 45
        Me.DataGridView1.Size = New System.Drawing.Size(682, 490)
        Me.DataGridView1.TabIndex = 13
        '
        'perform_mapping
        '
        Me.perform_mapping.Location = New System.Drawing.Point(12, 107)
        Me.perform_mapping.Name = "perform_mapping"
        Me.perform_mapping.Size = New System.Drawing.Size(157, 23)
        Me.perform_mapping.TabIndex = 14
        Me.perform_mapping.Text = "Perform Mapping"
        Me.perform_mapping.UseVisualStyleBackColor = True
        '
        'load_ref_table_button
        '
        Me.load_ref_table_button.Location = New System.Drawing.Point(12, 213)
        Me.load_ref_table_button.Name = "load_ref_table_button"
        Me.load_ref_table_button.Size = New System.Drawing.Size(157, 22)
        Me.load_ref_table_button.TabIndex = 15
        Me.load_ref_table_button.Text = "Load Ref Table"
        Me.load_ref_table_button.UseVisualStyleBackColor = True
        '
        'RSS_file
        '
        Me.RSS_file.AutoSize = True
        Me.RSS_file.Location = New System.Drawing.Point(6, 16)
        Me.RSS_file.Name = "RSS_file"
        Me.RSS_file.Size = New System.Drawing.Size(51, 13)
        Me.RSS_file.TabIndex = 16
        Me.RSS_file.Text = "RSS File:"
        '
        'rss_path
        '
        Me.rss_path.AutoSize = True
        Me.rss_path.Location = New System.Drawing.Point(63, 16)
        Me.rss_path.Name = "rss_path"
        Me.rss_path.Size = New System.Drawing.Size(63, 13)
        Me.rss_path.TabIndex = 17
        Me.rss_path.Text = "Not Loaded"
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.ref_file_status)
        Me.GroupBox2.Controls.Add(Me.ref_file)
        Me.GroupBox2.Controls.Add(Me.RSS_file)
        Me.GroupBox2.Controls.Add(Me.rss_path)
        Me.GroupBox2.Location = New System.Drawing.Point(175, 509)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(682, 80)
        Me.GroupBox2.TabIndex = 18
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Software Status"
        '
        'ref_file_status
        '
        Me.ref_file_status.AutoSize = True
        Me.ref_file_status.Location = New System.Drawing.Point(63, 34)
        Me.ref_file_status.Name = "ref_file_status"
        Me.ref_file_status.Size = New System.Drawing.Size(41, 13)
        Me.ref_file_status.TabIndex = 19
        Me.ref_file_status.Text = "Default"
        '
        'ref_file
        '
        Me.ref_file.AutoSize = True
        Me.ref_file.Location = New System.Drawing.Point(6, 34)
        Me.ref_file.Name = "ref_file"
        Me.ref_file.Size = New System.Drawing.Size(46, 13)
        Me.ref_file.TabIndex = 18
        Me.ref_file.Text = "Ref File:"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(869, 601)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.load_ref_table_button)
        Me.Controls.Add(Me.perform_mapping)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.display_data_button)
        Me.Controls.Add(Me.find_invalid_mapping_button)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.load_file_button)
        Me.MinimumSize = New System.Drawing.Size(885, 642)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Automated Modbus Extraction"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents load_ref_table_button As Button
    Friend WithEvents RSS_file As Label
    Friend WithEvents rss_path As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents ref_file_status As Label
    Friend WithEvents ref_file As Label
End Class
