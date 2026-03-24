using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBS.VoucherWarehouse.Migrations
{
    /// <inheritdoc />
    public partial class NewTablesVoucherWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EcfVoucherWarehouses",
                table: "EcfVoucherWarehouses");

            migrationBuilder.RenameTable(
                name: "EcfVoucherWarehouses",
                newName: "EcfVoucherWarehouse");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EcfVoucherWarehouse",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "AuthenticationServiceUrl",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BancoPago",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoInternoComprador",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoVendedor",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComercialApprovalServiceUrl",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactoComprador",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactoEntrega",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorreoComprador",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorreoEmisor",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DGIIResponseMessage",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionComprador",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionEmisor",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionEntrega",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ENCF",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEmision",
                table: "EcfVoucherWarehouse",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntrega",
                table: "EcfVoucherWarehouse",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLimitePago",
                table: "EcfVoucherWarehouse",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaOrdenCompra",
                table: "EcfVoucherWarehouse",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimientoSecuencia",
                table: "EcfVoucherWarehouse",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ITBIS1",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ITBIS2",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ITBIS3",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificadorExtranjero",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IndicadorMontoGravado",
                table: "EcfVoucherWarehouse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IndicadorNotaCredito",
                table: "EcfVoucherWarehouse",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InformacionesAdicionalesJson",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoExento",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoGravadoI1",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoGravadoI2",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoGravadoI3",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoGravadoTotal",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoImpuestoAdicional",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoNoFacturable",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoTotal",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MunicipioComprador",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MunicipioEmisor",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreComercial",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroCuentaPago",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroFacturaInterna",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroOrdenCompra",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroPedidoInterno",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtraMonedaMontoGravadoTotal",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtraMonedaMontoTotal",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtraMonedaTipoCambio",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtraMonedaTipoMoneda",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrintFormat",
                table: "EcfVoucherWarehouse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProvinciaComprador",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinciaEmisor",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RNCComprador",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RNCEmisor",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RazonSocialComprador",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazonSocialEmisor",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceptionServiceUrl",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SendPrintedFile",
                table: "EcfVoucherWarehouse",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoAdicional",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TerminoPago",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoCuentaPago",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoECF",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoIngresos",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoPago",
                table: "EcfVoucherWarehouse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalISRPercepcion",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalISRRetencion",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalITBIS",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalITBIS1",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalITBIS2",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalITBIS3",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalITBISPercepcion",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalITBISRetenido",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TrackId",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransporteJson",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPagar",
                table: "EcfVoucherWarehouse",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "WebSite",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZonaVenta",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EcfVoucherWarehouse",
                table: "EcfVoucherWarehouse",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseAdditionalTaxes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    TipoImpuesto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TasaImpuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoImpuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseAdditionalTaxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseAdditionalTaxes_EcfVoucherWarehouse_EcfVoucherWarehouseId",
                        column: x => x.EcfVoucherWarehouseId,
                        principalTable: "EcfVoucherWarehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    NumeroLinea = table.Column<int>(type: "int", nullable: false),
                    IndicadorFacturacion = table.Column<int>(type: "int", nullable: false),
                    NombreItem = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IndicadorBienoServicio = table.Column<int>(type: "int", nullable: false),
                    DescripcionItem = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CantidadItem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnidadMedida = table.Column<int>(type: "int", nullable: false),
                    CantidadReferencia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnidadReferencia = table.Column<int>(type: "int", nullable: false),
                    GradosAlcohol = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioUnitarioReferencia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioUnitarioItem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DescuentoMonto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecargoMonto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoItem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseDetails_EcfVoucherWarehouse_EcfVoucherWarehouseId",
                        column: x => x.EcfVoucherWarehouseId,
                        principalTable: "EcfVoucherWarehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseEmitterPhones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseEmitterPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseEmitterPhones_EcfVoucherWarehouse_EcfVoucherWarehouseId",
                        column: x => x.EcfVoucherWarehouseId,
                        principalTable: "EcfVoucherWarehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseGlobalAdjustments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    TipoAjuste = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IndicadorNorma1007 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DescripcionDescuentooRecargo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TipoValor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ValorDecimal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoAjuste = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseGlobalAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseGlobalAdjustments_EcfVoucherWarehouse_EcfVoucherWarehouseId",
                        column: x => x.EcfVoucherWarehouseId,
                        principalTable: "EcfVoucherWarehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehousePaymentForms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    FormaPago = table.Column<int>(type: "int", nullable: false),
                    MontoPago = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehousePaymentForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehousePaymentForms_EcfVoucherWarehouse_EcfVoucherWarehouseId",
                        column: x => x.EcfVoucherWarehouseId,
                        principalTable: "EcfVoucherWarehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseSubtotals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    NumeroSubTotal = table.Column<int>(type: "int", nullable: false),
                    DescripcionSubTotal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseSubtotals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseSubtotals_EcfVoucherWarehouse_EcfVoucherWarehouseId",
                        column: x => x.EcfVoucherWarehouseId,
                        principalTable: "EcfVoucherWarehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseDetailAdditionalTaxes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    TipoImpuesto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TasaImpuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoImpuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseDetailAdditionalTaxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseDetailAdditionalTaxes_EcfVoucherWarehouseDetails_EcfVoucherWarehouseDetailsId",
                        column: x => x.EcfVoucherWarehouseDetailsId,
                        principalTable: "EcfVoucherWarehouseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseDetailDiscounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    TipoDescuento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DescripcionDescuento = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MontoDescuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PorcentajeDescuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseDetailDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseDetailDiscounts_EcfVoucherWarehouseDetails_EcfVoucherWarehouseDetailsId",
                        column: x => x.EcfVoucherWarehouseDetailsId,
                        principalTable: "EcfVoucherWarehouseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseDetailItemCodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    TipoCodigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CodigoItem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseDetailItemCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseDetailItemCodes_EcfVoucherWarehouseDetails_EcfVoucherWarehouseDetailsId",
                        column: x => x.EcfVoucherWarehouseDetailsId,
                        principalTable: "EcfVoucherWarehouseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseDetailOtherCurrencies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    TipoMoneda = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TipoCambio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioOtraMoneda = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoOtraMoneda = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseDetailOtherCurrencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseDetailOtherCurrencies_EcfVoucherWarehouseDetails_EcfVoucherWarehouseDetailsId",
                        column: x => x.EcfVoucherWarehouseDetailsId,
                        principalTable: "EcfVoucherWarehouseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseDetailRetentions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    IndicadorAgenteRetencionOpcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MontoITBISRetenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoISRRetenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseDetailRetentions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseDetailRetentions_EcfVoucherWarehouseDetails_EcfVoucherWarehouseDetailsId",
                        column: x => x.EcfVoucherWarehouseDetailsId,
                        principalTable: "EcfVoucherWarehouseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseDetailSubquantities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    Subcantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CodigoSubcantidad = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseDetailSubquantities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseDetailSubquantities_EcfVoucherWarehouseDetails_EcfVoucherWarehouseDetailsId",
                        column: x => x.EcfVoucherWarehouseDetailsId,
                        principalTable: "EcfVoucherWarehouseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EcfVoucherWarehouseDetailSurcharges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcfVoucherWarehouseDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    TipoRecargo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DescripcionRecargo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MontoRecargo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PorcentajeRecargo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcfVoucherWarehouseDetailSurcharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcfVoucherWarehouseDetailSurcharges_EcfVoucherWarehouseDetails_EcfVoucherWarehouseDetailsId",
                        column: x => x.EcfVoucherWarehouseDetailsId,
                        principalTable: "EcfVoucherWarehouseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseAdditionalTaxes_EcfVoucherWarehouseId",
                table: "EcfVoucherWarehouseAdditionalTaxes",
                column: "EcfVoucherWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseDetailAdditionalTaxes_EcfVoucherWarehouseDetailsId",
                table: "EcfVoucherWarehouseDetailAdditionalTaxes",
                column: "EcfVoucherWarehouseDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseDetailDiscounts_EcfVoucherWarehouseDetailsId",
                table: "EcfVoucherWarehouseDetailDiscounts",
                column: "EcfVoucherWarehouseDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseDetailItemCodes_EcfVoucherWarehouseDetailsId",
                table: "EcfVoucherWarehouseDetailItemCodes",
                column: "EcfVoucherWarehouseDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseDetailOtherCurrencies_EcfVoucherWarehouseDetailsId",
                table: "EcfVoucherWarehouseDetailOtherCurrencies",
                column: "EcfVoucherWarehouseDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseDetailRetentions_EcfVoucherWarehouseDetailsId",
                table: "EcfVoucherWarehouseDetailRetentions",
                column: "EcfVoucherWarehouseDetailsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseDetails_EcfVoucherWarehouseId",
                table: "EcfVoucherWarehouseDetails",
                column: "EcfVoucherWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseDetailSubquantities_EcfVoucherWarehouseDetailsId",
                table: "EcfVoucherWarehouseDetailSubquantities",
                column: "EcfVoucherWarehouseDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseDetailSurcharges_EcfVoucherWarehouseDetailsId",
                table: "EcfVoucherWarehouseDetailSurcharges",
                column: "EcfVoucherWarehouseDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseEmitterPhones_EcfVoucherWarehouseId",
                table: "EcfVoucherWarehouseEmitterPhones",
                column: "EcfVoucherWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseGlobalAdjustments_EcfVoucherWarehouseId",
                table: "EcfVoucherWarehouseGlobalAdjustments",
                column: "EcfVoucherWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehousePaymentForms_EcfVoucherWarehouseId",
                table: "EcfVoucherWarehousePaymentForms",
                column: "EcfVoucherWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_EcfVoucherWarehouseSubtotals_EcfVoucherWarehouseId",
                table: "EcfVoucherWarehouseSubtotals",
                column: "EcfVoucherWarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseAdditionalTaxes");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseDetailAdditionalTaxes");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseDetailDiscounts");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseDetailItemCodes");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseDetailOtherCurrencies");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseDetailRetentions");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseDetailSubquantities");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseDetailSurcharges");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseEmitterPhones");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseGlobalAdjustments");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehousePaymentForms");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseSubtotals");

            migrationBuilder.DropTable(
                name: "EcfVoucherWarehouseDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EcfVoucherWarehouse",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "AuthenticationServiceUrl",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "BancoPago",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "CodigoInternoComprador",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "CodigoVendedor",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ComercialApprovalServiceUrl",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ContactoComprador",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ContactoEntrega",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "CorreoComprador",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "CorreoEmisor",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "DGIIResponseMessage",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "DireccionComprador",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "DireccionEmisor",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "DireccionEntrega",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ENCF",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "FechaEmision",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "FechaEntrega",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "FechaLimitePago",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "FechaOrdenCompra",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "FechaVencimientoSecuencia",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ITBIS1",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ITBIS2",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ITBIS3",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "IdentificadorExtranjero",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "IndicadorMontoGravado",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "IndicadorNotaCredito",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "InformacionesAdicionalesJson",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "MontoExento",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "MontoGravadoI1",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "MontoGravadoI2",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "MontoGravadoI3",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "MontoGravadoTotal",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "MontoImpuestoAdicional",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "MontoNoFacturable",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "MontoTotal",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "MunicipioComprador",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "MunicipioEmisor",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "NombreComercial",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "NumeroCuentaPago",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "NumeroFacturaInterna",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "NumeroOrdenCompra",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "NumeroPedidoInterno",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "OtraMonedaMontoGravadoTotal",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "OtraMonedaMontoTotal",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "OtraMonedaTipoCambio",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "OtraMonedaTipoMoneda",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "PrintFormat",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ProvinciaComprador",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ProvinciaEmisor",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "RNCComprador",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "RNCEmisor",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "RazonSocialComprador",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "RazonSocialEmisor",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ReceptionServiceUrl",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "SendPrintedFile",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TelefonoAdicional",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TerminoPago",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TipoCuentaPago",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TipoECF",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TipoIngresos",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TipoPago",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TotalISRPercepcion",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TotalISRRetencion",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TotalITBIS",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TotalITBIS1",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TotalITBIS2",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TotalITBIS3",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TotalITBISPercepcion",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TotalITBISRetenido",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "TransporteJson",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ValorPagar",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "WebSite",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "ZonaVenta",
                table: "EcfVoucherWarehouse");

            migrationBuilder.RenameTable(
                name: "EcfVoucherWarehouse",
                newName: "EcfVoucherWarehouses");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "EcfVoucherWarehouses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "EcfVoucherWarehouses",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EcfVoucherWarehouses",
                table: "EcfVoucherWarehouses",
                column: "Id");
        }
    }
}
