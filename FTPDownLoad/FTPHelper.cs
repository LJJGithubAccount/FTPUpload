using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTPDownLoad
{
	class FTPHelper
	{
		private string ftpUserID = "784514076@qq.com";//用户名  
		private string ftpPassword = "junjie19900320";//密码  

		/// <summary>  
		/// 上传文件  
		/// </summary>  
		/// <param name="localFile">要上传到FTP服务器的本地文件</param>  
		/// <param name="ftpPath">FTP地址</param>  
		public void UpLoadFile(string localFile, string ftpPath)
		{
			if (!File.Exists(localFile))
			{
				MessageBox.Show("文件：“" + localFile + "” 不存在！");
				return;
			}
			FileInfo fileInf = new FileInfo(localFile);
			FtpWebRequest reqFTP;

			reqFTP = (FtpWebRequest)FtpWebRequest.Create(ftpPath);// 根据uri创建FtpWebRequest对象   
			reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);// ftp用户名和密码  
			reqFTP.KeepAlive = false;// 默认为true，连接不会被关闭 // 在一个命令之后被执行  
			reqFTP.Method = WebRequestMethods.Ftp.UploadFile;// 指定执行什么命令  
			reqFTP.UseBinary = true;// 指定数据传输类型  
			reqFTP.ContentLength = fileInf.Length;// 上传文件时通知服务器文件的大小  
			int buffLength = 2048;// 缓冲大小设置为2kb  
			byte[] buff = new byte[buffLength];
			int contentLen;

			// 打开一个文件流 (System.IO.FileStream) 去读上传的文件  
			FileStream fs = fileInf.OpenRead();
			try
			{
				Stream strm = reqFTP.GetRequestStream();// 把上传的文件写入流  
				contentLen = fs.Read(buff, 0, buffLength);// 每次读文件流的2kb  

				while (contentLen != 0)// 流内容没有结束  
				{
					// 把内容从file stream 写入 upload stream  
					strm.Write(buff, 0, contentLen);
					contentLen = fs.Read(buff, 0, buffLength);
				}
				// 关闭两个流  
				strm.Close();
				fs.Close();
				MessageBox.Show("文件【" + ftpPath + "/" + fileInf.Name + "】上传成功！<br/>");
			}
			catch (Exception ex)
			{
				MessageBox.Show("上传文件【" + ftpPath + "/" + fileInf.Name + "】时，发生错误：" + ex.Message + "<br/>");
			}
		}



		#region 上传文件夹  

		/// <summary>  
		/// 上传整个目录  
		/// </summary>  
		/// <param name="localDir">要上传的目录的上一级目录</param>  
		/// <param name="ftpPath">FTP路径</param>  
		/// <param name="dirName">要上传的目录名</param>  
		/// <param name="ftpUser">FTP用户名（匿名为空）</param>  
		/// <param name="ftpPassword">FTP登录密码（匿名为空）</param>  
		public void UploadDirectory(string localDir, string ftpPath, string dirName)
		{
			string dir = localDir + dirName + @"\"; //获取当前目录（父目录在目录名）  
													//检测本地目录是否存在  
			if (!Directory.Exists(dir))
			{
				MessageBox.Show("本地目录：“" + dir + "” 不存在！<br/>");
				return;
			}
			//检测FTP的目录路径是否存在  
			if (!CheckDirectoryExist(ftpPath, dirName))
			{
				MakeDir(ftpPath, dirName);//不存在，则创建此文件夹  
			}
			List<List<string>> infos = GetDirDetails(dir); //获取当前目录下的所有文件和文件夹  

			//先上传文件  
			//Response.Write(dir + "下的文件数：" + infos[0].Count.ToString() + "<br/>");  
			for (int i = 0; i < infos[0].Count; i++)
			{
				Console.WriteLine(infos[0][i]);
				UpLoadFile(dir + infos[0][i], ftpPath + dirName + @"/" + infos[0][i]);
			}
			//再处理文件夹  
			//Response.Write(dir + "下的目录数：" + infos[1].Count.ToString() + "<br/>");  
			for (int i = 0; i < infos[1].Count; i++)
			{
				UploadDirectory(dir, ftpPath + dirName + @"/", infos[1][i]);
				//Response.Write("文件夹【" + dirName + "】上传成功！<br/>");  
			}
		}

		/// <summary>  
		/// 判断ftp服务器上该目录是否存在  
		/// </summary>  
		/// <param name="ftpPath">FTP路径目录</param>  
		/// <param name="dirName">目录上的文件夹名称</param>  
		/// <returns></returns>  
		private bool CheckDirectoryExist(string ftpPath, string dirName)
		{
			bool flag = true;
			try
			{
				//实例化FTP连接  
				FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftpPath + dirName);
				ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
				ftp.Method = WebRequestMethods.Ftp.ListDirectory;
				FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
				response.Close();
			}
			catch (Exception)
			{
				flag = false;
			}
			return flag;
		}

		/// <summary>  
		/// 创建文件夹    
		/// </summary>    
		/// <param name="ftpPath">FTP路径</param>    
		/// <param name="dirName">创建文件夹名称</param>    
		public void MakeDir(string ftpPath, string dirName)
		{

			FtpWebRequest reqFTP;
			try
			{
				string ui = (ftpPath + dirName).Trim();
				reqFTP = (FtpWebRequest)FtpWebRequest.Create(ui);
				reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
				reqFTP.UseBinary = true;
				reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
				FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
				Stream ftpStream = response.GetResponseStream();
				ftpStream.Close();
				response.Close();
				MessageBox.Show("文件夹【" + dirName + "】创建成功！<br/>");
			}

			catch (Exception ex)
			{
				MessageBox.Show("新建文件夹【" + dirName + "】时，发生错误：" + ex.Message);
			}

		}

		/// <summary>  
		/// 获取目录下的详细信息  
		/// </summary>  
		/// <param name="localDir">本机目录</param>  
		/// <returns></returns>  
		private List<List<string>> GetDirDetails(string localDir)
		{
			List<List<string>> infos = new List<List<string>>();
			try
			{
				infos.Add(Directory.GetFiles(localDir).ToList()); //获取当前目录的文件  

				infos.Add(Directory.GetDirectories(localDir).ToList()); //获取当前目录的目录  

				for (int i = 0; i < infos[0].Count; i++)
				{
					int index = infos[0][i].LastIndexOf(@"\");
					infos[0][i] = infos[0][i].Substring(index + 1);
				}
				for (int i = 0; i < infos[1].Count; i++)
				{
					int index = infos[1][i].LastIndexOf(@"\");
					infos[1][i] = infos[1][i].Substring(index + 1);
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
			return infos;
		}

		#endregion
	}
}
