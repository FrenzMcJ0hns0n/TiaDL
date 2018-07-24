Namespace Models
    Public Class Preset

        Private _name As String
        Public Property Name As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Private _iwad As String
        Public Property Iwad As String
            Get
                Return _iwad
            End Get
            Set(ByVal value As String)
                _iwad = value
            End Set
        End Property

        Private _level As String
        Public Property Level As String
            Get
                Return _level
            End Get
            Set(ByVal value As String)
                _level = value
            End Set
        End Property

        Private _misc As String
        Public Property Misc As String
            Get
                Return _misc
            End Get
            Set(ByVal value As String)
                _misc = value
            End Set
        End Property

        Public Function Attributes_GetCount(p As Preset) As Integer

            If Not p.Level = Nothing And Not p.Misc = Nothing Then
                Return 4
            ElseIf Not p.Level = Nothing And p.Misc = Nothing Then
                Return 3
            End If
            Return 0

        End Function

    End Class
End Namespace
