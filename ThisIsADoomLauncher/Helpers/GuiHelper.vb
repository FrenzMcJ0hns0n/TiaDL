Module GuiHelper




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
