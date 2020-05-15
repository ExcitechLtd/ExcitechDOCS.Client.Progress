Imports System.ComponentModel
Imports MFilesAPI

Public Class frmYesNoDialog

#Region " Private "
    Private _closeTimer As Windows.Forms.Timer
#End Region

#Region " Properties "
    Public Property HandledList As UIActionHandledQueue
    Public Property ActionToTake As UIAction
    Public Property Vault As Vault
    Public Property ServerVault As Vault
    Public Property ShellFRame As Object ''urgh types as objects
#End Region

    Private Sub frmYesNoDialog_Load(sender As Object, e As EventArgs) Handles Me.Load
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

    Private Sub btnYes_Click(sender As Object, e As EventArgs) Handles btnYes.Click
        Dim value As String = ""
        If ActionToTake.Options.TryGetValue("BUTTON:YES", value) Then DialogAction(value)

        ''set the uiaction result as YES then close the dialog
        ActionToTake.Handled = True
        ActionToTake.Options.Add("RESULT", "YES")
        HandledList.HandledActions.Add(ActionToTake)
        Close()
    End Sub

    Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        Dim value As String = ""
        If ActionToTake.Options.TryGetValue("BUTTON:NO", value) Then DialogAction(value)

        ActionToTake.Handled = True
        ActionToTake.Options.Add("RESULT", "NO")
        HandledList.HandledActions.Add(ActionToTake)
        Close()
    End Sub

    Private Sub TimeoutTick()
        Dim value As String = ""
        If ActionToTake.Options.TryGetValue("BUTTON:TIMEOUT", value) Then DialogAction(value)

        _closeTimer.Stop()
        ActionToTake.Handled = True
        ActionToTake.Options.Add("RESULT", "TIMEOUT")
        HandledList.HandledActions.Add(ActionToTake)
        Close()
    End Sub

    Private Sub DialogAction(actionName As String)
        Select Case actionName.ToUpperInvariant
            Case "REFRESHLISTING:SMART"
                ShellFRame.ActiveListing.RefreshListing(True, True, False)
            Case "REFRESHLISTING:F5"
                ShellFRame.ActiveListing.RefreshListing(False, True, False)
            Case "REFRESHLISTING:SHIFTF5"
                ShellFRame.ActiveListing.RefreshListing(False, True, True)
        End Select
    End Sub

End Class