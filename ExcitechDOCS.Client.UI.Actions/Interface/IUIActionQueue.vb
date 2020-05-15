Imports System.Runtime.InteropServices

<Guid("4E0A3336-9CCE-41DD-9D12-712873FC202E"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface IUIActionQueue

    <DispId(1)> Property ActionToTake As List(Of UIAction)

    <DispId(2)> Sub CleanActionList(HandledList As List(Of UIAction))

End Interface
