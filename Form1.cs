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
        private TabControl tabControl;
        private TabPage booksTab, membersTab, categoriesTab, borrowsTab;
        private DataGridView dataGridViewBooks, dataGridViewMembers, dataGridViewCategories, dataGridViewBorrows;

        public Form1(LibraryDbContext dbContext)
        {
            _dbContext = dbContext;
            InitializeComponent();
            InitializeLayout();
            LoadBooks();
            LoadMembers();
            LoadCategories();
            LoadBorrows();
        }

        private void InitializeLayout()
        {
            tabControl = new TabControl { Dock = DockStyle.Fill };

            booksTab = CreateBooksTab();
            membersTab = CreateMembersTab();
            categoriesTab = CreateCategoriesTab();
            borrowsTab = CreateBorrowsTab();

            tabControl.TabPages.AddRange(new[] { booksTab, membersTab, categoriesTab, borrowsTab });
            Controls.Add(tabControl);
        }

        private TabPage CreateBooksTab()
        {
            var tab = new TabPage("Książki");
            var panel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };

            var btnLoad = new Button { Text = "Odśwież" };
            var btnAdd = new Button { Text = "Dodaj" };
            var btnEdit = new Button { Text = "Edytuj", Enabled = false };
            var btnDelete = new Button { Text = "Usuń", Enabled = false };

            var txtSearchBooks = new TextBox { Width = 200, PlaceholderText = "Wyszukaj książki..." };

            dataGridViewBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false
            };

            dataGridViewBooks.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id" },
                new DataGridViewTextBoxColumn { Name = "Title", HeaderText = "Tytuł", DataPropertyName = "Title" },
                new DataGridViewTextBoxColumn { Name = "Author", HeaderText = "Autor", DataPropertyName = "Author" },
                new DataGridViewTextBoxColumn { Name = "ISBN", HeaderText = "ISBN", DataPropertyName = "ISBN" },
                new DataGridViewTextBoxColumn { Name = "ReleaseYear", HeaderText = "Rok wydania", DataPropertyName = "ReleaseYear" },
                new DataGridViewTextBoxColumn { Name = "Categories", HeaderText = "Kategorie" }
            );

            txtSearchBooks.TextChanged += (s, e) => LoadBooks(txtSearchBooks.Text); // Filtrowanie książek

            dataGridViewBooks.SelectionChanged += (s, e) =>
            {
                bool selected = dataGridViewBooks.SelectedRows.Count > 0;
                btnEdit.Enabled = selected;
                btnDelete.Enabled = selected;
            };

            btnLoad.Click += (s, e) => LoadBooks(txtSearchBooks.Text); // Odświeżanie książek
            btnAdd.Click += (s, e) => { new BookForm(_dbContext).ShowDialog(); LoadBooks(txtSearchBooks.Text); };
            btnEdit.Click += (s, e) =>
            {
                if (dataGridViewBooks.SelectedRows.Count > 0)
                {
                    var book = (Book)dataGridViewBooks.SelectedRows[0].DataBoundItem;
                    new BookForm(_dbContext, book).ShowDialog();
                    LoadBooks(txtSearchBooks.Text);
                }
            };
            btnDelete.Click += (s, e) =>
            {
                if (dataGridViewBooks.SelectedRows.Count > 0)
                {
                    var book = (Book)dataGridViewBooks.SelectedRows[0].DataBoundItem;
                    if (MessageBox.Show($"Usunąć '{book.Title}'?", "Potwierdzenie", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _dbContext.Books.Remove(book);
                        _dbContext.SaveChanges();
                        LoadBooks(txtSearchBooks.Text);
                    }
                }
            };

            panel.Controls.AddRange(new Control[] { txtSearchBooks, btnLoad, btnAdd, btnEdit, btnDelete });
            tab.Controls.Add(dataGridViewBooks);
            tab.Controls.Add(panel);
            return tab;
        }

        private void LoadBooks(string filter = "")
        {
            var booksQuery = _dbContext.Books.Include(b => b.BookCategories).ThenInclude(bc => bc.Category).AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(filter) || b.Author.Contains(filter) || b.ISBN.Contains(filter));
            }

            var books = booksQuery.ToList();
            dataGridViewBooks.DataSource = books;

            foreach (DataGridViewRow row in dataGridViewBooks.Rows)
            {
                if (row.DataBoundItem is Book book)
                {
                    row.Cells["Categories"].Value = string.Join(", ", book.BookCategories.Select(bc => bc.Category?.Name));
                }
            }
        }



        private TabPage CreateMembersTab()
        {
            var tab = new TabPage("Członkowie");
            var panel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };

            var btnLoad = new Button { Text = "Odśwież" };
            var btnAdd = new Button { Text = "Dodaj" };
            var btnEdit = new Button { Text = "Edytuj", Enabled = false };
            var btnDelete = new Button { Text = "Usuń", Enabled = false };

            var txtSearchMembers = new TextBox { Width = 200, PlaceholderText = "Wyszukaj członków..." };

            dataGridViewMembers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false
            };

            dataGridViewMembers.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id" },
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Imię", DataPropertyName = "Name" },
                new DataGridViewTextBoxColumn { Name = "Surname", HeaderText = "Nazwisko", DataPropertyName = "Surname" },
                new DataGridViewTextBoxColumn { Name = "CardNumber", HeaderText = "Numer karty", DataPropertyName = "CardNumber" },
                new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", DataPropertyName = "Email" }
            );

            txtSearchMembers.TextChanged += (s, e) => LoadMembers(txtSearchMembers.Text); // Filtrowanie członków

            dataGridViewMembers.SelectionChanged += (s, e) =>
            {
                bool selected = dataGridViewMembers.SelectedRows.Count > 0;
                btnEdit.Enabled = selected;
                btnDelete.Enabled = selected;
            };

            btnLoad.Click += (s, e) => LoadMembers(txtSearchMembers.Text); // Odświeżanie członków
            btnAdd.Click += (s, e) => { new MemberForm(_dbContext).ShowDialog(); LoadMembers(txtSearchMembers.Text); };
            btnEdit.Click += (s, e) =>
            {
                var member = (Member)dataGridViewMembers.SelectedRows[0].DataBoundItem;
                new MemberForm(_dbContext, member).ShowDialog();
                LoadMembers(txtSearchMembers.Text);
            };
            btnDelete.Click += (s, e) =>
            {
                var member = (Member)dataGridViewMembers.SelectedRows[0].DataBoundItem;
                if (_dbContext.Borrows.Any(b => b.MemberId == member.Id))
                {
                    MessageBox.Show("Członek ma wypożyczenia");
                    return;
                }
                _dbContext.Members.Remove(member);
                _dbContext.SaveChanges();
                LoadMembers(txtSearchMembers.Text);
            };

            panel.Controls.AddRange(new Control[] { txtSearchMembers, btnLoad, btnAdd, btnEdit, btnDelete });
            tab.Controls.Add(dataGridViewMembers);
            tab.Controls.Add(panel);
            return tab;
        }

        private void LoadMembers(string filter = "")
        {
            var membersQuery = _dbContext.Members.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                membersQuery = membersQuery.Where(m => m.Name.Contains(filter) || m.Surname.Contains(filter) || m.Email.Contains(filter));
            }

            dataGridViewMembers.DataSource = membersQuery.ToList();
        }

        private TabPage CreateCategoriesTab()
        {
            var tab = new TabPage("Kategorie");
            var panel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };

            var btnLoad = new Button { Text = "Odśwież" };
            var btnAdd = new Button { Text = "Dodaj" };
            var btnEdit = new Button { Text = "Edytuj", Enabled = false };
            var btnDelete = new Button { Text = "Usuń", Enabled = false };

            var txtSearchCategories = new TextBox { Width = 200, PlaceholderText = "Wyszukaj kategorie..." };

            dataGridViewCategories = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false
            };

            dataGridViewCategories.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id" },
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Nazwa", DataPropertyName = "Name" }
            );

            txtSearchCategories.TextChanged += (s, e) => LoadCategories(txtSearchCategories.Text); // Filtrowanie kategorii

            dataGridViewCategories.SelectionChanged += (s, e) =>
            {
                bool selected = dataGridViewCategories.SelectedRows.Count > 0;
                btnEdit.Enabled = selected;
                btnDelete.Enabled = selected;
            };

            btnLoad.Click += (s, e) => LoadCategories(txtSearchCategories.Text); // Odświeżanie kategorii
            btnAdd.Click += (s, e) => { new CategoryForm(_dbContext).ShowDialog(); LoadCategories(txtSearchCategories.Text); };
            btnEdit.Click += (s, e) =>
            {
                var category = (Category)dataGridViewCategories.SelectedRows[0].DataBoundItem;
                new CategoryForm(_dbContext, category).ShowDialog();
                LoadCategories(txtSearchCategories.Text);
            };
            btnDelete.Click += (s, e) =>
            {
                var category = (Category)dataGridViewCategories.SelectedRows[0].DataBoundItem;
                _dbContext.Categories.Remove(category);
                _dbContext.SaveChanges();
                LoadCategories(txtSearchCategories.Text);
            };

            panel.Controls.AddRange(new Control[] { txtSearchCategories, btnLoad, btnAdd, btnEdit, btnDelete });
            tab.Controls.Add(dataGridViewCategories);
            tab.Controls.Add(panel);
            return tab;
        }

        private void LoadCategories(string filter = "")
        {
            var categoriesQuery = _dbContext.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                categoriesQuery = categoriesQuery.Where(c => c.Name.Contains(filter));
            }

            dataGridViewCategories.DataSource = categoriesQuery.ToList();
        }



        private TabPage CreateBorrowsTab()
        {
            var tab = new TabPage("Wypożyczenia");
            var panel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };

            var txtSearchBorrows = new TextBox { Width = 200, PlaceholderText = "Szukaj..." };
            var btnLoad = new Button { Text = "Odśwież" };
            var btnAdd = new Button { Text = "Dodaj" };
            var btnEdit = new Button { Text = "Edytuj", Enabled = false };
            var btnDelete = new Button { Text = "Usuń", Enabled = false };

            dataGridViewBorrows = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false
            };

            dataGridViewBorrows.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id" },
                new DataGridViewTextBoxColumn { Name = "BookTitle", HeaderText = "Książka", DataPropertyName = "BookTitle" },
                new DataGridViewTextBoxColumn { Name = "MemberName", HeaderText = "Członek", DataPropertyName = "MemberName" },
                new DataGridViewTextBoxColumn { Name = "BorrowDate", HeaderText = "Wypożyczono", DataPropertyName = "BorrowDate" },
                new DataGridViewTextBoxColumn { Name = "ReturnDate", HeaderText = "Zwrot", DataPropertyName = "ReturnDate" }
            );

            dataGridViewBorrows.SelectionChanged += (s, e) =>
            {
                bool selected = dataGridViewBorrows.SelectedRows.Count > 0;
                btnEdit.Enabled = selected;
                btnDelete.Enabled = selected;
            };

            // Filtrowanie wypożyczeń
            txtSearchBorrows.TextChanged += (s, e) => LoadBorrows(txtSearchBorrows.Text);

            btnLoad.Click += (s, e) => LoadBorrows(txtSearchBorrows.Text); // Odświeżanie wypożyczeń
            btnAdd.Click += (s, e) => { new BorrowForm(_dbContext).ShowDialog(); LoadBorrows(txtSearchBorrows.Text); };
            btnEdit.Click += (s, e) =>
            {
                var id = (int)dataGridViewBorrows.SelectedRows[0].Cells["Id"].Value;
                var borrow = _dbContext.Borrows.Include(b => b.Book).Include(b => b.Member).FirstOrDefault(b => b.Id == id);
                new BorrowForm(_dbContext, borrow).ShowDialog();
                LoadBorrows(txtSearchBorrows.Text);
            };
            btnDelete.Click += (s, e) =>
            {
                var id = (int)dataGridViewBorrows.SelectedRows[0].Cells["Id"].Value;
                var borrow = _dbContext.Borrows.Find(id);
                _dbContext.Borrows.Remove(borrow);
                _dbContext.SaveChanges();
                LoadBorrows(txtSearchBorrows.Text);
            };

            panel.Controls.AddRange(new Control[] { txtSearchBorrows, btnLoad, btnAdd, btnEdit, btnDelete });
            tab.Controls.Add(dataGridViewBorrows);
            tab.Controls.Add(panel);

            // Ensure the method returns the created tab page
            return tab;
        }

        private void LoadBooks()
        {
            var books = _dbContext.Books.Include(b => b.BookCategories).ThenInclude(bc => bc.Category).ToList();
            dataGridViewBooks.DataSource = books;

            foreach (DataGridViewRow row in dataGridViewBooks.Rows)
            {
                if (row.DataBoundItem is Book book)
                {
                    row.Cells["Categories"].Value = string.Join(", ", book.BookCategories.Select(bc => bc.Category?.Name));
                }
            }
        }

        private void LoadMembers() =>
            dataGridViewMembers.DataSource = _dbContext.Members.ToList();

        private void LoadCategories() =>
            dataGridViewCategories.DataSource = _dbContext.Categories.ToList();

        private void LoadBorrows(string filter = "")
        {
            var borrowsQuery = _dbContext.Borrows.Include(b => b.Book).Include(b => b.Member).AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                borrowsQuery = borrowsQuery.Where(b => b.Book.Title.Contains(filter) || (b.Member != null && (b.Member.Name.Contains(filter) || b.Member.Surname.Contains(filter))));
            }

            var borrows = borrowsQuery.ToList()
                .Select(b => new
                {
                    b.Id,
                    BookTitle = b.Book?.Title ?? "[Brak książki]",
                    MemberName = b.Member != null ? $"{b.Member.Name} {b.Member.Surname}" : "[Brak członka]",
                    b.BorrowDate,
                    b.ReturnDate
                }).ToList();

            dataGridViewBorrows.DataSource = borrows;
            if (dataGridViewBorrows.Columns["BorrowDate"] != null)
                dataGridViewBorrows.Columns["BorrowDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            if (dataGridViewBorrows.Columns["ReturnDate"] != null)
                dataGridViewBorrows.Columns["ReturnDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }
    }
}
