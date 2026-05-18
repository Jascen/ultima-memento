# Mannequin Deed System — Implementation Plan

## Summary

A carpenter-crafted **mannequin deed** that, when used inside a house owned/co-owned by the placer, spawns an invulnerable display mannequin (a plain `Mobile` subclass, not `BaseCreature`). The house owner and any co-owner (including same-account alts on AOS) can:

- Double-click the mannequin to open its backpack (storage; counts toward house secure storage).
- Open the mannequin's paperdoll via context menu and drag-equip / unequip cosmetic gear.
- Pack the mannequin back into a deed (only when fully empty).

Non-managers can only view its paperdoll. The mannequin is invulnerable, frozen, untargetable for harm, and tied at runtime to the house it stands in (no stored owner mobile/account).

## Scope

**v1 (this PR)**

- `Mannequin` mobile + `MannequinBackpack`
- `MannequinDeed` + placement target
- `BaseHouse` integration: `Mannequins` list, `KillMannequins`, secure-storage tally
- Carpenter craft recipe + `ItemSales` carpenter stock
- Drag-equip via paperdoll + view-only paperdoll for non-managers
- Pack-up (returns a deed; blocked unless worn + backpack empty)
- Frozen + invulnerable + cannot be moved out

**v2 (follow-up — not in v1)**

- Race-change gump (cosmetic body only)
- Swap-gear button (full worn-item exchange between player and mannequin)
- Optional `PlayerVendorCustomizeGump` integration

Rationale for phasing: swap-gear has many edge cases (layer conflicts, modded/blessed items, monster body equip rejection); race needs careful handling to avoid `BaseRace.CreateRace` side effects; `PlayerVendorCustomizeGump` is tightly typed to `PlayerVendor` and would require refactoring. None of them block v1 utility.

## Verified codebase facts

| Claim | Verified |
|------|----------|
| `BaseHouse.IsCoOwner` includes `IsOwner` (`BaseHouse.cs:3248–3257`), and `IsOwner` includes same-account check on AOS (`BaseHouse.cs:3235–3246`) via `CheckAccount`. | ✅ |
| `VendorBackpack.CheckHold` (`PlayerVendor.cs:101–120`) and `GetAosCurSecures` (`BaseHouse.cs:283–330`) both gate vendor secure-storage on `!NewVendorSystem` — so on AOS, vendor backpacks do **not** count. Mannequins need explicit support. | ✅ |
| `Mobile.CanBeDamaged()` returns `!m_Blessed` (`Mobile.cs:5254`), so `Blessed = true` already prevents damage. Override is belt-and-suspenders. | ✅ |
| `BaseHouse.KillVendors()` (`BaseHouse.cs:183–194`) is called from `Decay_Sandbox` and `OnAfterDelete` — vendors self-register in `PlayerVendor` constructor via `House = house`. **Mannequins must register the same way**, or they'll be orphaned on decay/demolish. | ✅ |
| `CharacterStatue.cs:607–617` exposes **static** `CouldFit` and `CheckDoors` helpers — reusable. But `CouldFit` calls `BaseAddon.CheckHouse` which only verifies `IsOwner`, not `IsCoOwner`. The mannequin placement target must do its own `IsCoOwner` check (post-`CanFit`/`CheckDoors`) instead of relying on `CheckHouse`. | ✅ |
| `BaseRace.CreateRace` (`BaseRace.cs:878–947`) applies gameplay side effects (`BodyMod`, `RaceID`, sounds, `SetMonsterMagic` stat mods) and is gated on `m is PlayerMobile`. Must **not** be called for mannequin — clone body cosmetically (mirroring `CharacterStatue.CloneBody` at `CharacterStatue.cs:304–310`). | ✅ |
| `Scripts.csproj` is SDK-style — new `.cs` files auto-included. | ✅ |
| No existing `Mannequin` / `Manakin` / `Manequin` / `DisplayDummy` class — name is safe. | ✅ |

## Corrections from earlier draft

1. **`PlayerBarkeeper.IsOwner` analogy dropped** — it has no same-account logic. The real precedent is `PlayerVendor.IsOwner` → `House.IsOwner` (AOS path).
2. **File paths corrected** — `Mobiles/Misc/` does not exist. Use `Mobiles/Base/Mannequin.cs` (next to `PlayerVendor.cs`) and `Items/Misc/MannequinDeed.cs` (next to `PlayerVendorDeed.cs`).
3. **`PlayerVendorCustomizeGump` reuse removed** from v1 — it is tightly coupled to `PlayerVendor` type.
4. **Swap gear deferred to v2** — significant edge-case complexity.
5. **`PlayerVendorDeed`'s `!house.Public` rejection deliberately not copied** — mannequins must work in private houses.

## v1 design (final)

### `Mannequin` mobile — `World/Source/Scripts/Mobiles/Base/Mannequin.cs`

State:
- Constructor `Mannequin( BaseHouse house )` — sets `Body = 0x190`, `Name = "a mannequin"`, `Frozen = true`, `Blessed = true`, `CantWalk = true`, `InitStats(100,100,100)`, creates `MannequinBackpack`, registers with `house.Mannequins.Add(this)`, stores `m_House`.
- Serialized fields: `BaseHouse m_House`, `bool m_Female`. (No owner mobile/account.)

Permission helper:
```csharp
public bool CanManage( Mobile from )
{
    if ( from == null || from.Deleted || Deleted ) return false;
    if ( from.AccessLevel >= AccessLevel.GameMaster ) return true;
    BaseHouse house = BaseHouse.FindHouseAt( this );
    return house != null && house.IsCoOwner( from );
}
```

Behavior overrides:

| Method | Behavior |
|--------|----------|
| `OnDoubleClick` | `CanManage` → `OpenBackpack(from)`; else → `DisplayPaperdollTo(from)` |
| `AllowEquipFrom(Mobile)` | `CanManage(mob)` |
| `CheckNonlocalLift` / `CheckNonlocalDrop` | `CanManage(from)` |
| `AllowItemUse(Item)` | `false` (no using items on mannequin) |
| `CanBeRenamedBy` | `false` |
| `CanBeDamaged` | `false` (belt-and-suspenders over `Blessed`) |
| `OnDamage` | no-op (don't call base) |
| `CanPaperdollBeOpenedBy` | `true` |
| `OnLocationChange` / `OnMapChange` | If new location not in any house, revert (or just rely on `Frozen = true`; staff bypass acceptable) |
| `GetContextMenuEntries` | If `CanManage`: add `ManageMannequinEntry` (opens paperdoll) + `PackUpMannequinEntry`. Always add base entries. |
| `OnAfterDelete` | If `m_House != null`, `m_House.Mannequins.Remove(this)` |
| `GetProperties` | Add "(mannequin)" suffix |

Pack-up:
```csharp
public bool IsEmptyForPackup()
{
    if ( Backpack != null && Backpack.Items.Count > 0 ) return false;
    foreach ( Item item in this.Items )
        if ( item.Layer != Layer.Backpack ) return false;
    return true;
}

public void PackUp( Mobile from )
{
    if ( !CanManage( from ) ) return;
    if ( !IsEmptyForPackup() )
    {
        from.SendMessage( "The mannequin must be empty before you can pack it up." );
        return;
    }
    MannequinDeed deed = new MannequinDeed();
    if ( !from.AddToBackpack( deed ) )
    {
        deed.Delete();
        from.SendMessage( "Your backpack is full." );
        return;
    }
    Delete();
}
```

### `MannequinBackpack : Backpack` (same file)

- `Layer = Layer.Backpack`, `Movable = false`.
- Override `CheckHold` — enforce house AOS storage limit when `Parent is Mannequin` (mirror `VendorBackpack` but without the `NewVendorSystem` gate, so it always counts).

### `MannequinDeed` — `World/Source/Scripts/Items/Misc/MannequinDeed.cs`

- `Item` subclass; `ItemID = 0x14F0`; `Weight = 1.0`; `Hue` ~ neutral.
- `[Constructable] MannequinDeed()` — sets `Name = "a mannequin deed"` (or via `LabelNumber`).
- `OnDoubleClick`: require `IsChildOf(from.Backpack)`; `from.Target = new MannequinPlacementTarget(this);`.

`MannequinPlacementTarget : Target`:
- `OnTarget(from, targeted)`:
  1. `IPoint3D p = targeted as IPoint3D`; if null → fail message.
  2. `SpellHelper.GetSurfaceTop(ref p)`.
  3. `Point3D loc = new Point3D(p)`.
  4. `BaseHouse house = null;`
  5. `AddonFitResult result = CharacterStatue.CouldFit(loc, map, from, ref house);` — handles tile fit, in-house, doors.
  6. If `result != Valid`, send matching localized message (500269/1076192/500271).
  7. **Additional check**: `if ( house == null || !house.IsCoOwner( from ) ) → fail`. (Because `CouldFit` uses `BaseAddon.CheckHouse` which only validates `IsOwner`; we want co-owners too.)
  8. Create `Mannequin m = new Mannequin( house );`
  9. `m.MoveToWorld( loc, from.Map );`
  10. `m_Deed.Delete();`

### `BaseHouse` integration — `World/Source/Scripts/Items/Houses/BaseHouse.cs`

1. Add field + property:
   ```csharp
   private ArrayList m_Mannequins = new ArrayList();
   public ArrayList Mannequins { get { return m_Mannequins; } }
   ```
2. Extend `GetAosCurSecures` — add a `fromMannequins` accumulator (out param not needed; fold into existing total). Iterate `Mannequins`:
   ```csharp
   int fromMannequins = 0;
   foreach ( Mannequin man in Mannequins )
       if ( man.Backpack != null )
           fromMannequins += man.Backpack.TotalItems;
   ```
   Add to return value.
3. Extend `KillVendors`: after barkeeper loop, add:
   ```csharp
   list = new ArrayList( Mannequins );
   foreach ( Mannequin m in list )
       m.Delete();
   ```
   (Mannequin contents are tied to house storage; on decay/demolish, the house is going away, so deleting matches existing barkeeper behavior. v2 could add Destroy-to-bank handling.)
4. **Serialization**: persist `m_Mannequins` (write count + each mobile, mirror existing serialization patterns for `m_PlayerVendors`). Look at how `m_PlayerVendors` is (de)serialized in `BaseHouse.Serialize`/`Deserialize` and mirror it bump the version number.

### Carpenter craft — `DefCarpentry.cs`

Add near Dressform recipe (~line 296):
```csharp
index = AddCraft( typeof( MannequinDeed ), 1044294, "mannequin deed", 75.0, 100.0, typeof( Board ), 1015101, 50, 1044351 );
AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
AddRes( index, typeof( Fabric ), 1044286, 30, 1044287 );
```
(Adjust skill caps if shard balance dictates.)

### Carpenter vendor stock — `ItemSales.cs`

One row in the carpenter list (near other deeds):
```csharp
new ItemSalesInfo( typeof( MannequinDeed ), 175, 1, 87, false, false, World.None, Category.None, Material.None, Market.Carpenter ),
```

## Implementation order

1. Write `Mannequin.cs` (mobile + `MannequinBackpack`) — compiles standalone.
2. Write `MannequinDeed.cs` — depends on `Mannequin`.
3. Patch `BaseHouse.cs` — `Mannequins` list, serialization version bump, `KillVendors` extension, `GetAosCurSecures` tally.
4. Patch `DefCarpentry.cs` — craft recipe.
5. Patch `ItemSales.cs` — carpenter stock.
6. Build (`dotnet build` on the World solution if available; otherwise rely on `Scripts.csproj`).
7. Commit.

## Manual test checklist (post-merge)

1. Place deed inside owned house → mannequin spawns, deed consumed.
2. Place deed inside co-owner's house → succeeds.
3. Place deed in **private** house (`Public == false`) → succeeds (key difference vs PlayerVendorDeed).
4. Place deed outside house / blocked tile / next to door → fails with proper message.
5. Non-manager double-click → view-only paperdoll.
6. Manager double-click → backpack opens.
7. Manager context menu → paperdoll opens; can drag-equip items.
8. Same-account alt of house owner can manage (AOS only).
9. Try to attack/spell the mannequin → invulnerable.
10. Fill backpack past house secure limit → blocked with localized message (1061839).
11. Pack-up with items still on/in mannequin → blocked.
12. Pack-up when fully empty → deed returned, mannequin deleted.
13. Buy `MannequinDeed` from carpenter; craft one from carpentry menu.
14. Demolish/decay house → `KillVendors` deletes mannequin without crashing.
15. Save world / restart → mannequins persist with backpack contents; `BaseHouse.Mannequins` rebuilt.
