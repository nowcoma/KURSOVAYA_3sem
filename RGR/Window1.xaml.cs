using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RGR
{
    public partial class Window1 : Window
    {
        private string jsonFileName = "memes.json"; // Имя файла JSON
        private string jsonFilePath; // Полный путь к JSON файлу
        private Meme newMeme = new Meme();

        public Window1()
        {
            InitializeComponent();
            FillComboBox(catigory_box);

            // Получаем путь к папке Debug
            string debugFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Формируем полный путь к файлу JSON в папке Debug
            jsonFilePath = Path.Combine(debugFolder, jsonFileName);
        }

        private void FillComboBox(ComboBox comboBox)
        {
            List<string> defaultCategories = new List<string> { "Коты", "Вопросы", "Другое" };

            foreach (var category in defaultCategories)
            {
                comboBox.Items.Add(category);
            }
        }

        private void ok_button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем данные из TextBox и ComboBox
            string title = title_box.Text;
            string category = catigory_box.Text;

            // Если у нас нет пути к изображению, попросим пользователя загрузить его
            if (string.IsNullOrEmpty(newMeme.ImagePath))
            {
                MessageBox.Show("Пожалуйста, загрузите изображение");
                return; // Возвращаемся из метода, чтобы пользователь загрузил изображение
            }

            // Чтение текущих данных из файла JSON
            List<Meme> memes = JsonConvert.DeserializeObject<List<Meme>>(File.ReadAllText(jsonFilePath));

            // Создаем новый мем
            Meme memeToAdd = new Meme
            {
                Title = title,
                ImagePath = newMeme.ImagePath,
                Category = category
            };

            // Добавляем новый мем в список
            memes.Add(memeToAdd);

            // Сохраняем обновленные данные в JSON файл
            string updatedJson = JsonConvert.SerializeObject(memes, Formatting.Indented);
            File.WriteAllText(jsonFilePath, updatedJson);

            // Закрываем окно
            this.Close();
        }

        private void load_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                // Присваиваем путь к изображению переменной newMeme.ImagePath
                newMeme.ImagePath = imagePath;

                // Обновляем прогрессбар и выводим сообщение
                progress.Value = 100; // Устанавливаем значение прогресса на максимальное (100)
                MessageBox.Show("Изображение загружено!");
            }
        }


    }
}