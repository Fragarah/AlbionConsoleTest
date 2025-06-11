namespace AlbionGui
{
    partial class TrackedItemsForm
    {
        private System.ComponentModel.IContainer components = null;
        private ListView listView1;
        private Button deleteButton;
        private Button refreshButton;
        private TextBox searchTextBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            listView1 = new ListView();
            deleteButton = new Button();
            refreshButton = new Button();
            searchTextBox = new TextBox();

            SuspendLayout();

            // 
            // searchTextBox
            // 
            searchTextBox.Dock = DockStyle.Top;
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new Size(700, 23);
            searchTextBox.PlaceholderText = "Szukaj przedmiotu";
            searchTextBox.TextChanged += searchTextBox_TextChanged;

            // 
            // listView1
            // 
            listView1.Dock = DockStyle.Fill;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.View = View.Details;
            listView1.Name = "listView1";
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;

            // Dodajemy kolumny
            listView1.Columns.Add("Nazwa", 300, HorizontalAlignment.Left);
            listView1.Columns.Add("Miasto", 300, HorizontalAlignment.Left);

            // 
            // deleteButton
            // 
            deleteButton.Dock = DockStyle.Bottom;
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(700, 30);
            deleteButton.Text = "Usuń zaznaczone";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += deleteButton_Click;

            // 
            // refreshButton
            // 
            refreshButton.Dock = DockStyle.Bottom;
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(700, 30);
            refreshButton.Text = "Odśwież";
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.Click += refreshButton_Click;

            // 
            // TrackedItemsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 576);
            Controls.Add(listView1);
            Controls.Add(refreshButton);
            Controls.Add(deleteButton);
            Controls.Add(searchTextBox);
            Name = "TrackedItemsForm";
            Text = "Przeszukiwane przedmioty";
            Load += TrackedItemsForm_Load;

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}