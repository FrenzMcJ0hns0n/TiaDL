Imports System.IO
Imports System.Net.NetworkInformation
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports ThisIsADoomLauncher.Helpers.DoomWorld
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models

<TestClass()> Public Class DoomWorldTests_Search

    ''' <summary>
    ''' Search OK, getting results. Getting between 1 and 100 items.
    ''' </summary>
    <TestMethod()> Public Sub Search_OK()
        Dim searchText As String = "chest"

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of List(Of Level)) = _doomWorldService.SearchLevels(searchText)

        Dim result As List(Of Level) = task.Result

        Assert.AreNotEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Search fails because over 100 matches.
    ''' </summary>
    <TestMethod()> Public Sub Search_KO_Over100Results()
        Dim searchText As String = "doom"

        Dim _doomWorldService As New DoomWorldService()

        Dim result As Integer = 1
        Try
            Dim task As Task(Of List(Of Level)) = _doomWorldService.SearchLevels(searchText)

            Dim levls As List(Of Level) = task.Result
        Catch ex As OverflowException
            result = 0
        End Try


        Assert.AreEqual(0, result)
    End Sub

    ''' <summary>
    ''' Search. Fails because no results.
    ''' </summary>
    <TestMethod()> Public Sub Search_KO_NoResults()
        Dim searchText As String = "hkghdekcjghek"

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of List(Of Level)) = _doomWorldService.SearchLevels(searchText)

        Dim result As List(Of Level) = task.Result

        Assert.AreNotEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Search. Fails DoomWorld unreachable.
    ''' </summary>
    <TestMethod()> Public Sub Search_KO_DoomWorldUnreachable()
        Dim searchText As String = "chest"

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of List(Of Level)) = _doomWorldService.SearchLevels(searchText)

        Dim result As List(Of Level) = task.Result

        Assert.AreNotEqual(0, result.Count)
    End Sub

    ''' <summary>
    ''' Search. Fails no internet.
    ''' </summary>
    <TestMethod()> Public Sub Search_KO_NoInternet()
        Dim searchText As String = "chest"

        Dim _doomWorldService As New DoomWorldService()
        Dim task As Task(Of List(Of Level)) = _doomWorldService.SearchLevels(searchText)

        Dim result As List(Of Level) = task.Result

        Assert.AreNotEqual(0, result.Count)
    End Sub

End Class

