using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;

namespace WindowsFormsApp3
{
    class xmlHandler
    {
        List<string> paths;
        Form1 form1;
        public xmlHandler(List<string> paths, Form1 f1)
        {
            this.form1 = f1;
            this.paths = paths;
        }

        public void openPlaylist()
        {
            OpenFileDialog openlist = new OpenFileDialog();
        }

        public void LoadPlaylist()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if(openFile.ShowDialog()==DialogResult.OK)
            {
                form1.listBox1.Items.Clear();
                XDocument xml = XDocument.Load(openFile.FileName);
                var items = xml.Root.Elements("Track").Select(element => element.Value).ToList();
                foreach(string path in items)
                {
                    paths.Add(path);
                    string[] fileName = path.Split('\\');
                    form1.listBox1.Items.Add(fileName[fileName.Count() - 1]);
                }
            }
        }

        public void savePlayList()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = "playlist";
            saveFile.DefaultExt = "xml";
            if (saveFile.ShowDialog()==DialogResult.OK)
            {
                
                XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("TrackList",
                from track in paths
                select new XElement("Track",
                new XElement("Path", track)
                 )
                 )
                 );
                xml.Save(saveFile.FileName);
            }
            
        }
    }
}