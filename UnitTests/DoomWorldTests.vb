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
        Dim id As String = "15156"

        Dim task As Task(Of Level) = DoomWorldService.GetLevel(id)

        Dim level As Level = task.Result

        Assert.AreEqual(id, level.Id)
    End Sub

End Class