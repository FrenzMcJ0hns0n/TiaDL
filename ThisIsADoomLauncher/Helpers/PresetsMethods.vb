Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.FileIO
Imports ThisIsADoomLauncher.Models

Friend Module PresetsMethods

    'TODO? v3+ Use JSON instead of CSV
    'Well, we will see...


#Region "Currently enabled"

    'TODO: Do more tests + TODO? Rewrite
    ''' <summary>
    ''' Configure the TextFieldParser object to parse CSV data
    ''' </summary>
    ''' <param name="presetsType">Type of content to get</param>
    ''' <returns></returns>
    Public Function ConfigureTextFieldParser(presetsType As String) As TextFieldParser
        Dim parser As TextFieldParser = Nothing

        Try
            Select Case presetsType
                Case "base_levels"
                    parser = New TextFieldParser(New StringReader(My.Resources.base_presets_Levels))
                Case "base_mods"
                    parser = New TextFieldParser(New StringReader(My.Resources.base_presets_Mods))
                Case "user_levels"
                    parser = New TextFieldParser(Path.Combine(GetDirectoryPath(), "presets.csv"))
                Case "user_mods"
                    'TODO
                Case Else 'TODO?
            End Select

            With parser
                .CommentTokens = New String() {"#"}
                .Delimiters = New String() {";"}
                .TextFieldType = FieldType.Delimited
                .TrimWhiteSpace = True
            End With

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {presetsType}")
        End Try

        Return parser
    End Function

    Public Function GetLevelPresets_FromCsv(presetsType As String) As List(Of LevelPreset)
        Dim levelPresets As New List(Of LevelPreset)

        Try
            Dim parser As TextFieldParser = ConfigureTextFieldParser(presetsType)
            Using parser
                Do While Not parser.EndOfData
                    Try
                        Dim parsedValues As String() = parser.ReadFields()

                        'Name, Type, Year and Iwad are mandatory
                        If parsedValues.Length < 4 Then Continue Do

                        'Skip Wolf3D, as not functional yet
                        If parsedValues(0) = "Wolfenstein 3D" Then Continue Do

                        levelPresets.Add(New LevelPreset() With
                        {
                            .Name = parsedValues(0),
                            .Type = parsedValues(1),
                            .Year = Convert.ToInt32(parsedValues(2)),
                            .Desc = $"Release year : { .Year}{vbCrLf}Type : { .Type}",
 _
                            .Iwad = parsedValues(3),
                            .Maps = If(parsedValues.Length >= 5, parsedValues(4), String.Empty),
                            .Misc = If(parsedValues.Length >= 6, parsedValues(5), String.Empty),
                            .Pict = If(parsedValues.Length = 7, parsedValues(6), String.Empty)
                        })
                    Catch exception As MalformedLineException
                        WriteToLog(Date.Now & " - Error : Got MalformedLineException while parsing presets") ' use errorLine ?
                    End Try
                Loop
            End Using
        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {presetsType}")
        End Try

        Return levelPresets
    End Function

    Public Function GetModPresets_FromCsv(presetsType As String) As List(Of ModPreset)
        Dim modPresets As New List(Of ModPreset)

        Try
            Dim parser As TextFieldParser = ConfigureTextFieldParser(presetsType)
            Using parser
                Do While Not parser.EndOfData
                    Try
                        Dim parsedValues As String() = parser.ReadFields()
                        'If a preset does not contain all info, it is ignored (useless safety ?)
                        If parsedValues.Length < 4 Then Continue Do

                        modPresets.Add(New ModPreset() With
                        {
                            .Name = parsedValues(0),
                            .Desc = parsedValues(1),
                            .Pict = parsedValues(2),
                            .Files = parsedValues(3).Split(",").ToList
                        })
                    Catch exception As MalformedLineException
                        WriteToLog(Date.Now & " - Error : Got MalformedLineException while parsing presets") ' use errorLine ?
                    End Try
                Loop
            End Using
        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {presetsType}")
        End Try

        Return modPresets
    End Function

#End Region


#Region "Not used yet / to implement"

    Public Sub DeletePreset(name As String)

        Dim rootDirPath = GetDirectoryPath()
        Dim presetFile As String = Path.Combine(rootDirPath, "presets.csv")

        Dim lines As List(Of String) = File.ReadAllLines(presetFile).ToList
        Dim count As Integer = 0

        For Each line As String In lines
            count += 1
            If line.StartsWith(name) Then
                lines.RemoveAt(count - 1)
                File.WriteAllLines(presetFile, lines)
                Exit For
            End If
        Next

    End Sub

    '''' <summary>
    '''' <para>Triggered when user right-clicks a custom preset</para>
    '''' <para>Delete a preset by its name</para>
    '''' </summary>
    ''''
    'Public Sub DeleteUserLevelPreset(presetName As String)

    '    Dim message As String = String.Format("Delete preset ""{0}"" ?", presetName)

    '    If MessageBox.Show(message, "Delete user preset", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
    '        DeletePreset(presetName)
    '        DisplayUserPresets(GetLevelPresets_FromCsv("user")) 'Update GUI
    '    End If

    'End Sub


    'Public  Function ReturnUserPresetButtons(presetsList As List(Of LevelPreset)) As List(Of Button)

    '    Dim buttonsList As List(Of Button) = New List(Of Button)

    '    For Each preset As LevelPreset In presetsList
    '        Dim button As Button = New Button() With
    '        {
    '            .Height = 28,
    '            .Margin = New Thickness(0, 0, 0, 2),
    '            .FontSize = 14,
    '            .Content = preset.Name
    '        }

    '        'Left click
    '        AddHandler button.Click,
    '            Sub(sender, e)
    '                'SelectUserLevelPreset(preset.Iwad, If(preset.Level, Nothing), If(preset.Misc, Nothing))
    '            End Sub
    '        'Right click
    '        AddHandler button.MouseRightButtonDown,
    '            Sub(sender, e)
    '                DeleteUserLevelPreset(preset.Name)
    '            End Sub

    '        buttonsList.Add(button)
    '    Next

    '    Return buttonsList

    'End Function


    '''' <summary>
    '''' Handle New user preset save from GUI event
    '''' </summary>
    '''' 
    'Public Sub Save_NewPreset()

    '    Dim mainWindow As MainWindow = Windows.Application.Current.Windows(0)

    '    With mainWindow
    '        Try
    '            Dim nameToSave As String = .TextBox_NewPreset_Name.Text
    '            If nameToSave = "Enter preset name..." Or nameToSave = Nothing Then
    '                MessageBox.Show("New user preset requires a name to be saved")
    '                Return
    '            End If

    '            Dim iwadToSave As String = Nothing 'KnowSelectedIwad_NewPreset()
    '            If iwadToSave = Nothing Then
    '                MessageBox.Show("New user preset requires an IWAD to be saved")
    '                Return
    '            End If

    '            Dim levelToSave As String = Nothing 'KnowSelectedLevel_NewPreset()
    '            Dim miscToSave As String = Nothing 'KnowSelectedMisc_NewPreset()

    '            WritePresetToFile(nameToSave, iwadToSave, levelToSave, miscToSave)
    '            MessageBox.Show(String.Format("Preset ""{0}"" saved !", nameToSave))

    '        Catch ex As Exception
    '            WriteToLog(DateTime.Now & " - Error in 'Button_NewPreset_Save_Click()'. Exception : " & ex.ToString)
    '        End Try
    '    End With

    'End Sub


    'TODO Use LevelPreset as input parameter
    ''' <summary>
    ''' Write attributes for New user preset.
    ''' As line in 'presets.csv'
    ''' </summary>
    ''' 
    Public Sub WritePresetToFile(name As String, iwad As String, Optional level As String = Nothing, Optional misc As String = Nothing)

        Try
            'Check if user-presets file exists
            Dim rootDirPath = GetDirectoryPath()
            Dim presetFile As String = Path.Combine(rootDirPath, "presets.csv")
            If Not File.Exists(presetFile) Then WritePresetsFileHeader()

            'Format preset to be written
            Dim presetLine As String = String.Format("{0},{1}", name, iwad)
            presetLine &= If(level = Nothing, Nothing, "," & level)
            presetLine &= If(misc = Nothing, Nothing, "," & misc)

            'Check if last char is CR-LF (Windows EOL) 'TODO! Remove that and do clean things
            Dim end_ok As Boolean = False
            Using reader As New StreamReader(presetFile, Text.Encoding.UTF8)
                reader.BaseStream.Seek(-2, SeekOrigin.End)
                If reader.Read = 13 Then end_ok = True
            End Using

            'Write new preset at end of file
            Using writer As New StreamWriter(presetFile, True, Text.Encoding.UTF8)
                If end_ok = False Then writer.WriteLine() 'create new line if necessary
                writer.WriteLine(presetLine)
            End Using

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : (TODO)")
        End Try

    End Sub

#End Region


End Module
