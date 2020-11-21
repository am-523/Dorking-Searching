using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Dorking_ProjectAM523
{
    public partial class Form3 : Form
    {
        public Form3(string url,int engine)
        {
            InitializeComponent();
            this.url = url;
            this.engine = engine;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            base.KeyPreview = true;
            base.KeyUp += this.KeyEvent;
            this.keeprun = true;
            string fileName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", fileName, 11000, RegistryValueKind.DWord);
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Navigate(this.url);
        }

        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            bool flag = e.KeyCode == Keys.Escape || e.KeyCode == Keys.X;
            if (flag)
            {
                MessageBox.Show("Requested!");
                this.keeprun = false;
            }
        }

        private void KeyEvent(object sender, KeyEventArgs e)
        {
            bool flag = e.KeyCode == Keys.Escape || e.KeyCode == Keys.X;
            if (flag)
            {
                this.keeprun = false;
            }
        }


        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string pattern = "<h3\\s+class=\"r\">\\s*<a\\s+href=\"";
            bool flag = Regex.IsMatch(this.webBrowser1.DocumentText, pattern, RegexOptions.IgnoreCase);
            if (flag)
            {
                this.datacontent = this.webBrowser1.DocumentText;
                this.lanjut = true;
                base.Close();
                base.Dispose();
            }
            else
            {
                bool flag2 = Regex.IsMatch(this.webBrowser1.DocumentText, "<div data-jibp=\"h\" data-jiis=\"uc\" id=\"topstuff\">", RegexOptions.IgnoreCase) || Regex.IsMatch(this.webBrowser1.DocumentText, "<p style=\"padding - top:.33em", RegexOptions.IgnoreCase) || Regex.IsMatch(this.webBrowser1.DocumentText, "<div id=\"bres\"></div><div class=\"card-section\"><p id=\"ofr\"><i>", RegexOptions.IgnoreCase) || Regex.IsMatch(this.webBrowser1.DocumentText, "<p style=\"margin-top:1em\">Suggestions:</p>", RegexOptions.IgnoreCase);
                if (flag2)
                {
                    this.lanjut = false;
                    this.datacontent = this.webBrowser1.DocumentText;
                    base.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lanjut = false;
            datacontent = "";
            Close();
        }

        public bool keeprun;

        // Token: 0x0400003D RID: 61
        public bool lanjut;

        // Token: 0x0400003E RID: 62
        private string url;

        // Token: 0x0400003F RID: 63
        private int engine;

        // Token: 0x04000040 RID: 64
        public string datacontent;

    }
}
