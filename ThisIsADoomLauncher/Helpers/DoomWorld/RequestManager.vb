Imports System.Net.Http
Imports System.Reflection

Namespace Helpers.DoomWorld
    Public Class RequestManager

        '===========================================================================
        ' Members & Properties
        '===========================================================================
        Private Const BASE_URL As String = "https://www.doomworld.com/idgames/api/api.php"
        Private Const FORMAT As String = "out=json"

        Private _action As String
        Public Property Action As String
            Get
                Return _action
            End Get
            Set(value As String)
                _action = value
            End Set
        End Property

        Private _params As List(Of String)
        Public Property Params As List(Of String)
            Get
                Return _params
            End Get
            Set(value As List(Of String))
                _params = value
            End Set
        End Property

        Private _uriString As String
        Public Property UriString As String
            Get
                Return _uriString
            End Get
            Set(value As String)
                _uriString = value
            End Set
        End Property

        'TODO: Add HttpResponseMessage properties 'Headers' and 'Version' ?

        Private _response As HttpResponseMessage

        '===========================================================================
        ' Constructor
        '===========================================================================
        'Cancelled to ensure more generic behavior. TODO? Set multiple constructors?
        'Public Sub New()
        '    Debug.Print("[Constructor] Initialize new RequestManager object")
        'End Sub'

        '===========================================================================
        ' Private methods
        '===========================================================================
        Private Function GetFormattedParams() As String
            Return String.Join("&", Params)
        End Function

        Private Function GetUriString() As String
            Return $"{BASE_URL}?action={Action}&{GetFormattedParams()}&{FORMAT}"
        End Function

        '===========================================================================
        ' Public methods
        '===========================================================================
        ''' <summary>
        ''' This method is dedicated to API calls towards the idGames Archive Public API.
        ''' Properties 'Action' and 'Params' are required (must not be Nothing)
        ''' </summary>
        ''' <returns>A JSON response in String format</returns>
        Public Async Function FetchResponse() As Task(Of String)
            Dim strResponse As String = Nothing

            Try
                If Action = Nothing OrElse Params Is Nothing Then
                    Throw New Exception($"Both 'Action' and 'Params' are required in order to perform API requests.")
                End If

                UriString = GetUriString()
                Debug.Print($"[RequestManager::FetchResponse] Call URI '{UriString}'")
                _response = Await DoomWorldHttpClient.GetInstance().GetAsync(New Uri(UriString))

                If Not _response.IsSuccessStatusCode Then
                    Throw New Exception($"API response status code ({_response.StatusCode}) was not Success.")
                End If

                strResponse = Await _response.Content.ReadAsStringAsync()

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf}Properties : action={Action}, parameters={Params}")
            End Try

            Return strResponse
        End Function

    End Class
End Namespace