Imports MFilesAPI

Public Class frmSimple

#Region " Properties "
    Public Property Vault As Vault
    Public Property Action As UIAction
#End Region

#Region " Events "
    Private Sub frmSimple_Load(sender As Object, e As EventArgs) Handles Me.Load
        Label2.Text = Vault.Name
    End Sub
#End Region

#Region " Methods "

#End Region

#Region " Buttons "
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Close()
        Me.DialogResult = Windows.Forms.DialogResult.OK

    End Sub

#End Region

#Region " Invoked Methods "
    Public Sub CloseForm()
        BeginInvoke(CType(Sub() Me.Close(), Action))
    End Sub

    Public Sub ChangeText(TextValue As String)
        BeginInvoke(CType(Sub() TextBox1.Text = TextValue, Action))
    End Sub

    Public Sub UpdateProgress(value As Integer)
        BeginInvoke(CType(Sub() ProgressBar1.Value = value, Action))
    End Sub

    Public Sub ShowForm()
        Me.TopMost = True

        Task.Run(Sub() Me.ShowDialog())
    End Sub

#End Region



End Class