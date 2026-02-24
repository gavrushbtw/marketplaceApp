using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace marketplaceApp
{
    public partial class CheckoutForm : Form
    {
        DatabaseHelper db = new DatabaseHelper();
        public CheckoutForm()
        {
            InitializeCheckout();
            LoadUserAddress();
        }

        private void InitializeCheckout()
        {
            this.Text = "Оформление заказа";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label title = new Label()
            {
                Text = "Оформление заказа",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Size = new Size(250, 40),
                Location = new Point(20, 20)
            };

            int y = 80;

            // Поля для оформления
            var fields = new[]
            {
            new { Label = "Адрес доставки:", Type = "text" },
            new { Label = "Способ оплаты:", Type = "combo" },
            new { Label = "Телефон:", Type = "text" },
            new { Label = "Комментарий:", Type = "textarea" }
        };

            foreach (var field in fields)
            {
                Label lbl = new Label()
                {
                    Text = field.Label,
                    Location = new Point(30, y),
                    Size = new Size(150, 25),
                    Font = new Font("Segoe UI", 10)
                };

                Control input = field.Type == "combo"
                    ? (Control)new ComboBox()
                    {
                        Location = new Point(180, y),
                        Size = new Size(250, 25),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    }
                    : new TextBox()
                    {
                        Name = field.Label == "Адрес доставки:" ? "txtAddress" : "",
                        Location = new Point(180, y),
                        Size = new Size(250, field.Type == "textarea" ? 60 : 25),
                        Multiline = field.Type == "textarea"
                    };

                if (input is ComboBox combo)
                {
                    combo.Items.AddRange(new[] { "Картой онлайн", "Наличными при получении" });
                    combo.SelectedIndex = 0;
                }

                this.Controls.Add(lbl);
                this.Controls.Add(input);
                y += field.Type == "textarea" ? 70 : 40;
            }

            // Кнопка подтверждения
            Button confirmBtn = new Button()
            {
                Text = "Подтвердить заказ",
                Size = new Size(200, 40),
                Location = new Point(150, y + 20),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            confirmBtn.Click += (s, e) =>
            {
                Logger.Log($"Пользователь {UserSession.CurrentUserName} оформил заказ");
                MessageBox.Show("Заказ успешно оформлен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            };

            this.Controls.Add(title);
            this.Controls.Add(confirmBtn);
        }

        private void CheckoutForm_Load(object sender, EventArgs e)
        {

        }
        private void LoadUserAddress()
        {
            try
            {
                using (SqlConnection connection = db.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT Адрес FROM Пользователи WHERE ID_пользователя = @UserID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", UserSession.CurrentUserID);
                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // Заполняем поле адреса
                            foreach (Control control in this.Controls)
                            {
                                if (control is TextBox textBox && textBox.Name == "txtAddress")
                                {
                                    textBox.Text = result.ToString();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки адреса: " + ex.Message);
            }
        }
    }
}
