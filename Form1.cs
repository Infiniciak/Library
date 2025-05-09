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
    }
}