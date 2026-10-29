Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Acceso a datos de Usuarios y de sus permisos (tabla Accesos). Port de frmClaves (V.5.0).
''' </summary>
Public Class CsUsuarioDAL
    Inherits CsDALBase

    ''' <summary>Permisos que la V.5.0 daba a todo usuario nuevo (frmClaves.CmdGrabar).</summary>
    Private Shared ReadOnly AccesosPredeterminados() As String = {
        "Abrir Editar Niveles",
        "Abrir Registro Empleados", "Grabar Registro Empleados", "Borrar Registro Empleados",
        "Abrir Tabla Anotaciones", "Grabar Tabla Anotaciones", "Borrar Tabla Anotaciones",
        "Abrir Tabla Departamentos", "Grabar Tabla Departamentos", "Borrar Tabla Departamentos",
        "Tabla Cargo", "Abrir Tabla Cargo", "Grabar Tabla Cargo", "Borrar Tabla Cargo",
        "Abrir Tabla Incapacidad", "Grabar Tabla Incapacidad", "Borrar Tabla Incapacidad",
        "Abrir Tabla Incentivos", "Grabar Tabla Incentivos", "Borrar Tabla Incentivos",
        "Abrir Tabla Tipo Incapacidad", "Grabar Tabla Tipo Incapacidad", "Borrar Tabla Tipo Incapacidad",
        "Tabla Tipo Deducciones", "Abrir Tabla Tipo Deducciones", "Grabar Tabla Tipo Deducciones", "Borrar Tabla Tipo Deducciones",
        "Abrir Tabla Tipo Subsidio", "Grabar Tabla Tipo Subsidio", "Borrar Tabla Tipo Subsidio",
        "Abrir Tabla Tipo Comision", "Grabar Tabla Tipo Comision", "Borrar Tabla Tipo Comision",
        "Abrir Tabla Tipo Destajo", "Grabar Tabla Tipo Destajo", "Borrar Tabla Tipo Destajo",
        "Abrir Tabla Tipo Nomina", "Grabar Tabla Tipo Nomina", "Borrar Tabla Tipo Nomina",
        "Abrir Tabla Tipo INSS/IR", "Grabar Tabla Tipo INSS/IR", "Borrar Tabla Tipo INSS/IR",
        "Abrir Tabla Listado Nominas", "Grabar Tabla Listado Nominas", "Borrar Tabla Listado Nominas",
        "Abrir Calcular Nominas", "Abrir Activar Nominas",
        "Abrir Movimientos de la Nomina", "Grabar Movimientos de la Nomina",
        "Calcular 13vo y Vacaciones", "Abrir Calcular 13vo y Vacaciones",
        "Nomina de Subsidio", "Abrir Nomina de Subsidio",
        "Despido y Renuncias", "Abrir Despido y Renuncias",
        "Abrir Entradas y Salidas", "Abrir Calcular Horas Extra o Falta",
        "Abrir Reportes", "Abrir Controles Personalizados",
        "Abrir Usuarios", "Grabar Usuarios", "Borrar Usuarios"
    }

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT CodUsuario, NombreUsuario, NivelAcceso FROM Usuarios ORDER BY CodUsuario")
    End Function

    Public Function ProximoCodigo() As Integer
        Dim r As Object = Escalar("SELECT ISNULL(MAX(CodUsuario), 0) + 1 FROM Usuarios")
        Return Convert.ToInt32(r)
    End Function

    Public Function ExisteNombre(nombre As String, Optional excluirCodigo As Integer = 0) As Boolean
        Return Convert.ToInt32(Escalar(
            "SELECT COUNT(*) FROM Usuarios WHERE NombreUsuario = @Nom AND CodUsuario <> @Cod",
            New SqlParameter("@Nom", nombre), New SqlParameter("@Cod", excluirCodigo))) > 0
    End Function

    Public Function Buscar(codigo As Integer) As Usuarios

        Dim dt As DataTable = ConsultarTabla(
            "SELECT CodUsuario, NombreUsuario, Clave, NivelAcceso FROM Usuarios WHERE CodUsuario = @Cod",
            New SqlParameter("@Cod", codigo))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New Usuarios With {
            .Cod_Usuario = Convert.ToInt32(r("CodUsuario")),
            .Nombre_Usuario = If(IsDBNull(r("NombreUsuario")), "", Convert.ToString(r("NombreUsuario"))),
            .Clave_Usuario = If(IsDBNull(r("Clave")), "", Convert.ToString(r("Clave"))),
            .Nivel_Acceso = If(IsDBNull(r("NivelAcceso")), 0, Convert.ToInt32(r("NivelAcceso")))
        }

    End Function

    ''' <summary>Inserta (con los permisos predeterminados, en una transacción) o actualiza clave y nivel.</summary>
    Public Sub Guardar(u As Usuarios, esNuevo As Boolean)

        Dim nombre As String = u.Nombre_Usuario.Trim

        If ExisteNombre(nombre, If(esNuevo, 0, u.Cod_Usuario)) Then
            Throw New Exception("Ya existe un usuario llamado " & nombre & ".")
        End If

        If Not esNuevo Then
            Ejecutar("UPDATE Usuarios SET NombreUsuario = @Nom, Clave = @Clave, NivelAcceso = @Nivel WHERE CodUsuario = @Cod",
                     New SqlParameter("@Cod", u.Cod_Usuario), New SqlParameter("@Nom", nombre),
                     New SqlParameter("@Clave", u.Clave_Usuario), New SqlParameter("@Nivel", u.Nivel_Acceso))
            Exit Sub
        End If

        EnTransaccion(
            Sub(cn, tr)
                ' Código = MAX + 1 dentro de la transacción (CodUsuario no es identity).
                Dim codigo As Integer = Convert.ToInt32(EscalarEn(cn, tr,
                    "SELECT ISNULL(MAX(CodUsuario), 0) + 1 FROM Usuarios WITH (UPDLOCK, HOLDLOCK)"))

                EjecutarEn(cn, tr,
                    "INSERT INTO Usuarios (CodUsuario, NombreUsuario, Clave, NivelAcceso) VALUES (@Cod, @Nom, @Clave, @Nivel)",
                    New SqlParameter("@Cod", codigo), New SqlParameter("@Nom", nombre),
                    New SqlParameter("@Clave", u.Clave_Usuario), New SqlParameter("@Nivel", u.Nivel_Acceso))

                For Each modulo As String In AccesosPredeterminados
                    EjecutarEn(cn, tr, "INSERT INTO Accesos (CodUsuario, AccesoModulo) VALUES (@Cod, @Mod)",
                               New SqlParameter("@Cod", codigo), New SqlParameter("@Mod", modulo))
                Next

                u.Cod_Usuario = codigo
            End Sub)

    End Sub

    ''' <summary>Borra el usuario y sus permisos en una transacción.</summary>
    Public Sub Eliminar(codigo As Integer)

        EnTransaccion(
            Sub(cn, tr)
                EjecutarEn(cn, tr, "DELETE FROM Accesos WHERE CodUsuario = @Cod", New SqlParameter("@Cod", codigo))
                EjecutarEn(cn, tr, "DELETE FROM Usuarios WHERE CodUsuario = @Cod", New SqlParameter("@Cod", codigo))
            End Sub)

    End Sub

End Class
