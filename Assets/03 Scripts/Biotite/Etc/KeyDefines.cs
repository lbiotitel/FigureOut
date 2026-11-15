using UnityEngine;

public class KeyDefines : MonoBehaviour
{

    public static class ResourceKey
    {
        public const string PLAYER_DATA = "Player Data";

        public const string MELEE_WEAPON_PREFAB = "Melee Weapon Prefab";
        public const string RANGED_WEAPON_PREFAB = "Ranged Weapon Prefab";

        public const string DROP_ITEM_PREFAB = "Drop Item Prefab";

        public const string DAMAGE_TEXT_PREFAB = "Damage Text Prefab";

        public const string INVENTORY_SLOT_PREFAB = "Inventory Slot UI Prefab";
    }

    public static class ParticleKey
    {
        public const string DEAD_PARTICLE_PREFAB = "Dead Particle Prefab";
        public const string Freeze_PARTICLE_PREFAB = "Freeze Particle Prefab";
        public const string Ignite_PARTICLE_PREFAB = "Ignite Particle Prefab";
        public const string Irradiation_PARTICLE_PREFAB = "Irradiation Particle Prefab";
        public const string Corrosion_PARTICLE_PREFAB = "Corrosion Particle Prefab";
        public const string Marked_PARTICLE_PREFAB = "Marked Particle Prefab";
        public const string Injury_PARTICLE_PREFAB = "Injury Particle Prefab";
    }


    public class MaskKey
    {
        public const string PLAYER_MASK = "Player";
        public const string ENEMY_MASK = "Enemy";
    }


    public static class SoundKey
    {
        public const string GAME_BGM = "Game BGM";

        public const string ReloadSfx = "Reload";
        public const string HitSfx = "Hit Sfx";
        public const string DeadSfx = "Dead SFX";
        public const string EquipSfx = "Equip SFX";
        public const string EvolutionSfx = "Evolution Sfx";
        public const string AttributionSfx = "Attribution Sfx";
        public const string RepairSfx = "Repair Sfx";
        public const string QuestAcceptSfx = "Quest Accept Sfx";
        public const string QuestCompleteSfx = "Quest Complete Sfx";

        public const string StepSfx1 = "Step Sfx 1";
        public const string StepSfx2 = "Step Sfx 2";
    }
    public static class TagKey
    {
        public const string DROP_ITEM_TAG = "DropItem";
        public const string PLAYER = "Player";
        public const string ENEMY = "Enemy";
    }
}
