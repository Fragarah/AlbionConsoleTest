using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AlbionConsole.Models;
using AlbionConsole.Services;

namespace AlbionGui
{
    public partial class TrackedItemsForm : Form
    {
        private readonly DatabaseService _databaseService;
        private List<TrackedItem> _allTrackedItems = new();

        public TrackedItemsForm(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;

            this.Shown += async (_, _) => await RefreshTrackedItemsAsync();
        }

        private async void TrackedItemsForm_Load(object sender, EventArgs e)
        {
            await RefreshTrackedItemsAsync();
        }

        private async Task LoadTrackedItemsAsync()
        {
            _allTrackedItems = await _databaseService.GetTrackedItemsAsync();
            FilterAndDisplayItems();
        }

        private void FilterAndDisplayItems()
        {
            listView1.Items.Clear();
            string query = searchTextBox.Text.Trim().ToLower();

            var filtered = _allTrackedItems
                .Where(t =>
                    (t.Item?.Name?.ToLower().Contains(query) ?? false) ||
                    (t.Location?.ToLower().Contains(query) ?? false))
                .ToList();

            foreach (var tracked in filtered)
            {
                var itemName = tracked.Item?.Name ?? "[Brak nazwy]";
                var row = new ListViewItem(itemName);
                row.SubItems.Add(tracked.Location);

                listView1.Items.Add(row);
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            FilterAndDisplayItems();
        }

        private async void deleteButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Zaznacz przynajmniej jeden element do usunięcia.");
                return;
            }

            var toDelete = new List<TrackedItem>();

            foreach (ListViewItem selected in listView1.SelectedItems)
            {
                string itemName = selected.SubItems[0].Text;
                string location = selected.SubItems[1].Text;

                var trackedItem = _allTrackedItems.FirstOrDefault(t =>
                    t.Item?.Name == itemName &&
                    t.Location == location);

                if (trackedItem != null)
                    toDelete.Add(trackedItem);
            }

            foreach (var trackedItem in toDelete)
            {
                await _databaseService.RemoveTrackedItemAsync(trackedItem);
            }

            await LoadTrackedItemsAsync();
            MessageBox.Show("Usunięto zaznaczone przedmioty.");
        }
        private void refreshButton_Click(object sender, EventArgs e)
        {
            _ = RefreshTrackedItemsAsync();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Można dodać dodatkowe funkcje np. szczegóły
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true; // Nie zamykaj
            this.Hide();     // Tylko ukryj
        }
        private async Task RefreshTrackedItemsAsync()
        {
            await LoadTrackedItemsAsync();
        }
    }
}
