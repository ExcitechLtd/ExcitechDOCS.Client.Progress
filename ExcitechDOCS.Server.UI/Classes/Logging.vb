Imports MFilesAPI

Public Class Logging

#Region " Private "
    Private Shared _lock As New Object
    Private Shared _ScriptLogBasePath As String
    Private Shared _ServerLogBasePath As String
#End Region

#Region " Public "
    Public Shared Property DeleteInterval As Integer = 0 ''disabled

    Public Shared Property ScriptLogBasePath As String
        Get
            If String.IsNullOrWhiteSpace(_ScriptLogBasePath) Then
                _ScriptLogBasePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                _ScriptLogBasePath = IO.Path.Combine(_ScriptLogBasePath, "Excitech DOCS\UI Processor\Logs\")
            End If

            Return _ScriptLogBasePath
        End Get
        Set(value As String)
            _ScriptLogBasePath = value
        End Set
    End Property
    Public Shared Property ServerLogBasePath As String
        Get
            If String.IsNullOrWhiteSpace(_ServerLogBasePath) Then
                _ServerLogBasePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                _ServerLogBasePath = IO.Path.Combine(_ServerLogBasePath, "Excitech DOCS\Server Logs\")
            End If

            Return _ServerLogBasePath
        End Get
        Set(value As String)
            _ServerLogBasePath = value
        End Set
    End Property
    Public Shared Property ServerLogFile As String
#End Region

#Region " Shared "
    Public Shared Function SetUpLog(Vault As Vault) As Boolean
        If ScriptLogBasePath = "" Then

        End If


        If String.IsNullOrWhiteSpace(_ScriptLogBasePath) Then
            Dim _message As String = "ERROR LogPath setting is blank or invalid"
            EventLog.WriteEntry("Excitech DOCS Server Application", _message, EventLogEntryType.Error)
            Return False
        End If

        If Not IO.Directory.Exists(_ScriptLogBasePath) Then
            Try
                IO.Directory.CreateDirectory(_ScriptLogBasePath)
            Catch ex As Exception
                Dim _message As String = "ERROR Log Folder doesnt exists unable to create log directory '" + _ScriptLogBasePath + "'" + vbCrLf + ex.ToString
                EventLog.WriteEntry("Excitech DOCS Server Application", _message, EventLogEntryType.Error)
                Return False
            End Try
        End If

        ''server log path
        _ServerLogFile = "UIProcessor_" + Now.ToString("ddMMyyyy") + ".log"
        Dim _vaultDR As String = Vault.Name + " " + Vault.GetGUID

        If Not _ScriptLogBasePath.TrimEnd("\").ToUpperInvariant.EndsWith(_vaultDR.ToUpperInvariant) Then
            _ScriptLogBasePath = IO.Path.Combine(_ScriptLogBasePath, _vaultDR)
        End If

        _ServerLogFile = IO.Path.Combine(_ScriptLogBasePath, ServerLogFile)

        Return True
    End Function

    Public Shared Sub WriteToLog(value As String, LogID As Object)
        If LogID Is Nothing Then Exit Sub
        If String.IsNullOrWhiteSpace(LogID.ToString) Then Exit Sub
    End Sub

    Public Shared Sub WriteToLog(value As String, logFile As String)
        SyncLock _lock
            Try
                Using _sw As New IO.StreamWriter(logFile, True)
                    _sw.WriteLine(value)
                End Using
            Catch ex As Exception
                Dim _message As String = "ERROR unable to write to the log '" + logFile + "'" + vbCrLf + ex.ToString
                EventLog.WriteEntry("Excitech DOCS Server Application", _message, EventLogEntryType.Error)
            End Try
        End SyncLock
    End Sub

    Public Shared Sub WriteToServerLog(value As String)
        WriteToLog(value, ServerLogFile)
    End Sub

    Public Shared Sub DeleteLogFiles()
        If DeleteInterval <= 0 Then Exit Sub

        Try
            Dim _deleteDate As DateTime = Now.Subtract(TimeSpan.FromHours(DeleteInterval))
            Dim _logInfo As New IO.DirectoryInfo(ScriptLogBasePath)

            For Each _fi As IO.FileInfo In _logInfo.GetFiles("*.log", IO.SearchOption.AllDirectories)
                If _fi.LastAccessTime < _deleteDate Then
                    Try
                        _fi.Delete()
                    Catch ex As Exception
                        ''consume this for now
                    End Try
                End If
            Next

        Catch ex As Exception
            Dim _message As String = "ERROR when trying to process 'DeleteLogFiles'" & vbCrLf & ex.ToString
            WriteToLog(_message, ServerLogFile)
        End Try
    End Sub

    Public Shared Function GetScriptLogFilename(logID As String) As String
        Return IO.Path.Combine(ScriptLogBasePath, logID.ToLower + ".log")
    End Function

#End Region

End Class
