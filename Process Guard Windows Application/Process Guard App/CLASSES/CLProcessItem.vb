Imports Newtonsoft.Json

Public Class CLProcessItem

    Public exe As String
    Public filename As String
    Public description As String
    Public color As String
    Public colorNavigation As Object

    ' Principal constructor of the class
    Public Sub New()

    End Sub

    ' Second constructor of the class
    Public Sub New(xexe As String, xfile As String, xdescription As String, xcolor As String)
        exe = xexe
        filename = xfile
        description = xdescription
        color = xcolor
    End Sub

    ''' <summary>
    ''' Returns a string with the resume of the object
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Dim output As String = String.Format("EXE : {0}" & vbCrLf & "DESCRIPTION : {1}" & vbCrLf & "COLOR : {2}" & vbCrLf & "FILENAME : {3}" & vbCrLf, exe, description, color, filename)
        Return (output)
    End Function

    ''' <summary>
    ''' Returns the actual object in a string json format indented
    ''' </summary>
    ''' <returns></returns>
    Public Function ToJson() As String
        ' Retornamos el objeto actual con formato string transformado a json con el formato de sangrado correcto
        Return JsonConvert.SerializeObject(Me, Formatting.Indented)
    End Function

End Class

