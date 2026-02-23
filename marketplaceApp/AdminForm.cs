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
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
            CenterToScreen();
            if (UserSession.CurrentUserRole != "Admin")
            {
                Logger.Log($"Попытка несанкционированного доступа к AdminForm пользователем {UserSession.CurrentUserName}");
                MessageBox.Show("Доступ запрещён!");
                this.Close();
                return;
            }

            this.Text = "Панель администратора";
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {

        }
    }
}
