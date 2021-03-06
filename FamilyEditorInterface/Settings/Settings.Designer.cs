﻿namespace FamilyEditorInterface.Settings
{
    partial class Settings
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
            this.components = new System.ComponentModel.Container();
            this.OKButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.roundingLabel = new System.Windows.Forms.Label();
            this.roudingDropDown = new System.Windows.Forms.ComboBox();
            this.roudingTextBox = new System.Windows.Forms.TextBox();
            this.sysParamChk = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(206, 108);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 6;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(287, 108);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(153, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Rounding Increment:";
            // 
            // roundingLabel
            // 
            this.roundingLabel.AutoSize = true;
            this.roundingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundingLabel.Location = new System.Drawing.Point(12, 19);
            this.roundingLabel.Name = "roundingLabel";
            this.roundingLabel.Size = new System.Drawing.Size(56, 13);
            this.roundingLabel.TabIndex = 5;
            this.roundingLabel.Text = "Rounding:";
            // 
            // roudingDropDown
            // 
            this.roudingDropDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roudingDropDown.FormattingEnabled = true;
            this.roudingDropDown.Items.AddRange(new object[] {
            "0 decimal places",
            "1 decimal places",
            "2 decimal places"});
            this.roudingDropDown.Location = new System.Drawing.Point(12, 40);
            this.roudingDropDown.Name = "roudingDropDown";
            this.roudingDropDown.Size = new System.Drawing.Size(121, 21);
            this.roudingDropDown.TabIndex = 4;
            this.roudingDropDown.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // roudingTextBox
            // 
            this.roudingTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roudingTextBox.Location = new System.Drawing.Point(156, 40);
            this.roudingTextBox.Name = "roudingTextBox";
            this.roudingTextBox.ReadOnly = true;
            this.roudingTextBox.Size = new System.Drawing.Size(194, 20);
            this.roudingTextBox.TabIndex = 3;
            this.roudingTextBox.Text = "1";
            // 
            // sysParamChk
            // 
            this.sysParamChk.AutoSize = true;
            this.sysParamChk.Location = new System.Drawing.Point(12, 81);
            this.sysParamChk.Name = "sysParamChk";
            this.sysParamChk.Size = new System.Drawing.Size(114, 17);
            this.sysParamChk.TabIndex = 9;
            this.sysParamChk.Text = "Built-In Parameters";
            this.toolTip1.SetToolTip(this.sysParamChk, "Show Built-In Parameters. Built-In Parameters cannot be renamed or deleted.");
            this.sysParamChk.UseVisualStyleBackColor = true;
            this.sysParamChk.CheckedChanged += new System.EventHandler(this.sysParamChk_CheckedChanged);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 143);
            this.Controls.Add(this.sysParamChk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.roundingLabel);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.roudingDropDown);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.roudingTextBox);
            this.MaximumSize = new System.Drawing.Size(390, 182);
            this.MinimumSize = new System.Drawing.Size(390, 182);
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox roudingDropDown;
        private System.Windows.Forms.TextBox roudingTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label roundingLabel;
        private System.Windows.Forms.CheckBox sysParamChk;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}