using System.Windows.Forms;
using AlbionConsole.Models;
using AlbionConsole.Services;

namespace AlbionGui
{
    public partial class Form1 : Form
    {
        private readonly DatabaseService _databaseService;
        private readonly ItemImporter _itemImporter;
        private readonly TrackedItemsForm _trackedItemsForm;
        public Form1()
        {
            InitializeComponent();
        }
        public Form1(DatabaseService databaseService, ItemImporter itemImporter, TrackedItemsForm trackedItemsForm)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _itemImporter = itemImporter;
            _trackedItemsForm = trackedItemsForm;
        }
        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void addButton_Click(object sender, EventArgs e)
        {
            // Sprawdzenie, czy zaznaczono miasta
            if (citiesList.CheckedItems.Count == 0)
            {
                MessageBox.Show("Zaznacz przynajmniej jedno miasto.");
                return;
            }

            // Sprawdzenie, czy zaznaczono przedmioty
            if (itemsList.CheckedItems.Count == 0)
            {
                MessageBox.Show("Zaznacz przynajmniej jeden przedmiot.");
                return;
            }

            int addedCount = 0;

            // Dla ka�dego wybranego miasta
            foreach (var selectedCityRaw in citiesList.CheckedItems)
            {
                // Pobieramy nazw� miasta jako tekst
                string selectedCity = selectedCityRaw.ToString();

                // Normalizujemy nazw� miasta (np. usu� spacje, popraw wielko�� liter)
                string normalizedCity = City.NormalizeName(selectedCity);

                // Je�li miasto nie jest poprawne, pomi�
                if (!City.IsValid(normalizedCity))
                {
                    MessageBox.Show($"Miasto \"{selectedCity}\" jest niepoprawne. {City.GetSuggestion(selectedCity)}");
                    continue;
                }

                // Dla ka�dego zaznaczonego przedmiotu
                foreach (var selectedItem in itemsList.CheckedItems)
                {
                    // Sprawdzenie, czy element to faktycznie Item
                    if (selectedItem is Item item)
                    {
                        var trackedItem = new TrackedItem
                        {
                            ItemId = item.ItemId,
                            Location = normalizedCity,
                            Item = item
                        };

                        // Zapisz do bazy (brak duplikat�w, je�li obs�u�ysz to w bazie lub metodzie Save)
                        await _databaseService.SaveTrackedItemAsync(trackedItem);
                        addedCount++;
                    }
                }
            }

            // Informacja zwrotna o liczbie dodanych powi�za�
            MessageBox.Show($"Dodano {addedCount} wpis(�w) do �ledzenia.");


        }

        private void searchingButton_Click(object sender, EventArgs e)
        {
            try {
                _trackedItemsForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("B��d: " + ex.Message);
            }
        }

        private async void findButton_Click(object sender, EventArgs e)
        {
            var searchTerm = textBox1.Text;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                MessageBox.Show("Please enter a search term.");
                return;
            }

            var items = await _databaseService.GetItemsAsync();
            Console.WriteLine($"W bazie: {items.Count} przedmiot�w");

            var matchingItems = items
                .Where(i => i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            i.UniqueName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!matchingItems.Any())
            {
                MessageBox.Show("No items found.");
                return;
            }

            itemsList.DisplayMember = "Name"; // <-- poka� nazw�
            itemsList.Items.Clear();

            foreach (var item in matchingItems)
            {
                itemsList.Items.Add(item); // <-- dodaj ca�y obiekt
            }
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            

        }

        private async void importButton_Click(object sender, EventArgs e)
        {
            importButton.Enabled = false;
            try
            {
                await _itemImporter.ImportAsync();
                MessageBox.Show("Import zako�czony!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("B��d: " + ex.Message);
            }
            finally
            {
                importButton.Enabled = true;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Czy�cimy wszystko, je�li co� ju� tam by�o
            citiesList.Items.Clear();

            // Dodajemy wszystkie dost�pne miasta z klasy City
            foreach (var city in City.ValidCities)
            {
                citiesList.Items.Add(city);
            }
        }
    }
}
