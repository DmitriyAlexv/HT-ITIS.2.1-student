using System.ComponentModel.DataAnnotations;

namespace LogicServer.Models
{
    public class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HitPoints { get; set; }
        public int AttackModifier { get; set; }
        public int AttackPerRound { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public int DamageModifier { get; set; }
        public int Weapon { get; set; }
        public int ArmorClass { get; set; }

        public static Entity Clone(Entity entity)
        {
            return new Entity()
            {
                Id = entity.Id,
                Name = entity.Name,
                HitPoints = entity.HitPoints,
                AttackModifier = entity.AttackModifier,
                AttackPerRound = entity.AttackPerRound,
                MinDamage = entity.MinDamage,
                MaxDamage = entity.MaxDamage,
                DamageModifier = entity.DamageModifier,
                Weapon = entity.Weapon,
                ArmorClass = entity.ArmorClass
            };
        }
    }
}
