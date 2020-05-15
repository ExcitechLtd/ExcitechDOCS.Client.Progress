Imports System.Runtime.InteropServices

<Guid("4DCF3100-8115-4082-ACE1-3CD6663F99E9"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface IUIActionHandledQueue
    <DispId(1)> Property HandledActions As List(Of UIAction)


End Interface
