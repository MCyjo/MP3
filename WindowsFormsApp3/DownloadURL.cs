using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace WindowsFormsApp3
{
    class DownloadURL
    {
        WebClient client = new WebClient();
        SaveFileDialog save = new SaveFileDialog();
        string url;

        public DownloadURL(string url)
        {
            this.url = url;
        }


        public void download()
        {
            if (save.ShowDialog()==DialogResult.OK)
            {
                client.DownloadFileAsync(new System.Uri(url)  , save.FileName);
            }
        }
        
    }
}
