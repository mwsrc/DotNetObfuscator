Imports System.Threading

Friend Class Program
    <STAThread()>
    Public Shared Sub Main(ByVal Args As String())
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Dim instanceCountOne As Boolean = False
        Using mtex As Mutex = New Mutex(True, Application.ProductName, instanceCountOne)
            If instanceCountOne Then
                Application.Run(New Frm_Main)
                mtex.ReleaseMutex()
            End If
        End Using
    End Sub
End Class

