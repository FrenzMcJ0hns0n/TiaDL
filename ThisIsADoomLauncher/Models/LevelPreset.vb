Namespace Models
    Public Class LevelPreset
        Inherits Preset


        Public Property Type As String 'Assets type : Doom 1 / Doom2 / Other
        Public Property Year As Integer 'Year of last release

        'Associated filepaths
        Public Property Iwad As String
        Public Property Maps As String
        Public Property Misc As String


    End Class
End Namespace
