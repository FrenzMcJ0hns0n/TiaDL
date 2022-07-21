Namespace Models
    Public Class LevelPreset

        Public Property Name As String

        'Preset properties
        Public Property Desc As String 'Short description, for Tooltip text
        Public Property Type As String 'Assets type : Doom 1 / Doom2 / Other
        Public Property Year As Integer 'Year of last release

        'Associated files
        Public Property Iwad As String
        Public Property Level As String
        Public Property Misc As String
        Public Property Pict As String

    End Class
End Namespace
