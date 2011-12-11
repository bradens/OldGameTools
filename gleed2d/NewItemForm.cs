using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GLEED2D
{
    public partial class NewItemForm : Form
    {
        public Item CurrentItem;
        
        public NewItemForm()
        {
            InitializeComponent();
            CurrentItem = new Item();
            texturePreview.SizeMode = PictureBoxSizeMode.StretchImage;
            iconPreview.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void texturePreview_Click(object sender, EventArgs e)
        {
            DialogResult d = textureFileDialog.ShowDialog();
            texturePreview.ImageLocation = textureFileDialog.FileName;
        }

        private void iconPreview_Click(object sender, EventArgs e)
        {
            DialogResult d = iconFileDialog.ShowDialog();
            iconPreview.ImageLocation = iconFileDialog.FileName;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            // TODO complete item build.
            CurrentItem.Name = nameBox.Text;
            CurrentItem.description = descBox.Text;
            CurrentItem.attributes = new SerializableDictionary();
            if (healthBox.Text != "")
                CurrentItem.attributes.Add("Health", new CustomProperty("hp", healthBox.Text, typeof(string), "Health"));
            if (energyBox.Text != "")
                CurrentItem.attributes.Add("Energy", new CustomProperty("eng", energyBox.Text, typeof(string), "Energy"));
            if (strengthBox.Text != "")
                CurrentItem.attributes.Add("Strength", new CustomProperty("str", strengthBox.Text, typeof(string), "Strength"));
            if (agilityBox.Text != "")
                CurrentItem.attributes.Add("Agility", new CustomProperty("agility", agilityBox.Text, typeof(string), "Agility"));
            if (cunningBox.Text != "")
                CurrentItem.attributes.Add("Cunning", new CustomProperty("cunning", cunningBox.Text, typeof(string), "Cunning"));
            CurrentItem.iconPath = iconPreview.ImageLocation;
            CurrentItem.texturePath = texturePreview.ImageLocation;
            CurrentItem.asset_name = texturePreview.ImageLocation;

            bool valid = validateFields();
            if (valid)
            {
                Editor.Instance.itemLibrary.Add(CurrentItem.Name, CurrentItem);
            }
        }

        /// <summary>
        /// Validates the current item's values for syntax, and checks the item isnt
        /// already in the library.
        /// </summary>
        /// <returns>True for valid, false, otherwise</returns>
        private bool validateFields()
        {
            if (Editor.Instance.itemLibrary.ContainsKey(CurrentItem.Name))
                return false;
            return true;
        }
    }
}
