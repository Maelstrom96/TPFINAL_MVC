using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrDragon.Models.Game
{
    public class Entity
    {
        public String Name { get; set; }
        public OrDragon.Models.Map.Noeud Noeud { get; set; }

        public Entity(string name, OrDragon.Models.Map.Noeud noeud )
        {
            Name = name;
            Noeud = noeud;
        }
    }
}