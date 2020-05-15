Imports System.Runtime.InteropServices

<Guid("2DA15DBC-37E1-49D4-9899-1F6CE303E8BC"),
InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface IUIActionList
    <DispId(1)> Property Items As UIAction()
    <DispId(2)> ReadOnly Property Count As Integer

    <DispId(3)> Sub Add(item As UIAction)
    <DispId(4)> Sub Insert(index As Integer, item As UIAction)
    <DispId(5)> Function InedxOf(item As UIAction) As Integer
    <DispId(6)> Function Item(index As Integer) As UIAction

    <DispId(-4)> Function GetEnumerator() As IEnumerator(Of UIAction)
End Interface
