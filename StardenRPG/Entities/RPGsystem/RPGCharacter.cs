using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardenRPG.Entities.RPGsystem
{
    public enum Element
    {
        Earth,
        Water,
        Air,
        Fire,
        Plant
    }

    public class RPGCharacter
    {
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Stamina { get; set; }
        public Element ElementalType { get; set; }

        public RPGCharacter(string name, int maxHealth, int stamina, Element element)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Stamina = stamina;
            ElementalType = element;
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }
        }

        public void RestoreHealth(int health)
        {
            CurrentHealth += health;
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
        }

        public void UseStamina(int amount)
        {
            Stamina -= amount;
            if (Stamina < 0)
            {
                Stamina = 0;
            }
        }

        public void RestoreStamina(int amount)
        {
            Stamina += amount;
            // ... And so on for other RPG mechanics
        }
    }

}
