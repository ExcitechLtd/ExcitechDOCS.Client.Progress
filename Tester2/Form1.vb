Public Class Form1
    Private _thatForm As frmOther
    Private _timer As Timer
    Private _tickcount As Integer = 0

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        _thatForm = New frmOther
        _thatForm.ShowForm()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        _thatForm.ChangeText(TextBox1.Text)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        _thatForm.UpdateProgress(TextBox2.Text)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Not _timer Is Nothing Then
            _timer.Stop()
            _timer.Dispose()
        End If
        _timer = New Timer
        _timer.Interval = 10000
        AddHandler _timer.Tick, AddressOf TimerTick
        _timer.Start()
    End Sub

    Private Sub TimerTick()
        TextBox3.Text += _tickcount.ToString & " "
        _tickcount += 1
        If Not _thatForm Is Nothing Then
            _thatForm.ChangeText(_tickcount.ToString & " ")
            _thatForm.UpdateProgress(_tickcount)
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Me.BeginInvoke(CType(Sub()

                                 _thatForm = New frmOther
                                 '_thatForm.Parent = Me
                                 _thatForm.StartPosition = FormStartPosition.CenterParent
                                 _thatForm.TopMost = True
                                 _thatForm.ShowDialog()

                             End Sub, Action))
    End Sub
End Class
