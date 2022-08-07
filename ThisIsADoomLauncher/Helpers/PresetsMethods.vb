Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.FileIO
Imports ThisIsADoomLauncher.Models

Friend Module PresetsMethods


#Region "Currently enabled"

    ''' <summary>
    ''' Configure the TextFieldParser object to parse CSV data
    ''' </summary>
    ''' <param name="presetsType">Type of content to get</param>
    ''' <returns></returns>
    Public Function ConfigureTextFieldParser(presetsType As String) As TextFieldParser
        Dim parser As TextFieldParser = Nothing

        Try
            Select Case presetsType
                Case "base_levels"
                    parser = New TextFieldParser(New StringReader(My.Resources.base_presets_Levels))
                Case "base_mods"
                    parser = New TextFieldParser(New StringReader(My.Resources.base_presets_Mods))
                Case "user_levels"
                    parser = New TextFieldParser(GetCsvFilepath("UserLevels"))
                Case "user_mods"
                    'TODO
                Case Else 'TODO?
            End Select

            With parser
                .CommentTokens = New String() {"#"}
                .Delimiters = New String() {";"}
                .TextFieldType = FieldType.Delimited
                .TrimWhiteSpace = True
            End With

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {presetsType}")
        End Try

        Return parser
    End Function

    Public Function GetLevelPresets_FromCsv(presetsType As String) As List(Of LevelPreset)
        Dim levelPresets As New List(Of LevelPreset)

        Try
            Dim parser As TextFieldParser = ConfigureTextFieldParser(presetsType)
            Using parser
                Do While Not parser.EndOfData
                    Try
                        Dim parsedValues As String() = parser.ReadFields()

                        If presetsType = "base_levels" Then

                            If parsedValues.Length < 4 Then Continue Do 'Skip if first 4 mandatory data are not preset: Name, Type, Year, Iwad
                            If parsedValues(0) = "Wolfenstein 3D" Then Continue Do 'Skip Wolf3D, as not functional yet

                            levelPresets.Add(New LevelPreset() With
                            {
                                .Name = parsedValues(0),
                                .Type = parsedValues(1),
                                .Year = Convert.ToInt32(parsedValues(2)),
                                .Desc = $"Year : { .Year}{vbCrLf}Type : { .Type}",
                                .Iwad = parsedValues(3),
                                .Maps = If(parsedValues.Length >= 5, parsedValues(4), String.Empty),
                                .Misc = If(parsedValues.Length >= 6, parsedValues(5), String.Empty),
                                .Pict = If(parsedValues.Length = 7, parsedValues(6), String.Empty)
                            })

                        Else

                            If parsedValues.Length < 3 Then Continue Do 'Skip if first 3 mandatory data are not preset: Name, Iwad, Maps

                            levelPresets.Add(New LevelPreset() With
                            {
                                .Name = parsedValues(0),
                                .Iwad = parsedValues(1),
                                .Maps = parsedValues(2),
                                .Misc = If(parsedValues.Length >= 4, parsedValues(3), String.Empty)
                            })

                        End If


                    Catch mleEx As MalformedLineException
                        WriteToLog($"{Date.Now} - Error : Got MalformedLineException while parsing presets {presetsType}")
                    End Try
                Loop
            End Using

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {presetsType}")
        End Try

        Return levelPresets
    End Function

    Public Function GetModPresets_FromCsv(presetsType As String) As List(Of ModPreset)
        Dim modPresets As New List(Of ModPreset)

        Try
            Dim parser As TextFieldParser = ConfigureTextFieldParser(presetsType)
            Using parser
                Do While Not parser.EndOfData
                    Try
                        Dim parsedValues As String() = parser.ReadFields()

                        'Name, Desc, Pict and Files are mandatory
                        If parsedValues.Length < 4 Then Continue Do

                        modPresets.Add(New ModPreset() With
                        {
                            .Name = parsedValues(0),
                            .Desc = parsedValues(1),
                            .Pict = parsedValues(2),
                            .Files = parsedValues(3).Split(",").ToList
                        })
                    Catch mleEx As MalformedLineException
                        WriteToLog($"{Date.Now} - Error : Got MalformedLineException while parsing presets {presetsType}")
                    End Try
                Loop
            End Using

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {presetsType}")
        End Try

        Return modPresets
    End Function

#End Region


#Region "Not used yet / to implement"

    Public Sub DeletePreset(name As String)

        Dim rootDirPath = GetDirectoryPath()
        Dim presetFile As String = Path.Combine(rootDirPath, "presets.csv")

        Dim lines As List(Of String) = File.ReadAllLines(presetFile).ToList
        Dim count As Integer = 0

        For Each line As String In lines
            count += 1
            If line.StartsWith(name) Then
                lines.RemoveAt(count - 1)
                File.WriteAllLines(presetFile, lines)
                Exit For
            End If
        Next

    End Sub

    'TODO
    Public Sub PersistUserLevelPresets(levelPresets As List(Of LevelPreset))
        Try
            Dim csvPath As String = GetCsvFilepath("UserLevels")

            Using writer As New StreamWriter(csvPath, False)
                writer.Write(String.Join(vbCrLf, USER_LEVELS_HEADER))
                levelPresets.ForEach(Sub(lp As LevelPreset) writer.WriteLine($"{lp.Name}; {lp.Iwad}; {lp.Maps}; {lp.Misc}"))
            End Using

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            Dim presetNames As List(Of String) = levelPresets.Select(Function(lp As LevelPreset) lp.Name).ToList
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : Preset names = {String.Join(", ", presetNames)}")
        End Try
    End Sub

#End Region


End Module
