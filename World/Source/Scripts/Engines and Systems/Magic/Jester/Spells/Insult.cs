using System;
using Server.Targeting;
using System.Collections;

namespace Server.Spells.Jester
{
	public class Insult : JesterSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 264,
					IconGraphic = 20747,
					Name = "Insult",
					PowerWords = "You know what?",
					Description = "Get ready to deal a sharp tongued insult! Although your words won't have them slipping on their tears, it will demoralize them to the point where their mana will begin to fade as they reflect on their hurt feelings. The duration of the decline, as well as how much mana is lost each second, is dependent on your jester skills.",
					ManaCost = 60,
					TithingCost = 40,
					MinSkill = 10,
					TargetType = TargetFlags.Harmful
				},
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public Insult( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		private static Hashtable m_Table = new Hashtable();

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public static bool HasEffect( Mobile m )
		{
			return ( m_Table[m] != null );
		}

		public static void RemoveEffect( Mobile m )
		{
			Timer t = (Timer)m_Table[m];

			if ( t != null )
			{
				t.Stop();
				m_Table.Remove( m );
			}
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				int TotalLoss = Server.Spells.Jester.JesterSpell.Buff( Caster, "range" )+1;
				int TotalTime = (int)(Server.Spells.Jester.JesterSpell.Buff( Caster, "time" )/2)+1;

				Timer t = new InternalTimer( m, TotalLoss, TotalTime );

				m_Table[m] = t;

				t.Start();

				Caster.Say( GetInsult() );

				switch ( Utility.Random( 3 ))
				{
					case 0: Caster.PlaySound( Caster.Female ? 794 : 1066 ); break;
					case 1: Caster.PlaySound( Caster.Female ? 801 : 1073 ); break;
					case 2: Caster.PlaySound( 792 ); break;
				};

				if ( Utility.RandomBool() ){ Effects.PlaySound( m.Location, m.Map, m.Female ? 0x31B : 0x42B ); m.Say( "*groans*" ); }
				else { Effects.PlaySound( m.Location, m.Map, m.Female ? 0x338 : 0x44A ); m.Say( "*growls*" ); }

				m.SendMessage( "You have been quite insulted!" );
			}

			FinishSequence();
		}

		public static string GetInsult()
		{
			string str = "A goblin with one hand nailed to a tree would be more of a threat than you!";
			switch( Utility.RandomMinMax( 1, 100 ) )
			{
				case 1: str = "A goblin with one hand nailed to a tree would be more of a threat than you!"; break;
				case 2: str = "A wet cat is tougher than you!"; break;
				case 3: str = "Animal friendship magic was the only way your parents could get puppies to play with you!"; break;
				case 4: str = "Are you an orc crossed with a pig? Oh yeah, there are some things a pig wouldn't do!"; break;
				case 5: str = "Are you always stupid, or are you making a special effort today!"; break;
				case 6: str = "By looking at you, now I know what you get when you scrape out the bottom of the barrel!"; break;
				case 7: str = "By the gods you are ugly! I bet your father regrets meeting your mother!"; break;
				case 8: str = "Could you go get your husband? I don't like fighting ugly women!"; break;
				case 9: str = "Did your mother cast a darkness spell to feed you!"; break;
				case 10: str = "Didn't I hear that you were tougher than this?"; break;
				case 11: str = "Do you have a pen? Well you'd better get back to it before the farmer knows you are missing!"; break;
				case 12: str = "You look like something I saw on the stable floor!"; break;
				case 13: str = "Have you ever seen a pile of dung? Then maybe look in a mirror!"; break;
				case 14: str = "Even ghouls wouldn't touch something as gross as you!"; break;
				case 15: str = "Hey, have you ever been mistaken for a carcass worm?"; break;
				case 16: str = "Hey, you pox ridden dung heap, I bet not even a starving vampire would go near you!"; break;
				case 17: str = "How does it feel that you're not worthy of anyone casting a decent spell on you!"; break;
				case 18: str = "I can tell your reservoir of courage is fed by the tributary running down your leg!"; break;
				case 19: str = "I could say you're as ugly as an ogre, but that would be an insult to ogres!"; break;
				case 20: str = "I don't know whether to use a charm person spell or charm monster!"; break;
				case 21: str = "I heard what happened to your mother, it's not everyday your reflection kills you!"; break;
				case 22: str = "I swear, if you were any worse at this, you'd be doing my job for me!"; break;
				case 23: str = "I was going to cast read mind, but I don't think I'm going to find anything up there!"; break;
				case 24: str = "I was thinking of casting feeblemind, but I doubt it would work on you!"; break;
				case 25: str = "I was wondering what you are, you are fat enough to be an ogre, but I have never seen an ogre that ugly before!"; break;
				case 26: str = "I wish I still had that blindness spell, then I wouldn't have to endure that face anymore!"; break;
				case 27: str = "I would contact your mother about your death, but I don't speak goblin!"; break;
				case 28: str = "I would try to insult your father, but you were probably mistaken for an orc, and disowned!"; break;
				case 29: str = "I'd draw my sword, but I wouldn't want to make you jealous!"; break;
				case 30: str = "I'd insult your parents, but you probably don't know who they are!"; break;
				case 31: str = "I'd like to leave you with one thought...but I'm not sure you have anywhere to put it!"; break;
				case 32: str = "I'd like to see things from your point of view, but I can't get my head that far up my arse!"; break;
				case 33: str = "I'd say you were a worthy opponent, but I once fought a bunny wielding a dandelion!"; break;
				case 34: str = "If I were you, I'd go and get my gold back for that remove curse spell!"; break;
				case 35: str = "If ignorance is bliss, you must be the happiest one alive!"; break;
				case 36: str = "If this fight gets any harder, I'll have to actually try!"; break;
				case 37: str = "If your mind exploded, it wouldn't even mess up your hair!"; break;
				case 38: str = "I'm glad you're tall...It means there's more of you I can despise!"; break;
				case 39: str = "It gives me a headache just trying to think down to your level!"; break;
				case 40: str = "I've heard of goats with better fighting skills than you!"; break;
				case 41: str = "I've seen more threatening birds!"; break;
				case 42: str = "No treasure is worth having to look at you!"; break;
				case 43: str = "No wonder you're hiding behind cover, I'd hide too with a face like that!"; break;
				case 44: str = "Oh look, you are actually trying to fight!"; break;
				case 45: str = "I thought troglodytes smelled bad!"; break;
				case 46: str = "Why don't you give me your weapon so I can hit myself with it, because that'd be more effective than you trying it!"; break;
				case 47: str = "Did an ogre breath on me or is that you?"; break;
				case 48: str = "One day I'm going to make a story of this fight. Tell me your name, I hope it rhymes with horribly slaughtered!"; break;
				case 49: str = "Phew! Have you just cast stinking cloud or do you always smell like that!"; break;
				case 50: str = "Didn't your mother ever teach you how to fight?"; break;
				case 51: str = "I like how you pretend to fight!"; break;
				case 52: str = "Some day you'll go far and I hope you stay there!"; break;
				case 53: str = "Some day you'll meet a doppelganger of yourself and be disappointed!"; break;
				case 54: str = "Somewhere, you are depriving a village of it's idiot!"; break;
				case 55: str = "You look cuddly, or are you trying to be menacing?"; break;
				case 56: str = "Tell me, did you run away from your parents, or did they run away from you!"; break;
				case 57: str = "There is no beholder's eye in which you are beautiful!"; break;
				case 58: str = "They say every rose has its thorn, ain't that right, buttercup!"; break;
				case 59: str = "Ugh. What is that all over your face? Oh...its just your face!"; break;
				case 60: str = "Very impressive, I think I'll hire you out for a puppet show!"; break;
				case 61: str = "When the gods were handing out ugly faces, were you first in line?"; break;
				case 62: str = "Wait, wait, I just need to ask, what do you need me to put on your headstone!"; break;
				case 63: str = "Well, my time of not taking you seriously is coming to a middle!"; break;
				case 64: str = "Well...I have met sharper loaves of bread!"; break;
				case 65: str = "We're you once hit by an acid elemental or have you always looked like a half eaten steak?"; break;
				case 66: str = "What smells worse than a goblin? Oh yeah, you!"; break;
				case 67: str = "What's that smell? I thought breath weapons were suppose to come out of your mouth!"; break;
				case 68: str = "What's the difference between you and a sick bunny? The bunny could probably give me a challenge!"; break;
				case 69: str = "What's the difference between you and a tree? A tree could probably dodge me better!"; break;
				case 70: str = "When your god created you, did they forget to add a brain?"; break;
				case 71: str = "I met someone that fought as good as you! It was the tastiest chicken ever!"; break;
				case 72: str = "Would you like me to remove that curse? Oh my mistake, you were just born that way!"; break;
				case 73: str = "Wow, you are so fat that I guess anyone behind you are gaining cover for this fight!"; break;
				case 74: str = "You are maggot pie served from a dwarf's codpiece!"; break;
				case 75: str = "You are the feces that is created when shame eats too much stupidly!"; break;
				case 76: str = "You are the worst example of your kind that I've ever come across!"; break;
				case 77: str = "You call that an attack, I've seen dead kittens hit harder than that!"; break;
				case 78: str = "You do know I am just standing right here if you want to try and hit me!"; break;
				case 79: str = "You look like a scab on a troll's wart!"; break;
				case 80: str = "You look like the armpit of an unshaven bog hag!"; break;
				case 81: str = "You look like your mother, and, your mother looks like your father!"; break;
				case 82: str = "You would bore the legs off a village idiot!"; break;
				case 83: str = "Your breath would cause a manure elemental to run!"; break;
				case 84: str = "I might have to resurrect you after this so we can try again!"; break;
				case 85: str = "Your mother is so ugly, priest try to cast banish undead on her!"; break;
				case 86: str = "Your mother is so fat that making a joke here would detract from the seriousness of her condition!"; break;
				case 87: str = "Your mother takes up more ground than Lord British's castle!"; break;
				case 88: str = "Your mother was a kobold and your father smelled of elderberry!"; break;
				case 89: str = "Your mother was so stupid, zombies made her a dunce hat!"; break;
				case 90: str = "Your mother is so ugly, folks turn to stone just incase they might happen to catch a glimpse of her face!"; break;
				case 91: str = "Your ugly face makes a good argument against raising the dead!"; break;
				case 92: str = "Your very existence is an insult to all!"; break;
				case 93: str = "You're going to make an excellent belt!"; break;
				case 94: str = "You're like a dragon, only not as strong or feirce...or anything!"; break;
				case 95: str = "You're like a gnome on stilts, real cute, but it's not working!"; break;
				case 96: str = "You're like a trained ape, only, without the training!"; break;
				case 97: str = "You're lucky to be born beautiful, unlike me, who was born to be a big liar!"; break;
				case 98: str = "You're not a complete idiot...Some parts are obviously missing!"; break;
				case 99: str = "You're so stupid, if a mind flayer tried to eat your brain, it would starve to death!"; break;
				case 100: str = "You're the reason baby gnomes cry!"; break;
			}

			return str;
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Owner;
			private DateTime m_Expire;
			private double m_Time;
			private int m_Loss;

			public InternalTimer( Mobile owner, int loss, int time ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.5 ) )
			{
				m_Time = (double)time;
				m_Loss = loss;
				m_Owner = owner;
				m_Expire = DateTime.Now + TimeSpan.FromSeconds( m_Time );

				BuffInfo.RemoveBuff( owner, BuffIcon.Insult );
				BuffInfo.AddBuff( owner, new BuffInfo( BuffIcon.Insult, 1063518, TimeSpan.FromSeconds( m_Time ), owner ) );
			}

			protected override void OnTick()
			{
				if ( !m_Owner.CheckAlive() || DateTime.Now >= m_Expire )
				{
					Stop();
					m_Table.Remove( m_Owner );
					m_Owner.SendMessage( "The insult is wearing off." );
				}
				else if ( m_Owner.Mana < m_Loss )
				{
					m_Owner.Mana = 0;
				}
				else
				{
					m_Owner.Mana -= m_Loss;
				}
			}
		}

		private class InternalTarget : Target
		{
			private Insult m_Owner;

			public InternalTarget( Insult owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}
