Imports System.IO

Module SavePresetMethods

    ''' <summary>
    ''' Write attributes for New user preset.
    ''' As line in 'presets.txt'
    ''' </summary>
    ''' 
    Sub WritePresetToFile(name As String, iwad As String, Optional level As String = Nothing, Optional misc As String = Nothing)

        Try
            Dim presetFile As String = My.Settings.RootDirPath & "\presets.txt"

            If Not File.Exists(presetFile) Then
                WritePresetsFileHeader()
            End If

            Dim presetLine As String = String.Format("Name = {0} IWAD = {1}", name, iwad)
            presetLine &= If(level = Nothing, Nothing, " Level = " & level)
            presetLine &= If(misc = Nothing, Nothing, " Misc. = " & misc)

            'Write preset
            Using writer As StreamWriter = New StreamWriter(presetFile, True, Text.Encoding.UTF8)
                writer.WriteLine(presetLine)
            End Using

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'WritePresetToFile()'. Exception : " & ex.ToString)
        End Try

    End Sub

End Module
