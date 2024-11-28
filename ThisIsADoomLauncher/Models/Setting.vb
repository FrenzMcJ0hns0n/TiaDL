Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json.Schema

Namespace Models
    Public Class Setting

        'Private ReadOnly jsonSchema As String = My.Resources.SettingSchema

        Public Property Port As String
        Public Property PortParameters As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Public Property Iwad As String
        Public Property Maps As String
        Public Property Misc As String
        Public Property Mods As List(Of String) = New List(Of String)


        'Not used (yet?)
        'Public Function ValidateSchema() As Boolean
        '    Dim schema As JSchema = JSchema.Parse(jsonSchema)
        '    Dim setting As JObject = JObject.Parse(GetJsonData(FileLocation))

        '    Return setting.IsValid(schema)
        'End Function

    End Class
End Namespace