using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Drawing.Imaging;

namespace UCC_converter_Tools
{
    public partial class Form1 : Form
    {
        string ActualFileSD;
        Bitmap FrontImg;
        Bitmap BackImg;
        Bitmap IconImg;
        bool isRChar = false;
        bool AutoTransparancy = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "SelfDesign File (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ActualFileSD = System.IO.Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                    ExtractZipFile(openFileDialog1.FileName, "" , "C:\\Windows\\Temp\\davedevils\\" + ActualFileSD);
                    ReadPangyaPicture("front");
                    ReadPangyaPicture("back");
                    //ReadPangyaPicture("icon");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        public void ReadPangyaPicture(string filename)
        {

            int width = 128;
            int height = 128;

            int xx = 0;
            int yy = 0;
            int[] hexacolor;
            int[] transcolor;
            Color myRgbColor = new Color();

            FileStream fs = new FileStream("C:\\Windows\\Temp\\davedevils\\" + ActualFileSD + "\\" + filename, FileMode.Open);

            Bitmap flag = new Bitmap(128, 128);

            // SD Standard = 49152/65536 BYTE
            // SD 256x256  = 196608 BYTE
            string size = fs.Length.ToString();
            MessageBox.Show(size);

            if (fs.Length > 65536 || fs.Length == 24576)
            {
                flag = new Bitmap(256, 256);
                width = 256;
                height = 256;
            }
            //short 
            //128x64
            if (fs.Length < 30000)
            {
                flag = new Bitmap(128, 64);
                width = 128;
                height = 64;
            }

            transcolor = new int[4];
            for (int i = 0; yy < height- 1 ; i += 8)
            {
                int zz = 0;
                hexacolor = new int[4];
                if (isRChar == true)
                {
                    while (zz < 3)
                    {
                        int z = fs.ReadByte();
                        string s = z.ToString("X");
                        if (s.Length < 2)
                            s = "0" + s;
                        hexacolor[zz] = int.Parse(s, System.Globalization.NumberStyles.HexNumber);
                        zz++;

                        if (hexacolor[zz] < 0)
                            hexacolor[zz] = 0;
                    }

                    if (AutoTransparancy == true && i == 0)
                    {
                        transcolor = hexacolor;

                        myRgbColor = Color.FromArgb(0, hexacolor[2], hexacolor[1], hexacolor[0]);
                    }
                    else if (AutoTransparancy == true)
                    {
                        if (transcolor[0] == hexacolor[0] 
                         && transcolor[1] == hexacolor[1]
                         && transcolor[2] == hexacolor[2])
                            myRgbColor = Color.FromArgb(0, hexacolor[2], hexacolor[1], hexacolor[0]);
                        else
                            myRgbColor = Color.FromArgb(255,hexacolor[2], hexacolor[1], hexacolor[0]);
                    }
                    else
                    {
                        myRgbColor = Color.FromArgb(255,hexacolor[2], hexacolor[1], hexacolor[0]);
                    }
                }
                else
                {
                    while (zz < 4)
                    {
                        int z = fs.ReadByte();
                        string s = z.ToString("X");
                        if (s.Length < 2)
                            s = "0" + s;
                        hexacolor[zz] = int.Parse(s, System.Globalization.NumberStyles.HexNumber);

                        if (hexacolor[zz] < 0)
                            hexacolor[zz] = 0;

                        zz++;

                    }
                    myRgbColor = Color.FromArgb(hexacolor[3], hexacolor[2], hexacolor[1], hexacolor[0]);
                }

                flag.SetPixel(xx, yy, myRgbColor);

                  if (xx == width-1)
                  {
	                yy++;
	                xx = 0;
                  }
                  else
                  {
	                xx++;
                  }
            }

            Graphics flagGraphics = Graphics.FromImage(flag);

            if (filename == "front")
            {
                picfront.Image = flag;
                FrontImg = flag;
            }
            else if (filename == "back")
            {
                picback.Image = flag;
                BackImg = flag;
            }
            else if (filename == "icon")
            {
                //picicon.Image = flag;
                IconImg = flag;
            }
                fs.Close();
        }

        public void SavePangyaPicture(string filename , Bitmap Img)
        {

            int width = Img.Width;
            int height = Img.Height;

            int xx = 0;
            int yy = 0;
            FileStream fs = new FileStream("C:\\Windows\\Temp\\davedevils\\" + ActualFileSD + "\\" + filename, FileMode.Create, FileAccess.Write);

            for (int i = 0; yy < height - 1; i += 8)
            {
                if (isRChar == true)
                {
                    //Color.FromArgb(hexacolor[2], hexacolor[1], hexacolor[0]);
                    // Blue
                    // Green
                    // Red
                    Color pixelColor = Img.GetPixel(xx, yy);
                    fs.WriteByte(pixelColor.B);
                    fs.WriteByte(pixelColor.G);
                    fs.WriteByte(pixelColor.R);

                }
                else
                {
                    //Color.FromArgb(hexacolor[3], hexacolor[2], hexacolor[1], hexacolor[0]);
                    // Blue
                    // Green
                    // Red
                    // Alpha
                    Color pixelColor = Img.GetPixel(xx, yy);
                    fs.WriteByte(pixelColor.B);
                    fs.WriteByte(pixelColor.G);
                    fs.WriteByte(pixelColor.R);
                    fs.WriteByte(pixelColor.A);
                }

                if (xx == width - 1)
                {
                    yy++;
                    xx = 0;
                }
                else
                {
                    xx++;
                }
            }

            fs.Close();
        }
        public void ExtractZipFile(string archiveFilenameIn, string password, string outFolder)
        {
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);
                if (!String.IsNullOrEmpty(password))
                {
                    zf.Password = password;     // AES encrypted entries are handled automatically
                }
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            SavePangyaPicture("front" , FrontImg);
            SavePangyaPicture("back" , BackImg);
            ICSharpCode.SharpZipLib.Zip.FastZip z = new ICSharpCode.SharpZipLib.Zip.FastZip();
            z.CreateEmptyDirectories = false;
            z.CreateZip( ActualFileSD + ".jpg", "C:\\Windows\\Temp\\davedevils\\" + ActualFileSD, true, "");

            if (File.Exists(ActualFileSD + ".jpg"))
                MessageBox.Show("Saved at " + ActualFileSD + ".jpg");
            else
                MessageBox.Show("Save has fail :(");

        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //export
            bool exists = System.IO.Directory.Exists(ActualFileSD);

            if (!exists)
                System.IO.Directory.CreateDirectory(ActualFileSD);

            Bitmap frontsave = new Bitmap(FrontImg);
            Bitmap backsave = new Bitmap(BackImg);
            frontsave.Save(ActualFileSD + "\\" + "front.png", ImageFormat.Png);
            backsave.Save(ActualFileSD + "\\" + "back.png", ImageFormat.Png);


            frontsave.Dispose();
            backsave.Dispose();

            MessageBox.Show("Saved in folder :" + ActualFileSD);
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ActualFileSD + "front.png") == false 
                || File.Exists(ActualFileSD + "back.png") == false)
            {
                MessageBox.Show("You need import before export ... The Folder of import ->  :" + ActualFileSD);
            }
            else
            {
                //import
                Image Front = Image.FromFile(ActualFileSD + "\\" + "front.png");
                Image Back = Image.FromFile(ActualFileSD + "\\" + "back.png");

                Bitmap frontload = new Bitmap(Front);
                Bitmap backload = new Bitmap(Back);

                picfront.Image = frontload;
                FrontImg = new Bitmap(picfront.Image);
                picback.Image = backload;
                BackImg = new Bitmap(picback.Image);

                Back.Dispose();
                Front.Dispose();
                MessageBox.Show("File have been imported");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
                 isRChar = true;
            else
                isRChar = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                AutoTransparancy = true;
            else
                AutoTransparancy = false;
        }
    }
}
