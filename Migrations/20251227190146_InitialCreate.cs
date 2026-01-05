using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartoesCredito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Banco = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DiaVencimento = table.Column<int>(type: "INTEGER", nullable: false),
                    DiaFechamento = table.Column<int>(type: "INTEGER", nullable: false),
                    Limite = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartoesCredito", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NotificationId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ReferenceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DataAgendamento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Enviada = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DataEnvio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Cancelada = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recebimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Categoria = table.Column<int>(type: "INTEGER", nullable: false),
                    DataPrevista = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataRecebimento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValorEsperado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorRecebido = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    RecebimentoCompleto = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recebimentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pendencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Prioridade = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoDivida = table.Column<int>(type: "INTEGER", nullable: false),
                    CartaoCreditoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Parcelada = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pendencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pendencias_CartoesCredito_CartaoCreditoId",
                        column: x => x.CartaoCreditoId,
                        principalTable: "CartoesCredito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Acordos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PendenciaId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataAcordo = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NumeroParcelas = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acordos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Acordos_Pendencias_PendenciaId",
                        column: x => x.PendenciaId,
                        principalTable: "Pendencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parcelas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroParcela = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PendenciaId = table.Column<int>(type: "INTEGER", nullable: false),
                    AcordoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcelas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parcelas_Acordos_AcordoId",
                        column: x => x.AcordoId,
                        principalTable: "Acordos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Parcelas_Pendencias_PendenciaId",
                        column: x => x.PendenciaId,
                        principalTable: "Pendencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acordos_Ativo",
                table: "Acordos",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Acordos_DataAcordo",
                table: "Acordos",
                column: "DataAcordo");

            migrationBuilder.CreateIndex(
                name: "IX_Acordos_PendenciaId",
                table: "Acordos",
                column: "PendenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_CartoesCredito_Ativo",
                table: "CartoesCredito",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_CartoesCredito_Nome",
                table: "CartoesCredito",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DataAgendamento",
                table: "Notifications",
                column: "DataAgendamento");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Enviada",
                table: "Notifications",
                column: "Enviada");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationId",
                table: "Notifications",
                column: "NotificationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parcelas_AcordoId",
                table: "Parcelas",
                column: "AcordoId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcelas_DataVencimento",
                table: "Parcelas",
                column: "DataVencimento");

            migrationBuilder.CreateIndex(
                name: "IX_Parcelas_PendenciaId_NumeroParcela",
                table: "Parcelas",
                columns: new[] { "PendenciaId", "NumeroParcela" });

            migrationBuilder.CreateIndex(
                name: "IX_Parcelas_Status",
                table: "Parcelas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Pendencias_CartaoCreditoId",
                table: "Pendencias",
                column: "CartaoCreditoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pendencias_DataCriacao",
                table: "Pendencias",
                column: "DataCriacao");

            migrationBuilder.CreateIndex(
                name: "IX_Pendencias_Status",
                table: "Pendencias",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Recebimentos_DataPrevista",
                table: "Recebimentos",
                column: "DataPrevista");

            migrationBuilder.CreateIndex(
                name: "IX_Recebimentos_RecebimentoCompleto",
                table: "Recebimentos",
                column: "RecebimentoCompleto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Parcelas");

            migrationBuilder.DropTable(
                name: "Recebimentos");

            migrationBuilder.DropTable(
                name: "Acordos");

            migrationBuilder.DropTable(
                name: "Pendencias");

            migrationBuilder.DropTable(
                name: "CartoesCredito");
        }
    }
}
