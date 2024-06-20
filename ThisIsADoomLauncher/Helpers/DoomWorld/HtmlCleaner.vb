Imports System.Drawing.Imaging
Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions

Namespace Helpers.DoomWorld
    Public Class HtmlCleaner

        ''' <summary>
        ''' Cleans html leftovers tags (b, br...)
        ''' </summary>
        ''' <param name="html"></param>
        ''' <returns></returns>
        Public Shared Function HtmlToPlainText(html As String) As String

            Const tagWhiteSpace As String = "(>|$)(\W|\n|\r)+<" 'matches one Or more (white space Or line breaks) between '>' and '<'
            Const stripFormatting As String = "<[^>]*(>|$)" 'match any character between '<' and '>', even when end tag is missing
            Const lineBreak As String = "<(br|BR)\s{0,1}\/{0,1}>\s{0,1}" 'matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />,"<br> ","<BR> "

            Dim lineBreakRegex As Regex = New Regex(lineBreak, RegexOptions.Multiline)
            Dim stripFormattingRegex As Regex = New Regex(stripFormatting, RegexOptions.Multiline)
            Dim tagWhiteSpaceRegex As Regex = New Regex(tagWhiteSpace, RegexOptions.Multiline)

            Dim text As String = html
            Try
                'Decode html specific characters
                text = System.Net.WebUtility.HtmlDecode(text)
                'Remove tag whitespace/line breaks
                text = tagWhiteSpaceRegex.Replace(text, "><")
                'Replace <br /> with line breaks
                text = lineBreakRegex.Replace(text, Environment.NewLine)
                'Strip formatting
                text = stripFormattingRegex.Replace(text, String.Empty)

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {html}")

            End Try


            Return text
        End Function
    End Class
End Namespace