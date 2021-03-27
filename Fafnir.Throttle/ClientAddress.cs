using System;

namespace Fafnir.Throttle
{
    public class ClientAddress
    {
        public string Address { get; set; }
        public int RquestsCount { get; private set; }
        public DateTime LastRequest { get; private set; }
        public bool IsBan { get; private set; }

        public void Ban() => IsBan = true;
        public void Clear()
        {
            IsBan = false;
            RquestsCount = 0;
        }
        public void IncreaseCounter()
        {
            RquestsCount++;
            LastRequest = DateTime.Now;
        }

        public bool EndBan(TimeSpan penalty) => LastRequest < DateTime.Now.Subtract(penalty);
        public bool ShouldClear(TimeSpan period) => LastRequest < DateTime.Now.Subtract(period) && !IsBan;
    }
}
