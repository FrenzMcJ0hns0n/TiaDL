Imports System.Globalization
Imports System.IO

Namespace Helpers.Converters


    Public Class LvlImagePathConverter : Implements IValueConverter
        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If value.ToString = String.Empty Then
                Return "pack://application:,,,/Resources/Images/Presets/Lvl_default.png"
            Else
                Return Path.Combine("pack://application:,,,/Resources/Images/Presets/Common", value.ToString)
            End If
        End Function
        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class


    Public Class ModImagePathConverter : Implements IValueConverter
        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If value.ToString = String.Empty Then
                Return "pack://application:,,,/Resources/Images/Presets/Mod_default.png"
            Else
                Return Path.Combine("pack://application:,,,/Resources/Images/Presets/Common", value.ToString)
            End If
        End Function
        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class


End Namespace
