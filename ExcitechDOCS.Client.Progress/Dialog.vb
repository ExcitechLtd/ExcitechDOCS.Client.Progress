Imports System.Windows.Forms
Imports MFilesAPI

Public Class Dialog


#Region " Private "
    Private _interval As Integer = 0
    Private _timer As Timer
    Private _name As String
    Private _vault As Vault
    Private _vaultName As String

    Private _frmDialog As frmSimple
    Private _handledList As UIActionHandledQueue

    Private vaultConnection As VaultConnection
#End Region

#Region " Constrcutor "
    Public Sub New()
        _timer = New Timer
        _handledList = New UIActionHandledQueue

        ''does the log folder exist?
    End Sub
#End Region

#Region " Properties "
    Public WindowHandle As Long
    Public ShellUI As Object
    Public ShellFrame As Object
#End Region

    Public Sub StartListener(timerTick As Integer, name As String, vaultName As String)
        Dim clientApp As New MFilesAPI.MFilesClientApplication
        vaultConnection = clientApp.GetVaultConnection(vaultName)
        Dim appVault As MFilesAPI.Vault = vaultConnection.BindToVault(0, True, False)

        _name = name
        _vault = appVault
        Logging.SetUpLog(_vault)

        Logging.WriteToServerLog("Client listener instance started: " & Now.ToString)
        Logging.WriteToServerLog("Vault: " & _vault.Name)
        Logging.WriteToServerLog("UserID: " & _vault.CurrentLoggedInUserID.ToString)

        initTimer()
        _timer.Interval = timerTick
        _timer.Start()

    End Sub

    Public Sub StopListener()
        If Not _timer Is Nothing Then
            _timer.Stop()
            _timer = Nothing

            _handledList = Nothing

            Logging.WriteToServerLog("Listener stopped at: " & Now.ToString)
        End If
    End Sub

    Public Sub ShowDialog(parentHWND As Int32, message As String, Vault As Vault)
        Dim frmDlg As New frmSimple
        frmDlg.Parent = Control.FromHandle(parentHWND)
        frmDlg.StartPosition = FormStartPosition.CenterParent

        If frmDlg.ShowDialog = DialogResult.OK Then
            Exit Sub
        End If
    End Sub


    Private Sub initTimer()
        Logging.WriteToServerLog("Initalising Timer")

        If Not _timer Is Nothing Then _timer.Stop()
        If _timer Is Nothing Then
            _timer = New Timer
        End If

        AddHandler _timer.Tick, AddressOf TimerTick
    End Sub

    Private Function GetActions() As UIActionQueue
        Dim ret As New UIActionQueue
        Dim str As String = _vault.ExtensionMethodOperations.ExecuteVaultExtensionMethod("ExDOCSClientUI", "GETUSERACTIONS|" & _vault.CurrentLoggedInUserID.ToString)

        If String.IsNullOrWhiteSpace(str) Then Return New UIActionQueue

        ret = Newtonsoft.Json.JsonConvert.DeserializeObject(Of UIActionQueue)(str)

        Return ret
    End Function


    Private Sub TimerTick()
        _timer.Stop()

        Dim actionQueue As UIActionQueue = GetActions()

        If actionQueue Is Nothing Then
            _timer.Start()
            Exit Sub
        End If

        Dim updatedList As New UIActionHandledQueue
        For Each _ac As UIAction In actionQueue.ActionToTake
            ''do our stuff
            Select Case _ac.ActionType
                Case "SHOWYESNODIALOG"
                    Logging.WriteToServerLog("Showing YES/NO Dialog at: " & Now.ToString)
                    updatedList = DoActionShowYesNoDialog(_ac, _handledList, ShellFrame)
                Case "SHOWSIMPLEPROGRESS"
                    Logging.WriteToServerLog("Showing Progress Dialog at: " & Now.ToString)
                    updatedList = DoActionSimpleProgress(_ac, _handledList)
            End Select
        Next

        Dim checkList1 = updatedList.HandledActions.Except(_handledList.HandledActions).ToList
        Dim checkList2 = _handledList.HandledActions.Except(updatedList.HandledActions).ToList

        If Not checkList1.Any And Not checkList2.Any Then
            ''only update the server if the lists have changed
            _handledList = updatedList
            UpdateAction()
        End If

        _timer.Start()
    End Sub

    Private Sub UpdateAction()
        Dim handledStr As String = Newtonsoft.Json.JsonConvert.SerializeObject(_handledList)
        Dim str As String = _vault.ExtensionMethodOperations.ExecuteVaultExtensionMethod("ExDOCSClientUI", "UPDATEACTION|" & handledStr)
    End Sub


#Region " Action Methods "
    Private Function DoActionShowYesNoDialog(ac As UIAction, acHandled As UIActionHandledQueue, sFRame As Object) As UIActionHandledQueue
        Dim _frmYesNo As New frmYesNoDialog
        Dim nWindow As New NativeWindow
        nWindow.AssignHandle(WindowHandle)

        _frmYesNo.StartPosition = FormStartPosition.CenterParent
        _frmYesNo.ActionToTake = ac
        _frmYesNo.HandledList = acHandled
        _frmYesNo.ShellFrame = sFRame
        _frmYesNo.Vault = _vault
        '_frmYesNo.ServerVault = _svrVault
        _frmYesNo.TopMost = True
        _frmYesNo.ShowDialog(nWindow)

        Return _frmYesNo.HandledList
    End Function

    Private Function DoActionSimpleProgress(ac As UIAction, acHandled As UIActionHandledQueue) As UIActionHandledQueue
        Dim _frmProgress As New frmProgress
        Dim nWindow As New NativeWindow
        nWindow.AssignHandle(WindowHandle)

        _frmProgress.StartPosition = FormStartPosition.CenterParent
        _frmProgress.ActionToTake = ac
        _frmProgress.Vault = _vault
        _frmProgress.HandledList = acHandled
        _frmProgress.TopMost = True
        _frmProgress.ShowDialog(nWindow)

        ''if needs refresh then somehow get the javascript to do that?

        Return _frmProgress.HandledList
    End Function
#End Region

End Class
