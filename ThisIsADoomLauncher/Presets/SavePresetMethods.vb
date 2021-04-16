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

End Module
