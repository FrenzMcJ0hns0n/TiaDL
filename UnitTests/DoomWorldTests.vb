Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ThisIsADoomLauncher.Helpers.DoomWorld

<TestClass()> Public Class DoomWorldTests

    ''' <summary>
    ''' Test get root directories
    ''' </summary>
    <TestMethod()> Public Sub GetRootDirectoriesTest_OK()
        Dim parentDirectory As String = String.Empty

        Dim task As Task(Of List(Of Object)) = DoomWorldService.GetDirectories(parentDirectory)

        Dim result As List(Of Object) = task.Result

        Assert.AreNotEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Test get level directories from non-existing directory
    ''' </summary>
    <TestMethod()> Public Sub GetRootDirectoriesTest_KO()
        Dim parentDirectory As String = "dOOOOOm"

        Dim task As Task(Of List(Of Object)) = DoomWorldService.GetDirectories(parentDirectory)

        Dim result As List(Of Object) = task.Result

        Assert.AreEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Test get doom1 directories
    ''' </summary>
    <TestMethod()> Public Sub GetDirectoriesFromBaseGameTest_OK()
        Dim parentDirectory As String = "doom/"

        Dim task As Task(Of List(Of Object)) = DoomWorldService.GetDirectories(parentDirectory)

        Dim result As List(Of Object) = task.Result

        Assert.AreNotEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Test get level from id
    ''' </summary>
    <TestMethod()> Public Sub GetLevelTest_OK()
        Dim id As Integer = 15156

        Dim task As Task(Of Level) = DoomWorldService.GetLevel(id)

        Dim level As Level = task.Result

        Assert.AreEqual(id, level.Id)
    End Sub

    ''' <summary>
    ''' Test get level download urls from the level idgames url
    ''' </summary>
    <TestMethod()> Public Sub GetLevelDownloadLinksTest_OK()
        Dim url As String = "https://www.doomworld.com/idgames/levels/doom2/a-c/arch"

        Dim task As Task(Of List(Of String)) = DoomWorldService.GetLevelDownloadLinks(url)

        Dim links As List(Of String) = task.Result

        Assert.AreNotEqual(0, links.Count)
    End Sub

    ''' <summary>
    ''' Test download file from doomworld game url
    ''' </summary>
    <TestMethod()> Public Sub DownloadLevelFromUrl_OK()
        'Dim url As String = "https://www.quaddicted.com/files/idgames/levels/doom2/a-c/arch.zip"
        Dim url As String = "https://www.quaddicted.com/files/idgames/levels/doom2/megawads/av.zip"

        Dim task As Task(Of String) = DoomWorldService.DownloadLevel(url)

        Dim downloadedFileName = task.Result

        Assert.IsNotNull(downloadedFileName)
    End Sub

    ''' <summary>
    ''' Test download file from doomworld game url
    ''' </summary>
    <TestMethod()> Public Sub ExtractLevelFromZip_OK()

        Dim fileName As String = "/av.zip"

        Dim directoryPath As String = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

        Dim task As Task(Of Integer) = DoomWorldService.ExtractLevelFromZip(directoryPath, fileName)

        Dim downloadedFileName = task.Result

        Assert.AreEqual(1, downloadedFileName)
    End Sub

End Class