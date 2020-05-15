<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmProgress
    Inherits System.Windows.Forms.Form

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
        Me.pbIcon = New System.Windows.Forms.PictureBox()
        Me.lblMessage = New System.Windows.Forms.Label()
        Me.pBar = New System.Windows.Forms.ProgressBar()
        Me.btnDone = New System.Windows.Forms.Button()
        CType(Me.pbIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pbIcon
        '
        Me.pbIcon.Image = Global.ExcitechDOCS.Client.UI.My.Resources.Resources.information
        Me.pbIcon.Location = New System.Drawing.Point(22, 21)
        Me.pbIcon.Name = "pbIcon"
        Me.pbIcon.Size = New System.Drawing.Size(100, 100)
        Me.pbIcon.TabIndex = 5
        Me.pbIcon.TabStop = False
        '
        'lblMessage
        '
        Me.lblMessage.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMessage.Location = New System.Drawing.Point(128, 21)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(399, 100)
        Me.lblMessage.TabIndex = 4
        Me.lblMessage.Text = "Label1"
        '
        'pBar
        '
        Me.pBar.Location = New System.Drawing.Point(22, 131)
        Me.pBar.Name = "pBar"
        Me.pBar.Size = New System.Drawing.Size(505, 23)
        Me.pBar.TabIndex = 6
        '
        'btnDone
        '
        Me.btnDone.Enabled = False
        Me.btnDone.Location = New System.Drawing.Point(232, 165)
        Me.btnDone.Name = "btnDone"
        Me.btnDone.Size = New System.Drawing.Size(84, 23)
        Me.btnDone.TabIndex = 7
        Me.btnDone.Text = "Please wait..."
        Me.btnDone.UseVisualStyleBackColor = True
        '
        'frmProgress
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(549, 196)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnDone)
        Me.Controls.Add(Me.pBar)
        Me.Controls.Add(Me.pbIcon)
        Me.Controls.Add(Me.lblMessage)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmProgress"
        Me.Text = "frmProgress"
        CType(Me.pbIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pbIcon As Windows.Forms.PictureBox
    Friend WithEvents lblMessage As Windows.Forms.Label
    Friend WithEvents pBar As Windows.Forms.ProgressBar
    Friend WithEvents btnDone As Windows.Forms.Button
End Class
