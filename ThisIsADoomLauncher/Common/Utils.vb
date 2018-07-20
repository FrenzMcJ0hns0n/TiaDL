Module Utils

    ''' <summary>
    ''' Provide a shortcut to interact with MainWindow GUI items
    ''' </summary>
    '''
    Function MainWindow_Instance() As MainWindow

        Dim mainWindow = Windows.Application.Current.Windows(0)
        Return mainWindow

    End Function

End Module
