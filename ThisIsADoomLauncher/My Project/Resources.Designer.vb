﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Ce code a été généré par un outil.
'     Version du runtime :4.0.30319.42000
'
'     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
'     le code est régénéré.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    'à l'aide d'un outil, tel que ResGen ou Visual Studio.
    'Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    'avec l'option /str ou régénérez votre projet VS.
    '''<summary>
    '''  Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Public Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("ThisIsADoomLauncher.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Remplace la propriété CurrentUICulture du thread actuel pour toutes
        '''  les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Recherche une chaîne localisée semblable à #COMMON PRESETS
        '''#Name						#Iwad			#Level			#Misc.
        '''
        '''Ultimate Doom, 				Doom.wad
        '''Doom II: Hell on Earth, 	Doom2.wad
        '''TNT: Evilution, 			TNT.wad
        '''The Plutonia Experiment,	Plutonia.wad
        '''2002 A Doom Odyssey,		Doom.wad, 		2002ad10.wad
        '''Icarus: Alien Vanguard,		Doom2.wad, 		ICARUS.wad
        '''Requiem, 					Doom2.wad,		Requiem.wad
        '''Plutonia 2,					Doom2.wad,		PL2.wad
        '''Hell Revealed,				Doom2.wad, 		hr.wad
        '''Hell Revealed 2,			Doom2.wad, 		hr2final.wad
        '''DTS-T						Doom2.wad, 		DTS-T.pk3
        '''Plutonia Revisited,			Doom2.wad [le reste de la chaîne a été tronqué]&quot;;.
        '''</summary>
        Public ReadOnly Property common_presets() As String
            Get
                Return ResourceManager.GetString("common_presets", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
