Imports System.Globalization
Imports System.IO

Namespace Helpers.Converters
    Public Class ImagePathConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert

            If value = Nothing Then
                Return "pack://application:,,,/Resources/Images/Presets/icon_default.png"
            Else
                Return Path.Combine("pack://application:,,,/Resources/Images/Presets/Common", value)
            End If

        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack

            Throw New NotImplementedException() 'Useless !?

        End Function

    End Class
End Namespace
