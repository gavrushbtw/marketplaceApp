using marketplaceApp;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public partial class CatalogForm : Form
{
    private FlowLayoutPanel productsPanel;
    DatabaseHelper db = new DatabaseHelper();

    public CatalogForm()
    {
        InitializeComponent();
        InitializeCatalog();
    }

    private void InitializeCatalog()
    {
        this.Text = "Каталог товаров";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;

        Label title = new Label()
        {
            Text = "Каталог товаров",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(70, 70, 70),
            Size = new Size(300, 40),
            Location = new Point(20, 20)
        };

        productsPanel = new FlowLayoutPanel()
        {
            Location = new Point(20, 70),
            Size = new Size(760, 480),
            AutoScroll = true,
            BackColor = Color.White
        };

        this.Controls.Add(title);
        this.Controls.Add(productsPanel);

        LoadProductsFromDatabase(); // Вызываем после создания контролов
    }

    private void LoadProductsFromDatabase()
    {
        SqlCommand command = null;
        SqlDataReader reader = null;

        try
        {
            using (SqlConnection connection = db.GetConnection())
            {
                connection.Open();

                string query = "SELECT ID_товара, НазваниеТовара, Описание, Цена, СсылкаНаИзображение FROM Товары";
                command = new SqlCommand(query, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int productId = reader.GetInt32(0);
                    string productName = reader.GetString(1);
                    string description = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    decimal price = reader.GetDecimal(3);
                    string imageUrl = reader.IsDBNull(4) ? "" : reader.GetString(4);

                    productsPanel.Controls.Add(
                        CreateProductCard(productId, productName, description, price, imageUrl)
                    );
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка загрузки товаров: " + ex.Message);
            AddSampleProducts();
        }
        finally
        {
            reader?.Close();
            reader?.Dispose();
            command?.Dispose();
        }
    }

    private Panel CreateProductCard(int productId, string productName, string description, decimal price, string imageUrl)
    {
        Panel card = new Panel()
        {
            Size = new Size(180, 220),
            BackColor = Color.FromArgb(250, 250, 250),
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(10),
            Tag = productId
        };

        PictureBox pic = new PictureBox()
        {
            Size = new Size(160, 120),
            Location = new Point(10, 10),
            BackColor = Color.LightGray,
            SizeMode = PictureBoxSizeMode.StretchImage
        };

        // Упрощенная загрузка изображения
        if (!string.IsNullOrEmpty(imageUrl))
        {
            try
            {
                if (File.Exists(imageUrl))
                {
                    using (var tempImage = Image.FromFile(imageUrl))
                    {
                        pic.Image = new Bitmap(tempImage);
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки загрузки изображения
            }
        }

        Label nameLabel = new Label()
        {
            Text = productName,
            Location = new Point(10, 140),
            Size = new Size(160, 20),
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        Label priceLabel = new Label()
        {
            Text = $"{price} руб.",
            Location = new Point(10, 165),
            Size = new Size(160, 20),
            ForeColor = Color.FromArgb(0, 100, 200),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        Button buyBtn = new Button()
        {
            Text = "В корзину",
            Size = new Size(160, 30),
            Location = new Point(10, 185),
            BackColor = Color.FromArgb(255, 152, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Tag = productId
        };

        buyBtn.Click += (s, e) => AddToCart(productId);

        card.Controls.AddRange(new Control[] { pic, nameLabel, priceLabel, buyBtn });
        return card;
    }

    private void AddToCart(int productId)
    {
        SqlCommand checkCommand = null;
        SqlCommand updateCommand = null;
        SqlCommand insertCommand = null;

        try
        {
            using (SqlConnection connection = db.GetConnection())
            {
                connection.Open();

                string checkQuery = "SELECT ID_корзины FROM Корзина WHERE ID_пользователя = @UserId AND ID_товара = @ProductId";
                checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@UserId", UserSession.CurrentUserID);
                checkCommand.Parameters.AddWithValue("@ProductId", productId);

                var result = checkCommand.ExecuteScalar();

                if (result != null)
                {
                    string updateQuery = "UPDATE Корзина SET Количество = Количество + 1 WHERE ID_пользователя = @UserId AND ID_товара = @ProductId";
                    updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@UserId", UserSession.CurrentUserID);
                    updateCommand.Parameters.AddWithValue("@ProductId", productId);
                    updateCommand.ExecuteNonQuery();
                }
                else
                {
                    string insertQuery = "INSERT INTO Корзина (ID_пользователя, ID_товара, Количество) VALUES (@UserId, @ProductId, 1)";
                    insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@UserId", UserSession.CurrentUserID);
                    insertCommand.Parameters.AddWithValue("@ProductId", productId);
                    insertCommand.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Товар добавлен в корзину!");
            Logger.Log($"Пользователь {UserSession.CurrentUserName} добавил товар ID={productId} в корзину");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка добавления в корзину: " + ex.Message);
        }
        finally
        {
            checkCommand?.Dispose();
            updateCommand?.Dispose();
            insertCommand?.Dispose();
        }
    }

    private void AddSampleProducts()
    {
        productsPanel.Controls.Add(CreateProductCard(1, "Моющее средство", "Для посуды", 299.00m, ""));
        productsPanel.Controls.Add(CreateProductCard(2, "Губки для посуды", "Набор 10 шт", 149.00m, ""));
    }

    private void InitializeComponent()
    {
        // Для дизайнера
    }
}