namespace Server.Items
{
	public static class TrapPotionEffects
	{
		public static bool IsAllowed( BasePotion potion )
		{
			if ( potion == null || potion.Deleted ) return false;

			switch ( potion.PotionEffect )
			{
				case PotionEffect.ExplosionLesser:
				case PotionEffect.Explosion:
				case PotionEffect.ExplosionGreater:
				case PotionEffect.Conflagration:
				case PotionEffect.ConflagrationGreater:
				case PotionEffect.Frostbite:
				case PotionEffect.FrostbiteGreater:
				case PotionEffect.PoisonLesser:
				case PotionEffect.Poison:
				case PotionEffect.PoisonGreater:
				case PotionEffect.PoisonDeadly:
				case PotionEffect.PoisonLethal:
				case PotionEffect.ConfusionBlast:
				case PotionEffect.ConfusionBlastGreater:
					return true;

				default:
					return false;
			}
		}

		public static bool TryCoat( TrapKit kit, Mobile from, BasePotion potion )
		{
			if ( kit == null || kit.Deleted || from == null || potion == null || potion.Deleted ) return false;

			if ( !kit.IsChildOf( from.Backpack ) || !potion.IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
				return false;
			}

			if ( kit.HasSecondaryEffect )
			{
				from.SendMessage( "This kit is already coated with something." );
				return false;
			}

			if ( !IsAllowed( potion ) )
			{
				from.SendMessage( "That potion cannot be used with trapping tools." );
				return false;
			}

			kit.SetSecondaryEffect( potion.PotionEffect );			
			potion.Consume();
			BasePotion.PlayDrinkEffect( from );
			from.SendMessage( "You coat the trapping tools with the potion." );

			return true;
		}

		public static string GetEffectDisplayName( PotionEffect effect )
		{
			switch ( effect )
			{
				case PotionEffect.ExplosionLesser: return "a slightly explosive liquid";
				case PotionEffect.Explosion: return "a moderately explosive liquid";
				case PotionEffect.ExplosionGreater: return "an extremely explosive liquid";
				
				case PotionEffect.Conflagration: return "a flammable liquid";
				case PotionEffect.ConflagrationGreater: return "an extremely flammable liquid";
				
				case PotionEffect.Frostbite: return "a freezing liquid";
				case PotionEffect.FrostbiteGreater: return "an extremely freezing liquid";
				
				case PotionEffect.PoisonLesser: return "a barely poisonous liquid";
				case PotionEffect.Poison: return "a slightly poisonous liquid";
				case PotionEffect.PoisonGreater: return "a moderately poisonous liquid";
				case PotionEffect.PoisonDeadly: return "a deadly poisonous liquid";
				case PotionEffect.PoisonLethal: return "a lethally poisonous liquid";

				case PotionEffect.ConfusionBlast: return "a calming liquid";
				case PotionEffect.ConfusionBlastGreater: return "a very calming liquid";

				default: return "an unknown liquid";
			}
		}

		public static void ApplySecondary( PotionEffect effect, Mobile owner, Mobile victim, Point3D trapLoc, Map map )
		{
			if ( map == null || map == Map.Internal || victim == null || victim.Deleted ) return;

			var potion = PotionKeg.CreatePotion( effect );
			if ( potion == null ) return;

			try
			{
				var explosion = potion as BaseExplosionPotion;

				if ( explosion != null )
				{
					explosion.Explode( owner, true, trapLoc, map, false );
					return;
				}

				var conflagration = potion as BaseConflagrationPotion;
				if ( conflagration != null )
				{
					conflagration.Explode( owner, trapLoc, map, false );
					return;
				}

				var frostbite = potion as BaseFrostbitePotion;
				if ( frostbite != null )
				{
					frostbite.Explode( owner, trapLoc, map, false );
					return;
				}

				var confusion = potion as BaseConfusionBlastPotion;
				if ( confusion != null )
				{
					confusion.Explode( owner, trapLoc, map, false );
					return;
				}

				var poison = potion as BasePoisonPotion;
				if ( poison != null )
				{
					if ( owner != null )
						owner.DoHarmful( victim );

					victim.ApplyPoison( owner, poison.Poison );
					return;
				}
			}
			finally
			{
				potion.Delete();
			}
		}
	}
}
