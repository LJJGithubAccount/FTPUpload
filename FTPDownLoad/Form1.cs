using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace FTPDownLoad
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}


		string localPath;
		string fileName;
		bool UploadFile = false;
		private void button1_Click(object sender, EventArgs e)
		{

			if (string.IsNullOrEmpty(localPath) || string.IsNullOrEmpty(fileName))
			{
				MessageBox.Show("路径不对");
				return;
			}

			FileInfo fi = new FileInfo(localPath);
			FTPHelper ftp = new FTPHelper();
			if (UploadFile)//文件
			{
				ftp.UpLoadFile(localPath, "ftp://192.168.0.15/" + fileName);
			}
			else
			{
				ftp.UploadDirectory(localPath, "ftp://192.168.0.15/", fileName);
			}

			localPath = "";
			fileName = "";
			UploadFile = false;
		}

		/// <summary>
		/// 上传文件夹
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button2_Click(object sender, EventArgs e)
		{
			string path = "";
			
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				UploadFile = false;
				path = folderBrowserDialog1.SelectedPath;
			}

			string[] temp = path.Split('\\');
			fileName = temp.Last();
			localPath = path.Replace(fileName, "");
		}

		/// <summary>
		/// 上传文件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button3_Click(object sender, EventArgs e)
		{

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				UploadFile = true;
				localPath = openFileDialog1.FileName;
				fileName = localPath.Split('\\').Last();
			}
		}
	}
}
