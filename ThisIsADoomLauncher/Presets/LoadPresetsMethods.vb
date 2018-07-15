Imports System.IO
Imports Microsoft.VisualBasic.FileIO

Module LoadPresetsMethods

    ''' <summary>
    ''' <para>Parse, sort and display loaded presets data.</para>
    ''' <para>
    ''' For each valid preset : create a button with a .Click function 'HandleUserPresetClick'.
    ''' To be valid, each preset must have at least a Name and an Iwad.
    ''' readPresetsData = presets as lines read from 'presets.txt', values = each line.
    ''' | values(0) : Preset Name | values(1) : Preset Iwad | values(2) : Preset Level | values(3) : Preset Misc |
    ''' </para>
    ''' </summary>
    ''' 
    Sub DisplayLoadedPresets(readPresetsData As List(Of List(Of String)))

        'Nothing to read : exit
        If readPresetsData Is Nothing Then
            Return
        End If

        Dim mainWindow As MainWindow = Windows.Application.Current.Windows(0)

        For Each values As List(Of String) In readPresetsData
            If values.Count < 2 Then
                Continue For 'invalid preset
            End If

            Dim button As Button = New Button() With
            {
                .Margin = New Thickness(0, 0, 0, 2),
                .Height = 28,
                .FontSize = 14,
                .Content = values(0)
            }

            AddHandler button.Click,
                Sub(sender, e)
                    HandleUserPresetClick(values(1), If(values.Count >= 3, values(2), Nothing), If(values.Count = 4, values(3), Nothing))
                    'TODO v2+ : HandleUserPresetDelete() ?
                End Sub

            mainWindow.Label_NoUserPresetsFound.Visibility = Visibility.Collapsed
            mainWindow.StackPanel_DisplayUserPresets.Children.Add(button)
        Next

    End Sub

    ''' <summary>
    ''' Format then return parsed values, from file "presets.txt".
    ''' As List of List of String
    ''' 
    ''' For instance : ("MyPreset","path\to\doom.wad","path\to\level.wad"), ("Freedoom Phase 1", "path\to\freedoom1.wad")
    ''' </summary>
    ''' 
    Function FormatPresetsData_FromCsv() As List(Of List(Of String))

        Dim presetLines As List(Of List(Of String)) = New List(Of List(Of String))

        Try
            Using parser As TextFieldParser = New TextFieldParser(My.Settings.RootDirPath & "\presets.csv") With {
                .TextFieldType = FieldType.Delimited,
                .Delimiters = New String() {","},
                .CommentTokens = New String() {"#"}
            }

                Do While Not parser.EndOfData
                    Dim line As List(Of String) = New List(Of String)

                    Try
                        Dim readValues As String() = parser.ReadFields() 'a line in file : contains preset values (Name, Iwad, Level, Misc.)

                        If readValues.Length < 2 Then 'line has less than 2 values (Name and Iwad are mandatory) => Continue to next line
                            Continue Do
                        End If

                        For Each value As String In readValues
                            line.Add(value)
                        Next
                        presetLines.Add(line)

                    Catch ex As MalformedLineException
                        WriteToLog(DateTime.Now & " - Error : Got MalformedLineException when parsing 'presets.csv' at line(?) => " & parser.ErrorLineNumber)
                    End Try
                Loop

            End Using
        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'FormatPresetsData_FromCsv()'. Exception : " & ex.ToString)
        End Try

        Return presetLines

    End Function

    ''' <summary>
    ''' Triggered when user clicks a preset in list (second tab : User presets).
    ''' Handle files validation : write red text if invalid file
    ''' 
    ''' Set SelectedIwad, SelectedLevel, Selected Misc. for launch
    ''' </summary>
    ''' 
    Sub HandleUserPresetClick(iwadPath As String, Optional levelPath As String = Nothing, Optional miscPath As String = Nothing)

        'Dim str As String = String.Format(
        '    "iwadPath: '{0}'{1}levelPath:'{2}'{3}miscPath:'{4}'",
        '    iwadPath,
        '    Environment.NewLine & Environment.NewLine, levelPath,
        '    Environment.NewLine & Environment.NewLine, miscPath)
        'MessageBox.Show(str)

        Try
            With My.Settings

                Dim mainWindow As MainWindow = Windows.Application.Current.Windows(0)

                'Reset TextBox.Foreground
                mainWindow.TextBox_IwadToLaunch.ClearValue(Control.ForegroundProperty)
                mainWindow.TextBox_LevelToLaunch.ClearValue(Control.ForegroundProperty)
                mainWindow.TextBox_MiscToLaunch.ClearValue(Control.ForegroundProperty)

                If ValidateFile(iwadPath) = "iwad" Then
                    .SelectedIwad = iwadPath
                Else
                    mainWindow.TextBox_IwadToLaunch.Foreground = New SolidColorBrush(Colors.Red)
                End If
                mainWindow.TextBox_IwadToLaunch.Text = iwadPath

                If ValidateFile(levelPath) = "level" Then
                    .SelectedLevel = levelPath
                Else
                    mainWindow.TextBox_LevelToLaunch.Foreground = New SolidColorBrush(Colors.Red)
                End If
                mainWindow.TextBox_LevelToLaunch.Text = levelPath

                If ValidateFile(miscPath) = "misc" Then
                    .SelectedMisc = miscPath
                Else
                    mainWindow.TextBox_MiscToLaunch.Foreground = New SolidColorBrush(Colors.Red)
                End If
                mainWindow.TextBox_MiscToLaunch.Text = miscPath

                WriteToLog(DateTime.Now & " - From 'HandleUserPresetClick()' : " &
                           Environment.NewLine & "Preset IWAD : " & iwadPath &
                           Environment.NewLine & "Preset level : " & levelPath &
                           Environment.NewLine & "Preset Misc. :" & miscPath)
            End With

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'HandleUserPresetClick()'. Exception : " & ex.ToString)
        End Try

    End Sub

End Module
