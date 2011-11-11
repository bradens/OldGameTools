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
        public NewItemForm()
        {
            InitializeComponent();
        }

        private void texturePreview_Click(object sender, EventArgs e)
        {
            DialogResult d = textureFileDialog.ShowDialog();
        }

        private void iconPreview_Click(object sender, EventArgs e)
        {
            DialogResult d = iconFileDialog.ShowDialog();
        }
    }
}
