using BlazorTrackerUI.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace BlazorTrackerUI.Pages.Prize
{
    public partial class Create
    {
        private DisplayPrizeModel prize = new DisplayPrizeModel();
        private PrizeModel prizeToDB = new PrizeModel();
        [Parameter]
        public bool messageBox { get; set; } = false;

        private bool IsValid(DisplayPrizeModel model)
        {
            bool output = true;
            if (model.PrizeAmount > 0 && model.PrizePercentage > 0 ||
                model.PrizeAmount == 0 && model.PrizePercentage == 0)
            {
                output = false;
            }
            return output;
        }

        private void HandleValidSubmit()
        {
            if (IsValid(prize))
            {
                messageBox = false;
                prizeToDB.PlaceNumber = prize.PlaceNumber;
                prizeToDB.PlaceName = prize.PlaceName;
                prizeToDB.PrizeAmount = prize.PrizeAmount;
                prizeToDB.PrizePercentage = prize.PrizePercentage;

                prizeData.CreatePrize(prizeToDB);

                navigationManager.NavigateTo("/"); // To the home page for now!
            }
            else
            {
                messageBox = true;
            }

        }
    }
}
