using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fasttool_modern.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    DeviceID = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<float>(type: "REAL", nullable: false),
                    Port = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DeviceID);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    ProfileID = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    ProfileName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.ProfileID);
                });

            migrationBuilder.CreateTable(
                name: "ButtonDatas",
                columns: table => new
                {
                    ButtonID = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    ProfileID = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceID = table.Column<string>(type: "TEXT", nullable: true),
                    ActionID = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    Color = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ButtonDatas", x => new { x.ButtonID, x.ProfileID });
                    table.ForeignKey(
                        name: "FK_ButtonDatas_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "DeviceID");
                    table.ForeignKey(
                        name: "FK_ButtonDatas_Profiles_ProfileID",
                        column: x => x.ProfileID,
                        principalTable: "Profiles",
                        principalColumn: "ProfileID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    ActionID = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Queue = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    DoAction = table.Column<string>(type: "TEXT", nullable: true),
                    ButtonID = table.Column<string>(type: "TEXT", nullable: true),
                    ProfileID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.ActionID);
                    table.ForeignKey(
                        name: "FK_Actions_ButtonDatas_ButtonID_ProfileID",
                        columns: x => new { x.ButtonID, x.ProfileID },
                        principalTable: "ButtonDatas",
                        principalColumns: new[] { "ButtonID", "ProfileID" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_ButtonID_ProfileID",
                table: "Actions",
                columns: new[] { "ButtonID", "ProfileID" });

            migrationBuilder.CreateIndex(
                name: "IX_ButtonDatas_DeviceID",
                table: "ButtonDatas",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_ButtonDatas_ProfileID",
                table: "ButtonDatas",
                column: "ProfileID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "ButtonDatas");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
