using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace Dorking_ProjectAM523
{
    public partial class Form1 : Form
    {
		private List<string> datasaved = new List<string>();
		public List<string> dork = new List<string>();
		public Form1()
        {
			InitializeComponent();
		
			closethread = false;
			startvalue = true;
			comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
			comboBox1.DropDownHeight = 1;
			comboBox1.DropDownWidth = 1;
			comboBox1.FormattingEnabled = true;
			comboBox1.IntegralHeight = false;
			checkedListBox1.Visible = false;
			this.search_engine = new bool[this.checkedListBox1.Items.Count];
			for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
			{
				this.search_engine[0] = false;
			}
		}

        private void Form1_Load(object sender, EventArgs e)
        {
			base.KeyPreview = true;
			base.KeyUp += this.KeyEvent;
			this.keeprun = true;
			this.button3.Enabled = false;
			this.AddDork(this.dork);
		}

        private void button3_Click(object sender, EventArgs e)
        {
			this.keeprun = false;
		}

		private string curl(string url_, string data = null, bool responKode = false, string ua = null)
		{
			Uri requestUri = new Uri(url_);
			string result;
			try
			{
				this.req = (HttpWebRequest)WebRequest.Create(requestUri);
				this.req.Accept = "*/*";
				this.req.AllowAutoRedirect = true;
				this.req.Timeout = Convert.ToInt32(this.numericUpDown1.Value) * 1000;
				bool flag = ua != null;
				if (flag)
				{
					this.req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.101 Safari/537.36";
				}
				bool flag2 = !string.IsNullOrEmpty(data);
				if (flag2)
				{
					byte[] bytes = Encoding.UTF8.GetBytes(data);
					this.req.Method = "POST";
					this.req.ContentType = "application/x-www-form-urlencoded";
					this.req.ContentLength = (long)bytes.Length;
					Stream stream = this.req.GetRequestStream();
					stream.Write(bytes, 0, bytes.Length);
					stream.Close();
				}
				else
				{
					this.req.Method = "GET";
				}
				HttpWebResponse httpWebResponse2;
				HttpWebResponse httpWebResponse = httpWebResponse2 = (HttpWebResponse)this.req.GetResponse();
				try
				{
					string str = string.Empty;
					if (responKode)
					{
						str = "_rieqyns13gay_" + httpWebResponse.StatusCode.ToString() + "_rieqyns13homo_";
					}
					Stream responseStream;
					Stream stream = responseStream = httpWebResponse.GetResponseStream();
					try
					{
						StreamReader streamReader = new StreamReader(stream);
						string str2 = WebUtility.HtmlDecode(streamReader.ReadToEnd().ToString());
						stream.Flush();
						stream.Close();
						this.curlFinish = true;
						result = str2 + str;
					}
					finally
					{
						if (responseStream != null)
						{
							((IDisposable)responseStream).Dispose();
						}
					}
				}
				finally
				{
					if (httpWebResponse2 != null)
					{
						((IDisposable)httpWebResponse2).Dispose();
					}
				}
			}
			catch (WebException ex)
			{
				try
				{
					string str3 = string.Empty;
					HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
					if (responKode)
					{
						str3 = "_rieqyns13gay_" + httpWebResponse.StatusCode.ToString() + "_rieqyns13homo_";
					}
					Stream stream = httpWebResponse.GetResponseStream();
					StreamReader streamReader = new StreamReader(stream);
					string str4 = WebUtility.HtmlDecode(streamReader.ReadToEnd().ToString());
					streamReader.Close();
					httpWebResponse.Close();
					this.curlFinish = true;
					result = str4 + str3;
				}
				catch (Exception)
				{
					this.curlFinish = false;
					result = "error";
				}
			}
			catch (Exception ex2)
			{
				this.curlFinish = true;
				result = ex2.ToString();
			}
			return result;
		}


		private void removeClones(ListView.ListViewItemCollection data)
		{
			try
			{
				int count = this.listView1.Items.Count;
				string[] host = new string[count];
				string[] array = new string[count];
				string[] array2 = new string[count];
				string[] array3 = new string[count];
				string[] array4 = new string[count];
				int[] array5 = new int[count];
				for (int i = 0; i < count; i++)
				{
					try
					{
						Uri uri = new Uri(data[i].Text);
						host[i] = uri.Host;
						array[i] = uri.Scheme;
						array2[i] = array[i] + "://" + host[i] + (uri.IsLoopback ? (":" + uri.Port) : string.Empty);
						array3[i] = uri.PathAndQuery;
					}
					catch
					{
						host[i] = null;
						array[i] = null;
						array2[i] = data[i].Text;
						array3[i] = null;
					}
				}
				IEnumerable<int> enumerable = from a in host.Distinct<string>()
											  select Array.IndexOf<string>(host, a);
				int num = 0;
				this.listView1.Items.Clear();
				int jem = 0;
				foreach (int num2 in enumerable)
				{
					array4[num] = array2[num2] + array3[num2];
					this.listView1.Items.Add(array4[num]);
					num++;
					int jem2 = jem;
					jem = jem2 + 1;
				}
				base.Invoke(new MethodInvoker(delegate ()
				{
					this.lbl_total.Text = "Total : " + jem.ToString();
				}));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}


		private void SaveResult(List<string> data)
		{
			string text = "";
			bool flag = data.Count != 0;
			if (flag)
			{
				for (int i = 0; i < data.Count; i++)
				{
					text = text + data[i] + "\r\n";
				}
				using (StreamWriter streamWriter = File.AppendText(this.textBox1.Text))
				{
					streamWriter.WriteLine(text);
				}
			}
		}

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
			bool flag = this.listView1.Items.Count == 0;
			if (flag)
			{
				this.copySelectedItemToolStripMenuItem.Enabled = false;
			}
			else
			{
				this.copySelectedItemToolStripMenuItem.Enabled = true;
			}
			bool flag2 = this.listView1.SelectedItems.Count == 0;
			if (flag2)
			{
				this.copyAllToolStripMenuItem.Enabled = false;
			}
			else
			{
				this.copyAllToolStripMenuItem.Enabled = true;
			}
		}

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				ListView.SelectedListViewItemCollection selectedItems = this.listView1.SelectedItems;
				for (int i = 0; i < selectedItems.Count; i++)
				{
					bool flag = i == selectedItems.Count - 1;
					if (flag)
					{
						stringBuilder.Append(selectedItems[i].Text);
					}
					else
					{
						stringBuilder.AppendLine(selectedItems[i].Text);
					}
				}
				Clipboard.SetText(stringBuilder.ToString());
			}
			catch
			{
			}
		}

        private void importFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "txt files (*.txt)|*.txt";
				openFileDialog.Multiselect = false;
				bool flag = openFileDialog.ShowDialog() == DialogResult.OK;
				if (flag)
				{
					this.listView1.Items.Clear();
					string[] array = File.ReadAllLines(openFileDialog.FileName);
					foreach (string text in array)
					{
						this.listView1.Items.Add(text.Trim());
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

        private void copySelectedItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
			bool flag = this.listView1.Items.Count > 0;
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.listView1.Items.Count; i++)
				{
					stringBuilder.AppendLine(this.listView1.Items[i].Text);
				}
				Clipboard.SetText(stringBuilder.ToString());
			}
		}

		private void UpdatelistView(ListView lst, string[] items)
		{
			bool invokeRequired = lst.InvokeRequired;
			if (invokeRequired)
			{
				Form1.updatelist method = new Form1.updatelist(this.updategay);
				lst.Invoke(method, new object[]
				{
					lst,
					items
				});
			}
			else
			{
				this.updategay(lst, items);
			}
		}

		private void updategay(ListView lst, string[] items)
		{
			ListViewItem value = new ListViewItem(items);
			lst.Items.Add(value);
		}

        private void button1_Click(object sender, EventArgs e)
        {
			
		}

		private void AddDork(List<string> dork)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Dork");
			dt.Columns.Add("Catched");
			for (int i = 0; i < dork.Count<string>(); i++)
			{
				dt.Rows.Add(new object[]
				{
					dork[i],
					""
				});
			}
			bool flag = dork.Count<string>() < 20;
			if (flag)
			{
				for (int j = 0; j < 20 - dork.Count<string>(); j++)
				{
					dt.Rows.Add(new object[]
					{
						"",
						""
					});
				}
			}
			base.BeginInvoke(new MethodInvoker(delegate ()
			{
				this.dataGridView1.DataSource = null;
				this.dataGridView1.DataSource = dt;
				this.dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				this.dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
				this.dataGridView1.RowHeadersVisible = false;
				this.dataGridView1.Columns[0].Width = 320;
				this.dataGridView1.Columns[1].Width = 100;
				this.dataGridView1.Columns[0].Resizable = DataGridViewTriState.False;
				this.dataGridView1.Columns[1].Resizable = DataGridViewTriState.False;
			}));
		}

		private void BingWork(List<string> dork)
		{
			int i;
			int num2;
			for (i = 0; i < dork.Count; i = num2 + 1)
			{
				int num = 0;
				int catched = 0;
				this.UpdateStatus("Bing on work, dork : " + dork[i]);
				this.lanjut = true;
				base.Invoke(new MethodInvoker(delegate ()
				{
					bool flag3 = this.dataGridView1[1, i].Value != "";
					if (flag3)
					{
						catched = Convert.ToInt32(this.dataGridView1[1, i].Value);
					}
				}));

				while (this.lanjut)
				{
					string input = this.curl("http://www.bing.com/search?q=" + HttpUtility.UrlEncode(dork[i]) + "&first=" + Convert.ToString(num * 10 + 1), null, false, null);
					string pattern = "<li class=\"b_algo\"><h2><a href=\"";
					bool flag = Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
					if (flag)
					{
						MatchCollection matchCollection = Regex.Matches(input, "<li class=\"b_algo\"><h2><a href=\"(.*?)\" h=\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
						for (int j = 0; j < matchCollection.Count; j++)
						{
							num2 = catched;
							catched = num2 + 1;
							Match match = matchCollection[j];
							string text = match.Groups[1].Value.ToString();
							this.UpdatelistView(this.listView1, new string[]
							{
								text
							});
							this.datasaved.Add(text);
						}
						this.SaveResult(this.datasaved);
						this.datasaved.Clear();
						bool flag2 = !this.keeprun;
						if (flag2)
						{
							throw new Exception("Anisa!");
						}

						base.Invoke(new MethodInvoker(delegate ()
						{
							this.label3.Text = "Total : " + Convert.ToString(this.listView1.Items.Count);
							this.dataGridView1[1, i].Value = Convert.ToString(catched);
						}));
					}
					else
					{
						this.lanjut = false;
					}
					num++;
				}
				num2 = i;
			}
		}

		private void AskWork(List<string> dork)
		{
			int i;
			int num2;
			for (i = 0; i < dork.Count; i = num2 + 1)
			{
				this.UpdateStatus("Ask on work, dork : " + dork[i]);
				int num = 1;
				int catched = 0;
				this.lanjut = true;
				base.Invoke(new MethodInvoker(delegate ()
				{
					bool flag3 = this.dataGridView1[1, i].Value != "";
					if (flag3)
					{
						catched = Convert.ToInt32(this.dataGridView1[1, i].Value);
					}
				}));
				while (this.lanjut)
				{
					string input = this.curl("http://www.ask.com/web?q=" + HttpUtility.UrlEncode(dork[i]) + "&page=" + Convert.ToString(num), null, false, null);
					string pattern = "target=\"_blank\" href='";
					bool flag = Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
					if (flag)
					{
						MatchCollection matchCollection = Regex.Matches(input, "target=\"_blank\" href='(.*?)'", RegexOptions.IgnoreCase | RegexOptions.Singleline);
						for (int j = 0; j < matchCollection.Count; j++)
						{
							num2 = catched;
							catched = num2 + 1;
							Match match = matchCollection[j];
							string text = match.Groups[1].Value.ToString();
							this.UpdatelistView(this.listView1, new string[]
							{
								text
							});
							this.datasaved.Add(text);
						}
						this.SaveResult(this.datasaved);
						this.datasaved.Clear();
						bool flag2 = !this.keeprun;
						if (flag2)
						{
							throw new Exception("Anisa!");
						}
						base.Invoke(new MethodInvoker(delegate ()
						{
							this.label3.Text = "Total : " + Convert.ToString(this.listView1.Items.Count);
							this.dataGridView1[1, i].Value = Convert.ToString(catched);
						}));
					}
					else
					{
						this.lanjut = false;
					}
					num++;
				}
				num2 = i;
			}
		}

		private void YahooWork(List<string> dork)
		{
			int i;
			int num2;
			for (i = 0; i < dork.Count; i = num2 + 1)
			{
				this.UpdateStatus("Yahoo on work, dork : " + dork[i]);
				int num = 0;
				int catched = 0;
				this.lanjut = true;
				base.Invoke(new MethodInvoker(delegate ()
				{
					bool flag3 = this.dataGridView1[1, i].Value != "";
					if (flag3)
					{
						catched = Convert.ToInt32(this.dataGridView1[1, i].Value);
					}
				}));
				while (this.lanjut)
				{
					string input = this.curl("https://search.yahoo.com/search;?p=" + HttpUtility.UrlEncode(dork[i]) + "&b=" + Convert.ToString(num * 10 + 1), null, false, null);
					string pattern = "<h3\\s+class=\"title\"><a\\s+class=\"\\s+";
					bool flag = Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
					if (flag)
					{
						MatchCollection matchCollection = Regex.Matches(input, "<h3\\s+class=\"title\"><a\\s+class=\"\\s+(.*?)\"\\s+href=\"(.*?)\"\\s+target=\"_blank", RegexOptions.IgnoreCase | RegexOptions.Singleline);
						for (int j = 0; j < matchCollection.Count; j++)
						{
							num2 = catched;
							catched = num2 + 1;
							Match match = matchCollection[j];
							string text = match.Groups[2].Value.ToString();
							this.UpdatelistView(this.listView1, new string[]
							{
								text
							});
							this.datasaved.Add(text);
						}
						this.SaveResult(this.datasaved);
						this.datasaved.Clear();
						bool flag2 = !this.keeprun;
						if (flag2)
						{
							throw new Exception("Anisa!");
						}
						base.Invoke(new MethodInvoker(delegate ()
						{
							this.label3.Text = "Total : " + Convert.ToString(this.listView1.Items.Count);
							this.dataGridView1[1, i].Value = Convert.ToString(catched);
						}));
					}
					else
					{
						this.lanjut = false;
					}
					num++;
				}
				num2 = i;
			}
		}

		private void GoogleWork(List<string> dork)
		{
			int i;
			int num2;
			for (i = 0; i < dork.Count; i = num2 + 1)
			{
				this.UpdateStatus("Google on work, dork : " + dork[i]);
				this.lanjut = true;
				int catched = 0;
				base.Invoke(new MethodInvoker(delegate ()
				{
					bool flag2 = this.dataGridView1[1, i].Value != "";
					if (flag2)
					{
						catched = Convert.ToInt32(this.dataGridView1[1, i].Value);
					}
				}));
				int num = 0;
				while (this.lanjut)
				{
					string url = "https://www.google.com/search?q=" + HttpUtility.UrlEncode(dork[i]) + "&start=" + Convert.ToString(num * 10);
					Form3 form = new Form3(url, this.engine);
					form.ShowDialog();
					this.datacontent = form.datacontent;
					this.lanjut = form.lanjut;
					this.keeprun = form.keeprun;
					form.Dispose();
					bool flag = !this.keeprun;
					if (flag)
					{
						throw new Exception("Pencarian Berhenti");
					}
					MatchCollection matchCollection = Regex.Matches(this.datacontent, "<h3\\s+class=\"r\">\\s*<a\\s+href=\"(.*?)\" onmousedown=\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
					for (int j = 0; j < matchCollection.Count; j++)
					{
						num2 = catched;
						catched = num2 + 1;
						Match match = matchCollection[j];
						string text = match.Groups[1].Value.ToString();
						this.UpdatelistView(this.listView1, new string[]
						{
							text
						});
						this.datasaved.Add(text);
					}
					this.SaveResult(this.datasaved);
					this.datasaved.Clear();
					base.Invoke(new MethodInvoker(delegate ()
					{
						this.label3.Text = "Total : " + Convert.ToString(this.listView1.Items.Count);
						this.dataGridView1[1, i].Value = Convert.ToString(catched);
					}));
					num++;
				}
				num2 = i;
			}
		}

		private void YandexWork(List<string> dork)
		{
			int i;
			int num2;
			for (i = 0; i < dork.Count; i = num2 + 1)
			{
				int num = 1;
				int catched = 0;
				this.UpdateStatus("Yandex on work, dork : " + dork[i]);
				this.lanjut = true;
				base.Invoke(new MethodInvoker(delegate ()
				{
					bool flag3 = this.dataGridView1[1, i].Value != "";
					if (flag3)
					{
						catched = Convert.ToInt32(this.dataGridView1[1, i].Value);
					}
				}));
				while (this.lanjut)
				{
					string input = this.curl("https://www.yandex.com/search/?text=" + HttpUtility.UrlEncode(dork[i]) + "&lr=11513&p=" + Convert.ToString(num), null, false, null);
					string pattern = "</span><a class=\"link link_outer_yes link_theme_outer path__item i-bem\" data-bem='{\"link\":{\"_tabindex\":\"0\",\"origTabindex\":\"0\"}}' target=\"_blank\" tabindex=\"-1\" href=\"";
					bool flag = Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
					if (flag)
					{
						MatchCollection matchCollection = Regex.Matches(input, "</span><a class=\"link link_outer_yes link_theme_outer path__item i-bem\" data-bem='{\"link\":{\"_tabindex\":\"0\",\"origTabindex\":\"0\"}}' target=\"_blank\" tabindex=\"-1\" href=\"(.*?)\" rel=\"noopener\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
						for (int j = 0; j < matchCollection.Count; j++)
						{
							num2 = catched;
							catched = num2 + 1;
							Match match = matchCollection[j];
							string text = match.Groups[1].Value.ToString();
							this.UpdatelistView(this.listView1, new string[]
							{
								text
							});
							this.datasaved.Add(text);
						}
						this.SaveResult(this.datasaved);
						this.datasaved.Clear();
						bool flag2 = !this.keeprun;
						if (flag2)
						{
							throw new Exception("Anisa!");
						}
						base.Invoke(new MethodInvoker(delegate ()
						{
							this.label3.Text = "Total : " + Convert.ToString(this.listView1.Items.Count);
							this.dataGridView1[1, i].Value = Convert.ToString(catched);
						}));
					}
					else
					{
						this.lanjut = false;
					}
					num++;
				}
				num2 = i;
			}
		}

		private void cari()
		{
			bool flag = !this.closethread;
			if (flag)
			{
				base.Dispose();
			}
			try
			{
				this.keeprun = true;
				this.total = 0;
				base.Invoke(new MethodInvoker(delegate ()
				{
					this.button3.Enabled = true;
					this.button2.Enabled = false;
				}));
				bool flag2 = this.search_engine[0];
				if (flag2)
				{
					this.GoogleWork(this.dork);
				}
				bool flag3 = this.search_engine[1];
				if (flag3)
				{
					this.BingWork(this.dork);
				}
				bool flag4 = this.search_engine[2];
				if (flag4)
				{
					this.YahooWork(this.dork);
				}
				bool flag5 = this.search_engine[3];
				if (flag5)
				{
					this.YandexWork(this.dork);
				}
				bool flag6 = this.search_engine[4];
				if (flag6)
				{
					this.AskWork(this.dork);
				}
				MessageBox.Show("Done!");
				base.Invoke(new MethodInvoker(delegate ()
				{
					this.button3.Enabled = false;
					this.button2.Enabled = true;
				}));
			}
			catch (Exception ex)
			{
				base.Invoke(new MethodInvoker(delegate ()
				{
					this.button2.Enabled = true;
					this.button3.Enabled = false;
				}));
				MessageBox.Show("Pencarian Berhenti");
			}
			this.UpdateStatus("Ready");
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
			this.AddDork(this.dork);
		}

		private void KeyEvent(object sender, KeyEventArgs e)
		{
			bool flag = e.KeyCode == Keys.Escape || e.KeyCode == Keys.X;
			if (flag)
			{
				this.keeprun = false;
			}
		}

        private void button5_Click(object sender, EventArgs e)
        {
			
		}

		private void UpdateStatus(string text)
		{
			base.BeginInvoke(new MethodInvoker(delegate ()
			{
				this.lbl_status.Text = "Status : " + text;
			}));
		}

        private void button2_Click(object sender, EventArgs e)
        {
			
		}

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
			bool flag = this.comboBox1.SelectedIndex == 0;
			if (flag)
			{
				this.engine = 0;
			}
			else
			{
				bool flag2 = this.comboBox1.SelectedIndex == 1;
				if (flag2)
				{
					this.engine = 1;
				}
				else
				{
					bool flag3 = this.comboBox1.SelectedIndex == 2;
					if (flag3)
					{
						this.engine = 2;
					}
					else
					{
						bool flag4 = this.comboBox1.SelectedIndex == 3;
						if (flag4)
						{
							this.engine = 3;
						}
						else
						{
							bool flag5 = this.comboBox1.SelectedIndex == 4;
							if (flag5)
							{
								this.engine = 4;
							}
							else
							{
								bool flag6 = this.comboBox1.SelectedIndex == 5;
								if (flag6)
								{
									this.engine = 5;
								}
								else
								{
									this.engine = 0;
								}
							}
						}
					}
				}
			}
		}

        private void button4_Click(object sender, EventArgs e)
        {
			this.removeClones(this.listView1.Items);
		}

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
			bool flag = e.KeyCode == Keys.Escape || e.KeyCode == Keys.X;
			if (flag)
			{
				this.keeprun = false;
			}
		}

        private void button6_Click(object sender, EventArgs e)
        {
			
		}

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
			this.checkedListBox1.Visible = true;
		}

        //protected override void Dispose(bool disposing)
        //{
        //    bool flag = disposing && this.components != null;
        //    if (flag)
        //    {
        //        this.components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}


        // Token: 0x04000001 RID: 1
        private bool closethread;

		// Token: 0x04000002 RID: 2
		private string engine_selected;

		// Token: 0x04000003 RID: 3
		private bool[] search_engine;

		// Token: 0x04000004 RID: 4
		private HttpWebRequest req = null;

		// Token: 0x04000005 RID: 5
		private XmlDocument xmldoc;

		// Token: 0x04000006 RID: 6
		private bool keeprun;

		// Token: 0x04000007 RID: 7
		private bool proxy = false;

		// Token: 0x04000008 RID: 8
		private bool curlFinish = false;

		// Token: 0x04000009 RID: 9
		

		// Token: 0x0400000A RID: 10
		private int total;

		// Token: 0x0400000B RID: 11
		private bool startvalue;

		// Token: 0x0400000C RID: 12
		public string datacontent;

		// Token: 0x0400000D RID: 13
		private int engine;

		// Token: 0x0400000E RID: 14
		public bool lanjut;

		// Token: 0x0400000F RID: 15
		
		private delegate void updatelist(ListView lst, string[] items);

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
			Form2 form = new Form2();
			form.dork = this.dork;
			form.ShowDialog();
			this.dork = form.dork;
			this.lbl_dork.Text = "Dork : " + Convert.ToString(this.dork.Count);
			bool flag = this.dork != null;
			if (flag)
			{
				this.backgroundWorker1.RunWorkerAsync();
			}
			form.Dispose();
		}

        private void guna2Button2_Click(object sender, EventArgs e)
        {
			this.dork.Clear();
			this.AddDork(this.dork);
			this.lbl_dork.Text = "Dork : " + Convert.ToString(this.dork.Count);
		}

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
			this.engine_selected = "";
			this.checkedListBox1.Visible = false;
			for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
			{
				bool flag = this.checkedListBox1.GetItemCheckState(i) == CheckState.Checked;
				if (flag)
				{
					this.search_engine[i] = true;
					this.engine_selected = this.engine_selected + this.checkedListBox1.Items[i] + "; ";
				}
				else
				{
					this.search_engine[i] = false;
				}
			}
			bool flag2 = this.engine_selected == "";
			if (flag2)
			{
				this.engine_selected = "Please Select";
			}
			this.comboBox1.Text = this.engine_selected;
		}

        private void guna2Button3_Click(object sender, EventArgs e)
        {
			bool flag = !this.closethread;
			if (flag)
			{
				base.Dispose();
			}
			bool flag2 = this.dork.Count == 0;
			if (flag2)
			{
				MessageBox.Show("Dork cannot be empty!");
			}
			else
			{
				Thread thread = new Thread(
					new ThreadStart(this.cari)
					);
				thread.Priority = ThreadPriority.AboveNormal;
				thread.IsBackground = true;
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}
		}

        private void guna2Button5_Click(object sender, EventArgs e)
        {
			this.keeprun = false;
		}

        private void guna2Button8_Click(object sender, EventArgs e)
        {
			DialogResult dialogResult = MessageBox.Show("Clear all items?", "Sure?", MessageBoxButtons.YesNo);
			bool flag = dialogResult == DialogResult.Yes;
			if (flag)
			{
				this.listView1.Items.Clear();
				this.lbl_total.Text = "Total : " + Convert.ToString(this.listView1.Items.Count);
				for (int i = 0; i < this.dork.Count; i++)
				{
					this.dataGridView1[1, i].Value = "";
				}
			}
		}

        private void guna2Button7_Click(object sender, EventArgs e)
        {
			removeClones(listView1.Items);
		}
    }
}
