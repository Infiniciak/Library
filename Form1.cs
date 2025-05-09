using Library.Data;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Library
{
    public partial class Form1 : Form
    {
        private readonly LibraryDbContext _dbContext;
        private DataGridView dataGridView1;
        private Button btnLoadData;
        private Button btnAddBook;
        private Button btnDeleteBook;
        private Button btnEditBook;
        private DataGridView dataGridViewMembers;
        private Button btnAddMember;
        private Button btnEditMember;
        private Button btnDeleteMember;
        private DataGridView dataGridViewCategories;
        private Button btnAddCategory, btnEditCategory, btnDeleteCategory;

        private DataGridView dataGridViewBorrows;
        private Button btnAddBorrow, btnEditBorrow, btnDeleteBorrow;

        public Form1(LibraryDbContext dbContext)
        {
            _dbContext = dbContext;
            InitializeComponent();
            InitializeControls();
            LoadBooks(); // Automatyczne ładowanie przy starcie
        }

        private void InitializeControls()
        {
            // Panel na przyciski
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40
            };

            // Przycisk ładowania danych
            btnLoadData = new Button
            {
                Text = "Odśwież",
                Width = 80,
                Top = 5,
                Left = 5
            };
            btnLoadData.Click += (s, e) => LoadBooks();

            // Przycisk dodawania książki
            btnAddBook = new Button
            {
                Text = "Dodaj książkę",
                Width = 100,
                Top = 5,
                Left = 90
            };
            btnAddBook.Click += BtnAddBook_Click;

            // Przycisk edycji książki
            btnEditBook = new Button
            {
                Text = "Edytuj książkę",
                Width = 100,
                Top = 5,
                Left = 195,
                Enabled = false
            };
            btnEditBook.Click += BtnEditBook_Click;

            // Przycisk usuwania książki
            btnDeleteBook = new Button
            {
                Text = "Usuń książkę",
                Width = 100,
                Top = 5,
                Left = 300,
                Enabled = false
            };
            btnDeleteBook.Click += BtnDeleteBook_Click;

            // DataGridView
            dataGridView1 = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false
            };

            // Konfiguracja kolumn
            dataGridView1.Columns.Add("Id", "ID");
            dataGridView1.Columns.Add("Title", "Tytuł");
            dataGridView1.Columns.Add("Author", "Autor");
            dataGridView1.Columns.Add("ISBN", "ISBN");
            dataGridView1.Columns.Add("ReleaseYear", "Rok wydania");
            dataGridView1.Columns.Add("Categories", "Kategorie");

            dataGridView1.Columns["Id"].DataPropertyName = "Id";
            dataGridView1.Columns["Title"].DataPropertyName = "Title";
            dataGridView1.Columns["Author"].DataPropertyName = "Author";
            dataGridView1.Columns["ISBN"].DataPropertyName = "ISBN";
            dataGridView1.Columns["ReleaseYear"].DataPropertyName = "ReleaseYear";

            dataGridView1.SelectionChanged += (s, e) =>
            {
                btnDeleteBook.Enabled = dataGridView1.SelectedRows.Count > 0;
                btnEditBook.Enabled = dataGridView1.SelectedRows.Count > 0;
            };

            // Dodawanie kontrolek


            panel.Controls.Add(btnLoadData);
            panel.Controls.Add(btnAddBook);
            panel.Controls.Add(btnEditBook);
            panel.Controls.Add(btnDeleteBook);

            Controls.Add(dataGridView1);
            Controls.Add(panel);

            btnAddMember = new Button { Text = "Dodaj członka", Width = 100, Left = 410, Top = 5 };
            btnEditMember = new Button { Text = "Edytuj członka", Width = 100, Left = 515, Top = 5, Enabled = false };
            btnDeleteMember = new Button { Text = "Usuń członka", Width = 100, Left = 620, Top = 5, Enabled = false };

            btnAddMember.Click += BtnAddMember_Click;
            btnEditMember.Click += BtnEditMember_Click;
            btnDeleteMember.Click += BtnDeleteMember_Click;

            panel.Controls.AddRange(new[] { btnAddMember, btnEditMember, btnDeleteMember });

            dataGridViewMembers = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false
            };

            dataGridViewMembers.Columns.Add("Id", "ID");
            dataGridViewMembers.Columns.Add("Name", "Imię");
            dataGridViewMembers.Columns.Add("Surname", "Nazwisko");
            dataGridViewMembers.Columns.Add("CardNumber", "Numer karty");
            dataGridViewMembers.Columns.Add("Email", "Email");

            dataGridViewMembers.Columns["Id"].DataPropertyName = "Id";
            dataGridViewMembers.Columns["Name"].DataPropertyName = "Name";
            dataGridViewMembers.Columns["Surname"].DataPropertyName = "Surname";
            dataGridViewMembers.Columns["CardNumber"].DataPropertyName = "CardNumber";
            dataGridViewMembers.Columns["Email"].DataPropertyName = "Email";

            dataGridViewMembers.SelectionChanged += (s, e) =>
            {
                btnEditMember.Enabled = btnDeleteMember.Enabled = dataGridViewMembers.SelectedRows.Count > 0;
            };

            Controls.Add(dataGridViewMembers);

            btnAddCategory = new Button { Text = "Dodaj kategorię", Width = 100, Left = 730, Top = 5 };
            btnEditCategory = new Button { Text = "Edytuj kategorię", Width = 100, Left = 835, Top = 5, Enabled = false };
            btnDeleteCategory = new Button { Text = "Usuń kategorię", Width = 100, Left = 940, Top = 5, Enabled = false };

            btnAddCategory.Click += BtnAddCategory_Click;
            btnEditCategory.Click += BtnEditCategory_Click;
            btnDeleteCategory.Click += BtnDeleteCategory_Click;

            panel.Controls.AddRange(new[] { btnAddCategory, btnEditCategory, btnDeleteCategory });

            dataGridViewCategories = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false
            };
            dataGridViewCategories.Columns.Add("Id", "ID");
            dataGridViewCategories.Columns.Add("Name", "Nazwa");
            dataGridViewCategories.Columns["Id"].DataPropertyName = "Id";
            dataGridViewCategories.Columns["Name"].DataPropertyName = "Name";

            dataGridViewCategories.SelectionChanged += (s, e) =>
            {
                btnEditCategory.Enabled = btnDeleteCategory.Enabled = dataGridViewCategories.SelectedRows.Count > 0;
            };
            Controls.Add(dataGridViewCategories);

            btnAddBorrow = new Button { Text = "Dodaj wypożyczenie", Width = 120, Left = 1050, Top = 5 };
            btnEditBorrow = new Button { Text = "Edytuj wypożyczenie", Width = 120, Left = 1175, Top = 5, Enabled = false };
            btnDeleteBorrow = new Button { Text = "Usuń wypożyczenie", Width = 120, Left = 1300, Top = 5, Enabled = false };

            btnAddBorrow.Click += BtnAddBorrow_Click;
            btnEditBorrow.Click += BtnEditBorrow_Click;
            btnDeleteBorrow.Click += BtnDeleteBorrow_Click;

            panel.Controls.AddRange(new[] { btnAddBorrow, btnEditBorrow, btnDeleteBorrow });

            dataGridViewBorrows = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false
            };
            dataGridViewBorrows.Columns.Add("Id", "ID");
            dataGridViewBorrows.Columns.Add("BookTitle", "Tytuł książki");
            dataGridViewBorrows.Columns.Add("MemberName", "Członek");
            dataGridViewBorrows.Columns.Add("BorrowDate", "Data wypożyczenia");
            dataGridViewBorrows.Columns.Add("ReturnDate", "Data zwrotu");

            dataGridViewBorrows.SelectionChanged += (s, e) =>
            {
                btnEditBorrow.Enabled = btnDeleteBorrow.Enabled = dataGridViewBorrows.SelectedRows.Count > 0;
            };
            Controls.Add(dataGridViewBorrows);
        }

        private void LoadBooks()
        {
            try
            {
                var books = _dbContext.Books
                    .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                    .ToList();

                dataGridView1.DataSource = books;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.DataBoundItem is Book book)
                    {
                        row.Cells["Categories"].Value = string.Join(", ",
                            book.BookCategories.Select(bc => bc.Category?.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania danych: {ex.Message}", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddBook_Click(object sender, EventArgs e)
        {
            using (var form = new BookForm(_dbContext))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBooks();
                }
            }
        }



        private void BtnEditBook_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            var selectedBook = (Book)dataGridView1.SelectedRows[0].DataBoundItem;

            using (var form = new BookForm(_dbContext, selectedBook))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBooks();
                }
            }
        }

        private void BtnDeleteBook_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            var selectedBook = (Book)dataGridView1.SelectedRows[0].DataBoundItem;

            if (MessageBox.Show($"Czy na pewno chcesz usunąć książkę '{selectedBook.Title}'?",
                              "Potwierdzenie usunięcia",
                              MessageBoxButtons.YesNo,
                              MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _dbContext.Books.Remove(selectedBook);
                    _dbContext.SaveChanges();
                    LoadBooks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas usuwania książki: {ex.Message}", "Błąd",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LoadMembers()
        {
            try
            {
                var members = _dbContext.Members.ToList();
                dataGridViewMembers.DataSource = members;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania członków: {ex.Message}");
            }
        }
        private void BtnAddMember_Click(object sender, EventArgs e)
        {
            using var form = new MemberForm(_dbContext);
            if (form.ShowDialog() == DialogResult.OK) LoadMembers();
        }

        private void BtnEditMember_Click(object sender, EventArgs e)
        {
            if (dataGridViewMembers.SelectedRows.Count == 0) return;
            var member = (Member)dataGridViewMembers.SelectedRows[0].DataBoundItem;
            using var form = new MemberForm(_dbContext, member);
            if (form.ShowDialog() == DialogResult.OK) LoadMembers();
        }

        private void BtnDeleteMember_Click(object sender, EventArgs e)
        {
            if (dataGridViewMembers.SelectedRows.Count == 0) return;
            var member = (Member)dataGridViewMembers.SelectedRows[0].DataBoundItem;

            if (MessageBox.Show($"Czy chcesz usunąć członka {member.Name} {member.Surname}?", "Potwierdzenie",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _dbContext.Members.Remove(member);
                    _dbContext.SaveChanges();
                    LoadMembers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd usuwania: {ex.Message}");
                }
            }
        }

        private void LoadCategories()
        {
            try
            {
                dataGridViewCategories.DataSource = _dbContext.Categories.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania kategorii: {ex.Message}");
            }
        }

        private void LoadBorrows()
        {
            try
            {
                var borrows = _dbContext.Borrows
                    .Include(b => b.Book)
                    .Include(b => b.Member)
                    .Select(b => new
                    {
                        b.Id,
                        BookTitle = b.Book != null ? b.Book.Title : "[Brak książki]",
                        MemberName = b.Member != null ? $"{b.Member.Name} {b.Member.Surname}" : "[Brak członka]",
                        b.BorrowDate,
                        b.ReturnDate
                    })
                    .ToList();

                dataGridViewBorrows.DataSource = borrows;

                // Formatowanie daty (opcjonalne)
                if (dataGridViewBorrows.Columns["BorrowDate"] != null)
                    dataGridViewBorrows.Columns["BorrowDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                if (dataGridViewBorrows.Columns["ReturnDate"] != null)
                    dataGridViewBorrows.Columns["ReturnDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania wypożyczeń: {ex.Message}");
            }
        }
        private void BtnAddCategory_Click(object sender, EventArgs e)
        {
            using var form = new CategoryForm(_dbContext);
            if (form.ShowDialog() == DialogResult.OK) LoadCategories();
        }

        private void BtnEditCategory_Click(object sender, EventArgs e)
        {
            if (dataGridViewCategories.SelectedRows.Count == 0) return;
            var category = (Category)dataGridViewCategories.SelectedRows[0].DataBoundItem;
            using var form = new CategoryForm(_dbContext, category);
            if (form.ShowDialog() == DialogResult.OK) LoadCategories();
        }

        private void BtnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (dataGridViewCategories.SelectedRows.Count == 0) return;
            var category = (Category)dataGridViewCategories.SelectedRows[0].DataBoundItem;

            if (MessageBox.Show($"Usunąć kategorię '{category.Name}'?", "Potwierdzenie", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _dbContext.Categories.Remove(category);
                    _dbContext.SaveChanges();
                    LoadCategories();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd usuwania kategorii: {ex.Message}");
                }
            }
        }

        private void BtnAddBorrow_Click(object sender, EventArgs e)
        {
            using var form = new BorrowForm(_dbContext);
            if (form.ShowDialog() == DialogResult.OK) LoadBorrows();
        }

        private void BtnEditBorrow_Click(object sender, EventArgs e)
        {
            if (dataGridViewBorrows.SelectedRows.Count == 0 || dataGridViewBorrows.SelectedRows[0] == null)
            {
                MessageBox.Show("Proszę wybrać wypożyczenie do edycji");
                return;
            }

            try
            {
                // Get the selected row
                var selectedRow = dataGridViewBorrows.SelectedRows[0];

                // Safely get the Id value
                if (selectedRow.Cells["Id"].Value == null || !int.TryParse(selectedRow.Cells["Id"].Value.ToString(), out int borrowId))
                {
                    MessageBox.Show("Nie można odczytać ID wypożyczenia");
                    return;
                }

                // Load the borrow with related entities
                var borrow = _dbContext.Borrows
                    .Include(b => b.Book)
                    .Include(b => b.Member)
                    .FirstOrDefault(b => b.Id == borrowId);

                if (borrow == null)
                {
                    MessageBox.Show("Wypożyczenie nie zostało znalezione w bazie danych");
                    return;
                }

                using var form = new BorrowForm(_dbContext, borrow);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBorrows();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas edycji wypożyczenia: {ex.Message}");
            }
        }

        private void BtnDeleteBorrow_Click(object sender, EventArgs e)
        {
            if (dataGridViewBorrows.SelectedRows.Count == 0 || dataGridViewBorrows.SelectedRows[0] == null)
            {
                MessageBox.Show("Proszę wybrać wypożyczenie do usunięcia");
                return;
            }

            try
            {
                // Get the selected row
                var selectedRow = dataGridViewBorrows.SelectedRows[0];

                // Safely get the Id value
                if (selectedRow.Cells["Id"].Value == null || !int.TryParse(selectedRow.Cells["Id"].Value.ToString(), out int borrowId))
                {
                    MessageBox.Show("Nie można odczytać ID wypożyczenia");
                    return;
                }

                if (MessageBox.Show("Czy na pewno chcesz usunąć to wypożyczenie?", "Potwierdzenie",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var borrow = _dbContext.Borrows.FirstOrDefault(b => b.Id == borrowId);
                    if (borrow == null)
                    {
                        MessageBox.Show("Wypożyczenie nie zostało znalezione w bazie danych");
                        return;
                    }

                    _dbContext.Borrows.Remove(borrow);
                    _dbContext.SaveChanges();
                    LoadBorrows();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd usuwania wypożyczenia: {ex.Message}");
            }
        }




    }
}