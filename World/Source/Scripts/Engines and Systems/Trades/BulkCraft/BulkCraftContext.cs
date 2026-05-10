namespace Server.Engines.Craft
{
    public class BulkCraftContext
    {
        public int Success { get; set; }
        public int Exceptional { get; set; }
        public int Fail { get; set; }
        public int Current { get; set; }
        public int Amount { get; set; }
        public bool Cancelled { get; set; }
        public bool Suppressed { get; set; }
        public bool Paused { get; set; }
		public object LastMessage { get; set; }
    }
}