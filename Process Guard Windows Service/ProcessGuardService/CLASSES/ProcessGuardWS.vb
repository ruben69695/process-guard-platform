Imports System.Net.Http
Imports System.Threading
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Diagnostics

Public Class ProcessGuardWS
    Implements IDisposable

#Region "PROPIEDADES"
    Public service_url As String = ""
    Public ProcessList As IEnumerable(Of CLProcessItem)
    Public processlistHashCode As Long = 0
    Private WithEvents timer As New Timers.Timer
    Private ReadOnly client As New HttpClient()
    Private responseString As String = ""
    Private ReadOnly prime As Integer = 31
    Private eventLog As New EventLog
#End Region

#Region "CONSTRUCTORES"
    ''' <summary>
    ''' Constructor principal
    ''' </summary>
    ''' <param name="url"></param>
    Public Sub New(url As String, logSource As EventLog)
        service_url = url
        eventLog = logSource
        With timer
            .Interval = 100000
            .Enabled = True
            .Start()
        End With
        CheckAPI()
    End Sub
#End Region

#Region "EVENTOS"
    ''' <summary>
    ''' Método que saltara cuando el timer empiece, arrancara un thread que nos irá actualizando la lista de procesos contra la API
    ''' </summary>
    Private Sub CheckAPI() Handles timer.Elapsed
        Dim thread As Thread
        Try
            timer.Stop()
            thread = New Thread(AddressOf LaunchRequest)
            thread.IsBackground = True
            thread.Name = "API_Web-Request"
            thread.Start()
        Catch ex As Exception
            eventLog.WriteEntry(String.Format("{0} - ProcessGuardWS - FATAL ERROR, MENSAJE DE ERROR -> {1}", DateTime.Now, ex.ToString))
        Finally
            timer.Start()
        End Try
    End Sub
#End Region

#Region "MÉTODOS"

    ''' <summary>
    ''' Método que se encarga de procesar los datos recividos por parte del servidor
    ''' </summary>
    Private Async Sub LaunchRequest()
        Dim processArray As JArray
        Dim newprocess As CLProcessItem
        Dim newGeneralList As New List(Of CLProcessItem)()

        Try
            ' Get Request to get the process list from the web API in the Linux Server on Internet
            responseString = Await client.GetStringAsync(service_url)

            If Not String.IsNullOrEmpty(responseString) Then

                ' Get the JSON Array from the string request
                processArray = JArray.Parse(responseString)

                ' Reecolect the new objects JTokens and add it to the list
                If processArray.Count > 0 Then

                    ' Reseteamos el hash code
                    Me.processlistHashCode = 0

                    For i = 0 To processArray.Count - 1
                        newprocess = JsonConvert.DeserializeObject(Of CLProcessItem)(processArray(i).ToString())        ' Convertir el objeto JSON a objeto .NET
                        newGeneralList.Add(newprocess)                                                                  ' Añadir el objeto a lista de procesos
                        Me.processlistHashCode += newprocess.GetHashCode()                                              ' Add the hash code of the new object added to the list
                    Next
                    Me.ProcessList = newGeneralList
                Else
                    eventLog.WriteEntry(String.Format("{0} - ProcessGuardWS - La lista que retorna el servidor web esta vacía", DateTime.Now))
                End If

            End If
        Catch ex As Exception
            eventLog.WriteEntry(String.Format("{0} - ProcessGuardWS - FATAL ERROR, MENSAJE DE ERROR -> {1}", DateTime.Now, ex.ToString()))
        End Try
    End Sub

    ''' <summary>
    ''' Función que retorna el hashcode de la lista para saber si hay datos actualizados
    ''' </summary>
    ''' <returns></returns>
    Public Function GetProcessListHashCode() As Long
        Return (Me.processlistHashCode)
    End Function

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Para detectar llamadas redundantes

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: elimine el estado administrado (objetos administrados).
                Me.timer.Stop()
                Me.timer.Enabled = False
                Me.timer.Dispose()
            End If

            ' TODO: libere los recursos no administrados (objetos no administrados) y reemplace Finalize() a continuación.
            ' TODO: configure los campos grandes en nulos.
        End If
        disposedValue = True
    End Sub

    ' TODO: reemplace Finalize() solo si el anterior Dispose(disposing As Boolean) tiene código para liberar recursos no administrados.
    'Protected Overrides Sub Finalize()
    '    ' No cambie este código. Coloque el código de limpieza en el anterior Dispose(disposing As Boolean).
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic agrega este código para implementar correctamente el patrón descartable.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' No cambie este código. Coloque el código de limpieza en el anterior Dispose(disposing As Boolean).
        Dispose(True)
        ' TODO: quite la marca de comentario de la siguiente línea si Finalize() se ha reemplazado antes.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region



End Class