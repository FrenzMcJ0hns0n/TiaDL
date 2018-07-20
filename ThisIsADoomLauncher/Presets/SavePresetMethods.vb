Imports System.IO

Module SavePresetMethods

    ''' <summary>
    ''' Handle New user preset save from GUI event
    ''' </summary>
    ''' 
    Sub Save_NewPreset()

        With MainWindow_Instance()
            Try
                Dim nameToSave As String = .TextBox_NewPreset_Name.Text
                If nameToSave = "Enter preset name ..." Or nameToSave = Nothing Then
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
            Dim presetFile As String = My.Settings.RootDirPath & "\presets.csv"

            If Not File.Exists(presetFile) Then
                WritePresetsFileHeader()
            End If

            Dim presetLine As String = String.Format("{0},{1}", name, iwad)
            presetLine &= If(level = Nothing, Nothing, "," & level)
            presetLine &= If(misc = Nothing, Nothing, "," & misc)

            'Write preset
            Using writer As StreamWriter = New StreamWriter(presetFile, True, Text.Encoding.UTF8)
                writer.WriteLine("") 'to make sure to write 'presetLine' on a new line
                writer.WriteLine(presetLine)
            End Using

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'WritePresetToFile()'. Exception : " & ex.ToString)
        End Try

    End Sub

End Module
