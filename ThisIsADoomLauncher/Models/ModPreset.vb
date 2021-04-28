Namespace Models
    Public Class ModPreset

        Private _name As String
        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property

        Private _desc As String
        Public Property Desc As String
            Get
                Return _desc
            End Get
            Set(value As String)
                _desc = value
            End Set
        End Property

        Private _imagePath As String
        Public Property ImagePath As String
            Get
                Return _imagePath
            End Get
            Set(value As String)
                _imagePath = value
            End Set
        End Property

        Private _files As List(Of String)
        Public Property Files As List(Of String)
            Get
                Return _files
            End Get
            Set(value As List(Of String))
                _files = value
            End Set
        End Property

    End Class
End Namespace