using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroRipper
{
    public class Hero
    {

        private List<string> roles = new List<string>();

        public Hero()
        {

        }

        public string Name { get; set; }

        public string PrimaryAttribute { get; set; }

        public string AttackRange { get; set; }

        public List<string> Roles
        {
            set { roles = value; }
            get { return roles; }
        }

    }
}
