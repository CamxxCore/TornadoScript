using System;
using GTA;
using GTA.Native;
using GTA.Math;
using ScriptCore;

namespace TornadoScript.Script
{
    /// <summary>
    /// Represents a particle in the tornado.
    /// </summary>
    public sealed class TParticle : ScriptProp
    {
        public int LayerIndex { get { return layer; } }

        public TVortex Parent
        {
            get { return parent; }

            set { parent = value; }
        }

        private TVortex parent;

        private Vector3 centerPos;

        private Quaternion rotation;

        private LoopedParticle ptfx;

        private float angle, xRadius, yRadius, layerMask;

        private int layer;

        /// <summary>
        /// Instantiate the class.
        /// </summary>
        /// <param name="vortex"></param>
        /// <param name="fxAsset"></param>
        /// <param name="fxName"></param>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <param name="radiusX"></param>
        /// <param name="radiusY"></param>
        /// <param name="layerIdx"></param>
        public TParticle(TVortex vortex, Vector3 position, Vector3 angle, string fxAsset, string fxName, float radiusX, float radiusY, int layerIdx) 
            : base(Setup(position))
        {   
            parent = vortex;      
            centerPos = position;
            rotation = MathEx.Euler(angle);
            ptfx = new LoopedParticle(fxAsset, fxName);
            xRadius = radiusX;
            yRadius = radiusY;
            layer = layerIdx;
            PostSetup();
        }

        private void PostSetup()
        {
            layerMask = 1.0f - (layer / parent.MaxLayers);

            layerMask *= (0.1f * layer);

            layerMask = 1.0f - layerMask;

            if (layerMask < 0.20f)
                layerMask = 0.20f;
        }

        /// <summary>
        /// Setup the base entity.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private static Prop Setup(Vector3 position)
        {
            Model model = new Model("prop_beachball_02");

            if (!model.IsLoaded) model.Request(1000);

            Prop prop = World.CreateProp(model, position, false, false);

            Function.Call(Hash.SET_ENTITY_COLLISION, prop.Handle, 0, 0);

            prop.IsVisible = false;

            return prop;
        }

        /// <summary>
        /// Set the center position that the particle should rotate around.
        /// </summary>
        /// <param name="center"></param>
        public void SetPosition(Vector3 center)
        {
            centerPos = center;
        }

        /// <summary>
        /// Set the particle scale.
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(float scale)
        {
            ptfx.Scale = scale;
        }

        public override void OnUpdate(int gameTime)
        {
            if (parent == null)
            {
                Dispose();
            }

            else
            {
                centerPos = parent.Position + new Vector3(0, 0, parent.LayerSize * layer);

                Ref.Position = centerPos + MathEx.MultiplyVector(new Vector3(xRadius * (float)Math.Cos(angle), yRadius * (float)Math.Sin(angle), 0), rotation);

                angle -= (parent.Speed * layerMask) * Function.Call<float>(Hash.GET_FRAME_TIME);

               /* if (angle > Math.PI * 2.0f)
                {
                    angle = 0.0f;
                }*/
            }

            base.OnUpdate(gameTime);
        }

        public void StartFX(float scale)
        {
            if (!ptfx.IsLoaded)
            {
                ptfx.Load();
            }

            ptfx.Start(this, scale);

            ptfx.Colour = System.Drawing.Color.Black;
        }

        public void RemoveFX()
        {
            ptfx.Remove();
        }

        public override void Dispose()
        {
            RemoveFX();

            base.Dispose();
        }
    }
}