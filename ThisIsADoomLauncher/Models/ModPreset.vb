Namespace Models
    Public Class ModPreset
        Inherits Preset

        Public Property Desc As String

        Private _shortDesc As String
        Public ReadOnly Property ShortDesc As String
            Get
                If _shortDesc = Nothing Then
                    If Desc.Length > 80 Then
                        _shortDesc = $"{Desc.Substring(0, 80).Trim}[...]"
                    Else
                        _shortDesc = Desc
                    End If
                End If
                    Return _shortDesc
            End Get
        End Property

        Private _filesTotalResult As String
        Public ReadOnly Property FilesTotalResult As String
            Get
                If _filesTotalResult = Nothing Then
                    If Files.Count = 1 Then
                        _filesTotalResult = $"({Files.Count} file)"
                    Else
                        _filesTotalResult = $"({Files.Count} files)"
                    End If
                End If
                Return _filesTotalResult
            End Get
        End Property

        Public Property Files As List(Of String)

    End Class
End Namespace