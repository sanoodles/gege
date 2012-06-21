Public Class CommentsDialogForm

    'Public Sub New()

    ' This call is required by the Windows Form Designer.
    '   InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.


    'End Sub

    Public Sub New()
        'Me.New()
        'Text = "Comment for " + ic.GetIdentifier()
        'txtComment.Text = ic.Documentation
        Me.InitializeComponent()
        Me.AcceptButton = Button1
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Hide()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Hide()
    End Sub

    Public ReadOnly Property CommentTypedIn() As String
        Get
            Return txtComment.Text
        End Get
    End Property

    Private Sub CommentsDialogForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class