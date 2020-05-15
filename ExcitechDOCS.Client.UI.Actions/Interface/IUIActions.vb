Imports System.Runtime.InteropServices

<Guid("F1E1F045-9B46-43DD-9A11-CF2E9E500347"),
InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface IUIActions

    <DispId(1)> Property ActionType As String
    <DispId(2)> Property Options As UIActionOptions
    <DispId(3)> Property Handled As Boolean
    <DispId(4)> Property Key As Guid

End Interface
