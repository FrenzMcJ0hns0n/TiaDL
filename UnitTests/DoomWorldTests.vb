Imports System.IO
Imports System.Net.NetworkInformation
Imports System.Reflection
Imports ThisIsADoomLauncher.Helpers.DoomWorld
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models

<TestClass()> Public Class DoomWorldTests

    ''' <summary>
    ''' Test get root directories
    ''' </summary>
    <TestMethod()> Public Sub GetRootDirectoriesTest_OK()
        Dim parentDirectory As String = String.Empty

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of List(Of String)) = _doomWorldService.GetDirectories(parentDirectory)

        Dim result As List(Of String) = task.Result

        Assert.AreNotEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Test get level directories from non-existing directory
    ''' </summary>
    <TestMethod()> Public Sub GetRootDirectoriesTest_KO()
        Dim parentDirectory As String = "dOOOOOm"

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of List(Of String)) = _doomWorldService.GetDirectories(parentDirectory)

        Dim result As List(Of String) = task.Result

        Assert.AreEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Test get parent directory OK
    ''' </summary>
    <TestMethod()> Public Sub GetParentDirectoriesTest_OK()
        Dim currentDirectory As String = "levels/doom/a-c/"

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of Folder) = _doomWorldService.GetParentDirectory(currentDirectory)

        Dim result As Folder = task.Result

        Assert.IsFalse(String.IsNullOrWhiteSpace(result.Name))
    End Sub

    ''' <summary>
    ''' Test get parent directory KO from empty path
    ''' </summary>
    <TestMethod()> Public Sub GetParentDirectoriesTest_KO_EmptyPath()
        Dim currentDirectory As String = String.Empty

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of Folder) = _doomWorldService.GetParentDirectory(currentDirectory)

        Dim result As Folder = task.Result

        Assert.IsFalse(String.IsNullOrWhiteSpace(result.Name))
    End Sub

    ''' <summary>
    ''' Test get parent directory KO from bad directory
    ''' </summary>
    <TestMethod()> Public Sub GetParentDirectoriesTest_KO_BadDirectory()
        Dim currentDirectory As String = "/"

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of Folder) = _doomWorldService.GetParentDirectory(currentDirectory)

        Dim result As Folder = task.Result

        Assert.IsFalse(String.IsNullOrWhiteSpace(result.Name))
    End Sub

    ''' <summary>
    ''' Test get doom1 directories
    ''' </summary>
    <TestMethod()> Public Sub GetDirectoriesFromBaseGameTest_OK()
        Dim parentDirectory As String = "doom/"

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of List(Of String)) = _doomWorldService.GetDirectories(parentDirectory)

        Dim result As List(Of String) = task.Result

        Assert.AreNotEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Test get level from id
    ''' </summary>
    <TestMethod()> Public Sub GetLevelTest_OK()
        Dim id As Long = 13024

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of Level) = _doomWorldService.GetLevel(id)

        Dim level As Level = task.Result

        Assert.AreEqual(id, level.Id)
    End Sub

    ''' <summary>
    ''' Test download file from doomworld game url
    ''' </summary>
    <TestMethod()> Public Sub DownloadLevel_1_CChest2_FromUrl_OK()
        Dim url As String = "https://www.quaddicted.com/files/idgames/levels/doom2/Ports/megawads/cchest2.zip"

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of String) = _doomWorldService.DownloadLevelZipArchive(url)

        Dim downloadedFileName = task.Result

        Assert.IsNotNull(downloadedFileName)
    End Sub

    ''' <summary>
    ''' Test download file from doomworld game url
    ''' </summary>
    <TestMethod()> Public Sub DownloadLevel_2_Abyss_BuildUriFromLevel_OK()
        Dim _doomWorldService As New DoomWorldService()

        Dim mirror As String = _doomWorldService.GetMirror("germany")
        Dim levelAbyss As Level = New Level With {.Dir = "levels/doom/a-c/", .Filename = "abyss.zip"}
        Dim url As String = String.Concat(mirror, _doomWorldService.GetLevelDownloadUrl(levelAbyss))

        Dim task As Task(Of String) = _doomWorldService.DownloadLevelZipArchive(url)

        Dim downloadedFileName = task.Result

        Assert.IsNotNull(downloadedFileName)
    End Sub

    ''' <summary>
    ''' Test download file from doomworld game url
    ''' </summary>
    <TestMethod()> Public Sub DownloadLevel_3_Alien_Vendetta_BuildUriFromID_OK()
        Dim _doomWorldService As New DoomWorldService()
        Dim levelId As Integer = 11790

        Dim mirror As String = _doomWorldService.GetMirror("germany")
        Dim taskLevelAV As Task(Of Level) = _doomWorldService.GetLevel(levelId)
        Dim levelAV As Level = taskLevelAV.Result

        Dim url As String = String.Concat(mirror, _doomWorldService.GetLevelDownloadUrl(levelAV))

        Dim task As Task(Of String) = _doomWorldService.DownloadLevelZipArchive(url)

        Dim downloadedFileName = task.Result

        Assert.IsNotNull(downloadedFileName)
    End Sub

    ''' <summary>
    ''' Test download file from doomworld game url
    ''' </summary>
    <TestMethod()> Public Sub ExtractLevelFromZip_OK()

        Dim archiveName As String = Path.Combine(Directory.GetCurrentDirectory(), "cchest2.zip")
        Dim dirNameResult As String = "cchest2"

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of DirectoryInfo) = _doomWorldService.ExtractLevelFromZip(archiveName)

        Dim extractedDir As DirectoryInfo = task.Result
        Assert.AreEqual(dirNameResult, extractedDir.Name)
    End Sub

    <TestMethod()> Public Sub MoveFiles_OK()
        'Dim directoryName As New Uri(String.Concat(IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/av"))
        Dim directoryName As New Uri(String.Concat(IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/jpcp"))
        'Dim directoryName As New Uri(String.Concat(IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/cchest2"))

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of Integer) = _doomWorldService.MoveFilesIntoDirectories(directoryName.AbsolutePath)

        Dim nbMovedFiles = task.Result

        Assert.AreNotEqual(-1, nbMovedFiles)
        Assert.AreNotEqual(0, nbMovedFiles)
    End Sub

    <TestMethod()> Public Sub GetDirsAndFilesTest_OK()
        Dim noFolderPath As String = ""
        Dim doomFolderPath As String = "doom/"
        Dim a_cFolderPath As String = "doom/a-c/"

        Dim _doomWorldService As New DoomWorldService()

        Dim taskRienFolders As Task(Of List(Of Object)) = _doomWorldService.GetContent(noFolderPath)
        Dim taskDoomFolders As Task(Of List(Of Object)) = _doomWorldService.GetContent(doomFolderPath)
        Dim taskA_CFolders As Task(Of List(Of Object)) = _doomWorldService.GetContent(a_cFolderPath)

        Dim folders1 = taskRienFolders.Result
        Dim folders2 = taskDoomFolders.Result
        Dim levelsList = taskA_CFolders.Result

        Assert.AreNotEqual(0, folders1.Count)
        Assert.AreNotEqual(0, folders2.Count)
        Assert.AreNotEqual(0, levelsList.Count)
    End Sub

    <TestMethod()> Public Sub GetMirrorTest_OK()
        Dim _doomWorldService As New DoomWorldService()

        Dim mirror As String = _doomWorldService.GetMirror("germany")

        Assert.IsFalse(String.IsNullOrWhiteSpace(mirror))
    End Sub

    ''' <summary>
    ''' Test ping mirror.
    ''' </summary>
    <TestMethod()> Public Sub PingMirrorTest_OK()
        Dim _doomWorldService As New DoomWorldService()

        Dim mirror As String = _doomWorldService.GetMirror("germany")
        Dim uri As New Uri(mirror) '=> Typically https://www.quaddicted.com/files/idgames/

        Dim p As New Ping
        Dim pingResult As Task(Of PingReply) = p.SendPingAsync(uri.Host)
        Dim pingReply As PingReply = pingResult.Result

        Assert.AreEqual(IPStatus.Success, pingReply.Status)
    End Sub

    <TestMethod()> Public Sub GetMirrorsTest_OK()
        Dim _doomWorldService As New DoomWorldService()

        Dim mirrors As List(Of String) = _doomWorldService.GetMirrors()

        Assert.AreNotEqual(0, mirrors.Count)
    End Sub

    <TestMethod()> Public Sub DownloadLevelFullTest_OK()
        Dim _doomWorldService As New DoomWorldService()

        Dim lvl As Level = New Level With
        {
            .Filename = "cchest.zip",
            .Dir = "levels/doom2/Ports/megawads/",
            .Id = 12021,
            .Idgamesurl = "idgames://levels/doom2/Ports/megawads/cchest.zip",
            .Title = "The Community Chest Project",
            .Url = "https://www.doomworld.com/idgames/levels/doom2/Ports/megawads/cchest"
        }

        Dim task As Task(Of Boolean) = _doomWorldService.DownloadLevelFull(lvl)
        Dim res As Boolean = task.Result

        Assert.AreEqual(True, res)
    End Sub

End Class