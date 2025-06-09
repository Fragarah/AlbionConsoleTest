using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AlbionConsole.Services;

namespace AlbionGui
{
    public partial class TrackedItemsForm : Form
    {
        private readonly DatabaseService _databaseService;
        public TrackedItemsForm(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }

        private async void TrackedItemsForm_Load(object sender, EventArgs e)
        {
            {
                listView1.Items.Clear();

                // Pobierz śledzone przedmioty z bazy
                var trackedItems = await _databaseService.GetTrackedItemsAsync();

                foreach (var tracked in trackedItems)
                {
                    // Utwórz nowy wiersz (rząd) z nazwą przedmiotu
                    var itemName = tracked.Item?.Name ?? "[Brak nazwy]";
                    var row = new ListViewItem(itemName);

                    // Dodaj kolejne kolumny (miasto i czas śledzenia)
                    row.SubItems.Add(tracked.Location);

                    listView1.Items.Add(row);
                }
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void deleteButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Zaznacz przynajmniej jeden element do usunięcia.");
                return;
            }

            foreach (ListViewItem selected in listView1.SelectedItems)
            {
                string itemName = selected.SubItems[0].Text;
                string location = selected.SubItems[1].Text;

                // Pobierz wszystkie śledzone elementy z bazy
                var trackedItems = await _databaseService.GetTrackedItemsAsync();

                // Znajdź dopasowany element (uwzględniając nazwę i lokalizację)
                var trackedItem = trackedItems.FirstOrDefault(t =>
                    t.Item != null &&
                    t.Item.Name == itemName &&
                    t.Location == location);

                if (trackedItem != null)
                {
                    await _databaseService.RemoveTrackedItemAsync(trackedItem);
                    listView1.Items.Remove(selected);
                }
            }

            MessageBox.Show("Usunięto zaznaczone przedmioty.");
        }
    }
}
