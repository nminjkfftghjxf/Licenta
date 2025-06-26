using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace plante2._3
{
    public partial class Test:Popup
    {
        private readonly PlantInfo remove_selected;
        private readonly Action<PlantInfo> delete; 
        public Test(PlantInfo selectedPlant, Action<PlantInfo> Delete)
        {
            InitializeComponent();
  
            Image.Source = selectedPlant.Image;
            AnotherCommon.Text = $"{selectedPlant.Name}";
            AnotherFamily.Text = $"{selectedPlant.Family}";
            AnotherCycle.Text = $"{selectedPlant.Cycle}";
            AnotherWater.Text = $"{selectedPlant.Watering}";
            AnotherSun.Text = $"{selectedPlant.Sunlight}";
            AnotherO.Text= $"{selectedPlant.Origin}";
            AnotherT.Text= $"{selectedPlant.Type}";
            AnotherP.Text= $"{selectedPlant.PruningMonth}";
            AnotherWM.Text= $"{selectedPlant.Wateringmark}";
            AnotherA.Text = $"{selectedPlant.Attracts}";
            AnotherFS.Text = $"{selectedPlant.FloweringSeason}";
            AnotherPh.Text= $"{selectedPlant.PH}";
            AnotherPp.Text= $"{selectedPlant.PP}";
            AnotherWP.Text= $"{selectedPlant.WateringPeriod}";
            AnotherE.Text = $"{selectedPlant.EdibleP}";


            Debug.WriteLine($"AnotherFS = {selectedPlant.FloweringSeason}");
            Debug.WriteLine($"AnotherA = {selectedPlant.Attracts}");

            remove_selected = selectedPlant;
            delete = Delete;
        }
        private void OnCloseClicked(object sender, EventArgs e)
        {
            CloseAsync();
        }
        private async void RemovePlant(object sender,EventArgs e)
        {
            bool question=await Application.Current.MainPage.DisplayAlert("Hello,you are about to delete your plant!", "Are you sure about it?", "Yes", "No");

            if(question)
            {
                delete?.Invoke(remove_selected);
                await CloseAsync();
            }
        }

    }
}
