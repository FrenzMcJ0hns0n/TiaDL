Imports System.IO

Module GuiHelper




#Region "Common presets"

    ''' <summary>
    ''' Build absolute path from Iwad relative filename (Common presets)
    ''' </summary>
    ''' 
    Private Function Path_Iwad_RelativeToAbsolute(iwad As String) As String

        With My.Settings
            Return If(File.Exists(.IwadsDir & "\" & iwad), .IwadsDir & "\" & iwad, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Build absolute path from Level relative filename (Common presets)
    ''' </summary>
    ''' 
    Private Function Path_Level_RelativeToAbsolute(level As String) As String

        With My.Settings
            Return If(File.Exists(.LevelsDir & "\" & level), .LevelsDir & "\" & level, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Build absolute path from Misc relative filename (Common presets)
    ''' </summary>
    ''' 
    Private Function Path_Misc_RelativeToAbsolute(misc As String) As String

        With My.Settings
            Return If(File.Exists(.MiscDir & "\" & misc), .MiscDir & "\" & misc, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Handle checks on Common presets values before confirm them
    ''' </summary>
    '''
    Sub ValidateCommonPreset(iwad As String, Optional level As String = Nothing, Optional misc As String = Nothing)

        Dim errorText As String = ""
        Dim enl As String = Environment.NewLine

        Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
        Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
        Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)

        With My.Settings
            If iwadPath = Nothing Then
                errorText &= String.Format("File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, enl, .IwadsDir, enl)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("File ""{0}"" doesn't exist in :{1}{2}{3}", level, enl, .IwadsDir, enl)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("File ""{0}"" doesn't exist in :{1}{2}{3}", misc, enl, .MiscDir, enl)
            End If

            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                MainWindow_Instance().TextBox_IwadToLaunch.Text = .SelectedIwad
                MainWindow_Instance().TextBox_LevelToLaunch.Text = .SelectedLevel
                MainWindow_Instance().TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show("Error : " & enl & enl & errorText)
            End If
        End With

    End Sub

#End Region




#Region "New user preset"

    ''' <summary>
    ''' Get Iwad file path from green-colored button
    ''' </summary>
    ''' 
    Function KnowSelectedIwad_NewPreset() As String

        With MainWindow_Instance()
            Dim iwadButton As SolidColorBrush = New SolidColorBrush()

            iwadButton = .Button_NewPreset_SetDoomIwad.Background
            If iwadButton.Color = Colors.LightGreen Then
                Return My.Settings.IwadsDir & "\Doom.wad"
            End If

            iwadButton = .Button_NewPreset_SetDoom2Iwad.Background
            If iwadButton.Color = Colors.LightGreen Then
                Return My.Settings.IwadsDir & "\Doom2.wad"
            End If

            iwadButton = .Button_NewPreset_SetFreedoomIwad.Background
            If iwadButton.Color = Colors.LightGreen Then
                Return My.Settings.IwadsDir & "\freedoom1.wad"
            End If

            iwadButton = .Button_NewPreset_SetFreedoom2Iwad.Background
            If iwadButton.Color = Colors.LightGreen Then
                Return My.Settings.IwadsDir & "\freedoom2.wad"
            End If

            Return Nothing
        End With

    End Function

    ''' <summary>
    ''' Get Level file path as text from TextBox_DropWadFile
    ''' </summary>
    ''' 
    Function KnowSelectedLevel_NewPreset() As String

        With MainWindow_Instance()
            Return If(.TextBox_DropWadFile.Text = "Drop a .wad/.pk3 file here ...", Nothing, .TextBox_DropWadFile.Text)
        End With

    End Function

    ''' <summary>
    ''' Get Misc file path as text from TextBox_DropMiscFile
    ''' </summary>
    ''' 
    Function KnowSelectedMisc_NewPreset() As String

        With MainWindow_Instance()
            Return If(.TextBox_DropMiscFile.Text = "Drop a .deh/.bex file here ...", Nothing, .TextBox_DropMiscFile.Text)
        End With

    End Function

    ''' <summary>
    ''' Reset GUI items to their default appearance
    ''' </summary>
    ''' 
    Sub ResetFields_NewPreset()

        With MainWindow_Instance()
            .Button_NewPreset_SetDoomIwad.Background = Brushes.Transparent
            .Button_NewPreset_SetDoom2Iwad.Background = Brushes.Transparent
            .Button_NewPreset_SetFreedoomIwad.Background = Brushes.Transparent
            .Button_NewPreset_SetFreedoom2Iwad.Background = Brushes.Transparent

            .TextBox_DropWadFile.FontStyle = FontStyles.Italic
            .TextBox_DropWadFile.Background = New SolidColorBrush(Colors.Transparent)
            .TextBox_DropWadFile.Foreground = New BrushConverter().ConvertFrom("#444")
            .TextBox_DropWadFile.Text = "Drop a .wad/.pk3 file here ..."

            .TextBox_DropMiscFile.FontStyle = FontStyles.Italic
            .TextBox_DropMiscFile.Background = New SolidColorBrush(Colors.Transparent)
            .TextBox_DropMiscFile.Foreground = New BrushConverter().ConvertFrom("#444")
            .TextBox_DropMiscFile.Text = "Drop a .deh/.bex file here ..."

            .TextBox_NewPreset_Name.FontStyle = FontStyles.Italic
            .TextBox_NewPreset_Name.Foreground = New BrushConverter().ConvertFrom("#444")
            .TextBox_NewPreset_Name.Text = "Enter preset name ..."
        End With

    End Sub

#End Region




End Module
