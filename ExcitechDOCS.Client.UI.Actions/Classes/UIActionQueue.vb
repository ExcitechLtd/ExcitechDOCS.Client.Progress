Imports MFilesAPI

Public Class UIActionQueue
    Implements IUIActionQueue

#Region " Properties "
    Public Property ActionToTake As List(Of UIAction) Implements IUIActionQueue.ActionToTake

    Public Shared ReadOnly Property Identifier As String
        Get
            Return "ExcitechDOCS.Client.UI.Settings"
        End Get
    End Property
#End Region

#Region " Constructor "
    Public Sub New()
        ActionToTake = New List(Of UIAction)
    End Sub
#End Region

#Region " Methods "

    Public Sub CleanActionList(HandledList As List(Of UIAction)) Implements IUIActionQueue.CleanActionList
        ActionToTake.RemoveAll(Function(acT) HandledList.Any(Function(hAC) hAC.Key = acT.Key))
    End Sub

    Public Shared Function readSettings(Vault As Vault) As UIActionQueue
        Try
            Dim UserID As String = Vault.CurrentLoggedInUserID.ToString
            Dim namedValues As New NamedValues
            namedValues = Vault.NamedValueStorageOperations.GetNamedValues(MFNamedValueType.MFConfigurationValue, UIActionQueue.Identifier)
            If IsDBNull(namedValues("USER:ACTIONS:" + UserID)) Then Return New UIActionQueue

            Dim settings As New UIActionQueue
            settings = Newtonsoft.Json.JsonConvert.DeserializeObject(Of UIActionQueue)(namedValues("USER:ACTIONS:" + UserID))
            '  Newtonsoft.Json.JsonConvert.PopulateObject(namedValues("USER:ACTIONS:" + UserID), settings)

            If settings Is Nothing Then Throw New Exception("Error reading settings from vault")

            Return settings
        Catch ex As Exception
            Throw
        End Try

        Return New UIActionQueue
    End Function

    Public Shared Function readSettings(Vault As Vault, USerID As String) As UIActionQueue
        Try
            'Dim UserID As String = Vault.CurrentLoggedInUserID.ToString
            Dim namedValues As New NamedValues
            namedValues = Vault.NamedValueStorageOperations.GetNamedValues(MFNamedValueType.MFConfigurationValue, UIActionQueue.Identifier)
            If IsDBNull(namedValues("USER:ACTIONS:" + USerID.ToString)) Then Return New UIActionQueue

            Dim settings As New UIActionQueue
            settings = Newtonsoft.Json.JsonConvert.DeserializeObject(Of UIActionQueue)(namedValues("USER:ACTIONS:" + USerID))
            'Newtonsoft.Json.JsonConvert.PopulateObject(namedValues("USER:ACTIONS:" + USerID), settings)

            If settings Is Nothing Then Throw New Exception("Error reading settings from vault")

            Return settings
        Catch ex As Exception
            Throw
        End Try

        Return New UIActionQueue
    End Function

    Public Shared Function saveSettings(Vault As Vault, settings As UIActionQueue) As Boolean

        Dim UserID As String = Vault.CurrentLoggedInUserID.ToString
            Dim namedvalues As New NamedValues
            namedvalues("USER:ACTIONS:" + UserID) = Newtonsoft.Json.JsonConvert.SerializeObject(settings)
            Vault.NamedValueStorageOperations.SetNamedValues(MFNamedValueType.MFConfigurationValue, Identifier, namedvalues)
            Return True

            Return False
    End Function

    Public Shared Function saveSettings(Vault As Vault, settings As UIActionQueue, userID As String) As Boolean

        'Dim UserID As String = Vault.CurrentLoggedInUserID.ToString
        Dim namedvalues As New NamedValues
            namedvalues("USER:ACTIONS:" + userID.ToString) = Newtonsoft.Json.JsonConvert.SerializeObject(settings)
            Vault.NamedValueStorageOperations.SetNamedValues(MFNamedValueType.MFConfigurationValue, Identifier, namedvalues)
            Return True

            Return False
    End Function

    Public Shared Function ClearSettings(vault As Vault) As Boolean
        Dim UserID As String = vault.CurrentLoggedInUserID.ToString
        Dim _namedValueNames As New Strings
        _namedValueNames.Add(-1, "USER:ACTIONS:" + UserID)
        vault.NamedValueStorageOperations.RemoveNamedValues(MFNamedValueType.MFConfigurationValue, Identifier, _namedValueNames)

        Return True
    End Function

    Public Shared Function ClearSettings(vault As Vault, UserID As String) As Boolean
        'Dim UserID As String = vault.CurrentLoggedInUserID.ToString
        Dim _namedValueNames As New Strings
        _namedValueNames.Add(-1, "USER:ACTIONS:" + UserID.ToString)
        vault.NamedValueStorageOperations.RemoveNamedValues(MFNamedValueType.MFConfigurationValue, Identifier, _namedValueNames)

        Return True
    End Function
#End Region

End Class
