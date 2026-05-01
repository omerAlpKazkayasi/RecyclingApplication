using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecyclingApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transaction_type = table.Column<int>(type: "integer", nullable: false),
                    transaction_mode = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plate_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    opened_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    approved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    opened_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    closed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    total_weight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    mismatch_flag = table.Column<bool>(type: "boolean", nullable: false),
                    manual_override_flag = table.Column<bool>(type: "boolean", nullable: false),
                    override_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    facility_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "internal_weights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    weight_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    measured_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_type = table.Column<int>(type: "integer", nullable: false),
                    is_cancelled = table.Column<bool>(type: "boolean", nullable: false),
                    cancel_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_internal_weights", x => x.id);
                    table.ForeignKey(
                        name: "FK_internal_weights_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transaction_checks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expected_weight_difference = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    actual_weight_difference = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    internal_items_total_weight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    tolerance_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    difference_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    check_result = table.Column<int>(type: "integer", nullable: false),
                    checked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    checked_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_checks", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaction_checks_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transaction_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity_weight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    pricing_source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaction_items_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_scale_weights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    weight_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    measured_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_type = table.Column<int>(type: "integer", nullable: false),
                    is_valid = table.Column<bool>(type: "boolean", nullable: false),
                    invalid_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_scale_weights", x => x.id);
                    table.ForeignKey(
                        name: "FK_vehicle_scale_weights_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_internal_weights_public_id",
                table: "internal_weights",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_internal_weights_transaction_id",
                table: "internal_weights",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_checks_public_id",
                table: "transaction_checks",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transaction_checks_transaction_id",
                table: "transaction_checks",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_items_public_id",
                table: "transaction_items",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transaction_items_transaction_id",
                table: "transaction_items",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_public_id",
                table: "transactions",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_tenant_id_facility_id_transaction_no",
                table: "transactions",
                columns: new[] { "tenant_id", "facility_id", "transaction_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_scale_weights_public_id",
                table: "vehicle_scale_weights",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_scale_weights_transaction_id",
                table: "vehicle_scale_weights",
                column: "transaction_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "internal_weights");

            migrationBuilder.DropTable(
                name: "transaction_checks");

            migrationBuilder.DropTable(
                name: "transaction_items");

            migrationBuilder.DropTable(
                name: "vehicle_scale_weights");

            migrationBuilder.DropTable(
                name: "transactions");
        }
    }
}
