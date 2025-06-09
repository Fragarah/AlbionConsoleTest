namespace AlbionGui
{
    partial class TrackedItemsForm
    {
        private ListView listView1;
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
            listView1 = new ListView();
            deleteButton = new Button();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(10, 10);
            listView1.Margin = new Padding(3, 2, 3, 2);
            listView1.Name = "listView1";
            listView1.Size = new Size(660, 400);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;

            // Dodajemy kolumny – TO BYŁO BRAKUJĄCE
            listView1.Columns.Add("Nazwa przedmiotu", 300);
            listView1.Columns.Add("Miasto", 200);

            // Ustawienie Anchor, żeby dobrze się skalował
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // 
            // deleteButton
            // 
            deleteButton.Location = new Point(10, 420);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(200, 30);
            deleteButton.TabIndex = 1;
            deleteButton.Text = "Usuń zaznaczone";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += deleteButton_Click;
            deleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            // 
            // TrackedItemsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 480);
            Controls.Add(deleteButton);
            Controls.Add(listView1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "TrackedItemsForm";
            Text = "Przeszukiwane przedmioty";
            Load += TrackedItemsForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button deleteButton;
    }
}