Imports System.IO
Imports System.Reflection

Module General

    Function GetFormattedAppTitle() As String
        Dim executingAssembly As Assembly = Assembly.GetExecutingAssembly()
        Dim eaName As AssemblyName = executingAssembly.GetName()
        Dim eaFilePath As String = executingAssembly.Location

        Dim hasMinor As Boolean = eaName.Version.Minor > 0
        Dim hasBuild As Boolean = eaName.Version.Build > 0
        Dim hasRevis As Boolean = eaName.Version.Revision > 0

        Dim major As String = eaName.Version.Major.ToString
        Dim minor As String = IIf(hasMinor Or hasBuild Or hasRevis, $".{eaName.Version.Minor}", "")
        Dim build As String = IIf(hasBuild Or hasRevis, $".{eaName.Version.Build}", "")
        Dim revis As String = IIf(hasRevis, $".{eaName.Version.Revision}", "")

        Dim eaLastEdit As String = File.GetLastWriteTime(eaFilePath).ToString("yyyy-MM-dd")

        Return $"This is a Doom Launcher - v{major}{minor}{build}{revis} ({eaLastEdit})"
    End Function

End Module
