namespace sevenA.Core.Stats
{
    public struct LinearFitCoefficients
    {
        public double A;
        public double B;
        public double R2;
        public double Sigma;
    }

    public struct LinearFit
    {
        public double Predicted;
        public double Lower1Sigma;
        public double Upper1Sigma;
    }
}