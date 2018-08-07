Imports System.Net.Http
Imports System.Net.Http.Headers


Namespace Helpers.API
    Public Class DoomWorldClient

        Private Const DOOMWORLD_BASE_URL = "https://www.doomworld.com/idgames/api/api.php?out=json&action="

        Public Async Function GetGameInfosById(gameId As Integer) As Task(Of Models.Content)
            Dim game As Models.Content = Nothing

            Try
                Dim uri = String.Concat(DOOMWORLD_BASE_URL, "get&id=", gameId)
                Dim response = Await Client.HttpClient.GetAsync(uri)

                If response.IsSuccessStatusCode Then

                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim result = Serialization.DeserializeObject(Of Models.RootObject)(json)

                    game = result.Content

                End If

            Catch ex As Exception
                Dim r = ex
                game = Nothing

            End Try

            Return game
        End Function
    End Class
End Namespace
