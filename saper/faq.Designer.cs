
namespace saper
{
    partial class FAQ
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FAQ));
            this.FAQPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.FAQPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // FAQPanel
            // 
            this.FAQPanel.Controls.Add(this.label4);
            this.FAQPanel.Controls.Add(this.label3);
            this.FAQPanel.Controls.Add(this.label2);
            this.FAQPanel.Controls.Add(this.label1);
            this.FAQPanel.Location = new System.Drawing.Point(-1, 0);
            this.FAQPanel.Name = "FAQPanel";
            this.FAQPanel.Size = new System.Drawing.Size(435, 315);
            this.FAQPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.45F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(117, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Игра \"Сапер\"";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(238, 65);
            this.label2.TabIndex = 1;
            this.label2.Text = "Цель игры \r\nоткрыть все игровое поле свободное от мин.\r\n\r\nИгра продолжается до те" +
    "х пор \r\nпока имеются закрыте клетки без мин.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(389, 91);
            this.label3.TabIndex = 2;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 253);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(311, 52);
            this.label4.TabIndex = 3;
            this.label4.Text = "Клекти можно метить с помощью клика ПКМ по ней, \r\nэто превратит её помеченую клет" +
    "ку.\r\nТакая клетка при клике ЛКМ не будет вызывать открытие,\r\nна ней нельзя подор" +
    "ваться, если там есть бомба.\r\n";
            // 
            // FAQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 321);
            this.Controls.Add(this.FAQPanel);
            this.Name = "FAQ";
            this.Text = "faq";
            this.Load += new System.EventHandler(this.faq_Load);
            this.FAQPanel.ResumeLayout(false);
            this.FAQPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel FAQPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}