Imports System.Reflection
Imports System.Text.RegularExpressions

Namespace Helpers.DoomWorld
    Public Class HtmlCleaner

        ''' <summary>
        ''' Cleans html leftovers linebreaks tags (br, \n) and surrounding white spaces.<br/>
        ''' If 2 or more at once -> replace with 2 line breaks.<br/>
        ''' If 1 -> replace with 1 line break.
        ''' </summary>
        ''' <param name="html"></param>
        ''' <returns></returns>
        Public Shared Function ReplaceHtmlLineBreaks(html As String) As String
            Const manyLineBreaks As String = "(\s{0,}(\\n|<(br|BR)\s{0,}\/{0,}>)\s{0,}){2,}"
            Const singleLineBreak As String = "\s{0,}(\\n|<(br|BR)\s{0,}\/{0,}>)\s{0,}"

            Dim text As String = html
            Dim manyLinesBreakRegex As New Regex(manyLineBreaks, RegexOptions.Multiline)
            Dim singleLineBreakRegex As New Regex(singleLineBreak, RegexOptions.Multiline)

            Try
                text = manyLinesBreakRegex.Replace(text, String.Concat(Environment.NewLine, Environment.NewLine))
                text = singleLineBreakRegex.Replace(text, Environment.NewLine)
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {html}")
            End Try

            Return text
        End Function
    End Class
End Namespace