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
    }
}
