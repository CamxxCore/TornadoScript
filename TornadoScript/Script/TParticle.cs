using System;
using GTA;
using GTA.Native;
using GTA.Math;

namespace TornadoScript.Script
{
    public sealed class TParticle : Entity
    {
        /// <summary>
        /// Eliptical radius width.
        /// </summary>
        public float RadiusA { get; set; } = 10.0f;

        /// <summary>
        /// Eliptical radius length.
        /// </summary>
        public float RadiusB { get; set; } = 10.0f;

        /// <summary>
        /// Particle rotation speed.
        /// </summary>
        public float Speed { get; set; } = 1.5f;

        private Quaternion rotation;

        private GameSound wind, swoosh;

        private LoopedPTFX fx;

        private float zCoord, angle;

        private static int Setup(Vector3 position)
        {
            Model model = new Model("prop_beachball_02");

            if (!model.IsLoaded) model.Request(1000);

            Prop prop = World.CreateProp(model, position, false, false);

            Function.Call(Hash.SET_ENTITY_COLLISION, prop.Handle, 0, 0);

            prop.IsVisible = false;

            return prop.Handle;
        }

        public TParticle(Vector3 position, Vector3 rotation) : base(Setup(position))
        {
            this.zCoord = position.Z;
            this.rotation = Util.Euler(rotation);
            //this.wind = new GameSound("Helicopter_Wind", "BASEJUMPS_SOUNDS");
            this.swoosh = new GameSound("Woosh_01", "FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");
            //wind.Play(this);
            swoosh.Play(this);
            SetPTFX("core", "ent_amb_smoke_foundry");
        }

        public TParticle(Vector3 position) : this(position, new Vector3())
        { }

        public void SetPTFX(string assetName, string fxName)
        {
            fx?.Remove();

            fx = new LoopedPTFX(assetName, fxName);

            if (!fx.IsLoaded) fx.Load();

            fx.Start(this, 1.0f);

            fx.Colour = System.Drawing.Color.Red;

            fx.SetEvolution("LOD", 10000f);

            fx.SetEvolution("smoke", -10f);
        }

        public void SetScale(float scale)
        {
            fx.Scale = scale;
        }

        public void Rotate(Vector3 vec)
        {
            rotation = Util.Euler(vec);
        }

        public void RemoveFX()
        {
            fx.Remove();
            swoosh.Destroy();
            //wind.Destroy();
        }

        public void Update(Vector3 position)
        {
            position.Z = zCoord;

            fx.SetEvolution("debris", 1.0f);

            angle += Speed * Function.Call<float>(Hash.GET_FRAME_TIME);

            Position = position + Util.MultiplyVector(new Vector3(RadiusA * (float)Math.Cos(angle), RadiusB * (float)Math.Sin(angle), 0), rotation);
        }
    }
}