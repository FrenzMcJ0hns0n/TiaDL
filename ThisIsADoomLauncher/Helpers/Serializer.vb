Imports System.Reflection
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports ThisIsADoomLauncher.Models

''' <summary>
''' Helper to serialize/deserialize TiaDL objects with JSON
''' </summary>
Friend Module Serializer

    Public Function CanLoadJsonArray(jsonArr As String) As Boolean
        Try
            JArray.Parse(jsonArr)
        Catch jrEx As JsonReaderException
            Return False
        End Try

        Return True
    End Function

    Public Function CanLoadJsonObject(jsonObj As String) As Boolean
        Try
            JObject.Parse(jsonObj)
        Catch jrEx As JsonReaderException
            Return False
        End Try

        Return True
    End Function

    Public Function LoadUserLevels(jsonString As String) As List(Of LevelPreset)
        Dim userLevels As New List(Of LevelPreset)

        Try
            userLevels = JsonConvert.DeserializeObject(Of List(Of LevelPreset))(jsonString)

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
        End Try

        Return userLevels
    End Function

    Public Function LoadUserMods(jsonString As String) As List(Of ModPreset)
        Dim userLevels As New List(Of ModPreset)

        Try
            userLevels = JsonConvert.DeserializeObject(Of List(Of ModPreset))(jsonString)

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
        End Try

        Return userLevels
    End Function

    Public Sub SaveUserLevels(levelPresets As List(Of LevelPreset), filepath As String)
        Try
            Dim jsonData As String = JsonConvert.SerializeObject(levelPresets, Formatting.Indented)
            PersistJsonData(filepath, jsonData)

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            Dim presetNames As List(Of String) = levelPresets.Select(Function(lp As LevelPreset) lp.Name).ToList
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : Preset names = {String.Join(", ", presetNames)}")
        End Try
    End Sub

    Public Sub SaveUserMods(modPresets As List(Of ModPreset), filepath As String)
        Try
            Dim jsonData As String = JsonConvert.SerializeObject(modPresets, Formatting.Indented)
            PersistJsonData(filepath, jsonData)

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            Dim presetNames As List(Of String) = modPresets.Select(Function(mp As ModPreset) mp.Name).ToList
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : Preset names = {String.Join(", ", presetNames)}")
        End Try
    End Sub

End Module
