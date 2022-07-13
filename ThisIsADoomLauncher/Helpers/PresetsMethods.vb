Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports ThisIsADoomLauncher.Models
Imports ThisIsADoomLauncher.Views

Module PresetsMethods

    'TODO? v3+ Use JSON instead of CSV
    'Well, we will see...

#Region "Click on user preset"

    ''' <summary>
    ''' <para>Triggered when user right-clicks a custom preset</para>
    ''' <para>Delete a preset by its name</para>
    ''' </summary>
    '''
    'Sub DeleteUserLevelPreset(presetName As String)

    '    Dim message As String = String.Format("Delete preset ""{0}"" ?", presetName)

    '    If MessageBox.Show(message, "Delete user preset", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
    '        DeletePreset(presetName)
    '        DisplayUserPresets(GetLevelPresets_FromCsv("user")) 'Update GUI
    '    End If

    'End Sub

#End Region


    'Private Function ReturnUserPresetButtons(presetsList As List(Of LevelPreset)) As List(Of Button)

    '    Dim buttonsList As List(Of Button) = New List(Of Button)

    '    For Each preset As LevelPreset In presetsList
    '        Dim button As Button = New Button() With
    '        {
    '            .Height = 28,
    '            .Margin = New Thickness(0, 0, 0, 2),
    '            .FontSize = 14,
    '            .Content = preset.Name
    '        }

    '        'Left click
    '        AddHandler button.Click,
    '            Sub(sender, e)
    '                'SelectUserLevelPreset(preset.Iwad, If(preset.Level, Nothing), If(preset.Misc, Nothing))
    '            End Sub
    '        'Right click
    '        AddHandler button.MouseRightButtonDown,
    '            Sub(sender, e)
    '                DeleteUserLevelPreset(preset.Name)
    '            End Sub

    '        buttonsList.Add(button)
    '    Next

    '    Return buttonsList

    'End Function


    ''' <summary>
    ''' Use the parser object to collect data from CSV file
    ''' </summary>
    ''' 
    Function GetLevelPresets_FromCsv(presetsType As String) As List(Of LevelPreset)

        Dim presets As New List(Of LevelPreset)
        Dim parser As TextFieldParser = ConfigureTextFieldParser(presetsType)

        Try
            Using parser
                Do While Not parser.EndOfData

                    Try
                        Dim readValues As String() = parser.ReadFields()
                        If readValues.Length < 2 Then Continue Do 'If a preset does not contain BOTH Name and Iwad path, it is ignored (useless safety ?)

                        presets.Add(
                            New LevelPreset() With
                            {
                                .Name = readValues(0),
                                .Iwad = readValues(1),
                                .Level = If(readValues.Length >= 3, readValues(2), Nothing),
                                .Misc = If(readValues.Length >= 4, readValues(3), Nothing),
                                .ImagePath = If(readValues.Length = 5, readValues(4), Nothing)
                            }
                        )

                    Catch exception As MalformedLineException
                        WriteToLog(Date.Now & " - Error : Got MalformedLineException while parsing presets") ' use errorLine ?
                    End Try

                Loop
            End Using

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'FormatPresetsData_FromCsv()'. Exception : " & ex.ToString)
        End Try

        Return presets

    End Function

    Function GetModPresets_FromCSV(presetsType As String) As List(Of ModPreset)

        Dim presets As New List(Of ModPreset)
        Dim parser As TextFieldParser = ConfigureTextFieldParser(presetsType)

        Try
            Using parser
                Do While Not parser.EndOfData

                    Try
                        Dim readValues As String() = parser.ReadFields()
                        If readValues.Length < 4 Then Continue Do 'If a preset does not contain all info, it is ignored (useless safety ?)

                        presets.Add(
                            New ModPreset() With
                            {
                                .Name = readValues(0),
                                .Desc = readValues(1),
                                .ImagePath = readValues(2),
                                .Files = readValues(3).Split(",").ToList
                            }
                        )

                    Catch exception As MalformedLineException
                        WriteToLog(Date.Now & " - Error : Got MalformedLineException while parsing presets") ' use errorLine ?

                    End Try

                Loop
            End Using

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'GetModPresets_FromCSV()'. Exception : " & ex.ToString)
        End Try

        Return presets

    End Function


    ''' <summary>
    ''' Configure the parser as needed
    ''' </summary>
    ''' 
    Function ConfigureTextFieldParser(presetsType As String) As TextFieldParser

        Dim parser As TextFieldParser = Nothing

        'TODO: Use Select Case
        If presetsType = "base_levels" Then
            parser = New TextFieldParser(New StringReader(My.Resources.base_presets_Levels))
        ElseIf presetsType = "base_mods" Then
            parser = New TextFieldParser(New StringReader(My.Resources.base_presets_Mods))
        ElseIf presetsType = "user_levels" Then
            parser = New TextFieldParser(Path.Combine(GetDirectoryPath(""), "presets.csv")) 'TODO : Change/Add RootDirPath variable
        ElseIf presetsType = "user_mods" Then
            'TODO
        End If

        With parser
            .TextFieldType = FieldType.Delimited
            .CommentTokens = New String() {"#"}
            .Delimiters = New String() {";"}
            .TrimWhiteSpace = True
        End With

        Return parser

    End Function



    ''' <summary>
    ''' Handle New user preset save from GUI event
    ''' </summary>
    ''' 
    'Sub Save_NewPreset()

    '    Dim mainWindow As MainWindow = Windows.Application.Current.Windows(0)

    '    With mainWindow
    '        Try
    '            Dim nameToSave As String = .TextBox_NewPreset_Name.Text
    '            If nameToSave = "Enter preset name..." Or nameToSave = Nothing Then
    '                MessageBox.Show("New user preset requires a name to be saved")
    '                Return
    '            End If

    '            Dim iwadToSave As String = Nothing 'KnowSelectedIwad_NewPreset()
    '            If iwadToSave = Nothing Then
    '                MessageBox.Show("New user preset requires an IWAD to be saved")
    '                Return
    '            End If

    '            Dim levelToSave As String = Nothing 'KnowSelectedLevel_NewPreset()
    '            Dim miscToSave As String = Nothing 'KnowSelectedMisc_NewPreset()

    '            WritePresetToFile(nameToSave, iwadToSave, levelToSave, miscToSave)
    '            MessageBox.Show(String.Format("Preset ""{0}"" saved !", nameToSave))

    '        Catch ex As Exception
    '            WriteToLog(DateTime.Now & " - Error in 'Button_NewPreset_Save_Click()'. Exception : " & ex.ToString)
    '        End Try
    '    End With

    'End Sub

    ''' <summary>
    ''' Write attributes for New user preset.
    ''' As line in 'presets.csv'
    ''' </summary>
    ''' 
    Private Sub WritePresetToFile(name As String, iwad As String, Optional level As String = Nothing, Optional misc As String = Nothing)

        Try
            'Check if user-presets file exists
            Dim rootDirPath = GetDirectoryPath("")
            Dim presetFile As String = Path.Combine(rootDirPath, "presets.csv")
            If Not File.Exists(presetFile) Then WritePresetsFileHeader()

            'Format preset to be written
            Dim presetLine As String = String.Format("{0},{1}", name, iwad)
            presetLine &= If(level = Nothing, Nothing, "," & level)
            presetLine &= If(misc = Nothing, Nothing, "," & misc)

            'Check if last char is CR-LF (Windows EOL) 'TODO! Remove that and do clean things
            Dim end_ok As Boolean = False
            Using reader As New StreamReader(presetFile, Text.Encoding.UTF8)
                reader.BaseStream.Seek(-2, SeekOrigin.End)
                If reader.Read = 13 Then end_ok = True
            End Using

            'Write new preset at end of file
            Using writer As New StreamWriter(presetFile, True, Text.Encoding.UTF8)
                If end_ok = False Then writer.WriteLine() 'create new line if necessary
                writer.WriteLine(presetLine)
            End Using

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'WritePresetToFile()'. Exception : " & ex.ToString)
        End Try

    End Sub


    Sub DeletePreset(name As String)

        Dim rootDirPath = GetDirectoryPath("")
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

End Module
