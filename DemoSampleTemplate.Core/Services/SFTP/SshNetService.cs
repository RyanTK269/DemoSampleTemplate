using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;
using Renci.SshNet;
using System.Threading.Tasks;
using System.Threading;
using Renci.SshNet.Sftp;
using DemoSampleTemplate.Core.DataObjects.Config;
using DemoSampleTemplate.Core.DataObjects.File;

namespace DemoSampleTemplate.Core.Services.SFTP
{
    public class SshNetService
    {
        private static SFTPConfig _config;

        public SshNetService() 
        {
            _config = new SFTPConfig();
        }

        public SshNetService(SFTPConfig config)
        {
            _config = config;
        }

        private async Task<bool> CheckConnection()
        {
            try
            {
                using (var client = new SftpClient(_config.Host, _config.Port, _config.Username, _config.Password))
                {
                    await client.ConnectAsync(CancellationToken.None);
                    return client.IsConnected;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Connect to SFTP server failed", ex);
            }
        }

        private async Task<SftpClient> OpenConnection()
        {
            try
            {
                var client = new SftpClient(_config.Host, _config.Port, _config.Username, _config.Password);
                await client.ConnectAsync(CancellationToken.None);
                return client;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CloseConnection(SftpClient client)
        {
            try
            {
                client.Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception("Disconnect to SFTP server failed", ex);
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task<ISftpFile> GetFile(string path)
        {
            SftpClient client = await OpenConnection();
            try
            {
                client = await OpenConnection();
                var file = client.Get(path);
                return file;
            }
            catch (Exception ex)
            {
                throw new Exception("Get file failed", ex);
            }
            finally
            {
                CloseConnection(client);
            }
        }

        public async Task<IEnumerable<ISftpFile>> GetFiles(string path)
        {
            SftpClient client = await OpenConnection();
            try
            {
                client = await OpenConnection();
                var listFile = client.ListDirectory(path);
                return listFile;
            }
            catch (Exception ex)
            {
                throw new Exception("Get list of file failed", ex);
            }
            finally 
            {
                CloseConnection(client);
            }
        }

        public async Task<string> UploadDocument(Document doc, string path)
        {
            SftpClient client = await OpenConnection();
            try
            {
                string[] split = doc.Filename.Replace("\\", "/").Split('/');
                string fileName = split.Last();
                string sftpfullpath = string.Format("{0}", path);

                client = await OpenConnection();
                var fullFileName = Path.GetFileName(Path.Combine(path, Guid.NewGuid().ToString() + "_" + fileName));
                client.UploadFile(new MemoryStream(doc.Content), fullFileName);
                sftpfullpath = string.Format("{0}/{1}", sftpfullpath, fullFileName);
                return sftpfullpath;
            }
            catch (Exception ex)
            {
                throw new Exception("Upload file failed", ex);
            }
            finally
            {
                CloseConnection(client);
            }
        }
    }
}
