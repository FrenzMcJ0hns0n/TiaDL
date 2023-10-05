Imports System.IO
Imports System.IO.Compression
Imports System.Net.Http
Imports System.Reflection
Imports System.Text
Imports System.Web.Hosting
Imports System.Windows.Shell
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models

Namespace Helpers.DoomWorld
    Public Class DoomWorldService

        Private Const XPATH_URL_NODES As String = "/html/body/table/tr[2]/td/table/tr/td[2]/table/tr/td/ul[1]/li"


        Public Async Function GetContent(Optional resourcePath As String = "") As Task(Of List(Of Object))

            Dim returnItems As New List(Of Object)

            Dim uriPath As String = String.Concat("api.php?action=getcontents&name=levels/", resourcePath)
            Dim requestUri As Uri = New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))
            Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
            If response.IsSuccessStatusCode Then

                Dim jsonObject As JObject = JObject.Parse(Await response.Content.ReadAsStringAsync())

                If jsonObject.SelectToken("warning") IsNot Nothing Or jsonObject.SelectToken("error") IsNot Nothing Then
                    Throw New ArgumentException($"Invalid path: {resourcePath} is not correct")
                End If

                If jsonObject.SelectToken("content.file") Is Nothing And jsonObject.SelectToken("dir") Is Nothing Then
                    Throw New ArgumentException($"No results")
                End If

                jsonObject.SelectToken("content.dir")?.ToList().ForEach(
                    Sub(jFolder) returnItems.Add(CreateFolderFromJToken(jFolder))
                )

                jsonObject.SelectToken("content.file")?.ToList().ForEach(
                Sub(jLevel) returnItems.Add(CreateLevelFromJToken(jLevel))
            )
            End If

            Return returnItems
        End Function

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
        Public Async Function GetLevel(id As Integer) As Task(Of Models.Level)
            Dim uriPath As String = String.Concat("api.php?action=get&id=", id)
            Dim requestUri As Uri = New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))
            Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
            If response.IsSuccessStatusCode Then

                Dim jsonResult As String = Await response.Content.ReadAsStringAsync()

                Dim jsonObject As JObject = JObject.Parse(jsonResult)

                Dim level As JToken = jsonObject.SelectToken("content")

                Return New Models.Level() With
                {
                    .Id = level.Value(Of Integer)("id"),
                    .Idgamesurl = level.Value(Of String)("idgamesurl")
                }
            End If

            Return Nothing
        End Function

        Public Async Function SearchLevels(searchText As String) As Task(Of List(Of Models.Level))
            Dim levels As List(Of Models.Level)

            Dim uriPath As String = String.Concat("api.php?action=search&query=", searchText, "&type=filename&sort=date")
            Dim requestUri As Uri = New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))
            Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
            If response.IsSuccessStatusCode Then
                Dim jsonObject As JObject = JObject.Parse(Await response.Content.ReadAsStringAsync())

                If jsonObject.SelectToken("warning") IsNot Nothing Or jsonObject.SelectToken("error") IsNot Nothing Then
                    Throw New ArgumentException($"Search text : {searchText} returned no results")
                End If

                levels = New List(Of Models.Level)

                jsonObject.SelectToken("content.files").ToList().ForEach(
                    Sub(jLevel)
                        levels.Add(CreateLevelFromJToken(jLevel))
                    End Sub
                    )

                Return levels
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Gets a mirror root URI from json file. It can be either FTP or HTTP.
        ''' </summary>
        ''' <param name="protocol">FTP or HTTP from Protocol (Enum).</param>
        ''' <param name="name">Name of prefered mirror.</param>
        ''' <returns></returns>
        Public Function GetMirror(protocol As Helpers.DoomWorld.Models.Protocol, name As String) As String
            Dim mirror As String
            Try
                Dim jsonMirrors As JObject = JObject.Parse(File.ReadAllText("doomworld_mirrors.json"))

                Dim mirrorToken As JToken = jsonMirrors.SelectToken($"{protocol.ToString()}.{name}")

                mirror = mirrorToken.Value(Of String)

                ' if ping : 
                ' - Dim p As System.Net.NetworkInformation.Ping = New Net.NetworkInformation.Ping
                ' - Dim pingResult As System.Net.NetworkInformation.PingReply = Await p.SendPingAsync(mirror)

            Catch ex As Exception
                mirror = String.Empty

                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {protocol}, {name}")
            End Try

            Return mirror
        End Function

        Public Function GetMirrors(protocol As Helpers.DoomWorld.Models.Protocol) As List(Of String)
            Dim mirrors As List(Of String)
            Try
                Dim jsonMirrors As JObject = JObject.Parse(File.ReadAllText("doomworld_mirrors.json"))

                Dim mirrorsTokens As List(Of JToken) = jsonMirrors.SelectToken($"{protocol.ToString()}").ToList()

                mirrors = New List(Of String)
                mirrorsTokens.ForEach(Sub(mirror)
                                          mirrors.Add(mirror.First.Value(Of String))
                                      End Sub)

                ' if ping : 
                ' - Dim p As System.Net.NetworkInformation.Ping = New Net.NetworkInformation.Ping
                ' - Dim pingResult As System.Net.NetworkInformation.PingReply = Await p.SendPingAsync(mirror)

            Catch ex As Exception
                mirrors = Nothing

                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {protocol}")
            End Try

            Return mirrors
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
            '
            ' Add level in a Doomworld registry file
            ' it will be easier to display Name, show Install folder, (open folder ?), Uninstall...
            ' 
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

                If Not IO.File.Exists(levelZipUri.AbsolutePath) Then
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
                IO.File.Move(currentFile, Path.Combine(destinationDirectory.FullName, currentFileInfo.Name))
            ElseIf Constants.VALID_EXTENSIONS_MISC.Contains(currentFileInfo.Extension) Then
                destinationDirectory = Directory.CreateDirectory(Path.Combine(PROVISOIRE_GetFolder("Misc"), currentFileInfo.Directory.Name))
                IO.File.Move(currentFile, Path.Combine(destinationDirectory.FullName, currentFileInfo.Name))
            ElseIf Constants.VALID_EXTENSIONS_MODS.Contains(currentFileInfo.Extension) Then
                destinationDirectory = Directory.CreateDirectory(Path.Combine(PROVISOIRE_GetFolder("Mods"), currentFileInfo.Directory.Name))
                IO.File.Move(currentFile, Path.Combine(destinationDirectory.FullName, currentFileInfo.Name))
            Else
                destinationDirectory = Directory.CreateDirectory(Path.Combine(PROVISOIRE_GetFolder("Pict"), currentFileInfo.Directory.Name))
                IO.File.Move(currentFile, Path.Combine(destinationDirectory.FullName, currentFileInfo.Name))
            End If
        End Sub

        Private Function CreateLevelFromJToken(jLevel As JToken) As Models.Level
            Return New Models.Level With {
                                               .Id = jLevel.Value(Of Integer)("id"),
                                               .Title = jLevel.Value(Of String)("title"),
                                               .Dir = jLevel.Value(Of String)("dir"),
                                               .Filename = jLevel.Value(Of String)("filename"),
                                               .Size = jLevel.Value(Of Long)("size"),
                                               .Age = jLevel.Value(Of Long)("age"),
                                               .ReleaseDate = jLevel.Value(Of String)("date"),
                                               .Author = jLevel.Value(Of String)("author"),
                                               .Email = jLevel.Value(Of String)("email"),
                                               .Description = jLevel.Value(Of String)("description"),
                                               .Rating = jLevel.Value(Of Decimal)("rating"),
                                               .Votes = jLevel.Value(Of Integer)("votes"),
                                               .Url = jLevel.Value(Of String)("url"),
                                               .Idgamesurl = jLevel.Value(Of String)("idgamesurl")
                                               }
        End Function

        Private Function CreateFolderFromJToken(jFolder As JToken) As Models.Folder
            Return New Models.Folder With {
                                               .Id = jFolder.Value(Of Integer)("id"),
                                               .Name = jFolder.Value(Of String)("name")
                                               }
        End Function

        Private Function PROVISOIRE_GetFolder(folder As String) As String

            Dim folderPath As String = Path.Combine("test", folder)
            If Not (Directory.Exists(folderPath)) Then
                Directory.CreateDirectory(folderPath)
            End If

            Return folderPath
        End Function
    End Class
End Namespace