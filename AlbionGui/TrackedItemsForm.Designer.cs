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
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(10, 9);
            listView1.Margin = new Padding(3, 2, 3, 2);
            listView1.Name = "listView1";
            listView1.Size = new Size(526, 301);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            
            // Dodajemy tylko dwie kolumny:
            listView1.Columns.Add("Nazwa przedmiotu", 250);
            listView1.Columns.Add("Lokalizacja", 200);
            // 
            // TrackedItemsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(551, 322);
            Controls.Add(listView1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "TrackedItemsForm";
            Text = "Przeszukiwane przedmioty";
            Load += TrackedItemsForm_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}