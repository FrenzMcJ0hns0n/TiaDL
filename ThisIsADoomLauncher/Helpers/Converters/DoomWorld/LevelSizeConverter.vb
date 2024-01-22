Imports System.Globalization

Namespace Helpers.Converters.DoomWorld
    Public Class LevelSizeConverter : Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Dim levelSize As Long = CType(value, Long)
            Return IOHelper.GetFileInfo_Size2(levelSize)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace