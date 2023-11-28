﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(gomoru.su.clothfire.ndmf.PluginDefinition))]

namespace gomoru.su.clothfire.ndmf
{
    internal sealed class PluginDefinition : Plugin<PluginDefinition>
    {
        public override string DisplayName => "Clothfire";
        public override string QualifiedName => "gomoru.su.clothfire";

        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming)
                .BeforePlugin("nadena.dev.modular-avatar")
                .Run(new InitializePass()).Then
                .Run(new GeneratePass()).Then
                .Run(new GenerateAdditionalControlPass()).Then
                .Run(new FinalizePass());
        }
    }
}