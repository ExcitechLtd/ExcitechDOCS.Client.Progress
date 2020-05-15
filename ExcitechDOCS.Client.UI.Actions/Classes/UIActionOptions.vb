Imports ExcitechDOCS.Client.UI
Imports Newtonsoft.Json

Public Class UIActionOptions
    Inherits Dictionary(Of String, String)
    Implements IUIActionOptions

    Public ReadOnly Property Items As String() Implements IUIActionOptions.Items
        Get
            Return MyBase.Values.ToArray
        End Get
    End Property

    Private Overloads ReadOnly Property Keys As String() Implements IUIActionOptions.Keys
        Get
            Return MyBase.Keys.ToArray
        End Get
    End Property

    Private Overloads Sub Add(key As String, value As String) Implements IUIActionOptions.Add
        MyBase.Add(key.ToUpperInvariant, value)
    End Sub

    Private Overloads Sub Remove(key As String) Implements IUIActionOptions.Remove
        MyBase.Remove(key.ToUpperInvariant)
    End Sub

    Public Overloads Function Contains(key As String) As Boolean Implements IUIActionOptions.Contains
        Return MyBase.ContainsKey(key.ToUpperInvariant)
    End Function

    Public Overloads Property Item(key As String) As String Implements IUIActionOptions.Item
        Get
            Return MyBase.Item(key.ToUpperInvariant)
        End Get
        Set(value As String)
            MyBase.Item(key.ToUpperInvariant) = value
        End Set
    End Property

    Public Overloads Function TryGetValue(key As String, ByRef value As String) As Boolean
        Return MyBase.TryGetValue(key.ToUpperInvariant, value)
    End Function

End Class
