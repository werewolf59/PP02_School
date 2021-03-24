using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace theSchool
{
    public partial class ServiceList : Form
    {
        int idNextService = 0, minutes;
        int? ServicesCount;
        double price, discount, discountedPrice;
        DataTable Services = new DataTable();
        string GetConnect = @"Data Source=WEREWOLF59\SQLEXPRESS;Initial Catalog=theSchool;Integrated Security=True";
        string title, discountText, showID1, showID2, showID3;
        bool stop = false;
        public static bool opened = false;

        private void button4_Click(object sender, EventArgs e)
        {
            editService(showID2);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            editService(showID3);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            editService(showID1);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "0000")
            {
                MessageBox.Show("Вы успешно вошли в режим администратора!");
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                button10.Enabled = true;
            }
            else
            {
                MessageBox.Show("Вы ввели неверный код!");
            }
        }



        private void button8_Click(object sender, EventArgs e)
        {
            if (idNextService >= 6)
            {
                idNextService -= 6;
                outputServices();
                button7.Enabled = true;
            }
            if (idNextService < 6)
                button8.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshServices();
        }

        private void editService(string id)
        {
            if (!opened)
            {
                addEditService go = new addEditService(id);
                opened = true;
                go.Show();
            }
            else
            {
                MessageBox.Show("Вы уже редактируете или добавляете услугу!");
            }
        }

        private void addService()
        {
            if (!opened)
            {
                addEditService go = new addEditService();
                opened = true;
                go.Show();
            }
            else
            {
                MessageBox.Show("Вы уже редактируете или добавляете услугу!");
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshServices();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            refreshServices();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            deleteService(showID1);
        }

        private void deleteService(string id)
        {
            if (!opened)
            {
                if (MessageBox.Show("Вы точно хотите удалить услугу?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string query = "DELETE FROM [Service] WHERE [ID] = " + id;
                    using (SqlConnection connection = new SqlConnection(GetConnect))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Close();
                    }
                    int startFrom = idNextService - 3;
                    ServicesCount--;
                    prepareServices();
                    idNextService = startFrom;
                    outputServices();
                }
            }
            else
            {
                MessageBox.Show("Нельзя удалять услугу во время добавления/редактирования услуги!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            deleteService(showID2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            deleteService(showID3);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            addService();
        }

    

        public ServiceList()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(255, 255, 255);
            comboBox1.BackColor = Color.FromArgb(231, 250, 191);
            comboBox2.BackColor = Color.FromArgb(231, 250, 191);
            groupBox1.BackColor = Color.FromArgb(231, 250, 191);
            groupBox2.BackColor = Color.FromArgb(231, 250, 191);
            groupBox3.BackColor = Color.FromArgb(231, 250, 191);
            button7.BackColor = Color.FromArgb(4, 160, 255);
            button8.BackColor = Color.FromArgb(4, 160, 255);
            button10.BackColor = Color.FromArgb(4, 160, 255);

        }


        private void button7_Click(object sender, EventArgs e)
        {
            outputServices();
            if (idNextService >= 6)
                button8.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            refreshServices();
        }

        public void refreshServices()
        {
            button7.Enabled = true;
            button8.Enabled = false;
            prepareServices();
            if (Services.Rows.Count <= 3)
                button7.Enabled = false;
            outputServices();
        }

        private void prepareServices()
        {
            Services = new DataTable();
            idNextService = 0;
            string query = "SELECT * FROM [Service] WHERE [ID]=[ID]";
            if (textBox2.Text.Trim() != "")
            {
                query += " AND ([Title] LIKE '%" + textBox2.Text.Trim() + "%' OR [Description] LIKE '%" + textBox2.Text.Trim() + "%')";
            }
            switch (comboBox2.SelectedIndex)
            {
                case 1:
                    query += " AND [Discount] >= 0 AND [Discount] < 0.05";
                    break;
                case 2:
                    query += " AND [Discount] >= 0.05 AND [Discount] < 0.15";
                    break;
                case 3:
                    query += " AND [Discount] >= 0.15 AND [Discount] < 0.3";
                    break;
                case 4:
                    query += " AND [Discount] >= 0.3 AND [Discount] < 0.7";
                    break;
                case 5:
                    query += " AND [Discount] >= 0.7 AND [Discount] < 1";
                    break;
            }
            if (comboBox1.SelectedIndex == 1)
                query += " ORDER BY [Cost] ASC";
            if (comboBox1.SelectedIndex == 2)
                query += " ORDER BY [Cost] DESC";
            using (SqlConnection connection = new SqlConnection(GetConnect))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                da.Fill(Services);
                if (Services.Rows.Count < 1)
                {
                    stop = true;
                    MessageBox.Show("Услуг нет!");
                }
                else
                {
                    stop = false;
                }
            }
            if (!ServicesCount.HasValue)
                ServicesCount = Services.Rows.Count;
            label4.Text = Services.Rows.Count + " из " + ServicesCount;
        }

        private void outputServices()
        {
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            if (!stop)
            {
                prepareShowData();
                showName1.Text = title;
                showPriceDuration1.Text = discount != 0 ? "(" + (price) + ") " + discountedPrice : price + "";
                showPriceDuration1.Text += " рублей за " + minutes + " минут";
                showDiscount1.Text = discountText;
                pictureBox1.ImageLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName +
                                            @"\" + Services.Rows[idNextService]["MainImagePath"];
                showID1 = Services.Rows[idNextService]["ID"].ToString();
            }
            else
            {
                showName1.Text = "";
                showPriceDuration1.Text = "";
                showDiscount1.Text = "";
                pictureBox1.ImageLocation = "";
                button1.Visible = false;
                button2.Visible = false;
            }
            idNextService++;
            if (idNextService < Services.Rows.Count && !stop)
            {
                prepareShowData();
                showName2.Text = title;
                showPriceDuration2.Text = discount != 0 ? "(" + (price) + ") " + discountedPrice : price + "";
                showPriceDuration2.Text += " рублей за " + minutes + " минут";
                showDiscount2.Text = discountText;
                pictureBox2.ImageLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName +
                                            @"\" + Services.Rows[idNextService]["MainImagePath"];
                showID2 = Services.Rows[idNextService]["ID"].ToString();
            }
            else
            {
                showName2.Text = "";
                showPriceDuration2.Text = "";
                showDiscount2.Text = "";
                pictureBox2.ImageLocation = "";
                button3.Visible = false;
                button4.Visible = false;
            }
            idNextService++;
            if (idNextService < Services.Rows.Count && !stop)
            {
                prepareShowData();
                showName3.Text = title;
                showPriceDuration3.Text = discount != 0 ? "(" + (price) + ") " + discountedPrice : price + "";
                showPriceDuration3.Text += " рублей за " + minutes + " минут";
                showDiscount3.Text = discountText;
                pictureBox3.ImageLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName +
                                            @"\" + Services.Rows[idNextService]["MainImagePath"];
                showID3 = Services.Rows[idNextService]["ID"].ToString();
            }
            else
            {
                showName3.Text = "";
                showPriceDuration3.Text = "";
                showDiscount3.Text = "";
                pictureBox3.ImageLocation = "";
                button5.Visible = false;
                button6.Visible = false;
            }
            idNextService++;
            if (idNextService >= Services.Rows.Count)
                button7.Enabled = false;
        }

        private void prepareShowData()
        {
            title = Services.Rows[idNextService]["Title"].ToString();
            discount = 0;
            discountText = "";
            price = Math.Round(Convert.ToDouble(Services.Rows[idNextService]["Cost"].ToString()));
            if (Services.Rows[idNextService]["Discount"].ToString() != "")
            {
                discount = Convert.ToDouble(Services.Rows[idNextService]["Discount"].ToString());
                discountedPrice = Math.Round((price - (price * discount)), 2);
                discount = Math.Round(discount * 100);
                discountText = "* скидка " + discount + "%";
            }
            minutes = Convert.ToInt32(Services.Rows[idNextService]["DurationInSeconds"].ToString()) / 60;
        }
    }
}
