Imports System.Net.Http
Namespace Helpers.DoomWorld

    Public Class DoomWorldHttpClient

        Public Const BASE_URL As String = "https://www.doomworld.com/idgames/api/"

        Private Shared _instance As HttpClient

        Public Shared Function GetInstance() As HttpClient
            If _instance Is Nothing Then
                _instance = New HttpClient()
                _instance.BaseAddress = New Uri(BASE_URL)
            End If
            Return _instance
        End Function


    End Class
End Namespace
