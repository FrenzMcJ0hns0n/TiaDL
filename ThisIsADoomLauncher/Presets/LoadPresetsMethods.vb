﻿Imports Microsoft.VisualBasic.FileIO

Module LoadPresetsMethods

    ''' <summary>
    ''' <para>Build and display loaded presets as buttons.</para>
    ''' <para>For each preset : create a button with .Click function 'HandleUserPresetClick'.</para>
    ''' <para>values(0) : Preset Name | values(1) : Preset Iwad | values(2) : Preset Level | values(3) : Preset Misc</para>
    ''' </summary>
    ''' 
    Sub DisplayLoadedPresets(readPresetsData As List(Of List(Of String)))

        MainWindow_Instance().StackPanel_DisplayUserPresets.Children.Clear()

        If readPresetsData.Count = 0 Then
            MainWindow_Instance().Label_NoUserPresetsFound.Visibility = Visibility.Visible
            Return 'Nothing to read : exit
        End If

        MainWindow_Instance().Label_NoUserPresetsFound.Visibility = Visibility.Collapsed

        For Each values As List(Of String) In readPresetsData

            Dim button As Button = New Button() With
            {
                .Margin = New Thickness(0, 0, 0, 2),
                .Height = 28,
                .FontSize = 14,
                .Content = values(0)
            }

            'TODO v2+ : HandleUserPresetDelete() ?
            AddHandler button.Click,
                Sub(sender, e)
                    HandleUserPresetClick(values(1), If(values.Count >= 3, values(2), Nothing), If(values.Count = 4, values(3), Nothing))
                End Sub

            AddHandler button.MouseRightButtonDown,
                Sub(sender, e)
                    HandleRightClick(values(0))
                End Sub

            MainWindow_Instance().StackPanel_DisplayUserPresets.Children.Add(button)
        Next

    End Sub

    ''' <summary>
    ''' <para> Format then return parsed values, from file "presets.txt". As List of List of String.</para>
    ''' <para>For instance : ("MyPreset","path\to\doom.wad","path\to\level.wad"), ("Freedoom Phase 1", "path\to\freedoom1.wad")</para>
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

End Module
