Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class DoomWorldAPITests
    <TestMethod()> Public Async Function GetGameInfosByIdTest() As Task

        Dim client As ThisIsADoomLauncher.Helpers.API.DoomWorldClient = New Helpers.API.DoomWorldClient()

        Dim actual = Await client.GetGameInfosById(1)

        Assert.IsNotNull(actual)

    End Function
End Class