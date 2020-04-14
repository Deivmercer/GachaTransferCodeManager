using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Xml;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gacha_Transfer_Code_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private double PreviousX, PreviousY;

        public MainWindow()
        {
            InitializeComponent();
            App.InitLogger();
            XmlDocument xmlDocument = new XmlDocument();
            if (File.Exists(App.FilePath + "GTCMan.xml"))
            {
                App.Log.Info("An existing xml has been found. Proceding to parse.");

                xmlDocument.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Gacha Transfer Code Manager\GTCMan.xml");

                PreviousX = AddNewEntryButton.Margin.Left * 2 + AddNewEntryButton.Width;
                PreviousY = AddNewEntryButton.Margin.Top;
                foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes)
                {
                    CreateNewButton(childNode);
                }
                App.Log.Info("Finished loading GTCMan.xml");
            }
            else
            {
                App.Log.Info("No existing GTCMan.xml has been found. Creating it from scratch.");
                if (!Directory.Exists(App.FilePath))
                    Directory.CreateDirectory(App.FilePath);
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", null, null));
                xmlDocument.AppendChild(xmlDocument.CreateElement("games"));
                xmlDocument.Save(App.FilePath + "GTCMan.xml");
                App.Log.Info("The new GTCMan.xml has been successfully created in the following path: " + App.FilePath + "GTCMan.xml.");
            }
        }

        private void CreateNewButton(XmlNode childNode)
        {
            Button button = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(PreviousX, PreviousY, 0, 0),
                Height = AddNewEntryButton.Height,
                Width = AddNewEntryButton.Width
            };
            ImageBrush imageBrush = new ImageBrush();
            if (childNode.SelectSingleNode("icon") != null)
            {
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(childNode.SelectSingleNode("icon").InnerText));
                imageBrush.ImageSource = BitmapFrame.Create(memoryStream);
            }
            else
            {
                imageBrush.ImageSource = BitmapFrame.Create((BitmapImage)Application.Current.Resources["addNewEntryIcon"]);
            }
            button.Background = imageBrush;
            button.Name = "game" + childNode.Attributes.GetNamedItem("id").Value;
            button.Click += new RoutedEventHandler(ShowGameDetailsButton_Click);
            FormGrid.Children.Add(button);
            App.Log.Debug("Created new button \"" + button.Name + "\" with the following margin: [" + button.Margin.Left + "," + button.Margin.Top + "," + button.Margin.Right + "," + button.Margin.Bottom + "]");
            PreviousX += AddNewEntryButton.Margin.Left + AddNewEntryButton.Width;
            if (PreviousX > Width)
            {
                PreviousX = AddNewEntryButton.Margin.Left;
                PreviousY += AddNewEntryButton.Margin.Top * 2 + AddNewEntryButton.Height;
            }
        }

        private void AddNewEntryButton_Click(object sender, RoutedEventArgs e)
        {
            EditEntryForm editEntryForm = new EditEntryForm(0);
            editEntryForm.ShowDialog();
            if (editEntryForm.Node != null)
                CreateNewButton(editEntryForm.Node);
        }

        private void ShowGameDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            EditEntryForm editEntryForm = new EditEntryForm(int.Parse(((Button)sender).Name.Substring(4)));
            editEntryForm.Show();
            if(editEntryForm.Node != null)
            {
                // TODO: Reload name and image
            }
        }
    }
}
