using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dorking_ProjectAM523
{
    

    public partial class Form2 : Form
    {
        public List<string> dork = new List<string>();

        public Form2()
        {
            InitializeComponent();
        }

        private void UpdateTotalLines()
        {
            this.lbl_total.Text = "Total Lines : " + Convert.ToString(this.richTextBox1.Lines.Count<string>());
        }

        private void AddData(string path)
        {
            string[] array = File.ReadAllText(path).Split(new string[]
            {
                "\r\n"
            }, StringSplitOptions.None);
            for (int i = 0; i < array.Count<string>(); i++)
            {
                bool flag = array[i] != string.Empty;
                if (flag)
                {
                    RichTextBox richTextBox = this.richTextBox1;
                    richTextBox.Text = richTextBox.Text + array[i] + "\r\n";
                }
            }
        }


        
        private void Form2_Load(object sender, EventArgs e)
        {
            this.UpdateTotalLines();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] array = this.richTextBox1.Text.Split(new string[]
            {
                "\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Count<string>(); i++)
            {
                bool flag = array[i] != string.Empty;
                if (flag)
                {
                    dork.Add(textBox2.Text + array[i] + textBox1.Text);
                }
            }
            base.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text|*.txt|All|*.*";
            bool flag = openFileDialog.ShowDialog() == DialogResult.OK;
            if (flag)
            {
                this.AddData(openFileDialog.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = string.Join(Environment.NewLine, this.richTextBox1.Lines.Distinct<string>());
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateTotalLines();
        }
    }
}
