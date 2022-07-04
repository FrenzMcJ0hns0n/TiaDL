Imports System.Reflection

Module General

    Function GetFormattedAppVersion() As String
        Dim ExecutingAssemblyName As AssemblyName = Assembly.GetExecutingAssembly().GetName()
        'AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();

        Dim hasMinor As Boolean = ExecutingAssemblyName.Version.Minor > 0
        Dim hasBuild As Boolean = ExecutingAssemblyName.Version.Build > 0
        Dim hasRevision As Boolean = ExecutingAssemblyName.Version.Revision > 0

        Dim major As String = ExecutingAssemblyName.Version.Major.ToString
        Dim minor As String = IIf(hasMinor Or hasBuild Or hasRevision, $".{ExecutingAssemblyName.Version.Minor}", "")
        Dim build As String = IIf(hasBuild Or hasRevision, $".{ExecutingAssemblyName.Version.Build}", "")
        Dim revision As String = IIf(hasRevision, $".{ExecutingAssemblyName.Version.Revision}", "")

        Return $"This is a Doom Launcher - v{major}{minor}{build}{revision}"
    End Function

End Module
