using Library.Data;
using Library.Models;
using System;
using System.Drawing;
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
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Nazwa kategorii jest wymagana!", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            try
            {
                _category.Name = txtName.Text;

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