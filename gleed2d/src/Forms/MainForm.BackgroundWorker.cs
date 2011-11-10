using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using GLEED2D.Properties;
using System.Threading;

namespace GLEED2D
{
    public partial class MainForm : Form
    {

        public void loadfolder_background(string path, TabPage currPage)
        {
            if (backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync();
            while (backgroundWorker1.IsBusy)
            {
                Application.DoEvents();
                Thread.Sleep(50);
            }
            Image img = null;
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] folders = di.GetDirectories();
            string filters = "*.jpg;*.png;*.bmp;";
            List<FileInfo> fileList = new List<FileInfo>();
            string[] extensions = filters.Split(';');
            foreach (string filter in extensions) fileList.AddRange(di.GetFiles(filter));
            FileInfo[] files = fileList.ToArray();
            switch (currPage.Name)
            {
                case ITEM_TAB_PAGE:
                    img = Resources.folder;
                    textureImageList48.Images.Clear();
                    textureImageList64.Images.Clear();
                    textureImageList96.Images.Clear();
                    textureImageList128.Images.Clear();
                    textureImageList256.Images.Clear();

                    textureImageList48.Images.Add(img);
                    textureImageList64.Images.Add(img);
                    textureImageList96.Images.Add(img);
                    textureImageList128.Images.Add(img);
                    textureImageList256.Images.Add(img);
                    itemsLV.Clear();
                    itemDirBox.Text = di.FullName;
                    foreach (DirectoryInfo folder in folders)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = folder.Name;
                        lvi.ToolTipText = folder.Name;
                        lvi.ImageIndex = 0;
                        lvi.Tag = "folder";
                        lvi.Name = folder.FullName;
                        itemsLV.Items.Add(lvi);
                    }
                    
                    foreach (FileInfo file in files)
                    {
                        Bitmap bmp = new Bitmap(file.FullName);
                        
                        textureImageList48.Images.Add(file.FullName, getThumbNail(bmp, 48, 48));
                        textureImageList64.Images.Add(file.FullName, getThumbNail(bmp, 64, 64));
                        textureImageList96.Images.Add(file.FullName, getThumbNail(bmp, 96, 96));
                        textureImageList128.Images.Add(file.FullName, getThumbNail(bmp, 128, 128));
                        textureImageList256.Images.Add(file.FullName, getThumbNail(bmp, 256, 256));

                        ListViewItem lvi = new ListViewItem();
                        lvi.Name = file.FullName;
                        lvi.Text = file.Name;
                        lvi.ImageKey = file.FullName;
                        lvi.Tag = "file";
                        lvi.ToolTipText = file.Name + " (" + bmp.Width.ToString() + " x " + bmp.Height.ToString() + ")";

                        texturesLV.Items.Add(lvi);
                    }
                    sw.Start();
                    backgroundWorker1.RunWorkerAsync(files);
                    break;
                case TEXTURE_TAB_PAGE:
                    img = Resources.folder;
                    textureImageList48.Images.Clear();
                    textureImageList64.Images.Clear();
                    textureImageList96.Images.Clear();
                    textureImageList128.Images.Clear();
                    textureImageList256.Images.Clear();

                    textureImageList48.Images.Add(img);
                    textureImageList64.Images.Add(img);
                    textureImageList96.Images.Add(img);
                    textureImageList128.Images.Add(img);
                    textureImageList256.Images.Add(img);
                    texturesLV.Clear();
                    textureDirBox.Text = di.FullName;
                    
                    foreach (DirectoryInfo folder in folders)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = folder.Name;
                        lvi.ToolTipText = folder.Name;
                        lvi.ImageIndex = 0;
                        lvi.Tag = "folder";
                        lvi.Name = folder.FullName;
                        texturesLV.Items.Add(lvi);
                    }
                    foreach (FileInfo file in files)
                    {
                        Bitmap bmp = new Bitmap(file.FullName);
                        textureImageList48.Images.Add(file.FullName, getThumbNail(bmp, 48, 48));
                        textureImageList64.Images.Add(file.FullName, getThumbNail(bmp, 64, 64));
                        textureImageList96.Images.Add(file.FullName, getThumbNail(bmp, 96, 96));
                        textureImageList128.Images.Add(file.FullName, getThumbNail(bmp, 128, 128));
                        textureImageList256.Images.Add(file.FullName, getThumbNail(bmp, 256, 256));

                        ListViewItem lvi = new ListViewItem();
                        lvi.Name = file.FullName;
                        lvi.Text = file.Name;
                        lvi.ImageKey = file.FullName;
                        lvi.Tag = "file";
                        lvi.ToolTipText = file.Name + " (" + bmp.Width.ToString() + " x " + bmp.Height.ToString() + ")";

                        texturesLV.Items.Add(lvi);
                    }
                    sw.Start();
                    backgroundWorker1.RunWorkerAsync(files);
                    break;
            }

        }

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public class PassedObject //for passing to background worker
        {
            public Bitmap bmp;
            public FileInfo fileinfo;
            public PassedObject()
            {
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            FileInfo[] files = (FileInfo[])e.Argument;
            BackgroundWorker worker = (BackgroundWorker)sender;
            int filesprogressed = 0;
            foreach (FileInfo file in files)
            {
                try
                {
                    PassedObject po = new PassedObject();
                    po.bmp = new Bitmap(file.FullName);
                    po.fileinfo = file;
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    filesprogressed++;
                    worker.ReportProgress(filesprogressed, po);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PassedObject po = (PassedObject)e.UserState;
            
            textureImageList48.Images.Add(po.fileinfo.FullName, getThumbNail(po.bmp, 48, 48));
            textureImageList64.Images.Add(po.fileinfo.FullName, getThumbNail(po.bmp, 64, 64));
            textureImageList96.Images.Add(po.fileinfo.FullName, getThumbNail(po.bmp, 96, 96));
            textureImageList128.Images.Add(po.fileinfo.FullName, getThumbNail(po.bmp, 128, 128));
            textureImageList256.Images.Add(po.fileinfo.FullName, getThumbNail(po.bmp, 256, 256));
            itemImageList48.Images.Add(po.fileinfo.FullName, getThumbNail(po.bmp, 48, 48));
            itemImageList64.Images.Add(po.fileinfo.FullName, getThumbNail(po.bmp, 64, 64));
            itemImageList96.Images.Add(po.fileinfo.FullName, getThumbNail(po.bmp, 96, 96));
            itemImageList128.Images.Add(po.fileinfo.FullName, getThumbNail(po.bmp, 128, 128));
            itemImageList256.Images.Add(po.fileinfo.FullName, getThumbNail(po.bmp, 256, 256));

            texturesLV.Items[po.fileinfo.FullName].ImageKey = po.fileinfo.FullName;
            texturesLV.Items[po.fileinfo.FullName].ToolTipText = po.fileinfo.Name + " (" + po.bmp.Width.ToString() + " x " + po.bmp.Height.ToString() + ")";

            /*ListViewItem lvi = new ListViewItem();
            lvi.Name = po.fileinfo.FullName;
            lvi.Text = po.fileinfo.Name;
            lvi.ImageKey = po.fileinfo.FullName;
            lvi.Tag = "file";
            lvi.ToolTipText = po.fileinfo.Name + " (" + po.bmp.Width + " x " + po.bmp.Height + ")";
            listView1.MapObjects.Add(lvi);
             * */
            
            toolStripStatusLabel1.Text = e.ProgressPercentage.ToString();
        }



        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            sw.Stop();
            toolStripStatusLabel1.Text = "Time: " + sw.Elapsed.TotalSeconds.ToString();
            sw.Reset();
        }



















    }
}
