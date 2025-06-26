using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Extensions;
using GoogleGson;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using static Android.App.DownloadManager;
using static Android.Provider.Settings;


namespace plante2._3
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            ViewPlant();
        }

        private int count = 0;

        private async void MainRedirect(object sender,EventArgs e)
        {
            count++;
            if (count == 3)
            {
                count = 0;
                await DisplayAlert("Hello!", "You are currectly in Home Page", "OK");
            }
        }

        private async void GoToSettings(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Settings(this));
        }


        private string? lastname = null;
        private string? common = null;
        private string? gen = null;

        private string? currentImagePath;

        string file1 = Path.Combine(FileSystem.AppDataDirectory, $"saved_perenual.json");

        public ObservableCollection<PlantInfo> SavedPlants { get; set; } = new();
        public ObservableCollection<PlantInfo> PlanteFiltrate { get; set; } = new();

        public async void CaptureClicked(object? sender, EventArgs e)
        {
            var image = await MediaPicker.CapturePhotoAsync();


            var imagefile = $"{Path.GetFileNameWithoutExtension(image.FileName)}_{DateTime.Now.Ticks}.jpg";
            var ilp = Path.Combine(FileSystem.AppDataDirectory, imagefile);

            using (var sourceStream = await image.OpenReadAsync())
            using (var destinationStream = File.OpenWrite(ilp))
            {
                await sourceStream.CopyToAsync(destinationStream);
            }

            currentImagePath = ilp;


            if (image == null)
            {
                await DisplayAlert("Error", "Image could not be found!", "?!");
                return;
            }

            using var stream = await image.OpenReadAsync();
            using var content = new MultipartFormDataContent
             {
                 { new StreamContent(stream), "images", image.FileName }
             };


           
            string apiKey = "2b10eckCBJylwMwviKoO9fYCe";
            string url = $"https://my-api.plantnet.org/v2/identify/all?include-related-images=true&no-reject=true&nb-results=1&lang=en&type=legacy&api-key={apiKey}";
            using var client = new HttpClient();
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                //await DisplayAlert("Debug", json, "OK");


                var results = doc.RootElement.GetProperty("results");

                var name = doc.RootElement.GetProperty("results")[0].GetProperty("species").GetProperty("scientificName").GetString();
                var commonName = doc.RootElement.GetProperty("results")[0].GetProperty("species").GetProperty("commonNames")[0].GetString();


                var family = doc.RootElement.GetProperty("results")[0].GetProperty("species").GetProperty("family").GetProperty("scientificNameWithoutAuthor").GetString();

                var genus = doc.RootElement.GetProperty("results")[0].GetProperty("species").GetProperty("genus").GetProperty("scientificNameWithoutAuthor").GetString();

                var organ = doc.RootElement.GetProperty("query").GetProperty("organs")
                .EnumerateArray()
                .Select(o => o.GetString())
                .Where(o => !string.IsNullOrEmpty(o) && o != "auto")
                .ToList();
                var organdisplay = string.Join(", ",organ.Select(o =>
                 o == "leaf" ? "🍃 Leaf" :
                 o == "flower" ? "🌸 Flower" :
                 o == "fruit" ? "🍏 Fruit" :
                 o == "stem" ? "🌱 Stem" :
                 o == "bark" ? "🌲 Bark" :
                 o == "seed" ? "🌾 Seed" :
                 o == "autumnLeaves" ? "🍂 Autumn Leaves" :
                         "🪴 " + o ));



                var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                lastname = string.Join(" ", parts.Take(2));
                common = commonName;
                gen = genus;

                
                await DisplayAlert("Plant Details",
                    $"\n🌸Common Name: {commonName}\n"+
                    $"\n🌿Scientific Name: {lastname}\n " +
                    $"\n🌱Genus: {genus}\n " +
                    $"\n🌳Family: {family}\n" +
                    $"\n{organdisplay}", "OK");

                bool save = await DisplayAlert("New Plant Discovered!", "Would you like to add to garden?", "Yes","No");
                if(save)
                {

                    await FindPlant();
                }
            }
            else
            {
                await DisplayAlert("Error", "PlantNet failed once again!", "OK");
            }

        }

        public async Task FindPlant()
        {
            //var start = DateTime.Now;
            string key = "sk-H0Ws684fd4ce2fd9510690";
            string query1 = Uri.EscapeDataString(gen);
            string query2 = Uri.EscapeDataString(common);
            string query3 = Uri.EscapeDataString(lastname);

            Console.WriteLine($"Query1: {query1}");
            Console.WriteLine($"Query2: {query2}");
            Console.WriteLine($"Query3: {query3}");


            string url1 = $"https://perenual.com/api/v2/species-list?key={key}&q={query1}";
            string url2 = $"https://perenual.com/api/v2/species-list?key={key}&q={query2}";
            string url3 = $"https://perenual.com/api/v2/species-list?key={key}&q={query3}";

            using var client = new HttpClient();
            using var client2 = new HttpClient();
            using var client3 = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(url1);
            //HttpResponseMessage response2 = await client2.GetAsync(url2);
            //HttpResponseMessage response3 = await client3.GetAsync(url2);

                await Task.Delay(3000);

            if(!response.IsSuccessStatusCode)
            {
                response = await client.GetAsync(url2);
                await Task.Delay(3000);
            }
            if(!response.IsSuccessStatusCode)
            {
                response = await client.GetAsync(url3);
            }
            if(!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Dang!", "All Api calls to identify perenual plant id failed", "OK");
            }


            if (response.IsSuccessStatusCode)
            {

                var Json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(Json);
                Debug.WriteLine($"📂 Full API Response: {await response.Content.ReadAsStringAsync()}");


                if (!doc.RootElement.TryGetProperty("data", out System.Text.Json.JsonElement data) || data.GetArrayLength() == 0)
                {
                    await DisplayAlert("❌ Error", "DATA  IN FINDPLANT EMPTY!", "OK");
                    return;
                }

                //var plantinfo = doc.RootElement.GetProperty("data")[0];
                int id = data[0].GetProperty("id").GetInt32();
                Debug.WriteLine($"✅ Retrieved ID from API: {id}");

                //id = 25;

               // var durata = DateTime.Now - start;
                //Debug.WriteLine($"Timpul de răspuns: {durata.TotalMilliseconds} ms");

                await SavePlant(id);

            }
            else
            {
                await DisplayAlert("I'm sorry", "Could not retrieve plant id from Perenual", "OK");
            }
        }

        public async Task SavePlant(int id)
        {
            string key = "sk-H0Ws684fd4ce2fd9510690";
            string url = $"https://perenual.com/api/v2/species/details/{id}?key={key}";

            //string get = "https://perenual.com/api/v2/species/details/{id}?key=sk-H0Ws684fd4ce2fd9510690";

            using var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);


            if (response.IsSuccessStatusCode)
            {
                //Debug.WriteLine($"KILL ME!!!");
                var Json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"recieved id from perenual find plant: {Json}");
                Debug.WriteLine($"📂 API Response: {Json}");


                await Task.Delay(1000);
               using var doc = JsonDocument.Parse(Json);
               Debug.WriteLine($"Json parsed successfully!");
                
                await Task.Delay(1000);

                var plantInfo = doc.RootElement;

                string? perenualName = plantInfo.TryGetProperty("common_name", out System.Text.Json.JsonElement name) &&
                name.ValueKind != JsonValueKind.Null ? name.GetString() : "Unknown Name";

                string? perenualFamily = plantInfo.TryGetProperty("family", out System.Text.Json.JsonElement family) &&
                family.ValueKind != JsonValueKind.Null ? family.GetString() : "Unknown Family";

                string? perenualCycle = plantInfo.TryGetProperty("cycle", out System.Text.Json.JsonElement cycle) &&
                cycle.ValueKind != JsonValueKind.Null ? cycle.GetString() : "Unknown Cycle";

                string? wateringNeeds = plantInfo.TryGetProperty("watering", out System.Text.Json.JsonElement watering) &&
                watering.ValueKind != JsonValueKind.Null ? watering.GetString() : "Unknown Cycle";

                string sunlightNeeds = string.Join(", ", plantInfo.TryGetProperty("sunlight", out System.Text.Json.JsonElement sunlight) 
                    && sunlight.ValueKind == JsonValueKind.Array
                     ? string.Join(", ", sunlight.EnumerateArray().Select(s => s.GetString()))
                      : "No Sunlight Info");

                string? perenualOrigin = plantInfo.TryGetProperty("origin", out var origin) && origin.ValueKind == JsonValueKind.Array
                ? string.Join(", ", origin.EnumerateArray().Select(o => o.GetString())): "Unknown";


                string? perenualType = plantInfo.TryGetProperty("type", out System.Text.Json.JsonElement type) ? type.GetString() : "Unknown";

                string? perenualWateringmark = plantInfo.TryGetProperty("watering_general_benchmark", out var mark) && mark.ValueKind == JsonValueKind.Object &&
                 mark.TryGetProperty("value", out var value) &&mark.TryGetProperty("unit", out var unit)
                 ? $"{value.GetString()} {unit.GetString()}": "Unknown";

                string perenualpruningMonth = plantInfo.TryGetProperty("pruning_month", out System.Text.Json.JsonElement pruning) &&
                      pruning.ValueKind == JsonValueKind.Array? string.Join(", ", pruning.EnumerateArray().Select(m => m.GetString())): "Unknown";


                string perenualAttracts = plantInfo.TryGetProperty("attracts", out System.Text.Json.JsonElement attracts) &&
                  attracts.ValueKind == JsonValueKind.Array ? string.Join(", ", attracts.EnumerateArray().Select(a => a.GetString())): "None";
                if(string.IsNullOrWhiteSpace(perenualAttracts))
                {
                    perenualAttracts = "None";
                }


                string? perenualfS = plantInfo.TryGetProperty("flowering_season", out System.Text.Json.JsonElement fS)
                ? fS.GetString() : "Whenever";

                if (string.IsNullOrWhiteSpace(perenualfS))
                {
                    perenualfS = "Whenever";
                }

                bool perenualpoisonousH = plantInfo.TryGetProperty("poisonous_to_humans", out var poisonH) && poisonH.GetBoolean();

                bool perenualpoisonousP = plantInfo.TryGetProperty("poisonous_to_pets", out var poisonP) && poisonP.GetBoolean();

                string PerenualwateringTime = plantInfo.TryGetProperty("xWateringPeriod", out System.Text.Json.JsonElement wt) &&
                        wt.ValueKind == JsonValueKind.Array? string.Join(", ", wt.EnumerateArray().Select(p => p.GetString())): "Anytime";

                List<string> edibleP = new();
                if (plantInfo.TryGetProperty("edible_fruit", out System.Text.Json.JsonElement edibleFruit) && edibleFruit.GetBoolean())
                    edibleP.Add("Fruit");
                if (plantInfo.TryGetProperty("edible_leaf", out System.Text.Json.JsonElement edibleLeaf) && edibleLeaf.GetBoolean())
                    edibleP.Add("Leaf");
                string ediblePJ = edibleP.Count > 0 ? string.Join(", ", edibleP) : "None";

                string? user = await SecureStorage.GetAsync("current_user") ?? "Guest";

                Debug.WriteLine(perenualAttracts);
                Debug.WriteLine(perenualfS);

                var AddNew = new PlantInfo
                {
                    Name = perenualName,
                    Family = perenualFamily,
                    Cycle = perenualCycle,
                    Watering = wateringNeeds,
                    Sunlight = sunlightNeeds,
                    Time=DateTime.UtcNow,
                    Image = currentImagePath,
                    Account = user,
                    Origin=perenualOrigin,
                    Type=perenualType,
                    Wateringmark=perenualWateringmark,
                    PruningMonth=perenualpruningMonth,
                    Attracts = perenualAttracts,
                    FloweringSeason = perenualfS,
                    PH=perenualpoisonousH,
                    PP=perenualpoisonousP,
                    WateringPeriod=PerenualwateringTime,
                    EdibleP=ediblePJ
                };

                Debug.WriteLine($"✅ AccountPlantPathAsync() executed, path: {file1}");

                List<PlantInfo> savedPlants = new();
                if (File.Exists(file1))
                {
                    string existingJson = await File.ReadAllTextAsync(file1);
                    Debug.WriteLine($"🗂 Salvăm în: {file1}");

                    savedPlants = JsonSerializer.Deserialize<List<PlantInfo>>(existingJson) ?? new List<PlantInfo>();
                }
                savedPlants.Add(AddNew);
                SavedPlants.Add(AddNew);

                string updatedJson = JsonSerializer.Serialize(savedPlants);
                await File.WriteAllTextAsync(file1, updatedJson);
                await DisplayAlert("Yeyy!", "New Plant Added!","Wonderful");
            }
            else
            {
                // await DisplayAlert("Failed", "To SAVE PERENUAL PLANT", "OK");
                if (id > 3000)
                {
                    await DisplayAlert("I'm sorry", "You have free access to species 1-3000 from Perenual", "OK");
                }
                else
                {

                    var errorContent = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine($"PERENUAL FAILED!!! \nStatus: {response.StatusCode}");

                    await DisplayAlert("I'm sorry!", $"You have reached the limit of 100 saved plants per day", "Ok");
                }
            }
        }

        private async void ViewPlant()
        {
            string? user = await SecureStorage.GetAsync("current_user") ?? "Guest";

            if (File.Exists(file1))
            { 

                string existingJson = await File.ReadAllTextAsync(file1);

                var list = JsonSerializer.Deserialize<List<PlantInfo>>(existingJson);

                if (list != null)
                {

                    var LIST = list.Where(p => p.Account == user).ToList();

                    Debug.WriteLine($"Loaded {list?.Count ?? 0} total plants");
                    Debug.WriteLine($"User: {user}, Showing: {LIST.Count} plants");

                    SavedPlants.Clear();
                    foreach (var plant in LIST)
                     {
                       
                        SavedPlants.Add(plant);
                               
                     }
                    FiltreazaLista("All"); 
                }
            }
        }
        private void All(object sender, EventArgs e) => FiltreazaLista("All");
        private void Name_A(object sender, EventArgs e) => FiltreazaLista("Name-A");

        private void Family(object sender, EventArgs e) => FiltreazaLista("Family");
        private void Recents(object sender, EventArgs e) => FiltreazaLista("Recents");
        private void Edible(object sender, EventArgs e) => FiltreazaLista("Edible");


        public void FiltreazaLista(string filtru)
        {
            PlanteFiltrate.Clear();
            IEnumerable<PlantInfo> opt = SavedPlants;

            switch(filtru)
            {
                case "All":
                    opt = SavedPlants;
                    break;

                case "Name-A":
                    opt = SavedPlants.OrderBy(p => p.Name);
                    break;

                case "Family":
                    opt = SavedPlants.OrderBy(p => p.Family);
                    break;

                case "Recents":
                    opt = SavedPlants.Where(p => p.Time > DateTime.MinValue).OrderByDescending(p => p.Time);
                    break;

                case "Edible":
                    opt = SavedPlants.Where(p => !string.IsNullOrWhiteSpace(p.EdibleP) && p.EdibleP != "None");
                    break;

                default:
                    opt = SavedPlants;
                    break;
            }

            foreach(var plant in opt)
            {
                PlanteFiltrate.Add(plant);
            }

        }


        private async void FullyRemovePlant(PlantInfo delete)
        {
            string? user = await SecureStorage.GetAsync("current_user") ?? "Guest";

            SavedPlants.Remove(delete);
            List<PlantInfo> existingPlants = new();
            if (File.Exists(file1))
            {
                string json = await File.ReadAllTextAsync(file1);
                existingPlants = JsonSerializer.Deserialize<List<PlantInfo>>(json) ?? new List<PlantInfo>();
            }
                 existingPlants = existingPlants
                .Where(p => !(p.Name == delete.Name && p.Family == delete.Family && p.Image == delete.Image &&p.Account== user))
                .ToList();

            string newJson = JsonSerializer.Serialize(existingPlants);
            await File.WriteAllTextAsync(file1, newJson);
        }

        private async void OnDotsTapped(object sender, EventArgs e)
        {
            if (sender is Image image && image.BindingContext is PlantInfo selectedPlant)
            {
                var p = new Test(selectedPlant ,FullyRemovePlant);
                await Shell.Current.ShowPopupAsync(p);
            }
        }

        private DateTime lasttap;

        protected override bool OnBackButtonPressed()
        {
            if ((DateTime.Now - lasttap).TotalMilliseconds < 500)
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                return true;
            }
            lasttap = DateTime.Now;
            Android.Widget.Toast.MakeText(Android.App.Application.Context, "Double tap to exit", Android.Widget.ToastLength.Short).Show();
            return true;
        }

    }
}
