Public Class frmPBar
    Dim pc As Integer = 10
    Public Sub updatepBar(value As Integer)
        If ProgressBar1.InvokeRequired Then
            BeginInvoke(CType(Sub() ProgressBar1.Value = value, Action))
            Exit Sub
        End If

        ProgressBar1.Value = value
    End Sub

    Public Sub up(value As Integer)
        RaiseEvent UpdatePBarEvent(value)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        pc += 1
        updatepBar(pc)
    End Sub

    Public Event UpdatePBarEvent(value As Integer)

    Public Sub dp(value As String) Handles Me.UpdatePBarEvent
        ProgressBar1.Value = value
        ProgressBar1.Update()
        ProgressBar1.Refresh()
        ProgressBar1.Invalidate()

        Application.DoEvents()
    End Sub

    Private Sub frmPBar_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub
End Class