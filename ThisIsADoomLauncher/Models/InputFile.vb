Namespace Models
    Public Class InputFile

        Public Property Directory As String
        'Public Property ErrorsList As List(Of String)
        Public Property Extension As String
        Public Property Name As String
        Public Property Size As Integer



        'Public Sub New(name As String, extension As String, directory As String, size As Integer)
        '    Me.Directory = directory
        '    Me.Extension = extension
        '    Me.Name = name
        '    Me.Size = size
        'End Sub

        Public Sub New(filepath As String)
            Directory = GetFileInfo_Directory(filepath)
            Extension = GetFileInfo_Extension(filepath)
            Name = GetFileInfo_Name(filepath, True)
            Size = GetFileInfo_Size(filepath) '.ToString
        End Sub

        'Public Sub CutDirectoryPath()
        '    If Directory.Length > 30 Then
        '        Directory = Directory.Substring(0, 30) & "..."
        '    End If
        'End Sub

    End Class
End Namespace