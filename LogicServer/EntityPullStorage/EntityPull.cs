using LogicServer.Models;

namespace LogicServer.EntityPullStorage
{
    public static class EntityPull
    {
        public static List<Entity> Entities { get; } = new() 
        { 
            new Entity()
            {
                Id = 1,
                Name = "Hellwasp",
                HitPoints = 52,
                AttackModifier = 18,
                AttackPerRound = 1,
                MinDamage = 1,
                MaxDamage = 8,
                DamageModifier = 4,
                Weapon = 0,
                ArmorClass = 19
            },
            new Entity()
            {
                Id = 2,
                Name = "Griffon",
                HitPoints = 59,
                AttackModifier = 18,
                AttackPerRound = 2,
                MinDamage = 2,
                MaxDamage = 6,
                DamageModifier = 4,
                Weapon = 0,
                ArmorClass = 12
            },
            new Entity()
            {
                Id = 3,
                Name = "Vampirate",
                HitPoints = 42,
                AttackModifier = 12,
                AttackPerRound = 1,
                MinDamage = 2,
                MaxDamage = 8,
                DamageModifier = 1,
                Weapon = 0,
                ArmorClass = 14
            }
        };
    }
}
