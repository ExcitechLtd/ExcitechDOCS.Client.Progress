Public Class UIAction
    Implements IUIActions

#Region " Private "
    Private _actiontype As String
#End Region

#Region " Properties "

    Public Property ActionType As String Implements IUIActions.ActionType
        Get
            Return _actiontype
        End Get
        Set(value As String)
            _actiontype = value.ToUpperInvariant.Trim
        End Set
    End Property

    Public Property Options As UIActionOptions Implements IUIActions.Options

    Public Property Handled As Boolean Implements IUIActions.Handled

    Public Property Key As Guid Implements IUIActions.Key
#End Region

#Region " Constructor "
    Public Sub New()
        Options = New UIActionOptions
        Handled = False
        Key = Guid.NewGuid
    End Sub
#End Region


End Class
