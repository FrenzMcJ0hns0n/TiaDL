Imports System.IO
Imports System.IO.Compression
Imports System.Net.Http
Imports System.Reflection
Imports System.Text
Imports System.Web.Hosting
Imports System.Web.ModelBinding
Imports System.Windows.Shell
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models

Namespace Helpers.DoomWorld
    Public Class DoomWorldService

        Private Const HTTP As String = "HTTP"
        Private Const DOOMWORLD_REGISTRY_FILE As String = "doomworld_registry.json"
        '↓ TODO : Get from a user file
        Private Const DOOMWORLD_MIRRORS_FILE As String = "doomworld_mirrors.json"

        ''' <summary>
        ''' Gets DoomWorld Directories or Levels. 
        ''' <br> This method can replace both GetDirectories and GetLevel. (but returns not all Level properties).</br>
        ''' </summary>
        ''' <param name="resourcePath">The resource URI path. (i.e. doom2/a-c/ for folders, or doom2/a-c/av for Level Alien Vendetta)</param>
        ''' <returns>A list of Object (Directories or Levels).</returns>
        Public Async Function GetContents(Optional resourcePath As String = "levels/") As Task(Of List(Of Object))
            Dim contents As New List(Of Object)

            Try
                Dim action As String = "getcontents"
                Dim params As New List(Of String) From {$"name={resourcePath}"}
                Dim apiRequest As New ApiRequestManager(action, params)
                Dim jsonResponse As JObject = Await apiRequest.FetchJsonResponse()

                With jsonResponse
                    If .SelectToken("warning") IsNot Nothing Then : Throw New Exception($"Warning: { .SelectToken("warning.message") }")
                    ElseIf .SelectToken("error") IsNot Nothing Then : Throw New Exception($"Error: { .SelectToken("error.message") }")
                    End If

                    If .SelectToken("content.file") Is Nothing AndAlso .SelectToken("content.dir") Is Nothing Then
                        Throw New Exception($"No contents returned for name ""{resourcePath}""")
                    End If

                    .SelectToken("content.dir")?.ToList().ForEach(
                        Sub(jFolder) contents.Add(CreateFolderFromJToken(jFolder))
                    )

                    .SelectToken("content.file")?.ToList().ForEach(
                        Sub(jLevel) contents.Add(CreateLevelFromJToken(jLevel))
                    )
                End With

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {resourcePath}")
            End Try

            Return contents
        End Function

        ''' <summary>
        ''' Get DoomWorld levels directories.
        ''' </summary>
        ''' <param name="currentDirectory"> Current directory. If empty, get root levels directories.</param>
        ''' <returns></returns>
        Public Async Function GetDirectories(Optional currentDirectory As String = "") As Task(Of List(Of String))
            Dim directories As New List(Of String)

            Try
                Dim action As String = "getdirs"
                Dim params As New List(Of String) From {$"name=levels/{currentDirectory}"}
                Dim apiRequest As New ApiRequestManager(action, params)
                Dim jsonResponse As JObject = Await apiRequest.FetchJsonResponse()

                'Dim uriPath As String = "api.php?action=getdirs&name=levels/"
                'Dim requestUri As New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, currentDirectory, "&out=json"))

                With jsonResponse
                    If .SelectToken("warning") IsNot Nothing Then : Throw New Exception($"Warning: { .SelectToken("warning.message") }")
                    ElseIf .SelectToken("error") IsNot Nothing Then : Throw New Exception($"Error: { .SelectToken("error.message") }")
                    End If

                    .SelectToken("content.dir").ToList().ForEach(
                        Sub(directory) directories.Add(directory.Value(Of String)("name"))
                    )
                End With

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
                Dim action As String = "getparentdir"
                Dim params As New List(Of String) From {$"name={currentDirectory}"}
                Dim apiRequest As New ApiRequestManager(action, params)
                Dim jsonResponse As JObject = Await apiRequest.FetchJsonResponse()

                'Dim uriPath As String = "api.php?action=getparentdir&name="
                'Dim requestUri As New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, currentDirectory, "&out=json"))
                With jsonResponse
                    If .SelectToken("warning") IsNot Nothing Then : Throw New Exception($"Warning: { .SelectToken("warning.message") }")
                    ElseIf .SelectToken("error") IsNot Nothing Then : Throw New Exception($"Error: { .SelectToken("error.message") }")
                    End If

                    Dim parentDirToken As JToken = .SelectToken("content")

                    directory = New Folder With {
                        .Id = parentDirToken.Value(Of Long)("id"),
                        .Name = parentDirToken.Value(Of String)("name")
                    }
                End With

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {currentDirectory}")
            End Try

            Return directory
        End Function

        ''' <summary>
        ''' Get all Level's information based on its ID.
        ''' </summary>
        ''' <param name="id">Level id.</param>
        ''' <returns>A Level</returns>
        Public Async Function GetLevel(id As Integer) As Task(Of Models.Level)
            Dim level As Models.Level = Nothing

            Try
                Dim action As String = "get"
                Dim params As New List(Of String) From {$"id={id}"}
                Dim apiRequest As New ApiRequestManager(action, params)
                Dim strResponse As String = Await apiRequest.FetchStringResponse()

                'Dim uriPath As String = String.Concat("api.php?action=get&id=", id)
                'Dim requestUri As New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))
                Dim jsonObject As JObject = JObject.Parse(Helpers.DoomWorld.HtmlCleaner.HtmlToPlainText(strResponse))
                Dim jLevel As JToken = jsonObject.SelectToken("content")
                level = CreateLevelFromJToken(jLevel)
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {id}")
            End Try
            Return level
        End Function

        ''' <summary>
        ''' Search Levels
        ''' TODO: Set "type" as function input param
        ''' </summary>
        ''' <param name="searchText"></param>
        ''' <returns>A list of Level.</returns>
        Public Async Function SearchLevels(searchText As String) As Task(Of List(Of Models.Level))
            Dim levels As New List(Of Models.Level)

            Try
                Dim action As String = "search"
                Dim params As New List(Of String) From {$"query={searchText}", "type=title"}
                Dim apiRequest As New ApiRequestManager(action, params)
                Dim jsonResponse As JObject = Await apiRequest.FetchJsonResponse()
                'Dim uriPath As String = String.Concat("api.php?action=search&query=", searchText, "&type=title")
                'Dim requestUri As New Uri(String.Concat(DoomWorldHttpClient.BASE_URL, uriPath, "&out=json"))

                With jsonResponse
                    If .SelectToken("warning") IsNot Nothing Then
                        If .SelectToken("warning.type").Value(Of String) = "Limit Reached" Then
                            Throw New OverflowException("Search query returned too many results.")
                        End If
                        Throw New Exception($"Warning: { .SelectToken("warning.message") }")

                    ElseIf .SelectToken("error") IsNot Nothing Then : Throw New Exception($"Error: { .SelectToken("error.message") }")
                    End If

                    'If the 1st element is of type JObject, then we have a list of Level entities
                    'TODO: Create a Boolean for clearness AND/OR Use more specialized Newtonsoft methods
                    Dim listResultLevels As List(Of JToken) = .SelectToken("content.file").ToList() 'listResultLevels
                    If listResultLevels.FirstOrDefault().GetType() Is GetType(JObject) Then
                        listResultLevels.ForEach(
                            Sub(jLevel) levels.Add(CreateLevelFromJToken(jLevel))
                        )
                    Else
                        Dim resultLevel As JToken = .SelectToken("content.file")
                        levels.Add(CreateLevelFromJToken(resultLevel))
                    End If
                End With

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {searchText}")
                Throw
            End Try

            Return levels
        End Function

        ''' <summary>
        ''' Gets a mirror base URI from json file.
        ''' </summary>
        ''' <param name="name">Name of prefered mirror.</param>
        ''' <returns>A mirror base URI.</returns>
        Public Function GetMirror(name As String) As String
            Dim mirror As String = String.Empty

            Try
                Dim jsonMirrors As JObject = JObject.Parse(File.ReadAllText(Path.Combine(IOHelper.GetDirectoryPath("DoomWorld"), DOOMWORLD_MIRRORS_FILE)))

                Dim mirrorToken As JToken = jsonMirrors.SelectToken($"{HTTP}.{name}")
                mirror = mirrorToken.Value(Of String)
                ' if ping : 
                ' - Dim p As System.Net.NetworkInformation.Ping = New Net.NetworkInformation.Ping
                ' - Dim pingResult As System.Net.NetworkInformation.PingReply = Await p.SendPingAsync(mirror)

            Catch ex As Exception
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
            Dim mirrors As New List(Of String)
            Try
                Dim jsonMirrors As JObject = JObject.Parse(File.ReadAllText("doomworld_mirrors.json"))

                Dim mirrorsTokens As List(Of JToken) = jsonMirrors.SelectToken(HTTP).ToList()
                mirrorsTokens.ForEach(
                    Sub(mirror) mirrors.Add(mirror.First.Value(Of String))
                )

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : none")
            End Try

            Return mirrors
        End Function

        ''' <summary>
        ''' Downloads a .zip Level.
        ''' </summary>
        ''' <param name="levelUrl">The level download url.</param>
        ''' <returns>The .zip level archive path</returns>
        Public Async Function DownloadLevelZip(levelUrl As String, downloadsDirectory As String) As Task(Of String)
            Dim zipArchivePath As String = Nothing

            Try
                Dim requestUri As New Uri(levelUrl)
                Dim response As HttpResponseMessage = Await DoomWorldHttpClient.GetInstance().GetAsync(requestUri)

                Dim levelFileName As String = requestUri.Segments.Last()

                If Not Directory.Exists(downloadsDirectory) Then
                    Directory.CreateDirectory(downloadsDirectory)
                End If

                Using fileStream As New FileStream(Path.Combine(downloadsDirectory, levelFileName), FileMode.OpenOrCreate)
                    Await response.Content.CopyToAsync(fileStream)
                    zipArchivePath = fileStream.Name
                End Using

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {levelUrl}")
            End Try

            Return zipArchivePath
        End Function

        ''' <summary>
        ''' Extracts Level's files from zip archive in a folder.
        ''' </summary>
        ''' <param name="fileZipPath">path/filename.zip</param>
        ''' <returns>DirectoryInfo if exists, else Nothing</returns>
        Public Async Function ExtractLevelFromZip(fileZipPath As String) As Task(Of DirectoryInfo)
            Dim dirInfo As DirectoryInfo = Nothing

            Try
                If Not File.Exists(fileZipPath) Then Throw New FileNotFoundException

                Dim extractDirectory As String = IOHelper.GetFileInfo_RemoveExtension(fileZipPath)

                If Directory.Exists(extractDirectory) Then
                    Directory.Delete(extractDirectory, recursive:=True)
                End If

                Await Task.Run(Sub() ZipFile.ExtractToDirectory(fileZipPath, extractDirectory))
                If Directory.Exists(extractDirectory) Then 'TODO? Remove this useless If
                    dirInfo = New DirectoryInfo(extractDirectory)
                End If
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {fileZipPath}")
            End Try

            Return dirInfo
        End Function

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

        Public Function GetLevelDownloadUrl(level As Models.Level) As String
            Dim url As String = String.Empty

            Try
                url = String.Concat(level.Dir, level.Filename)
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {level}")
            End Try

            Return url
        End Function

        Private Function FormatLevelDLUri(idgamesurl As String) As String
            Dim uri As String = String.Empty

            Try
                uri = idgamesurl.Replace("idgames://", "")
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {idgamesurl}")
            End Try

            Return uri
        End Function

        ''' <summary>
        ''' Full process of downloading Level, and extract it in a "Downloads directory" (see TODO).
        ''' ''' </summary>
        ''' <param name="level"></param>
        ''' <param name="downloadsDirectory">TODO : Get downloadsDirectory from a Settings file (My.Settings ?)</param>
        ''' <returns>True if no error, false if something went wrong</returns>
        Public Async Function DownloadLevelFull(level As Models.Level, Optional downloadsDirectory As String = "DoomWorld/Downloads/") As Task(Of Boolean)
            Try
                ' Get mirror -> TODO : get from /DoomWorld/doomworld_mirrors.json file or whatever location
                Dim mirror As String = Me.GetMirror("germany")
                ' Get level download url
                Dim dlUrl As String = String.Concat(mirror, Me.GetLevelDownloadUrl(level))
                ' Get level zip
                Dim zipArchivePath As String = Await Me.DownloadLevelZip(dlUrl, downloadsDirectory)
                ' extract level
                Dim extractedDirInfo As DirectoryInfo = Await Me.ExtractLevelFromZip(zipArchivePath)

                If extractedDirInfo.Exists Then
                    ' TODO : create / write to json registry file
                    Me.WriteLevelIntoRegistry(level, extractedDirInfo.FullName)

                    'clean downloaded archive + extracted folder
                    Me.CleanUpZipArchive(zipArchivePath)

                    Return True
                End If

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : Level: {level}")
            End Try
            'else ROLLBACK and return false
            Return False
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="registryFilePath"></param>
        ''' <returns>List of levels if any. Nothing if not.</returns>
        Public Function GetInstalledLevels(registryFilePath As String) As List(Of Models.InstalledLevel)
            Dim installedLevels As List(Of Models.InstalledLevel) = Nothing

            Try
                Dim registryContent As String = IOHelper.GetJsonData(registryFilePath)
                installedLevels = JsonConvert.DeserializeObject(Of List(Of Models.InstalledLevel))(registryContent)

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {registryFilePath}")
            End Try

            Return installedLevels
        End Function

        ''' <summary>
        ''' create / write to json registry file
        ''' <br>for better finding/removing installed levels</br>
        ''' </summary>
        ''' <param name="level"></param>
        ''' <param name="directoryName"></param>
        ''' <returns>0 if success, -1 if error</returns>
        Public Function WriteLevelIntoRegistry(level As Models.Level, directoryName As String) As Integer
            Dim returnCode As Integer = 0

            Try
                Dim registryFilePath As String = Path.Combine(IOHelper.GetDirectoryPath("DoomWorld"), DOOMWORLD_REGISTRY_FILE)

                Dim installedLevels As List(Of Models.InstalledLevel) = Me.GetInstalledLevels(registryFilePath)
                If installedLevels Is Nothing Then
                    installedLevels = New List(Of InstalledLevel)
                End If

                Dim instLevel As New Models.InstalledLevel With
                {
                    .Title = level.Title,
                    .Id = level.Id,
                    .FileName = level.Filename,
                    .DirectoryName = directoryName, 'TODO? Add this as a Property in the Models.Level entity
                    .InstallDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                }
                installedLevels.Add(instLevel)

                Me.SaveRegistry(installedLevels, registryFilePath)

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : Level: {level}, DirectoryPath: {directoryName}")
                returnCode = -1
            End Try

            Return returnCode
        End Function


        ''' <summary>
        ''' Serialize installed levels list and save it into the registry file.
        ''' </summary>
        ''' <param name="installedLevels"></param>
        ''' <param name="registryFilePath"></param>
        Private Sub SaveRegistry(installedLevels As List(Of InstalledLevel), registryFilePath As String)
            Dim installedLevelsJson As String = JsonConvert.SerializeObject(installedLevels, Formatting.Indented)
            IOHelper.PersistJsonData(registryFilePath, installedLevelsJson)
        End Sub

        ''' <summary>
        ''' Removes downloaded zip archive and extracted folder from the archive.
        ''' </summary>
        ''' <param name="zipArchive"></param>
        Private Sub CleanUpZipArchive(zipArchive As String)
            File.Delete(zipArchive)
        End Sub

        ''' <summary>
        ''' Deletes specified level from the computer and from the registry.
        ''' </summary>
        ''' <param name="level"></param>
        Public Sub DeleteLevel(level As Models.Level)
            Dim deletedDirs As Integer = 0
            Try
                Dim registryFilePath As String = Path.Combine(IOHelper.GetDirectoryPath("DoomWorld"), DOOMWORLD_REGISTRY_FILE)

                Dim installedLevels As List(Of InstalledLevel) = Me.GetInstalledLevels(registryFilePath)
                Dim levelToDelete As Models.InstalledLevel = installedLevels.Find(Function(instLvl) instLvl.Id = level.Id)

                'Delete Level folder/files
                Me.DeleteLevelDirectory(levelToDelete.DirectoryName)

                'Delete Level from registry
                installedLevels.Remove(levelToDelete)
                Me.SaveRegistry(installedLevels, registryFilePath)

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {level}")
            End Try
        End Sub

        ''' <summary>
        ''' Delete the level directory.
        ''' </summary>
        ''' <param name="levelDirName"></param>
        Private Sub DeleteLevelDirectory(levelDirName As String)
            Try
                If Directory.Exists(levelDirName) Then Directory.Delete(levelDirName, True)

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {levelDirName}")
            End Try

        End Sub

        ''' Open Level's DoomWorld page.
        Public Sub OpenInBrowser(currentLevel As Level)
            Try
                Process.Start(currentLevel.Url)

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {currentLevel}")
            End Try
        End Sub

        ''' <summary>
        ''' Open installed Level directory in file exlorer.
        ''' </summary>
        ''' <param name="currentLevel"></param>
        ''' <remarks>- Throws exception if folder not found. -> (TODO : inform user)</remarks>
        Public Sub OpenInFileExplorer(currentLevel As Level)
            Try
                Dim registryFilePath As String = Path.Combine("DoomWorld", "doomworld_registry.json")
                Dim installdLevel As InstalledLevel = GetInstalledLevels(registryFilePath).Find(Function(instlldLvl) instlldLvl.FileName = currentLevel.Filename)
                Dim installedLevelDirPath As String = Path.Combine(GetDirectoryPath(), installdLevel.DirectoryName)

                Process.Start(installedLevelDirPath)
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {currentLevel}")

                Throw
            End Try
        End Sub
    End Class
End Namespace