Namespace Helpers.DoomWorld.Models
    Public Class Level
        Public Property Id As Long
        Public Property Title As String
        Public Property Dir As String
        Public Property Filename As String
        Public Property Size As Long
        Public Property Age As Long
        Public Property ReleaseDate As String 'Convert date into ReleaseDate
        Public Property Author As String
        Public Property Email As String
        Public Property Description As String
        Public Property Credits As String
        Public Property Base As String
        Public Property Buildtime As String
        Public Property Editors As String
        Public Property Bugs As String
        Public Property Textfile As String
        Public Property Rating As Decimal
        Public Property Votes As Integer
        Public Property Url As String
        Public Property Idgamesurl As String
        Public Property Reviews As List(Of Review)

        Public Sub New()
            Reviews = New List(Of Review)
        End Sub

    End Class
End Namespace