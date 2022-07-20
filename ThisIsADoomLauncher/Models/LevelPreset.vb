Namespace Models
    Public Class LevelPreset

        'Preset information
        Public Property Name As String
        Public Property Desc As String 'Short description, for Tooltip text
        Public Property Base As String 'Base assets : Doom 1 / Doom2 / Other
        Public Property Year As Integer 'Year of last release

        'Associated files
        Public Property Iwad As String
        Public Property Level As String
        Public Property Misc As String
        Public Property Pict As String

    End Class
End Namespace
