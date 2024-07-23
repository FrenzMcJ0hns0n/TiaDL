Imports System.Net.Http
Imports System.Reflection
Imports Newtonsoft.Json.Linq

Namespace Helpers.DoomWorld
    Public Class ApiRequestManager

        '===========================================================================
        ' Members & Properties
        '===========================================================================
        Private Const BASE_URL As String = "https://www.doomworld.com/idgames/api/api.php"
        Private Const FORMAT As String = "out=json"

        Private _action As String
        Public Property Action() As String
            Get
                Return _action
            End Get
            Set(value As String)
                _action = value
            End Set
        End Property

        Private _parameters As List(Of String)
        Public Property Parameters() As List(Of String)
            Get
                Return _parameters
            End Get
            Set(value As List(Of String))
                _parameters = value
            End Set
        End Property

        Private _uriString As String
        Public Property UriString() As String
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
        Public Sub New(action As String, parameters As List(Of String))
            Debug.Print("Initializing ApiRequestManager object...")

            Me.Action = action
            Me.Parameters = parameters


            Debug.Print("Initialized ApiRequestManager object successfully with:")
            Debug.Print("Action = " & Me.Action)
            Debug.Print("Parameters = " & GetFormattedParams())
        End Sub

        '===========================================================================
        ' Private methods
        '===========================================================================
        Private Function GetFormattedParams() As String
            Return String.Join("&", Parameters)
        End Function

        Private Function GetUriString() As String
            Return $"{BASE_URL}?action={Action}&{GetFormattedParams()}&{FORMAT}"
        End Function

        ''TODO someday?
        'Private Sub WriteLogLineNowUTC(message As String)
        '    _logs.Append($"{Date.NowUTC} (UTC) - {message}{Environment.NewLine}")
        'End Sub
        ''Example:
        'Me.WriteLogLineNowUTC(String.Format("Preparing request with action '{0}' and parameters '{1}'...", Me.Action, Join(Me.Parameters, ","))

        '===========================================================================
        ' Public methods
        '===========================================================================

        Public Async Function FetchJsonResponse() As Task(Of JObject)
            Dim jsonResponse As JObject = Nothing

            Try
                UriString = GetUriString()
                _response = Await DoomWorldHttpClient.GetInstance().GetAsync(New Uri(UriString))

                If Not _response.IsSuccessStatusCode Then Throw New Exception($"Status code ({_response.StatusCode}) was not Success.")

                jsonResponse = JObject.Parse(Await _response.Content.ReadAsStringAsync())
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : action={Action}, parameters={Parameters}")
            End Try

            Return jsonResponse
        End Function

        Public Async Function FetchStringResponse() As Task(Of String)
            Dim strResponse As String = Nothing

            Try
                UriString = GetUriString()
                _response = Await DoomWorldHttpClient.GetInstance().GetAsync(New Uri(UriString))

                If Not _response.IsSuccessStatusCode Then Throw New Exception($"Status code ({_response.StatusCode}) was not Success.")

                strResponse = Await _response.Content.ReadAsStringAsync()
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : action={Action}, parameters={Parameters}")
            End Try

            Return strResponse
        End Function

    End Class
End Namespace