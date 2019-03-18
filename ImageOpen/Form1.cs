using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetadataExtractor;


namespace ImageOpen
{
    public partial class Form1 : Form
    {
        OpenFileDialog OFD;
        string GPS_Latitude_Ref = "";
        string GPS_Latitude = "";
        string GPS_Longitude_Ref = "";
        string GPS_Longitude = "";
        string Coordiante = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            OFD = new OpenFileDialog();
            OFD.Filter = "Файлы изображений (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            OFD.InitialDirectory = @"c:\";
            OFD.Title = "Выберите интересующий вас файл изображения";
            
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            if (OFD.ShowDialog() == DialogResult.OK)                             //Открывается окно диалога для выбора изображения
            {
                pictureBox1.Image = (Bitmap)Bitmap.FromFile(OFD.FileName);      //открываем в pictureBox нашего изображения
            }

            var directories = ImageMetadataReader.ReadMetadata(OFD.FileName);
            List<string> str = new List<string>();
            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                {
                    //Console.WriteLine($"[{directory.Name}] {tag.Name} = {tag.Description}");
                    str.Add($"[{directory.Name}] {tag.Name} = {tag.Description}");
                    if(tag.Name == "GPS Latitude Ref")
                        GPS_Latitude_Ref = tag.Description;
                    if (tag.Name == "GPS Latitude")                    
                        GPS_Latitude = tag.Description.Replace(" ", "");  
                    if (tag.Name == "GPS Longitude Ref")
                        GPS_Longitude_Ref = tag.Description;
                    if (tag.Name == "GPS Longitude")
                        GPS_Longitude = tag.Description.Replace(" ", "");
                }                
                    
                if (directory.HasError)
                {
                    foreach (var error in directory.Errors)
                        Console.WriteLine($"ERROR: {error}");
                    
                }
            }
            Coordiante = $"{GPS_Latitude}{GPS_Latitude_Ref} {GPS_Longitude}{GPS_Longitude_Ref}";
            if (Coordiante == " ")
                label1.Text = "В данном изображении нет GPS координат";
            else
                label1.Text = "GPS Координаты:";
            Coordiante = Coordiante.Replace(",",".");
            Console.WriteLine($"Координаты: {Coordiante}");
            GPStextBox.Text = Coordiante;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Clear();                                                   //Перед выводом очистили TextBox
            textBox1.Text = String.Join(Environment.NewLine, str);              //Поместили List<string> в TextBox
            StringBuilder queryaddress = new StringBuilder();
            //http://maps.google.com/maps?q=
            queryaddress.Append("https://www.google.com/maps/place/" + Coordiante);              //https://www.google.com/maps/place/

            webBrowser1.Navigate(queryaddress.ToString());
        }
        //https://www.google.com/maps/place/56%C2%B006'47.2%22N+46%C2%B044'26.0%22E/@56.1131113,46.738353,17z/data=!4m5!3m4!1s0x0:0x0!8m2!3d56.1131083!4d46.7405417
        private void TakeGPS_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image!=null)
            {                
                label2.Text = "Координаты скопированы в буфер обмена";
                System.Windows.Forms.Clipboard.SetText(Coordiante);
            }            
        }
    }
}
