Partial Public Class App
    Inherits Application

    Private Shared _StoreDB As StoreDB = New StoreDB()

    Public Shared Property StoreDB As StoreDB
        Get
            Return _StoreDB
        End Get
        Set
            _StoreDB = Value
        End Set
    End Property

End Class