Imports Newtonsoft.Json

Namespace Models
    Public Class Setting

        Public Property Port As String
        Public Property PortParameters As List(Of String) = New List(Of String)
        Public Property Iwad As String
        Public Property Level As String
        Public Property Misc As String
        Public Property Mods As List(Of String) = New List(Of String)

        Public Sub Load()
            Dim jsonData As String = GetJsonData(GetJsonFilepath("Settings"))

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

            Dim filepath As String = GetJsonFilepath("Settings")
            PersistJsonData(filepath, jsonData)
        End Sub

    End Class
End Namespace