var m_shellUI;
var m_shellFrame;
var m_vault;
var m_count = 0;

var clrObj;

function OnNewShellUI(shellUI) {
    /// <summary>The entry point of ShellUI module.</summary>
    /// <param name="shellUI" type="MFiles.ShellUI">The new shell UI object.</param> 

    // Register to listen new shell frame creation event.
    shellUI.Events.Register(Event_NewNormalShellFrame, newShellFrameHandler);
    m_shellUI = shellUI;

    shellUI.Events.Register(Event_Started,newShellUiHandler)



    shellUI.Events.Register(Event_Stop, stopShellUiHandler)
       }

function newShellUiHandler(shellFrame) {


}

function stopShellUiHandler(shellFrame) {
   // m_shellUI.ShowMessage("ShellUI Stopped, CLRObj is " + clrObj);

    try {
        clrObj.StopListener();
    }
    catch (ex) {

    }

}

function newShellFrameHandler(shellFrame) {
    /// <summary>Handles the OnNewShellFrame event.</summary>
    /// <param name="shellFrame" type="MFiles.ShellFrame">The new shell frame object.</param> 

    // Register to listen the started event.
    shellFrame.Events.Register(Event_Started, getShellFrameStartedHandler(shellFrame));
    shellFrame.Events.Register(Event_Stop, getShellFrameStopHandler(shellFrame));


}

function getShellFrameStopHandler(shellFrame) {
    /// <summary>Returns a function which handles the OnStarted event for an IShellFrame.</summary>

    return function () {
        // The shell frame is now started and can be used.
        // Note: we need to use the global-scope variable.

       // shellFrame.ShowMessage("Shell frame stopped, CLRObj: " + clrObj);

        try {
            clrObj.StopListener();
        }
        catch (ex) {

        }
        
    }
}

function getShellFrameStartedHandler(shellFrame) {
    /// <summary>Returns a function which handles the OnStarted event for an IShellFrame.</summary>

    return function () {
        // The shell frame is now started and can be used.
        // Note: we need to use the global-scope variable.
       // shellFrame.ShowMessage("Shell frame stated");

        m_vault = m_shellUI.Vault;
        m_shellFrame = shellFrame;

    var clrName = Math.floor((Math.random() * 10) + 1);
    clrObj = MFiles.CreateObjectCLR("ExcitechDOCS.Client.UI.dll", "ExcitechDOCS.Client.UI.Dialog");
    clrObj.WindowHandle = shellFrame.OuterWindow.Handle;
    clrObj.ShellFrame = shellFrame;
    clrObj.ShellUI = m_shellUI;
    clrObj.StartListener(10000, clrName, m_shellUI.Vault.Name);
    }
}

