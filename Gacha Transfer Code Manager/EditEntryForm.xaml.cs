using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;
using Microsoft.Win32;

namespace Gacha_Transfer_Code_Manager
{
    /// <summary>
    /// Logica di interazione per EditEntryForm.xaml
    /// </summary>
    public partial class EditEntryForm : Window
    {
        public XmlNode Node { get; set; } = null;

        public EditEntryForm(int GameId)
        {
            InitializeComponent();
            if(GameId != 0)
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlNode xmlNode = null;
                xmlDocument.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Gacha Transfer Code Manager\GTCMan.xml");
                for (int i = 0; i < xmlDocument.DocumentElement.ChildNodes.Count; i++)
                    if (int.Parse(xmlDocument.DocumentElement.ChildNodes[i].Attributes.GetNamedItem("id").Value) == GameId)
                        xmlNode = xmlDocument.DocumentElement.ChildNodes[i];
                if(xmlNode != null)
                {
                    GameNameTextBox.Text = xmlNode.SelectSingleNode("name").FirstChild.Value;
                    TransferCodeTextBox.Text = xmlNode.SelectSingleNode("transferCode").FirstChild?.Value;
                    PasswordTextBox.Text = xmlNode.SelectSingleNode("password")?.FirstChild.Value;
                    if(xmlNode.SelectSingleNode("expirationDate")?.ChildNodes.Count > 0) 
                        ExpirationDateCalendar.SelectedDate = DateTime.Parse(xmlNode.SelectSingleNode("expirationDate").FirstChild.Value);
                    if (xmlNode.SelectSingleNode("icon")?.ChildNodes.Count > 0)
                    {
                        MemoryStream memoryStreamm = new MemoryStream(Convert.FromBase64String(xmlNode.SelectSingleNode("icon").FirstChild.Value));
                        BitmapImage bitmapImage = new BitmapImage();
                        JpegBitmapEncoder jpegBitmapEncoderr = new JpegBitmapEncoder();
                        jpegBitmapEncoderr.Frames.Add(BitmapFrame.Create(memoryStreamm));
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = new MemoryStream();
                        jpegBitmapEncoderr.Save(bitmapImage.StreamSource);
                        bitmapImage.StreamSource.Seek(0, SeekOrigin.Begin);
                        bitmapImage.EndInit();
                        IconPreviewImage.Source = bitmapImage;
                    }   
                }
            }
        }

        private void FindIconButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Filter = "Portable Network Graphics|*.png|Joint Photographic Experts Group|*.jpg,*.jpeg,*.jfif,*.JPG,*.JPE|Graphics Interchange Format|*.gif|Windows Bitmap|*.bmp",
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmapImage = new BitmapImage();
                JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(new Uri(openFileDialog.FileName)));
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream();
                jpegBitmapEncoder.Save(bitmapImage.StreamSource);
                bitmapImage.StreamSource.Seek(0, SeekOrigin.Begin);
                bitmapImage.DecodePixelHeight = 75;
                bitmapImage.DecodePixelWidth = 75;
                bitmapImage.EndInit();
                IconPreviewImage.Source = bitmapImage;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Gacha Transfer Code Manager\GTCMan.xml");
            XmlNode newNode = xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("game"));
            XmlAttribute idAttribute = xmlDocument.CreateAttribute("id");
            idAttribute.Value = "" + xmlDocument.DocumentElement.ChildNodes.Count;
            newNode.Attributes.Append(idAttribute);
            newNode.AppendChild(xmlDocument.CreateElement("name")).InnerText = GameNameTextBox.Text;
            newNode.AppendChild(xmlDocument.CreateElement("transferCode")).InnerText = TransferCodeTextBox.Text;
            if (!PasswordTextBox.Text.Equals(""))
                newNode.AppendChild(xmlDocument.CreateElement("password")).InnerText = PasswordTextBox.Text;
            if(ExpirationDateCalendar.SelectedDate != null)
                newNode.AppendChild(xmlDocument.CreateElement("expirationDate")).InnerText = ExpirationDateCalendar.SelectedDate.ToString();
            if (IconPreviewImage.Source != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(IconPreviewImage.Source as BitmapSource));
                jpegBitmapEncoder.Save(memoryStream);
                newNode.AppendChild(xmlDocument.CreateElement("icon")).InnerText = Convert.ToBase64String(memoryStream.ToArray());
            }
            xmlDocument.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Gacha Transfer Code Manager\GTCMan.xml");
            Node = newNode;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

