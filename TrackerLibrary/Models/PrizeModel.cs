using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrackerLibrary.Models
{
    public class PrizeModel
    {
        public int Id { get; set; }

        [Required]  // Install System.ComponentModel.DataAnnotations package
        [Range(1, 9999, ErrorMessage = "Place number must be between 1 and 9999")]
        public int PlaceNumber { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "No more than 50 character")]
        public string PlaceName { get; set; }

        [Range(0,10000, ErrorMessage = "Should be between (1-10000)")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "No more than 2 deciaml places")]
        public decimal PrizeAmount { get; set; }

        [Range(0,100)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Should be a whole number (1-100)")]
        //[RegularExpression(@"\d+(\.\d{1,2})?")] // stackoverflow 2012
        // [RegularExpression(@"^\$?\d+(\.(\d{2}))?$")] //microsoft 2009
        public double PrizePercentage { get; set; }
      

        public PrizeModel()
        {

        }
        public PrizeModel(string placeName, string placeNumber, string prizeAmount, string prizePercentage)
        {
            PlaceName = placeName;

            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;
        }
    }
}
