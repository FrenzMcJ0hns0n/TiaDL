Imports System.IO
Imports System.Reflection
Imports ThisIsADoomLauncher.Models

Friend Module General

    Public Function GetFormattedAppTitle() As String
        Dim executingAssembly As Assembly = Assembly.GetExecutingAssembly()
        Dim eaName As AssemblyName = executingAssembly.GetName()
        Dim eaFilePath As String = executingAssembly.Location
        Dim eaLastGeneration As Date = If(Debugger.IsAttached, Now, File.GetLastWriteTime(eaFilePath))

        Dim hasMinor As Boolean = eaName.Version.Minor > 0
        Dim hasBuild As Boolean = eaName.Version.Build > 0
        Dim hasRevis As Boolean = eaName.Version.Revision > 0

        Dim major As String = eaName.Version.Major.ToString
        Dim minor As String = If(hasMinor Or hasBuild Or hasRevis, $".{eaName.Version.Minor}", "")
        Dim build As String = If(hasBuild Or hasRevis, $".{eaName.Version.Build}", "")
        Dim revis As String = If(hasRevis, $".{eaName.Version.Revision}", "")

        Return $"This is a Doom Launcher - v{major}{minor}{build}{revis} ({eaLastGeneration:yyyy-MM-dd})"
    End Function

    ''' <summary>
    ''' Sort list of LevelPreset objects based on selected criterion
    ''' </summary>
    ''' <param name="levelPresets">List of LevelPreset objects</param>
    ''' <param name="criterion">LevelPreset property to use as sorting criterion</param>
    ''' <param name="isAscending">Is sorting order ascending?</param>
    ''' <returns></returns>
    Public Function SortLevelPresets(levelPresets As List(Of LevelPreset), criterion As SortCriterion, isAscending As Boolean) As List(Of LevelPreset)
        Select Case criterion

            Case SortCriterion.Name
                Return If(isAscending,
                          levelPresets.OrderBy(Function(preset) preset.Name).ToList,
                          levelPresets.OrderByDescending(Function(preset) preset.Name).ToList)

            Case SortCriterion.Year
                Return If(isAscending,
                          levelPresets.OrderBy(Function(preset) preset.Year).ToList,
                          levelPresets.OrderByDescending(Function(preset) preset.Year).ToList)

            Case SortCriterion.Type
                Return If(isAscending,
                          levelPresets.OrderBy(Function(preset) preset.Type).ToList,
                          levelPresets.OrderByDescending(Function(preset) preset.Type).ToList)

            Case Else 'No sorting
                Return levelPresets

        End Select
    End Function

    ''' <summary>
    ''' Create a new preset in the list user's presets (or in a new list if none exists).
    ''' </summary>
    ''' <param name="presetToCreate"></param>
    Public Sub CreateNewPreset(presetToCreate As LevelPreset)

        Dim jsonFilepath As String = GetJsonFilepath("UserLevels")
        Dim jsonString As String = GetJsonData(jsonFilepath)
        Dim hasUserData As Boolean = CanLoadJsonArray(jsonString)

        Dim currentLevels As List(Of LevelPreset) = If(hasUserData, LoadUserLevels(jsonString), New List(Of LevelPreset))

        currentLevels.Add(presetToCreate)
        SaveUserLevels(currentLevels, jsonFilepath)
    End Sub
End Module
