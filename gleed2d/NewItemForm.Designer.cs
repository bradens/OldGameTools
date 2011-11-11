namespace GLEED2D
{
    partial class NewItemForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.texturePreview = new System.Windows.Forms.PictureBox();
            this.iconPreview = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.iconFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.textureFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.texturePreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // texturePreview
            // 
            this.texturePreview.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.texturePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.texturePreview.Location = new System.Drawing.Point(12, 22);
            this.texturePreview.Name = "texturePreview";
            this.texturePreview.Size = new System.Drawing.Size(222, 169);
            this.texturePreview.TabIndex = 0;
            this.texturePreview.TabStop = false;
            // 
            // iconPreview
            // 
            this.iconPreview.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.iconPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.iconPreview.Location = new System.Drawing.Point(12, 210);
            this.iconPreview.Name = "iconPreview";
            this.iconPreview.Size = new System.Drawing.Size(64, 64);
            this.iconPreview.TabIndex = 1;
            this.iconPreview.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Texture Preview";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 194);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Icon Preview";
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(418, 656);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(115, 23);
            this.okBtn.TabIndex = 4;
            this.okBtn.Text = "Submit";
            this.okBtn.UseVisualStyleBackColor = true;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.Location = new System.Drawing.Point(539, 656);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(115, 23);
            this.cancelBtn.TabIndex = 5;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // iconFileDialog
            // 
            this.iconFileDialog.FileName = "iconPreviewFile";
            this.iconFileDialog.Title = "iconFileDialog";
            // 
            // textureFileDialog
            // 
            this.textureFileDialog.FileName = "textureFilePreview";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(241, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Name: ";
            // 
            // nameBox
            // 
            this.nameBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nameBox.Location = new System.Drawing.Point(283, 19);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(361, 20);
            this.nameBox.TabIndex = 7;
            // 
            // NewItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 682);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.iconPreview);
            this.Controls.Add(this.texturePreview);
            this.Name = "NewItemForm";
            this.Text = "New Item Form";
            ((System.ComponentModel.ISupportInitialize)(this.texturePreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox texturePreview;
        private System.Windows.Forms.PictureBox iconPreview;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.OpenFileDialog iconFileDialog;
        private System.Windows.Forms.OpenFileDialog textureFileDialog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox nameBox;
    }
}