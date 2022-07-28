Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ThisIsADoomLauncher.Helpers.DoomWorld

<TestClass()> Public Class DoomWorldTests

    ''' <summary>
    ''' Test get root directories
    ''' </summary>
    <TestMethod()> Public Sub GetRootDirectoriesTestOK()
        Dim parentDirectory As String = String.Empty

        Dim task As Task(Of List(Of Object)) = DoomWorldService.GetDirectories(parentDirectory)

        Dim result As List(Of Object) = task.Result

        Assert.AreNotEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Test get root directories
    ''' </summary>
    <TestMethod()> Public Sub GetRootDirectoriesTestKO()
        Dim parentDirectory As String = "dOOOOOm"

        Dim task As Task(Of List(Of Object)) = DoomWorldService.GetDirectories(parentDirectory)

        Dim result As List(Of Object) = task.Result

        Assert.AreEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Test get doom1 directories
    ''' </summary>
    <TestMethod()> Public Sub GetDirectoriesFromBaseGameTestOK()
        Dim parentDirectory As String = "doom/"

        Dim task As Task(Of List(Of Object)) = DoomWorldService.GetDirectories(parentDirectory)

        Dim result As List(Of Object) = task.Result

        Assert.AreNotEqual(0, result.Count)
    End Sub

End Class