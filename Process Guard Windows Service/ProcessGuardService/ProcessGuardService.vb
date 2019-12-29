Imports System.ServiceProcess
Imports System.Linq
Imports System.IO
Imports System.Text
Imports UtilityLibraries.DatesLibrary
Imports System.Net
Imports System.Net.NetworkInformation

Public Class ProcessGuardService

#Region "PROPERTIES"
    Private WithEvents timer As New Timers.Timer
    Private processList As IEnumerable(Of CLProcessItem)
    Private processguardWS As ProcessGuardWS
    Private HashOfProcessList As Int64 = 0
    Private rutaAPI As String = ""
    Private rutaSeeds As String = ""
    Private PingServerOk As Boolean = False
    Private SeedPathOk As Boolean = False
#End Region

#Region "SERVICE STATUS METHODS"
    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Agregue el código aquí para iniciar el servicio. Este método debería poner
        ' en movimiento los elementos para que el servicio pueda funcionar.

        EventLog.WriteEntry(String.Format("{0} - STARTING PROCESS GUARD SERVICE...", DateTime.Now))

        ' Hasta que no obtengamos una ruta de conexión a la API y una ruta de semillas correctamente no salimos del bucle.
        While Not (PingServerOk) OrElse Not (SeedPathOk)
            Try
                ' Obtenemos el HOST para la API realizando un test (PING) de red y luego la ruta de las SEMILLAS
                PingServerOk = ConfigRutaAPI()
                SeedPathOk = ConfigRutaSemillas()

                ' Controlamos el flujo lógico, si hay errores mostramos errores, si no arrancamos el servicio
                If Not SeedPathOk OrElse Not PingServerOk Then
                    If String.IsNullOrWhiteSpace(rutaSeeds) Then
                        EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - La cadena de las semillas esta vacía, no se podrá dejar semillas, se volverá a intentar en 20 segundos...", DateTime.Now))
                    End If
                    If String.IsNullOrWhiteSpace(rutaAPI) Then
                        EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - La cadena de conexión a la API esta vacía, no se podrá connectar con la API, se volverá a intentar en 20 segundos...", DateTime.Now))
                    End If
                    If Not SeedPathOk Then
                        EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - El directorio de las semillas con ruta no existe y no se ha podido crear, se volverá a intentar en 20 segundos...", DateTime.Now))
                    End If
                    If Not PingServerOk Then
                        EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - No se puede conectar con el host en la nube para obtener los datos, se volverá a intentar en 20 segundos...", DateTime.Now))
                    End If
                    System.Threading.Thread.Sleep(20000)
                Else
                    EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Las cadenas de la API y de las semillas son correctas, starting service...", DateTime.Now))
                    StartService()
                End If

            Catch ex As Exception
                EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - FATAL ERROR -> {1}", DateTime.Now, ex.ToString()))
            End Try

        End While
    End Sub

    Protected Overrides Sub OnStop()
        ' Agregue el código aquí para realizar cualquier anulación necesaria para detener el servicio.
        EventLog.WriteEntry(String.Format("{0} - STOPING PROCESS GUARD SERVICE...", DateTime.Now))
        timer.Stop()
        timer.Enabled = False

        ' Hacemos el dispose de ProcessGuard y otras variables que ocupen
        If Not processguardWS Is Nothing Then
            processguardWS.Dispose()
        End If
    End Sub

    ''' <summary>
    ''' Método que arranca el servicio al completo, se encarga de instanciar la clase que nos gestionara la obtención de las listas contra el host en la nube, y también encenderá el timer de esta clase para empezar a trabajar
    ''' </summary>
    Private Sub StartService()
        processguardWS = New ProcessGuardWS(rutaAPI, EventLog)
        With timer
            .Interval = 10000
            .Enabled = True
            .Start()
        End With
    End Sub

#End Region

#Region "PROCESS CONTROLS"

    ''' <summary>
    ''' Método que comprueba los servicios cuando el timer salta
    ''' </summary>
    Private Sub CheckProcesses() Handles timer.Elapsed
        Try
            timer.Stop()

            ' Obtenemos los procesos que hay en ejecución en el sistema
            Dim localExecutingProcess As IEnumerable(Of Process) = Process.GetProcesses()

            ' Solo obtenemos la lista de procesos negros y blancos si ha sido actualizada y si tiene elementos
            If HashOfProcessList <> processguardWS.GetProcessListHashCode() AndAlso processguardWS.ProcessList.Count > 0 Then
                HashOfProcessList = processguardWS.processlistHashCode
                processList = processguardWS.ProcessList
            End If

            ' Miramos si se ha actualizado las claves del registro
            ConfigRutaSemillas()

            ' Si la ruta de configuración cambia, avisamos a la clase que gestiona la interacción con la API para que cambie la ruta de las peticiones
            If ConfigRutaAPI() Then
                processguardWS.service_url = rutaAPI
            End If

            ' Eliminamos los archivos de las semillas que sobrepasen 1 minuto en el directorio actual de semillas
            DeleteOldSeedFiles()

            If Not processList Is Nothing AndAlso processList.Count > 0 Then

                ' Miramos los procesos que hay recuperados de la API y cerramos los correspondientes
                For i = 0 To processList.Count - 1

                    ' Recorremos todos los procesos que hay en ejecución en el sistema ya que puede haber mas de uno abierto, y tenemos que cerrar todos
                    Dim processFinded = False
                    For j = 0 To localExecutingProcess.Count - 1
                        Try
                            ' Si el proceso de la API coincide con uno del sistema y es negro lo matamos y activamos a true la variable de processFinded
                            If processList(i).exe = localExecutingProcess(j).ProcessName Then
                                If Not processFinded Then
                                    processFinded = True
                                End If
                                If processList(i).color = "black" Then
                                    ' ****** IMPORTANTE****** 
                                    ' Acabamos de comprobar que se pueda matar al proceso ya que si no da Fatal Error pk hay procesos(padre) que si los matas,
                                    ' matas a sus hijos, entonces llegas aqui y ya el proceso ya no se puede matar pk esta muerto y peta, con este if se controla si ya finalizo o no
                                    If Not localExecutingProcess(j).HasExited Then
                                        localExecutingProcess(j).Kill()
                                        EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Proceso {1} prohibido detectado, se mata el proceso", DateTime.Now, processList(i).exe))
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - FATAL ERROR : {1}", DateTime.Now, ex.ToString()))
                        End Try
                    Next

                    ' Si processFinded es falso ya que no se ha encontrado el proceso en la lista de procesos del sistema y el proceso es de tipo white lanzamos semilla para que se habra el programa
                    If Not processFinded AndAlso processList(i).color = "white" Then
                        Try
                            CreateSeed(processList(i))
                        Catch ex As IO.FileNotFoundException
                            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - This application {1} with this filename: {2} doesn't exist in the system", DateTime.Now, processList(i).exe, processList(i).filename))
                        Catch ex As Exception
                            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - FATAL ERROR : {1}", DateTime.Now, ex.ToString()))
                        End Try
                    End If
                Next
            Else
                EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - La lista de procesos que ha proporcionado ProcessGuardWS esta vacío, compruebelo", DateTime.Now))
            End If
        Catch ex As Exception
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - FATAL ERROR -> {1}", DateTime.Now, ex.ToString()))
        Finally
            timer.Start()
        End Try

    End Sub

#End Region

#Region "CHECK CONFIGURATION"


    ''' <summary>
    ''' Configura la ruta del servidor si ha cambiado
    ''' </summary>
    Private Function ConfigRutaAPI() As Boolean
        Dim valorClave = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\rubenASIX", "ProcessguardAPI", "")
        Dim http As String = "http://"
        Dim https As String = "https://"
        Dim onlyHost As String = ""
        Dim connection As Boolean = False
        Dim intentos As Integer = 5
        Dim correctos As Integer = 0
        Dim connectionStrong As Boolean = False
        Try
            ' Si el valor es correcto y ha cambiado respecto a la pasada ruta, intentamos configurar una nueva si no nos quedamos con la antigua
            If Not String.IsNullOrWhiteSpace(valorClave) AndAlso valorClave <> rutaAPI Then
                ' Quitamos lo innecesario de la ruta para poder hacer un ping a un host, en el caso de que tenga cadenas http o https las eliminamos y luego las rutas de la API que empiecen por /
                If valorClave.Contains(http) Then
                    onlyHost = valorClave.Replace(http, "")
                ElseIf valorClave.Contains(https) Then
                    onlyHost = valorClave.Replace(https, "")
                End If
                Dim indicebarra = onlyHost.IndexOf("/")
                onlyHost = onlyHost.Remove(indicebarra, onlyHost.Length - indicebarra)

                ' Intentar hacer ping durante el numero de intentos, y sumar los que sean correctos
                For i = 0 To intentos - 1
                    connection = MakePing(onlyHost)
                    If connection Then
                        correctos += 1
                    End If
                Next
                ' Si de el numero de veces que ha intentado hacer ping minimo han habido 2 pongs que han salido bien se considera que la conexión es correcta
                If correctos >= 2 Then
                    rutaAPI = valorClave        ' Configuramos el valor de la clave en la variable que guarda la ruta de la API
                    connectionStrong = True     ' Devolvemos true
                    EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Nueva cadena de conexión para la API configurada : {1}", DateTime.Now, rutaAPI))
                End If
            End If
        Catch ex As Exception
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Nueva cadena de conexión para la API configurada : {1}", DateTime.Now, ex.ToString))
        End Try
        Return (connectionStrong)
    End Function

    ''' <summary>
    ''' Nos configura una nueva ruta en la variable de la clase si el valor ha cambiado en el regedit, no comprueba si el directorio existe o no, también en el caso de que se haya cambiado en tiempo de ejecución
    ''' la ruta eliminara todos los ficheros de la antigua ruta para dejarlo limpio
    ''' </summary>
    ''' <returns></returns>
    Private Function ConfigRutaSemillas() As Boolean
        Dim valorChanged As Boolean = False
        Try
            Dim valorClave As String = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\rubenASIX", "RutaSemilla", "")
            ' Si el valor es correcto y ha cambiado respecto a la pasada ruta, intentamos configurar una nueva si no nos quedamos con la antigua
            If Not String.IsNullOrWhiteSpace(valorClave) AndAlso valorClave <> rutaSeeds Then
                DeleteAllFiles(rutaSeeds)   ' Delete all the files of the old directory only if the directory exists
                rutaSeeds = valorClave      ' Configure the new seeds path in the main class property
                valorChanged = True
            End If
        Catch ex As Exception
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - FATAL ERROR : {1}", DateTime.Now, ex.ToString()))
        End Try

        Return (valorChanged)
    End Function

#End Region

#Region "WORKING WITH FILES"


    ''' <summary>
    ''' Creamos la semilla, si el directorio no existe el servicio lo crea y si no lo crea lanzara mensaje de log
    ''' </summary>
    ''' <param name="cLProcessItem"></param>
    Private Sub CreateSeed(cLProcessItem As CLProcessItem)
        Dim xpath As String = ""
        Dim fs As FileStream
        Dim info As Byte()
        Try
            ' Aqui creamos como se va a llamar el fichero y donde se guardar y con que extensión, ej: C:\Semillas25\CProgramFilesinternetexploreriexplore.exe.json
            xpath = String.Format("{0}{1}{2}.{3}", rutaSeeds, Path.DirectorySeparatorChar, cLProcessItem.filename.Replace("\", "").Replace(" ", "").Replace(":", ""), "json")

            If Not Directory.Exists(rutaSeeds) Then
                Directory.CreateDirectory(rutaSeeds)        ' Creamos directorio si no existe
                ConfigurarPermisosRutaSemilla(rutaSeeds)    ' Le ponemos los permisos correspondientes
            End If

            If Directory.Exists(rutaSeeds) Then
                If Not File.Exists(xpath) Then
                    fs = File.Create(xpath)
                    info = New UTF8Encoding(True).GetBytes(cLProcessItem.ToJson())
                    fs.Write(info, 0, info.Length)
                    fs.Close()
                    fs.Dispose()
                    File.SetCreationTime(xpath, DateTime.Now)
                    EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Se deja semilla en directorio {1} para abrir el proceso {2}", DateTime.Now, rutaSeeds, cLProcessItem.exe))
                End If
            Else
                EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - La ruta {1} de los seeds no existía y no se ha podido crear", DateTime.Now, rutaSeeds))
            End If
        Catch ex As UnauthorizedAccessException
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Error no tienes permisos, mas detalles --> {1}", DateTime.Now, ex.Message()))
        Catch ex As IOException
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Error de entrada y salida, mas detalles -> {1}", DateTime.Now, ex.Message()))
        Catch ex As Exception
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - FATAL ERROR -> {1}", DateTime.Now, ex.ToString()))
        End Try
    End Sub

    ''' <summary>
    ''' Elimina todos los ficheros del directorio si existen
    ''' </summary>
    Private Sub DeleteAllFiles(xpath As String)
        Try
            If Directory.Exists(xpath) Then
                For Each filename In Directory.GetFiles(xpath)
                    File.Delete(filename)
                Next
            End If
        Catch ex As UnauthorizedAccessException
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Error no tienes permisos, mas detalles --> {1}", DateTime.Now, ex.Message()))
        Catch ex As IOException
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Error de entrada y salida, mas detalles -> {1}", DateTime.Now, ex.Message()))
        Catch ex As Exception
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - FATAL ERROR : {1}", DateTime.Now, ex.ToString()))
        End Try
    End Sub

    ''' <summary>
    ''' Elimina los ficheros que sean mas viejos o iguales de 1 minuto
    ''' </summary>
    Private Sub DeleteOldSeedFiles()
        Dim fechaActual As DateTime = DateTime.Now
        Dim timeInterval As TimeSpan
        Dim dateFile As DateTime
        Try
            If Directory.Exists(rutaSeeds) Then
                For Each filename In Directory.GetFiles(rutaSeeds)
                    dateFile = File.GetCreationTime(filename)
                    timeInterval = CalculateIntervalBetweenDates(dateFile, fechaActual)
                    If timeInterval.Minutes >= 1 Then
                        File.Delete(filename)
                        'EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Fichero con ruta {1} , eliminado del directorio semillas", DateTime.Now, filename))
                    End If
                Next
            End If
        Catch ex As UnauthorizedAccessException
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Error no tienes permisos, mas detalles --> {1}", DateTime.Now, ex.Message))
        Catch ex As IOException
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - Error de entrada y salida, mas detalles -> {1}", DateTime.Now, ex.Message))
        Catch ex As Exception
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - FATAL ERROR -> {1}", DateTime.Now, ex.ToString()))
        End Try
    End Sub

#End Region

#Region "NETWORK FUNCTIONS"

    ''' <summary>
    ''' Función que retorna booleano si tenemos conexión con el host pasado por parametro
    ''' </summary>
    ''' <param name="host"></param>
    ''' <returns></returns>
    Private Function MakePing(host As String) As Boolean
        Dim pingSender As New Ping()
        Dim options As New PingOptions()
        Dim connectionOk As Boolean = False

        Try
            'Use the Default Ttl value which Is 128,
            'but change the fragmentation behavior.
            options.DontFragment = True

            ' Create a buffer of 32 bytes of data to be transmitted
            Dim data As String = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
            Dim buffer As Byte() = Encoding.ASCII.GetBytes(data)
            Dim timeout As Integer = 120
            Dim reply As PingReply = pingSender.Send(host, timeout, buffer, options)
            If reply.Status = IPStatus.Success Then
                connectionOk = True
            End If
        Catch ex As Exception
            EventLog.WriteEntry(String.Format("{0} - ProcessGuardService - FATAL ERROR -> {1}", DateTime.Now, ex.ToString()))
        End Try
        Return (connectionOk)
    End Function

#End Region

End Class
