Imports System.Data
Imports System.Data.SqlClient

Class MainWindow
    Private Property CustomerData As DataView
    Private Property CountyData As DataView

    Private Const _connectionString = "Data Source=Crockett;Initial Catalog=Benson;Integrated Security=True"

    Public Sub New()

        InitializeComponent()

        If TryGetCustomerDataView() Then
            customerDataGrid.ItemsSource = CustomerData
        End If

    End Sub

    Private Sub CustomerDataGrid_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        okButton.IsEnabled = customerDataGrid.SelectedItem IsNot Nothing
    End Sub

    Private Sub CountyDataGrid_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        updateButton.IsEnabled = countyDataGrid.SelectedItem IsNot Nothing AndAlso customerDataGrid.SelectedItem IsNot Nothing
    End Sub

    Private Sub OK_Click(sender As Object, e As RoutedEventArgs)

        If customerDataGrid.SelectedItem IsNot Nothing Then

            Try
                Dim row As DataRowView = customerDataGrid.SelectedItem
                Dim stateID As String = row("StateID").ToString()

                If TryGetCountyDataView(stateID) Then
                    countyDataGrid.ItemsSource = CountyData
                End If
            Catch ex As Exception
                MessageBox.Show("Cannot update the county for this customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

        End If

    End Sub

    Private Sub Update_Click(sender As Object, e As RoutedEventArgs)

        If customerDataGrid.SelectedItem IsNot Nothing AndAlso countyDataGrid.SelectedItem IsNot Nothing Then

            Try
                Dim custRow As DataRowView = customerDataGrid.SelectedItem
                Dim custID As String = custRow("CustomerID").ToString()
                Dim custName As String = custRow("CustomerName").ToString()

                Dim countyRow As DataRowView = countyDataGrid.SelectedItem
                Dim countyID As String = countyRow("ID").ToString()
                Dim countyName As String = countyRow("County").ToString()

                Dim result As MessageBoxResult = MessageBox.Show(
                    $"You are about to update the county for '{custName}' with '{countyName}'. Do you want to continue?",
                    "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning)

                If result = MessageBoxResult.Yes Then
                    Dim count As Integer = Update(custID, countyID)

                    If count < 0 Then
                        MessageBox.Show("An error occurred when attempting to update the customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                    Else
                        MessageBox.Show($"{count} row(s) updated.", "Update Completed", MessageBoxButton.OK, MessageBoxImage.Information)
                    End If
                End If

            Catch ex As Exception
                MessageBox.Show("Cannot update the county for this customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

        End If

    End Sub

    Private Function TryGetCustomerDataView() As Boolean

        Dim sql = "select distinct c.CustomerID, c.CustomerName, c.City, c.State, c.StateID from Customers c with(nolock) where CountyID is null order by CustomerName"

        CustomerData = GetDataView(sql)

        Return CustomerData IsNot Nothing

    End Function

    Private Function TryGetCountyDataView(ByVal stateID As String) As Boolean

        Dim success As Boolean = False

        If Not String.IsNullOrWhiteSpace(stateID) Then
            Dim sql = "select c.ID, c.County from CountiesNew c with(nolock) where c.StateID=" & stateID & " order by c.County"
            CountyData = GetDataView(sql)
            success = CountyData IsNot Nothing
        End If

        Return success

    End Function

    Private Shared Function GetDataView(ByVal sql As String) As DataView

        Dim dv As DataView = Nothing

        Try
            If Not String.IsNullOrWhiteSpace(sql) Then
                Using con As New SqlConnection(_connectionString)
                    Using cmd As New SqlCommand(sql, con)
                        cmd.CommandType = CommandType.Text
                        Using sda As New SqlDataAdapter(cmd)
                            Using dt As New DataTable()
                                sda.Fill(dt)
                                dv = dt.AsDataView()
                            End Using
                        End Using
                    End Using
                End Using
            End If

        Catch ex As Exception
        End Try

        Return dv

    End Function

    Private Shared Function Update(ByVal custID As String, ByVal countyID As String) As Integer

        Dim count As Integer = 0

        If String.IsNullOrWhiteSpace(custID) AndAlso String.IsNullOrWhiteSpace(countyID) Then
            Return count
        End If

        Dim sql = "update customers set CountyID=" & countyID & " where customerid=" & custID
        Dim con As New SqlConnection(_connectionString)
        Dim trans As SqlTransaction = Nothing

        Try
            con.Open()
            trans = con.BeginTransaction()

            count = RCA.Core.DB.Execute(con, trans, sql, Nothing)

            If count = 1 Then
                trans.Commit()
            End If

            trans = Nothing

        Catch ex As Exception
            count = -1
        Finally
            If trans IsNot Nothing Then
                trans.Rollback()
            End If
            If con IsNot Nothing Then
                con.Dispose()
            End If
        End Try

        Return count

    End Function

End Class
