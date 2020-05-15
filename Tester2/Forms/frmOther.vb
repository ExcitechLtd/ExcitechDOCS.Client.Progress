Public Class frmOther

#Region " Invoked form Methods "

    Public Sub ShowForm()
        Me.TopMost = True
        Task.Run(Sub() Me.ShowDialog())
    End Sub


    Public Sub CloseForm()
        BeginInvoke(CType(Sub() Me.Close(), Action))
    End Sub

    Public Sub ChangeText(TextValue As String)
        BeginInvoke(CType(Sub() TextBox1.Text = TextValue, Action))
    End Sub

    Public Sub UpdateProgress(value As Integer)
        BeginInvoke(CType(Sub() ProgressBar1.Value = value, Action))
    End Sub

#End Region

End Class