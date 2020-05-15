Public Class UIActionList
    Implements IUIActionList

#Region " Private "
    Private _list As List(Of UIAction)
#End Region

#Region " Constructor "
    Public Sub New()
        _list = New List(Of UIAction)
    End Sub
#End Region

#Region " Methdos "
    Public Property Items As UIAction() Implements IUIActionList.Items
        Get
            Return _list.ToArray()
        End Get
        Set(value As UIAction())
            _list = New List(Of UIAction)(value)
        End Set
    End Property

    Public ReadOnly Property Count As Integer Implements IUIActionList.Count
        Get
            Return _list.Count
        End Get
    End Property

    Public Sub Add(item As UIAction) Implements IUIActionList.Add
        If _list Is Nothing Then _list = New List(Of UIAction)
        _list.Add(item)
    End Sub

    Public Sub Insert(index As Integer, item As UIAction) Implements IUIActionList.Insert
        If _list Is Nothing Then _list = New List(Of UIAction)
        If index = -1 Then _list.Add(item) : Exit Sub
        _list.Insert(index, item)
    End Sub

    Public Function IndexOf(item As UIAction) As Integer Implements IUIActionList.InedxOf
        If _list Is Nothing Then _list = New List(Of UIAction)
        Return _list.IndexOf(item)
    End Function

    Public Function Item(index As Integer) As UIAction Implements IUIActionList.Item
        If _list Is Nothing Then _list = New List(Of UIAction)
        If index > _list.Count Then Return Nothing

        Return _list(index)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of UIAction) Implements IUIActionList.GetEnumerator
        If _list Is Nothing Then _list = New List(Of UIAction)

        Return _list.GetEnumerator
    End Function
#End Region

End Class
