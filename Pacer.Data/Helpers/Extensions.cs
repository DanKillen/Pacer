namespace Pacer.Data.Extensions
{
    public struct PaceTimeSpan
    {
        public int Minutes { get; }
        public int Seconds { get; }

        public PaceTimeSpan(TimeSpan timeSpan)
        {
            Minutes = timeSpan.Minutes;
            Seconds = timeSpan.Seconds;
        }

        public override string ToString()
        {
            return $"{Minutes:00}:{Seconds:00}";
        }
    }

}
