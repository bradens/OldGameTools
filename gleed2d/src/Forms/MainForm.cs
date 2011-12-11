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
using System.Xml;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml.Serialization;

namespace GLEED2D
{
    public partial class MainForm : Form
    {
        public const String ITEM_TAB_PAGE = "itemsPage";
        public const String TEXTURE_TAB_PAGE = "texturesPage";
        public const String ITEM_LV_NAME_KEY = "itemsLV";
        public const String TEXTURE_LV_NAME_KEY = "texturesLV";

        public static MainForm Instance;
        String levelfilename;
        //BackgroundWorker bgw = new BackgroundWorker();

        bool dirtyflag;
        public bool DirtyFlag
        {
            get { return dirtyflag; }
            set { dirtyflag = value; updatetitlebar(); }
        }

        Cursor dragcursor;
        LinkItemsForm linkItemsForm;

        [DllImport("User32.dll")]
        private static extern int SendMessage(int Handle, int wMsg, int wParam, int lParam);

        public static void SetListViewSpacing(ListView lst, int x, int y)
        {
            SendMessage((int)lst.Handle, 0x1000 + 53, 0, y * 65536 + x);
        }


        public MainForm()
        {
            Instance = this;
            InitializeComponent();
        }
        public IntPtr getHandle()
        {
            return pictureBox1.Handle;
        }
        public void updatetitlebar()
        {
            Text = "GLEED2D - " + levelfilename + (DirtyFlag ? "*" : "");
        }

        public static Image getThumbNail(Bitmap bmp, int imgWidth, int imgHeight)
        {
            Bitmap retBmp = new Bitmap(imgWidth, imgHeight, System.Drawing.Imaging.PixelFormat.Format64bppPArgb);
            Graphics grp = Graphics.FromImage(retBmp);
            int tnWidth = imgWidth, tnHeight = imgHeight;
            if (bmp.Width > bmp.Height)
                tnHeight = (int)(((float)bmp.Height / (float)bmp.Width) * tnWidth);
            else if (bmp.Width < bmp.Height)
                tnWidth = (int)(((float)bmp.Width / (float)bmp.Height) * tnHeight);
            int iLeft = (imgWidth / 2) - (tnWidth / 2);
            int iTop = (imgHeight / 2) - (tnHeight / 2);
            grp.DrawImage(bmp, iLeft, iTop, tnWidth, tnHeight);
            retBmp.Tag = bmp;
            return retBmp;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //this.SetDesktopLocation(1300, 20);
            linkItemsForm = new LinkItemsForm();

            //fill zoom combobox
            for (int i = 25; i <= 200; i += 25)
            {
                zoomcombo.Items.Add(i.ToString() + "%");
            }
            zoomcombo.SelectedText = "100%";

            iconSizeBox.Items.Add("48x48");
            iconSizeBox.Items.Add("64x64");
            iconSizeBox.Items.Add("96x96");
            iconSizeBox.Items.Add("128x128");
            iconSizeBox.Items.Add("256x256");
            iconSizeBox.SelectedIndex = 1;

            SetListViewSpacing(listView2, 128 + 8, 128 + 32);

            pictureBox1.AllowDrop = true;
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            exportItemLib();
            Constants.Instance.export("settings.xml");
            Game1.Instance.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkCurrentLevelAndSaveEventually() == DialogResult.Cancel) e.Cancel = true;
        }

        //TREEVIEW
        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.N) ActionNewLayer(sender, e);
            if (e.KeyCode == Keys.Delete) ActionDelete(sender, e);
            if (e.KeyCode == Keys.F7) ActionMoveUp(sender, e);
            if (e.KeyCode == Keys.F8) ActionMoveDown(sender, e);
            if (e.KeyCode == Keys.F4) ActionCenterView(sender, e);
            if (e.KeyCode == Keys.F2) treeView1.SelectedNode.BeginEdit();
            if (e.KeyCode == Keys.D && e.Control) ActionDuplicate(sender, e);
        }
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null) return;

            TreeNode[] nodes = treeView1.Nodes.Find(e.Label, true);
            if (nodes.Length > 0)
            {
                MessageBox.Show("A layer or item with the name \"" + e.Label + "\" already exists in the level. Please use another name!");
                e.CancelEdit = true;
                return;
            }
            if (e.Node.Tag is Level)
            {
                Level l = (Level)e.Node.Tag;
                Editor.Instance.beginCommand("Rename Level (\"" + l.Name + "\" -> \"" + e.Label + "\")");
                l.Name = e.Label;
                e.Node.Name = e.Label;
                Editor.Instance.endCommand();
            }
            if (e.Node.Tag is Layer)
            {
                Layer l = (Layer)e.Node.Tag;
                Editor.Instance.beginCommand("Rename Layer (\"" + l.Name + "\" -> \"" + e.Label + "\")");
                l.Name = e.Label;
                e.Node.Name = e.Label;
                Editor.Instance.endCommand();
            }
            if (e.Node.Tag is MapObject)
            {
                MapObject i = (MapObject)e.Node.Tag;
                Editor.Instance.beginCommand("Rename MapObject (\"" + i.Name + "\" -> \"" + e.Label + "\")");
                i.Name = e.Label;
                e.Node.Name = e.Label;
                Editor.Instance.endCommand();
            }
            propertyGrid1.Refresh();
            pictureBox1.Select();
        }
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is Level)
            {
                Editor.Instance.level.Visible = e.Node.Checked;
            }
            if (e.Node.Tag is Layer)
            {
                Layer l = (Layer)e.Node.Tag;
                l.Visible = e.Node.Checked;
            }
            if (e.Node.Tag is MapObject)
            {
                MapObject i = (MapObject)e.Node.Tag;
                i.Visible = e.Node.Checked;
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is Level)
            {
                Editor.Instance.selectlevel();
            }
            if (e.Node.Tag is Layer)
            {
                Layer l = (Layer)e.Node.Tag;
                Editor.Instance.selectlayer(l);
            }
            if (e.Node.Tag is MapObject)
            {
                MapObject i = (MapObject)e.Node.Tag;
                Editor.Instance.selectitem(i);
            }
        }
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
            }
        }
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (((TreeNode)e.Item).Tag is Layer) return;
            if (((TreeNode)e.Item).Tag is Level) return;
            Editor.Instance.beginCommand("Drag MapObject");
            DoDragDrop(e.Item, DragDropEffects.Move);
        }
        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            //get source node
            TreeNode sourcenode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            if (sourcenode == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            else e.Effect = DragDropEffects.Move;

            //get destination node and select it
            Point p = treeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode destnode = treeView1.GetNodeAt(p);
            if (destnode.Tag is Level) return;
            treeView1.SelectedNode = destnode;

            if (destnode != sourcenode)
            {
                MapObject i1 = (MapObject)sourcenode.Tag;
                if (destnode.Tag is MapObject)
                {
                    MapObject i2 = (MapObject)destnode.Tag;
                    Editor.Instance.moveItemToLayer(i1, i2.layer, i2);
                    int delta = 0;
                    if (destnode.Index > sourcenode.Index && i1.layer == i2.layer) delta = 1;
                    sourcenode.Remove();
                    destnode.Parent.Nodes.Insert(destnode.Index + delta, sourcenode);
                }
                if (destnode.Tag is Layer)
                {
                    Layer l2 = (Layer)destnode.Tag;
                    Editor.Instance.moveItemToLayer(i1, l2, null);
                    sourcenode.Remove();
                    destnode.Nodes.Insert(0, sourcenode);
                }
                Editor.Instance.selectitem(i1);
                Editor.Instance.draw(Game1.Instance.spriteBatch);
                Game1.Instance.GraphicsDevice.Present();
                Application.DoEvents();
            }
        }
        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            Editor.Instance.endCommand();
        }



        //PICTURE BOX
        private void pictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            KeyEventArgs kea = new KeyEventArgs(e.KeyData);
            treeView1_KeyDown(sender, kea);
        }
        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            Logger.Instance.log("pictureBox1_Resize().");
            if (Game1.Instance != null) Game1.Instance.resizebackbuffer(pictureBox1.Width, pictureBox1.Height);
            if (Editor.Instance != null) Editor.Instance.camera.updateviewport(pictureBox1.Width, pictureBox1.Height);
        }
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Select();
        }
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            menuStrip1.Select();

        }
        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            ListViewItem lvi = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            if (lvi.Tag.ToString() == "xmlItem")
            {
                Item item = Editor.Instance.itemLibrary[lvi.Name];
                Editor.Instance.createItemBrush(item.cloneItem());
            }
            else
                Editor.Instance.createTextureBrush(lvi.Name);
        }
        private void pictureBox1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            Point p = pictureBox1.PointToClient(new Point(e.X, e.Y));
            Editor.Instance.setmousepos(p.X, p.Y);
            Editor.Instance.draw(Game1.Instance.spriteBatch);
            Game1.Instance.GraphicsDevice.Present();
        }
        private void pictureBox1_DragLeave(object sender, EventArgs e)
        {
            Editor.Instance.destroyTextureBrush();
            Editor.Instance.draw(Game1.Instance.spriteBatch);
            Game1.Instance.GraphicsDevice.Present();
        }
        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            Editor.Instance.paintTextureBrush(false);
            texturesLV.Cursor = Cursors.Default;
            itemsLV.Cursor = Cursors.Default;
            pictureBox1.Cursor = Cursors.Default;
        }




        // ACTIONS
        private void ActionDuplicate(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Tag is Layer)
            {
                Layer l = (Layer)treeView1.SelectedNode.Tag;
                Layer layercopy = l.clone();
                layercopy.Name = getUniqueNameBasedOn(layercopy.Name);
                for (int i = 0; i < layercopy.MapObjects.Count; i++)
                {
                    layercopy.MapObjects[i].Name = getUniqueNameBasedOn(layercopy.MapObjects[i].Name);
                }
                Editor.Instance.beginCommand("Duplicate Layer \"" + l.Name + "\"");
                Editor.Instance.addLayer(layercopy);
                Editor.Instance.endCommand();
                Editor.Instance.updatetreeview();
            }
        }
        private void ActionCenterView(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Tag is Level)
            {
                Editor.Instance.camera.Position = Microsoft.Xna.Framework.Vector2.Zero;
            }
            if (treeView1.SelectedNode.Tag is MapObject)
            {
                MapObject i = (MapObject)treeView1.SelectedNode.Tag;
                Editor.Instance.camera.Position = i.pPosition;
            }
        }
        private void ActionRename(object sender, EventArgs e)
        {
            treeView1.SelectedNode.BeginEdit();
        }
        private void ActionNewLayer(object sender, EventArgs e)
        {
            AddLayer f = new AddLayer(this);
            f.ShowDialog();
        }
        private void ActionDelete(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) return;
            if (treeView1.SelectedNode.Tag is Layer)
            {
                Layer l = (Layer)treeView1.SelectedNode.Tag;
                Editor.Instance.deleteLayer(l);
            }
            else if (treeView1.SelectedNode.Tag is MapObject)
            {
                Editor.Instance.deleteSelectedItems();
            }
        }
        private void ActionMoveUp(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Tag is Layer)
            {
                
                Layer l = (Layer)treeView1.SelectedNode.Tag;
                if (l.level.Layers.IndexOf(l) > 0)
                {
                    Editor.Instance.beginCommand("Move Up Layer \"" + l.Name + "\"");
                    Editor.Instance.moveLayerUp(l);
                    Editor.Instance.endCommand();
                    Editor.Instance.updatetreeview();
                }
            }
            if (treeView1.SelectedNode.Tag is MapObject)
            {
                MapObject i = (MapObject)treeView1.SelectedNode.Tag;
                if (i.layer.MapObjects.IndexOf(i) > 0)
                {
                    Editor.Instance.beginCommand("Move Up MapObject \"" + i.Name + "\"");
                    Editor.Instance.moveItemUp(i);
                    Editor.Instance.endCommand();
                    Editor.Instance.updatetreeview();
                }
            }
        }
        private void ActionMoveDown(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Tag is Layer)
            {
                Layer l = (Layer)treeView1.SelectedNode.Tag;
                if (l.level.Layers.IndexOf(l) < l.level.Layers.Count - 1)
                {
                    Editor.Instance.beginCommand("Move Down Layer \"" + l.Name + "\"");
                    Editor.Instance.moveLayerDown(l);
                    Editor.Instance.endCommand();
                    Editor.Instance.updatetreeview();
                }
            }
            if (treeView1.SelectedNode.Tag is MapObject)
            {
                MapObject i = (MapObject)treeView1.SelectedNode.Tag;
                if (i.layer.MapObjects.IndexOf(i) < i.layer.MapObjects.Count - 1)
                {
                    Editor.Instance.beginCommand("Move Down MapObject \"" + i.Name + "\"");
                    Editor.Instance.moveItemDown(i);
                    Editor.Instance.endCommand();
                    Editor.Instance.updatetreeview();
                }
            }
        }
        private void ActionAddCustomProperty(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Tag is MapObject)
            {
                MapObject i = (MapObject)treeView1.SelectedNode.Tag;
                AddCustomProperty form = new AddCustomProperty(i.CustomProperties);
                form.ShowDialog();
            }
            if (treeView1.SelectedNode.Tag is Level)
            {
                Level l = (Level)treeView1.SelectedNode.Tag;
                AddCustomProperty form = new AddCustomProperty(l.CustomProperties);
                form.ShowDialog();
            }
            if (treeView1.SelectedNode.Tag is Layer)
            {
                Layer l = (Layer)treeView1.SelectedNode.Tag;
                AddCustomProperty form = new AddCustomProperty(l.CustomProperties);
                form.ShowDialog();
            }
            propertyGrid1.Refresh();
        }







        //MENU
        public void newLevel()
        {
            Application.DoEvents();
            Level newlevel = new Level();
            newlevel.EditorRelated.Version = Editor.Instance.Version;
            Editor.Instance.loadLevel(newlevel);
            levelfilename = "untitled";
            DirtyFlag = false;
        }
        public void saveLevel(String filename)
        {
            Editor.Instance.saveLevel(filename);
            levelfilename = filename;
            DirtyFlag = false;

            if (Constants.Instance.SaveLevelStartApplication)
            {
                if (!File.Exists(Constants.Instance.SaveLevelApplicationToStart))
                {
                    MessageBox.Show("The file \"" + Constants.Instance.SaveLevelApplicationToStart + "\" doesn't exist!\nPlease provide a valid application executable in Tools -> Settings -> Save Level!\nLevel was saved.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Constants.Instance.SaveLevelAppendLevelFilename)
                {
                    Process.Start(Constants.Instance.SaveLevelApplicationToStart, "\"" + levelfilename + "\"");
                }
                else
                {
                    Process.Start(Constants.Instance.SaveLevelApplicationToStart);
                }
            }

        }
        public void loadLevel(String filename)
        {
            Level level = Level.FromFile(filename, Game1.Instance.Content);


            if (level.EditorRelated.Version == null || level.EditorRelated.Version.CompareTo("1.3") < 0)
            {
                DialogResult dr = MessageBox.Show(
                    "This file was created with a version of GLEED2D less than 1.3.\n" +
                    "In version 1.3 the datatype of 'Scale' changed from 'float' to 'Vector2'.\n" +
                    "The file you tried to open should therefore be converted accordingly.\n" +
                    "GLEED2D can do that automatically for you.\n\n"+
                    "(Basically, all that's done is convert \n"+
                    "<Scale>1.234</Scale> to \n"+
                    "<Scale><X>1.234</X><Y>1.234</Y></Scale>)\n\n" +
                    "Do you want the file to be converted before opening?",
                    "Convert?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    //Convert the file, save it and open it 
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filename);
                    //change Scale from float to Vector2 with X = Y = previous scale factor
                    doc.InnerXml = Regex.Replace(doc.InnerXml, @"<Scale>(\d+\.?\d*)", "<Scale><X>$1</X><Y>$1</Y>");

                    //update/insert <Version> tag
                    if (doc.GetElementsByTagName("Version").Count > 0) //<Version> tag exists and must be updated
                    {
                        doc.InnerXml = Regex.Replace(doc.InnerXml, @"<Version>(\d+\.?\d*\.?\d*\.?\d*)", "<Version>" + Editor.Instance.Version);
                    }
                    else //level files prior to 1.3 didn't have a version tag so insert one
                    {
                        doc.GetElementsByTagName("EditorRelated").Item(0).InnerXml += "<Version>" + Editor.Instance.Version + "</Version>";
                    }
                    //save the changes...
                    doc.Save(filename);

                    //...and load it again
                    level = Level.FromFile(filename, Game1.Instance.Content);
                }
                else
                {
                    return;
                }
            }


            Editor.Instance.loadLevel(level);
            levelfilename = filename;
            DirtyFlag = false;
        }
        public DialogResult checkCurrentLevelAndSaveEventually()
        {
            if (DirtyFlag)
            {
                DialogResult dr = MessageBox.Show("The current level has not been saved. Do you want to save now?", "Save?",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    if (levelfilename == "untitled")
                    {
                        SaveFileDialog dialog = new SaveFileDialog();
                        dialog.Filter = "XML Files (*.xml)|*.xml";
                        if (dialog.ShowDialog() == DialogResult.OK) saveLevel(dialog.FileName);
                        else return DialogResult.Cancel;
                    }
                    else
                    {
                        saveLevel(levelfilename);
                    }
                }
                if (dr == DialogResult.Cancel) return DialogResult.Cancel;
            }
            return DialogResult.OK;
        }

        private void FileNew(object sender, EventArgs e)
        {
            if (checkCurrentLevelAndSaveEventually() == DialogResult.Cancel) return;
            newLevel();
        }
        private void FileOpen(object sender, EventArgs e)
        {
            if (checkCurrentLevelAndSaveEventually() == DialogResult.Cancel) return;
            OpenFileDialog opendialog = new OpenFileDialog();
            opendialog.Filter = "XML Files (*.xml)|*.xml";
            if (opendialog.ShowDialog() == DialogResult.OK) loadLevel(opendialog.FileName);
        }
        private void FileSave(object sender, EventArgs e)
        {
            if (levelfilename == "untitled") FileSaveAs(sender, e);
            else saveLevel(levelfilename);

        }
        private void FileSaveAs(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML Files (*.xml)|*.xml";
            if (dialog.ShowDialog() == DialogResult.OK) saveLevel(dialog.FileName);
        }
        private void FileExit(object sender, EventArgs e)
        {
            Close();
        }



        private void EditUndo(object sender, EventArgs e)
        {
            Editor.Instance.undo();
        }

        private void EditRedo(object sender, EventArgs e)
        {
            Editor.Instance.redo();
        }

        private void EditSelectAll(object sender, EventArgs e)
        {
            Editor.Instance.selectAll();
        }

        private void zoomcombo_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = zoomcombo.SelectedText;
            if (zoomcombo.Text.Length > 0 && Editor.Instance != null)
            {
                float zoom = float.Parse(zoomcombo.Text.Substring(0, zoomcombo.Text.Length - 1));
                Editor.Instance.camera.Scale = zoom / 100;
            }
        }

        private void HelpQuickGuide(object sender, EventArgs e)
        {
            new QuickGuide().Show();
        }

        private void HelpAbout(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }


        private void ToolsMenu_MouseEnter(object sender, EventArgs e)
        {
            moveSelectedItemsToLayerToolStripMenuItem.Enabled =
            copySelectedItemsToLayerToolStripMenuItem.Enabled = Editor.Instance.SelectedItems.Count > 0;
            alignHorizontallyToolStripMenuItem.Enabled =
            alignVerticallyToolStripMenuItem.Enabled =
            alignRotationToolStripMenuItem.Enabled =
            alignScaleToolStripMenuItem.Enabled = Editor.Instance.SelectedItems.Count > 1;

            linkItemsByACustomPropertyToolStripMenuItem.Enabled = Editor.Instance.SelectedItems.Count == 2;

        }
        private void ToolsMenu_Click(object sender, EventArgs e)
        {
        }
        private void ToolsMoveToLayer(object sender, EventArgs e)
        {
            LayerSelectForm f = new LayerSelectForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                Layer chosenlayer = (Layer)f.treeView1.SelectedNode.Tag;
                Editor.Instance.moveSelectedItemsToLayer(chosenlayer);
            }

        }
        private void ToolsCopyToLayer(object sender, EventArgs e)
        {
            LayerSelectForm f = new LayerSelectForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                Layer chosenlayer = (Layer)f.treeView1.SelectedNode.Tag;
                Editor.Instance.copySelectedItemsToLayer(chosenlayer);
            }
        }
        private void ToolsLinkItems(object sender, EventArgs e)
        {
            linkItemsForm.ShowDialog();
        }
        private void ToolsAlignHorizontally(object sender, EventArgs e)
        {
            Editor.Instance.alignHorizontally();
        }
        private void ToolsAlignVertically(object sender, EventArgs e)
        {
            Editor.Instance.alignVertically();
        }
        private void ToolsAlignRotation(object sender, EventArgs e)
        {
            Editor.Instance.alignRotation();
        }
        private void ToolsAlignScale(object sender, EventArgs e)
        {
            Editor.Instance.alignScale();
        }
        private void ToolsSettings(object sender, EventArgs e)
        {
            SettingsForm f = new SettingsForm();
            f.ShowDialog();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Editor.Instance.draw(Game1.Instance.spriteBatch);
            Game1.Instance.GraphicsDevice.Present();
            Application.DoEvents();
        }


        private void propertyGrid1_Enter(object sender, EventArgs e)
        {
            Editor.Instance.beginCommand("Edit in PropertyGrid");
        }
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Editor.Instance.endCommand();
            Editor.Instance.beginCommand("Edit in PropertyGrid");
        }

        public void UndoManyCommands(object sender, ToolStripItemClickedEventArgs e)
        {
            Command c = (Command)e.ClickedItem.Tag;
            Editor.Instance.undoMany(c);
        }

        private void RedoManyCommands(object sender, ToolStripItemClickedEventArgs e)
        {
            Command c = (Command)e.ClickedItem.Tag;
            Editor.Instance.redoMany(c);
        }

        private void comboSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (iconSizeBox.SelectedIndex)
            {
                case 0:
                    texturesLV.LargeImageList = textureImageList48;
                    SetListViewSpacing(texturesLV, 48 + 8, 48 + 32);
                    itemsLV.LargeImageList = itemImageList48;
                    SetListViewSpacing(itemsLV, 48 + 8, 48 + 32);
                    break;
                case 1:
                    texturesLV.LargeImageList = textureImageList64;
                    SetListViewSpacing(texturesLV, 64 + 8, 64 + 32);
                    itemsLV.LargeImageList = itemImageList64;
                    SetListViewSpacing(itemsLV, 64 + 8, 64 + 32);
                    break;
                case 2:
                    texturesLV.LargeImageList = textureImageList96;
                    SetListViewSpacing(texturesLV, 96 + 8, 96 + 32);
                    itemsLV.LargeImageList = itemImageList96;
                    SetListViewSpacing(itemsLV, 96 + 8, 96 + 32);
                    break;
                case 3:
                    texturesLV.LargeImageList = textureImageList128;
                    SetListViewSpacing(texturesLV, 128 + 8, 128 + 32);
                    itemsLV.LargeImageList = itemImageList128;
                    SetListViewSpacing(itemsLV, 128 + 8, 128 + 32);
                    break;
                case 4:
                    texturesLV.LargeImageList = textureImageList256;
                    SetListViewSpacing(texturesLV, 256 + 8, 256 + 32);
                    itemsLV.LargeImageList = itemImageList256;
                    SetListViewSpacing(itemsLV, 256 + 8, 256 + 32);
                    break;
            }
        }

        private void buttonFolderUp_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = null;
            TabPage currPage = tabControl1.SelectedTab;
            switch (tabControl1.SelectedTab.Name) {
                case ITEM_TAB_PAGE:
                    di = Directory.GetParent(itemDirBox.Text);
                    break;
                case TEXTURE_TAB_PAGE:
                    di = Directory.GetParent(textureDirBox.Text);
                    break;
            }

            if (di == null) return;
            loadfolder(di.FullName, currPage);
        }
        private void chooseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            switch (this.getSelectedPage().Name)
            {
                case ITEM_TAB_PAGE:
                    d.SelectedPath = itemDirBox.Text;
                    break;
                case TEXTURE_TAB_PAGE:
                    d.SelectedPath = textureDirBox.Text;
                    break;
            }
            if (d.ShowDialog() == DialogResult.OK) loadfolder(d.SelectedPath, this.getSelectedPage());
        }
        private void LV_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = ((ListView)sender).FocusedItem.ToolTipText;
        }
        private void LV_DoubleClick(object sender, MouseEventArgs e)
        {
            ListView currView = (ListView)sender;
            string itemtype = currView.FocusedItem.Tag.ToString();
            switch (itemtype)
            {
                case "folder":
                    loadfolder(currView.FocusedItem.Name, this.getSelectedPage());
                    break;
                case "file":
                    Editor.Instance.createTextureBrush(currView.FocusedItem.Name);
                    break;
                case "xmlItem":
                    Editor.Instance.createTextureBrush(currView.FocusedItem.Name);
                    break;
            }
        }
        private void LV_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListViewItem lvi = (ListViewItem)e.Item;
            if (lvi.Tag.ToString() == "folder") return;
            toolStripStatusLabel1.Text = lvi.ToolTipText;
            Bitmap bmp = new Bitmap(((ListView)sender).LargeImageList.Images[lvi.ImageKey]);
            dragcursor = new Cursor(bmp.GetHicon());
            ((ListView)sender).DoDragDrop(e.Item, DragDropEffects.Move);
        }
        private void LV_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        private void LV_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effect == DragDropEffects.Move)
            {
                e.UseDefaultCursors = false;
                ((ListView)sender).Cursor = dragcursor;
                pictureBox1.Cursor = Cursors.Default;
            }
            else
            {
                e.UseDefaultCursors = true;
                ((ListView)sender).Cursor = Cursors.Default;
                pictureBox1.Cursor = Cursors.Default;
            }
        }
        private void LV_DragDrop(object sender, DragEventArgs e)
        {
            ((ListView)sender).Cursor = Cursors.Default;
            pictureBox1.Cursor = Cursors.Default;
        }

        public string getUniqueNameBasedOn(string name)
        {
            int i=0;
            string newname = "Copy of " + name;
            while (treeView1.Nodes.Find(newname, true).Length>0) 
            {
                newname = "Copy(" + i++.ToString() + ") of " + name;
            }
            return newname;
        }

        public TabPage getSelectedPage()
        {
            return tabControl1.SelectedTab;
        }

        public void loadfolder(string path, TabPage currPage)
        {
            //loadfolder_background(path);
            loadfolder_foreground(path, currPage);
        }

        public void setDefaultItemFolder(string path)
        {
            itemDirBox.Text = new DirectoryInfo(path).FullName;
        }

        public void loadfolder_foreground(string path, TabPage currPage)
        {
            Image img = Resources.folder;
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] folders = di.GetDirectories();
            string filters;
            List<FileInfo> fileList = new List<FileInfo>();
            string[] extensions;
            FileInfo[] files = fileList.ToArray();
            filters = "*.jpg;*.png;*.bmp;";
            extensions = filters.Split(';');
            img = Resources.folder;
            fileList = new List<FileInfo>();
            foreach (string filter in extensions) fileList.AddRange(di.GetFiles(filter));
            files = fileList.ToArray();
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
        }

        /// <summary>
        /// Clears and loads all the items from the Editor.itemLibrary into the listView
        /// </summary>
        public void reloadItemsLV()
        {
            itemImageList48.Images.Clear();
            itemImageList64.Images.Clear();
            itemImageList96.Images.Clear();
            itemImageList128.Images.Clear();
            itemImageList256.Images.Clear();

            itemsLV.Clear();

            foreach (Item i in Editor.Instance.itemLibrary.Values)
            {
                Bitmap bmp = new Bitmap(i.iconPath);
                itemImageList48.Images.Add(i.Name, getThumbNail(bmp, 48, 48));
                itemImageList64.Images.Add(i.Name, getThumbNail(bmp, 64, 64));
                itemImageList96.Images.Add(i.Name, getThumbNail(bmp, 96, 96));
                itemImageList128.Images.Add(i.Name, getThumbNail(bmp, 128, 128));
                itemImageList256.Images.Add(i.Name, getThumbNail(bmp, 256, 256));

                ListViewItem lvi = new ListViewItem();
                lvi.Name = i.Name;
                lvi.Text = i.Name;
                lvi.ImageKey = i.Name;
                lvi.Tag = "xmlItem";
                lvi.ToolTipText = i.description + " (" + bmp.Width.ToString() + " x " + bmp.Height.ToString() + ")";

                itemsLV.Items.Add(lvi);
            }
        }

        public void loadItems()
        {
            FileStream stream;
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableGenericDictionary<string, Item>));
            stream = File.Open(Constants.Instance.ItemLibraryLocation, FileMode.Open);
            Editor.Instance.itemLibrary = (SerializableGenericDictionary<string, Item>)serializer.Deserialize(stream);
            stream.Close();
        }

        public void exportItemLib()
        {
            FileStream fs = File.Open(Constants.Instance.ItemLibraryLocation, FileMode.Create);
            XmlSerializer xs = new XmlSerializer(typeof(SerializableGenericDictionary<string, Item>));
            xs.Serialize(fs, Editor.Instance.itemLibrary);
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView2.FocusedItem.Text == "CollsionRectangle")
            {
                Editor.Instance.createPrimitiveBrush(PrimitiveType.CollisionRectangle);
            }
            else if (listView2.FocusedItem.Text == "Circle")
            {
                Editor.Instance.createPrimitiveBrush(PrimitiveType.Circle);
            }
            else if (listView2.FocusedItem.Text == "Path")
            {
                Editor.Instance.createPrimitiveBrush(PrimitiveType.Path);
            }
        }



        private void RunLevel(object sender, EventArgs e)
        {
            if (Constants.Instance.RunLevelStartApplication)
            {
                if (!System.IO.File.Exists(Constants.Instance.RunLevelApplicationToStart))
                {
                    MessageBox.Show("The file \"" + Constants.Instance.RunLevelApplicationToStart + "\" doesn't exist!\nPlease provide a valid application executable in Tools -> Settings -> Run Level!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                FileSave(sender, e);
                if (Constants.Instance.RunLevelAppendLevelFilename)
                {
                    System.Diagnostics.Process.Start(Constants.Instance.RunLevelApplicationToStart, "\"" + levelfilename + "\"");
                }
                else
                {
                    System.Diagnostics.Process.Start(Constants.Instance.RunLevelApplicationToStart);
                }
            }
        }

        private void CustomPropertyContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (propertyGrid1.SelectedGridItem.Parent.Label != "Custom Properties") e.Cancel = true;
        }

        private void deleteCustomProperty(object sender, EventArgs e)
        {
            Editor.Instance.beginCommand("Delete Custom Property");
            DictionaryPropertyDescriptor dpd = (DictionaryPropertyDescriptor)propertyGrid1.SelectedGridItem.PropertyDescriptor;
            dpd.sdict.Remove(dpd.Name);
            propertyGrid1.Refresh();
            Editor.Instance.endCommand();
        }

        private void ViewGrid_CheckedChanged(object sender, EventArgs e)
        {
            Constants.Instance.ShowGrid = ShowGridButton.Checked = ViewGrid.Checked;
        }
        
        private void ViewWorldOrigin_CheckedChanged(object sender, EventArgs e)
        {
            Constants.Instance.ShowWorldOrigin = ShowWorldOriginButton.Checked = ViewWorldOrigin.Checked;
        }
        
        private void ShowGridButton_CheckedChanged(object sender, EventArgs e)
        {
            Constants.Instance.ShowGrid = ViewGrid.Checked = ShowGridButton.Checked;
        }

        private void ShowWorldOriginButton_CheckedChanged(object sender, EventArgs e)
        {
            Constants.Instance.ShowWorldOrigin = ViewWorldOrigin.Checked = ShowWorldOriginButton.Checked;
        }

        private void SnapToGridButton_CheckedChanged(object sender, EventArgs e)
        {
            Constants.Instance.SnapToGrid =  ViewSnapToGrid.Checked = SnapToGridButton.Checked;
        }

        private void ViewSnapToGrid_CheckedChanged(object sender, EventArgs e)
        {
            Constants.Instance.SnapToGrid = SnapToGridButton.Checked = ViewSnapToGrid.Checked;
        }

        private void soSwitch_CheckedChanged(object sender, EventArgs e)
        {
            Editor.Instance.staticItemMode = soSwitch.Checked;
        }

        private void poSwitch_CheckedChanged(object sender, EventArgs e)
        {
            Editor.Instance.physicsItemMode = poSwitch.Checked;
        }

        private void aoSwitch_CheckedChanged(object sender, EventArgs e)
        {
            Editor.Instance.animationMode = aoSwitch.Checked;
        }

        private void newItemBtn_Click(object sender, EventArgs e)
        {
            NewItemForm newItemDialog = new NewItemForm();
            newItemDialog.ShowDialog(this);
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == ITEM_TAB_PAGE)
                reloadItemsLV();
        }
    }
}
