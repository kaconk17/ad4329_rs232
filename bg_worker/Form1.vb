Imports System.ComponentModel
Imports System.IO.Ports
Public Class Form1
    Dim port As String
    Dim strhasil As String()
    Public Sub New()
        InitializeComponent()
        BackgroundWorker1.WorkerReportsProgress = True
        BackgroundWorker1.WorkerSupportsCancellation = True
        GetSerialPortNames()
    End Sub
    Sub GetSerialPortNames()
        ' Show all available COM ports.
        For Each sp As String In My.Computer.Ports.SerialPortNames
            ComboBox1.Items.Add(sp)
        Next
    End Sub
    Private Sub startAsyncButton_Click(ByVal sender As System.Object,
    ByVal e As System.EventArgs) Handles btn_start.Click
        If ComboBox1.Text = "" Then
            MsgBox("COM Port belum diisi !")
            Exit Sub
        End If
        port = ComboBox1.Text
        If BackgroundWorker1.IsBusy <> True Then
            ' Start the asynchronous operation.
            BackgroundWorker1.RunWorkerAsync()
        End If
    End Sub

    Private Sub cancelAsyncButton_Click(ByVal sender As System.Object,
    ByVal e As System.EventArgs) Handles stop_btn.Click
        If BackgroundWorker1.WorkerSupportsCancellation = True Then
            ' Cancel the asynchronous operation.
            BackgroundWorker1.CancelAsync()
        End If
    End Sub

    ' This event handler is where the time-consuming work is done.
    Private Sub backgroundWorker1_DoWork(ByVal sender As System.Object,
    ByVal e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)

        'Dim hitung As Decimal = 0
        Dim hasil As String = ""

        Dim com1 As SerialPort
        com1 = New SerialPort(port, 2400, Parity.None, 7, StopBits.One)
        If com1.IsOpen Then
            com1.Close()
        End If
        Try
            com1.Open()

        Catch ex As Exception
            ' returnStr = ex.ToString()

            com1.Close()
            Exit Sub
        End Try
        com1.ReadTimeout = 10000
        Dim Incoming As String = "0"
        Do
            'hitung = hitung + 1
            If (worker.CancellationPending = True) Then
                e.Cancel = True
                com1.Close()
                Exit Do
            Else
                ' Perform a time consuming operation and report progress.
                Incoming = com1.ReadLine()
                ' Incoming = "gg," & hitung.ToString()
                hasil = Incoming & vbCrLf
                Dim words As String() = hasil.Split(New Char() {","c})
                Dim r As Integer = words.Length()
                If r > 2 Then
                    worker.ReportProgress(10, words)
                End If
                System.Threading.Thread.Sleep(100)


            End If
        Loop

    End Sub

    ' This event handler updates the progress.
    Private Sub backgroundWorker1_ProgressChanged(ByVal sender As System.Object,
    ByVal e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        'result.Text = (e.ProgressPercentage.ToString())
        strhasil = e.UserState
        'Dim words As String() = strhasil.Split(New Char() {","c})
        Dim ba As String = strhasil(2)
        Dim st As String = strhasil(0)

        result.Text = ba.Substring(3)
        result.ForeColor = Color.DarkRed

        result.BackColor = SystemColors.ControlLight
        If st = "ST" Then
            result.ForeColor = Color.White

            result.BackColor = Color.Black

        End If

    End Sub

    ' This event handler deals with the results of the background operation.
    Private Sub backgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object,
    ByVal e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If e.Cancelled = True Then
            result.Text = "Canceled!"
        ElseIf e.Error IsNot Nothing Then
            result.Text = "Error: " & e.Error.Message
        Else
            result.Text = "Done!"
        End If
    End Sub

End Class
