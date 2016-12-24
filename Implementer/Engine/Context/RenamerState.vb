Imports Helper.RandomizeHelper
Imports Implementer.Core.Obfuscation.Exclusion

Namespace Engine.Context
    Public NotInheritable Class RenamerState

#Region " Fields "
        Private m_Namespaces As Boolean
        Private m_Types As Boolean
        Private m_Methods As Boolean
        Private m_Properties As Boolean
        Private m_Fields As Boolean
        Private m_CustomAttributes As Boolean
        Private m_Events As Boolean
        Private m_Variables As Boolean
        Private m_Parameters As Boolean
        Private m_ReplaceNamespacesSetting As ReplaceNamespaces
        Private m_RenameMainNamespaceSetting As RenameMainNamespace
        Private m_RenamingType As RandomizerType.RenameEnum
        Private m_ExclusionRule As ExcludeList
        Private m_ExcludeReflection As Boolean
#End Region

#Region " Properties "
        Public ReadOnly Property Namespaces As Boolean
            Get
                Return m_Namespaces
            End Get
        End Property

        Public ReadOnly Property Types As Boolean
            Get
                Return m_Types
            End Get
        End Property

        Public ReadOnly Property Methods As Boolean
            Get
                Return m_Methods
            End Get
        End Property

        Public ReadOnly Property Properties As Boolean
            Get
                Return m_Properties
            End Get
        End Property

        Public ReadOnly Property Fields As Boolean
            Get
                Return m_Fields
            End Get
        End Property

        Public ReadOnly Property CustomAttributes As Boolean
            Get
                Return m_CustomAttributes
            End Get
        End Property

        Public ReadOnly Property Events As Boolean
            Get
                Return m_Events
            End Get
        End Property

        Public ReadOnly Property Variables As Boolean
            Get
                Return m_Variables
            End Get
        End Property

        Public ReadOnly Property Parameters As Boolean
            Get
                Return m_Parameters
            End Get
        End Property

        Public ReadOnly Property ReplaceNamespacesSetting As ReplaceNamespaces
            Get
                Return m_ReplaceNamespacesSetting
            End Get
        End Property

        Public ReadOnly Property RenameMainNamespaceSetting As RenameMainNamespace
            Get
                Return m_RenameMainNamespaceSetting
            End Get
        End Property

        Public ReadOnly Property RenamingType As RandomizerType.RenameEnum
            Get
                Return m_RenamingType
            End Get
        End Property

        Public ReadOnly Property ExclusionRule As ExcludeList
            Get
                Return m_ExclusionRule
            End Get
        End Property

        Public ReadOnly Property ExcludeReflection As Boolean
            Get
                Return m_ExcludeReflection
            End Get
        End Property


#End Region

#Region " Constructor "
        Public Sub New(Namespac As Boolean, Typ As Boolean, Meth As Boolean, Prop As Boolean, Fiel As Boolean, Custom As Boolean, Even As Boolean, Vari As Boolean, _
                param As Boolean, ReplaceNamespace As Boolean, RenameMainNamespace As Boolean, RenamingType%, Exclusion As ExcludeList, ExludeReflect As Boolean)
            m_Namespaces = Namespac
            m_Types = Typ
            m_Methods = Meth
            m_Properties = Prop
            m_Fields = Fiel
            m_CustomAttributes = Custom
            m_Events = Even
            m_Variables = Vari
            m_Parameters = param
            m_ReplaceNamespacesSetting = ReplaceNamespacesValue(ReplaceNamespace)
            m_RenameMainNamespaceSetting = RenameMainNamespaceValue(RenameMainNamespace)
            m_RenamingType = RenameTypeValue(RenamingType)
            m_ExclusionRule = Exclusion
            m_ExcludeReflection = ExludeReflect
        End Sub
#End Region

#Region " Methods "
        Private Function ReplaceNamespacesValue(boolValue As Boolean) As ReplaceNamespaces
            Return If(boolValue, RenamerState.ReplaceNamespaces.Empty, RenamerState.ReplaceNamespaces.ByDefault)
        End Function

        Private Function RenameMainNamespaceValue(boolValue As Boolean) As RenameMainNamespace
            Return If(boolValue, RenamerState.RenameMainNamespace.Only, RenamerState.RenameMainNamespace.NotOnly)
        End Function

        Private Function RenameTypeValue(intValue%) As RandomizerType.RenameEnum
            Return Randomizer.GetScheme(intValue)
        End Function

        Public Sub CleanUp()
            m_Namespaces = False
            m_Types = False
            m_Methods = False
            m_Properties = False
            m_Fields = False
            m_CustomAttributes = False
            m_Events = False
            m_Variables = False
            m_Parameters = False
            m_ExclusionRule.CleanUp()
            m_ExcludeReflection = False
        End Sub
#End Region

#Region " Enumerations "
        ''' <summary>
        ''' INFO : ByDefault : Namespaces of the assembly stayed on first level of the tree. 
        '''        Empty : Namespaces are renamed by String.Empty value and store the types into the -1 level. 
        ''' </summary>
        Enum ReplaceNamespaces
            ByDefault = 0
            Empty = 1
        End Enum

        ''' <summary>
        ''' INFO : Full : rename all types and members. 
        '''        Medium : set to false events, variables, parameters. It will set the other one automatically to True.
        '''        Personnalize : requires you to set the boolean values manually for each types and members. 
        ''' </summary>
        Enum RenameRule
            Full = 0
            Medium = 1
            Personalize = 2
        End Enum

        ''' <summary>
        ''' INFO : NotOnly : Rename all namespaces.
        '''        Only : It will maybe solve many problems due to rename namespaces of merged assembly(s) !
        ''' </summary>
        Enum RenameMainNamespace
            NotOnly = 0
            Only = 1
        End Enum

#End Region

    End Class
End Namespace
