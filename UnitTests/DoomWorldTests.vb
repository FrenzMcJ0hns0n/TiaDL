Imports System.Net.NetworkInformation
Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
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
        Dim id As Integer = 13024

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
        Dim task As Task(Of String) = _doomWorldService.DownloadLevel(url)

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

        Dim task As Task(Of String) = _doomWorldService.DownloadLevel(url)

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

        Dim task As Task(Of String) = _doomWorldService.DownloadLevel(url)

        Dim downloadedFileName = task.Result

        Assert.IsNotNull(downloadedFileName)
    End Sub

    ''' <summary>
    ''' Test download file from doomworld game url
    ''' </summary>
    <TestMethod()> Public Sub ExtractLevelFromZip_OK()

        'Dim fileName As String = "/av.zip"
        Dim fileName As String = "/cchest2.zip"

        Dim directoryPath As String = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of Integer) = _doomWorldService.ExtractLevelFromZip(directoryPath, fileName)

        Dim downloadedFileName = task.Result

        Assert.AreEqual(1, downloadedFileName)
    End Sub

    <TestMethod()> Public Sub MoveFiles_OK()

        'Dim directoryName As New Uri(String.Concat(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/av"))
        Dim directoryName As New Uri(String.Concat(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/jpcp"))
        'Dim directoryName As New Uri(String.Concat(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/cchest2"))

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of Integer) = _doomWorldService.MoveFilesIntoDirectories(directoryName.AbsolutePath)

        Dim nbMovedFiles = task.Result

        Assert.AreNotEqual(-1, nbMovedFiles)
        Assert.AreNotEqual(0, nbMovedFiles)
    End Sub

    '<TestMethod()> Public Sub MoveFiles_KO_NoChanges()

    '    Dim directoryName As New Uri(String.Concat(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/av"))

    '    Dim _doomWorldService As New DoomWorldService()
    '    Dim task As Task(Of Integer) = _doomWorldService.MoveFilesIntoDirectories(directoryName.AbsolutePath)

    '    Dim downloadedFileName = task.Result

    '    Assert.AreEqual(0, downloadedFileName)
    'End Sub

    '<TestMethod()> Public Sub MoveFiles_KO_DirectoryNotFound()

    '    Dim directoryName As New Uri(String.Concat(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/av"))

    '    Dim _doomWorldService As New DoomWorldService()
    '    Dim task As Task(Of Integer) = _doomWorldService.MoveFilesIntoDirectories(directoryName.AbsolutePath)

    '    Dim downloadedFileName = task.Result

    '    Assert.AreEqual(-1, downloadedFileName)
    'End Sub

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

        Dim uri As New Uri(mirror)
        'uri is not valid as is - not complete
        'TODO ping from host ? uri host ? no ping ?

        Dim p As New Net.NetworkInformation.Ping
        Dim pingResult As Task(Of System.Net.NetworkInformation.PingReply) = p.SendPingAsync(mirror)

        Dim pingReply As System.Net.NetworkInformation.PingReply = pingResult.Result

        Assert.AreEqual(IPStatus.Success, pingReply.Status)

    End Sub

    <TestMethod()> Public Sub GetMirrorsTest_OK()

        Dim _doomWorldService As New DoomWorldService()

        Dim mirrors As List(Of String) = _doomWorldService.GetMirrors()

        Assert.AreNotEqual(0, mirrors.Count)
    End Sub

End Class