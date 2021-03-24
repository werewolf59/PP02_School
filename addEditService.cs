using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace theSchool
{
    public partial class addEditService : Form
    {
        string serviceId;
        string GetConnect = @"Data Source=WEREWOLF59\SQLEXPRESS;Initial Catalog=theSchool;Integrated Security=True";
        OpenFileDialog fb = new OpenFileDialog();
        bool imgON = false;

        public addEditService()
        {
            InitializeComponent();
            button2.BackColor = Color.FromArgb(4, 160, 255);

        }

        public addEditService(string id)
        {
            serviceId = id;
            InitializeComponent();

        }

        private void addEditService_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(serviceId))
            {
                label8.Visible = true;
                textBox6.Visible = true;
                label7.Text = "Редактирование услуги";
                button2.Text = "Редактировать";
                button3.Visible = true;
                string query = "SELECT * FROM [Service] WHERE [ID] = " + serviceId;
                using (SqlConnection connection = new SqlConnection(GetConnect))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        textBox1.Text = reader["Title"].ToString();
                        textBox2.Text = reader["Cost"].ToString();
                        textBox3.Text = reader["DurationInSeconds"].ToString();
                        textBox4.Text = reader["Description"].ToString();
                        textBox5.Text = reader["Discount"].ToString();
                        textBox6.Text = reader["ID"].ToString();
                        pictureBox1.ImageLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\" + reader["MainImagePath"].ToString();
                    }
                    reader.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string query = "SELECT [Title] FROM [Service]";
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(GetConnect))
            {
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                da.Fill(dt);
            }
            if (textBox1.Text.Trim() != "" || textBox2.Text.Trim() != "" || textBox3.Text.Trim() != "")
            {
                string description = textBox4.Text.Trim() != "" ? textBox4.Text.Trim() : "";
                string discount = textBox5.Text.Trim() != "" ? textBox5.Text.Trim().Replace(" ", "") : "NULL";
                string cost = textBox2.Text.Trim().Replace(',', '.').Replace(" ", "");
                string duration = textBox3.Text.Trim().Replace(',', '.').Replace(" ", "");
                if (String.IsNullOrEmpty(serviceId))
                {
                    bool goon = true;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][0].ToString() == textBox1.Text.Trim())
                        {
                            goon = false;
                        }
                        if (!goon)
                            MessageBox.Show("Услуга с таким названием уже есть в базе данных!");
                    }
                    double Num;
                    int Num2;
                    bool isNum = double.TryParse(cost, out Num);
                    bool isNum2 = int.TryParse(duration, out Num2);
                    bool isNum3 = int.TryParse(discount, out Num2);
                    if (isNum == false || isNum2 == false || (discount != "NULL" && isNum3 == false))
                    {
                        MessageBox.Show("Стоимость, длительность и скидка должны быть числами, длительность должна быть целым числом!");
                        goon = false;
                    }
                    else
                    {
                        if (Convert.ToInt32(duration) < 0 || Convert.ToInt32(duration) > 240)
                        {
                            MessageBox.Show("Длительность не может быть больше 4 часов и не может быть отрицательным числом!");
                            goon = false;
                        }
                    }
                    if (goon)
                    {
                        query = "INSERT INTO [Service] ([Title],[Cost],[DurationInSeconds],[Description],[Discount],[MainImagePath]) VALUES " +
                        "('" + textBox1.Text.Trim() + "'," + cost + "," + duration + ",'" + description + "',"
                        + discount + "," + (imgON ? "'Услуги школы\\" + textBox1.Text.Trim() + ".jpg'" : "NULL") + ")";
                        using (SqlConnection connection = new SqlConnection(GetConnect))
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(query, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Close();
                        }
                        if (imgON)
                            pictureBox1.Image.Save(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Услуги школы\" + textBox1.Text.Trim() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        MessageBox.Show("Услуга успешно добавлена!");
                    }
                }
                else
                {
                    query = "UPDATE [Service] SET [Title] = '" + textBox1.Text.Trim() + "', [Cost] = " + cost + ", [DurationInSeconds] = " +
                            duration + ", [Description] = '" + description + "', [Discount] = " + discount 
                            + (imgON ? ", [MainImagePath] = 'Услуги школы\\" + textBox1.Text.Trim() + ".jpg'" : "") + " WHERE [ID] = " + serviceId;
                    using (SqlConnection connection = new SqlConnection(GetConnect))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Close();
                    }
                    if (imgON)
                        pictureBox1.Image.Save(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Услуги школы\" + textBox1.Text.Trim() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    MessageBox.Show("Услуга успешно обновлена!");
                }
                ServiceList sl = new ServiceList();
                sl.refreshServices();
            }
            else
            {
                MessageBox.Show("Вы не заполнили все необходимые поля!\nПоля необходимые для заполнения отмечены звездочкой.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fb.FilterIndex = 1;
            fb.Filter = "JPEG|*.jpg|PNG|*.png";
            if (fb.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(fb.FileName);
                imgON = true;
            }
        }

        private void addEditService_FormClosing(object sender, FormClosingEventArgs e)
        {
            ServiceList.opened = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ServiceImages si = new ServiceImages(serviceId);
            si.Show();
        }
    }
}
