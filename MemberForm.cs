using Library.Data;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Library
{
    public partial class MemberForm : Form
    {
        private readonly LibraryDbContext _dbContext;
        private readonly Member _member;

        private TextBox txtName;
        private TextBox txtSurname;
        private TextBox txtCardNumber;
        private TextBox txtEmail;
        private Button btnSave;
        private Button btnCancel;

        public MemberForm(LibraryDbContext dbContext, Member member = null)
        {
            _dbContext = dbContext;
            _member = member ?? new Member();

            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = _member.Id == 0 ? "Dodaj członka" : "Edytuj członka";
            this.Size = new System.Drawing.Size(400, 250);

            var lblName = new Label { Text = "Imię:", Left = 20, Top = 20 };
            var lblSurname = new Label { Text = "Nazwisko:", Left = 20, Top = 50 };
            var lblCard = new Label { Text = "Numer karty:", Left = 20, Top = 80 };
            var lblEmail = new Label { Text = "Email:", Left = 20, Top = 110 };

            txtName = new TextBox { Left = 120, Top = 20, Width = 200, Text = _member.Name };
            txtSurname = new TextBox { Left = 120, Top = 50, Width = 200, Text = _member.Surname };
            txtCardNumber = new TextBox { Left = 120, Top = 80, Width = 200, Text = _member.CardNumber };
            txtEmail = new TextBox { Left = 120, Top = 110, Width = 200, Text = _member.Email };

            btnSave = new Button { Text = "Zapisz", Left = 120, Top = 160 };
            btnCancel = new Button { Text = "Anuluj", Left = 220, Top = 160 };

            btnSave.Click += async (s, e) =>
            {
                string email = txtEmail.Text;
                string cardNumber = txtCardNumber.Text;

                // Walidacja, czy pola email i numer karty nie są puste
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(cardNumber))
                {
                    MessageBox.Show("Email i numer karty są wymagane!");
                    return;
                }

                // Walidacja formatu emaila
                if (!email.Contains("@"))
                {
                    MessageBox.Show("E-mail musi zawierać znak '@'.");
                    return;
                }

                try
                {
                    // Sprawdzenie poprawności formatu e-maila
                    var addr = new System.Net.Mail.MailAddress(email);
                    if (addr.Address != email) // Potwierdzenie poprawności formatu
                    {
                        MessageBox.Show("Niepoprawny format e-maila.");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Niepoprawny format e-maila.");
                    return;
                }

                // Walidacja numeru karty (sprawdzamy, czy składa się tylko z cyfr)
                if (!cardNumber.All(char.IsDigit))
                {
                    MessageBox.Show("Numer karty musi składać się tylko z cyfr.");
                    return;
                }

                // Asynchroniczna walidacja unikalności
                bool emailExists = await _dbContext.Members.AnyAsync(m => m.Email == email && m.Id != _member.Id);
                bool cardExists = await _dbContext.Members.AnyAsync(m => m.CardNumber == cardNumber && m.Id != _member.Id);

                if (emailExists)
                {
                    MessageBox.Show("Email jest już używany.");
                    return;
                }

                if (cardExists)
                {
                    MessageBox.Show("Numer karty jest już używany.");
                    return;
                }

                // Walidacja: Imię i Nazwisko nie mogą być puste
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtSurname.Text))
                {
                    MessageBox.Show("Imię i nazwisko są wymagane.");
                    return;
                }

                _member.Name = txtName.Text;
                _member.Surname = txtSurname.Text;
                _member.CardNumber = cardNumber;
                _member.Email = email;

                if (_member.Id == 0)
                    _dbContext.Members.Add(_member);

                await _dbContext.SaveChangesAsync();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblName, txtName,
                lblSurname, txtSurname,
                lblCard, txtCardNumber,
                lblEmail, txtEmail,
                btnSave, btnCancel
            });
        }
    }
}
