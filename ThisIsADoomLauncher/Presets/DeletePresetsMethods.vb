Imports System.IO

Module DeletePresetsMethods

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
