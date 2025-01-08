using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eve.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    category_name = table.Column<string>(type: "text", nullable: false),
                    published = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "icons",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_icons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "names",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_names", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    group_name = table.Column<string>(type: "text", nullable: false),
                    published = table.Column<bool>(type: "boolean", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_groups_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id");
                });

            migrationBuilder.CreateTable(
                name: "market_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    group_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    has_types = table.Column<bool>(type: "boolean", nullable: false),
                    icon_id = table.Column<int>(type: "integer", nullable: true),
                    parent_group_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_market_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_market_groups_icons_icon_id",
                        column: x => x.icon_id,
                        principalTable: "icons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_market_groups_market_groups_parent_group_id",
                        column: x => x.parent_group_id,
                        principalTable: "market_groups",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regions", x => x.id);
                    table.ForeignKey(
                        name: "fk_regions_names_name_id",
                        column: x => x.name_id,
                        principalTable: "names",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "types",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    type_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    published = table.Column<bool>(type: "boolean", nullable: false),
                    capacity = table.Column<float>(type: "real", nullable: true),
                    mass = table.Column<float>(type: "real", nullable: true),
                    packaged_volume = table.Column<float>(type: "real", nullable: true),
                    portion_size = table.Column<int>(type: "integer", nullable: true),
                    radius = table.Column<float>(type: "real", nullable: true),
                    volume = table.Column<float>(type: "real", nullable: true),
                    is_product = table.Column<bool>(type: "boolean", nullable: false),
                    icon_id = table.Column<int>(type: "integer", nullable: true),
                    market_group_id = table.Column<int>(type: "integer", nullable: true),
                    group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_types", x => x.type_id);
                    table.ForeignKey(
                        name: "fk_types_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_types_icons_icon_id",
                        column: x => x.icon_id,
                        principalTable: "icons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_types_market_groups_market_group_id",
                        column: x => x.market_group_id,
                        principalTable: "market_groups",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "constellations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_id = table.Column<int>(type: "integer", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_constellations", x => x.id);
                    table.ForeignKey(
                        name: "fk_constellations_names_name_id",
                        column: x => x.name_id,
                        principalTable: "names",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_constellations_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    time = table.Column<int>(type: "integer", nullable: false),
                    max_production_limit = table.Column<int>(type: "integer", nullable: false),
                    blueprint_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.type_id);
                    table.ForeignKey(
                        name: "fk_products_types_type_id",
                        column: x => x.type_id,
                        principalTable: "types",
                        principalColumn: "type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reprocess_materials",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    material_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reprocess_materials", x => new { x.type_id, x.material_id });
                    table.ForeignKey(
                        name: "fk_reprocess_materials_types_material_id",
                        column: x => x.material_id,
                        principalTable: "types",
                        principalColumn: "type_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reprocess_materials_types_type_id",
                        column: x => x.type_id,
                        principalTable: "types",
                        principalColumn: "type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "solar_systems",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    security_status = table.Column<float>(type: "real", nullable: false),
                    is_hub = table.Column<bool>(type: "boolean", nullable: false),
                    name_id = table.Column<int>(type: "integer", nullable: false),
                    constellation_id = table.Column<int>(type: "integer", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_solar_systems", x => x.id);
                    table.ForeignKey(
                        name: "fk_solar_systems_constellations_constellation_id",
                        column: x => x.constellation_id,
                        principalTable: "constellations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_solar_systems_names_name_id",
                        column: x => x.name_id,
                        principalTable: "names",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_solar_systems_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "production_materials",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_production_materials", x => x.id);
                    table.ForeignKey(
                        name: "fk_production_materials_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "type_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_production_materials_types_type_id",
                        column: x => x.type_id,
                        principalTable: "types",
                        principalColumn: "type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "production_skills",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    level = table.Column<int>(type: "integer", nullable: false),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_production_skills", x => x.id);
                    table.ForeignKey(
                        name: "fk_production_skills_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "type_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_production_skills_types_type_id",
                        column: x => x.type_id,
                        principalTable: "types",
                        principalColumn: "type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    corporation_id = table.Column<int>(type: "integer", nullable: true),
                    docking_cost_per_volume = table.Column<float>(type: "real", nullable: true),
                    max_ship_volume_dockable = table.Column<int>(type: "integer", nullable: true),
                    office_rental_cost = table.Column<int>(type: "integer", nullable: true),
                    operation_id = table.Column<int>(type: "integer", nullable: true),
                    reprocessing_efficiency = table.Column<float>(type: "real", nullable: true),
                    reprocessing_hangar_flag = table.Column<int>(type: "integer", nullable: true),
                    reprocessing_station_take = table.Column<float>(type: "real", nullable: true),
                    security = table.Column<double>(type: "double precision", nullable: true),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false),
                    constellation_id = table.Column<int>(type: "integer", nullable: false),
                    solar_system_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stations", x => x.id);
                    table.ForeignKey(
                        name: "fk_stations_constellations_constellation_id",
                        column: x => x.constellation_id,
                        principalTable: "constellations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_stations_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_stations_solar_systems_solar_system_id",
                        column: x => x.solar_system_id,
                        principalTable: "solar_systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_stations_types_type_id",
                        column: x => x.type_id,
                        principalTable: "types",
                        principalColumn: "type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_categories_name",
                table: "categories",
                column: "category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_constellations_name_id",
                table: "constellations",
                column: "name_id");

            migrationBuilder.CreateIndex(
                name: "ix_constellations_region_id",
                table: "constellations",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_groups_category_id",
                table: "groups",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_groups_name",
                table: "groups",
                column: "group_name");

            migrationBuilder.CreateIndex(
                name: "ix_market_groups_icon_id",
                table: "market_groups",
                column: "icon_id");

            migrationBuilder.CreateIndex(
                name: "ix_market_groups_name",
                table: "market_groups",
                column: "group_name");

            migrationBuilder.CreateIndex(
                name: "ix_market_groups_parent_group_id",
                table: "market_groups",
                column: "parent_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_materials_product_id",
                table: "production_materials",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_materials_type_id",
                table: "production_materials",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_skills_product_id",
                table: "production_skills",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_skills_type_id",
                table: "production_skills",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_regions_name_id",
                table: "regions",
                column: "name_id");

            migrationBuilder.CreateIndex(
                name: "ix_reprocess_materials_material_id",
                table: "reprocess_materials",
                column: "material_id");

            migrationBuilder.CreateIndex(
                name: "ix_solar_systems_constellation_id",
                table: "solar_systems",
                column: "constellation_id");

            migrationBuilder.CreateIndex(
                name: "ix_solar_systems_name_id",
                table: "solar_systems",
                column: "name_id");

            migrationBuilder.CreateIndex(
                name: "ix_solar_systems_region_id",
                table: "solar_systems",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_stations_constellation_id",
                table: "stations",
                column: "constellation_id");

            migrationBuilder.CreateIndex(
                name: "ix_stations_region_id",
                table: "stations",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_stations_solar_system_id",
                table: "stations",
                column: "solar_system_id");

            migrationBuilder.CreateIndex(
                name: "ix_stations_type_id",
                table: "stations",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_types_group_id",
                table: "types",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_types_icon_id",
                table: "types",
                column: "icon_id");

            migrationBuilder.CreateIndex(
                name: "ix_types_market_group_id",
                table: "types",
                column: "market_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_types_name",
                table: "types",
                column: "type_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "production_materials");

            migrationBuilder.DropTable(
                name: "production_skills");

            migrationBuilder.DropTable(
                name: "reprocess_materials");

            migrationBuilder.DropTable(
                name: "stations");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "solar_systems");

            migrationBuilder.DropTable(
                name: "types");

            migrationBuilder.DropTable(
                name: "constellations");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "market_groups");

            migrationBuilder.DropTable(
                name: "regions");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "icons");

            migrationBuilder.DropTable(
                name: "names");
        }
    }
}
