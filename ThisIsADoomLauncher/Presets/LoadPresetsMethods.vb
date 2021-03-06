﻿Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports ThisIsADoomLauncher.Models

Module LoadPresetsMethods

    ''' <summary>
    ''' <para>Build and display loaded presets as buttons.</para>
    ''' <para>For each preset : create a button with .Click functions.</para>
    ''' <para>values(0) : Preset Name | values(1) : Preset Iwad | values(2) : Preset Level | values(3) : Preset Misc</para>
    ''' </summary>
    ''' 
    Sub DisplayPresets(presetsList As List(Of Preset))

        With MainWindow_Instance()

            .StackPanel_UserPresets.Children.Clear()

            If presetsList.Count = 0 Then
                .Label_NoUserPresetsFound.Visibility = Visibility.Visible
                Return 'Nothing to read : exit
            End If

            .Label_NoUserPresetsFound.Visibility = Visibility.Collapsed

        End With

        For Each preset As Preset In presetsList

            Dim iwad As Boolean = If(preset.Level = Nothing And preset.Misc = Nothing, True, False)

            'Create a button for each preset -> TODO : improve style
            Dim button As Button = New Button() With
            {
                .Height = 28,
                .Margin = New Thickness(0, 0, 0, 2),
                .FontSize = 14,
                .Content = preset.Name
            }

            'Add anonymous functions to it, to handle clicks
            AddHandler button.Click,
                Sub(sender, e)
                    HandleUserPreset_Select(preset.Iwad, If(preset.Level, Nothing), If(preset.Misc, Nothing))
                End Sub

            AddHandler button.MouseRightButtonDown,
                Sub(sender, e)
                    HandleUserPreset_Delete(preset.Name)
                End Sub

            'Add the button into StackPanel
            MainWindow_Instance().StackPanel_UserPresets.Children.Add(button)

        Next

    End Sub

    ''' <summary>
    ''' Format then return parsed values, from 'csv' content. 
    ''' As a list of Presets.
    ''' </summary>
    ''' 
    Function FormatPresetsData_FromCsv(presetsType As String) As List(Of Preset)

        Dim presets As List(Of Preset) = New List(Of Preset)
        Dim parser As TextFieldParser = SetTextFieldParser(presetsType)

        Try

            Using parser
                Do While Not parser.EndOfData

                    Try
                        Dim readValues() As String = parser.ReadFields()

                        If readValues.Length < 2 Then
                            Continue Do
                        End If

                        presets.Add(
                            New Preset() With
                            {
                                .Name = readValues(0),
                                .Iwad = readValues(1),
                                .Level = If(readValues.Length >= 3, readValues(2), String.Empty),
                                .Misc = If(readValues.Length >= 4, readValues(3), String.Empty),
                                .ImagePath = If(readValues.Length = 5, readValues(4), String.Empty)
                            }
                        )
                    Catch ex As MalformedLineException
                        WriteToLog(DateTime.Now & " - Error : Got MalformedLineException while parsing presets") ' use errorLine ?
                    End Try

                Loop
            End Using

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'FormatPresetsData_FromCsv()'. Exception : " & ex.ToString)
        End Try

        Return presets

    End Function

    ''' <summary>
    ''' Return adequat TextFieldParser, from presetsType
    ''' </summary>
    ''' 
    Function SetTextFieldParser(presetsType As String) As TextFieldParser

        Dim parser As TextFieldParser = Nothing

        If presetsType = "common" Then
            parser = New TextFieldParser(New StringReader(My.Resources.common_presets))
        Else
            parser = New TextFieldParser(My.Settings.RootDirPath & "\presets.csv")
        End If

        With parser
            .TextFieldType = FieldType.Delimited
            .CommentTokens = New String() {"#"}
            .Delimiters = New String() {","}
            .TrimWhiteSpace = True
        End With

        Return parser

    End Function

End Module
