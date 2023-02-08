using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace projectApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int id;
        public MainWindow()
        {
            InitializeComponent();
        }

        public string connectionString = "Data Source=LAPTOP-J43QPNEA\\SQLEXPRESS;Initial Catalog=AppDataBase;Integrated Security=True";

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM UserInfoTab WHERE user_name='" + usernameClient.Text + "' AND user_password='" + passwordClient.Password.ToString() + "'", con);

            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows[0][0].ToString() == "1")
            {
                con.Open();
                SqlCommand curId = new SqlCommand("SELECT id FROM UserInfoTab WHERE user_name='" + usernameClient.Text + "'", con);
                SqlDataReader readId = curId.ExecuteReader();
                
                if(readId.Read())
                {
                    id = readId.GetInt32(0);
                }

                UserWindow window = new UserWindow(id);

                window.Owner = this;
                this.Hide();
                window.Show();
                con.Close();
            }
            else
            {
                MessageBox.Show("Niepoprawny login lub/i hasło");
                usernameClient.Clear();
                passwordClient.Clear();
                usernameClient.Focus();
            }
        }


        private void Singin_Button_Click(object sender, RoutedEventArgs e)
        {
            var windowRegistration = new RegistrationUser();

            windowRegistration.Owner = this;
            windowRegistration.ShowDialog();
        }
    }
}