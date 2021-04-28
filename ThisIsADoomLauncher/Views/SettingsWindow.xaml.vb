Imports System.Text.RegularExpressions

Namespace Views
    Public Class SettingsWindow

        Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)

            Try
                With My.Settings
                    'Engine
                    RadioButton_Engine_GZDoom.IsChecked = If(.SelectedPort.ToLowerInvariant = "gzdoom", True, False)
                    RadioButton_Engine_Zandronum.IsChecked = If(.SelectedPort.ToLowerInvariant = "zandronum", True, False)

                    'Resolution
                    TextBox_Resolution_Width.Text = .ScreenWidth
                    TextBox_Resolution_Height.Text = .ScreenHeight
                    CheckBox_Fullscreen.IsChecked = .FullscreenEnabled

                    'Brutal Doom
                    CheckBox_UseBrutalDoom.IsChecked = .UseBrutalDoom
                    ComboBox_BrutalDoomVersions.ItemsSource = GetLocalBrutalDoomVersions() '-> Fill BD versions ComboBox
                    If .BrutalDoomVersion = Nothing Then
                        Return 'Quick fix : program crash if .BrutalDoomVersion = Nothing
                    End If
                    ComboBox_BrutalDoomVersions.SelectedValue = If(File_GetName(.BrutalDoomVersion), Nothing) '-> Get filename of .BrutalDoomVersion from full path

                End With

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'SettingsWindow:Window_Loaded()'. Exception : " & ex.ToString)
            End Try

        End Sub


        Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)

            My.Settings.Save()
            MainWindow_Instance.UpdateGUI()

        End Sub


        Private Sub Button_OkClose_Click(sender As Object, e As RoutedEventArgs) Handles Button_OkClose.Click

            Close()

        End Sub




#Region "Engine settings"

        Private Sub RadioButton_Engine_GZDoom_Checked(sender As Object, e As RoutedEventArgs) Handles RadioButton_Engine_GZDoom.Checked

            'MainWindow_Instance.Label_EngineToLaunch.Content = "GZDoom"

            With My.Settings
                .SelectedPort = "GZDoom"
                '.Save()
            End With

        End Sub

        Private Sub RadioButton_Engine_Zandronum_Checked(sender As Object, e As RoutedEventArgs) Handles RadioButton_Engine_Zandronum.Checked

            With My.Settings
                .SelectedPort = "Zandronum"
                '.Save()
            End With

        End Sub

#End Region




#Region "Screen settings"

        Private Sub TextBox_Resolution_Width_PreviewTextInput(sender As Object, e As TextCompositionEventArgs) Handles TextBox_Resolution_Width.PreviewTextInput

            'Restrict input to integers
            e.Handled = New Regex("[^0-9]+").IsMatch(e.Text)

        End Sub

        Private Sub TextBox_Resolution_Height_PreviewTextInput(sender As Object, e As TextCompositionEventArgs) Handles TextBox_Resolution_Height.PreviewTextInput

            'Restrict input to integers
            e.Handled = New Regex("[^0-9]+").IsMatch(e.Text)

        End Sub

        Private Sub TextBox_Resolution_Width_LostFocus(sender As Object, e As RoutedEventArgs) Handles TextBox_Resolution_Width.LostFocus

            With My.Settings
                .ScreenWidth = If(TextBox_Resolution_Width.Text = Nothing, 0, Int(TextBox_Resolution_Width.Text))
                '.Save()
            End With

        End Sub

        Private Sub TextBox_Resolution_Height_LostFocus(sender As Object, e As RoutedEventArgs) Handles TextBox_Resolution_Height.LostFocus

            With My.Settings
                .ScreenHeight = If(TextBox_Resolution_Height.Text = Nothing, 0, Int(TextBox_Resolution_Height.Text))
                '.Save()
            End With

        End Sub

        Private Sub CheckBox_Fullscreen_Checked(sender As Object, e As RoutedEventArgs) Handles CheckBox_Fullscreen.Checked

            With My.Settings
                .FullscreenEnabled = True
                '.Save()
            End With

        End Sub

        Private Sub CheckBox_Fullscreen_UnChecked(sender As Object, e As RoutedEventArgs) Handles CheckBox_Fullscreen.Unchecked

            With My.Settings
                .FullscreenEnabled = False
                '.Save()
            End With

        End Sub

        Private Sub Button_GetSetResolution_Click(sender As Object, e As RoutedEventArgs) Handles Button_GetSetResolution.Click

            'Set Windows screen resolution
            Dim horizontal As Integer = GetResolution_Width()
            Dim vertical As Integer = GetResolution_Height()

            TextBox_Resolution_Width.Text = horizontal
            TextBox_Resolution_Height.Text = vertical

            With My.Settings
                .ScreenWidth = horizontal
                .ScreenHeight = vertical
                '.Save()
            End With

        End Sub

#End Region




#Region "Brutal Doom settings"

        Private Sub CheckBox_UseBrutalDoom_Checked(sender As Object, e As RoutedEventArgs) Handles CheckBox_UseBrutalDoom.Checked

            With My.Settings
                .UseBrutalDoom = True
                ComboBox_BrutalDoomVersions.IsEnabled = True
                ComboBox_BrutalDoomVersions.SelectedItem = If(.BrutalDoomVersion, Nothing)
                '.Save()
            End With

        End Sub

        Private Sub CheckBox_UseBrutalDoom_UnChecked(sender As Object, e As RoutedEventArgs) Handles CheckBox_UseBrutalDoom.Unchecked

            With My.Settings
                .UseBrutalDoom = False
                ComboBox_BrutalDoomVersions.IsEnabled = False
                ComboBox_BrutalDoomVersions.SelectedItem = Nothing
                '.Save()
            End With

        End Sub

        Private Sub ComboBox_BrutalDoomVersions_DropDownClosed(sender As Object, e As EventArgs) Handles ComboBox_BrutalDoomVersions.DropDownClosed

            With My.Settings
                .BrutalDoomVersion = .ModDir & "\" & ComboBox_BrutalDoomVersions.SelectedValue
                '.Save()
            End With

        End Sub


#End Region




    End Class
End Namespace