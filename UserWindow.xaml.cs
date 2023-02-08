using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
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
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;
using System.Windows.Markup;
using System.Reflection;

namespace projectApp
{
    /// <summary>
    /// Logika interakcji dla klasy UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public string connectionString = "Data Source=LAPTOP-J43QPNEA\\SQLEXPRESS;Initial Catalog=AppDataBase;Integrated Security=True";

        ObservableCollection<Pet> listOfPets = new ObservableCollection<Pet>();
        ObservableCollection<Pet> listOfAdoptedPets = new ObservableCollection<Pet>();
        ObservableCollection<User> listUser = new ObservableCollection<User>();

        public UserWindow(int id)
        {
            InitializeComponent();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand data = new SqlCommand("SELECT id, pet_name, pet_species, pet_breed, pet_gender, pet_size, pet_weight, pet_age, pet_date_of_admission, pet_image FROM PetInfoTab WHERE pet_owner IS NULL", con);
                SqlCommand dataAdopt = new SqlCommand("SELECT id, pet_name, pet_species, pet_breed, pet_gender, pet_size, pet_weight, pet_age, pet_date_of_admission, pet_image FROM PetInfoTab WHERE pet_owner='" + id + "'", con);
                SqlCommand userInfo = new SqlCommand("SELECT id, user_name, user_first_name, user_surname, user_email,user_password FROM UserInfoTab WHERE id='" + id + "'", con);

                using (SqlDataReader reader = data.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Pet pet = new Pet
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1).Trim(),
                            Species = reader.GetString(2).Trim(),
                            Breed = reader.GetString(3).Trim(),
                            Gender = reader.GetString(4).Trim(),
                            Size = reader.GetString(5).Trim(),
                            Weight = reader.GetInt32(6),
                            Age = reader.GetInt32(7),
                            DateOfAdmission = reader.GetDateTime(8)
                        };
                        try
                        {
                            byte[] imageBytes = (byte[])reader.GetValue(9);
                            ImageSourceConverter imageConverter = new ImageSourceConverter();
                            pet.Image = (ImageSource)imageConverter.ConvertFrom(imageBytes);
                        }
                        catch
                        {
                            pet.Image = new BitmapImage(new Uri($"pack://application:,,,/Image/no-image.png"));
                        }
                        listOfPets.Add(pet);
                    }
                }

                using (SqlDataReader reader = dataAdopt.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Pet pet = new Pet
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1).Trim(),
                            Species = reader.GetString(2).Trim(),
                            Breed = reader.GetString(3).Trim(),
                            Gender = reader.GetString(4).Trim(),
                            Size = reader.GetString(5).Trim(),
                            Weight = reader.GetInt32(6),
                            Age = reader.GetInt32(7),
                            DateOfAdmission = reader.GetDateTime(8)
                        };
                        try
                        {
                            byte[] imageBytes = (byte[])reader.GetValue(9);
                            ImageSourceConverter imageConverter = new ImageSourceConverter();
                            pet.Image = (ImageSource)imageConverter.ConvertFrom(imageBytes);
                        }
                        catch
                        {
                            pet.Image = new BitmapImage(new Uri($"pack://application:,,,/Image/no-image.png"));
                        }
                        listOfAdoptedPets.Add(pet);
                    }
                }

                using (SqlDataReader readUser = userInfo.ExecuteReader())
                {
                    if (readUser.Read())
                    {
                        User user = new User
                        {
                            UserId = readUser.GetInt32(0),
                            UserName = readUser.GetString(1).Trim(),
                            UserFirstName = readUser.GetString(2).Trim(),
                            UserSurname = readUser.GetString(3).Trim(),
                            UserEmail = readUser.GetString(4).Trim(),
                            UserPassword = readUser.GetString(5).Trim(),
                        };
                        listUser.Add(user);
                    }
                }
                con.Close();
            }
            welcomer();
            FillUserInfo();
            UpdateOnfo();

            ListViewPets.ItemsSource = listOfPets;
            DataEditingPets.ItemsSource = listOfPets;

            ListAdoptedPets.ItemsSource = listOfAdoptedPets;

            int numberOfPets = listOfPets.Count;
            CollectionSize.Text = $"Znaleziono {numberOfPets} zwierzaków";
        }
        // uzupełnienie informacji o użytkowniku
        private void FillUserInfo()
        {
            CurrentUserNameTextBox.Text = listUser[0].UserName;
            CurrentUserFirstNameTextBox.Text = listUser[0].UserFirstName;
            CurrentUserSurnameTextBox.Text = listUser[0].UserSurname;
            CurrentUserEmailTextBox.Text= listUser[0].UserEmail;
            CurrentUserPasswordPassword.Password = listUser[0].UserPassword;
        }
        // uzupełnienie informacji w stronie testowej
        private void UpdateOnfo()
        {
            int users;
            int petToAdopt = listOfPets.Count;
            int adoptedPet;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand numberUsers = new SqlCommand("SELECT COUNT (id) FROM UserInfoTab", con);
                SqlCommand numberAdopted = new SqlCommand("SELECT COUNT (id) FROM PetInfoTab WHERE pet_owner IS NOT NULL", con);

                using (SqlDataReader readNumberUser = numberUsers.ExecuteReader())
                {
                    if (readNumberUser.Read())
                    {
                        users = readNumberUser.GetInt32(0);
                        if(users == 1)
                        {
                            NumberUsersTextBlock.Text = $"Jest już nas razem {users} użytkownik!";
                        }
                        else
                        {
                            NumberUsersTextBlock.Text = $"Jest już nas razem {users} użytkowników!";
                        }
                    }
                }
                using (SqlDataReader readadoptedPet = numberAdopted.ExecuteReader())
                {
                    if (readadoptedPet.Read())
                    {
                        adoptedPet = readadoptedPet.GetInt32(0);
                        if(adoptedPet == 1)
                        {
                            AllAdoptTextBlock.Text = $"Przez naszą aplikację zaadoptowano {adoptedPet} zwierzaka!";
                        }
                        else if(adoptedPet < 5)
                        {
                            AllAdoptTextBlock.Text = $"Przez naszą aplikację zaadoptowano {adoptedPet} zwierzaki!";
                        }
                        else
                        {
                            AllAdoptTextBlock.Text = $"Przez naszą aplikację zaadoptowano {adoptedPet} zwierzaków!";
                        }
                    }
                }
                con.Close();
            }
            if (petToAdopt == 1)
            {
                NumberPetsTextBlock.Text = $"W naszych schroniskach znajduje się {petToAdopt} zwierzak";
            }
            else if (petToAdopt < 5)
            {
                NumberPetsTextBlock.Text = $"W naszych schroniskach znajdują się {petToAdopt} zwierzaki";
            }
            else
            {
                NumberPetsTextBlock.Text = $"W naszych schroniskach znajduje się {petToAdopt} zwierzaków";
            }
            // uptade daty w dodawaniu zwierzaka
            AddDateOfAdmissionDataPicker.BlackoutDates.Add(new CalendarDateRange(DateTime.Now.AddDays(1), DateTime.Now.AddYears(10)));
        }

        // nadpisanie wyłączenia aplikacji
        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // funkcja ignorująca liczby lub litery
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void LetterValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // przycisk do uaktualnienia podglądu
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((AddNameTextBox.Text == "") || (AddSpeciesComboBox.Text == "") || (AddBreedTextBox.Text == "")
                || (AddGenderComboBox.Text == "") || (AddAgeTextBox.Text == "") || (AddWeightTextBox.Text == "")
                || (AddSizeComboBox.Text == "") || (AddAgeTextBox.Text == "") || (AddDateOfAdmissionDataPicker.SelectedDate.HasValue == false))
            {
                MessageBox.Show("Podgląd templatki dostępny po uzupełnieniu danych");
            }
            else
            {
                PreviewNameTextBox.Text = AddNameTextBox.Text;
                PreviewSpeciesBreedTextBox.Text = AddSpeciesComboBox.Text + ", " + AddBreedTextBox.Text;
                PreviewSexAgeWeightTextBox.Text = AddGenderComboBox.Text + ", " + AddAgeTextBox.Text + " lat, " + AddWeightTextBox.Text + " kg";
                PreviewSizeTextBox.Text = AddSizeComboBox.Text;
                DateTime selectedDate = AddDateOfAdmissionDataPicker.SelectedDate.Value;
                PreviewDateOfAdmissionTextBox.Text = selectedDate.ToString("dd-MM-yyyy");
            }
        }

        // przycisk do usuwania
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int index = DataEditingPets.SelectedIndex;

            if (index == -1)
            {
                MessageBox.Show("Nie wybrano żadnego zwierzaka");
            }
            else
            {
                Pet selectedPet = listOfPets[index];
                listOfPets.RemoveAt(index);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Usuń zaznaczone zwierzę z bazy danych
                    SqlCommand cmd = new SqlCommand("DELETE FROM PetInfoTab WHERE id = @id", con);
                    cmd.Parameters.AddWithValue("@id", selectedPet.Id);
                    cmd.ExecuteNonQuery();

                    int numberOfPets = listOfPets.Count;
                    CollectionSize.Text = $"Znaleziono {numberOfPets} zwierzaków";

                    con.Close();
                }
            }
        }

        // przycisk do dodawania
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if ((AddNameTextBox.Text == "") || (AddSpeciesComboBox.Text == "") || (AddBreedTextBox.Text == "")
                || (AddGenderComboBox.Text == "") || (AddAgeTextBox.Text == "") || (AddWeightTextBox.Text == "")
                || (AddSizeComboBox.Text == "") || (AddAgeTextBox.Text == "") || (AddDateOfAdmissionDataPicker.SelectedDate.HasValue == false))
            {
                MessageBox.Show("Uzupełnij dane przed dodaniem");
            }
            else
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    int nextId = 1;
                    if (listOfPets.Count > 0)
                    {
                        nextId = listOfPets.Last().Id;
                        nextId++;
                    }

                    // sprawdzanie czy jest zdjęcie
                    if(petImage.Source == null)
                    {
                        petImage.Source = new BitmapImage(new Uri("pack://application:,,,/Image/no-image.png"));
                    }

                    listOfPets.Add(new Pet
                    {
                        Id = nextId,
                        Name = AddNameTextBox.Text,
                        Species = AddSpeciesComboBox.Text,
                        Breed = AddBreedTextBox.Text,
                        Gender = AddGenderComboBox.Text,
                        Size = AddSizeComboBox.Text,
                        Weight = Int16.Parse(AddWeightTextBox.Text),
                        Age = Int16.Parse(AddAgeTextBox.Text),
                        DateOfAdmission = AddDateOfAdmissionDataPicker.SelectedDate.Value,
                        Image = petImage.Source
                    });

                    SqlCommand data = new SqlCommand("INSERT INTO PetInfoTab (id, pet_name, pet_species, pet_breed, pet_gender, pet_size, pet_weight, pet_age, pet_date_of_admission, pet_image) VALUES (@id, @pet_name, @pet_species, @pet_breed, @pet_gender, @pet_size, @pet_weight, @pet_age, @pet_date_of_admission, @pet_image)", con);
                    data.Parameters.AddWithValue("@id", listOfPets.Last().Id);
                    data.Parameters.AddWithValue("@pet_name", listOfPets.Last().Name);
                    data.Parameters.AddWithValue("@pet_species", listOfPets.Last().Species);
                    data.Parameters.AddWithValue("@pet_breed", listOfPets.Last().Breed);
                    data.Parameters.AddWithValue("@pet_gender", listOfPets.Last().Gender);
                    data.Parameters.AddWithValue("@pet_size", listOfPets.Last().Size);
                    data.Parameters.AddWithValue("@pet_weight", listOfPets.Last().Weight);
                    data.Parameters.AddWithValue("@pet_age", listOfPets.Last().Age);
                    data.Parameters.AddWithValue("@pet_date_of_admission", listOfPets.Last().DateOfAdmission);

                    // dodawanie zdjęcia
                    if (petImage.Source != null)
                    {
                        BitmapImage bmp = petImage.Source as BitmapImage;
                        byte[] photoArr;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(bmp));
                            encoder.Save(ms);
                            photoArr = ms.ToArray();
                        }
                        data.Parameters.AddWithValue("@pet_image", photoArr);
                    }
                    data.ExecuteNonQuery();

                    int numberOfPets = listOfPets.Count;
                    if (numberOfPets > 0)
                    {
                        CollectionSize.Text = $"Znaleziono {numberOfPets} zwierzaków";
                    }
                    else
                    {
                        CollectionSize.Text = "Brak zwierzaków w bazie danych";
                    }

                    con.Close();

                    AddNameTextBox.Text = string.Empty;
                    AddSpeciesComboBox.Text = string.Empty;
                    AddBreedTextBox.Text = string.Empty;
                    AddGenderComboBox.Text = string.Empty;
                    AddAgeTextBox.Text = string.Empty;
                    AddWeightTextBox.Text = string.Empty;
                    AddSizeComboBox.Text = string.Empty;
                    AddAgeTextBox.Text = string.Empty;
                }
            }
        }

        void welcomer()
        {
            String hour = DateTime.Now.ToString("HH");
            int TimeOfDay = Int32.Parse(hour);

            if ((TimeOfDay > 5) && (TimeOfDay < 19))
            {
                welcomerbox.Text = $"Dzień dobry, {listUser[0].UserFirstName}"; 
            }
            else
            {
                welcomerbox.Text = $"Dobry wieczór, {listUser[0].UserFirstName}";
            }
        }

        public Image petImage = new Image();
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            addImageButton.Content = "";

            ImageBrush brush = new ImageBrush();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                petImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));

                brush.ImageSource = petImage.Source;
                addImageButton.Background = brush;
            }
        }

        // Adopcja zwierzaka
        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null)
            {
                int currentId = (int)checkbox.Content;
                AdopptPet(currentId, sender);
            }
        }
        private void AdopptPet(int currentId, object sender)
        {
            var checkbox = sender as CheckBox;
            MessageBoxResult result = MessageBox.Show("Czy chcesz dokonać adopcji?", "Adopcja", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlCommand addopt = new SqlCommand("UPDATE PetInfoTab SET pet_owner=@pet_owner WHERE id = @id", con);

                        addopt.Parameters.AddWithValue("@pet_owner", listUser[0].UserId);
                        addopt.Parameters.AddWithValue("@id", currentId);
                        addopt.ExecuteNonQuery();
                        con.Close();
                    }
                    listOfAdoptedPets.Add(listOfPets.FirstOrDefault(p => p.Id == currentId));
                    checkbox.IsEnabled = false;
                    break;
                case MessageBoxResult.No:
                    checkbox.IsChecked = false;
                    break;
            }
        }
    }
}