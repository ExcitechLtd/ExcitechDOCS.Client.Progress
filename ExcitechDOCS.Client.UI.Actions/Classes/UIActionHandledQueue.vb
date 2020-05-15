Imports MFilesAPI

Public Class UIActionHandledQueue
    Implements IUIActionHandledQueue

#Region " Properties "
    Public Property HandledActions As List(Of UIAction) Implements IUIActionHandledQueue.HandledActions

    Public Shared ReadOnly Property Identifier As String
        Get
            Return "ExcitechDOCS.Client.UI.Settings.Handled"
        End Get
    End Property
#End Region

#Region " Constructor "
    Public Sub New()
        HandledActions = New List(Of UIAction)
    End Sub
#End Region

#Region " Methods "

    Public Shared Function readSettings(Vault As Vault) As UIActionHandledQueue
        Try
            Dim UserID As String = Vault.CurrentLoggedInUserID.ToString
            Dim namedValues As New NamedValues
            namedValues = Vault.NamedValueStorageOperations.GetNamedValues(MFNamedValueType.MFConfigurationValue, UIActionHandledQueue.Identifier)
            If IsDBNull(namedValues("USER:ACTIONSHANDLED:" + UserID)) Then Return New UIActionHandledQueue

            Dim settings As UIActionHandledQueue
            settings = Newtonsoft.Json.JsonConvert.DeserializeObject(Of UIActionHandledQueue)(namedValues("USER:ACTIONSHANDLED:" + UserID))

            If settings Is Nothing Then Throw New Exception("Error reading settings from vault")

            Return settings
        Catch ex As Exception
            Throw
        End Try

        Return New UIActionHandledQueue
    End Function

    Public Shared Function readSettings(Vault As Vault, UserID As String) As UIActionHandledQueue
        Try
            Dim namedValues As New NamedValues
            namedValues = Vault.NamedValueStorageOperations.GetNamedValues(MFNamedValueType.MFConfigurationValue, UIActionHandledQueue.Identifier)
            If IsDBNull(namedValues("USER:ACTIONSHANDLED:" + UserID.ToString)) Then Return New UIActionHandledQueue

            Dim settings As UIActionHandledQueue
            settings = Newtonsoft.Json.JsonConvert.DeserializeObject(Of UIActionHandledQueue)(namedValues("USER:ACTIONSHANDLED:" + UserID))

            If settings Is Nothing Then Throw New Exception("Error reading settings from vault")

            Return settings
        Catch ex As Exception
            Throw
        End Try

        Return New UIActionHandledQueue
    End Function

    Public Shared Function saveSettings(Vault As Vault, settings As UIActionHandledQueue) As Boolean

        Dim UserID As String = Vault.CurrentLoggedInUserID.ToString
            Dim namedvalues As New NamedValues
            namedvalues("USER:ACTIONSHANDLED:" + UserID) = Newtonsoft.Json.JsonConvert.SerializeObject(settings)
        Vault.NamedValueStorageOperations.SetNamedValues(MFNamedValueType.MFConfigurationValue, Identifier, namedvalues)
        Return True

            Return False
    End Function

    Public Shared Function saveSettings(Vault As Vault, settings As UIActionHandledQueue, userID As String) As Boolean

        'Dim UserID As String = Vault.CurrentLoggedInUserID.ToString
        Dim namedvalues As New NamedValues
            namedvalues("USER:ACTIONSHANDLED:" + userID.ToString) = Newtonsoft.Json.JsonConvert.SerializeObject(settings)
        Vault.NamedValueStorageOperations.SetNamedValues(MFNamedValueType.MFConfigurationValue, Identifier, namedvalues)
        Return True

            Return False
    End Function

    Public Shared Function ClearSettings(vault As Vault) As Boolean
        Dim UserID As String = vault.CurrentLoggedInUserID.ToString
        Dim _namedValueNames As New Strings
        _namedValueNames.Add(-1, "USER:ACTIONSHANDLED:" + UserID)
        vault.NamedValueStorageOperations.RemoveNamedValues(MFNamedValueType.MFConfigurationValue, Identifier, _namedValueNames)

        Return True
    End Function


    Public Shared Function ClearSettings(vault As Vault, userID As String) As Boolean
        'Dim UserID As String = vault.CurrentLoggedInUserID.ToString
        Dim _namedValueNames As New Strings
        _namedValueNames.Add(-1, "USER:ACTIONSHANDLED:" + userID)
        vault.NamedValueStorageOperations.RemoveNamedValues(MFNamedValueType.MFConfigurationValue, Identifier, _namedValueNames)

        Return True
    End Function
#End Region
End Class
