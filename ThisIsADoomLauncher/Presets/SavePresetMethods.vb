Imports System.IO

Module SavePresetMethods

    ''' <summary>
    ''' Write attributes for New user preset.
    ''' As line in 'presets.csv'
    ''' </summary>
    ''' 
    Sub WritePresetToFile(name As String, iwad As String, Optional level As String = Nothing, Optional misc As String = Nothing)

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
