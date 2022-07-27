Namespace Models
    Public Class LevelPreset
        Inherits Preset


        'Preset properties
        Public Property Type As String 'Assets type : Doom 1 / Doom2 / Other
        Public Property Year As Integer 'Year of last release

        'Associated files
        Public Property Iwad As String
        Public Property Maps As String
        Public Property Misc As String


    End Class
End Namespace
