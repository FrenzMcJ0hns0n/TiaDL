Imports System.Net.Http
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace Helpers.DoomWorld
    Public Class DoomWorldService

        ''' <summary>
        ''' Get directories
        ''' </summary>
        ''' <param name="parentDirectory"> Parent directory. If empty, get root levels directories</param>
        ''' <returns></returns>
        Public Shared Async Function GetDirectories(parentDirectory As String) As Task(Of List(Of Object))
            Dim directories As List(Of Object) = New List(Of Object)

            Dim uriPath As String = "api.php?action=getdirs&name=levels/"
            Dim requestUri As Uri = New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, parentDirectory, "&out=json"))
            Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
            If response.IsSuccessStatusCode Then
                Dim folders As List(Of Object) = New List(Of Object)

                Dim jsonResult As String = Await response.Content.ReadAsStringAsync()

                Dim jsonObject = JObject.Parse(jsonResult)

                jsonObject.SelectToken("content.dir").ToList().ForEach(
                    Sub(directory) directories.Add(
                        New With {
                        .id = directory.Item("id"),
                        .name = directory.Item("name")}))


            End If
            Return directories

        End Function

        ''' <summary>
        ''' Get all Level information
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Shared Async Function GetLevel(id As String) As Task(Of Level)
            Dim uriPath As String = "api.php?action=get&id=" + id
            Dim requestUri As Uri = New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))
            Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
            If response.IsSuccessStatusCode Then

                Dim jsonResult As String = Await response.Content.ReadAsStringAsync()

                Dim jsonObject = JObject.Parse(jsonResult)

                Dim level = jsonObject.SelectToken("content")

                Return New Level() With
                {
                    .Id = level.Item("id").ToString(),
                    .Idgamesurl = level.Item("idgamesurl").ToString()
                }
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Downloads a level and extracts it in the levels directory
        ''' </summary>
        ''' <param name="idGamesUrl"></param>
        ''' <returns></returns>
        Public Shared Async Function DownloadLevel(idGamesUrl As String) As Task(Of Level)
            'TODO :
            '- go to level url
            '- scrap DL links
            '- handle DL
            '- extract zip in levels folder
        End Function
    End Class
End Namespace