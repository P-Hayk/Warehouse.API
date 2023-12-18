using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Infrastructure.Migrations.OrderState
{
    /// <inheritdoc />
    public partial class State1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProcessingSagaStates",
                table: "OrderProcessingSagaStates");

            migrationBuilder.RenameTable(
                name: "OrderProcessingSagaStates",
                newName: "OrderProcessingSagaState");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProcessingSagaState",
                table: "OrderProcessingSagaState",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProcessingSagaState",
                table: "OrderProcessingSagaState");

            migrationBuilder.RenameTable(
                name: "OrderProcessingSagaState",
                newName: "OrderProcessingSagaStates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProcessingSagaStates",
                table: "OrderProcessingSagaStates",
                column: "Id");
        }
    }
}
