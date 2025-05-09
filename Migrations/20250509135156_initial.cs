using Microsoft.EntityFrameworkCore.Migrations;

namespace Library.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCategories_Books_BookId",
                table: "BookCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCategories_Categories_CategoryId",
                table: "BookCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Borrows_Books_BookId",
                table: "Borrows");

            migrationBuilder.DropForeignKey(
                name: "FK_Borrows_Members_MemberId",
                table: "Borrows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Borrows",
                table: "Borrows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Books",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookCategories",
                table: "BookCategories");

            migrationBuilder.RenameTable(
                name: "Members",
                newName: "members");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "categories");

            migrationBuilder.RenameTable(
                name: "Borrows",
                newName: "borrows");

            migrationBuilder.RenameTable(
                name: "Books",
                newName: "books");

            migrationBuilder.RenameTable(
                name: "BookCategories",
                newName: "books_categories");

            migrationBuilder.RenameIndex(
                name: "IX_Borrows_MemberId",
                table: "borrows",
                newName: "IX_borrows_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Borrows_BookId",
                table: "borrows",
                newName: "IX_borrows_BookId");

            migrationBuilder.RenameIndex(
                name: "IX_BookCategories_CategoryId",
                table: "books_categories",
                newName: "IX_books_categories_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_members",
                table: "members",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_categories",
                table: "categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_borrows",
                table: "borrows",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_books",
                table: "books",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_books_categories",
                table: "books_categories",
                columns: new[] { "BookId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_books_categories_books_BookId",
                table: "books_categories",
                column: "BookId",
                principalTable: "books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_books_categories_categories_CategoryId",
                table: "books_categories",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_borrows_books_BookId",
                table: "borrows",
                column: "BookId",
                principalTable: "books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_borrows_members_MemberId",
                table: "borrows",
                column: "MemberId",
                principalTable: "members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_books_categories_books_BookId",
                table: "books_categories");

            migrationBuilder.DropForeignKey(
                name: "FK_books_categories_categories_CategoryId",
                table: "books_categories");

            migrationBuilder.DropForeignKey(
                name: "FK_borrows_books_BookId",
                table: "borrows");

            migrationBuilder.DropForeignKey(
                name: "FK_borrows_members_MemberId",
                table: "borrows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_members",
                table: "members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categories",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_borrows",
                table: "borrows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_books",
                table: "books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_books_categories",
                table: "books_categories");

            migrationBuilder.RenameTable(
                name: "members",
                newName: "Members");

            migrationBuilder.RenameTable(
                name: "categories",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "borrows",
                newName: "Borrows");

            migrationBuilder.RenameTable(
                name: "books",
                newName: "Books");

            migrationBuilder.RenameTable(
                name: "books_categories",
                newName: "BookCategories");

            migrationBuilder.RenameIndex(
                name: "IX_borrows_MemberId",
                table: "Borrows",
                newName: "IX_Borrows_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_borrows_BookId",
                table: "Borrows",
                newName: "IX_Borrows_BookId");

            migrationBuilder.RenameIndex(
                name: "IX_books_categories_CategoryId",
                table: "BookCategories",
                newName: "IX_BookCategories_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Borrows",
                table: "Borrows",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookCategories",
                table: "BookCategories",
                columns: new[] { "BookId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookCategories_Books_BookId",
                table: "BookCategories",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCategories_Categories_CategoryId",
                table: "BookCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Borrows_Books_BookId",
                table: "Borrows",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Borrows_Members_MemberId",
                table: "Borrows",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
