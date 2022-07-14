Imports System.IO
Imports ThisIsADoomLauncher.Models

Imports Newtonsoft.Json

Friend Module Serialization

    Public Function LoadFromJsonData() As Setting
        Dim jsonData As String = File.ReadAllText(GetJsonFilepath())
        Dim storedSettings As Setting = JsonConvert.DeserializeObject(Of Setting)(jsonData)
        Return storedSettings
    End Function

    Public Sub SaveToJsonData(obj As Object)
        Dim jsonData As String = JsonConvert.SerializeObject(obj, Formatting.Indented)
        Dim filepath As String = GetJsonFilepath()
        File.WriteAllText(filepath, jsonData)
    End Sub

End Module