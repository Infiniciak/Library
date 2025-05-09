using Library.Data;
using Library.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Library
{
    public partial class BookForm : Form
    {
        private readonly LibraryDbContext _dbContext;
        private readonly Book _book;

        private TextBox txtTitle;
        private TextBox txtAuthor;
        private TextBox txtISBN;
        private NumericUpDown numReleaseYear;
        private CheckedListBox chkCategories;
        private Button btnSave;
        private Button btnCancel;

        public BookForm(LibraryDbContext dbContext, Book book = null)
        {
            _dbContext = dbContext;
            _book = book ?? new Book();
            InitializeForm();
            LoadCategories();
        }

        private void InitializeForm()
        {
            this.Text = _book.Id == 0 ? "Dodaj nową książkę" : "Edytuj książkę";
            this.ClientSize = new Size(400, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            // Kontrolki
            txtTitle = new TextBox { Location = new Point(120, 20), Width = 250, Text = _book.Title };
            txtAuthor = new TextBox { Location = new Point(120, 50), Width = 250, Text = _book.Author };
            txtISBN = new TextBox { Location = new Point(120, 80), Width = 250, Text = _book.ISBN };

            numReleaseYear = new NumericUpDown
            {
                Location = new Point(120, 110),
                Width = 100,
                Minimum = 1900,
                Maximum = DateTime.Now.Year,
                Value = _book.ReleaseYear > 0 ? _book.ReleaseYear : DateTime.Now.Year
            };

            chkCategories = new CheckedListBox
            {
                Location = new Point(120, 140),
                Width = 250,
                Height = 100
            };

            btnSave = new Button
            {
                Text = "Zapisz",
                DialogResult = DialogResult.OK,
                Location = new Point(150, 270)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Anuluj",
                DialogResult = DialogResult.Cancel,
                Location = new Point(250, 270)
            };

            // Etykiety
            var labels = new[]
            {
                new Label { Text = "Tytuł:", Location = new Point(20, 20) },
                new Label { Text = "Autor:", Location = new Point(20, 50) },
                new Label { Text = "ISBN:", Location = new Point(20, 80) },
                new Label { Text = "Rok wydania:", Location = new Point(20, 110) },
                new Label { Text = "Kategorie:", Location = new Point(20, 140) }
            };

            // Dodawanie kontrolek
            this.Controls.AddRange(labels);
            this.Controls.Add(txtTitle);
            this.Controls.Add(txtAuthor);
            this.Controls.Add(txtISBN);
            this.Controls.Add(numReleaseYear);
            this.Controls.Add(chkCategories);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadCategories()
        {
            var categories = _dbContext.Categories.ToList();
            chkCategories.Items.Clear();

            foreach (var category in categories)
            {
                var isChecked = _book.BookCategories?.Any(bc => bc.CategoryId == category.Id) ?? false;
                chkCategories.Items.Add(category.Name, isChecked);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Tytuł i autor są wymagane!", "Błąd",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            try
            {
                _book.Title = txtTitle.Text;
                _book.Author = txtAuthor.Text;
                _book.ISBN = txtISBN.Text;
                _book.ReleaseYear = (int)numReleaseYear.Value;

                // Aktualizacja kategorii
                _book.BookCategories?.Clear();

                for (int i = 0; i < chkCategories.Items.Count; i++)
                {
                    if (chkCategories.GetItemChecked(i))
                    {
                        var categoryName = chkCategories.Items[i].ToString();
                        var category = _dbContext.Categories.FirstOrDefault(c => c.Name == categoryName);

                        if (category != null)
                        {
                            if (_book.BookCategories == null)
                                _book.BookCategories = new System.Collections.Generic.List<BookCategory>();

                            _book.BookCategories.Add(new BookCategory
                            {
                                BookId = _book.Id,
                                CategoryId = category.Id
                            });
                        }
                    }
                }

                if (_book.Id == 0)
                {
                    _dbContext.Books.Add(_book);
                }

                _dbContext.SaveChanges();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania książki: {ex.Message}", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}