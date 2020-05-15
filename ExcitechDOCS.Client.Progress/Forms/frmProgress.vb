Imports System.ComponentModel
Imports MFilesAPI

Public Class frmProgress

#Region " Private "
    Private _closeTimer As Windows.Forms.Timer
    Private _guiTimer As Windows.Forms.Timer
    Private _guiActions As UIActionQueue
#End Region

#Region " Properties "
    Public Property HandledList As UIActionHandledQueue
    Public Property ActionToTake As UIAction
    Public Property Vault As Vault
#End Region

    Private Sub frmProgress_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        btnDone.Enabled = False
        btnDone.Text = "Please wait..."
        initOptions()
    End Sub

    Private Sub initOptions()
        Dim val As String = ""
        If Not ActionToTake.Options.TryGetValue("title", val) Then val = "ExcitechDOCS"
        Text = val

        If Not ActionToTake.Options.TryGetValue("message", val) Then val = "Information from ExcitechDOCS"
        lblMessage.Text = val

        If Not ActionToTake.Options.TryGetValue("icon", val) Then val = "information"

        Select Case val.ToUpperInvariant
            Case "INFORMATION" : pbIcon.Image = My.Resources.information
            Case "WARNING" : pbIcon.Image = My.Resources.warning
            Case "QUESTION" : pbIcon.Image = My.Resources.question
            Case "TICK" : pbIcon.Image = My.Resources.check
        End Select

        If ActionToTake.Options.TryGetValue("timeout", val) Then
            ''setup a timer to close the dialog
            If Not _closeTimer Is Nothing Then
                _closeTimer.Stop()
                _closeTimer.Dispose()
            End If

            If IsNumeric(val) AndAlso CInt(val) > 0 Then
                _closeTimer = New Windows.Forms.Timer
                _closeTimer.Interval = TimeSpan.FromSeconds(val).TotalMilliseconds
                AddHandler _closeTimer.Tick, AddressOf TimeoutTick
                _closeTimer.Start()
            End If

        End If
    End Sub

    Private Sub TimeoutTick()
        _guiTimer.Stop()
        _closeTimer.Stop()
        ActionToTake.Options.Add("RESULT", "TIMEOUT")
        DialogResult = Windows.Forms.DialogResult.OK
        Close()
    End Sub

    Private Sub frmProgress_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ''start our gui timer
        If Not _guiTimer Is Nothing Then
            _guiTimer.Stop()
            _guiTimer.Dispose()
        End If

        _guiTimer = New Windows.Forms.Timer
        _guiTimer.Interval = TimeSpan.FromSeconds(2.5).TotalMilliseconds
        AddHandler _guiTimer.Tick, AddressOf GuiTimerTick

        ''flag that we have shown this dialog as handled
        ActionToTake.Handled = True

        If HandledList.HandledActions.FindIndex(Function(hac) hac.Key = ActionToTake.Key) = -1 Then
            HandledList.HandledActions.Add(ActionToTake)
            'UIActionHandledQueue.saveSettings(Vault, HandledList)
            UpdateAction()
        End If

        _guiTimer.Start()
    End Sub

    Private Sub GuiTimerTick()
        _guiTimer.Stop()

        ''have we any updates to process
        '_guiActions = UIActionQueue.readSettings(Vault)
        '_guiActions.CleanActionList(HandledList.HandledActions)
        _guiActions = GetActions()

        For Each ac As UIAction In _guiActions.ActionToTake
            Select Case ac.ActionType
                Case "UPDATEMESSAGE"
                    Dim val As String = ""
                    If Not ac.Options.TryGetValue("VALUE", val) Then val = ""

                    ChangeText(val)
                Case "UPDATEPROGRESS"
                    Dim val As String = ""
                    Dim intVal As Integer = 0
                    If Not ac.Options.TryGetValue("VALUE", val) Then val = ""

                    If Not IsNumeric(val) Then intVal = 0 Else intVal = CInt(val)

                    UpdateProgress(intVal)
                Case "DONE"
                    Dim val As String = ""
                    If Not ac.Options.TryGetValue("VALUE", val) Then val = "Finished"

                    Updatedone(val)
            End Select

            ac.Handled = True

            If HandledList.HandledActions.FindIndex(Function(hac) hac.Key = ac.Key) = -1 Then
                HandledList.HandledActions.Add(ac)
                'UIActionHandledQueue.saveSettings(Vault, HandledList)
                UpdateAction()
            End If
        Next

        Windows.Forms.Application.DoEvents()

        _guiTimer.Start()
    End Sub

    Private Sub UpdateAction()
        Dim handledStr As String = Newtonsoft.Json.JsonConvert.SerializeObject(_HandledList)

        Dim str As String = _Vault.ExtensionMethodOperations.ExecuteVaultExtensionMethod("ExDOCSClientUI", "UPDATEACTION|" & handledStr)


    End Sub

    Private Function GetActions() As UIActionQueue
        Dim ret As New UIActionQueue
        Dim str As String = _Vault.ExtensionMethodOperations.ExecuteVaultExtensionMethod("ExDOCSClientUI", "GETUSERACTIONS|" & _Vault.CurrentLoggedInUserID.ToString)

        If String.IsNullOrWhiteSpace(str) Then Return New UIActionQueue

        ret = Newtonsoft.Json.JsonConvert.DeserializeObject(Of UIActionQueue)(str)

        Return ret
    End Function

#Region " Update Methods "
    Public Sub ChangeText(Value As String)
        If Not lblMessage.InvokeRequired Then
            lblMessage.Text = Value
            lblMessage.Refresh()
        Else
            BeginInvoke(CType(Sub()
                                  lblMessage.Text = Value
                                  lblMessage.Refresh()
                              End Sub, Action))
        End If
    End Sub

    Public Sub UpdateProgress(value As Integer)
        If Not pBar.InvokeRequired Then
            If value > pBar.Maximum Then
                pBar.Value = pBar.Maximum
                Exit Sub
            End If
            If value < pBar.Minimum Then
                pBar.Value = pBar.Minimum
                Exit Sub
            End If

            pBar.Value = value
            pBar.Update()
            pBar.Refresh()
        Else
            BeginInvoke(CType(Sub()
                                  If value > pBar.Maximum Then
                                      pBar.Value = pBar.Maximum
                                      Exit Sub
                                  End If
                                  If value < pBar.Minimum Then
                                      pBar.Value = pBar.Minimum
                                      Exit Sub
                                  End If

                                  pBar.Value = value
                                  pBar.Update()
                                  pBar.Refresh()
                              End Sub, Action))
        End If
    End Sub

    Public Sub Updatedone(value As String)
        If Not btnDone.InvokeRequired Then
            btnDone.Text = value
            btnDone.Enabled = True
            btnDone.Refresh()
        Else
            BeginInvoke(CType(Sub()
                                  btnDone.Text = value
                                  btnDone.Enabled = True
                                  btnDone.Refresh()
                              End Sub, Action))
        End If
    End Sub

    Private Sub frmProgress_DoubleClick(sender As Object, e As EventArgs) Handles Me.DoubleClick
        If (ModifierKeys And Windows.Forms.Keys.Control) = Windows.Forms.Keys.Control Then
            ControlBox = True
            btnDone.Enabled = True
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        pBar.Value += 1
    End Sub

    Private Sub btnDone_Click(sender As Object, e As EventArgs) Handles btnDone.Click
        If Not _guiTimer Is Nothing Then _guiTimer.Stop()
        If Not _closeTimer Is Nothing Then _closeTimer.Stop()
        ActionToTake.Options.Add("RESULT", "TIMEOUT")
        DialogResult = Windows.Forms.DialogResult.OK
        Close()
    End Sub
#End Region
End Class