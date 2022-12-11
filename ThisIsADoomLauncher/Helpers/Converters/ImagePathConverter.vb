Imports System.Globalization
Imports System.IO

Namespace Helpers.Converters
    Public Class ImagePathConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert

            If value.ToString = String.Empty Then Return "pack://application:,,,/Resources/Images/Presets/icon_default.png"

            Return Path.Combine("pack://application:,,,/Resources/Images/Presets/Common", value.ToString)

        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack

            Throw New NotImplementedException() 'Useless !?

        End Function

    End Class
End Namespace
