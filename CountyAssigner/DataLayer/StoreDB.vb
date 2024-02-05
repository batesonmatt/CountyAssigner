Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient

Public Class StoreDB

    Private _connectionString As String = GetConnectionStringByName("Benson")

    Private Function GetConnectionStringByName(ByVal name As String) As String

        Dim connection As String
        Dim settings As ConnectionStringSettings

        Try
            settings = ConfigurationManager.ConnectionStrings(name)
            connection = settings.ConnectionString

        Catch ex As Exception
            connection = String.Empty
        End Try

        Return connection

    End Function

    Private Function GetDataView(ByVal sql As String) As DataView

        Dim dv As DataView

        Try
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

        Catch ex As Exception
            dv = Nothing
        End Try

        Return dv

    End Function

    Public Function GetCustomerDataView() As DataView

        Dim sql = "select distinct c.CustomerID, c.CustomerName, c.City, c.State, c.StateID from Customers c where CountyID is null order by CustomerName"

        Return GetDataView(sql)

    End Function

    Public Function GetCountyDataView(ByVal stateID As Integer) As DataView

        Dim sql = "select c.ID, c.County from CountiesNew c where c.StateID=" & stateID.ToString() & " order by c.County"

        Return GetDataView(sql)

    End Function

    Public Function UpdateCustomerCounty(ByVal customerID As Integer, ByVal countyID As Integer) As Integer

        Dim count As Integer = 0

        If customerID <= 0 OrElse countyID <= 0 Then
            Return count
        End If

        Dim sql = "update customers set CountyID=" & countyID.ToString() & " where customerid=" & customerID.ToString()
        Dim con As New SqlConnection(_connectionString)
        Dim trans As SqlTransaction = Nothing

        Try
            con.Open()
            trans = con.BeginTransaction()

            count = RCA.Core.DB.Execute(con, trans, sql, Nothing)

            If count = 1 Then
                trans.Commit()
            End If

        Catch ex As Exception
            trans.Rollback()
            count = -1
        Finally
            trans.Dispose()
            con.Dispose()
        End Try

        Return count

    End Function

End Class
