using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.MLQuests.Rewards
{
	public class ItemReward : BaseReward
	{
		private Type m_Type;
		private int m_Amount;

		public ItemReward()
			: this( null, null )
		{
		}

		public ItemReward( TextDefinition name, Type type )
			: this( name, type, 1 )
		{
		}

		public ItemReward( TextDefinition name, Type type, int amount )
			: base( name )
		{
			m_Type = type;
			m_Amount = amount;
		}

		public virtual Item CreateItem()
		{
			Item spawnedItem = null;

			try
			{
				spawnedItem = Activator.CreateInstance( m_Type ) as Item;
			}
			catch ( Exception e )
			{
				if ( MLQuestSystem.Debug )
					Console.WriteLine( "WARNING: ItemReward.CreateItem failed for {0}: {1}", m_Type, e );
			}

			return spawnedItem;
		}

		public override void AddRewardItems( PlayerMobile pm, List<Item> rewards )
		{
			Item reward = CreateItem();

			if ( reward == null )
				return;

			if ( reward.Stackable )
			{
				if ( m_Amount > 1 )
					reward.Amount = m_Amount;

				rewards.Add( reward );
			}
			else
			{
				for ( int i = 0; i < m_Amount; ++i )
				{
					rewards.Add( reward );

					if ( i < m_Amount - 1 )
					{
						reward = CreateItem();

						if ( reward == null )
							return;
					}
				}
			}
		}
	}
}
