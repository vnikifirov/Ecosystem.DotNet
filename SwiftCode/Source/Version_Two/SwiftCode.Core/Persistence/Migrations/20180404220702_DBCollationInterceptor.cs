
namespace SwiftCode.Core.Persistence.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class DBCollationInterceptor : Migration
    {
        private readonly string dbName = "BNKSEEKDb";
        private readonly string collate = "Cyrillic_General_CI_AS";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                string.Format
                (
                    @"ALTER DATABASE {0} COLLATE {1};",
                    dbName,
                    collate
                ),
                suppressTransaction: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.Sql(string.Format("DROP DATABASE IF EXISTS {0}", dbName));
        }
    }
}
