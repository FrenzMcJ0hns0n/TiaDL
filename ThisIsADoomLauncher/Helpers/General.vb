Imports System.IO
Imports System.Reflection
Imports ThisIsADoomLauncher.Models

Friend Module General

    Public Function GetFormattedAppTitle() As String
        Dim executingAssembly As Assembly = Assembly.GetExecutingAssembly()
        Dim eaName As AssemblyName = executingAssembly.GetName()
        Dim eaFilePath As String = executingAssembly.Location

        Dim hasMinor As Boolean = eaName.Version.Minor > 0
        Dim hasBuild As Boolean = eaName.Version.Build > 0
        Dim hasRevis As Boolean = eaName.Version.Revision > 0

        Dim major As String = eaName.Version.Major.ToString
        Dim minor As String = If(hasMinor Or hasBuild Or hasRevis, $".{eaName.Version.Minor}", "")
        Dim build As String = If(hasBuild Or hasRevis, $".{eaName.Version.Build}", "")
        Dim revis As String = If(hasRevis, $".{eaName.Version.Revision}", "")

        Dim eaLastEdit As String = File.GetLastWriteTime(eaFilePath).ToString("yyyy-MM-dd")

        Return $"This is a Doom Launcher - v{major}{minor}{build}{revis} ({eaLastEdit})"
    End Function

    ''' <summary>
    ''' Sorts Presets (NOT TESTED YET)
    ''' </summary>
    ''' <param name="levelPresets">List of LevelPreset</param>
    ''' <param name="sortCriterion">Sorting type (Enum)</param>
    ''' <returns></returns>
    Public Function SortLevelPresets(levelPresets As List(Of LevelPreset), sortCriterion As SortCriterion, ascending As Boolean) As List(Of LevelPreset)
        Select Case sortCriterion

            Case SortCriterion.Name
                Return If(ascending, levelPresets.OrderBy(Function(preset) preset.Name).ToList(), levelPresets.OrderByDescending(Function(preset) preset.Name).ToList)

            Case SortCriterion.Year
                Return If(ascending, levelPresets.OrderBy(Function(preset) preset.Year).ToList, levelPresets.OrderByDescending(Function(preset) preset.Year).ToList)

            Case SortCriterion.Type
                Return If(ascending, levelPresets.OrderBy(Function(preset) preset.Type).ToList, levelPresets.OrderByDescending(Function(preset) preset.Type).ToList)

            Case Else 'By default : no sort
                Return levelPresets

        End Select
    End Function
End Module
