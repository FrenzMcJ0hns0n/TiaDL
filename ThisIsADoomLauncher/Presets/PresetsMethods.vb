Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports ThisIsADoomLauncher.Models

Module PresetsMethods


#Region "Click on user preset"

    ''' <summary>
    ''' <para>Triggered when user clicks a user-preset-button.</para>
    ''' <para>Set SelectedIwad, SelectedLevel, Selected Misc. for launch</para>
    ''' <para>Display red text on filepath if does not exist</para>
    ''' </summary>
    ''' 
    Sub SelectUserLevelPreset(iwadPath As String, Optional levelPath As String = Nothing, Optional miscPath As String = Nothing)

        Try
            With MainWindow_Instance()
                .TextBox_IwadToLaunch.Text = iwadPath
                .TextBox_LevelToLaunch.Text = levelPath
                .TextBox_MiscToLaunch.Text = miscPath
            End With

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'HandleUserPresetClick()'. Exception : " & ex.ToString)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' <para>Triggered when user right-clicks a custom preset</para>
    ''' <para>Delete a preset by its name</para>
    ''' </summary>
    '''
    Sub DeleteUserLevelPreset(presetName As String)

        Dim message As String = String.Format("Delete preset ""{0}"" ?", presetName)

        If MessageBox.Show(message, "Delete user preset", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
            DeletePreset(presetName)
            DisplayUserPresets(GetLevelPresets_FromCsv("user")) 'Update GUI
        End If

    End Sub

#End Region


#Region "Create and display User presets"

#End Region

    ''' <summary>
    ''' <para>Build and display loaded presets as buttons = create a button with .Click functions for each preset
    ''' values(0) : Preset Name, values(1) : Preset Iwad, values(2) : Preset Level, values(3) : Preset Misc</para>
    ''' </summary>
    ''' 
    Sub DisplayUserPresets(presetsList As List(Of LevelPreset))

        With MainWindow_Instance()
            .StackPanel_UserPresets.Children.Clear()

            If presetsList.Count = 0 Then
                .Label_NoUserPresetsFound.Visibility = Visibility.Visible
                Return 'Nothing to read : exit
            Else
                .Label_NoUserPresetsFound.Visibility = Visibility.Collapsed
            End If


            Dim buttons As List(Of Button) = ReturnUserPresetButtons(presetsList)

            For Each btn As Button In buttons
                .StackPanel_UserPresets.Children.Add(btn)
            Next
        End With

    End Sub


    Private Function ReturnUserPresetButtons(presetsList As List(Of LevelPreset)) As List(Of Button)

        Dim buttonsList As List(Of Button) = New List(Of Button)

        For Each preset As LevelPreset In presetsList
            Dim button As Button = New Button() With
            {
                .Height = 28,
                .Margin = New Thickness(0, 0, 0, 2),
                .FontSize = 14,
                .Content = preset.Name
            }

            'Left click
            AddHandler button.Click,
                Sub(sender, e)
                    SelectUserLevelPreset(preset.Iwad, If(preset.Level, Nothing), If(preset.Misc, Nothing))
                End Sub
            'Right click
            AddHandler button.MouseRightButtonDown,
                Sub(sender, e)
                    DeleteUserLevelPreset(preset.Name)
                End Sub

            buttonsList.Add(button)
        Next

        Return buttonsList

    End Function


    ''' <summary>
    ''' Use the parser object to collect data from CSV file
    ''' </summary>
    ''' 
    Function GetLevelPresets_FromCsv(presetsType As String) As List(Of LevelPreset)

        Dim presets As List(Of LevelPreset) = New List(Of LevelPreset)
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
                        WriteToLog(DateTime.Now & " - Error : Got MalformedLineException while parsing presets") ' use errorLine ?
                    End Try

                Loop
            End Using

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'FormatPresetsData_FromCsv()'. Exception : " & ex.ToString)
        End Try

        Return presets

    End Function

    Function GetModPresets_FromCSV(presetsType As String) As List(Of ModPreset)

        Dim presets As List(Of ModPreset) = New List(Of ModPreset)
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
                                .Files = readValues(3).Split(";").ToList
                            }
                        )

                    Catch exception As MalformedLineException
                        WriteToLog(DateTime.Now & " - Error : Got MalformedLineException while parsing presets") ' use errorLine ?

                    End Try

                Loop
            End Using

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'GetModPresets_FromCSV()'. Exception : " & ex.ToString)
        End Try

        Return presets

    End Function


    ''' <summary>
    ''' Configure the parser as needed
    ''' </summary>
    ''' 
    Function ConfigureTextFieldParser(presetsType As String) As TextFieldParser

        Dim parser As TextFieldParser = Nothing

        If presetsType = "base_levels" Then
            parser = New TextFieldParser(New StringReader(My.Resources.base_presets_Levels))
        ElseIf presetsType = "base_mods" Then
            parser = New TextFieldParser(New StringReader(My.Resources.base_presets_Mods))
        ElseIf presetsType = "user_levels" Then
            parser = New TextFieldParser(Path.Combine(My.Settings.RootDirPath, "presets.csv"))
        ElseIf presetsType = "user_mods" Then
            'TODO
        End If

        With parser
            .TextFieldType = FieldType.Delimited
            .CommentTokens = New String() {"#"}
            .Delimiters = New String() {","}
            .TrimWhiteSpace = True
        End With

        Return parser

    End Function



    ''' <summary>
    ''' Handle New user preset save from GUI event
    ''' </summary>
    ''' 
    Sub Save_NewPreset()

        With MainWindow_Instance()
            Try
                Dim nameToSave As String = .TextBox_NewPreset_Name.Text
                If nameToSave = "Enter preset name..." Or nameToSave = Nothing Then
                    MessageBox.Show("New user preset requires a name to be saved")
                    Return
                End If

                Dim iwadToSave As String = KnowSelectedIwad_NewPreset()
                If iwadToSave = Nothing Then
                    MessageBox.Show("New user preset requires an IWAD to be saved")
                    Return
                End If

                Dim levelToSave As String = KnowSelectedLevel_NewPreset()
                Dim miscToSave As String = KnowSelectedMisc_NewPreset()

                WritePresetToFile(nameToSave, iwadToSave, levelToSave, miscToSave)
                MessageBox.Show(String.Format("Preset ""{0}"" saved !", nameToSave))

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'Button_NewPreset_Save_Click()'. Exception : " & ex.ToString)
            End Try
        End With

    End Sub

    ''' <summary>
    ''' Write attributes for New user preset.
    ''' As line in 'presets.csv'
    ''' </summary>
    ''' 
    Private Sub WritePresetToFile(name As String, iwad As String, Optional level As String = Nothing, Optional misc As String = Nothing)

        Try
            'Check if user-presets file exists
            Dim presetFile As String = My.Settings.RootDirPath & "\presets.csv"
            If Not File.Exists(presetFile) Then WritePresetsFileHeader()

            'Format preset to be written
            Dim presetLine As String = String.Format("{0},{1}", name, iwad)
            presetLine &= If(level = Nothing, Nothing, "," & level)
            presetLine &= If(misc = Nothing, Nothing, "," & misc)

            'Check if last char is CR-LF (Windows EOL)
            Dim end_ok As Boolean = False
            Using reader As StreamReader = New StreamReader(presetFile, Text.Encoding.UTF8)
                reader.BaseStream.Seek(-2, SeekOrigin.End)
                If reader.Read = 13 Then end_ok = True
            End Using

            'Write new preset at end of file
            Using writer As StreamWriter = New StreamWriter(presetFile, True, Text.Encoding.UTF8)
                If end_ok = False Then writer.WriteLine() 'create new line if necessary
                writer.WriteLine(presetLine)
            End Using

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'WritePresetToFile()'. Exception : " & ex.ToString)
        End Try

    End Sub


    Sub DeletePreset(name As String)

        Dim lines As List(Of String) = File.ReadAllLines(My.Settings.RootDirPath & "\presets.csv").ToList
        Dim count As Integer = 0

        For Each line As String In lines
            count += 1
            If line.StartsWith(name) Then
                lines.RemoveAt(count - 1)
                File.WriteAllLines(My.Settings.RootDirPath & "\presets.csv", lines)
                Exit For
            End If
        Next

    End Sub

End Module
