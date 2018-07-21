﻿Module Utils

    ''' <summary>
    ''' Provide a shortcut to interact with MainWindow GUI items
    ''' </summary>
    '''
    Function MainWindow_Instance() As Views.MainWindow

        Dim mainWindow = Windows.Application.Current.Windows(0)
        Return mainWindow

    End Function

    Function GetResolution_Width() As Integer

        Return My.Computer.Screen.Bounds.Size.Width

    End Function

    Function GetResolution_Height() As Integer

        Return My.Computer.Screen.Bounds.Size.Height

    End Function

End Module
