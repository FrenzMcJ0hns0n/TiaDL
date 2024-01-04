﻿Imports System.IO
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

        Private Const HTTP As String = "HTTP"

        ''' <summary>
        ''' Gets DoomWorld Directories or Levels. This method could replace both GetDirectories and GetLevel. (needs more testing).
        ''' </summary>
        ''' <param name="resourcePath">The resource URI path. (i.e. doom2/a-c/ for folders, or doom2/a-c/av for Level Alien Vendetta)</param>
        ''' <returns>A list of Directories or Levels.</returns>
        Public Async Function GetContent(Optional resourcePath As String = "levels/") As Task(Of List(Of Object))

            Dim returnItems As New List(Of Object)

            Try
                Dim uriPath As String = String.Concat("api.php?action=getcontents&name=", resourcePath)
                Dim requestUri As New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))
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


            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {resourcePath}")
            End Try
            Return returnItems
        End Function

        ''' <summary>
        ''' Get DoomWorld levels directories.
        ''' </summary>
        ''' <param name="currentDirectory"> Current directory. If empty, get root levels directories.</param>
        ''' <returns></returns>
        Public Async Function GetDirectories(Optional currentDirectory As String = "") As Task(Of List(Of String))
            Dim directories As New List(Of String)

            Try
                Dim uriPath As String = "api.php?action=getdirs&name=levels/"
                Dim requestUri As New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, currentDirectory, "&out=json"))
                Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
                If response.IsSuccessStatusCode Then
                    Dim jsonObject As JObject = JObject.Parse(Await response.Content.ReadAsStringAsync())

                    If jsonObject.SelectToken("warning") IsNot Nothing Or jsonObject.SelectToken("error") IsNot Nothing Then
                        Throw New ArgumentException($"Parent directory : {currentDirectory} is not correct")
                    End If

                    jsonObject.SelectToken("content.dir").ToList().ForEach(
                        Sub(directory) directories.Add(directory.Value(Of String)("name"))
                    )

                End If

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {currentDirectory}")
            End Try

            Return directories
        End Function

        ''' <summary>
        ''' Get parent directory.
        ''' </summary>
        ''' <param name="currentDirectory"> Current directory.</param>
        ''' <returns></returns>
        Public Async Function GetParentDirectory(currentDirectory As String) As Task(Of Folder)
            Dim directory As Folder = Nothing

            Try
                Dim uriPath As String = "api.php?action=getparentdir&name="
                Dim requestUri As New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, currentDirectory, "&out=json"))
                Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
                If response.IsSuccessStatusCode Then
                    Dim jsonObject As JObject = JObject.Parse(Await response.Content.ReadAsStringAsync())

                    If jsonObject.SelectToken("warning") IsNot Nothing Or jsonObject.SelectToken("error") IsNot Nothing Then
                        Throw New ArgumentException($"Current directory : {currentDirectory} is not correct")
                    End If

                    Dim parentDirToken As JToken = jsonObject.SelectToken("content")

                    directory = New Folder With {
                        .Id = parentDirToken.Value(Of Long)("id"),
                        .Name = parentDirToken.Value(Of String)("name")
                    }

                End If

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {currentDirectory}")

                directory = Nothing
            End Try

            Return directory
        End Function

        ''' <summary>
        ''' Get all Level's information based on its ID.
        ''' </summary>
        ''' <param name="id">Level id.</param>
        ''' <returns>A Level</returns>
        Public Async Function GetLevel(id As Integer) As Task(Of Models.Level)
            Dim uriPath As String = String.Concat("api.php?action=get&id=", id)
            Dim requestUri As New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))
            Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
            If response.IsSuccessStatusCode Then

                Dim jsonResult As String = Await response.Content.ReadAsStringAsync()

                Dim jsonObject As JObject = JObject.Parse(jsonResult)

                Dim jLevel As JToken = jsonObject.SelectToken("content")

                Return CreateLevelFromJToken(jLevel)
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Search Levels based on FILENAME only (for the moment).
        ''' TODO : edit TYPE and SORT filters.
        ''' </summary>
        ''' <param name="searchText"></param>
        ''' <returns>A list of Level.</returns>
        Public Async Function SearchLevels(searchText As String) As Task(Of List(Of Models.Level))
            Dim levels As List(Of Models.Level)

            ' TODO : edit TYPE and SORT filters.
            Dim uriPath As String = String.Concat("api.php?action=search&query=", searchText, "&type=filename&sort=date")
            Dim requestUri As New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))
            Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)
            If response.IsSuccessStatusCode Then
                Dim jsonObject As JObject = JObject.Parse(Await response.Content.ReadAsStringAsync())

                If jsonObject.SelectToken("warning") IsNot Nothing Or jsonObject.SelectToken("error") IsNot Nothing Then
                    Throw New ArgumentException($"Search text : {searchText} returned no results")
                End If

                levels = New List(Of Models.Level)

                jsonObject.SelectToken("content.files").ToList().ForEach(
                    Sub(jLevel) levels.Add(CreateLevelFromJToken(jLevel))
                )

                Return levels
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Gets a mirror base URI from json file.
        ''' </summary>
        ''' <param name="name">Name of prefered mirror.</param>
        ''' <returns>A mirror base URI.</returns>
        Public Function GetMirror(name As String) As String
            Dim mirror As String
            Try
                Dim jsonMirrors As JObject = JObject.Parse(File.ReadAllText("doomworld_mirrors.json"))

                Dim mirrorToken As JToken = jsonMirrors.SelectToken($"{HTTP}.{name}")

                mirror = mirrorToken.Value(Of String)

                ' if ping : 
                ' - Dim p As System.Net.NetworkInformation.Ping = New Net.NetworkInformation.Ping
                ' - Dim pingResult As System.Net.NetworkInformation.PingReply = Await p.SendPingAsync(mirror)

            Catch ex As Exception
                mirror = String.Empty

                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {name}")
            End Try

            Return mirror
        End Function

        ''' <summary>
        ''' Gets all mirrors base URI from json file.
        ''' </summary>
        ''' <returns>All mirrors base URI.</returns>
        Public Function GetMirrors() As List(Of String)
            Dim mirrors As List(Of String)
            Try
                Dim jsonMirrors As JObject = JObject.Parse(File.ReadAllText("doomworld_mirrors.json"))

                Dim mirrorsTokens As List(Of JToken) = jsonMirrors.SelectToken(HTTP).ToList()

                mirrors = New List(Of String)
                mirrorsTokens.ForEach(
                    Sub(mirror) mirrors.Add(mirror.First.Value(Of String))
                )

            Catch ex As Exception
                mirrors = Nothing

                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : none")
            End Try

            Return mirrors
        End Function

        ''' <summary>
        ''' Downloads a .zip Level.
        ''' </summary>
        ''' <param name="levelUrl">The level download url.</param>
        ''' <returns>A .zip level archive.</returns>
        Public Async Function DownloadLevel(levelUrl As String) As Task(Of String)
            '
            ' Add level in a Doomworld registry file
            ' it will be easier to display Name, show Install folder, (open folder ?), Uninstall...
            ' 
            Try
                Dim requestUri As New Uri(levelUrl)
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
        ''' Extracts Level's files from zip archive in a folder.
        ''' </summary>
        ''' <param name="fileZipPath">path/filename.zip</param>
        ''' <returns>1 if folder is created; 0 if folder already exists; -1 if error.</returns>
        Public Async Function ExtractLevelFromZip(fileZipPath As String) As Task(Of Integer)
            Dim result As Integer = 0
            Try
                Dim levelZipUri As Uri = New Uri(fileZipPath)

                If Not File.Exists(levelZipUri.AbsolutePath) Then
                    Throw New FileNotFoundException
                End If

                Dim fileName As String = IOHelper.GetFileInfo_Name(fileZipPath, False)

                Dim fileNameFolderPathAfterExtraction As New Uri(String.Concat(IOHelper.GetFileInfo_Directory(fileZipPath), "/", fileName))

                If Directory.Exists(fileNameFolderPathAfterExtraction.AbsolutePath) Then
                    Return result
                End If

                Await Task.Run(
                Sub() ZipFile.ExtractToDirectory(levelZipUri.AbsolutePath, fileNameFolderPathAfterExtraction.AbsolutePath)
            )
                If Directory.Exists(fileNameFolderPathAfterExtraction.AbsolutePath) Then
                    result = 1
                End If
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {fileZipPath}")

                result = -1
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Move Level files in the matching directories.
        ''' </summary>
        ''' <param name="directoryName">Destination directory.</param>
        ''' <returns></returns>
        Public Async Function MoveFilesIntoDirectories(directoryName As String) As Task(Of Integer)
            ' Probably move this method to another file : 
            ' this file is DoomWorld only (read data, donwload DW mods).
            ' Sorting and moving files can be generically done for any mod.
            Dim result As Integer

            Try
                If Not Directory.Exists(directoryName) Then
                    Throw New DirectoryNotFoundException
                End If

                Dim files As List(Of String) = Directory.EnumerateFiles(directoryName).ToList

                Await Task.Run(
                    Sub()
                        For Each file As String In files
                            MoveToFolder(file)
                        Next
                        result = files.Count
                    End Sub
                )

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

        ''' <summary>
        ''' Creates a Level object from a JToken level
        ''' </summary>
        ''' <param name="jLevel">JToken level.</param>
        ''' <returns>A Level.</returns>
        Private Function CreateLevelFromJToken(jLevel As JToken) As Models.Level
            Dim level As New Models.Level With {
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
                .Bugs = jLevel.Value(Of String)("bugs"),
                .Credits = jLevel.Value(Of String)("credits"),
                .Base = jLevel.Value(Of String)("base"),
                .Buildtime = jLevel.Value(Of String)("buildtime"),
                .Editors = jLevel.Value(Of String)("editors"),
                .Rating = jLevel.Value(Of Decimal)("rating"),
                .Votes = jLevel.Value(Of Integer)("votes"),
                .Url = jLevel.Value(Of String)("url"),
                .Idgamesurl = jLevel.Value(Of String)("idgamesurl")
            }
            'Sometimes level doesn't have a Title, so filename is given
            If String.IsNullOrWhiteSpace(level.Title) Then
                level.Title = jLevel.Value(Of String)("filename")
            End If

            Return level
        End Function

        ''' <summary>
        ''' Creates a Folder object from a JToken Dir
        ''' </summary>
        ''' <param name="jFolder">JToken dir.</param>
        ''' <returns>A Folder.</returns>
        Private Function CreateFolderFromJToken(jFolder As JToken) As Models.Folder
            Return New Models.Folder With {
                .Id = jFolder.Value(Of Integer)("id"),
                .Name = jFolder.Value(Of String)("name")
            }
        End Function

        ''' <summary>
        ''' Temporary method to [create+]get destination folder for Level items.
        ''' </summary>
        ''' <param name="folder">Destination folder name.</param>
        ''' <returns>Folder's path.</returns>
        Private Function PROVISOIRE_GetFolder(folder As String) As String

            Dim folderPath As String = Path.Combine("test", folder)
            If Not (Directory.Exists(folderPath)) Then
                Directory.CreateDirectory(folderPath)
            End If

            Return folderPath
        End Function

        Public Function GetLevelDownloadUrl(level As Models.Level) As String
            Return String.Concat(level.Dir, level.Filename)
        End Function

        Private Function FormatLevelDLUri(idgamesurl As String) As String
            Try
                Return idgamesurl.Replace("idgames://", "")
            Catch ex As Exception
                Return Nothing

                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {idgamesurl}")
            End Try
        End Function

    End Class
End Namespace