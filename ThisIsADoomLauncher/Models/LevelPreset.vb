Namespace Models
    Public Class LevelPreset

        Private _name As String
        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property

        Private _iwad As String
        Public Property Iwad As String
            Get
                Return _iwad
            End Get
            Set(value As String)
                _iwad = value
            End Set
        End Property

        Private _level As String
        Public Property Level As String
            Get
                Return _level
            End Get
            Set(value As String)
                _level = value
            End Set
        End Property

        Private _misc As String
        Public Property Misc As String
            Get
                Return _misc
            End Get
            Set(value As String)
                _misc = value
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

    End Class
End Namespace
