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
    public partial class ServiceImages : Form
    {
        string GetConnect = @"Data Source=WEREWOLF59\SQLEXPRESS;Initial Catalog=theSchool;Integrated Security=True";
        string serviceId, currentId;
        DataTable images = new DataTable();
        int imgId = 0;
        OpenFileDialog fb = new OpenFileDialog();
        public ServiceImages(string id)
        {
            serviceId = id;
            InitializeComponent();
        }

        private void ServiceImages_Load(object sender, EventArgs e)
        {
            fb.FilterIndex = 1;
            fb.Filter = "JPEG|*.jpg|PNG|*.png";
            prepareImages();
            outputImage();
        }

        private void prepareImages()
        {
            images = new DataTable();
            string query = "SELECT * FROM [ServicePhoto] WHERE [ServiceID] = " + serviceId;
            using (SqlConnection connection = new SqlConnection(GetConnect))
            {
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                da.Fill(images);
            }
        }

        private void outputImage()
        {
            if (images.Rows.Count > 0)
            {
                currentId = images.Rows[imgId]["ID"].ToString();
                pictureBox1.ImageLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\" + images.Rows[imgId]["PhotoPath"].ToString();
                label1.Text = "";
                button1.Enabled = true;
                button2.Enabled = true;
            }
            else
            {
                pictureBox1.ImageLocation = "";
                label1.Text = "Изображений нет";
                button1.Enabled = false;
                button2.Enabled = false;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            imgId++;
            if (imgId > images.Rows.Count - 1)
                imgId = 0;
            outputImage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите удалить это изображение?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM [ServicePhoto] WHERE [ID] = " + currentId;
                using (SqlConnection connection = new SqlConnection(GetConnect))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Close();
                }
            }
            imgId = 0;
            prepareImages();
            outputImage();
            MessageBox.Show("Изображение успешно удалено!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (fb.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(fb.FileName);
                string query = "UPDATE [ServicePhoto] SET [ServiceID] = " + serviceId + ", [PhotoPath] = 'Услуги школы\\" + Path.GetFileNameWithoutExtension(fb.FileName) + ".jpg' WHERE [ID] = " + currentId;
                using (SqlConnection connection = new SqlConnection(GetConnect))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Close();
                }
                pictureBox1.Image.Save(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Услуги школы\" + Path.GetFileNameWithoutExtension(fb.FileName) + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            prepareImages();
            MessageBox.Show("Изображение успешно изменено!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (fb.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(fb.FileName);
                string query = "INSERT INTO [ServicePhoto] (ServiceID,PhotoPath) VALUES (" + serviceId + ", 'Услуги школы\\" + Path.GetFileNameWithoutExtension(fb.FileName) + ".jpg')";
                using (SqlConnection connection = new SqlConnection(GetConnect))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Close();
                }
                pictureBox1.Image.Save(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Услуги школы\" + Path.GetFileNameWithoutExtension(fb.FileName) + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            prepareImages();
            outputImage();
            imgId = images.Rows.Count - 1;
            MessageBox.Show("Изображение успешно добавлено!");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            imgId--;
            if (imgId < 0)
                imgId = images.Rows.Count - 1;
            outputImage();
        }
    }
}
