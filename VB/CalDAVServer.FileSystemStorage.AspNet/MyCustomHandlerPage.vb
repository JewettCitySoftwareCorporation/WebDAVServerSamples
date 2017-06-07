Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI
Imports System.Threading.Tasks
Imports ITHit.WebDAV.Server
Imports ITHit.WebDAV.Server.CalDav

Public Class MyCustomHandlerPage
    Inherits Page

    Protected Sub New()
        If Type.GetType("Mono.Runtime") Is Nothing Then
            AddHandler Me.Load, AddressOf Page_LoadAsync
        End If
    End Sub

    Private Sub Page_LoadAsync(sender As Object, e As EventArgs)
        RegisterAsyncTask(New PageAsyncTask(AddressOf GetPageDataAsync))
    End Sub

    Public Async Function GetPageDataAsync() As Task
        Dim context As DavContext = New DavContext(HttpContext.Current)
        Dim discovery As Discovery = New Discovery(context)
        For Each folder As IItemCollectionAsync In Await discovery.GetCalendarHomeSetAsync()
            Dim children As IEnumerable(Of IHierarchyItemAsync) = Await folder.GetChildrenAsync(New PropertyName(-1) {})
            AllUserCalendars.AddRange(children.Where(Function(x) TypeOf x Is ICalendarFolderAsync))
        Next
    End Function

    ''' <summary>
    ''' Gets all user calendars.
    ''' </summary>
    Public AllUserCalendars As List(Of IHierarchyItemAsync) = New List(Of IHierarchyItemAsync)()

    Public Shared ReadOnly Property ApplicationPath As String
        Get
            Dim context As DavContext = New DavContext(HttpContext.Current)
            Dim url As Uri = HttpContext.Current.Request.Url
            Dim server As String = url.Scheme & "://" & url.Host & (If(url.IsDefaultPort, "", ":" & url.Port.ToString())) & "/" & context.Request.ApplicationPath.Trim("/"c)
            Return server.TrimEnd("/"c) & "/"c
        End Get
    End Property
End Class