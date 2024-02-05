Imports System.Data

Class MainWindow

    Public Sub New()
        InitializeComponent()
        customerDataGrid.ItemsSource = App.StoreDB.GetCustomerDataView()
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
                Dim stateID As Integer = Integer.Parse(row("StateID"))

                countyDataGrid.ItemsSource = App.StoreDB.GetCountyDataView(stateID)

            Catch ex As Exception
                MessageBox.Show("Cannot update the county for this customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

        End If

    End Sub

    Private Sub Update_Click(sender As Object, e As RoutedEventArgs)

        If customerDataGrid.SelectedItem IsNot Nothing AndAlso countyDataGrid.SelectedItem IsNot Nothing Then

            Try
                Dim custRow As DataRowView = customerDataGrid.SelectedItem
                Dim custID As Integer = Integer.Parse(custRow("CustomerID"))
                Dim custName As String = custRow("CustomerName").ToString()

                Dim countyRow As DataRowView = countyDataGrid.SelectedItem
                Dim countyID As String = Integer.Parse(countyRow("ID"))
                Dim countyName As String = countyRow("County").ToString()

                Dim result As MessageBoxResult = MessageBox.Show(
                    $"You are about to update the county for '{custName}' with '{countyName}'. Do you want to continue?",
                    "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning)

                If result = MessageBoxResult.Yes Then
                    Dim count As Integer = App.StoreDB.UpdateCustomerCounty(custID, countyID)

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

End Class