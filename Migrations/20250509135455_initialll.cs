using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Library.Migrations
{
    public partial class initialll : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "books",
                columns: new[] { "Id", "Author", "ISBN", "ReleaseYear", "Title" },
                values: new object[,]
                {
                    { 1, "J.R.R. Tolkien", "9780261102217", 1937, "The Hobbit" },
                    { 2, "Frank Herbert", "9780441172719", 1965, "Dune" },
                    { 3, "Jane Austen", "9780141439518", 1813, "Pride and Prejudice" }
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Fantasy" },
                    { 2, "Science Fiction" },
                    { 3, "Romance" },
                    { 4, "Thriller" },
                    { 5, "Biography" }
                });

            migrationBuilder.InsertData(
                table: "members",
                columns: new[] { "Id", "CardNumber", "Email", "Name", "Surname" },
                values: new object[,]
                {
                    { 1, "M001", "john.doe@example.com", "John", "Doe" },
                    { 2, "M002", "jane.smith@example.com", "Jane", "Smith" }
                });

            migrationBuilder.InsertData(
                table: "books_categories",
                columns: new[] { "BookId", "CategoryId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 }
                });

            migrationBuilder.InsertData(
                table: "borrows",
                columns: new[] { "Id", "BookId", "BorrowDate", "MemberId", "ReturnDate" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 4, 25, 15, 54, 54, 910, DateTimeKind.Local).AddTicks(6380), 1, null },
                    { 2, 2, new DateTime(2025, 5, 2, 15, 54, 54, 912, DateTimeKind.Local).AddTicks(5261), 2, new DateTime(2025, 5, 8, 15, 54, 54, 912, DateTimeKind.Local).AddTicks(5288) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "books_categories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "books_categories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "books_categories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "borrows",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "borrows",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "members",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "members",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
