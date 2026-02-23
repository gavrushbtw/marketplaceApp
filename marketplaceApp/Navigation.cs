using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace marketplaceApp
{
    public partial class Navigation : Form
    {
        public Navigation()
        {
            InitializeComponent();
            CreateNavigation();
            this.SetGradientBackground(
                Color.FromArgb(255, 183, 77),
                Color.FromArgb(255, 138, 101)
            );
        }

        private void CreateNavigation()
        {
            this.Text = "Маркетплейс Хозтоваров";
            this.Width = 400;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Заголовок
            Label title = new Label()
            {
                Text = "Главное меню",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Size = new Size(250, 40),
                Location = new Point(75, 30)
            };

            // Кнопки навигации
            List<string> buttons = new List<string>
            {
                "🛍️ Каталог товаров",
                "🛒 Корзина",
                "👤 Мой профиль"
            };

            // Кнопка админ-панели только для Admin
            if (UserSession.CurrentUserRole == "Admin")
            {
                buttons.Add("⚙ Панель администратора");
            }

            buttons.Add("🚪 Выход");
            int y = 100;

            foreach (string btnText in buttons)
            {
                Button btn = new Button()
                {
                    Text = btnText,
                    Size = new Size(250, 45),
                    Location = new Point(75, y),
                    BackColor = Color.FromArgb(255, 152, 0),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                btn.Click += (s, e) => NavigateToForm(btnText);

                this.Controls.Add(btn);
                y += 60;
            }
            this.Height = y + 60;

            this.Controls.Add(title);
        }

        private void NavigateToForm(string formName)
        {
            Form formToOpen = null;

            // Классический switch вместо switch expression
            switch (formName)
            {
                case "🛍️ Каталог товаров":
                    formToOpen = new CatalogForm();
                    break;
                case "🛒 Корзина":
                    formToOpen = new CartForm();
                    break;
                case "👤 Мой профиль":
                    formToOpen = new ProfileForm();
                    break;
                case "⚙ Панель администратора":
                    formToOpen = new AdminForm();
                    break;
                case "🚪 Выход":
                    formToOpen = null;
                    break;
            }

            if (formToOpen != null)
            {
                formToOpen.Show();
                this.Hide(); // Скрываем главную форму
                formToOpen.FormClosed += (s, e) => this.Show(); // Показываем при закрытии
            }
            else
            {
                Application.Exit(); // Выход из приложения
            }
        }
    }
}
