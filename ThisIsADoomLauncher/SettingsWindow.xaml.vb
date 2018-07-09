Imports System.Text.RegularExpressions

Public Class SettingsWindow

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)

        With My.Settings

            'Engine
            If .SelectedEngine.ToLowerInvariant = "gzdoom" Then
                RadioButton_Engine_GZDoom.IsChecked = True
            ElseIf .SelectedEngine.ToLowerInvariant = "zandronum" Then
                RadioButton_Engine_Zandronum.IsChecked = True
            End If

            'Resolution
            TextBox_Resolution_Width.Text = .ScreenWidth
            TextBox_Resolution_Height.Text = .ScreenHeight

            If .FullscreenEnabled Then
                CheckBox_Fullscreen.IsChecked = True
            End If

            'Brutal Doom
            ComboBox_BrutalDoomVersions.ItemsSource = GetLocalBrutalDoomVersions() '-> Fill BD versions ComboBox

            If .UseBrutalDoom Then
                CheckBox_UseBrutalDoom.IsChecked = True
            End If

            ComboBox_BrutalDoomVersions.SelectedItem = If(.BrutalDoomVersion, Nothing)

            'If Not .BrutalDoomVersion Is Nothing Then
            '    'TODO : Does the program crash if ConfirmedBruVer string is not in ComboBox ?
            '    ComboBox_BrutalDoomVersions.SelectedItem = .BrutalDoomVersion
            'End If

        End With

    End Sub

    Private Sub TextBox_Resolution_Width_PreviewTextInput(sender As Object, e As TextCompositionEventArgs) Handles TextBox_Resolution_Width.PreviewTextInput

        'Restrict input to integers
        e.Handled = New Regex("[^0-9]+").IsMatch(e.Text)

    End Sub

    Private Sub TextBox_Resolution_Height_PreviewTextInput(sender As Object, e As TextCompositionEventArgs) Handles TextBox_Resolution_Height.PreviewTextInput

        'Restrict input to integers
        e.Handled = New Regex("[^0-9]+").IsMatch(e.Text)

    End Sub

    Private Sub Button_GetSetResolution_Click(sender As Object, e As RoutedEventArgs) Handles Button_GetSetResolution.Click

        'Get Windows screen resolution
        TextBox_Resolution_Width.Text = My.Computer.Screen.Bounds.Size.Width
        TextBox_Resolution_Height.Text = My.Computer.Screen.Bounds.Size.Height

        With My.Settings
            .ScreenWidth = My.Computer.Screen.Bounds.Size.Width
            .ScreenHeight = My.Computer.Screen.Bounds.Size.Height
            .Save()
        End With


    End Sub

    Private Sub CheckBox_UseBrutalDoom_Checked(sender As Object, e As RoutedEventArgs) Handles CheckBox_UseBrutalDoom.Checked

        With My.Settings
            .UseBrutalDoom = True
            ComboBox_BrutalDoomVersions.IsEnabled = True
            ComboBox_BrutalDoomVersions.SelectedItem = If(.BrutalDoomVersion, Nothing)
            .Save()
        End With

    End Sub

    Private Sub CheckBox_UseBrutalDoom_UnChecked(sender As Object, e As RoutedEventArgs) Handles CheckBox_UseBrutalDoom.Unchecked

        With My.Settings
            .UseBrutalDoom = False
            ComboBox_BrutalDoomVersions.IsEnabled = False
            ComboBox_BrutalDoomVersions.SelectedItem = Nothing
            .Save()
        End With


    End Sub

    Private Sub Button_OkClose_Click(sender As Object, e As RoutedEventArgs) Handles Button_OkClose.Click

        Close()

    End Sub

    Private Sub RadioButton_Engine_GZDoom_Checked(sender As Object, e As RoutedEventArgs) Handles RadioButton_Engine_GZDoom.Checked

        With My.Settings
            .SelectedEngine = "GZDoom"
            .Save()
        End With

    End Sub

    Private Sub RadioButton_Engine_Zandronum_Checked(sender As Object, e As RoutedEventArgs) Handles RadioButton_Engine_Zandronum.Checked

        With My.Settings
            .SelectedEngine = "Zandronum"
            .Save()
        End With

    End Sub

    Private Sub ComboBox_BrutalDoomVersions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ComboBox_BrutalDoomVersions.SelectionChanged

        With My.Settings
            .BrutalDoomVersion = .ModDir & "\" & ComboBox_BrutalDoomVersions.SelectedValue
            .Save()
        End With

    End Sub

    Private Sub TextBox_Resolution_Width_LostFocus(sender As Object, e As RoutedEventArgs) Handles TextBox_Resolution_Width.LostFocus

        With My.Settings
            .ScreenWidth = If(TextBox_Resolution_Width.Text = Nothing, 0, Int(TextBox_Resolution_Width.Text))
            .Save()
        End With

    End Sub

    Private Sub TextBox_Resolution_Height_LostFocus(sender As Object, e As RoutedEventArgs) Handles TextBox_Resolution_Height.LostFocus

        With My.Settings
            .ScreenHeight = If(TextBox_Resolution_Height.Text = Nothing, 0, Int(TextBox_Resolution_Height.Text))
            .Save()
        End With

    End Sub

End Class
