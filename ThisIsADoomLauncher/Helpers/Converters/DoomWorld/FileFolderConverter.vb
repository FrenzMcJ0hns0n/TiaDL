Imports System.Globalization
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models
Namespace Helpers.Converters.DoomWorld

    Public Class FileFolderConverter : Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert

            If value.GetType() Is GetType(Helpers.DoomWorld.Models.Folder) Then

                Return New Views.UserControls.DoomWorld.FolderControl With {.DataContext = value}

            ElseIf value.GetType() Is GetType(Helpers.DoomWorld.Models.Level) Then
                Dim level As Helpers.DoomWorld.Models.Level = CType(value, Helpers.DoomWorld.Models.Level)

                ''sometimes Title is empty -> replacing by Filename value
                'If String.IsNullOrWhiteSpace(level.Title) Then
                '    level.Title = level.Filename
                'End If

                Return New Views.UserControls.DoomWorld.LevelControl With {.DataContext = level}

            End If
            Return Nothing
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace