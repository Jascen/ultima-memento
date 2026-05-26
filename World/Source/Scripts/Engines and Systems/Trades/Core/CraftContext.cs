using System;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
	public class CraftContext
	{
		private List<CraftItem> m_Items;

        public List<CraftItem> Items { get { return m_Items; } }
        public List<CraftItem> SearchResults { get; private set; }
		public string SearchTerm { get; set; }
        public int LastResourceIndex { get; set; }
        public int LastResourceIndex2 { get; set; }
        public int LastGroupIndex { get; set; }
        public bool DoNotColor { get; set; }
        public CraftResource CraftingResource { get; set; }
        public string Description { get; set; }
        public Type ItemSelected { get; set; }
        public int ItemID {get; set;}
        public int Hue { get; set; }
        public string NameString { get; set; }
        public bool AutomaticallyMarkQuestItems { get; set;}

		public CraftContext()
		{
			m_Items = new List<CraftItem>();
			SearchResults = new List<CraftItem>();
			LastResourceIndex = -1;
			LastResourceIndex2 = -1;
			LastGroupIndex = -1;
			CraftingResource = CraftResource.None;
			Description = null;
			ItemSelected = null;
			ItemID = 0;
			Hue = 0;
			NameString = null;
			SearchTerm = null;
		}

		public CraftItem LastMade
		{
			get
			{
				if ( m_Items.Count > 0 )
					return m_Items[0];

				return null;
			}
		}

		public void ClearSearch()
		{
			SearchResults.Clear();
			SearchTerm = null;
		}

		public void OnMade( CraftItem item )
		{
			m_Items.Remove( item );

			if ( m_Items.Count == 10 )
				m_Items.RemoveAt( 9 );

			m_Items.Insert( 0, item );
		}
	}
}