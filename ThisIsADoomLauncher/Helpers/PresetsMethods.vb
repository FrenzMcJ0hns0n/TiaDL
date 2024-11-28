Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.FileIO
Imports ThisIsADoomLauncher.Models

Friend Module PresetsMethods


#Region "Currently enabled"

    ''' <summary>
    ''' Configure the TextFieldParser object to parse CSV data
    ''' </summary>
    ''' <param name="basePresetsType">Type of content to get</param>
    ''' <returns></returns>
    Public Function ConfigureTextFieldParser(basePresetsType As String) As TextFieldParser
        Dim parser As TextFieldParser = Nothing

        Try
            Select Case basePresetsType
                Case "levels" : parser = New TextFieldParser(New StringReader(My.Resources.base_presets_Levels))
                Case "mods" : parser = New TextFieldParser(New StringReader(My.Resources.base_presets_Mods))
            End Select

            With parser
                .CommentTokens = New String() {"#"}
                .Delimiters = New String() {";"}
                .TextFieldType = FieldType.Delimited
                .TrimWhiteSpace = True
            End With

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {basePresetsType}")
        End Try

        Return parser
    End Function

    Public Function GetBasePresetsFromCsv_Levels() As List(Of LevelPreset)
        Dim levelPresets As New List(Of LevelPreset)

        Try
            Dim parser As TextFieldParser = ConfigureTextFieldParser("levels")
            Using parser
                Do While Not parser.EndOfData
                    Try
                        Dim parsedValues As String() = parser.ReadFields()

                        If parsedValues.Length < 4 Then Continue Do 'Skip if first 4 mandatory data are not preset: Name, Type, Year, Iwad
                        If parsedValues(0) = "Wolfenstein 3D" Then Continue Do 'Skip Wolf3D, as not functional yet

                        levelPresets.Add(New LevelPreset() With
                        {
                            .Name = parsedValues(0),
                            .Type = parsedValues(1),
                            .Year = Convert.ToInt32(parsedValues(2)),
                            .Iwad = parsedValues(3),
                            .Maps = If(parsedValues.Length >= 5, parsedValues(4), String.Empty),
                            .Misc = If(parsedValues.Length >= 6, parsedValues(5), String.Empty),
                            .Pict = If(parsedValues.Length = 7, parsedValues(6), String.Empty)
                        })

                    Catch mleEx As MalformedLineException
                        WriteToLog($"{Date.Now} - Error : Got MalformedLineException while parsing base level presets")
                    End Try
                Loop
            End Using

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
        End Try

        Return levelPresets
    End Function

    Public Function GetBasePresetsFromCsv_Mods() As List(Of ModPreset)
        Dim modPresets As New List(Of ModPreset)

        Try
            Dim parser As TextFieldParser = ConfigureTextFieldParser("mods")
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
                            .Files = parsedValues(3).Split(CChar(",")).ToList
                        })
                    Catch mleEx As MalformedLineException
                        WriteToLog($"{Date.Now} - Error : Got MalformedLineException while parsing base mod presets")
                    End Try
                Loop
            End Using

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
        End Try

        Return modPresets
    End Function

#End Region

End Module
