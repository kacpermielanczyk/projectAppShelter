using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace projectApp
{
    /// <summary>
    /// Logika interakcji dla klasy RegistrationUser.xaml
    /// </summary>
    public partial class RegistrationUser : Window
    {
        public string connectionString = "Data Source=LAPTOP-J43QPNEA\\SQLEXPRESS;Initial Catalog=AppDataBase;Integrated Security=True";

        public RegistrationUser()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM UserInfoTab WHERE user_name='" + NewUserNameTextBox.Text + "'", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if ((NewUserNameTextBox.Text == "") || (NewUserFirstNameTextBox.Text == "") || (NewUserSurnameTextBox.Text == "") ||
                    (NewUserEmailTextBox.Text == "") || (NewUserPasswordPassword.Password.ToString() == "") || (NewUserPassword1Password.Password.ToString() == ""))
                {
                    MessageBox.Show("Uzupełnij pola");
                }
                else if (dt.Rows[0][0].ToString() == "1")
                {
                    MessageBox.Show("Użytkownik o podanej nazwie już istnieje!");
                    NewUserNameTextBox.Clear();
                    NewUserNameTextBox.Focus();
                }
                else if (NewUserPasswordPassword.Password.ToString() != NewUserPassword1Password.Password.ToString())
                {
                    MessageBox.Show("Podane hasła są różne");
                    NewUserPasswordPassword.Clear();
                    NewUserPassword1Password.Clear();
                    NewUserPasswordPassword.Focus();
                }
                else
                {
                    int maxId = 0;

                    SqlCommand data = new SqlCommand("INSERT INTO UserInfoTab (id, user_name, user_first_name, user_surname, user_email, user_password) VALUES (@id, @user_name, @user_first_name, @user_surname, @user_email, @user_password)", con);
                    String maxIdTable = "SELECT MAX(ID) FROM UserInfoTab";
                    SqlCommand commandId = new SqlCommand(maxIdTable, con);

                    using (SqlDataReader reader = commandId.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            maxId = reader.GetInt32(0);
                        }
                    }
                    maxId++;

                    User user = new User
                    {
                        UserId = maxId,
                        UserName = NewUserNameTextBox.Text,
                        UserFirstName = NewUserFirstNameTextBox.Text,
                        UserSurname = NewUserSurnameTextBox.Text,
                        UserEmail = NewUserEmailTextBox.Text,
                        UserPassword = NewUserPasswordPassword.Password.ToString()
                    };

                    data.Parameters.AddWithValue("@id", user.UserId);
                    data.Parameters.AddWithValue("@user_name", user.UserName);
                    data.Parameters.AddWithValue("@user_first_name", user.UserFirstName);
                    data.Parameters.AddWithValue("@user_surname", user.UserSurname);
                    data.Parameters.AddWithValue("@user_email", user.UserEmail);
                    data.Parameters.AddWithValue("@user_password", user.UserPassword);

                    data.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Zarejstrowano pomyślnie, możesz się zalogować");
                    this.Close();
                }
            }
        }
    }
}
