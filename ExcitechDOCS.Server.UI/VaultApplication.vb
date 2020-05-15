Imports ExcitechDOCS.Client.UI
Imports MFiles.VAF
Imports MFiles.VAF.Common
Imports MFiles.VAF.Configuration
Imports MFilesAPI

Public Class VaultApplication
    Inherits VaultApplicationBase

#Region " Private "
    Private _sessionID As String
    Private _hasSession As Boolean
    Private _bgUpdate As BackgroundOperation
#End Region

#Region " Constructor "
    Public Sub New()
        ''clear existing actions
        _hasSession = False
        _sessionID = ""
    End Sub
#End Region

#Region " Init "
    Public Overrides Sub StartOperations(vaultPersistent As Vault)

        MyBase.StartOperations(vaultPersistent)
    End Sub

    Public Overrides Sub Install(vault As Vault)
        ''create any defautl directories we need
        Dim _basePath As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        _basePath = IO.Path.Combine(_basePath, "Excitech DOCS\")

        ''server log directory
        Dim _path As String = ""
        Try
            Dim _vaultName As String = vault.Name + " " + vault.GetGUID
            Logging.ServerLogBasePath = IO.Path.Combine(IO.Path.Combine(_basePath, "Server Logs\"), _vaultName)
            If Not IO.Directory.Exists(Logging.ServerLogBasePath) Then IO.Directory.CreateDirectory(Logging.ServerLogBasePath)
        Catch ex As Exception

        End Try

        ''script logs
        Try
            If Not IO.Directory.Exists(Logging.ScriptLogBasePath) Then IO.Directory.CreateDirectory(Logging.ScriptLogBasePath)
        Catch ex As Exception

        End Try

        ''database
        Try
            Dim _vaultName As String = vault.Name + " " + vault.GetGUID
            _path = IO.Path.Combine(_basePath, "Client.UI\Database\" & _vaultName)

            If Not IO.Directory.Exists(_path) Then IO.Directory.CreateDirectory(_path)
            IO.File.Delete(IO.Path.Combine(_path, "ClientUI.db"))

            Dim dbPath As String = IO.Path.Combine(_path, "ClientUI.db")
            Dim dbCNStr As String = "Data source='" & dbPath & "';Version=3"

            ''create the db tables
            Using _cn As IDbConnection = New SQLite.SQLiteConnection(dbCNStr)
                _cn.Open()

                Dim _sql As String = "SELECT 1 FROM sqlite_master WHERE type='table' AND name = 'ClientUIActions'"
                Dim _cmd As IDbCommand = New SQLite.SQLiteCommand(_sql, _cn)
                Dim _value As Integer = _cmd.ExecuteScalar

                If Not _value = 1 Then
                    ''create tables
                    _sql = "CREATE TABLE 'ClientUIActions' ('ID' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  'UserID' INTEGER,'Key' TEXT,'ActionType' TEXT,	'Options' TEXT,	'Handled' INTEGER,'Result' TEXT);"
                    _cmd.CommandText = _sql
                    _cmd.ExecuteNonQuery()
                End If

            End Using

        Catch ex As Exception

        End Try

        MyBase.Install(vault)
    End Sub

    Protected Overrides Sub StartApplication()
        MyBase.StartApplication()

        Dim _checkStatus As Boolean = False
        _checkStatus = True        ''ignore the checking at this point

        If _checkStatus Then
            ''start the log clean up task
            Dim _logTask As BackgroundOperation = Me.BackgroundOperations.CreateBackgroundOperation("DeleteLogFiles", AddressOf Logging.DeleteLogFiles)

        End If




        ''create a background operation
        _bgUpdate = Me.BackgroundOperations.CreateBackgroundOperation("UpdateFlibble", AddressOf bgUpdate)
    End Sub
#End Region

#Region " Methods "
    Public Sub WriteLog(LogIdentifier As Object, message As Object)

        If LogIdentifier Is Nothing Then Exit Sub
        If String.IsNullOrWhiteSpace(LogIdentifier.ToString) Then Exit Sub

        Dim _logID As String

        'get identifier as string
        Select Case True
            Case TypeOf LogIdentifier Is TypedValue
                _logID = LogIdentifier.DisplayValue
            Case TypeOf LogIdentifier Is String
                _logID = LogIdentifier
            Case Else
                Throw New Exception("Identifier argument is an invalid type.")
        End Select

        If String.IsNullOrWhiteSpace(_logID) Then _logID = LogIdentifier
        If String.IsNullOrWhiteSpace(_logID) Then
            Dim _message As String = Now.ToString("MM/dd/yyyy HH:mm:ss") + " Attempt to call 'WriteLog' failed unable to set Logidentifier from Object"
            Logging.WriteToServerLog(_message)

            Exit Sub
        End If

        Dim _logFile As String = Logging.GetScriptLogFilename(_logID)
        Logging.WriteToLog(message, _logFile)
    End Sub

    Public Sub initSession(Vault As Vault, ActivityID As String, userID As String)
        _sessionID = ActivityID
        UIActionQueue.ClearSettings(Vault, userID)
        UIActionHandledQueue.ClearSettings(Vault, userID)
    End Sub

    ''new action
    Public Function NewAction() As UIAction
        Return New UIAction
    End Function

    Public Function AddActionBG(Vault As Vault, uiAction As UIAction, userID As String) As String
        If String.IsNullOrWhiteSpace(_sessionID) Then Throw New Exception("Client.UI session must be initalised before any actions can take place")

        ''read the actions we have in the dataabse
        Dim _acQueue As UIActionQueue = UIActionQueue.readSettings(Vault, userID)
        Dim _handled As UIActionHandledQueue = UIActionHandledQueue.readSettings(Vault, userID)

        ''clear the _action queue of anything that is in the handled queue
        _acQueue.CleanActionList(_handled.HandledActions)
        _handled.HandledActions.Clear()
        _acQueue.ActionToTake.Add(uiAction)

        Dim str As String = Newtonsoft.Json.JsonConvert.SerializeObject(_acQueue)
        MFUtils.SetTransactionVariable(PermanentVault, MFNamedValueType.MFConfigurationValue, "ExcitechDOCS.Client.UI.Settings", "USER:ACTIONS", str)

        '_bgUpdate.RunOnce()

        Return uiAction.Key.ToString
    End Function

    Private Sub bgUpdate()
        Using _sr As New IO.StreamWriter("c:\temp\bgtask.txt")
            ''pull the transaction variable
            Dim value As String = ""
            MFUtils.TryGetTransactionVariable(PermanentVault, MFNamedValueType.MFConfigurationValue, "ExcitechDOCS.Client.UI.Settings", "USER:ACTIONS", value)
            MFUtils.SetTransactionVariable(PermanentVault, MFNamedValueType.MFConfigurationValue, "ExcitechDOCS.Client.UI.Settings", "USER:ACTIONS:61", value)
            _sr.WriteLine(value)


        End Using
    End Sub


    Public Function GetUserActions(vault As Vault, userID As String) As String
        Dim _vaultName As String = vault.Name + " " + vault.GetGUID
        Dim dbPath As String = "C:\ProgramData\Excitech Docs\Client.UI\Database\" & _vaultName & "\ClientUI.db"
        Dim dbCNStr As String = "Data source='" & dbPath & "';Version=3"
        Dim sqlStr As String = "SELECT * FROM ClientUIActions WHERE UserID=_userID AND handled=0 ORDER BY ID LIMIT 1"
        ''get all the unhandled actions for this user in the order they where added

        sqlStr = sqlStr.Replace("_userID", userID)


        Dim userActions As New UIActionQueue

        Using cn As IDbConnection = New SQLite.SQLiteConnection(dbCNStr)
            cn.Open()

            Using cmd As IDbCommand = New SQLite.SQLiteCommand(sqlStr, cn)

                Using dr As IDataReader = cmd.ExecuteReader
                    While dr.Read
                        Dim ac As New UIAction
                        ac.ActionType = dr("ActionType").ToString.ToUpperInvariant
                        ac.Handled = False
                        ac.Key = New Guid(dr("key").ToString)
                        ac.Options = Newtonsoft.Json.JsonConvert.DeserializeObject(Of UIActionOptions)(dr("Options").ToString)

                        userActions.ActionToTake.Add(ac)
                    End While

                End Using

            End Using
        End Using

        Dim ret As String = Newtonsoft.Json.JsonConvert.SerializeObject(userActions)
        Return ret
    End Function

    Public Function UpdateUserAction(vault As Vault, userId As String, uiActionStr As String) As String
        Dim _vaultName As String = vault.Name + " " + vault.GetGUID
        Dim dbPath As String = "C:\ProgramData\Excitech Docs\Client.UI\Database\" & _vaultName & "\ClientUI.db"
        Dim dbCNStr As String = "Data source='" & dbPath & "';Version=3"
        Dim sqlStr As String = "UPDATE ClientUIActions " &
            "SET Options='_options',Handled=_handled, Result='_result' " &
            "WHERE key='_key'"

        ''desearlize the action
        Dim uiActionQueue As UIActionHandledQueue = Newtonsoft.Json.JsonConvert.DeserializeObject(Of UIActionHandledQueue)(uiActionStr)

        Using cn As IDbConnection = New SQLite.SQLiteConnection(dbCNStr)
            cn.Open()
            Using cmd As IDbCommand = New SQLite.SQLiteCommand("", cn)

                For Each _uiAction In uiActionQueue.HandledActions
                    Dim opStr As String = Newtonsoft.Json.JsonConvert.SerializeObject(_uiAction.Options)
                    Dim updateSTR As String = sqlStr.Replace("_options", "")
                    updateSTR = updateSTR.Replace("_handled", IIf(_uiAction.Handled, 1, 0))
                    updateSTR = updateSTR.Replace("_key", _uiAction.Key.ToString)

                    Dim resValue As String = ""
                    _uiAction.Options.TryGetValue("RESULT", resValue)

                    updateSTR = updateSTR.Replace("_result", resValue)

                    cmd.CommandText = updateSTR
                    cmd.ExecuteNonQuery()
                Next

            End Using
        End Using

        Return "ok"


    End Function

    Public Function AddActionDB(vault As Vault, uiAction As UIAction, userID As String) As String
        Dim _vaultName As String = vault.Name + " " + vault.GetGUID
        Dim dbPath As String = "C:\ProgramData\Excitech Docs\Client.UI\Database\" & _vaultName & "\ClientUI.db"
        Dim dbCNStr As String = "Data source='" & dbPath & "';Version=3"
        Dim sqlStr As String = "INSERT INTO 'ClientUIActions' (UserID,Key,ActionType,Options,Handled) Values (_userid,'_key','_actiontype','_options',_handled)"

        Dim ucOptions As New UIActionOptions
        For Each Key As String In uiAction.Options.Keys
            ucOptions.Add(Key.ToUpperInvariant, uiAction.Options(Key))
        Next

        Dim opStr As String = Newtonsoft.Json.JsonConvert.SerializeObject(ucOptions)

        sqlStr = sqlStr.Replace("_userid", userID)
        sqlStr = sqlStr.Replace("_key", uiAction.Key.ToString)
        sqlStr = sqlStr.Replace("_actiontype", uiAction.ActionType)
        sqlStr = sqlStr.Replace("_options", opStr)
        sqlStr = sqlStr.Replace("_handled", 0)

        Using cn As IDbConnection = New SQLite.SQLiteConnection(dbCNStr)
            cn.Open()

            Using cmd As IDbCommand = New SQLite.SQLiteCommand(sqlStr, cn)
                cmd.ExecuteNonQuery()
            End Using
        End Using

        Return uiAction.Key.ToString
    End Function

    Public Function WaitForActionResponseDB(Vault As Vault, userID As Integer, ActionKey As String, timeout As Integer) As String
        If timeout <= 0 Then timeout = 60 ''seconds
        Dim key As New Guid(ActionKey)
        Dim res As Boolean = False
        Dim hasTimedout As Boolean = False
        Dim result As String = ""
        Dim tOut As DateTime = Now.AddSeconds(60)

        Dim _vaultName As String = Vault.Name + " " + Vault.GetGUID
        Dim dbPath As String = "C:\ProgramData\Excitech Docs\Client.UI\Database\" & _vaultName & "\ClientUI.db"
        Dim dbCNStr As String = "Data source='" & dbPath & "';Version=3"
        Dim sqlStr As String = "SELECT 1 FROM ClientUIActions where UserID=_userId AND key='_key' AND handled=1"
        sqlStr = sqlStr.Replace("_userId", userID)
        sqlStr = sqlStr.Replace("_key", ActionKey)

        Using cn As IDbConnection = New SQLite.SQLiteConnection(dbCNStr)
            cn.Open()

            Using cmd As IDbCommand = New SQLite.SQLiteCommand(sqlStr, cn)
                ''get the handled actions

                Dim str As String = ""

                    While Not res OrElse Not hasTimedout
                        str = cmd.ExecuteScalar

                    If str = 1 Then
                        cmd.CommandText = "SELECT Result FROM 'ClientUIActions' where UserID=_userId AND key='_key' AND handled=1"
                        cmd.CommandText = cmd.CommandText.Replace("_userId", userID)
                        cmd.CommandText = cmd.CommandText.Replace("_key", ActionKey)

                        result = cmd.ExecuteScalar
                        res = True


                        Exit While
                    End If

                    ''have we hit our timeout?
                    If Now >= tOut Then
                            hasTimedout = Now >= tOut
                            result = Nothing ''return nothing if we have timedout
                            Exit While ''jump out of the while
                        End If

                        Threading.Thread.Sleep(600)
                        cmd.CommandText = sqlStr
                        str = cmd.ExecuteScalar()
                    End While

            End Using

        End Using

        Return result
    End Function

    Public Sub Sleep(interval As Integer)
        Threading.Thread.Sleep(interval)
    End Sub

    Public Function AddActionPV(Vault As Vault, uiAction As UIAction, userID As Integer) As String
        If String.IsNullOrWhiteSpace(_sessionID) Then Throw New Exception("Client.UI session must be initalised before any actions can take place")

        ''read the actions we have in the dataabse
        Dim _acQueue As UIActionQueue = UIActionQueue.readSettings(PermanentVault, userID)
        Dim _handled As UIActionHandledQueue = UIActionHandledQueue.readSettings(PermanentVault, userID)

        ''clear the _action queue of anything that is in the handled queue
        _acQueue.CleanActionList(_handled.HandledActions)
        _handled.HandledActions.Clear()

        'Dim clientApp As New MFilesClientApplication
        ''Dim svA As New MFilesAPI.MFilesServerApplication
        'Dim vaultConnection As MFilesAPI.VaultConnection = clientApp.GetVaultConnection("VM DOCS")
        ''svA.Connect(MFAuthType.MFAuthTypeSpecificMFilesUser, "Boris", "Excitech1", Nothing, vaultConnection.ProtocolSequence, vaultConnection.NetworkAddress, vaultConnection.Endpoint)
        ''Dim svVault As Vault = svA.LogInToVault(vaultConnection.GetGUID)
        'Dim svVault As Vault = vaultConnection.BindToVault(0, True, False)

        Using _sr As New IO.StreamWriter("C:\temp\addpv.txt")
            _acQueue.ActionToTake.Add(uiAction)

            Dim str As String = Newtonsoft.Json.JsonConvert.SerializeObject(_acQueue)
            _sr.WriteLine("Adding Actions: " + str)
            MFUtils.SetTransactionVariable(PermanentVault, MFNamedValueType.MFConfigurationValue, "ExcitechDOCS.Client.UI.Settings", "USER:ACTIONS:" + userID.ToString, str)
            _sr.WriteLine("Action has been added")

            'UIActionQueue.saveSettings(PermanentVault, _acQueue, userID)
            'UIActionHandledQueue.saveSettings(PermanentVault, _handled, userID)
            str = ""
            _sr.WriteLine("Reading back action")
            MFUtils.TryGetTransactionVariable(PermanentVault, MFNamedValueType.MFConfigurationValue, "ExcitechDOCS.Client.UI.Settings", "USER:ACTIONS:" + userID.ToString, str)
            _sr.WriteLine("Valusaved value is: " & str)

        End Using




        'UIActionQueue.saveSettings(svVault, _acQueue, userID)
        'UIActionHandledQueue.saveSettings(svVault, _handled, userID)


        Return uiAction.Key.ToString
    End Function

    ''add a new action
    Public Function AddAction(Vault As Vault, uiAction As UIAction, userID As Integer) As String
        If String.IsNullOrWhiteSpace(_sessionID) Then Throw New Exception("Client.UI session must be initalised before any actions can take place")

        ''read the actions we have in the dataabse
        Dim _acQueue As UIActionQueue = UIActionQueue.readSettings(Vault, userID)
        Dim _handled As UIActionHandledQueue = UIActionHandledQueue.readSettings(Vault, userID)

        ''clear the _action queue of anything that is in the handled queue
        _acQueue.CleanActionList(_handled.HandledActions)
        _handled.HandledActions.Clear()

        Dim clientApp As New MFilesClientApplication
        'Dim svA As New MFilesAPI.MFilesServerApplication
        Dim vaultConnection As MFilesAPI.VaultConnection = clientApp.GetVaultConnection("VM DOCS")
        'svA.Connect(MFAuthType.MFAuthTypeSpecificMFilesUser, "Boris", "Excitech1", Nothing, vaultConnection.ProtocolSequence, vaultConnection.NetworkAddress, vaultConnection.Endpoint)
        'Dim svVault As Vault = svA.LogInToVault(vaultConnection.GetGUID)
        Dim svVault As Vault = vaultConnection.BindToVault(0, True, False)

        _acQueue.ActionToTake.Add(uiAction)
        UIActionQueue.saveSettings(Vault, _acQueue, userID)
        UIActionHandledQueue.saveSettings(Vault, _handled, userID)

        UIActionQueue.saveSettings(svVault, _acQueue, userID)
        UIActionHandledQueue.saveSettings(svVault, _handled, userID)


        Return uiAction.Key.ToString
    End Function

    Public Function WaitForActionResponsePV(Vault As Vault, userID As Integer, ActionKey As String, timeout As Integer) As String
        If timeout <= 0 Then timeout = 60 ''seconds
        Dim key As New Guid(ActionKey)
        Dim res As Boolean = False
        Dim hasTimedout As Boolean = False
        Dim result As String = ""
        Dim tOut As DateTime = Now.AddSeconds(60)


        ''get the handled actions
        Using _sr As New IO.StreamWriter("c:\temp\waitPV.txt")
            Dim str As String = ""
            _sr.WriteLine("Reading actions..") : _sr.Flush()
            MFUtils.TryGetTransactionVariable(PermanentVault, MFNamedValueType.MFConfigurationValue, "ExcitechDOCS.Client.UI.Settings", "USER:ACTIONS:" + userID.ToString, str)
            _sr.WriteLine("Storedvalue is " & str) : _sr.Flush()

            Dim _handled As UIActionHandledQueue = UIActionHandledQueue.readSettings(PermanentVault, userID)

            While Not res OrElse Not hasTimedout
                Dim index As Integer = _handled.HandledActions.FindIndex(Function(ac) ac.Key = key)

                If Not index = -1 Then ''we have a response from the client
                    Dim ac As UIAction = _handled.HandledActions(index)
                    If Not ac.Options.TryGetValue("RESULT", result) Then
                        result = ""
                    End If
                    res = True
                    Exit While ''if we have a resutl jump out of the while
                End If

                ''have we hit our timeout?
                If Now >= tOut Then
                    hasTimedout = Now >= tOut
                    result = Nothing ''return nothing if we have timedout
                    Exit While ''jump out of the while
                End If

                Threading.Thread.Sleep(600)
                _handled = UIActionHandledQueue.readSettings(PermanentVault, userID)
            End While
        End Using



        Return result
    End Function

    Public Function WaitForActionResponse(Vault As Vault, userID As Integer, ActionKey As String, timeout As Integer) As String
        If timeout <= 0 Then timeout = 60 ''seconds
        Dim key As New Guid(ActionKey)
        Dim res As Boolean = False
        Dim hasTimedout As Boolean = False
        Dim result As String = ""
        Dim tOut As DateTime = Now.AddSeconds(60)

        Dim clientApp As New MFilesClientApplication
        'Dim svA As New MFilesAPI.MFilesServerApplication
        Dim vaultConnection As MFilesAPI.VaultConnection = clientApp.GetVaultConnection("VM DOCS")
        'svA.Connect(MFAuthType.MFAuthTypeSpecificMFilesUser, "Boris", "Excitech1", Nothing, vaultConnection.ProtocolSequence, vaultConnection.NetworkAddress, vaultConnection.Endpoint)
        'Dim svVault As Vault = svA.LogInToVault(vaultConnection.GetGUID)
        Dim svVault As Vault = vaultConnection.BindToVault(0, True, False)

        ''get the handled actions
        Dim _handled As UIActionHandledQueue = UIActionHandledQueue.readSettings(svVault, userID)
        _handled.HandledActions.AddRange(UIActionHandledQueue.readSettings(Vault, userID).HandledActions)
        While Not res OrElse Not hasTimedout
            Dim index As Integer = _handled.HandledActions.FindIndex(Function(ac) ac.Key = key)

            If Not index = -1 Then ''we have a response from the client
                Dim ac As UIAction = _handled.HandledActions(index)
                If Not ac.Options.TryGetValue("RESULT", result) Then
                    result = ""
                End If
                res = True
                Exit While ''if we have a resutl jump out of the while
            End If

            ''have we hit our timeout?
            If Now >= tOut Then
                hasTimedout = Now >= tOut
                result = Nothing ''return nothing if we have timedout
                Exit While ''jump out of the while
            End If

            Threading.Thread.Sleep(600)
            _handled = UIActionHandledQueue.readSettings(svVault, userID)
            _handled.HandledActions.AddRange(UIActionHandledQueue.readSettings(Vault, userID).HandledActions)
        End While

        Return result
    End Function
#End Region

End Class
