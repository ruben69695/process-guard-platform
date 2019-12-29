Imports System.ServiceProcess
Imports System.IO
Imports Newtonsoft.Json
Imports Process_Guard_App.Constantsvb
Imports Process_Guard_App.Permissions
Imports System.Management
Imports Microsoft.Win32

Public Class frmMain

    Private Service As ServiceController
    Private ServiceInstalled As Boolean = False
    Private WithEvents Timer As New Timer()
    Private ConfigurationOk As Boolean = False

#Region "EVENTOS"

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Mostramos notificación al usuario, para que sepa que se esta ejecutando la aplicación
        NotifyIcon.ShowBalloonTip(1, NotifyIcon.BalloonTipTitle, String.Format("Hola soy Process Guard te ayudo a ser mas productivo (: "), ToolTipIcon.Info)

        ' Establecemos el intervalo del timer
        With Timer
            .Interval = 4000
            .Enabled = False
        End With

        ' Si el usuario, es usuario administrador habilitamos el menu del system tray de la aplicación
        If IsAdministrator() Then
            ActivarItemsMenu()
        End If

        ' Comprobamos si el servicio process guard esta instalado
        ServiceInstalled = IsServiceInstalled(_SERVICENAME)

        If Not ServiceInstalled Then
            MsgBox(String.Format("El servicio {0} no se encuentra instalado en el sistema, por favor es necesario instalarlo", _SERVICENAME), MsgBoxStyle.Exclamation, "Atención")
        Else
            ' Instanciamos un ServiceController del servicio Process Guard. Si el servicio esta parado avisamos al usuario de que es necesario ponerlo en marcha
            Service = New ServiceController(_SERVICENAME)
            If Service.Status = ServiceControllerStatus.Stopped OrElse Service.Status = ServiceControllerStatus.StopPending Then
                MsgBox(String.Format("El servicio {0} esta parado, es necesario ponerlo en marcha", _SERVICENAME), MsgBoxStyle.Information, "Information")
            End If
        End If

        ConfigurationOk = RegeditOk()

        If Not ConfigurationOk Then
            Dim response As MsgBoxResult = MsgBox("No se ha encontrado una configuración para el directorio de las semillas y la API. Si no se configuran la aplicación no funcionara, desea configurarlas ahora?", MsgBoxStyle.YesNo, "Information")
            If response = MsgBoxResult.Yes Then
                ShowForm()
            End If
        Else
            IniciarTimer()
        End If

    End Sub

    Private Sub Seeds_TextBox_ButtonClick(sender As Object, e As EventArgs) Handles Seeds_TextBox.ButtonClick
        If FolderBrowser.ShowDialog() = DialogResult.OK Then
            Seeds_TextBox.Text = FolderBrowser.SelectedPath
        End If
    End Sub

    ''' <summary>
    ''' Si se pulsa el botón Save, guarda la configuración y esconde el formulario
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Save_btn_Click(sender As Object, e As EventArgs) Handles Save_btn.Click
        If API_TextBox.Text <> "" AndAlso API_TextBox.Text <> "" Then
            HideForm()
            SaveConfigurationIntoRegedit()
        Else
            MsgBox("Te faltan datos por rellenar, no puedes dejar la configuración en blanco", MsgBoxStyle.Exclamation, "Avíso")
        End If
    End Sub

    ''' <summary>
    ''' Método que se ejecutada cuando el timer salta, nos comprueba si hay semillas, las recoge las procesa y si el proceso que trae la semilla no se esta ejecutando lo ejecuta
    ''' </summary>
    Private Async Sub CheckSeeds() Handles Timer.Tick
        Try
            Dim seedvalue As String = My.Computer.Registry.GetValue(_REGEDITKEY, _SEEDVALUE, "")
            Dim newProcess As CLProcessItem
            Dim fileNames As IEnumerable(Of String)
            Dim fileContent As String = ""

            Timer.Stop()

            If Directory.Exists(seedvalue) Then
                ' Obtenemos los nombres de todos los ficheros del directorio
                fileNames = Directory.GetFiles(seedvalue)

                ' Recorremos cada directorio por su nombre
                For i = 0 To fileNames.Count - 1

                    fileContent = ""

                    ' Obtenemos el contenido del fichero usando using para librerar streamreader cuando ya ha leido el contenido
                    Using str As New StreamReader(fileNames(i), System.Text.Encoding.UTF8)
                        fileContent = Await str.ReadToEndAsync()
                    End Using

                    ' Transformamos el contenido del fichero que es un objeto JSON a un objeto .NET
                    newProcess = JsonConvert.DeserializeObject(Of CLProcessItem)(fileContent)

                    ' Si el proceso recuperado ya esta en marcha no hace falta que lo volvemos a ejecutar
                    If Not Process.GetProcessesByName(newProcess.exe).Count > 0 Then

                        ' Si la ruta de la aplicación empieza por \AppData Le añadimos el perfil del usuario a la ruta, esto es debido a que cada perfil de usuario es diferente por lo tanto si hay una ruta de un AppData en base de datos solo se guarda
                        ' a partir del \AppData luego por codigo le añadimos la parte previa en el caso habitual C:\Users\username + \AppData...
                        If newProcess.filename.StartsWith("\AppData") Then
                            newProcess.filename = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & newProcess.filename
                        End If

                        ' Si el archivo existe lo ejecutamos
                        If File.Exists(newProcess.filename) Then
                            Process.Start(newProcess.filename)
                            NotifyIcon.ShowBalloonTip(3, NotifyIcon.BalloonTipTitle, String.Format("Aplicación obligatoria {0} abierta.", newProcess.exe), ToolTipIcon.Info)
                        End If
                    End If
                Next

            End If

            Timer.Start()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Exception")
        End Try

    End Sub





#End Region

#Region "METHODS AND FUNCTIONS"

    ''' <summary>
    ''' Configura e inicializa el timer
    ''' </summary>
    Private Sub IniciarTimer()
        ' Inicializar Timer
        With Timer
            .Enabled = True
            .Start()
        End With
    End Sub

    ''' <summary>
    ''' Activa los items del menu del icono de la bandeja
    ''' </summary>
    Private Sub ActivarItemsMenu()
        For i = 0 To NotifyIcon.ContextMenuStrip.Items.Count - 1
            NotifyIcon.ContextMenuStrip.Items(i).Enabled = True
        Next
    End Sub

    ''' <summary>
    ''' Método que carga los datos de configuración del regedit en el textboxes
    ''' </summary>
    Private Sub LoadConfigurationIntoForm()
        ' Obtenemos los valores mediante los valores almacenados en  el regedit
        Try
            Dim regedit As RegistryKey = Registry.LocalMachine.CreateSubKey(_SUBKEY)

            ' Si existe la subkey agregamos los valores
            If Not regedit Is Nothing Then
                If Not regedit.GetValue(_APIVALUE, Nothing) = Nothing Then
                    API_TextBox.Text = regedit.GetValue(_APIVALUE, "")
                End If
                If Not regedit.GetValue(_SEEDVALUE, Nothing) = Nothing Then
                    Seeds_TextBox.Text = regedit.GetValue(_SEEDVALUE, "")
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Exception")
        End Try
    End Sub

    ''' <summary>
    ''' ´Metodo que guarda la configuración de los textboxes en el regedit
    ''' </summary>
    Private Sub SaveConfigurationIntoRegedit()
        ' Guardamos los valores de los textboxes en el regedit
        Try
            Dim newApiValue As String = API_TextBox.Text
            Dim newSeedValue As String = Seeds_TextBox.Text
            Dim regedit As RegistryKey = Registry.LocalMachine.CreateSubKey(_SUBKEY)  ' Comprobara si hay la subkey si no existe la crea y si no la abre

            ' Nos aseguramos que haya creado la subkey si no existe y si existe la haya abierto
            If Not regedit Is Nothing Then
                ' Si existen subtituye los valores si no los crea
                regedit.SetValue(_APIVALUE, newApiValue)
                regedit.SetValue(_SEEDVALUE, newSeedValue)

                ' Checkeamos que sea correcto, si es asi iniciamos el timer
                ConfigurationOk = RegeditOk()
                If ConfigurationOk And Not Timer.Enabled Then
                    IniciarTimer()
                End If
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Exception")
        End Try
    End Sub

    ''' <summary>
    ''' Method that show the form
    ''' </summary>
    Private Sub ShowForm()
        ' Mostramos el formulario y lo centramos en pantalla
        With Me
            .ShowInTaskbar = True
            .WindowState = FormWindowState.Normal
            .CenterToScreen()
        End With

        ' Obtenemos los datos de configuración y los cargamos en el formulario
        LoadConfigurationIntoForm()
    End Sub

    ''' <summary>
    ''' Method that hides the form
    ''' </summary>
    Private Sub HideForm()
        ' Escondemos el formulario
        With Me
            .WindowState = FormWindowState.Minimized
            .ShowInTaskbar = False
        End With
    End Sub

    ''' <summary>
    ''' Función que permite saber si existe el servicio pasado por valor
    ''' </summary>
    ''' <param name="name">Nombre del servicio</param>
    ''' <returns>True si existe el servicio ; False si no existe el servicio</returns>
    Private Function IsServiceInstalled(name As String) As Boolean
        Dim exist As Boolean = False
        Dim service As ServiceController = ServiceController.GetServices().FirstOrDefault(Function(s) s.ServiceName = _SERVICENAME)
        If Not service Is Nothing Then
            exist = True
        End If
        Return exist
    End Function

    ''' <summary>
    ''' Si nos cierran el formulario desde el botón a la hora de estar en la configuración, impedimos que se cierre la aplicación entera, solo minimizamos en la bandeja la aplicación
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        HideForm()
        e.Cancel = True
    End Sub


    ''' <summary>
    ''' Método que desinstalará el servicio Process Guard Service de nuestro sistema
    ''' </summary>
    Private Sub UninstallService()
        Dim query As String = String.Format("SELECT PathName FROM Win32_Service WHERE Name = '{0}'", _SERVICENAME)
        Dim ServicePath As String = ""
        Try
            Using mos As New ManagementObjectSearcher(query)
                For Each mo In mos.Get()
                    ServicePath = mo("PathName").ToString().Replace("""", "")
                Next
            End Using
            If File.Exists(_INSTALLUTIL) Then
                If File.Exists(ServicePath) Then
                    Dim arguments As String = String.Format("{0} {1}", ServicePath, "/u")
                    Process.Start(_INSTALLUTIL, arguments)
                    MsgBox(String.Format("Servicio {0} desinstalado con exito", _SERVICENAME))
                Else
                    MsgBox(String.Format("No existe la ruta {0} de instalación del servicio {1}", _SERVICENAME, ServicePath))
                End If
            Else
                MsgBox(String.Format("No se encuentra el archivo IntallUtil.exe en la dirección {0}, para desinstalar servicios necesitas la siguiente herramienta InstallUtil.exe", ServicePath), MsgBoxStyle.Critical, "Titulo")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Elimina las claves del registro si existen
    ''' </summary>
    Private Sub DeleteRegisterKeys()
        Dim regedit As RegistryKey = Registry.LocalMachine.OpenSubKey(_SUBKEY, True)

        ' Si la subclave existe entonces eliminamos su contenido (valores)
        If Not regedit Is Nothing Then
            If Not regedit.GetValue(_SEEDVALUE, Nothing) = Nothing Then
                regedit.DeleteValue(_SEEDVALUE)
            End If

            If Not regedit.GetValue(_APIVALUE, Nothing) = Nothing Then
                regedit.DeleteValue(_APIVALUE)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Función que retorna true si el regedit es correcto
    ''' </summary>
    ''' <returns></returns>
    Private Function RegeditOk() As Boolean
        Dim ok As Boolean = False
        Try
            Dim regedit As RegistryKey = Registry.LocalMachine.OpenSubKey(_SUBKEY)
            If Not regedit Is Nothing Then
                If Not regedit.GetValue(_APIVALUE, Nothing) = Nothing Then
                    If Not regedit.GetValue(_SEEDVALUE, Nothing) = Nothing Then
                        ok = True
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Excepció")
        End Try
        Return ok
    End Function

#End Region

#Region "CLICK ON MENU"

    ''' <summary>
    ''' Si nos seleccionan la opción de configuración, mostramos el formulario con los datos de la config cargados en textboxes
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ConfigToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfigToolStripMenuItem.Click
        ShowForm()
    End Sub


    ''' <summary>
    ''' Opción del menú que cierra la aplicación
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        NotifyIcon.Dispose()
        Close()
        Dispose()
    End Sub

    ''' <summary>
    ''' Opción menú para parar el servicio
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub StopServiceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopServiceToolStripMenuItem.Click
        Try
            ' Comprobamos si el servicio esta instalado
            ServiceInstalled = IsServiceInstalled(_SERVICENAME)

            If ServiceInstalled Then
                Service = New ServiceController(_SERVICENAME)
            End If

            ' Si el servicio esta parado lo encendemos
            If ((ServiceInstalled) AndAlso ((Service.Status = ServiceControllerStatus.Running) OrElse (Service.Status = ServiceControllerStatus.StartPending))) Then
                Service = New ServiceController(_SERVICENAME)
                Service.Refresh()
                Service.Stop()
            ElseIf Not ServiceInstalled Then
                MsgBox(String.Format("El servicio {0} no se encuentra instalado en el sistema", _SERVICENAME), MsgBoxStyle.Critical, "Error")
            Else
                MsgBox(String.Format("El servicio {0} ya se encuentra en estado pausado", _SERVICENAME), MsgBoxStyle.Information, "Information")
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Exception")
        End Try
    End Sub

    ''' <summary>
    ''' Opción menú para arrancar el servicio
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub StartServiceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartServiceToolStripMenuItem.Click
        Try
            ' Comprobamos si el servicio esta instalado
            ServiceInstalled = IsServiceInstalled(_SERVICENAME)

            If ServiceInstalled Then
                Service = New ServiceController(_SERVICENAME)
            End If

            If ((ServiceInstalled) AndAlso ((Service.Status = ServiceControllerStatus.Stopped) OrElse (Service.Status = ServiceControllerStatus.StopPending))) Then
                Service.Refresh()
                Service.Start()
            ElseIf Not ServiceInstalled Then
                MsgBox(String.Format("El servicio {0} no se encuentra instalado en el sistema", _SERVICENAME), MsgBoxStyle.Critical, "Error")
            Else
                MsgBox(String.Format("El servicio {0} ya esta en marcha", _SERVICENAME), MsgBoxStyle.Information, "Information")
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Exception")
        End Try
    End Sub

    ''' <summary>
    ''' Opción menú para desinstalar el servicio
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub UninstallToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UninstallToolStripMenuItem.Click
        Try
            ' Eliminamos los valores de la clave de registro de la aplicación
            DeleteRegisterKeys()
            ' Lanzamos la desinstalación del servicio como un nuevo thread
            Dim thread As New Threading.Thread(AddressOf UninstallService)
            thread.Start()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Excepción")
        End Try
    End Sub

#End Region


End Class
