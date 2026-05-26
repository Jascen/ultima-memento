using Server.Items;
using Server.Prompts;

namespace Server.Engines.Craft
{
	public class CraftSearchPrompt : Prompt
	{
		private readonly Mobile m_From;
		private readonly CraftSystem m_CraftSystem;
		private readonly BaseTool m_Tool;

		public static void Begin( Mobile from, CraftSystem craftSystem, BaseTool tool )
		{
			if ( from == null || craftSystem == null || tool == null )
				return;

			from.SendMessage( "Enter item name to search (ESC to cancel):" );
			from.Prompt = new CraftSearchPrompt( from, craftSystem, tool );
		}

		private CraftSearchPrompt( Mobile from, CraftSystem craftSystem, BaseTool tool )
		{
			m_From = from;
			m_CraftSystem = craftSystem;
			m_Tool = tool;
		}

		public override void OnResponse( Mobile from, string text )
		{
			if ( from != m_From )
				return;

			text = text != null ? text.Trim() : string.Empty;

			if ( text.Length == 0 )
			{
				ReopenGump( "Enter a search term." );
				return;
			}

			CraftContext context = m_CraftSystem.GetContext( m_From );

			if ( context == null )
			{
				ReopenGump( null );
				return;
			}

			context.ClearSearch();
			context.SearchResults.AddRange( CraftGump.SearchCraftItems( m_CraftSystem, text ) );
			context.SearchTerm = text;
			context.LastGroupIndex = CraftGump.SearchGroupIndex;

			string notice = context.SearchResults.Count == 0
				? "No matching items."
				: string.Format( "Found {0} item(s).", context.SearchResults.Count );

			m_From.SendGump( new CraftGump( m_From, m_CraftSystem, m_Tool, notice ) );
		}

		public override void OnCancel( Mobile from )
		{
			if ( from != m_From )
				return;

			ReopenGump( null );
		}

		private void ReopenGump( string notice )
		{
			m_From.SendGump( new CraftGump( m_From, m_CraftSystem, m_Tool, notice ) );
		}
	}
}
