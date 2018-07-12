Imports System.IO

Module SavePresetMethods

    ''' <summary>
    ''' Write attributes for New user preset.
    ''' As line in 'presets.txt'
    ''' </summary>
    ''' 
    Sub WritePresetToFile(name As String, iwad As String, Optional level As String = Nothing, Optional misc As String = Nothing)

        Try
            Dim pathToPreset As String = My.Settings.RootDirPath & "\presets.txt"

            If Not File.Exists(pathToPreset) Then
                Using streamWriter As New StreamWriter(pathToPreset, True, Text.Encoding.Unicode)
                    streamWriter.WriteLine("# Lines starting with ""#"" are ignored by the program")
                    streamWriter.WriteLine()
                    streamWriter.WriteLine("# Preset pattern :")
                    streamWriter.WriteLine("# Name = <value> IWAD = <path> [Level = <path> Misc. = <path>]")
                    streamWriter.WriteLine("# 'Name' and 'IWAD' are mandatory values")
                    streamWriter.WriteLine("# 'Misc.' refers to .deh or .bex files")
                    streamWriter.WriteLine("")
                    streamWriter.WriteLine("# Presets")
                End Using
            End If

            'Write preset
            Using streamWriter As New StreamWriter(pathToPreset, True, Text.Encoding.Unicode)

                Dim presetLine As String = String.Format("Name = {0} IWAD = {1}", name, iwad)

                presetLine &= If(level = Nothing, Nothing, " Level = " & level)
                presetLine &= If(misc = Nothing, Nothing, " Misc = " & misc)

                streamWriter.WriteLine(presetLine)

            End Using

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'WritePresetToFile()'. Exception : " & ex.ToString)
        End Try

    End Sub

End Module
