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

- **Espacio reducido en el MDI**: el área de `Main` (ribbon + barra de estado) deja poco alto y ancho a los formularios hijos, y lo que no cabe se corta (p. ej. `FrmBajas` perdía sus botones). Todo formulario nuevo o migrado debe **calzar dentro del MDI**: tamaño base moderado (alto ≈ 600 o menos, ancho ≈ 1200 o menos; la V.5.0 usaba pantallas más grandes, no copiar sus medidas), botones en una franja/columna **acoplada** (`Dock` abajo o a la derecha) y nunca en una `Location` fija al fondo, y el contenido en un `XtraScrollableControl` o con `Anchor`/`Dock` para que haga scroll si la ventana es menor. Los formularios de captura son `FrmBajas` (columna de botones a la derecha) y `FrmConsulta` (barra inferior) como referencia. Comprobar abriéndolos como hijos MDI, no solo sueltos.
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

### Listados y procesos (página "Acceso") y catálogos de incapacidad/comisión/destajo

Todos como clase con DAL propio y métodos **públicos** para que otros formularios los llamen (se abren como hijos MDI con `AbrirFormulario(My.Forms.X)` desde `Main`).

| Botón (`Main`) | Formulario | DAL | Públicos |
|---|---|---|---|
| `BarButtonListaEmpleado` | `FrmListadoEmpleados` (V.5.0 `FrmListaEmpleados`) | `CsEmpleadosDAL.ListarDetallado` | `Recargar`, `CodigoSeleccionado`; abre `FrmEmpleados.CargarEmpleado` / `NuevoEmpleado` (nuevo, público) |
| `BarButtonHorasExtra` | `FrmListadoHorasExtra` | `CsHorasExtraDAL` | `Recargar` |
| `BarButtonActivarNomina` | `FrmActivarNomina` | `CsActivarNominaDAL` | `RecargarTipos`, `SeleccionarTipoNomina`, evento `NominaActivada` |
| `BarButtonListaNomina` | `FrmListNomina` | `CsNominaDAL` | `Recargar`, `NominaSeleccionada` |
| `BarButtonIncapacidades` | `FrmIncapacidades` | `CsIncapacidadDAL` | `CargarEmpleado(codigoVisible)`, `RecargarTipos` |
| `BarButtonTipoComision` / `TipoDestajo` / `TipoIncapacidad` | `FrmTipoComision` / `FrmTipoDestajo` / `FrmTipoIncapacidad` | `CsTipoComisionDAL` / `CsTipoDestajoDAL` / `CsTipoIncapacidadDAL` | molde `FrmCargo` |

`FrmTipoNomina` ya estaba migrado (botón `BarButtonTipoNomina`). `ExportadorGrid.ExportarAExcel` (Clases/) exporta cualquier `GridView` a .xlsx y reemplaza los bucles con `Excel.Application` de la V.5.0.

- **Catálogos nuevos**: `TipoComision.Porcentaje` se guarda como fracción (`real`; la pantalla muestra 0-100). `TipoDestajo.Monto` es `float`; el botón "Monto a todos" es el "Repetir" de la V.5.0 (con confirmación). `CuentaContable` es texto libre (no se migró el buscador `FrmConsulta`). `Incapacidad` no tiene llave propia: se identifica por empleado + tipo + fecha (la V.5.0 grababa siempre 01/01/2000; ahora se elige la fecha y no se permiten duplicados).
- **Activar nómina** (`CsActivarNominaDAL.Activar`, una transacción): valida que el tipo esté inactivo, que el período exista en `Fecha_Planilla` y que no haya nómina cerrada en el rango; toma el número de `Consecutivos.Nominas` (UPDLOCK) y lo incrementa; inserta `Nomina` con totales en 0 (`Mes/Ano/Periodo` ya completos); deja el período como único `Actual` del tipo en `Fecha_Planilla` y `PeriodoFiscal` (solo si hay período fiscal que empiece ese día) con su `NumNomina`; `TipoNomina.Activa = 1`. `SugerirPeriodo`: el período actual si su nómina no está cerrada, si no el siguiente. **Diferencias con la V.5.0**: un solo algoritmo para todos los tipos (la V.5.0 invertía la comprobación de "cerrada" en Quincenal/Mensual); la fecha final solo se edita en Quincenal/Mensual; se valida inicio <= final (la V.5.0 comparaba contra una variable inexistente); limpia el `Actual` de todos los años del tipo, no solo del año elegido (en la BD de pruebas el tipo `01` tiene varios `Actual = 1` heredados).
- **Listado de nóminas** (`CsNominaDAL`): lista todas (`Estado` = Anulada/Cerrada/Activa/Procesada) con totales y detalle por empleado. **Anular** (solo `Cerrada`, una transacción) borra `DetalleNomina`, deja totales en 0 (`Procesada = 1`, `Anulada = 1`), reabre los movimientos (`DetalleDeduccion`/`DetalleIncentivo`/`Comisiones`/`Destajo` `Pagado = 0`, `HorasExtras.Pagada = 0`, `MovPrestamo.Cancelado = 0`) y devuelve `CuotaIgual` al `Prestamo.Saldo`. Diferencias: rechaza nóminas ya anuladas (la V.5.0 sumaba el saldo dos veces), contabilizadas o activas sin cerrar.
- **Listado de empleados**: `LEFT JOIN` a departamento/cargo/histórico (la V.5.0 con `INNER JOIN` ocultaba 1 de 769 activos).
- **No migrado**: reportes/impresión (`ArepHorasExtras`, `FrmReportes`/`FrmNominaActiva`, colilla, CSV, `CmdPrNomina`), (`FrmMovimientos` ya migrado: botones Nuevo/Editar del listado de horas extra, `CsMovimientoNominaDAL`; pestañas Horas extra / Comisiones / Otros destajos, interfaz armada por código; probado solo en lectura, sin grabar), "Agregar Viático" (`FrmIncentivosGrupo`), buscador de cuenta contable (`FrmConsulta`).
- **Verificado** en la BD de pruebas con datos temporales (tipo `ZZ`, préstamo/horas extra 99001, códigos `ZZ`) y limpieza posterior: CRUD de los tres catálogos, incapacidades (alta/duplicada/en uso), activación completa (número, consecutivo, calendarios, errores sin gastar consecutivo), período cerrado → siguiente, anulación con reversión de movimientos y saldo, rechazos; los 8 formularios instanciados y abiertos desde `Main` (iconos del ribbon por `ImageUri` sin comprobar visualmente).

### Anotaciones, carnet, tasa de cambio y RibbonPage "Opciones"

Cada formulario es solo UI sobre un DAL propio (`Clases/Cs*DAL.vb`) y expone métodos públicos para llamarlo desde otros.

| Acceso | Formulario | DAL | Públicos |
|---|---|---|---|
| `FrmEmpleados.BtnAnotar` | `FrmAnotaciones` (V.5.0 `frmAnotaciones`, tabla `Curriculum`) | `CsCurriculumDAL` | `CargarEmpleado(cod, codigoVisible)`, `Grabar`; `FrmEmpleados.AbrirAnotaciones` |
| `FrmEmpleados.BtnCarnet` | `FrmCarnet` (foto + código de barras Code 39 dibujado por `CodigoBarras39`) | `CsCarnetDAL` | `CargarEmpleado(cod)`; `FrmEmpleados.AbrirCarnet` |
| `BarButtonTasaCambio` | `FrmTasaCambio` (`CambioMoneda`) | `CsCambioMonedaDAL` | `Recargar`, `TasaSeleccionada` |
| `RibbonPageOpciones` > `BarButtonCargaMasiva` | `FrmCargaMasiva` | `CsCargaMasivaDAL` | `ProcesarDeduccion`, `ProcesarIncentivo` |
| `BarButtonControles` (segundo enlace al mismo botón del Catálogo) | `FrmControles` | — | — |
| `BarButtonHistorial` | `FrmHistorial` | `CsHistorialEmpleadoDAL` | `CargarEmpleado(codigo1)`, `Recargar` |
| `BarButtonImportar` | `FrmImportar` | `CsImportacionDAL` + `LectorExcel` (OleDb ACE) | `AbrirArchivo`, `Importar` (tipo 3 = catálogo de empleados: `ImportarEmpleados`, columnas por encabezado, crea cargo/departamento/grupo faltantes; sin Userinfo ni horario; tipo 4 = saldo de vacaciones: `CsVacacionesDAL.ImportarSaldos` + `CalculadoraVacaciones`, port de `CalculoDiasVacaciones`; también `CalcularSaldo(codigo, fecha)`; tipo 5 = ingresos/incentivos: `ImportarIncentivos`; tipo 6 = solicitudes: `ImportarSolicitudes`) |
| `BarButtonJustificaciones` | `FrmJustificaciones` | `CsJustificacionDAL` | `CargarEmpleado(codigo1)`, `RecargarTipos`, `RecargarLista` |
| `BarButtonQuery` | `FrmQuery` | `CsConsultaDAL` | `EjecutarConsulta` |
| `BarButtonInssIr` | `FrmInssIr` | `CsInssIrDAL` | `Recargar`, `Guardar` |

- **Lecciones de esquema**: `Consecutivos.Deducciones` está desfasado (5007 frente a un máximo real de 13079): los números de deducción salen de `MAX(NumDeduccion)+1` y no se toca `Consecutivos`. `DetalleIncentivo` tiene PK `(Id, NumIncentivo)`, con `Id` = número de vez (la carga masiva usa 1). `Curriculum.Id` no es identity y `Faltas` es `nvarchar(2)` (Si/No). `smalldatetime` llega hasta 2079. SQL Server 2014: no hay `CREATE OR ALTER`.
- **Carga masiva**: solo empleados activos del tipo de nómina, todo en una transacción.
- **Historial**: `CopiarPerfil` crea un empleado nuevo (código visible `S1`+aaMM+correlativo o el mismo) copiando ficha, histórico y horario. `Desactivar` también quita al empleado de la nómina activa.
- **Importar / Eliminar pendientes**: los `Eliminar*Pendientes` borran de verdad (con confirmación); no probarlos sobre datos reales.
- **FrmQuery** ejecuta SQL libre sin restricción de acceso (no se conoce la escala de `NivelAcceso`); los `SELECT`/`WITH` no piden confirmación, el resto sí.
- **FrmImportar rediseñado**: menú lateral `AccordionControl` (`Menu`) con las 8 importaciones + Consultas SQL + Eliminar pendientes; cada sección (`Seccion`) trae título, descripción y columnas esperadas, y descarta el archivo cargado al cambiar. Admite arrastrar y soltar un .xls/.xlsx. `CmbTipo` quedó oculto solo para conservar los índices de tipo de `Importar`. Revisado con `DrawToBitmap`, no con datos reales.
- **FrmImportar, opciones añadidas** (sin probar contra la BD): tipo 7 "Solicitud total" (`CsImportacionDAL.ImportarSolicitudTotal`, port de `CargarArchivoST`/`BtnImportarSolTotal`; valida todo antes de grabar, escribe `DescuentoDiasVacaciones`), "Eliminar solicitudes" y "Eliminar empleados" (borra TODO), y consola SQL Consultar/Ejecutar (reutiliza `CsConsultaDAL`); los controles nuevos se crean por código en `CrearPanelAvanzado`.
- **No migrado**: Constancia/Trasladar de Historial; reportes de INSS/IR y justificaciones; `Instalar_Tabla_Reportes`/`sp_LlenarReportes_AsistenciaMensual` de FrmQuery; botón "Pegar" de la tasa de cambio y el bloqueo al cerrar sin tasa de hoy (ahora es una etiqueta roja).
- **Verificado**: DAL contra la BD de pruebas con datos temporales `ZZ`; los 9 formularios abiertos con datos reales y capturados con `DrawToBitmap`. Pendiente de ver en pantalla real: iconos del ribbon (`Add`, `History`, `Open`, `Edit`, `Find`, `Percent`) y el flujo modal BtnAnotar/BtnCarnet desde `FrmEmpleados`.
- **Incidente de pruebas**: una limpieza mal calculada borró filas reales de `Deduccion`/`DetalleDeduccion` en la BD de pruebas; se restauraron desde el `.bak` y se verificaron los conteos. Regla: las pruebas destructivas usan códigos `ZZ` y borran solo por ids exactos.

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
### Pestañas Préstamo / Incentivos / Deducciones / Subsidios de `FrmEmpleados`

Migradas de `frmEmpleado.frm` (V.5.0). Se agregan por código en `FrmEmpleados.CrearPestanasMovimientos` (no están en el Designer) y se recargan con `CargarMovimientos` al cargar/limpiar la ficha; quedan inactivas si el empleado aún no está guardado.
- **UI**: `UcPrestamoEmpleado` y `UcMovimientosEmpleado(TipoMovimiento)` (un solo control para las tres pestañas de movimientos; solo cambian Descripción en subsidios y Anular en incentivos).
- **Datos/negocio**: `CsMovimientosDAL` (Incentivo/Deduccion/Subsidio), `CsPrestamoDAL`, `CalculadoraPrestamo` (libreta: interés sobre saldo, `CuotaIgual = (capital + interés total) / cuotas`), entidades `PrestamoEmpleado`/`CuotaPrestamo`. `CsDALBase` ganó `EnTransaccion`, `EscalarEn`, `EjecutarEn`.
- Incentivos y deducciones exigen nómina activa del tipo de nómina del empleado; "n" = indefinido (incentivo: una fila `NumVez='n'`; deducción: una fila `NumVez=9999`). Eliminar borra el movimiento completo (el detalle cae en cascada).
- **No migrado**: botones de histórico/reportes (`FrmVerReportes`), estado de préstamo (`ARPagoPrestamo`), exportar préstamo a archivo, edición en línea (`FrmPrestamos`), bitácora, `Consecutivos`. **Diferencia**: "Anular" incentivo deja en 0 la vez seleccionada (V.5.0 tocaba la primera fila del incentivo).
- **Verificado** en la BD de pruebas con el empleado `ZZTEST-CLAUDE`: alta/anular/eliminar de los tres movimientos, préstamo (1200, 4 cuotas, 5% → cuota igual 337.50), rechazo del segundo préstamo y cancelación; datos de prueba eliminados.
### Bajas / liquidación (RibbonPage "Procesos") y reportes DevExpress

`RibbonPageProcesosBajas` ("Procesos") > `BarButtonBajas` abre `FrmBajas` (V.5.0 `FrmBajas.frm` + reporte `ARBaja`); "Despidos y Renuncias" de la página Acceso (`BarButtonDespidos`) abre la misma pantalla. Interfaz armada por código (sin Designer).

| Pieza | Rol |
|---|---|
| `CsLiquidacion` (Clases/) | Servicio: `CargarEmpleado`, `CambiarFechaEgreso`, `CambiarFechaHistorial`, `Calcular` (graba `Bajas` + bitácora), `Procesar`, `CrearReporte` |
| `CalculadoraLiquidacion` | Sin BD: `DiasAntiguedad` (CalcularDiasAntiguedad), `DiasAguinaldo` (CalcularDiasAguinaldo, resta suspensiones), `DiasVacaciones` (CalcularDiasVaca), `DiasPlanilla`, `TiempoAlCargar` (÷360) / `TiempoAlCambiarEgreso` (÷358), `Calcular` (montos de CmdCalculos_Click) |
| `CalculadoraIRBaja` | IR de la baja (CalcularMontoIrBajas/BuscarIrAnual); tabla IR anual, se omite el tramo exento |
| `CsBajasDAL` | Datos (todo parametrizado); `Guardar` (update de la fila del empleado o insert con `MAX(Id)+1`) y `Procesar` en transacción |
| Entidades | `EmpleadoBaja`, `SalarioMesBaja`, `OpcionesLiquidacion`, `Liquidacion` |
| `RptLiquidacion` (Reportes/) | XtraReport por código con la estructura de ARBaja (datos generales, historial, ingresos, egresos, total, constancia, firmas) |
| `FrmVisorReporte` | **Visor común para todos los reportes**: `FrmVisorReporte.Mostrar(reporte, nombreArchivo, propietario)`; imprimir, zoom, exportar a PDF y Excel (.xlsx). Referencias nuevas: `DevExpress.XtraReports.v24.1`, `DevExpress.XtraPrinting.v24.1` |

- **Procesar** (una transacción): `Empleado.Liquidado = 1` y `Activo = PagarDiasPlanilla`; préstamos (`Prestamo.Cancelado`) y deducciones (`DetalleDeduccion.Pagado`) si se liquidaron; sin "pagar días en planilla" borra su `DetalleNomina` de la nómina activa, con ella agrega solicitud `AusenteBasico` por los días no trabajados; borra su detalle de las nóminas activas de subsidio, vacaciones y 13vo; `Bajas.Procesada = 1` (la V.5.0 no lo marcaba).
- Al cargar el empleado se recalcula `Historico.SueldoActual` (como la V.5.0 con `ActualizaSueldoActualHistorico`).
- **Diferencias con la V.5.0**: usa el `Periodo`/`IrUltimaSemana` del tipo de nómina **del empleado** (la V.5.0 leía la primera fila de `TipoNomina`); IR quincenal sin vacaciones = impuesto anual ÷ 12 (la V.5.0 devolvía el anual); nómina semanal divide entre los viernes/sábados del mes (la V.5.0 dividía entre una variable sin asignar); antigüedad base sin fila en la tabla `Antiguedad` deja el salario sin recargo (la V.5.0 lo dejaba en 0); horas extra pendientes = suma (la V.5.0 tomaba la primera fila); `TipoBaja` "Fin" siempre (nvarchar(10)); no graba solicitud con 0 días; `Otro` guarda el concepto de otros ingresos. **Se conserva** de la V.5.0: los adelantos de 13vo/vacaciones no se restan del monto final (sí de la base de INSS/IR de vacaciones) y "otros ingresos planilla" suma todo el historial.
- **No migrado**: "Imp Detalle" (`ArepDetalleLiquidaBono`), "Imp Historial" (`ArpHistorialLiquida`), "Renovación" (`CmdRenovar`), el botón oculto `CmdCalcular` (`ArepBajas`), buscador `FrmConsulta`.
- **Verificado** en la BD de pruebas: los montos de 3 liquidaciones grabadas por la V.5.0 (códigos 3670, 5078, 3706, baja 31/12/2025, quincenales) coinciden exactamente (vacaciones con los días digitados, aguinaldo, antigüedad, INSS, IR, años, meses); flujo completo con el empleado 1064 (cálculo, grabado, reporte, exportación PDF/XLSX) y `Procesar` con `ZZTEST-CLAUDE`, todo limpiado y restaurado después; `FrmBajas`, `FrmVisorReporte` y la página del ribbon capturados con `DrawToBitmap`. Pendiente: probar en la app real (no se probó la impresión física).
- Pruebas desde PowerShell 5.1: el `AssemblyResolve` hecho con un scriptblock provoca StackOverflow al resolver tipos como `[Nullable[double]]` o `New-Object` de tipos anidados; usar un resolver en C# con `Add-Type` y `$asm.CreateInstance(...)`.
### Solicitudes de vacaciones (`FrmSolicitud`) y reportes `RptSolicitudVacaLargo` / `RptSolicitudVacaCorto`

`BarButtonVacaciones` abre `FrmSolicitud`; está en la página **Procesos** (junto a Bajas, `RibbonPageGroupProcesos`) y también en el grupo "Procesos" (`RibbonPageProcesos`) de la página Acceso. Se abre con `My.Forms.FrmSolicitud` (V.5.0 `FrmSolicitud.frm` + `Solicitudes.bas`). Interfaz armada por código (sin Designer; la clase lleva `DesignerCategory("Code")` para que Visual Studio abra el código y no un diseñador vacío).

| Pieza | Rol |
|---|---|
| `CsSolicitudDAL` | `ListarEmpleados`, `ListarNominas`, `BuscarEmpleado`, `ListarPorEmpleado`, `Buscar(numero)`, `ProximoNumero`, `HorasDia`, `Guardar` (alta/cambio + `DescuentoDiasVacaciones` en una transacción), `GuardarParaNomina` (una por empleado activo de `TipoNomina.Nomina`, todo o nada), `Eliminar`, `DiasDisfrutar`; `GrabarDescuentosEn` (Friend) también lo usa `CsImportacionDAL` (se quitó su copia) |
| `CsVacacionesDAL` | `CalcularResumen(codigo, hasta)` (acumulado/solicitado/saldo) y `DescuentaVacaciones(tipo)`; `CalculadoraVacaciones.Calcular` devuelve el desglose (`Saldo` sigue igual) |
| Entidades | `SolicitudVacaciones`, `EmpleadoSolicitud`, `ResumenVacaciones` |
| `RptSolicitudVacaLargo` (Reportes/) | XtraReport por código que reemplaza `ArepSolicitudVacaLargo` (ActiveReports 2.0): encabezado, carta del empleado (fechas de descanso y regreso = fin + 1), firmas (empleado / RRHH / jefe), resumen de 4 líneas y observaciones. Textos recuperados del `.dsx` (typos corregidos) |
| `RptSolicitudVacaCorto` (Reportes/) | Reemplaza `ArepSolicitudVacaCorto`: encabezado, datos del empleado (nombre, código, cargo, departamento), de la solicitud (inicio, fin, días, horas, observaciones), del período vacacional (vacaciones, disfrutados, disponibles) y firmas (empleado / autorizado por). La V.5.0 no llenaba las observaciones; ahora sí |

- El botón de imprimir se llama **`BtnImprimir`** (en la V.5.0 era `CmdBorrar`/`cmdborrar_Click` aunque imprimía). Más de 3 días → `RptSolicitudVacaLargo`; 3 o menos → `RptSolicitudVacaCorto` (como la V.5.0).
- Pantalla: Nuevo, Seleccionar (o doble clic en el grid), Autorizar (= grabar), Anular (borra de verdad, como la V.5.0), Imprimir, Buscar solicitud (por número; reemplaza `FrmConsulta` "Solicitud"). "Toda la nómina" reemplaza teclear el nombre de nómina como código.
- **Diferencias con la V.5.0**: números de solicitud = `MAX` numérico + 1 bajo `TABLOCKX` (y `Consecutivos.Solicitud` solo sube); la constancia usa el mismo cálculo de la pantalla (la V.5.0 recalculaba con otro bucle que contaba todos los tipos); disfrutados/disponibles = solicitado - días ya guardados de esta solicitud + días actuales (sin el +1/-1 fijo ni la suma doble de días y horas de la V.5.0); tras grabar se reabre la solicitud grabada (la V.5.0 limpiaba). La línea "1. Vacaciones períodos anteriores" sale vacía como en la V.5.0.
- **Verificado** en la BD de pruebas (empleado 1064, solicitud temporal `ZZTEST-CLAUDE`, eliminada y `Consecutivos` restaurado): alta (5 descuentos), cambio a Vacaciones Pagadas por 4 h (0.5 día, 1 descuento), rechazos (horas > horas/día, fin < inicio), eliminación; acumulado igual al guardado por la V.5.0 en 4 de 5 solicitudes (la 5.ª difiere 0.17 por el redondeo 0.0833 frente a ÷12); ambos reportes exportados a PDF/PNG (el corto sin grabar nada) y pantalla capturada con `DrawToBitmap`. Pendiente: probar en la app real e imprimir en papel.

### Consulta de registros (`FrmConsulta`, buscador común)

Migración de `FrmConsulta` de la V.5.0 (el real es `Formularios\FrmConsulCompra.frm`: grid TDBGrid con barra de filtros y un `Select Case QueProducto`). Ventana modal que **devuelve el registro elegido** a quien la llama.

| Pieza | Rol |
|---|---|
| `FrmConsulta` | UI: `GridView` con fila de filtro automático (texto = contiene, como el `LIKE '%x%'` de la V.5.0; número/fecha = igual), Pegar/Cancelar, doble clic o Enter eligen. Se usa con `FrmConsulta.Elegir(TipoBusqueda.X, Me)` → `ResultadoBusqueda` o `Nothing` si se cancela |
| `TipoBusqueda` (Tablas/) | `Empleado` (activos), `EmpleadoTodos`, `Solicitud`, `Justificacion`, `Proceso` |
| `CatalogoBusquedas` (Clases/) | Un `DefinicionBusqueda` por tipo (SQL constante, columnas, columna clave). **Para agregar una consulta: un caso aquí y un valor del enum** |
| `CsBuscadorDAL` | `Listar(definicion)`; el filtrado lo hace el grid en memoria, sin SQL dinámico |
| `ResultadoBusqueda` | Reemplaza las propiedades públicas de la V.5.0 (`CodigoEmpleado1`, `NumeroSolicitud`...): `.Codigo` (columna clave), `.Texto("campo")`, `.Fecha("campo")` |

- Usado por `FrmSolicitud.BtnBuscar` (antes un `InputBox`) y, con `FrmConsulta.AgregarBotonBuscar(combo, tipo, Me)` (lupa en el combo de código), por `FrmSolicitud`, `FrmBajas`, `FrmHistorial` (todos los empleados) y `FrmJustificaciones`. Para otros formularios basta `FrmConsulta.Elegir(TipoBusqueda.Empleado, Me)` y leer `.Codigo`.
- **Diferencias con la V.5.0**: empleados con `LEFT JOIN` a departamento/cargo (el `INNER JOIN` ocultaba empleados); el orden se cambia haciendo clic en el encabezado (reemplaza `CmdOrden`); sin la búsqueda por teclas (`Respuesta`/`Lectura`, código retirado en la V.5.0).
- **No migrado**: `CuentaContable` (consulta la tabla `Cuentas` de la BD del sistema contable, `ConexionContable`; no existe en la BD de nómina) y los `QueProducto` de empleado/historial con "pegar" directo en controles de `FrmProduccion`.
- **Verificado**: compila; las 5 consultas contra la BD de pruebas (769 activos, 1174 totales, 2545 solicitudes; `LeaveClass` y `Procesos` vacías). Pendiente: ver la ventana en pantalla real.
