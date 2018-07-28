Imports System.Globalization
Imports System.IO

Namespace Helpers.Converters
    Public Class ImagePathConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If Not value = Nothing Then
                Return Path.Combine("pack://application:,,,/Resources/Images/", value)
            End If
            Return Nothing
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
