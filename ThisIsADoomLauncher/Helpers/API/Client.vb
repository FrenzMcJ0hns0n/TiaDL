Imports System.Net.Http

Namespace Helpers.API
    Public Class Client



        Private Shared _httpClient As HttpClient
        Public Shared Property HttpClient As HttpClient
            Get
                Return If(IsNothing(_httpClient), New HttpClient(), _httpClient)
            End Get
            Set(ByVal value As HttpClient)
                _httpClient = value
            End Set
        End Property

    End Class
End Namespace
