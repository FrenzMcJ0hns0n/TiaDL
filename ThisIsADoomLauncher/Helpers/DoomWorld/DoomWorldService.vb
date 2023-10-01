Imports System.IO
Imports System.IO.Compression
Imports System.Net.Http
Imports System.Reflection
Imports System.Web.Hosting
Imports System.Windows.Shell
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace Helpers.DoomWorld
    Public Class DoomWorldService

        Private Const XPATH_URL_NODES As String = "/html/body/table/tr[2]/td/table/tr/td[2]/table/tr/td/ul[1]/li"

        ''' <summary>
        ''' Get directories
        ''' </summary>
        ''' <param name="parentDirectory"> Parent directory. If empty, get root levels directories</param>
        ''' <returns></returns>
        Public Async Function GetDirectories(Optional parentDirectory As String = "") As Task(Of List(Of String))
            Dim directories As New List(Of String)

            Try
                Dim uriPath As String = "api.php?action=getdirs&name=levels/"
                Dim requestUri As Uri = New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, parentDirectory, "&out=json"))
                Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
                If response.IsSuccessStatusCode Then
                    Dim jsonObject As JObject = JObject.Parse(Await response.Content.ReadAsStringAsync())

                    If jsonObject.SelectToken("warning") IsNot Nothing Or jsonObject.SelectToken("error") IsNot Nothing Then
                        Throw New ArgumentException($"Parent directory : {parentDirectory} is not correct")
                    End If

                    jsonObject.SelectToken("content.dir").ToList().ForEach(
                        Sub(directory) directories.Add(directory.Value(Of String)("name"))
                    )

                End If

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {parentDirectory}")
            End Try

            Return directories
        End Function

        ''' <summary>
        ''' Get all Level information
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Async Function GetLevel(id As Integer) As Task(Of Level)
            Dim uriPath As String = String.Concat("api.php?action=get&id=", id)
            Dim requestUri As Uri = New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))
            Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
            If response.IsSuccessStatusCode Then

                Dim jsonResult As String = Await response.Content.ReadAsStringAsync()

                Dim jsonObject As JObject = JObject.Parse(jsonResult)

                Dim level As JToken = jsonObject.SelectToken("content")

                Return New Level() With
                {
                    .Id = level.Value(Of Integer)("id"),
                    .Idgamesurl = level.Value(Of String)("idgamesurl")
                }
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Get Level download links
        ''' </summary>
        ''' <param name="levelUrl">The level url</param>
        ''' <returns></returns>
        Public Async Function GetLevelDownloadLinks(levelUrl As String) As Task(Of List(Of String))
            Dim downloadLinks As New List(Of String)
            Try

                Dim requestUri As Uri = New Uri(levelUrl)
                Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
                If response.IsSuccessStatusCode Then

                    Dim htmlResult As String = Await response.Content.ReadAsStringAsync()

                    Dim html As HtmlAgilityPack.HtmlDocument = New HtmlAgilityPack.HtmlDocument
                    html.LoadHtml(htmlResult)

                    Dim htmlDocument As HtmlAgilityPack.HtmlNode = html.DocumentNode
                    Dim dlUrls As HtmlAgilityPack.HtmlNodeCollection = htmlDocument.SelectNodes(XPATH_URL_NODES)

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
        Public Async Function DownloadLevel(levelUrl As String) As Task(Of String)
            Try
                Dim requestUri As Uri = New Uri(levelUrl)
                Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)

                Dim levelFileName As String = requestUri.Segments.Last()
                Using fileStream As New FileStream(levelFileName, FileMode.OpenOrCreate)
                    Await response.Content.CopyToAsync(fileStream)
                End Using

                Return levelFileName

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {levelUrl}")

                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Extracts Level's files from zip archive in a folder
        ''' </summary>
        ''' <param name="fileNameZip">filename.zip</param>
        ''' <returns>1 if folder is created; 0 if folder already exists; -1 if error.</returns>
        Public Async Function ExtractLevelFromZip(directoryPath As String, fileNameZip As String) As Task(Of Integer)
            Dim result As Integer = 0
            Try
                Dim levelZipUri As Uri = New Uri(String.Concat(directoryPath, "/", fileNameZip))

                If Not File.Exists(levelZipUri.AbsolutePath) Then
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {directoryPath}, {fileNameZip}")

                result = -1
            End Try

            Return result
        End Function

        ' Probably move this method to another file : 
        ' this file is DoomWorld only (read data, donwload DW mods).
        ' Sorting and moving files can be generically done for any mod.
        Public Async Function MoveFilesIntoDirectories(directoryName As String) As Task(Of Integer)
            Dim result As Integer

            Try
                If Not Directory.Exists(directoryName) Then
                    Throw New DirectoryNotFoundException
                End If

                Dim files As List(Of String) = Directory.EnumerateFiles(directoryName).ToList

                Await Task.Run(Sub()
                                   For Each file As String In files
                                       MoveToFolder(file)
                                   Next
                                   result = files.Count
                               End Sub)

            Catch ex As Exception
                result = -1
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {directoryName}")
            End Try

            Return result
        End Function

        Private Sub MoveToFolder(currentFile As String)

            ' !! TiaDL integration : replace method PROVISOIRE_GetFolder("*") by GetDirectoryPath("*")

            Dim currentFileInfo As New FileInfo(currentFile)
            Dim destinationDirectory As DirectoryInfo
            If Constants.VALID_EXTENSIONS_MAPS.Contains(currentFileInfo.Extension) Then
                destinationDirectory = Directory.CreateDirectory(Path.Combine(PROVISOIRE_GetFolder("Maps"), currentFileInfo.Directory.Name))
                File.Move(currentFile, Path.Combine(destinationDirectory.FullName, currentFileInfo.Name))
            ElseIf Constants.VALID_EXTENSIONS_MISC.Contains(currentFileInfo.Extension) Then
                destinationDirectory = Directory.CreateDirectory(Path.Combine(PROVISOIRE_GetFolder("Misc"), currentFileInfo.Directory.Name))
                File.Move(currentFile, Path.Combine(destinationDirectory.FullName, currentFileInfo.Name))
            ElseIf Constants.VALID_EXTENSIONS_MODS.Contains(currentFileInfo.Extension) Then
                destinationDirectory = Directory.CreateDirectory(Path.Combine(PROVISOIRE_GetFolder("Mods"), currentFileInfo.Directory.Name))
                File.Move(currentFile, Path.Combine(destinationDirectory.FullName, currentFileInfo.Name))
            Else
                destinationDirectory = Directory.CreateDirectory(Path.Combine(PROVISOIRE_GetFolder("Pict"), currentFileInfo.Directory.Name))
                File.Move(currentFile, Path.Combine(destinationDirectory.FullName, currentFileInfo.Name))
            End If
        End Sub

        Private Function PROVISOIRE_GetFolder(folder As String) As String

            Dim folderPath As String = Path.Combine("test", folder)
            If Not (Directory.Exists(folderPath)) Then
                Directory.CreateDirectory(folderPath)
            End If

            Return folderPath
        End Function
    End Class
End Namespace