namespace AlbionGui
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            itemsList = new CheckedListBox();
            addButton = new Button();
            searchButton = new Button();
            findButton = new Button();
            label1 = new Label();
            label2 = new Label();
            citiesList = new CheckedListBox();
            label3 = new Label();
            importButton = new Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(31, 58);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(383, 23);
            textBox1.TabIndex = 0;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // itemsList
            // 
            itemsList.FormattingEnabled = true;
            itemsList.Location = new Point(31, 130);
            itemsList.Name = "itemsList";
            itemsList.Size = new Size(383, 220);
            itemsList.TabIndex = 2;
            itemsList.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged;
            // 
            // addButton
            // 
            addButton.Location = new Point(301, 370);
            addButton.Name = "addButton";
            addButton.Size = new Size(113, 23);
            addButton.TabIndex = 1;
            addButton.Text = "Dodaj przedmiot";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += addButton_Click;
            // 
            // searchButton
            // 
            searchButton.Location = new Point(546, 415);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(242, 23);
            searchButton.TabIndex = 3;
            searchButton.Text = "Przeszukiwane przedmioty";
            searchButton.UseVisualStyleBackColor = true;
            searchButton.Click += searchingButton_Click;
            // 
            // findButton
            // 
            findButton.Location = new Point(445, 58);
            findButton.Name = "findButton";
            findButton.Size = new Size(113, 23);
            findButton.TabIndex = 4;
            findButton.Text = "Wyszukaj";
            findButton.UseVisualStyleBackColor = true;
            findButton.Click += findButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(31, 24);
            label1.Name = "label1";
            label1.Size = new Size(138, 15);
            label1.TabIndex = 5;
            label1.Text = "Wpisz nazwę przedmiotu";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(32, 99);
            label2.Name = "label2";
            label2.Size = new Size(118, 15);
            label2.TabIndex = 6;
            label2.Text = "Wyniki wyszukiwania";
            // 
            // citiesList
            // 
            citiesList.FormattingEnabled = true;
            citiesList.Location = new Point(445, 130);
            citiesList.Name = "citiesList";
            citiesList.Size = new Size(279, 220);
            citiesList.TabIndex = 7;
            citiesList.SelectedIndexChanged += checkedListBox2_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(469, 99);
            label3.Name = "label3";
            label3.Size = new Size(47, 15);
            label3.TabIndex = 8;
            label3.Text = "Lokacje";
            // 
            // importButton
            // 
            importButton.Location = new Point(12, 415);
            importButton.Name = "importButton";
            importButton.Size = new Size(202, 23);
            importButton.TabIndex = 9;
            importButton.Text = "Zaimportuj dane do bazy";
            importButton.UseVisualStyleBackColor = true;
            importButton.Click += importButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(importButton);
            Controls.Add(label3);
            Controls.Add(citiesList);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(findButton);
            Controls.Add(searchButton);
            Controls.Add(itemsList);
            Controls.Add(addButton);
            Controls.Add(textBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Button addButton;
        private CheckedListBox itemsList;
        private Button searchButton;
        private Button findButton;
        private Label label1;
        private Label label2;
        private CheckedListBox citiesList;
        private Label label3;
        private Button importButton;
    }
}
