Module Utils

    ''' <summary>
    ''' Provide a shortcut to interact with MainWindow GUI items
    ''' </summary>
    '''
    Function MainWindow_Instance() As Views.MainWindow

        Return Windows.Application.Current.Windows(0)

    End Function

    Function GetResolution_Width() As Integer

        Return My.Computer.Screen.Bounds.Size.Width

    End Function

    Function GetResolution_Height() As Integer

        Return My.Computer.Screen.Bounds.Size.Height

    End Function

End Module
