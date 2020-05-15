Imports System.Runtime.InteropServices

<Guid("86337E2E-4B33-4814-8605-02109B147406"),
InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface IUIActionOptions
    <DispId(1)> ReadOnly Property Keys As String()
    <DispId(2)> ReadOnly Property Items As String()

    <DispId(0)> Property Item(key As String) As String
    <DispId(3)> Sub Add(key As String, value As String)
    <DispId(4)> Sub Remove(key As String)
    <DispId(5)> Function Contains(key As String) As Boolean
End Interface
