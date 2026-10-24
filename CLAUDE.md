# Sistema de Nóminas SQL V6.0 ("Zeus Nominas")

Aplicación de escritorio Windows Forms en **VB.NET** (.NET Framework 4.8) con **DevExpress 24.1.3** (Ribbon, XtraGrid, XtraEditors). Gestiona empleados de nómina sobre **SQL Server**, con soporte multi-compañía (una base de datos por compañía).

- Solución: `Sistema de Nominas 6.0/Sistema de Nominas 6.0.sln` (proyecto `.vbproj`, objeto de inicio `My.MyApplication`).
- Compilar: abrir en Visual Studio 2022 (requiere DevExpress 24.1 instalado/licenciado) o `msbuild "Sistema de Nominas 6.0.sln"`. No hay tests ni CI.
- No es un repo git. Hay archivos `.bak` y carpeta `.vs/` que no forman parte del código.

## Migración desde la V.5.0 y arquitectura por clases

Esta versión es una **migración de `Sistema de Nominas SQL V.5.0`** (VB6, en `D:\Sistemas\Sistema-de-Nominas-SQL-V.5.0`, con su propio CLAUDE.md) a VB.NET. La V.5.0 es la **referencia funcional**: reglas de negocio (INSS, IR, vacaciones, aguinaldo, liquidaciones), tablas y pantallas se portan desde allí (`Modulos/Funciones.bas`, `Registro.bas`, `Formularios/*.frm`, esquema en `Cambios Scripts/ScrptBaseDatosBlanco.sql`). No se copia su estilo: la V.6.0 trabaja **con clases**.

Reglas del proyecto:
- **Una clase por responsabilidad**, en tres capas:
  - `Tablas/` = entidades (propiedades, sin lógica de datos).
  - `Clases/` = acceso a datos (DAL) y servicios (`CsXxxDAL`, p. ej. `CsEmpleadosDAL`) y, al migrar cálculos, clases de negocio (p. ej. INSS, IR, vacaciones) en lugar de funciones sueltas.
  - Formularios = solo UI; llaman al DAL/servicio y a la entidad, sin SQL ni cálculos propios.
- **No usar módulos con funciones/globales nuevas** como en `Funciones.bas`/`Variables.bas` de la V.5.0. Cada función migrada va como método de una clase; evitar variables globales y recordsets compartidos (el estado entre pantallas se pasa por parámetros/propiedades).
- Nuevo DAL = molde de `CsEmpleadosDAL`: SQL **parametrizado**, transacciones en operaciones de varias tablas, `Mapear` fila -> entidad, `Parametros` entidad -> comando. Nada de concatenar SQL.
- Reutilizar `Conexion.Cadena_Conexion`; nunca construir cadenas de conexión nuevas ni embeber credenciales.
- Al migrar un módulo: leer primero la lógica en la V.5.0, portarla a clase y **contrastar resultados** con la V.5.0 sobre los mismos datos antes de darla por buena.
- Cambios de esquema SQL: seguir la regla de la V.5.0 (script `.sql` idempotente, aplicable a cada BD de compañía).
- Nombres en español: clases `CsNombre`/entidades en singular o plural según el patrón existente (`Empleados`, `Cargos`), métodos con verbo (`Guardar`, `Buscar`, `Listar`, `Eliminar`).

## Estructura (`Sistema de Nominas 6.0/`)

| Carpeta / archivo | Rol |
|---|---|
| `Clases/` | Capa de acceso a datos y conexión (`CsDALBase` + `Cs*DAL`) |
| `Tablas/` | Entidades/POCO con propiedades: `Empleados`, `Historico`, `Cargos`, `Departamento`, `TipoDeduccion`, `Controles`, `DatosEmpresa`, `TipoNomina`, `PeriodoNomina`, `AnioPeriodo`, `Bitacora` |
| `Modulo/` | `Variables.vb` (globales `Nombre_Usuario`, `Tipo_Usuario`), `Conversion.vb` (imagen <-> bytes) |
| `SplashScreen`, `FrmListadoCompañias`, `FrmEntrada`, `Main`, `FrmEmpleados` | Formularios (UI) |
| `My Project/` | Config estándar de VB (Application, Resources, Settings) |

## Flujo de la aplicación

1. `SplashScreen` -> `Compañias.ConsultaCompañia()` cuenta compañías en el archivo Access.
2. Si hay más de una -> `FrmListadoCompañias` (elige compañía). `BuscaCompañia` devuelve la cadena de conexión SQL y se guarda en `Conexion.Cadena_Conexion` (campo **Shared**, global a toda la app).
3. Si la tabla `Usuarios` está vacía -> entra directo a `Main`; si no -> `FrmEntrada` (login con `Compañias.ContraseñaCorrecta`).
4. `Main` (MDI con Ribbon) abre `FrmEmpleados` (ficha de empleado con pestañas, foto, CRUD) y, desde la página "Catalogo", `FrmCargo`, `FrmDepartamento`, `FrmDeduccion`, `FrmControles`, `FrmTipoNomina` y `FrmPeriodo` (Periodo Nómina / Periodo Fiscal).

## Acceso a datos (todo en `Clases/`)

- **`Conexion.vb`**: fabrica conexiones. Dos orígenes:
  - **Access/OleDb** (`conexionAccess`, `MiConexion_Access`): archivo `CntNominas.dll` (en realidad un .mdb, junto al ejecutable) con la tabla `Servidor` (`NombreBD`, `Servidor` = cadena de conexión SQL de cada compañía). Provider `Microsoft.Jet.OLEDB.4.0` (solo 32 bits).
  - **SQL Server** (`ConexionSQL`, `MiConexion_SQl`): usa `Cadena_Conexion` elegida en runtime. No hay cadena en `App.config`.
- **`Compañias.vb`**: catálogo de compañías (Access) y login/usuarios (`Usuarios` en SQL: `NombreUsuario`, `Clave`, `NivelAcceso`). Propiedades Shared con usuario/compañía actual.
- **`ConsultasSQL.vb`** (estilo antiguo): SQL por **concatenación de strings**. Contiene listados (`CaseListadoSistemaNomina` con `Select Case` por nombre), `InsertarEmpleados`, `ActualizarEmpleado`, `BuscarEmpleado`, `BuscarHistorico`, `BorrarEmpleado`, `BuscarCargo/Departamento(+xNombre)`, `AgregarBitacora`. Helpers privados: `EjecutarConsulta`, `FillDataTableSQL`, `FillDataSetSQL`.
- **`CsEmpleadoDAL.vb`** (clase `CsEmpleadosDAL`, estilo nuevo): DAL **parametrizado** (`SqlCommand` + `Parameters`) con regiones Guardar / Existe / Insertar / Actualizar / Buscar / Eliminar / Catalogos / Listar / Historico / Parametros / Mapear. Es el **único** acceso a datos de `FrmEmpleados`.
  - `Guardar`: si `Cod_Empleado > 0` actualiza; si no, valida que `CodEmpleado1` no exista e inserta (Empleado + Historico en una transacción).
  - `Eliminar(CodEmpleado)`: **no borra**, hace `Activo = 0`. `Buscar` sigue devolviendo inactivos; los listados (`ListarParaCombo`, `Listar`) solo traen activos.
  - Catálogos: `ListarDepartamentos`, `ListarCargos`, `ListarTiposNomina`, `ListarParaCombo`, `ProximoCodigo`.
  - `NormalizarParametros`: `Nothing` -> NULL y fechas fuera de rango smalldatetime (p.ej. `Date.MinValue`) -> NULL.

Tablas SQL conocidas: `Empleado`, `Historico`, `Cargo`, `Departamento`, `TipoNomina`, `Usuarios`, `Bitacora`.

## Convenciones y advertencias

- Nombres en español; entidades con propiedades estilo `Cod_Empleado`, `Primer_Nombre`, etc. (propiedad ≠ columna SQL: ver `Mapear`/`Parametros`).
- **Preferir `CsEmpleadosDAL` (parametrizado)** para código nuevo; `ConsultasSQL` y `Compañias` concatenan entrada del usuario (inyección SQL, incl. login con contraseña en texto plano). Migrarlos al estilo parametrizado si se tocan.
- Bugs visibles en `ConsultasSQL` (no corregidos): `InsertarEmpleados` lista `Numerocelular` dos veces y tiene `SET` dentro de un `VALUES` del INSERT a `Historico`; `ActualizarEmpleado` asigna `CodInss` desde `Codigo_Postal` y `PorcentajePension` desde `Porcentaje_Comision`; `BuscaCompañia` hace `Return` antes de cerrar la conexión Access; `Format(..., "HH:MM:ss")` en `AgregarBitacora` usa `MM` (mes) en vez de `mm` (minutos).
- Los `*.Designer.vb` son generados por el diseñador de VS: no editar a mano.
- `FrmEmpleados.vb` (~1400 líneas) concentra mucha lógica de UI y validación; contiene bloques grandes de código comentado (versiones antiguas de `BtnGuardar_Click` e `Insertar`).

## Catálogos migrados de la V.5.0 (RibbonPage "Catalogo" en `Main`)

Cuatro formularios (`FrmCargo`, `FrmDepartamento`, `FrmDeduccion`, `FrmControles`), cada uno con su botón grande en la página `RibbonPageCatalogo` de `Main` (se abren como hijos MDI con `AbrirFormulario`; iconos por `ImageOptions.ImageUri` con nombres cortos de la galería SVG, p. ej. `BO_Position`; con el prefijo de categoría no resuelven). Mismo diseño que `FrmEmpleados` (XtraForm, `SimpleButton` 95x35 en la franja inferior, `GroupControl`, `SearchLookUpEdit` de código).

- **Patrón de los 3 catálogos** (`FrmCargo` es el molde): `cmbCodigo` (SearchLookUpEdit) elige un registro; **Nuevo** muestra `TxtCodigo` con el código propuesto (máx. numérico + 1, rellenado con ceros) y `btnCancelar`; **Grabar** valida y llama `Guardar(entidad, esNuevo)`; **Eliminar** borra de verdad (a diferencia de empleados) y, si hay llave foránea (SQL 547), muestra un mensaje claro; **Anterior/Siguiente** recorren la lista ordenada por código.
- **DAL**: `CsDALBase` (abstracta: cadena de conexión, `ConsultarTabla`/`Escalar`/`Ejecutar` parametrizados, `SiguienteCodigo`) y `CsCargoDAL`, `CsDepartamentoDAL`, `CsTipoDeduccionDAL`, `CsControlesDAL`. Entidades nuevas: `TipoDeduccion`, `Controles`, `DatosEmpresa` (`Cargos` y `Departamento` ya existían).
- **Deducciones**: los tipos `01`-`05` los usa el cálculo de nómina: no se eliminan y solo permiten cambiar la `CuentaContable` (`CsTipoDeduccionDAL.EsProtegida`). `Tipo` = `F` fija / `V` variable.
- **Controles**: `FrmControles` edita dos tablas de **una sola fila**: `Controles` (`Id`=1: `DiasMes` 30/25/365÷12, `DiasSemana` 6/7, `VerificarTasa`, `SalarioPromedioReal`, `AntiguedadMenor`, `CalcularRedondeado`) y `DatosEmpresa` (`Numero`=1: datos de la empresa, formatos de colilla/nómina, `MetodoVacaciones`, `TipoCalculoIR`, `PorcientoSubsidio`, `HorasExtra` (nvarchar), `Calcular7mo`, `Restar3DiasSubsidio`, `ConexionSistemaContable`). Si la fila no existe se inserta. `Calcular7mo` solo se muestra con formato "Nomina Bono Produccion". **No migrado** de `FrmControles.frm` (V.5.0): procesos masivos (ajuste de salarios, asignación de horarios/históricos, salario mínimo y valor de puntos, agregar usuarios) y el asistente de cadena de conexión (`MSDASC.DataLinks`).
- **Verificado** contra la BD de pruebas: alta/cambio/baja/duplicado/en uso de los 3 catálogos, round-trip de `Controles` y `DatosEmpresa`, y UI instanciada desde PowerShell 5.1 (cargar el exe con `Assembly.LoadFrom`, namespace raíz `Sistema_de_Nominas_6._0`, y un `AssemblyResolve` hacia `bin\Debug` para las DLL de DevExpress).

### Tipos de nómina y calendarios de períodos

Tres botones más en `RibbonPageGroupCatalogos` (antes estaban sin handler en la página "Acceso"): **Tipos de Nomina**, **Periodo Nomina**, **Periodo Fiscal**.

- **`FrmTipoNomina`** (mismo patrón que `FrmCargo`; `CsTipoNominaDAL`, entidad `TipoNomina`): código de 2 dígitos, período (8 valores de la V.5.0), tipo de pago, moneda (`Mantenimiento de valor` solo en córdobas), INSS y IR por porcentaje o por tabla (los % solo se ven/guardan en modo porcentaje; con INSS por tabla no se toca `TasaInssPatronal`), y 4 opciones. El alta deja `Activa = 0`. **No migrado**: la bitácora al cambiar la tasa INSS.
- **`FrmPeriodo(TipoCalendario)`**: una sola pantalla para `Planilla` (V.5.0 `frmFecha`: `Fecha_Planilla` + `Año_Actual`) y `Fiscal` (V.5.0 `FrmPeriodoFiscal`: `PeriodoFiscal` + `Año_Fiscal`); se abre con `New FrmPeriodo(...)` (no `My.Forms`). Elige tipo de nómina, año y rango de fechas; **Generar** muestra la vista previa (Nuevo / Existe / Con nómina) y **Guardar** activa el año y graba todo en una transacción (`CsPeriodoDAL.Guardar`).
- **`GeneradorPeriodos`** (sin acceso a BD): Quincenal (24: 1-15 y 16-fin de mes), Mensual (12), Semanal Sábado/Viernes (52), Catorcenal Sábados/Viernes (26, un período extra en junio y diciembre si el año es bisiesto, o julio y diciembre si no). Trimestral/Semestral no tienen generador (se avisa al usuario). El período 1 queda `Actual`.
- **Diferencias con la V.5.0**: los períodos que ya tienen nómina calculada (`Calculada` o `NumNomina > 0`) no se modifican (se cuentan como omitidos); el mes se deriva de las fechas reales; el rango por defecto es 1-ene/31-dic (planilla) y 1-jul/30-jun (fiscal) cuando la nómina no tiene año actual.
- **Verificado**: el generador reproduce las 24 filas existentes de `Fecha_Planilla` 2024-2026 y `PeriodoFiscal` 2024 (tipo `01`, ene-dic); CRUD de `TipoNomina` con código temporal `ZZ` y "en uso" (`01`); grabación de calendario nuevo (24 insertados, período 1 actual), re-grabación con un período calculado (0/11/1) y cambio de año actual (el anterior pasa a `Actual = 0`); todo limpiado después.

## Formulario de empleados (`FrmEmpleados.vb`)

Refactorizado: solo usa `CsEmpleadosDAL` + la entidad `Empleados` (ya no usa `ConsultasSQL`).
- Buscador `cmbCodigoEmpleado` (activos) carga la ficha al elegir un código existente; `BtnBuscarEmpleado`, `BtnSiguiente`/`BtnAnterior` también.
- **Nuevo**: muestra `TxtCodEmpleado` (código propuesto = máx. numérico + 1, `BtnNumeroDisponible` lo recalcula) y `btnCancelar`.
- **Grabar** (`BtnGuardar`) valida y llama `_dal.Guardar`. **Anular** (`BtnAnular`) = inactivar (`Activo = 0`) con confirmación.
- **Foto**: `BtnAgregar` carga una imagen (200x200), `BtnBorrar` la quita; se guarda en `Empleado.Foto` como texto "b1,b2,..." (formato heredado, ver `Modulo/Conversion.vb`). La silueta es solo un marcador y no se guarda.
- `Sexo` se guarda como `M`/`F` (convención de los datos existentes) y se muestra como Masculino/Femenino.

## Esquema SQL relevante (verificado contra la BD de pruebas)

- `Empleado.CodEmpleado` es identity (numeric); `CodEmpleado1` es el código visible (nvarchar 50). Longitudes cortas: Nombre/Apellido 20, Nacionalidad 12, Sindicalista 2.
- `Historico.Id` es `int NOT NULL` **sin identity**: el DAL lo calcula (`MAX(Id)+1` con UPDLOCK/HOLDLOCK en la transacción). `Historico.CuentaBanco` = cuenta BAC; las cuentas BANPRO/BDF/LAFISE viven en `Empleado.CuentaBancoBANPRO/BDF/LAFISE`.
- Cargos del histórico (`CargoInicial/Actual/Anterior`) se guardan por **nombre**; `Empleado.CodCargo`/`CodDepartamento` por código.

## Base de datos de pruebas

Cadena de conexión que el usuario autorizó para pruebas (SQL Server 2014, instancia local del usuario):

```
Password=P@ssword;Persist Security Info=True;User ID=sa;Initial Catalog=SistemaNominasConnect;Data Source=JUANBERMUDEZ\SQL2014
```

- Contiene datos reales de prueba (~1173 empleados). **No** está en `App.config`: la app normalmente la obtiene del catálogo Access (`CntNominas.dll`, tabla `Servidor`). Para pruebas programáticas basta `New Conexion().Cadena_Conexion = "<cadena>"` antes de crear `CsEmpleadosDAL`.
- Credencial `sa`: solo para pruebas; no copiarla a código ni a `App.config`.
- Existe un empleado de prueba inactivo `ZZTEST-CLAUDE` creado al probar el DAL.

## Compilar

```
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "Sistema de Nominas 6.0\Sistema de Nominas 6.0.sln" /p:Configuration=Debug
```