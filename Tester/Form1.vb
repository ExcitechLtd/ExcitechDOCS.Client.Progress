Imports ExcitechDOCS.Client.UI
Imports MFilesAPI

Public Class Form1

    Private clientApp As New MFilesAPI.MFilesClientApplication
    Private Vault As MFilesAPI.Vault
    Private pCount As Integer = 10
    Private frmpb As frmPBar

    Private svrVault As MFilesAPI.Vault

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Dim clientApp As New MFilesAPI.MFilesClientApplication
        Dim namedValues As New NamedValues
        namedValues = Vault.NamedValueStorageOperations.GetNamedValues(MFNamedValueType.MFConfigurationValue, "ExcitechDOCS.UI")

        TextBox1.Text = ""

        Dim names As Strings = namedValues.Names
        For Each n In names
            TextBox1.Text += n & " " & namedValues.Value(n)
        Next


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim actionList As New UIActionQueue
        Dim ac As New UIAction
        ac.ActionType = "SHOWYESNODIALOG"
        ac.Options = New UIActionOptions
        ac.Options.Add("title", "Test Yes no dialog")
        ac.Options.Add("message", "Yes No dialog without timeout")
        ac.Options.Add("ICON", "tick")

        actionList.ActionToTake.Add(ac)
        UIActionQueue.saveSettings(Vault, actionList)

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim vaultConnection As MFilesAPI.VaultConnection = clientApp.GetVaultConnection("VM DOCS")
        Dim isL = vaultConnection.IsLoggedIn ''this says yes but we want to login as a different user
        Vault = vaultConnection.BindToVault(0, True, False)

        Dim svA As New MFilesAPI.MFilesServerApplication
        svA.Connect(MFAuthType.MFAuthTypeSpecificMFilesUser, "Boris", "Excitech1", Nothing, vaultConnection.ProtocolSequence, vaultConnection.NetworkAddress, vaultConnection.Endpoint)
        svrVault = svA.LogInToVault(vaultConnection.GetGUID)

        'Dim vc As MFilesAPI.VaultConnection = clA.GetVaultConnection("VM DOCS")
        'Dim v = vc.LogInAsUser(MFAuthType.MFAuthTypeSpecificMFilesUser, "Boris", "Excitech1")



    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        UIActionHandledQueue.ClearSettings(Vault)

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ''pull the handled items
        Dim handled As UIActionHandledQueue = UIActionHandledQueue.readSettings(Vault)
        PropertyGrid1.SelectedObject = handled
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        UIActionQueue.ClearSettings(Vault)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim actions As UIActionQueue = UIActionQueue.readSettings(Vault)
        PropertyGrid2.SelectedObject = actions
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim actionList As UIActionQueue = UIActionQueue.readSettings(Vault)
        Dim ac As New UIAction
        ac.ActionType = "SHOWSIMPLEPROGRESS"
        ac.Options = New UIActionOptions
        ac.Options.Add("title", "Test Progress dialog")
        ac.Options.Add("message", "Progress without timeout")
        ac.Options.Add("ICON", "tick")

        actionList.ActionToTake.Add(ac)
        UIActionQueue.saveSettings(Vault, actionList)

    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        pCount += 1

        Dim actionList As UIActionQueue = UIActionQueue.readSettings(Vault)
        Dim ac As New UIAction
        ac.ActionType = "UPDATEPROGRESS"
        ac.Options = New UIActionOptions
        ac.Options.Add("value", pCount)

        actionList.ActionToTake.Add(ac)
        UIActionQueue.saveSettings(Vault, actionList)
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim actionList As UIActionQueue = UIActionQueue.readSettings(Vault)
        Dim ac As New UIAction
        ac.ActionType = "DONE"
        ac.Options = New UIActionOptions
        ac.Options.Add("value", "Finished")

        actionList.ActionToTake.Add(ac)
        UIActionQueue.saveSettings(Vault, actionList)
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Dim actionList As UIActionQueue = UIActionQueue.readSettings(Vault)
        Dim ac As New UIAction
        ac.ActionType = "UPDATEMESSAGE"
        ac.Options = New UIActionOptions
        ac.Options.Add("value", "Progress Value should show" & pCount.ToString)

        actionList.ActionToTake.Add(ac)
        UIActionQueue.saveSettings(Vault, actionList)
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        frmpb = New frmPBar
        frmpb.StartPosition = FormStartPosition.CenterParent
        frmpb.ProgressBar1.Value = 0
        frmpb.Show()
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        pCount += 1
        frmPBar.updatepBar(pCount)
        frmPBar.Update()

        frmPBar.Refresh()

        frmPBar.up(pCount)
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        Dim vaf As New ExcitechDOCS.Server.UI.VaultApplication

        vaf.initSession(Vault, "123456", Vault.CurrentLoggedInUserID)

        Dim ac As UIAction = vaf.NewAction
        ac.ActionType = "SHOWSIMPLEPROGRESS"
        ac.Options.Add("TITLE", "Test VAF")
        ac.Options.Add("message", "Test Progress with vaf application")
        ac.Options.Add("ICON", "INFORMATION")

        vaf.AddAction(Vault, ac, Vault.CurrentLoggedInUserID)

        ''''implement the progress bar
        Dim pCount As Integer = 0
        ac = vaf.NewAction
        ac.ActionType = "UPDATEPROGRESS"
        ac.Options.Add("value", pCount)
        vaf.AddAction(Vault, ac, Vault.CurrentLoggedInUserID)


        For c As Integer = 0 To 9
            Threading.Thread.Sleep(500)
            pCount += 6

            ac = vaf.NewAction
            ac.ActionType = "UPDATEPROGRESS"
            ac.Options.Add("value", pCount)
            vaf.AddAction(Vault, ac, Vault.CurrentLoggedInUserID)
        Next

        ac = vaf.NewAction
        ac.ActionType = "DONE"
        ac.Options.Add("value", "Finished")
        vaf.AddAction(Vault, ac, Vault.CurrentLoggedInUserID)
    End Sub

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Dim vaf As New ExcitechDOCS.Server.UI.VaultApplication

        vaf.initSession(Vault, "123456", Vault.CurrentLoggedInUserID)

        Dim ac As UIAction = vaf.NewAction
        ac.ActionType = "SHOWYESNODIALOG"
        ac.Options.Add("TITLE", "Test VAF")
        ac.Options.Add("message", "Test YES NO with vaf application")
        ac.Options.Add("ICON", "QUESTION")

        Dim dlgKey As String = vaf.AddAction(Vault, ac, Vault.CurrentLoggedInUserID)

        'Dim resp As String = vaf.WaitForActionResponse(Vault, Vault.CurrentLoggedInUserID, dlgKey, 60)

        'MsgBox(resp)
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        Dim handled As UIActionHandledQueue = UIActionHandledQueue.readSettings(svrVault, Vault.CurrentLoggedInUserID)
        PropertyGrid1.SelectedObject = handled
    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        ''add a namved value to the user #61
        ''read it from user boris
        Dim UserID As String = Vault.CurrentLoggedInUserID.ToString
        Dim namedvalues As New NamedValues
        namedvalues("USER:ACTIONS:" + UserID) = "SET USERVALUE"
        Vault.NamedValueStorageOperations.SetNamedValues(MFNamedValueType.MFUserDefinedValue, "DOCSTEST", namedvalues)

        Dim res As String = ""
        namedvalues = New NamedValues
        namedvalues = svrVault.NamedValueStorageOperations.GetNamedValues(MFNamedValueType.MFUserDefinedValue, UIActionQueue.Identifier)
        If IsDBNull(namedvalues("USER:ACTIONS:" + UserID.ToString)) Then res = "USERVALUE IS BLANK" Else res = namedvalues("USER:ACTIONS:" + UserID.ToString)
        MsgBox(res)


        ''save a valuea aginast svr
        namedvalues = New NamedValues
        namedvalues("USER:ACTIONS:" + UserID) = "SET BORIS"
        svrVault.NamedValueStorageOperations.SetNamedValues(MFNamedValueType.MFUserDefinedValue, "DOCSTEST", namedvalues)

        ''read from vault
        res = ""
        namedvalues = New NamedValues
        namedvalues = Vault.NamedValueStorageOperations.GetNamedValues(MFNamedValueType.MFUserDefinedValue, UIActionQueue.Identifier)
        If IsDBNull(namedvalues("USER:ACTIONS:" + UserID.ToString)) Then res = "BORIS IS BLANK" Else res = namedvalues("USER:ACTIONS:" + UserID.ToString)
        MsgBox(res)
    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        Dim actions As UIActionQueue = UIActionQueue.readSettings(svrVault, Vault.CurrentLoggedInUserID)
        PropertyGrid2.SelectedObject = actions
    End Sub
End Class
