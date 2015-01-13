using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACKS_CB
{
    class item
    {
        //Blasted SP and CP are making me make the costs a string.
        public string name;
        public string cost;

        public item()
        {
            name = "";
            cost = "0";
        }

        public item(string inputName, string inputCost)
        {
            name = inputName;
            cost = inputCost;
        }

        public double getCost()
        {
            if (cost.Contains("gp"))
                return System.Convert.ToDouble(cost.Substring(0, cost.IndexOf("gp")));
            else if (cost.Contains("sp"))
                return System.Convert.ToDouble(cost.Substring(0, cost.IndexOf("sp"))) / 10;
            else if (cost.Contains("cp"))
                return System.Convert.ToDouble(cost.Substring(0, cost.IndexOf("cp"))) / 100;
            else return 0;
        }
    }

    class weapon
    {
        //A weapon's type is one-handed, two-handed, or versatile
        //A versatile weapon can be equipped in two hands for increased damage
        public string name;
        public string range;
        public int cost;
        public string damage;

        public weapon()
        {
            name = "";
            range = "";
            cost = 0;
            damage = "";
        }

        //Name, Cost, Damage, Range
        public weapon(string inputName, int inputCost, string inputDamage, string inputRange)
        {
            name = inputName;
            cost = inputCost;
            damage = inputDamage;
            range = inputRange;
            
            
        }
    }

    class armor
    {
        //Armor has a name, AC value, and cost
        public string name;
        public int ac;
        public int cost;

        public armor()
        {
            name = "";
            ac = 0;
            cost = 0;
        }

        public armor(string inputName, int inputAC, int inputCost)
        {
            name = inputName;
            ac = inputAC;
            cost = inputCost;
        }
    }
}
