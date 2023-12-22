using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace RGR
{
    public class Meme
    {
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string Category { get; set; }
    }

    public partial class MainWindow : Window
    {
        private string jsonFileName = "memes.json"; // Имя файла JSON
        private string jsonFilePath; // Полный путь к JSON файлу

        public MainWindow()
        {
            InitializeComponent();

            // Получаем путь к папке Debug
            string debugFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Формируем полный путь к файлу JSON в папке Debug
            jsonFilePath = Path.Combine(debugFolder, jsonFileName);

            // Проверяем существование файла и создаем его, если он не существует
            if (!File.Exists(jsonFilePath))
            {
                // Создаем пустой список мемов
                List<Meme> emptyMemes = new List<Meme>();

                // Сохраняем пустой список в файл JSON
                string emptyJson = JsonConvert.SerializeObject(emptyMemes, Formatting.Indented);
                File.WriteAllText(jsonFilePath, emptyJson);
            }

            // Загружаем данные из JSON файла
            FillComboBox(katigorii);
        }

        private void FillComboBox(ComboBox comboBox)
        {
            List<string> defaultCategories = new List<string> { "Коты", "Вопросы", "Другое" };

            foreach (var category in defaultCategories)
            {
                comboBox.Items.Add(category);
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новое окно Window1 и отображаем его
            Window1 window1 = new Window1();
            window1.Show();
        }

        private void for_image_Click(object sender, RoutedEventArgs e)
        {
            List<Meme> memes = JsonConvert.DeserializeObject<List<Meme>>(File.ReadAllText(jsonFilePath));

            spisok.Items.Clear();

            string searchText = poisk.Text.ToLower();
            string selectedCategory = (katigorii.SelectedItem != null && katigorii.SelectedItem.ToString() != "не выбрано")
                ? katigorii.SelectedItem.ToString()
                : null;

            foreach (var meme in memes)
            {
                bool matchesSearch = string.IsNullOrEmpty(searchText) || meme.Title.ToLower().Contains(searchText);
                bool matchesCategory = selectedCategory == null || meme.Category == selectedCategory;

                if (matchesSearch && matchesCategory)
                {
                    spisok.Items.Add(meme.Title);
                }
            }
        }

        private void DisplayMemesByCategory(string category)
        {
            // Чтение текущих данных из файла JSON
            List<Meme> memes = JsonConvert.DeserializeObject<List<Meme>>(File.ReadAllText(jsonFilePath));

            // Очистка TreeView перед добавлением новых элементов
            spisok.Items.Clear();

            // Фильтрация мемов по выбранной категории
            foreach (var meme in memes)
            {
                if (meme.Category == category)
                {
                    spisok.Items.Add(meme.Title);
                }
            }
        }

        private void DisplayAllMemes()
        {
            // Чтение текущих данных из файла JSON
            List<Meme> memes = JsonConvert.DeserializeObject<List<Meme>>(File.ReadAllText(jsonFilePath));

            // Очистка TreeView перед добавлением новых элементов
            spisok.Items.Clear();

            foreach (var meme in memes)
            {
                spisok.Items.Add(meme.Title);
            }
        }

        private void spisok_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string selectedMemeTitle = (string)spisok.SelectedItem;

            // Чтение текущих данных из файла JSON
            List<Meme> memes = JsonConvert.DeserializeObject<List<Meme>>(File.ReadAllText(jsonFilePath));

            // Поиск выбранного мема по названию
            Meme selectedMeme = memes.FirstOrDefault(m => m.Title == selectedMemeTitle);

            // Отображение изображения выбранного мема в ImageBox
            if (selectedMeme != null)
            {
                if (!string.IsNullOrEmpty(selectedMeme.ImagePath) && File.Exists(selectedMeme.ImagePath))
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(selectedMeme.ImagePath));
                    imageBox.Source = bitmap;
                }
                else
                {
                    // обработка случая отсутствия файла изображения
                    imageBox.Source = null;
                }
            }
        }

        private void delete_button_Click(object sender, RoutedEventArgs e)
        {
            if (spisok.SelectedItem != null)
            {
                string selectedMemeTitle = spisok.SelectedItem.ToString();

                // Чтение текущих данных из файла JSON 
                List<Meme> memes = JsonConvert.DeserializeObject<List<Meme>>(File.ReadAllText(jsonFilePath));

                // Поиск выбранного мема по названию 
                Meme selectedMeme = memes.FirstOrDefault(m => m.Title == selectedMemeTitle);

                if (selectedMeme != null)
                {
                    memes.Remove(selectedMeme);

                    // Обновляем JSON файл после удаления мема 
                    string updatedJson = JsonConvert.SerializeObject(memes, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, updatedJson);

                    // Удаляем выбранный мем из TreeView
                    spisok.Items.Remove(selectedMemeTitle);
                }
            }
            else
            {
                MessageBox.Show("Выберите мем для удаления");
            }
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            List<Meme> memes = new List<Meme>();

            // Получаем все текущие мемы из TreeView
            foreach (var item in spisok.Items)
            {
                string memeTitle = item.ToString();
                memes.Add(new Meme { Title = memeTitle });
            }

            // Дополнительно добавляем новые мемы, если они есть в TreeView
            foreach (var meme in memes)
            {
                if (!File.ReadAllText(jsonFilePath).Contains(meme.Title))
                {
                    List<Meme> currentMemes = JsonConvert.DeserializeObject<List<Meme>>(File.ReadAllText(jsonFilePath));
                    currentMemes.Add(meme);

                    // Обновляем JSON файл с добавленными мемами
                    string updatedJson = JsonConvert.SerializeObject(currentMemes, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, updatedJson);
                }
            }

            MessageBox.Show("Информация о мемах сохранена в файле.");
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            spisok.Items.Clear();
            imageBox.Source = null; // Очистка изображения
            poisk.Text = ""; // Очистка текстового поля поиска
                             //katigorii.SelectedIndex = 0; // Сброс выбранной категории на "не выбрано"
                             // сохраняем выбранную категорию
            string selectedCategory = null;
            if (katigorii.SelectedItem != null)
            {
                selectedCategory = katigorii.SelectedItem.ToString();
            }

            if (katigorii.Items.Count > 0)
            {
                katigorii.Items.Clear();
            }
            // восстанавливаем выбранную категорию
            FillComboBox(katigorii);
        }
    }
}