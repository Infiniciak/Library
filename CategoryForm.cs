using Library.Data;
using Library.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Library
{
    public partial class CategoryForm : Form
    {
        private readonly LibraryDbContext _dbContext;
        private readonly Category _category;

        private TextBox txtName;
        private Button btnSave;
        private Button btnCancel;

        public CategoryForm(LibraryDbContext dbContext, Category category = null)
        {
            _dbContext = dbContext;
            _category = category ?? new Category();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = _category.Id == 0 ? "Dodaj nową kategorię" : "Edytuj kategorię";
            this.ClientSize = new Size(400, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            // Kontrolki
            txtName = new TextBox { Location = new Point(120, 20), Width = 250, Text = _category.Name };

            btnSave = new Button
            {
                Text = "Zapisz",
                DialogResult = DialogResult.OK,
                Location = new Point(150, 70)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Anuluj",
                DialogResult = DialogResult.Cancel,
                Location = new Point(250, 70)
            };

            // Etykiety
            var label = new Label { Text = "Nazwa:", Location = new Point(20, 20) };

            // Dodawanie kontrolek
            this.Controls.Add(label);
            this.Controls.Add(txtName);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string categoryName = txtName.Text;

            // Walidacja, czy nazwa kategorii nie jest pusta
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                MessageBox.Show("Nazwa kategorii jest wymagana!", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            // Walidacja, że nazwa kategorii ma co najmniej 3 znaki
            if (categoryName.Length < 3)
            {
                MessageBox.Show("Nazwa kategorii musi mieć co najmniej 3 znaki.", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            // Walidacja, że nazwa kategorii nie zawiera cyfr
            if (Regex.IsMatch(categoryName, @"\d"))
            {
                MessageBox.Show("Nazwa kategorii nie może zawierać cyfr.", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            // Walidacja, że nazwa kategorii zawiera tylko litery i spacje
            if (!Regex.IsMatch(categoryName, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Nazwa kategorii może zawierać tylko litery i spacje.", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            try
            {
                _category.Name = categoryName;

                // Sprawdzanie, czy kategoria już istnieje
                if (_dbContext.Categories.Any(c => c.Name == categoryName && c.Id != _category.Id))
                {
                    MessageBox.Show("Kategoria o tej nazwie już istnieje!", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                    return;
                }

                if (_category.Id == 0)
                {
                    _dbContext.Categories.Add(_category);
                }

                _dbContext.SaveChanges();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania kategorii: {ex.Message}", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}
