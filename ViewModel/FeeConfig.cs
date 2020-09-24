namespace FeeCalculator.ViewModel
{ 
    public class FeeConfig
    {
        public long MinAmount { get; set; }
        public long MaxAmount { get; set; }
        public long FeeAmount { get; set; }
    }
    public class FeesArray
    {
            public FeeConfig[] Fees { get; set; }
    }

}