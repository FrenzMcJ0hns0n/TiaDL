Namespace Models
    Public Class Launcher

        Private _port As String
        Public Property Port As String
            Get
                Return _port
            End Get
            Set(value As String)
                _port = value
            End Set
        End Property

        Private _port_params As List(Of String)
        Public Property Port_Params As List(Of String)
            Get
                Return _port_params
            End Get
            Set(value As List(Of String))
                _port_params = value
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

        Private _files_mods As List(Of String)
        Public Property Files_Mods As List(Of String)
            Get
                Return _files_mods
            End Get
            Set(value As List(Of String))
                _files_mods = value
            End Set
        End Property

    End Class
End Namespace
