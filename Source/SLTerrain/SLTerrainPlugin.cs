using System;
using FlaxEngine;

namespace SLTerrain
{
    /// <summary>
    /// The sample game plugin.
    /// </summary>
    /// <seealso cref="FlaxEngine.GamePlugin" />
    public class SLTerrain : GamePlugin
    {
        /// <inheritdoc />
        public SLTerrain()
        {
            _description = new PluginDescription
            {
                Name = "SLTerrain",
                Category = "Terrain",
                Author = "",
                AuthorUrl = null,
                HomepageUrl = null,
                RepositoryUrl = "https://github.com/nothingTVatYT/SLTerrain",
                Description = "This is a port of Sebastian Lague's terrain code.",
                Version = new Version(1, 0, 0),
                IsAlpha = false,
                IsBeta = false,
            };
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            Debug.Log("Hello from plugin code!");
        }

        /// <inheritdoc />
        public override void Deinitialize()
        {
            // Use it to cleanup data

            base.Deinitialize();
        }
    }
}
