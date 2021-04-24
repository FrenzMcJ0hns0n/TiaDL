Imports System.IO
Imports System.Web.Script.Serialization

Namespace Helpers
    Public Class Serialization


        Public Function GetEmbeddedResource(assembly As Reflection.Assembly, name As String) As String

            Dim buf As String = Nothing
            Using s As Stream = assembly.GetManifestResourceStream(name)

                Using sr As New StreamReader(s)
                    buf = sr.ReadToEnd
                End Using
            End Using
            Return buf

        End Function


        Public Shared Function GetEmbeddedResource() As String

            Dim ger As Serialization = New Serialization()
            ' ICI ----------------------------------------------------- > TODO : Obtenir le chemin (String) vers le fichier ressource
            Dim s As String = ger.GetEmbeddedResource(Reflection.Assembly.GetExecutingAssembly(), My.Resources.common_presets.ToString)

            'Dim a As String = GetEmbeddedResource(Reflection.Assembly.GetExecutingAssembly(), My.Resources.common_presets.ToString)
            'Return GetEmbeddedResource(Reflection.Assembly.GetExecutingAssembly(), My.Resources.common_presets.ToString)
            Return s

        End Function


        'Public Shared Function Deserialize(Of T)(resource As Byte()) As T

        '    Dim content = Convert.ToBase64String(resource)
        '    Dim jss As JavaScriptSerializer = New JavaScriptSerializer()

        '    Return jss.Deserialize(Of T)(content)

        'End Function

        Public Shared Function Deserialize(Of Preset)(resource As Byte()) As String

            Dim test As String = Text.Encoding.UTF8.GetString(resource)


            Dim jav As JavaScriptSerializer = New JavaScriptSerializer()

            Dim pre As Preset = jav.Deserialize(Of Preset)(test)

            'Dim cestca As Byte() = resource
            Return test

            'Dim obj As Preset = javScrSer.Deserialize(Of Preset)(resource)




            'Dim content = Convert.ToBase64String(resource)
            'Dim jss As JavaScriptSerializer = New JavaScriptSerializer()

            'Return jss.Deserialize(Of Preset)(content)

        End Function

    End Class
End Namespace