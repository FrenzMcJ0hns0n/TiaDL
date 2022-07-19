Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json.Schema

Namespace Models
    Public Class Setting

        Private ReadOnly jsonSchema As String = My.Resources.SettingSchema

        <JsonIgnore>
        Public Property FileLocation As String

        Public Property Port As String
        Public Property PortParameters As List(Of String) = New List(Of String)
        Public Property Iwad As String
        Public Property Level As String
        Public Property Misc As String
        Public Property Mods As List(Of String) = New List(Of String)

        Public Sub New(filepath As String)
            FileLocation = filepath
        End Sub

        Public Function CanLoad() As Boolean
            Try
                JObject.Parse(GetJsonData(FileLocation))
            Catch jrEx As JsonReaderException
                Return False
            End Try

            Return True
        End Function

        Public Sub Load()
            Dim jsonData As String = GetJsonData(FileLocation)

            Dim settings As Setting = JsonConvert.DeserializeObject(Of Setting)(jsonData)
            With settings
                Port = .Port
                PortParameters = .PortParameters
                Iwad = .Iwad
                Level = .Level
                Misc = .Misc
                Mods = .Mods
            End With
        End Sub

        Public Sub Save()
            Dim jsonData As String = JsonConvert.SerializeObject(Me, Formatting.Indented)

            PersistJsonData(FileLocation, jsonData)
        End Sub

        'Not used (yet?)
        Public Function ValidateSchema() As Boolean
            Dim schema As JSchema = JSchema.Parse(jsonSchema)
            Dim setting As JObject = JObject.Parse(GetJsonData(FileLocation))

            Return setting.IsValid(schema)
        End Function

    End Class
End Namespace