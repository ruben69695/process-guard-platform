<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits MetroFramework.Forms.MetroForm

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.NotifyIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.menu = New MetroFramework.Controls.MetroContextMenu(Me.components)
        Me.StopServiceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StartServiceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UninstallToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.labelSeeds = New MetroFramework.Controls.MetroLabel()
        Me.Save_btn = New MetroFramework.Controls.MetroButton()
        Me.Seeds_TextBox = New MetroFramework.Controls.MetroTextBox()
        Me.MetroLabel1 = New MetroFramework.Controls.MetroLabel()
        Me.API_TextBox = New MetroFramework.Controls.MetroTextBox()
        Me.FolderBrowser = New System.Windows.Forms.FolderBrowserDialog()
        Me.menu.SuspendLayout()
        Me.SuspendLayout()
        '
        'NotifyIcon
        '
        Me.NotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Warning
        Me.NotifyIcon.BalloonTipText = "Process Guard Windows"
        Me.NotifyIcon.ContextMenuStrip = Me.menu
        Me.NotifyIcon.Icon = CType(resources.GetObject("NotifyIcon.Icon"), System.Drawing.Icon)
        Me.NotifyIcon.Text = "Process Guard Windows"
        Me.NotifyIcon.Visible = True
        '
        'menu
        '
        Me.menu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StopServiceToolStripMenuItem, Me.StartServiceToolStripMenuItem, Me.ConfigToolStripMenuItem, Me.UninstallToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.menu.Name = "menu"
        Me.menu.Size = New System.Drawing.Size(153, 136)
        '
        'StopServiceToolStripMenuItem
        '
        Me.StopServiceToolStripMenuItem.Enabled = False
        Me.StopServiceToolStripMenuItem.Name = "StopServiceToolStripMenuItem"
        Me.StopServiceToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.StopServiceToolStripMenuItem.Text = "Stop Service"
        '
        'StartServiceToolStripMenuItem
        '
        Me.StartServiceToolStripMenuItem.Enabled = False
        Me.StartServiceToolStripMenuItem.Name = "StartServiceToolStripMenuItem"
        Me.StartServiceToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.StartServiceToolStripMenuItem.Text = "Start Service"
        '
        'ConfigToolStripMenuItem
        '
        Me.ConfigToolStripMenuItem.Enabled = False
        Me.ConfigToolStripMenuItem.Name = "ConfigToolStripMenuItem"
        Me.ConfigToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.ConfigToolStripMenuItem.Text = "Configuration"
        '
        'UninstallToolStripMenuItem
        '
        Me.UninstallToolStripMenuItem.Enabled = False
        Me.UninstallToolStripMenuItem.Name = "UninstallToolStripMenuItem"
        Me.UninstallToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.UninstallToolStripMenuItem.Text = "Uninstall"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Enabled = False
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'labelSeeds
        '
        Me.labelSeeds.AutoSize = True
        Me.labelSeeds.Location = New System.Drawing.Point(23, 72)
        Me.labelSeeds.Name = "labelSeeds"
        Me.labelSeeds.Size = New System.Drawing.Size(101, 19)
        Me.labelSeeds.TabIndex = 1
        Me.labelSeeds.Text = "Seeds Directory"
        Me.labelSeeds.Theme = MetroFramework.MetroThemeStyle.Dark
        '
        'Save_btn
        '
        Me.Save_btn.FontSize = MetroFramework.MetroButtonSize.Medium
        Me.Save_btn.Location = New System.Drawing.Point(247, 231)
        Me.Save_btn.Name = "Save_btn"
        Me.Save_btn.Size = New System.Drawing.Size(105, 27)
        Me.Save_btn.TabIndex = 2
        Me.Save_btn.Text = "Save"
        Me.Save_btn.Theme = MetroFramework.MetroThemeStyle.Dark
        Me.Save_btn.UseSelectable = True
        '
        'Seeds_TextBox
        '
        '
        '
        '
        Me.Seeds_TextBox.CustomButton.Image = Nothing
        Me.Seeds_TextBox.CustomButton.Location = New System.Drawing.Point(534, 1)
        Me.Seeds_TextBox.CustomButton.Name = ""
        Me.Seeds_TextBox.CustomButton.Size = New System.Drawing.Size(21, 21)
        Me.Seeds_TextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue
        Me.Seeds_TextBox.CustomButton.TabIndex = 1
        Me.Seeds_TextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light
        Me.Seeds_TextBox.CustomButton.UseSelectable = True
        Me.Seeds_TextBox.FontSize = MetroFramework.MetroTextBoxSize.Medium
        Me.Seeds_TextBox.Lines = New String(-1) {}
        Me.Seeds_TextBox.Location = New System.Drawing.Point(26, 94)
        Me.Seeds_TextBox.MaxLength = 32767
        Me.Seeds_TextBox.Name = "Seeds_TextBox"
        Me.Seeds_TextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(0)
        Me.Seeds_TextBox.ScrollBars = System.Windows.Forms.ScrollBars.None
        Me.Seeds_TextBox.SelectedText = ""
        Me.Seeds_TextBox.SelectionLength = 0
        Me.Seeds_TextBox.SelectionStart = 0
        Me.Seeds_TextBox.ShortcutsEnabled = True
        Me.Seeds_TextBox.ShowButton = True
        Me.Seeds_TextBox.Size = New System.Drawing.Size(556, 23)
        Me.Seeds_TextBox.TabIndex = 3
        Me.Seeds_TextBox.Theme = MetroFramework.MetroThemeStyle.Dark
        Me.Seeds_TextBox.UseSelectable = True
        Me.Seeds_TextBox.WaterMarkColor = System.Drawing.Color.FromArgb(CType(CType(109, Byte), Integer), CType(CType(109, Byte), Integer), CType(CType(109, Byte), Integer))
        Me.Seeds_TextBox.WaterMarkFont = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel)
        '
        'MetroLabel1
        '
        Me.MetroLabel1.AutoSize = True
        Me.MetroLabel1.Location = New System.Drawing.Point(26, 144)
        Me.MetroLabel1.Name = "MetroLabel1"
        Me.MetroLabel1.Size = New System.Drawing.Size(102, 19)
        Me.MetroLabel1.TabIndex = 4
        Me.MetroLabel1.Text = "API Web Server"
        Me.MetroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark
        '
        'API_TextBox
        '
        '
        '
        '
        Me.API_TextBox.CustomButton.Image = Nothing
        Me.API_TextBox.CustomButton.Location = New System.Drawing.Point(532, 1)
        Me.API_TextBox.CustomButton.Name = ""
        Me.API_TextBox.CustomButton.Size = New System.Drawing.Size(21, 21)
        Me.API_TextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue
        Me.API_TextBox.CustomButton.TabIndex = 1
        Me.API_TextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light
        Me.API_TextBox.CustomButton.UseSelectable = True
        Me.API_TextBox.FontSize = MetroFramework.MetroTextBoxSize.Medium
        Me.API_TextBox.Lines = New String(-1) {}
        Me.API_TextBox.Location = New System.Drawing.Point(28, 166)
        Me.API_TextBox.MaxLength = 32767
        Me.API_TextBox.Name = "API_TextBox"
        Me.API_TextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(0)
        Me.API_TextBox.ScrollBars = System.Windows.Forms.ScrollBars.None
        Me.API_TextBox.SelectedText = ""
        Me.API_TextBox.SelectionLength = 0
        Me.API_TextBox.SelectionStart = 0
        Me.API_TextBox.ShortcutsEnabled = True
        Me.API_TextBox.ShowButton = True
        Me.API_TextBox.Size = New System.Drawing.Size(554, 23)
        Me.API_TextBox.TabIndex = 5
        Me.API_TextBox.Theme = MetroFramework.MetroThemeStyle.Dark
        Me.API_TextBox.UseSelectable = True
        Me.API_TextBox.WaterMarkColor = System.Drawing.Color.FromArgb(CType(CType(109, Byte), Integer), CType(CType(109, Byte), Integer), CType(CType(109, Byte), Integer))
        Me.API_TextBox.WaterMarkFont = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel)
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(617, 281)
        Me.Controls.Add(Me.API_TextBox)
        Me.Controls.Add(Me.MetroLabel1)
        Me.Controls.Add(Me.Seeds_TextBox)
        Me.Controls.Add(Me.Save_btn)
        Me.Controls.Add(Me.labelSeeds)
        Me.Name = "frmMain"
        Me.ShowInTaskbar = False
        Me.Text = "Process Guard Windows Application"
        Me.Theme = MetroFramework.MetroThemeStyle.Dark
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        Me.menu.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents NotifyIcon As NotifyIcon
    Friend WithEvents menu As MetroFramework.Controls.MetroContextMenu
    Friend WithEvents StopServiceToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StartServiceToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfigToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents UninstallToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents labelSeeds As MetroFramework.Controls.MetroLabel
    Friend WithEvents Save_btn As MetroFramework.Controls.MetroButton
    Friend WithEvents Seeds_TextBox As MetroFramework.Controls.MetroTextBox
    Friend WithEvents MetroLabel1 As MetroFramework.Controls.MetroLabel
    Friend WithEvents API_TextBox As MetroFramework.Controls.MetroTextBox
    Friend WithEvents FolderBrowser As FolderBrowserDialog
End Class
