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
                        Key .id = directory.Item("id"),
                        Key .name = directory.Item("name")}))


            End If
            Return directories

        End Function

    End Class
End Namespace