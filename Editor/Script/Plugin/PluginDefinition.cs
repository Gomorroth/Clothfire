using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nadena.dev.ndmf;

namespace gomoru.su.clothfire
{
    internal sealed class PluginDefinition : Plugin<PluginDefinition>
    {
        public override string DisplayName => "Clothfire";
        public override string QualifiedName => "gomoru.su.clothfire";

        protected override void Configure()
        {

        }
    }
}
