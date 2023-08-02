using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Pacer.Data.Extensions
{
    public struct PaceTime
    {
        public int Minutes { get; }
        public int Seconds { get; }

        public PaceTime(TimeSpan timeSpan)
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
