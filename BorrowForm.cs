using Library.Data;
using Library.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Library
{
    public partial class BorrowForm : Form
    {
        private readonly LibraryDbContext _dbContext;
        private readonly Borrow _borrow;

        private ComboBox cmbBooks;
        private ComboBox cmbMembers;
        private DateTimePicker dtpBorrowDate;
        private DateTimePicker dtpReturnDate;
        private Button btnSave;
        private Button btnCancel;

        public BorrowForm(LibraryDbContext dbContext, Borrow borrow = null)
        {
            _dbContext = dbContext;
            _borrow = borrow ?? new Borrow { BorrowDate = DateTime.Now };
            InitializeForm();
            LoadBooksAndMembers();
        }

        private void InitializeForm()
        {
            this.Text = _borrow.Id == 0 ? "Dodaj nowe wypożyczenie" : "Edytuj wypożyczenie";
            this.ClientSize = new Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            // Kontrolki
            cmbBooks = new ComboBox { Location = new Point(120, 20), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMembers = new ComboBox { Location = new Point(120, 50), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            dtpBorrowDate = new DateTimePicker { Location = new Point(120, 80), Width = 250, Format = DateTimePickerFormat.Short };
            dtpReturnDate = new DateTimePicker { Location = new Point(120, 110), Width = 250, Format = DateTimePickerFormat.Short };

            btnSave = new Button
            {
                Text = "Zapisz",
                DialogResult = DialogResult.OK,
                Location = new Point(150, 170)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Anuluj",
                DialogResult = DialogResult.Cancel,
                Location = new Point(250, 170)
            };

            // Etykiety
            var labels = new[]
            {
                new Label { Text = "Książka:", Location = new Point(20, 20) },
                new Label { Text = "Członek:", Location = new Point(20, 50) },
                new Label { Text = "Data wypożyczenia:", Location = new Point(20, 80) },
                new Label { Text = "Data zwrotu:", Location = new Point(20, 110) }
            };

            // Dodawanie kontrolek
            this.Controls.AddRange(labels);
            this.Controls.Add(cmbBooks);
            this.Controls.Add(cmbMembers);
            this.Controls.Add(dtpBorrowDate);
            this.Controls.Add(dtpReturnDate);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            // Ustawienie wartości dla istniejącego wypożyczenia
            if (_borrow.Id != 0)
            {
                dtpBorrowDate.Value = _borrow.BorrowDate;
                dtpReturnDate.Value = _borrow.ReturnDate ?? DateTime.Now;
            }
        }

        private void LoadBooksAndMembers()
        {
            try
            {
                // Ładowanie dostępnych książek (tylko nie wypożyczonych)
                var availableBooks = _dbContext.Books
                    .Where(b => !b.Borrows.Any(br => br.ReturnDate == null))
                    .ToList();

                cmbBooks.DataSource = availableBooks;
                cmbBooks.DisplayMember = "Title";
                cmbBooks.ValueMember = "Id";

                // Ładowanie członków
                var members = _dbContext.Members.ToList();
                cmbMembers.DataSource = members;
                cmbMembers.DisplayMember = "FullName";
                cmbMembers.ValueMember = "Id";

                // Jeśli edytujemy istniejące wypożyczenie
                if (_borrow.Id != 0)
                {
                    cmbBooks.SelectedValue = _borrow.BookId;
                    cmbMembers.SelectedValue = _borrow.MemberId;
                    dtpBorrowDate.Value = _borrow.BorrowDate;
                    dtpReturnDate.Value = _borrow.ReturnDate ?? DateTime.Now;
                    dtpReturnDate.Checked = _borrow.ReturnDate.HasValue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania danych: {ex.Message}");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbBooks.SelectedItem == null || cmbMembers.SelectedItem == null)
            {
                MessageBox.Show("Proszę wybrać książkę i członka!", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            try
            {
                var selectedBook = cmbBooks.SelectedItem as Book;
                var selectedMember = cmbMembers.SelectedItem as Member;

                if (selectedBook == null || selectedMember == null)
                {
                    MessageBox.Show("Nieprawidłowe dane książki lub członka", "Błąd",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                    return;
                }

                _borrow.BookId = selectedBook.Id;
                _borrow.MemberId = selectedMember.Id;
                _borrow.BorrowDate = dtpBorrowDate.Value;
                _borrow.ReturnDate = dtpReturnDate.Checked ? dtpReturnDate.Value : null;

                if (_borrow.Id == 0)
                {
                    _dbContext.Borrows.Add(_borrow);
                }

                _dbContext.SaveChanges();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania wypożyczenia: {ex.Message}", "Błąd",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}