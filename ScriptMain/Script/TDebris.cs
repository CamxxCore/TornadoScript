using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using TornadoScript.ScriptCore.Game;
using TornadoScript.ScriptMain.Utility;

namespace TornadoScript.ScriptMain.Script
{
    /// <summary>
    ///  Ttesting physical tornado debris
    /// </summary>
    class TDebris : ScriptProp
    {
        private float _radius;

        private int _spawnTime = 0;

        public TornadoVortex Parent { get; set; }

        enum MaterialGroup
        {
            urban,
            desert,
            countryside,
            dirt
        }

        private static readonly List<string> debrisItems = new List<string>
        {
            "prop_bush_med_02",
            "prop_fncwood_16d",
            "prop_fncwood_16e",
            "prop_railsleepers01"
        };

       /* readonly Dictionary<MaterialGroup, List<string>> materialDebrisMap = new Dictionary<MaterialGroup, List<string>>
        {
            { MaterialGroup.countryside, // countryside debris objects
                new List<string> {
                    "prop_bush_med_02",
                    "prop_bush_med_05",
                    "prop_fncwood_16d",
                    "prop_fncwood_16e",
                    "prop_fnclog_02b",
                    "prop_railsleepers01", }
            },

             { MaterialGroup.desert, // desert debris objects
                new List<string> {
                    "prop_joshua_tree_01d",
                    "prop_bush_med_02",
                                             }
            },

             { MaterialGroup.urban, // debris objects found in urban areas (downtown etc.)
                new List<string> {
                    "prop_wallbrick_01",
                    "prop_fncwood_16d",
                    "prop_fncwood_16e",
                    "prop_railsleepers01",
                    "ng_proc_food_burg02a",
                    "ng_proc_sodacub_03a",
                    "prop_fire_hydrant_1",
                    "prop_fnclink_03c",
                    "prop_dumpster_02a",
                    "prop_dumpster_01a",
                    "ng_proc_block_02a",
                    "ng_proc_brick_01a",
                    "prop_bin_01a",
                    "prop_postbox_01a",
                    "prop_cablespool_02",
                    "prop_barrier_work06a",
                    "prop_roadcone02a",
                    "prop_sign_road_03g",
                    "prop_lawnmower_01",
                    "prop_table_04",
                    "prop_chair_01a",
                    "prop_chair_01b",
                    "prop_table_03_chr",
                    "prop_rub_binbag_04",
                    "prop_rub_binbag_05",
                    "prop_sacktruck_02a" }
            }
        };*/

        public TDebris(TornadoVortex vortex, Vector3 position, float radius)
            : base(Setup(position))
        {
            Parent = vortex;
            _radius = radius;
            _spawnTime = Game.GameTime;
            PostSetup();
        }

        private void PostSetup()
        {
            var centerPos = Parent.Position + new Vector3(0, 0, 5.0f);

            var angle = Probability.GetFloat(0.0f, (float)Math.PI * 2.0f);

            Ref.Position = centerPos + new Vector3(_radius * (float)Math.Cos(angle), _radius * (float)Math.Sin(angle), 0);
        }

        /// <summary>
        /// Setup the base entity.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private static Prop Setup(Vector3 position)
        {
            var model = new Model(debrisItems[Probability.GetInteger(0, debrisItems.Count - 1)]);

            if (!model.IsLoaded) model.Request(1000);

            var prop = World.CreateProp(model, position, false, false);

            prop.LodDistance = 1000;

            return prop;
        }

        public override void OnUpdate(int gameTime)
        {
            base.OnUpdate(gameTime);

            if (gameTime - _spawnTime > 6400)
            {
                Dispose();
            }
        }
    }
}
