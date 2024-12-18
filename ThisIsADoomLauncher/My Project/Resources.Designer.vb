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
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0"),  _
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
        '''  Recherche une chaîne localisée semblable à #BASE PRESETS - LEVELS
        '''#Name						#Iwad			#Level				#Misc.				#Image
        '''
        '''Ultimate Doom; 				Doom.wad;		;					;					doom_icon.jpg
        '''Ultimate Doom + Sigil;		Doom.wad; 		SIGIL_v1_21.wad;	;					Sigil_cover.jpg
        '''Doom II: Hell on Earth; 	Doom2.wad;		;					;					doom2_icon.jpg
        '''TNT: Evilution; 			TNT.wad;		;					;					final_doom_icon.jpg
        '''The Plutonia Experiment;	Plutonia.wad;	;					;					final_doom_icon.jpg
        '''Doom Zero;					Doom2.wad; 		DoomZero.wad;		DOOMZERO.DEH;		DE_DOOM_Zero_edit.jpg
        '''Freedoom: Phase 1;			freed [le reste de la chaîne a été tronqué]&quot;;.
        '''</summary>
        Public ReadOnly Property base_presets_Levels() As String
            Get
                Return ResourceManager.GetString("base_presets_Levels", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Recherche une chaîne localisée semblable à #BASE PRESETS - MODS
        '''#Name										#Desc																		#Image						#Files
        '''
        '''No mod;										Play Vanilla Doom;															;							
        '''Brutal Doom v20b;							Older version of Brutal Doom;												;							brutalv20b_R.pk3
        '''Brutal Doom v21;							Last version of Brutal Doom;												;							brutalv21.pk3
        '''Brutal Doom v21 + Power Fantasy v2;			Last version of Brutal Doom + Addon Power Fantasy v2;						;							brutalv21.pk3,BrutalDoom PowerFantasy v2.pk3
        '''Brutal Doom Power Fantasy Final Beta;		Las [le reste de la chaîne a été tronqué]&quot;;.
        '''</summary>
        Public ReadOnly Property base_presets_Mods() As String
            Get
                Return ResourceManager.GetString("base_presets_Mods", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Recherche une chaîne localisée semblable à {
        '''    &quot;type&quot;: &quot;object&quot;,
        '''    &quot;default&quot;: {},
        '''    &quot;title&quot;: &quot;TiaDL settings&quot;,
        '''    &quot;required&quot;: [
        '''        &quot;Port&quot;,
        '''        &quot;PortParameters&quot;,
        '''        &quot;Iwad&quot;,
        '''        &quot;Level&quot;,
        '''        &quot;Misc&quot;,
        '''        &quot;Mods&quot;
        '''    ],
        '''    &quot;properties&quot;: {
        '''        &quot;Port&quot;: {
        '''            &quot;type&quot;: &quot;string&quot;,
        '''            &quot;default&quot;: &quot;&quot;,
        '''            &quot;title&quot;: &quot;Selected Port filepath&quot;,
        '''            &quot;examples&quot;: [
        '''                &quot;C:\\Path\\to\\gzdoom.exe&quot;
        '''            ]
        '''        },
        '''        &quot;PortParameters&quot;: {
        '''            &quot;type&quot;: &quot; [le reste de la chaîne a été tronqué]&quot;;.
        '''</summary>
        Public ReadOnly Property SettingSchema() As String
            Get
                Return ResourceManager.GetString("SettingSchema", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
