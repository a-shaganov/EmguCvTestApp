using Emgu.CV;
using Emgu.CV.Structure;
using EmguCvTestApp.Converters;
using EmguCvTestApp.Dispatchers;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmguCvTestApp
{
    /// <summary>
    /// Основной класс отображаемой формы
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Компонент отвечающий за воспроизведение видеофайла
        /// </summary>
        private VideoCapture Capture;

        /// <summary>
        /// Настройка для определения лица
        /// </summary>
        private CascadeClassifier CascadeFace;

        /// <summary>
        /// Конструктор
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Выбираем конфигурацию для определения лица на видео
            CascadeFace = LoadCascadeClassifier("haarcascade_frontalface_alt.xml");
            // Инициализация диспатчера
            UserInterfaceDispatcher.Initialize();
        }

        /// <summary>
        /// Метод обработки видео на распознавание лица
        /// </summary>
        /// <param name="sender">Объект который вызывает событие</param>
        /// <param name="e">Параметры события</param>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            // Открываем диалоговое окно для выбора файла
            String fileName = SelectFileFromFileDialog();

            // Проверяем все ли нормально выбрано и загрузилось
            if (String.IsNullOrEmpty(fileName) || (CascadeFace == null))
                return;

            // Проверяем, если компонент отвечающий за воспроизведение видеофайла существует
            // то остонавливаем воспроизведение и очищаем компонент
            if (Capture != null)
            {
                Capture.ImageGrabbed -= Capture_ImageGrabbed;
                Capture.Stop();
                Capture.Dispose();
            }

            // Создаем компонент отвечающий за воспроизведение видеофайла, загружаем в него файл
            // подключаем событие для возпроизвидения файла и стартуем
            try
            {
                Capture = new VideoCapture(fileName);
                Capture.ImageGrabbed += Capture_ImageGrabbed;
                Capture.Start();
            }
            catch
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            VideoCapture capture = sender as VideoCapture;
            if (capture == null)
                return;

            using (Mat frame = new Mat())
            {
                capture.Retrieve(frame);

                Image<Bgr, Byte> image = new Image<Bgr, byte>(frame.Size);
                frame.CopyTo(image);

                using (Image<Gray, Byte> gray = image.Convert<Gray, Byte>())
                {
                    System.Drawing.Rectangle[] faces = CascadeFace.DetectMultiScale(gray, 1.1, 10);
                    foreach (System.Drawing.Rectangle face in faces)
                        image.Draw(face, new Bgr(System.Drawing.Color.Yellow), 3);
                }

                UserInterfaceDispatcher.BeginInvokeOnUi(() =>
                {
                    if (image != null)
                    {
                        // Конвертируем изображение для дальнейшего его отображения в программе
                        BitmapSource source = BitmapSourceConvert.ToBitmapSource(image.ToBitmap());
                        image.Dispose();
                        if (source != null)
                            srcImg.Source = source;
                    }
                });
            }

            Thread.Sleep(20);
        }

        /// <summary>
        /// Метод для загрузки файла из диалогового окна
        /// </summary>
        /// <returns>Возвращает название файла или пустой объект</returns>
        private String SelectFileFromFileDialog()
        {
            // Открываем диалоговое окно для выбора файла
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() != true)
                return null;

            // Возвращаем название файла
            return fileDialog.FileName;
        }

        /// <summary>
        /// Проверяем на существованние файла настроек
        /// </summary>
        /// <param name="fileName">Название файла</param>
        /// <returns>Настройка для поиска лица</returns>
        private CascadeClassifier LoadCascadeClassifier(String fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                return null;

            if (!File.Exists(fileName))
                return null;

            return new CascadeClassifier(fileName);
        }
    }
}
