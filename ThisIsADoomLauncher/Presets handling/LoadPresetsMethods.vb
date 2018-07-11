Imports System.IO

Module LoadPresetsMethods

    ''' <summary>
    ''' <para>Parse, sort and display loaded presets data.</para>
    ''' <para>
    ''' For each valid preset : create a button with a .Click function 'HandleUserPresetClick'.
    ''' To be valid, each preset must have at least a Name and an Iwad.
    ''' readPresetsData = presets as lines read from 'presets.txt', values = each line
    ''' | values(0) : Preset Name | values(1) : Preset Iwad | values(2) : Preset Level | values(3) : Preset Misc |
    ''' </para>
    ''' </summary>
    ''' 
    Sub DisplayLoadedPresets(readPresetsData As List(Of List(Of String)))

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
                End Sub

            mainWindow.Label_NoUserPresetsFound.Visibility = Visibility.Collapsed
            mainWindow.StackPanel_DisplayUserPresets.Children.Add(button)
        Next

    End Sub

    ''' <summary>
    ''' Return parsed values, from file "presets.txt".
    ''' As a list of list of string
    ''' 
    ''' For instance : ("MyFirstPreset","Doom2","Level.wad"), ("MySecondPreset", "Doom2", "AnotherLevel.wad")
    ''' </summary>
    ''' 
    Function GetPresetsFromFile(presetFile As String) As List(Of List(Of String))

        Dim presetLines As List(Of List(Of String)) = New List(Of List(Of String))

        If Not File.Exists(My.Settings.RootDirPath & "\presets.txt") Then
            Return presetLines 'presetLines is Nothing
        End If

        Try
            For Each line As String In File.ReadLines(My.Settings.RootDirPath & "\presets.txt")

                If line.Trim.StartsWith("#") Then
                    Continue For 'Ignore lines with "#" as first read char
                End If

                If line.Contains("Name =") And line.Contains("IWAD =") Then

                    Dim argLine As List(Of String) = New List(Of String)
                    Dim _start As Integer
                    Dim _end As Integer

                    'Name -------------------------------------------------------------------------------------
                    _start = line.IndexOf("Name =") + 6
                    _end = line.IndexOf("IWAD =") - 6
                    Dim presetName As String = line.Substring(_start, _end).Trim 'need test
                    argLine.Add(presetName)

                    'IWAD -------------------------------------------------------------------------------------
                    Dim presetIwad As String = ""
                    If Not line.Contains("Level =") Then
                        _start = line.IndexOf("IWAD =") + 6
                        presetIwad = line.Substring(_start).Trim 'need test
                        argLine.Add(presetIwad)
                        presetLines.Add(argLine) '=> Return preset with : Name, IWAD
                        Continue For
                    End If
                    _start = line.IndexOf("IWAD =") + 6
                    _end = line.IndexOf("Level =")
                    presetIwad = line.Substring(_start, _end - _start).Trim 'need test
                    argLine.Add(presetIwad)

                    'Level -------------------------------------------------------------------------------------
                    Dim presetLevel As String = ""
                    If Not line.Contains("Misc. =") Then
                        _start = line.IndexOf("Level =") + 7
                        presetLevel = line.Substring(_start).Trim 'need test
                        argLine.Add(presetLevel)
                        presetLines.Add(argLine) '=> Return preset with : Name, IWAD, Level
                        Continue For
                    End If
                    _start = line.IndexOf("Level =") + 7
                    _end = line.IndexOf("Misc. =")
                    presetLevel = line.Substring(_start, _end - _start).Trim 'need test
                    argLine.Add(presetLevel)

                    'Misc. -------------------------------------------------------------------------------------
                    _start = line.IndexOf("Misc. =") + 7
                    Dim presetMisc As String = line.Substring(_start).Trim 'need test
                    argLine.Add(presetMisc)
                    presetLines.Add(argLine) '=> Return preset with : Name, IWAD, Level, Misc.
                End If
            Next

        Catch ex As Exception
            MessageBox.Show("Error. Exception :" & Environment.NewLine & ex.ToString)
        End Try

        Return presetLines

    End Function

    ''' <summary>
    ''' Triggered when user clicks a preset in list(second tab:User presets)
    ''' Handle files validation
    ''' 
    ''' Set SelectedIwad, SelectedLevel, Selected Misc. for launch
    ''' </summary>
    ''' 
    Sub HandleUserPresetClick(iwadPath As String, Optional levelPath As String = Nothing, Optional miscPath As String = Nothing)

        Dim str As String = String.Format(
                "iwadPath:'{0}'{1}levelPath:'{2}'{3}miscPath:'{4}'",
                iwadPath,
                Environment.NewLine & Environment.NewLine, levelPath,
                Environment.NewLine & Environment.NewLine, miscPath)
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

                '.SelectedIwad = If(ValidateFile(iwadPath) = "iwad", iwadPath, Nothing)
                '.SelectedLevel = If(ValidateFile(levelPath) = "level", levelPath, Nothing)
                '.SelectedMisc = If(File.Exists(miscPath), miscPath, Nothing)mainWindow.TextBox_MiscToLaunch.Text = .SelectedMisc

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
