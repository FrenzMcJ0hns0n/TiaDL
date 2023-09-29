﻿Imports System.IO
Imports System.IO.Compression
Imports System.Net.Http
Imports System.Reflection
Imports System.Web.Hosting
Imports System.Windows.Shell
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

                If jsonObject.SelectToken("warning") IsNot Nothing Or jsonObject.SelectToken("error") IsNot Nothing Then
                    Throw New ArgumentException($"Parent directory : {parentDirectory} is not correct")
                End If

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
        Public Shared Async Function GetLevel(id As Integer) As Task(Of Level)
            Dim uriPath As String = String.Concat("api.php?action=get&id=", id)
            Dim requestUri As Uri = New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))
            Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
            If response.IsSuccessStatusCode Then

                Dim jsonResult As String = Await response.Content.ReadAsStringAsync()

                Dim jsonObject = JObject.Parse(jsonResult)

                Dim level = jsonObject.SelectToken("content")

                Return New Level() With
                {
                    .Id = Integer.Parse(level.Item("id")),
                    .Idgamesurl = level.Item("idgamesurl").ToString()
                }
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Get Level download links
        ''' </summary>
        ''' <param name="levelUrl">The level url</param>
        ''' <returns></returns>
        Public Shared Async Function GetLevelDownloadLinks(levelUrl As String) As Task(Of List(Of String))
            Dim downloadLinks As List(Of String)
            Try

                Dim requestUri As Uri = New Uri(levelUrl)
                Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
                If response.IsSuccessStatusCode Then

                    downloadLinks = New List(Of String)

                    Dim htmlResult As String = Await response.Content.ReadAsStringAsync()

                    Dim html As HtmlAgilityPack.HtmlDocument = New HtmlAgilityPack.HtmlDocument
                    html.LoadHtml(htmlResult)

                    Dim htmlDocument As HtmlAgilityPack.HtmlNode = html.DocumentNode
                    Dim dlUrls As HtmlAgilityPack.HtmlNodeCollection = htmlDocument.SelectNodes("/html/body/table/tr[2]/td/table/tr/td[2]/table/tr/td/ul[1]/li")

                    For Each dlUrl As HtmlAgilityPack.HtmlNode In dlUrls
                        downloadLinks.Add(dlUrl.SelectSingleNode("a").Attributes.FirstOrDefault(Function(a) a.Name = "href").Value)
                    Next
                End If
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {levelUrl}")
            End Try

            Return downloadLinks
        End Function

        ''' <summary>
        ''' Downloads a level
        ''' </summary>
        ''' <param name="levelUrl">The level download url</param>
        ''' <returns></returns>
        Public Shared Async Function DownloadLevel(levelUrl As String) As Task(Of String)
            Try
                Dim requestUri As Uri = New Uri(levelUrl)
                Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)

                Dim levelFileName As String = requestUri.Segments.Last()
                Using fileStream As New FileStream(levelFileName, FileMode.OpenOrCreate)
                    'HostingEnvironment.MapPath("~/lol") + requestUri.Segments.Last()
                    Await response.Content.CopyToAsync(fileStream)
                End Using

                Return levelFileName
            Catch ex As Exception

                Return False
            End Try
        End Function

        ''' <summary>
        ''' Extracts Level's files from zip archive in a folder
        ''' </summary>
        ''' <param name="fileNameZip"></param>
        ''' <returns></returns>
        Public Shared Async Function ExtractLevelFromZip(directoryPath As String, fileNameZip As String) As Task(Of Integer)
            Dim result As Integer = 0
            Try
                Dim levelZipUri As Uri = New Uri(String.Concat(directoryPath, "/", fileNameZip))

                If Not System.IO.File.Exists(levelZipUri.AbsolutePath) Then
                    Throw New FileNotFoundException
                End If

                Dim fileName As String = fileNameZip.Split("."c).First()

                Dim fileNameFolderPathAfterExtraction As New Uri(String.Concat(directoryPath, "/", fileName))

                If Directory.Exists(fileNameFolderPathAfterExtraction.AbsolutePath) Then
                    Return result
                End If

                Await Task.Run(Sub()
                                   System.IO.Compression.ZipFile.ExtractToDirectory(levelZipUri.AbsolutePath, fileNameFolderPathAfterExtraction.AbsolutePath)
                               End Sub)
                If Directory.Exists(fileNameFolderPathAfterExtraction.AbsolutePath) Then
                    result = 1
                End If
            Catch ex As Exception
                Dim s As String = ex.Message
                result = -1
            End Try

            Return result
        End Function
    End Class
End Namespace