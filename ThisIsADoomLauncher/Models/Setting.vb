Namespace Models
    Public Class Setting

        Public Property Port As String
        Public Property Iwad As String
        Public Property Level As String
        Public Property Misc As String
        Public Property PortParameters As List(Of String) = New List(Of String)
        Public Property Mods As List(Of String) = New List(Of String)

        'Public Sub New(Port As String, Iwad As String, Level As String, Misc As String, PortParameters As List(Of String), FilesMods As List(Of String))
        '    Me.Port = Port
        '    Me.Iwad = Iwad
        '    Me.Level = Level
        '    Me.Misc = Misc
        '    Me.PortParameters = PortParameters
        '    Me.FilesMods = FilesMods
        'End Sub

    End Class
End Namespace