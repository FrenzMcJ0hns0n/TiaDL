Namespace Models
    Public Class InputFile

        Public Property Name As String
        Public Property Extension As String

        Public Property Directory As String

        Public Sub New(name As String, extension As String, directory As String)
            Me.Name = name
            Me.Extension = extension
            Me.Directory = directory
        End Sub

        Public Sub CutDirectoryPath()
            If Directory.Length > 30 Then
                Directory = Directory.Substring(0, 30) & "..."
            End If
        End Sub

    End Class
End Namespace