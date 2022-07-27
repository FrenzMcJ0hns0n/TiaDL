Imports System.IO
Imports System.Reflection
Imports ThisIsADoomLauncher.Models

Friend Module General

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

    ''' <summary>
    ''' Sorts Presets (NOT TESTED YET)
    ''' </summary>
    ''' <param name="presets">List of Preset</param>
    ''' <param name="sortCriterion">Sorting type (Enum)</param>
    ''' <returns></returns>
    Function SortPresets(presets As List(Of Preset), sortCriterion As SortCriterion, ascending As Boolean) As List(Of Preset)
        Select Case sortCriterion

            Case SortCriterion.Name
                Return If(ascending, presets.OrderBy(Function(preset) preset.Name), presets.OrderByDescending(Function(preset) preset.Name))

            Case SortCriterion.Year
                If presets.GetType() IsNot GetType(List(Of LevelPreset)) Then
                    Throw New NotSupportedException("Wrong type")
                End If
                Return If(ascending, presets.OrderBy(Function(preset As LevelPreset) preset.Year), presets.OrderByDescending(Function(preset As LevelPreset) preset.Year))

            Case SortCriterion.Type
                If presets.GetType() IsNot GetType(List(Of LevelPreset)) Then
                    Throw New NotSupportedException("Wrong type")
                End If
                Return If(ascending, presets.OrderBy(Function(preset As LevelPreset) preset.Type), presets.OrderByDescending(Function(preset As LevelPreset) preset.Type))

            Case Else 'By default : no sort
                Return presets

        End Select
    End Function
End Module
