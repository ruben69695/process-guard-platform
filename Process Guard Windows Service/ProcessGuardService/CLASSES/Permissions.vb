Imports System.IO
Imports System.Security.AccessControl
Imports System.Security.Principal

''' <summary>
''' Módulo que nos permite acceder a funciones relacionadas con permisos en el sistema local en sistemas Microsoft Windows sin tener que instanciar la clase
''' </summary>
Module Permissions

    ''' <summary>
    ''' Función que retorna true si la aplicación esta siendo ejecutada bajo derechos de administrador
    ''' </summary>
    ''' <returns></returns>
    Public Function IsAdministrator() As Boolean
        Return (New WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator)
    End Function

    ''' <summary>
    ''' Método para cambiar los permisos de un directorio
    ''' </summary>
    ''' <param name="directory">Directorio al que alicar los nuevos permisos</param>
    ''' <param name="name">Nombre del usuario o grupo al que añadir en el directorio</param>
    ''' <param name="deleteAllSecurity">Si esta a true eliminara todos los permisos de los demás usuarios antes de aplicar los nuevos</param>
    ''' <param name="groupAdministrators">Si esta marcado como True, se añadira el grupo administradores al directorio</param>
    ''' <param name="lectura">Si esta marcado como True se añadiran permisos de lectura</param>
    ''' <param name="escriptura">Si esta marcado como True se añadiran permisos de escritura</param>
    ''' <param name="total">Si esta marcado como True se añadira control total</param>
    Public Sub CambiarPermisos(directory As String, name As String, deleteAllSecurity As Boolean, groupAdministrators As Boolean, lectura As Boolean, escriptura As Boolean, Optional total As Boolean = False)

        Dim xcarpeta_info As IO.DirectoryInfo = New IO.DirectoryInfo(directory)
        Dim xcarpeta_acl As New DirectorySecurity
        Dim xuser As String = Environment.MachineName.ToString & "\" & name

        Try
            If deleteAllSecurity Then
                xcarpeta_acl.SetAccessRuleProtection(True, False)       ' Elimina todos los permisos actuales del directorio
            Else
                ' Mantenemos los permisos que habían en el directorioGraci
                xcarpeta_acl = xcarpeta_info.GetAccessControl()
            End If

            If groupAdministrators Then
                Dim adminGroup As New System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, Nothing)
                xcarpeta_acl.AddAccessRule(New FileSystemAccessRule(adminGroup, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow))
            End If

            If total = False Then
                If lectura Then
                    xcarpeta_acl.AddAccessRule(New FileSystemAccessRule(xuser, FileSystemRights.ReadAndExecute, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow))
                End If
                If escriptura Then
                    xcarpeta_acl.AddAccessRule(New FileSystemAccessRule(xuser, FileSystemRights.Write, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow))
                End If
            Else
                xcarpeta_acl.AddAccessRule(New FileSystemAccessRule(xuser, FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow))
            End If
            xcarpeta_info.SetAccessControl(xcarpeta_acl)        ' Aplicamos sobre el directorio seleccionado con toda su información las nuevas reglas de control y seguridad
        Catch ex As Exception
            Console.WriteLine(ex.ToString())
        End Try

    End Sub

    ''' <summary>
    ''' Configura los permisos para la ruta semillas
    ''' </summary>
    ''' <param name="directory"></param>
    ''' <returns></returns>
    Public Function ConfigurarPermisosRutaSemilla(directory As String) As Boolean
        Dim ok As Boolean = False
        Try
            Dim xcarpeta_info As IO.DirectoryInfo
            Dim xcarpeta_acl As New DirectorySecurity
            Dim adminGroup As New System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, Nothing)
            Dim usuarios As New System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.BuiltinUsersSid, Nothing)
            Dim xsystem As New System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.LocalSystemSid, Nothing)

            xcarpeta_info = New IO.DirectoryInfo(directory)
            xcarpeta_acl.SetAccessRuleProtection(True, False)       ' Elimina todos los permisos actuales del directorio

            'InheritanceFlags.ObjectInherit -> Los objetos hoja secundarios heredan la ACE.
            xcarpeta_acl.AddAccessRule(New FileSystemAccessRule(adminGroup, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow))
            xcarpeta_acl.AddAccessRule(New FileSystemAccessRule(usuarios, FileSystemRights.Read, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow))
            xcarpeta_acl.AddAccessRule(New FileSystemAccessRule(xsystem, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow))
            xcarpeta_info.SetAccessControl(xcarpeta_acl)        ' Aplicamos sobre el directorio seleccionado con toda su información las nuevas reglas de control y seguridad
            ok = True
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        Return ok
    End Function

End Module
